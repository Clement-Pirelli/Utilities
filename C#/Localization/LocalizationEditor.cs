using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalizationEditor : EditorWindow
{
    
    static string LastPath { 
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

    string filePath = null;
    string filter = "";

    bool sPressed = false;

    public LocalizationData data;

    bool saved = true;

    Vector2 scrollView;


    #region Menu items

    [MenuItem("Localization Editor/Merge")]
    static void MergeFiles() {
        LocalizationData data1, data2;

        string path = GetFilePathFromUser("Select first localisation data file");
        
        if (!string.IsNullOrEmpty(path))
        {
            string jsonData = File.ReadAllText(path);
            data1 = JsonUtility.FromJson<LocalizationData>(jsonData);
            LastPath = path;
        }
        else { return; }

        if(data1 == null || data1.keyValuePairs == null || data1.keyValuePairs.Count == 0) 
        {
            OnError("Couldn't read data from provided file! Please provide a valid file");
        }

        string path2 = GetFilePathFromUser("Select second localisation data file");

        if (!string.IsNullOrEmpty(path))
        {
            string jsonData = File.ReadAllText(path);
            data2 = JsonUtility.FromJson<LocalizationData>(jsonData);
            LastPath = path2;
        }
        else { return; }

        if (data2 == null || data2.keyValuePairs == null || data2.keyValuePairs.Count == 0)
        {
            OnError("Couldn't read data from provided file! Please provide a valid file");
        }

        System.Threading.Tasks.Task<bool> mergeTask = new System.Threading.Tasks.Task<bool>(
            () =>
            {
                data1.keyValuePairs.ForEach((pair) =>
                {
                    if (!data2.keyValuePairs.Exists((p) => p.key == pair.key))
                    {
                        data2.keyValuePairs.Add(pair);
                    }
                });

                data2.keyValuePairs.ForEach((pair) =>
                {
                    if (!data1.keyValuePairs.Exists((p) => p.key == pair.key))
                    {
                        data1.keyValuePairs.Add(pair);
                    }
                });

                SaveDataToSetPath(path, data1);
                SaveDataToSetPath(path2, data2);

                return true;
            }
            );

        mergeTask.Start();
        float i = 0;
        while (!mergeTask.IsCompleted) 
        {
            i++;
            if (i < 100) i = 100;
            EditorUtility.DisplayProgressBar("Merging data...", "Please stand by", i);
        }

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Operation successful!", "Your data has been merged successfully", "OK");
    }

    [MenuItem("Localization Editor/Edit")]
    static void StartEdit() => EditorWindow.GetWindow<LocalizationEditor>("Localization Editor").Show();

    #endregion

    private void OnGUI()
    {
        HandleInput();
        
        if(data != null)
        {
            DisplayDataFields();
        } else 
        {
            GUILayout.Space(20.0f);
            EditorGUILayout.Separator();
        }
        
        if(GUILayout.Button("Load File")) LoadData();
        if (GUILayout.Button("New File")) NewData();
    }

    private void DisplayDataFields()
    {
        filter = EditorGUILayout.TextField("Search", filter, GUILayout.ExpandWidth(true));

        GUILayout.Space(20);
        EditorGUILayout.LabelField(new GUIContent { text = "<b>Data : </b>" }, new GUIStyle { fontSize = 20, richText = true});
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
                if(EditorGUILayout.PropertyField(keyValuePairProp))
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(keyProp);
                    EditorGUILayout.PropertyField(valueProp, GUILayout.MinHeight(100.0f));

                    if (EditorGUI.EndChangeCheck())
                    {
                        saved = false;
                    }

                }

                DrawDivider();
                
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
            SaveDataToSetPath(filePath, data);
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
        filePath = GetFilePathFromUser();
        if (!string.IsNullOrEmpty(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<LocalizationData>(jsonData);
            saved = true;
            LastPath = filePath;
        }
    }

    private void SortData() => data.keyValuePairs.Sort(new KeyValuePairComparer());

    private void SaveData()
    {
        //opens a save file panel and saves user-inputted path
        filePath = GetFilePathFromUser();
        //if the path is valid
        if (!string.IsNullOrEmpty(filePath))
        {
            //serialize pairs
            string jsonData = JsonUtility.ToJson(data);
            //write
            File.WriteAllText(filePath, jsonData);
            saved = true;
            LastPath = filePath;
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

    private static void SaveDataToSetPath(string path, LocalizationData data)
    {
        //serialize pairs
        string jsonData = JsonUtility.ToJson(data);
        //write
        File.WriteAllText(path, jsonData);
    }
    private static string GetFilePathFromUser(string message = "Select localisation data file")
    {
        return EditorUtility.OpenFilePanel(message, LastPath, "json");
    }

    private static void OnError(string errorMessage)
    {
        EditorUtility.DisplayDialog(errorMessage, "", "OK");
    }

    private static void DrawDivider() 
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }

    class KeyValuePairComparer : IComparer<KeyValuePair>
    {
        public int Compare(KeyValuePair x, KeyValuePair y) => string.Compare(x.key, y.key, System.StringComparison.CurrentCultureIgnoreCase);
    }

    #endregion
}
