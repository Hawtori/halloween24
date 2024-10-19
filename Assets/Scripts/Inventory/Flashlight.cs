using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Flashlight : Item
{
    private GameObject lightPrefab;

    // true = on, flase = off
    private bool state = false;

    private float batteryDrain = 0;
    private float drainRate = 1f;

    public Flashlight(string name, GameObject prefab, Transform parent, Vector3 position, int batteryPercent = 100) : base(name, prefab, parent, position, batteryPercent)
    {
        lightPrefab = prefab;
    }

    public override void AltUseItem()
    {
        Debug.Log($"Flashlight is at {quantity} percent");
    }

    public override void UseItem()
    {
        if (quantity <= 0)
        {
            state = true;
        }

        state = !state;

        itemInstance.transform.GetChild(0).gameObject.SetActive(state);
        Debug.Log("Flashlight turning " + (state ? "on" : "off"));
    }

    public override void Active(float dt)
    {
        if (!state || quantity <= 0) return;
        batteryDrain += dt * drainRate;
        quantity -= (int)batteryDrain;
        batteryDrain %= 1;

        if (quantity <= 0) UseItem();
    }

    public override void Upgrade()
    {
        drainRate /= 1.75f;
    }

    public GameObject GetInstance() => itemInstance;
    public GameObject GetObject() => lightPrefab;
}
