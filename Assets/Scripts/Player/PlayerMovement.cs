using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 8f;
    public float jumpForce = 8f;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool canMove = false;
    private bool isGrounded = false;

    private bool jumpLocked = false;   // 🔥 prevents double-jump

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(StartDelay());
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.25f);
        canMove = true;
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        // Always move forward (Z)
        Vector3 vel = rb.linearVelocity;
        vel.z = forwardSpeed;
        rb.linearVelocity = vel;

        // Ground check
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, checkRadius, groundLayer);

        // 🔥 When grounded → unlock jump
        if (isGrounded)
            jumpLocked = false;
    }

    public void Jump()
    {
        if (!canMove) return;
        if (!isGrounded) return;
        if (jumpLocked) return;   // 🔥 stop jump spam

        jumpLocked = true;        // 🔥 lock jump until landing
        isGrounded = false;

        Vector3 vel = rb.linearVelocity;
        vel.y = jumpForce;
        rb.linearVelocity = vel;

        Debug.Log("JUMP TRIGGERED");
    }

    // For ghost sync
    public bool IsJumping => !isGrounded;
    public Vector3 CurrentVelocity => rb.linearVelocity;
}
