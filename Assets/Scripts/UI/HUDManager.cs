using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    [SerializeField]
    private GameObject textContainer;
    [SerializeField]
    private TMP_Text promptText;

    [SerializeField]
    private TMP_Text itemText;


    private string currentText = "";
    private bool isShowingSomething = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void UpdateItem(string text)
    {
        itemText.text = text;
    }

    public void UpdateText(string text)
    {
        if(currentText == text) return;

        isShowingSomething = true;

        textContainer.SetActive(true);
        Invoke(nameof(HideText), 1.25f);

        currentText = text;
        promptText.text = text;
    }

    public void UpdateTextPrompt(string text)
    {
        if (currentText == text || isShowingSomething) return;

        textContainer.SetActive(true);

        currentText = text;
        promptText.text = "Press E to " + text;
    }

    public void HidePrompt()
    {
        if (isShowingSomething) return;
        textContainer.SetActive(false);
        currentText = "";
        isShowingSomething = false;
    }

    private void HideText()
    {
        textContainer.SetActive(false);
        currentText = "";
        isShowingSomething = false;
    }
}
