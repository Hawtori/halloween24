using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTemp : MonoBehaviour
{

    public float rotationRate = 5f;
    private bool isOpening = false;
    float angle = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isOpening = true;
        }

        if(isOpening)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        angle += Time.deltaTime * rotationRate;
        angle = Mathf.Clamp(angle, 0, 90);
        Quaternion rot = Quaternion.Euler(-angle, 0, 0);
        transform.rotation = rot;
        if(angle == 90)
        {
            isOpening = false;
            angle = 0;
        }
    }
}
