using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage = 5;
    public float speed = 80f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * speed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 7)
        {
            Debug.Log("Damaged enemy");
            collision.transform.GetComponent<EnemyBase>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
