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

    private void Awake()
    {
        correctRotation = Random.Range(0, 36) * 10;
    }

    public void RotateShape(int dir) // -1 or 1 {left or right}
    {
        // make it rotate transform.rotoate.y += dir * 10
        transform.Rotate(0, dir * 10f, 0);
        checkOrientation();
    }

    public int GetCorrectRotation() => correctRotation;

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
