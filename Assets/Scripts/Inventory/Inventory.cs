using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private List<Item> items = new List<Item>();

    [SerializeField]
    private InputActionAsset playerControls;

    private InputAction useAction;
    private InputAction altUseAction;

    private int itemIndex = 0;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        useAction = playerControls.FindActionMap("UseItem").FindAction("Use");

        altUseAction = playerControls.FindActionMap("UseItem").FindAction("Alt use");
    }

    private void Start()
    {
        Gun gun = new Gun("AK", 25); // gun has 25 bullets
        AddItem(gun);

    }

    private void OnEnable()
    {
        useAction.Enable();
        altUseAction.Enable();

        useAction.started += ctx => UseItem();
        altUseAction.started += ctx => AltUseItem();
    }

    private void OnDisable()
    {
        useAction.Disable();
        altUseAction.Disable();

        useAction.started -= ctx => UseItem();
        altUseAction.started -= ctx => AltUseItem();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PrintInventory();
        }
    }

    private void PrintInventory()
    {
        string itemstring = "";
        foreach (var item in items)
        {
            itemstring += item.GetItemName() + " ";
        }
        Debug.Log("Items in inventory: " + itemstring);
    }

    private void UseItem()
    {
        items[itemIndex].UseItem();
    }


    private void AltUseItem()
    {
        items[itemIndex].AltUseItem();
    }

    public void AddItem(Item item)
    {
        Item existingItem = items.Find(i => i.GetItemName() == item.GetItemName());
        if (existingItem != null)
        {
            existingItem.IncreaseCount(item.GetItemCount());
        }
        else
        {
            items.Add(item);
        }
    }

    public void RemoveItem(Item item)
    {
        Item existingItem = items.Find(i => i.GetItemName() == item.GetItemName());
        if (existingItem != null)
        {
            existingItem.IncreaseCount(-item.GetItemCount());
            if (existingItem.GetItemCount() <= 0)
            {
                items.Remove(existingItem);
            }
        }
    }
}
