using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public abstract class Item
{
    protected string itemName;
    protected int quantity;

    public Item(string itemName, int quantity = 1)
    {
        this.itemName = itemName;
        this.quantity = quantity;
    }

    public abstract void UseItem();
    public abstract void AltUseItem();
    public virtual int GetItemCount() => quantity;
    public virtual string GetItemName() => itemName;
    public virtual void IncreaseCount(int count) => quantity += count;

}

[System.Serializable]
public class Gun : Item
{
    public int ammoCapacity;
    public int ammoCount;

    public Gun(string name, int ammoCapacity) : base(name, ammoCapacity)
    {
        //ammoCount = new List<Ammo>(ammoCapacity);
        this.ammoCapacity = quantity;
        ammoCount = ammoCapacity;
    }

    // TODO implemnent shooting logic
    public override void UseItem()
    {
        if(ammoCount == 0)
        {
            Debug.Log("No ammo left");
            return;
        }

        //ammoCount[ammoCount.Count - 1].UseItem();
        //ammoCount.Remove(ammoCount[ammoCount.Count - 1]);

        ammoCount--;
        // shoot a bullet


        Debug.Log("Shot a bullet");
    }

    public override void AltUseItem()
    {
        Debug.Log("No alt action for gun");
    }

    public override void IncreaseCount(int count)
    {
        ammoCount = Mathf.Min(ammoCount + count, ammoCapacity);
    }

    // return how much ammo we have left
    public override int GetItemCount()
    {
        return ammoCount;
    }
}

[System.Serializable]
public class Ammo : Item
{
    public int damage;

    public Ammo(string name, int quantity, int damage) : base(name, quantity)
    {
        this.itemName = name;
        this.quantity = quantity;
        this.damage = damage;
    }

    // TODO implement bullet logic
    public override void UseItem()
    {
        //GameObject b = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), transform.position, Quaternion.identity);
        // b.AddComponent<SphereCollider>();
        // b.AddComponent<Rigidbody>();

        // b.GetComponent<Rigidbody>().AddForce(50f * Camera.main.transform.forward, ForceMode.Impulse);
        // b.AddComponent<TrailRenderer>();
        Debug.Log("Ammo doesn't have usage");

    }

    public override void AltUseItem()
    {
        Debug.Log("Ammo doesn't have alt usage");
    }

}

[System.Serializable]
public class FlashLight : Item
{

    public FlashLight(string name, int quantity) : base(name, quantity)
    {
    }

    public override void AltUseItem()
    {
        
    }

    public override void UseItem()
    {

    }
}