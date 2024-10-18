using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraInputController : InputAxisControllerBase<CameraInputController.InputReader>
{

    public class InputReader : IInputAxisReader
    {
        public float lookSens = 5f;

        public float GetValue(Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
        {
            return lookSens;
        }
    }
}
