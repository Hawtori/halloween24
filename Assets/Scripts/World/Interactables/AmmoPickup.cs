using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : InteractableObject
{
    // create game object, have collider, have this script, set layer to interactable, done

    public int ammo;
    private string action;

    private void Start()
    {
        ammo = Random.Range(10, 15);
        action = "acquire " + ammo + " ammo.";
    }

    public override void Interact()
    {
        Inventory.Instance.AddAmmo(ammo);
        Destroy(gameObject);
    }

    public override void Use()
    {
        Debug.Log("Ammo has no use function");
    }


    public override string GetTooltip() => action;

}
