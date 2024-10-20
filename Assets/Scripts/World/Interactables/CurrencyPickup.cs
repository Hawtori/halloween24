using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyPickup : InteractableObject
{
    // create game object, have collider, have this script, set layer to interactable, done

    public int currency;
    private string action;

    private void Start()
    {
        currency = Random.Range(5, 11);
        action = "acquire " + currency + " Tinkets.";
    }

    public override void Interact()
    {
        Inventory.Instance.AddCurrency(currency);
        Destroy(gameObject);
    }

    public override void Use()
    {
        Debug.Log("Tinket has no use function");
    }


    public override string GetTooltip() => action;
}
