using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public abstract void Interact();
    public abstract void Use();
    public abstract string GetTooltip();
}
