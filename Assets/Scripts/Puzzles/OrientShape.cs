using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientShape : MonoBehaviour
{
    public delegate void OrientationHandler();
    public event OrientationHandler OnCorrectOrientation;
    public event OrientationHandler OnIncorrectOrientation;

    private bool isCorrect = false;
    private int correctRotation;

    private void Start()
    {
        correctRotation = Random.Range(0, 73) * 5;
    }

    public void RotateShape(int dir) // -1 or 1 {left or right}
    {
        // make it rotate transform.rotoate.y += dir * 5
        transform.Rotate(0, dir * 5f, 0);
        checkOrientation();
    }

    public void checkOrientation()
    {
        if (Mathf.Abs(transform.rotation.eulerAngles.y - correctRotation) < 0.1f)
        {
            isCorrect = true;
            OnCorrectOrientation?.Invoke();
        }
        else
        {
            if (isCorrect)
            {
                isCorrect = false;
                OnIncorrectOrientation?.Invoke();
            }
            
        }
    }
}
