using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeButtonObject : InteractableObject
{
    public string action = "activate button";

    private bool isActive = false;

    public override void Interact()
    {
        // light up the button
        if (isActive) return;

        isActive = true;

        GetComponentInChildren<Light>().enabled = true;

        gameObject.layer = 0;

        GetComponent<PressMazeButton>().AddButtonToList();

        enabled = false;
    }

    public void ResetButton()
    {
        isActive = false;

        GetComponentInChildren<Light>().enabled = false;
    }

    public override void Use()
    {
        Debug.Log("Maze button has no use");
    }

    public override string GetTooltip() => action;
}
