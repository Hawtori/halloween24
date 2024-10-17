using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 50f;
    public float gravityModifier = 25f;
    public float maxSpeed = 10f;
    public float maxVertSpeed = 15f;
    public float dragAmount = 2f;
    public float slowModifier = 50f;

    private bool isGrounded = false;
    private bool isJumping = false;
    private Vector3 movement;

    private Rigidbody rb;

    private bool jumpFlag = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetInputs();
        CheckGrounded();
        ImproveJumping();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        rb.AddForce(Vector3.down * gravityModifier * Time.deltaTime);
    }

    private void GetInputs()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");
        if (!isJumping)
        {
            isJumping = Input.GetKey(KeyCode.Space);
            Invoke(nameof(ResetJumping), 0.25f);
        }
    }

    private void MovePlayer()
    {
        if (isJumping && isGrounded)
        {
            rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
            isJumping = false;
        }
        Vector3 xzVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if(rb.velocity.y > maxVertSpeed) rb.velocity = xzVel + Vector3.up * maxVertSpeed;

        if(xzVel.magnitude > maxSpeed)
        {
            rb.AddForce(-rb.velocity.normalized * dragAmount * Time.deltaTime);
            //rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else rb.AddForce(movement.normalized * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);

        if(movement.x == 0f && movement.z == 0f && rb.velocity.magnitude > 0f)
        {
            rb.AddForce(-rb.velocity.normalized * dragAmount * slowModifier);
         
        }

        if(movement.magnitude > 0.1f) 
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement, Vector3.up), Time.deltaTime * 4f);
    }

    private void CheckGrounded()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position + Vector3.down, 0.25f);
        foreach(var h in hit)
        {
            if(h.CompareTag("Ground"))
            {
                isGrounded = true;
                jumpFlag = true;
                return;
            }
        }
        isGrounded = false;
    }

    private void ResetJumping()
    {
        if(jumpFlag)
        {
            jumpFlag = false;
            return;
        }
        isJumping = false;
    }

    private void ImproveJumping()
    {
        if (rb.velocity.y < 0f)
        {
            rb.AddForce(Vector3.down * jumpForce * gravityModifier * Time.deltaTime);
        }
    }

}
