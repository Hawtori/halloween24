using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FirstPersonMove : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField]
    private InputActionAsset playerControls;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    [Header("Input variables")]
    private Vector2 moveInput;
    //private Vector2 lookInput;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float dragAmount = 5f;
    [SerializeField] private float slowModifier = 75f;

    [Header("Stamina and Stats")]
    private PlayerStats stats;

    //[Header("Look")]
    //[SerializeField] private float mouseSens;
    //private float verticalRotation;

    [Header("Necessary Components")]
    private Rigidbody rb;

    [Header("UI")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Color exhaustColor;
    [SerializeField] private Color goodToGoColor;
    [SerializeField] private Color sprintingColor;

    private void Awake()
    {
        stats = new PlayerStats();

        moveAction = playerControls.FindActionMap("Movement").FindAction("Move");
        jumpAction = playerControls.FindActionMap("Movement").FindAction("Jump");
        sprintAction = playerControls.FindActionMap("Movement").FindAction("Sprint");
        lookAction = playerControls.FindActionMap("Movement").FindAction("Look");

        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        sprintAction.performed += ctx => stats.Sprint();
    }


    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
        sprintAction.performed -= ctx => stats.Sprint();
    }

    private void Update()
    {
        ReadMovement();
        stats.Update();
        UpdateUI();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleStopping();
        //HandleRotation();

    }

    private void ReadMovement()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        if (sprintAction.WasReleasedThisFrame()) stats.StopSprint();
    }

    private void HandleMovement()
    {
        Vector3 xzVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 moveDir = moveInput.y * forward + moveInput.x * right;

        if (xzVel.magnitude > maxSpeed * stats.GetSpeedModifier())
        {
            rb.AddForce(-rb.velocity.normalized * dragAmount * Mathf.Max(xzVel.magnitude - (maxSpeed * stats.GetSpeedModifier()), 1) * Time.deltaTime);
        }
        else
        {
            rb.AddForce(moveDir.normalized * moveSpeed * stats.GetSpeedModifier() * Time.deltaTime, ForceMode.VelocityChange);
        }

    }

    private void HandleStopping()
    {
        if (moveInput == Vector2.zero && rb.velocity.magnitude > 0.2f)
        {
            rb.AddForce(-rb.velocity.normalized * dragAmount * slowModifier);
        }
    }

    //private void HandleRotation()
    //{
    //    lookInput = lookAction.ReadValue<Vector2>();

    //    float mouseX = lookInput.x * mouseSens * Time.deltaTime;
    //    transform.Rotate(0, mouseX, 0);


    //    verticalRotation -= lookInput.y * mouseSens * Time.deltaTime;
    //    verticalRotation = Mathf.Clamp(verticalRotation, -85f, 85f);
    //    Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    //}


    private void UpdateUI()
    {
        if (staminaSlider)
        {
            staminaSlider.value = stats.GetStamina();
            switch(stats.GetState())
            {
                case 1: // exhausted
                    fillImage.color = exhaustColor;
                    break;
                case 2: // walking
                    fillImage.color = goodToGoColor;
                    break;
                case 3: // sprinting
                    fillImage.color = sprintingColor;
                    break;
            }
        }
    }
}
