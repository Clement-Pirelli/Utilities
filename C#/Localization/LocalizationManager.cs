using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class KeyValuePair
{
    public string key;
    public string value;
}

[System.Serializable]
public class LocalizationData
{
    public List<KeyValuePair> keyValuePairs = new List<KeyValuePair>();
}

public static class LocalizationManager
{
    private const string NoValueString = "LOCALIZATION NOT FOUND";
    public const string ChangedEventTag = "LChange";
    private static Dictionary<string, string> localisedStrings = new Dictionary<string, string>();
    public static bool Ready { get; private set; } = false;

    public static void LoadLocalisedText(string filename)
    {
        Ready = false;
        localisedStrings = new Dictionary<string, string>();
        string path = Path.Combine(Application.streamingAssetsPath, filename);
        bool set = false;

        LocalizationData data = IOUtility.LoadFromJSON<LocalizationData>(path, out set);
        if (set)
        {
            foreach (KeyValuePair pair in data.keyValuePairs)
            {
                localisedStrings.Add(pair.key, pair.value);
            }
        } else
        {
            Debug.LogError("Could not load file at path : " + path);
        }
        

        Ready = true;
        EventManager.BroadcastEvent(ChangedEventTag);
    }

    /// <summary>
    /// Returns a localized value mapped to a key
    /// </summary>
    /// <param name="key">key which the manager will lookup to find the value</param>
    /// <returns>Returns the value if the key exists. Otherwise, returns a placeholder string</returns>
    public static string getValue(string key)
    {
        string result = NoValueString;
        if (localisedStrings.ContainsKey(key))
        {
            result = localisedStrings[key];
        }
        return result;
    }
}

