using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    [SerializeField]
    public Material mat;

    private float inLightTimer = 0f;

    private Color a;

    private void Update()
    {
        if (mat)
        {
            if(inLightTimer <= 0) // in light long enough to flash
            {
                a.a = 0.4f;
                mat.color = a;

                Invoke(nameof(ResetMat), Random.Range(.26f, .73f)); // stop being visible after some time between these values
            }
            else // has not been in light long enough
            {
                inLightTimer -= Time.deltaTime;
            }
        }
    }

    private void ResetMat()
    {
        if (!mat) return;
        a.a = 0.05f;
        mat.color = a;

        ResetTimer();
    }

    // how long the light has to be on enemy to make them flash
    private void ResetTimer()
    {
        inLightTimer = Random.Range(1.17f, 2.63f);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Invisible"))
        {
            mat = other.GetComponent<MeshRenderer>().material;
            a = mat.color;
            ResetTimer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Invisible"))
        {
            a.a = 0.05f;
            mat.color = a;

            mat = null;
        }
    }
}
