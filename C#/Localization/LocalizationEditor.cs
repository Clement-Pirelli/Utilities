using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalizationEditor : EditorWindow
{
    string filePath = null;
    string filter = "";

    bool sPressed = false;

    public LocalizationData data;

    bool saved = true;

    Vector2 scrollView;

    [MenuItem("Localization Editor/Edit")]
    static void StartEdit() => GetWindow<LocalizationEditor>("Localization Editor").Show();

    private void OnGUI()
    {
        HandleInput();

        if (data != null)
        {
            DisplayDataFields();
        }
        else
        {
            GUILayout.Space(20.0f);
            EditorGUILayout.Separator();
        }

        if (GUILayout.Button("Load File")) LoadData();
        if (GUILayout.Button("New File")) NewData();
    }

    private void DisplayDataFields()
    {
        filter = EditorGUILayout.TextField("Search", filter, GUILayout.ExpandWidth(true));

        GUILayout.Space(20);
        EditorGUILayout.LabelField(new GUIContent { text = "<b>Data : </b>" }, new GUIStyle { fontSize = 20, richText = true });
        GUILayout.Space(10);


        scrollView = EditorGUILayout.BeginScrollView(scrollView);
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty keyValuePairListProperty = serializedObject.FindProperty("data.keyValuePairs");

        for (int i = 0; i < keyValuePairListProperty.arraySize; i++)
        {
            var keyValuePairProp = keyValuePairListProperty.GetArrayElementAtIndex(i);
            var keyProp = keyValuePairProp.FindPropertyRelative("key");
            var valueProp = keyValuePairProp.FindPropertyRelative("value");
            if (filter == "" || keyProp.stringValue.StartsWith(filter))
            {
                if (EditorGUILayout.PropertyField(keyValuePairProp))
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(keyProp);
                    EditorGUILayout.PropertyField(valueProp, GUILayout.MinHeight(100.0f));

                    if (EditorGUI.EndChangeCheck())
                    {
                        saved = false;
                    }

                }

                LocalizationEditorUtils.DrawDivider();

                GUILayout.Space(10.0f);
            }
        }
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Separator();

        if (GUILayout.Button("Add Pair")) data.keyValuePairs.Add(new KeyValuePair());

        GUILayout.Space(20.0f);

        EditorGUI.BeginDisabledGroup(saved);
        if (filePath != null && (GUILayout.Button("Save File") || (EditorGUI.actionKey && sPressed)))
        {
            LocalizationEditorUtils.SaveDataToSetPath(filePath, data);
            saved = true;
            Repaint();
        }
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("Save File As...")) SaveData();

        if (GUILayout.Button("Sort")) SortData();
    }

    private void LoadData()
    {
        if (!SaveCheck()) return;
        filePath = LocalizationEditorUtils.GetFilePathFromUser();
        if (!string.IsNullOrEmpty(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<LocalizationData>(jsonData);
            saved = true;
            LocalizationEditorUtils.LastPath = filePath;
        }
    }

    private void SortData() => data.keyValuePairs.Sort(new KeyValuePairComparer());

    private void SaveData()
    {
        //opens a save file panel and saves user-inputted path
        filePath = LocalizationEditorUtils.GetFilePathFromUser();
        //if the path is valid
        if (!string.IsNullOrEmpty(filePath))
        {
            //serialize pairs
            string jsonData = JsonUtility.ToJson(data);
            //write
            File.WriteAllText(filePath, jsonData);
            saved = true;
            LocalizationEditorUtils.LastPath = filePath;
        }

    }

    private void NewData()
    {
        if (SaveCheck()) data = new LocalizationData();
    }


    #region UTILITIES

    void HandleInput()
    {
        Event e = Event.current;
        switch (e.type)
        {
            case EventType.KeyDown:
                if (Event.current.keyCode == KeyCode.S) sPressed = true;
                break;
            case EventType.KeyUp:
                if (Event.current.keyCode == KeyCode.S) sPressed = false;
                break;
        }

    }

    private bool SaveCheck()
    {
        if (saved) return true;
        return EditorUtility.DisplayDialog("Unsaved changes!", "You have unsaved changes. Are you sure you wish to proceed?", "Yea", "Nah fam");
    }



    #endregion
}

class LocalizationMerger : EditorWindow
{
    struct MergeItem
    {
        public LocalizationData data;
        public string path;
    }
    MergeItem first, second;
    bool merged = false;

    [MenuItem("Localization Editor/Merge")]
    static void StartMerge() => GetWindow<LocalizationMerger>("Localization Merger").Show();

