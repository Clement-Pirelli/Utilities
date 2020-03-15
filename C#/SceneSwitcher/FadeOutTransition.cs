using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class FadeOutTransition : MonoBehaviour
{
    Image image;
    public float fadeOutTime = .0f;
    public string nextSceneName;


    float timer = .0f;

    public void OnStart()
    {
        image = GetComponent<Image>();
        image.color = new Color(.0f, .0f, .0f, .0f);
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("NEXT SCENE NAME IS EMPTY!");
            Destroy(gameObject);
            return;
        }
        image.rectTransform.localScale = new Vector3(10000.0f, 10000.0f, 1.0f);
        Button[] buttons = Object.FindObjectsOfType<Button>();
        foreach(Button b in buttons) { b.interactable = false; }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        image.color = new Color(.0f, .0f, .0f, timer/ fadeOutTime);
        if (timer >= fadeOutTime)
        {
            EventManager.BroadcastEvent(SceneSwitcher.sceneChangeTag, nextSceneName);
        }
    }
}
