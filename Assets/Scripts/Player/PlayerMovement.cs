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

    private PlayerInput playerInput;   // 🔥 New Input System reference
    private InputAction jumpAction;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions["Jump"];
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

        // Forward movement
        Vector3 vel = rb.linearVelocity;
        vel.z = forwardSpeed;
        rb.linearVelocity = vel;

        // ---- FIXED GROUND CHECK ----
        Collider[] cols = Physics.OverlapSphere(groundCheckPoint.position, checkRadius, groundLayer);

        isGrounded = false;

        foreach (var c in cols)
        {
            if (c.transform != transform)   // ignore player's own collider
            {
                isGrounded = true;
                break;
            }
        }

        if (isGrounded && !jumpAction.enabled)
            jumpAction.Enable();

    }


    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        Jump();
    }

    public void Jump()
    {
        if (!canMove) return;
        if (!isGrounded) return;

        Vector3 vel = rb.linearVelocity;
        vel.y = jumpForce;
        rb.linearVelocity = vel;

        Debug.Log("JUMP TRIGGERED");

        jumpAction.Disable(); 
    }


    public bool IsJumping => !isGrounded;
    public Vector3 CurrentVelocity => rb.linearVelocity;
}
