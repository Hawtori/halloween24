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
    private float lastShotTime = 0f;
    
    public Gun(string name, GameObject prefab, Transform parent, Vector3 position, GameObject bulletPrefab, int quantity = 1, float fireRate = 0.5f) : base(name, prefab, parent, position, quantity)
    {
        //ammoCount = new List<Ammo>(ammoCapacity);
        gunPrefab = prefab;
        ammoCapacity = quantity;
        ammoCount = ammoCapacity;
        this.fireRate = fireRate;

        this.bulletPrefab = bulletPrefab;
    }

    // TODO implemnent shooting logic
    public override void UseItem()
    {
        if (Time.time - lastShotTime < fireRate || ammoCount == 0)
        {
            Debug.Log("No ammo left");
            return;
        }

        lastShotTime = Time.time;

        ammoCount--;
        // shoot a bullet
        if(bulletPrefab)
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, itemInstance.transform.GetChild(0).position, Quaternion.identity);
        }

        Debug.Log("Shot a bullet");
    }

    public override void AltUseItem()
    {
        Debug.Log("Reloading gun");
    }

    public override void Active(float dt)
    {

    }

    public override void Upgrade()
    {
        fireRate -= 0.1f;
        ammoCapacity += 15;
        ammoCount = ammoCapacity;
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
