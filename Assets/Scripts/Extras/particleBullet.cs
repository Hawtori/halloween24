using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleBullet : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        GetComponent<ParticleSystem>().Stop();
    }
}
