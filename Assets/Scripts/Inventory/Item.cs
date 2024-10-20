using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public abstract class Item
{
    protected string itemName;
    protected int quantity;
    protected GameObject itemInstance;

    public Item(string itemName, GameObject prefab, Transform parent, Vector3 position, int quantity = 1)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        itemInstance = Object.Instantiate(prefab, parent);
        itemInstance.transform.localPosition = position;

        DeactivateItem();
    }


    public abstract void UseItem();
    public abstract void AltUseItem();

    public abstract void Active(float dt);

    // could change it so there are tiers
    public abstract void Upgrade();

    public virtual void Upgrade(int tier)
    {

    }

    public virtual void ActivateItem()
    {
        if(itemInstance) itemInstance.SetActive(true);
    }

    public virtual void DeactivateItem()
    {
        if(itemInstance) itemInstance.SetActive(false);
    }

    public virtual int GetItemCount() => quantity;
    public virtual string GetItemName() => itemName;
    public virtual void IncreaseCount(int count) => quantity += count;

}



//[System.Serializable]
//public class Ammo : Item
//{
//    public int damage;

//    public Ammo(string name, int quantity, int damage) : base(name, quantity)
//    {
//        this.itemName = name;
//        this.quantity = quantity;
//        this.damage = damage;
//    }

//    // TODO implement bullet logic
//    public override void UseItem()
//    {
//        //GameObject b = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), transform.position, Quaternion.identity);
//        // b.AddComponent<SphereCollider>();
//        // b.AddComponent<Rigidbody>();

//        // b.GetComponent<Rigidbody>().AddForce(50f * Camera.main.transform.forward, ForceMode.Impulse);
//        // b.AddComponent<TrailRenderer>();
//        Debug.Log("Ammo doesn't have usage");

//    }

//    public override void AltUseItem()
//    {
//        Debug.Log("Ammo doesn't have alt usage");
//    }

//}