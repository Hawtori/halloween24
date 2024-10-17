using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private Vector2 lookInput;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float maxSpeed = 10f;
    private float dragAmount = 2f;
    private float slowModifier = 50f;

    [Header("Stamina and Stats")]
    private PlayerStats stats;

    [Header("Look")]
    [SerializeField] private float mouseSens;
    private float verticalRotation;

    [Header("Necessary Components")]
    private Rigidbody rb;

    private void Awake()
    {
        stats = new PlayerStats();

        moveAction = playerControls.FindActionMap("Movement").FindAction("Move");
        jumpAction = playerControls.FindActionMap("Movement").FindAction("Jump"); // jumpAction.triggered = bool value

        // sprintAction . read value <float> > 0 ? then we can sprint, else we just walk
        sprintAction = playerControls.FindActionMap("Movement").FindAction("Sprint");

        // mouse x rotation = lookInput.x * mouse sens || y rotation = lookinput.y * sens
        // make sure we have vertical rotation -= look input y
        // we can transform.rotate(0, mouse x rotation, 0) to look around
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
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();   
    }

    private void Update()
    {
        ReadMovement();
        stats.Update();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleStopping();
        HandleRotation();

    }

    private void LateUpdate()
    {
    }

    private void ReadMovement()
    {
        moveInput = moveAction.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        // sprintAction read value of type float, is it greater than 0 ? (getting pressed) ? then increase speed, else normal
        if(sprintAction.ReadValue<float>() > 0)
            stats.Sprint();
        else 
            stats.StopSprint();


        Vector3 xzVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 moveDir = moveInput.y * Camera.main.transform.forward + moveInput.x * Camera.main.transform.right;

        if (xzVel.magnitude > maxSpeed * stats.GetSpeedModifier())
        {
            rb.AddForce(-rb.velocity.normalized * dragAmount * Time.deltaTime);
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

    private void HandleRotation()
    {
        lookInput = lookAction.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSens * Time.deltaTime;
        transform.Rotate(0, mouseX, 0);


        verticalRotation -= lookInput.y * mouseSens * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -85f, 85f);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void LookAround(InputAction.CallbackContext ctx)
    {
        Vector2 lookInput = ctx.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSens * Time.deltaTime;
        transform.Rotate(0, mouseX, 0);

        verticalRotation -= lookInput.y * mouseSens * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -85f, 85f);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

}
