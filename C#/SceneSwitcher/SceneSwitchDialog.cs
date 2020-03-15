using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

public class SceneSwitchDialog : MonoBehaviour
{
    [HideInInspector] public string sceneName;
    [SerializeField] Button okButton;
    [SerializeField] Button cancelButton;

    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        //set the parent of this object as being the current canvas of the scene
        transform.SetParent(canvas.transform);
        //set this object as being rendered on "top" of the rest
        transform.SetAsLastSibling();

        //set the rectTransform to be centered
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(.0f, .0f);
        rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        setButtonsInteractable(false);
        Time.timeScale = .0f;
    }


    void setButtonsInteractable(bool interactable)
    {
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button button in buttons)
        {
            button.interactable = interactable;
        }

        okButton.interactable = true;
        cancelButton.interactable = true;
    }

    public void OnOK()
    {
        Time.timeScale = 1.0f;
        setButtonsInteractable(true);
        EventManager.BroadcastEvent(SceneSwitcher.sceneChangeTag, sceneName);
    }

    public void OnCancel()
    {
        Time.timeScale = 1.0f;
        setButtonsInteractable(true);
        Destroy(gameObject);
    }

    //prevents timescale not being set back and locking timers indefinitely
    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }
}
