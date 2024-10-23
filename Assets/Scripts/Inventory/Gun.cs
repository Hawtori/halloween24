using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

[System.Serializable]
public class Gun : Item
{
    public int ammoCapacity;
    public int ammoCount;

    private GameObject gunPrefab;
    private GameObject shotDecal;

    private int damage;

    private float fireRate = 0.5f;
    private float lastShotTimer = 0f;

    
    public Gun(string name, GameObject prefab, Transform parent, Vector3 position, GameObject decal, int quantity = 1, float fireRate = 0.75f, int damage = 1) : base(name, prefab, parent, position, quantity)
    {
        gunPrefab = prefab;
        ammoCapacity = quantity;
        ammoCount = ammoCapacity;
        this.fireRate = fireRate;
        this.damage = damage;
        this.shotDecal = decal;
    }

    public override void UseItem()
    {
        if (lastShotTimer < fireRate || ammoCount == 0)
        {
            Debug.Log("Can not shoot yet!");
            return;
        }

        lastShotTimer = 0f;

        ammoCount--;
        ParticleSystem[] systems = itemInstance.transform.GetComponentsInChildren<ParticleSystem>();

        systems[0].Play(); // muzzle flash

        // shoot a bullet
        // raycast from the middle of screen forwards
        // if it hits something:
        // if its enemy, deal damage, knock back, stun
        // if its wall, spawn
        Vector3 pos = Camera.main.transform.position;
        Vector3 dir = Camera.main.transform.forward;
        LayerMask mask = (1 << 7) | (1 << 12);

        RaycastHit hit;
        bool landed = Physics.Raycast(pos, dir, out hit, 75f, mask);
        if (landed)
        {
            if (hit.transform.gameObject.layer != 7)
            {
                SpawnBulletHit(hit);
            }
            else
            {
                hit.transform.GetComponent<EnemyBase>().TakeDamage(damage, transform.position);
            }
        }

        BulletEffect(systems[1], (landed ? hit.point : dir * 10f));


        Debug.Log("Shot a bullet, current ammo count " + ammoCount);
    }

    private void BulletEffect(ParticleSystem system, Vector3 point)
    {

        Quaternion rotToPoint = Quaternion.LookRotation((point - transform.position).normalized, Vector3.up);

        system.transform.rotation = rotToPoint;
        system.Play();
    }

    private void SpawnBulletHit(RaycastHit hit)
    {
        Quaternion rot = Quaternion.LookRotation(hit.normal, Vector3.up) * Quaternion.Euler(90, 0, 0);
        GameObject.Instantiate(shotDecal, hit.point + hit.normal * 0.01f, rot);
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
