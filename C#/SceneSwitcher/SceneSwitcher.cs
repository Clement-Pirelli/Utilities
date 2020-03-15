using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public static string LastSceneName;

    public enum SCENE_SWITCH_TYPE
    {
        INSTANT,
        DIALOG,
        TRANSITION
    }

    public static readonly string sceneChangeTag = "SceneChange";

    void Start()
    {
        EventManager.SubscribeToEvent(sceneChangeTag, SceneChange);
    }

    private void OnDestroy()
    {
        EventManager.UnsubscribeToEvent(sceneChangeTag, SceneChange);
    }
    
    //necessary for button callbacks
    public void SceneChangeDialog(string s)
    {
        SceneChange(s, SCENE_SWITCH_TYPE.DIALOG);
    }

    public void SceneChangeTransition(string s)
    {
        SceneChange(s, SCENE_SWITCH_TYPE.TRANSITION);
    }

    public void SceneChange(string s)
    {
        LastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(s);
    }

    public void SceneChange(string s, SCENE_SWITCH_TYPE type)
    {

        switch (type)
        {
            case SCENE_SWITCH_TYPE.INSTANT:
                SceneChange(s);
                break;
            case SCENE_SWITCH_TYPE.DIALOG:
                GameObject dialogObject = GameObject.Instantiate(Resources.Load<GameObject>("SceneSwitchDialog"));
                dialogObject.GetComponent<SceneSwitchDialog>().sceneName = s;
                break;
            case SCENE_SWITCH_TYPE.TRANSITION:
                LastSceneName = SceneManager.GetActiveScene().name;
                //spawn a transition object
                Transform canvasTransform = Object.FindObjectOfType<Canvas>().gameObject.transform;
                GameObject transitionObject = new GameObject();
                transitionObject.transform.SetParent(canvasTransform);
                transitionObject.AddComponent<Image>();
                FadeOutTransition fadeOut = transitionObject.AddComponent<FadeOutTransition>();
                fadeOut.nextSceneName = s;
                fadeOut.fadeOutTime = 1.0f;
                fadeOut.OnStart();
                break;
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void SceneChange(object obj)
    {
        SceneChange((string)obj);
    }
}
