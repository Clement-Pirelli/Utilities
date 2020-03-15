using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
    //key which will be looked up in the LocalizationManager
    [SerializeField] string key;

    //string which will be added after the value text - useful for numbers, rich text tags
    string suffix = "";

    //string which will be added before the value text - useful for rich text tags
    string prefix = "";

    Text attachedText;
    EventSubscriber localizationChangedSubscriber;

    // Start is called before the first frame update
    void Start()
    {
        attachedText = GetComponent<Text>();
        OnLocalizationChanged(null);
        localizationChangedSubscriber = new EventSubscriber(LocalizationManager.ChangedEventTag, OnLocalizationChanged);
    }

    public void SetKey(string newKey)
    {
        key = newKey;
        OnLocalizationChanged(null);
    }

    public void SetSuffix(string newSuffix)
    {
        suffix = newSuffix;
        OnLocalizationChanged(null);
    }

    public void SetPrefix(string newPrefix)
    {
        prefix = newPrefix;
        OnLocalizationChanged(null);
    }

    private void OnLocalizationChanged(object o)
    {
        attachedText.text = System.Text.RegularExpressions.Regex.Unescape(prefix + LocalizationManager.getValue(key) + suffix);
    }
}
