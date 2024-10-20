using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : InteractableObject
{
    private readonly string action = "pick up box";

    [SerializeField]
    private LayerMask boxFinal;
    [SerializeField]
    private float radius;

    public override void Interact()
    {
        if (!Inventory.Instance.AddItem(this)) return;
        
        Rigidbody rb;
        if (TryGetComponent<Rigidbody>(out rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

    }

    public override void Use()
    {
        transform.parent = null;
        Inventory.Instance.RemoveItem(this);


        Collider[] boxCollisions = Physics.OverlapSphere(transform.position, radius, boxFinal);
        if(boxCollisions.Length > 0)
        {
            transform.position = boxCollisions[0].transform.position;
            transform.rotation = boxCollisions[0].transform.rotation;

            Destroy(this); // remove this if we want to still be able to interact with object after placing it in position
            Destroy(boxCollisions[0]);
            return;
        }

        Rigidbody rb;
        if(TryGetComponent<Rigidbody>(out rb))
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }

    public override string GetTooltip() => action;
}
