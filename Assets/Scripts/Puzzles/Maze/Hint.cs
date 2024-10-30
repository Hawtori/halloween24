using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hint : MonoBehaviour
{
    public static Hint Instance;

    public TMP_Text textBox;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void SetHintText(string text)
    {
        textBox.text = text;
    }
}
