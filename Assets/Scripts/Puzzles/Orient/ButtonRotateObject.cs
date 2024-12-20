using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonRotateObject : InteractableObject
{
    public int direction;
    public string action;

    public override void Interact()
    {
        OrientShape shape = transform.parent.parent.GetComponentInChildren<OrientShape>();
        if (shape != null)
        shape.RotateShape(direction);
    }

    public override void Use()
    {
        Debug.Log("Button has no use function");

    }

    public override string GetTooltip() => action;

}
