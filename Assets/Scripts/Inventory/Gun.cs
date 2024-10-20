using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun : Item
{
    public int ammoCapacity;
    public int ammoCount;

    private GameObject gunPrefab;
    private GameObject bulletPrefab;

    private float fireRate = 0.5f;
    private float lastShotTimer = 0f;
    
    public Gun(string name, GameObject prefab, Transform parent, Vector3 position, GameObject bulletPrefab, int quantity = 1, float fireRate = 0.5f) : base(name, prefab, parent, position, quantity)
    {
        //ammoCount = new List<Ammo>(ammoCapacity);
        gunPrefab = prefab;
        ammoCapacity = quantity;
        ammoCount = ammoCapacity;
        this.fireRate = fireRate;

        this.bulletPrefab = bulletPrefab;
    }

    // TODO implemnent shooting logic and change to hitscan
    public override void UseItem()
    {
        if (lastShotTimer < fireRate || ammoCount == 0)
        {
            Debug.Log("Can not shoot yet!");
            return;
        }

        lastShotTimer = 0f;

        ammoCount--;
        // shoot a bullet
        if(bulletPrefab)
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, itemInstance.transform.GetChild(0).position, Quaternion.identity);
        }

        Debug.Log("Shot a bullet, current ammo count " + ammoCount);
    }

    public override void AltUseItem()
    {
        int reloadAmmo = Inventory.Instance.UseAmmo(ammoCapacity - ammoCount);
        ammoCount += reloadAmmo;

        Debug.Log("Reloading gun, current ammo count " + ammoCount);
    }

    // update function
    public override void Active(float dt)
    {
        lastShotTimer += dt;
    }

    public override void Upgrade()
    {
        fireRate -= 0.23f;
        ammoCapacity += 15;
        ammoCount = ammoCapacity;
    }

    public override void Upgrade(int tier)
    {
        if (tier == 0) Upgrade();
        else
        {
            // give flashlight
        }
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

    public GameObject GetInstance() => itemInstance;
    public GameObject GetObject() => gunPrefab;
}
