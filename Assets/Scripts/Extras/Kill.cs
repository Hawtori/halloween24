using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        HUDManager.instance.UpdateText("You died from falling!");
        Destroy(collision.gameObject);
    }
}
