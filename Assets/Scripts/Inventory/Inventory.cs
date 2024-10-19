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
    private InputAction switchItem;

    [SerializeField]
    private GameObject hands;

    private int itemIndex = 0;

    public Vector3 itemPosition = new Vector3(0.686f, -0.876f, 0.064f);

    [SerializeField]
    private GameObject gunPrefab;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private GameObject flashlightPrefab;

    private GameObject activeItem;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        useAction = playerControls.FindActionMap("UseItem").FindAction("Use");
        altUseAction = playerControls.FindActionMap("UseItem").FindAction("Alt use");
        switchItem = playerControls.FindActionMap("UseItem").FindAction("Switch");
    }

    private void Start()
    {

        Flashlight light = new Flashlight("Flashlight", flashlightPrefab, hands.transform, itemPosition);
        AddItem(light);

        Gun gun = new Gun("AK", gunPrefab, hands.transform, itemPosition, bulletPrefab, 25); // gun has 25 bullets
        AddItem(gun);
    }

    private void OnEnable()
    {
        useAction.Enable();
        altUseAction.Enable();
        switchItem.Enable();

        useAction.started += ctx => UseItem();
        altUseAction.started += ctx => AltUseItem();
        switchItem.performed += ctx => OnSwitch(ctx.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        useAction.Disable();
        altUseAction.Disable();
        switchItem.Disable();

        useAction.started -= ctx => UseItem();
        altUseAction.started -= ctx => AltUseItem();

        switchItem.performed -= ctx => OnSwitch(ctx.ReadValue<Vector2>());
    }

    private void Update()
    {
        HandleInput();
        HandleUsing();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PrintInventory();
        }
        if (Input.GetKeyDown(KeyCode.H)) items[itemIndex].Upgrade();
    }

    private void HandleUsing()
    {
        items[itemIndex].Active(Time.deltaTime);
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
            if (items.Count == 1) items[itemIndex].ActivateItem();
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

    private void OnSwitch(Vector2 change)
    {
        int dir = change.y > 0 ? 1 : -1;
        SwitchItem(dir);
    }

    private void SwitchItem(int direction)
    {
        if(items.Count == 0)
        {
            Debug.Log("No items in inventory");
            return;
        }

        items[itemIndex].DeactivateItem();
        

        itemIndex = (itemIndex + direction) % items.Count;

        if (itemIndex < 0) itemIndex += items.Count;


        Item newItem = items[itemIndex];
        Debug.Log("Switched to " + newItem.GetItemName() + " as active.");

        newItem.ActivateItem();
    }
}
