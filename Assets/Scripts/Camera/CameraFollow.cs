using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    private InputAction lookAction;
    [SerializeField]
    private InputActionAsset playerControls;

    private void Awake()
    {
        lookAction = playerControls.FindActionMap("Movement").FindAction("Look");
    }

    private void OnEnable()
    {
        lookAction.Enable();
    }

    private void OnDisable()
    {
        lookAction.Disable();

    }

    private void LateUpdate()
    {
        transform.position = player.position + Vector3.up / 2;

    }
}
