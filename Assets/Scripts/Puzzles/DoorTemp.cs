using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTemp : InteractableObject
{
    public string action = "use a key";

    public GameObject doorCloseEffect;
    public List<GameObject> itemsToDrop = new List<GameObject>();

    public float rotationRate = 5f;
    private bool isOpening = false;
    float angle = 0;

    public bool deleteSelf = false;

    private void Start()
    {
        if (deleteSelf) Destroy(this);
    }

    private void Update()
    {
        if(isOpening)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        angle += Time.deltaTime * rotationRate;
        angle = Mathf.Clamp(angle, 0, 90);
        Quaternion rot = Quaternion.Euler(-angle, 0, 0);
        transform.rotation = rot;
        if(angle == 90)
        {
            isOpening = false;
            angle = 0;
        }
    }

    private void UnlockDoor()
    {
        if (doorCloseEffect)
            Instantiate(doorCloseEffect, transform.position, Quaternion.identity);
        else Debug.Log("Door closing has no effect attached");

        if(itemsToDrop.Count == 0)
        {
            Debug.Log("No droppable items");
            return;
        }

        int maxItems = Random.Range(0, itemsToDrop.Count);
        for(int i = 0; i < maxItems; i++)
        {
            Instantiate(itemsToDrop[Random.Range(0, itemsToDrop.Count)], transform.position + Vector3.up * 5f, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public override void Interact()
    {
        if (Inventory.Instance.OpenDoor())
        {
            UnlockDoor();
        }
    }

    public override void Use()
    {
        Debug.Log("Door has no usage");
    }

    public override string GetTooltip() => action;
}
