using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalizationEditor : EditorWindow
{
    public LocalizationData data;
    bool saved = true;
    Vector2 scrollView;

    [MenuItem("Window/Localization Editor")]
    static void Init()
    {
        EditorWindow.GetWindow<LocalizationEditor>().Show();
    }

    private void OnGUI()
    {
        scrollView = EditorGUILayout.BeginScrollView(scrollView);
        if(data != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("data");
            SerializedProperty keyValuePairProperty = serializedProperty.FindPropertyRelative("keyValuePairs");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(keyValuePairProperty, true);
            if (EditorGUI.EndChangeCheck())
            {
                saved = false;
            }
            serializedObject.ApplyModifiedProperties();
        }
        EditorGUILayout.EndScrollView();


        if(data != null)
        {
            if(GUILayout.Button("Add Pair"))
            {
                data.keyValuePairs.Add(new KeyValuePair());
            }

            GUILayout.Space(20.0f);


            if (filePath != null && GUILayout.Button("Save File"))
            {
                SaveDataToSetPath();
            }

            if (GUILayout.Button("Save File As..."))
            {
                SaveData();
            }
        } else 
        {
            GUILayout.Space(20.0f);
        }
        
        if(GUILayout.Button("Load File"))
        {
            LoadData();
        }

        if (GUILayout.Button("New File"))
        {
            NewData();
        }
    }

    string filePath = null;
    private void LoadData()
    {
        if (!SaveCheck()) return;
        filePath = EditorUtility.OpenFilePanel("Select localisation data file", Application.streamingAssetsPath, "json");
        if (!string.IsNullOrEmpty(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<LocalizationData>(jsonData);
            saved = true;
        }
    }

    private void SaveData()
    {
        //opens a save file panel and saves user-inputted path
        filePath = EditorUtility.SaveFilePanel("Save localisation data file path", Application.streamingAssetsPath, "", "json");
        //if the path is valid
        if (!string.IsNullOrEmpty(filePath))
        {
            //serialize pairs
            string jsonData = JsonUtility.ToJson(data);
            //write
            File.WriteAllText(filePath,jsonData);
            saved = true;
        }

    }

    private void SaveDataToSetPath()
    {
        //serialize pairs
        string jsonData = JsonUtility.ToJson(data);
        //write
        File.WriteAllText(filePath, jsonData);
        saved = true;
    }

    private void NewData()
    {
        if (SaveCheck()) data = new LocalizationData();
    }
    

    private bool SaveCheck()
    {
        if (saved) return true;
        return EditorUtility.DisplayDialog("Unsaved changes!", "You have unsaved changes. Are you sure you wish to proceed?", "Yea", "Nah fam");
    }
}
