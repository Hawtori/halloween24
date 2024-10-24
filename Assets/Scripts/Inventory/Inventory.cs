using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private List<Item> items = new List<Item>();
    private InteractableObject objectInHand;
    private int extraAmmo = 0, currency = 0;
    private int amountOfKeys = 3;

    [SerializeField]
    private InputActionAsset playerControls;

    private InputAction useAction;
    private InputAction altUseAction;
    private InputAction switchItem;
    private InputAction itemAction;

    [SerializeField]
    private GameObject hands;

    private int itemIndex = 0;

    public Vector3 itemPosition = new Vector3(0.686f, -0.876f, 0.064f);
    public Vector3 interactPosition;

    [SerializeField]
    private GameObject gunPrefab;
    [SerializeField]
    private GameObject decalPrefab;
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
        itemAction = playerControls.FindActionMap("Interact").FindAction("Interact");
    }

    private void Start()
    {

        Flashlight light = new Flashlight("Flashlight", flashlightPrefab, hands.transform, itemPosition);
        AddItem(light);

        Gun gun = new Gun("AK", gunPrefab, hands.transform, itemPosition, decalPrefab, 13); ; // gun has 13 bullets
        AddItem(gun);
    }

    private void OnEnable()
    {
        useAction.Enable();
        altUseAction.Enable();
        switchItem.Enable();
        itemAction.Enable();

        useAction.started += ctx => UseItem();
        altUseAction.started += ctx => AltUseItem();
        switchItem.performed += ctx => OnSwitch(ctx.ReadValue<Vector2>());
        itemAction.started += ctx => UseInteractable();
    }

    private void OnDisable()
    {
        useAction.Disable();
        altUseAction.Disable();
        switchItem.Disable();
        itemAction.Disable();

        useAction.started -= ctx => UseItem();
        altUseAction.started -= ctx => AltUseItem();

        switchItem.performed -= ctx => OnSwitch(ctx.ReadValue<Vector2>());
        itemAction.started -= ctx => UseInteractable();
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
        if(!objectInHand)
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

    public void AddAmmo(int amount)
    {
        extraAmmo += amount;

        Debug.Log($"We have {extraAmmo} ammo in inventory.");
    }

    // called when we reload
    public int UseAmmo(int amount)
    {
        // we have enough ammo for full
        if(extraAmmo >= amount) {
            extraAmmo -= amount;
            return amount;
        }

        // we have less than requested
        int res = extraAmmo;
        extraAmmo = 0;

        // return how much ammo we used
        return res;
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        Debug.Log($"We have {currency} currency in inventory.");
    }

    // called when we are shopping and wish to buy something
    public bool UseCurrency(int amount)
    {
        // we have enough money
        if(currency >= amount)
        {
            currency -= amount;
            return true;
        }

        // we have less than required
        return false;
    }

    // picking up an interactable
    public bool AddItem(InteractableObject item)
    {
        items[itemIndex].DeactivateItem();

        // holding something
        if(objectInHand && objectInHand != item)
        {
            return false;
        }

        objectInHand = item;
        objectInHand.transform.parent = hands.transform;
        objectInHand.transform.localPosition = interactPosition;
        objectInHand.transform.localScale /= 2f;

        return true;
    }
    
    // drop interactable
    public void RemoveItem(InteractableObject item)
    {
        if(objectInHand == item)
        {
            items[itemIndex].ActivateItem();
            objectInHand = null;
        }
    }

    // use the interactable
    private void UseInteractable()
    {
        if (!objectInHand) return;

        objectInHand.transform.localScale *= 2f;
        objectInHand.Use();
    }

    public void PuzzleSolved()
    {
        amountOfKeys++;
    }

    public bool OpenDoor()
    {
        if(amountOfKeys > 0)
        {
            amountOfKeys--;
            return true;
        }
        return false;
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

        if(objectInHand)
        {
            Debug.Log("Holding an object");
            return;
        }


        Item newItem = items[itemIndex];

        newItem.ActivateItem();
    }
}
