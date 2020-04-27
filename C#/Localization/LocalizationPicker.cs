using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalizationPicker : MonoBehaviour
{
    public static string localizationPrefKey = "PickedLocalization";
    [SerializeField] GameObject[] buttons = null;
    
    public enum LANGUAGE
    {
        ENGLISH = 1,
        SWEDISH = 2
    }

    bool transition = false;

    void Start()
    {
        foreach(GameObject b in buttons) b.SetActive(false);
        //get the language from the player prefs
        int language = PlayerPrefs.GetInt(localizationPrefKey);
        //if the language has been set, it won't be 0 (default value)
        if (language != 0)
        {
            //if the language has already been set, just pick that one
            OnLanguagePick(language);
        } else
        {
            //otherwise, the user will choose from the buttons on the canvas
            foreach (GameObject b in buttons) b.SetActive(true);
            transition = true;
        }
    }

    public void OnLanguagePick(int language)
    {
        //the buttons, if they were active, aren't needed anymore : the user has already chosen
        foreach (GameObject b in buttons) b.SetActive(false);

        //set the playerpref - that way the player won't have to select the same language every time
        PlayerPrefs.SetInt(localizationPrefKey, language);

        //set the localization manager's data to be from the right file for each language
        switch ((LANGUAGE)language)
        {
            case LANGUAGE.ENGLISH:
                LocalizationManager.LoadLocalisedText("localization_english.json");
                break;
            case LANGUAGE.SWEDISH:
                LocalizationManager.LoadLocalisedText("localization_swedish.json");
                break;
        }
    }
}