    private void OnGUI()
    {
        DisplayMergeItem(ref first, "Select first data");
        DisplayMergeItem(ref second, "Select second data");

        LocalizationEditorUtils.DrawDivider();

        bool disabled = string.IsNullOrEmpty(first.path) || string.IsNullOrEmpty(second.path);

        EditorGUI.BeginDisabledGroup(disabled);

        if (GUILayout.Button("Merge")) 
        {
            MergeFiles();
        }

        if(disabled) EditorGUI.EndDisabledGroup();
    }

    private static void DisplayMergeItem(ref MergeItem item, string message) 
    {
        if (!string.IsNullOrEmpty(item.path))
        {
            GUILayout.Label(item.path);
        }

        if (GUILayout.Button(message)) 
        {
            var path = LocalizationEditorUtils.GetFilePathFromUser("Select a localisation data file");
            if (!string.IsNullOrEmpty(path))
            {
                string jsonData = File.ReadAllText(path);
                item.data = JsonUtility.FromJson<LocalizationData>(jsonData);
                LocalizationEditorUtils.LastPath = path;
                if (LocalizationEditorUtils.IsDataInvalid(item.data))
                {
                    LocalizationEditorUtils.OnError("Couldn't read data from provided file! Please provide a valid file");
                }
                else 
                {
                    item.path = path;
                    LocalizationEditorUtils.LastPath = path;
                }
            }
        }

        GUILayout.Space(50.0f);
    }

    void MergeFiles()
    {
        var firstDict = new Dictionary<string, string>();
        var secondDict = new Dictionary<string, string>();

        EditorUtility.DisplayProgressBar("Merging data...", "Please stand by", 0);

        foreach (var pair in first.data.keyValuePairs)
        {
            if (!firstDict.ContainsKey(pair.key)) firstDict.Add(pair.key, pair.value);
        }

        foreach (var pair in second.data.keyValuePairs)
        {
            if (!secondDict.ContainsKey(pair.key)) secondDict.Add(pair.key, pair.value);
        }

        foreach (var key in firstDict.Keys)
        {
            if (!secondDict.ContainsKey(key)) secondDict.Add(key, firstDict[key]);
        }

        EditorUtility.DisplayProgressBar("Merging data...", "Please stand by", 50);

        foreach (var key in secondDict.Keys)
        {
            if (!firstDict.ContainsKey(key)) firstDict.Add(key, secondDict[key]);
        }

        EditorUtility.DisplayProgressBar("Merging data...", "Please stand by", 100);

        first.data.keyValuePairs.Clear();
        second.data.keyValuePairs.Clear();

        foreach (var key in firstDict.Keys)
        {
            var a = new KeyValuePair();
            a.key = key;
            a.value = firstDict[key];
            first.data.keyValuePairs.Add(a);
        }

        LocalizationEditorUtils.SaveDataToSetPath(first.path, first.data);

        foreach (var key in secondDict.Keys)
        {
            var a = new KeyValuePair();
            a.key = key;
            a.value = secondDict[key];
            second.data.keyValuePairs.Add(a);
        }

        LocalizationEditorUtils.SaveDataToSetPath(second.path, second.data);

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Operation successful!", "Your data has been merged successfully", "OK");
    }
}





class LocalizationEditorUtils
{
    public static string LastPath
    {
        get
        {
            var p = PlayerPrefs.GetString("lastPath");
            return p == "" ? Application.streamingAssetsPath : p;
        }
        set
        {
            PlayerPrefs.SetString("lastPath", value);
        }
    }

    public static void SaveDataToSetPath(string path, LocalizationData data)
    {
        //serialize pairs
        string jsonData = JsonUtility.ToJson(data, true);
        //write
        File.WriteAllText(path, jsonData);
    }
    public static string GetFilePathFromUser(string message = "Select localisation data file")
    {
        return EditorUtility.OpenFilePanel(message, LastPath, "json");
    }

    public static void OnError(string errorMessage)
    {
        EditorUtility.DisplayDialog(errorMessage, "", "OK");
    }

    public static void DrawDivider()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, Color.gray);
    }

    public static bool IsDataInvalid(LocalizationData data) => (data == null || data.keyValuePairs == null || data.keyValuePairs.Count == 0);
}

class KeyValuePairComparer : IComparer<KeyValuePair>
{
    public int Compare(KeyValuePair x, KeyValuePair y) => string.Compare(x.key, y.key, System.StringComparison.CurrentCultureIgnoreCase);
}
