using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 10f;
    public float jumpForce = 10f;

    [Header("Lane Settings")]
    public float laneOffset = 2.5f;
    public float laneChangeSpeed = 12f;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;

    private Rigidbody rb;

    private bool isGrounded;
    private bool jumpLocked;
    public bool canMove = true;

    private int currentLane = 0;     // -1 = left, 0 = middle, 1 = right
    private float targetX = 0f;

    // Swipe
    private bool isSwiping = false;
    private Vector2 swipeStart;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Freeze rotation ONLY (never freeze X)
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (!canMove) return;

        HandleKeyboardInput();
        HandleSwipeInput();
        GroundCheck();

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        ForwardMovement();
        LaneMovement();
    }

    // ---------------------------
    // FORWARD MOVEMENT
    // ---------------------------
    void ForwardMovement()
    {
        Vector3 vel = rb.velocity;
        vel.z = forwardSpeed;
        rb.velocity = vel;
    }

    // ---------------------------
    // LANE MOVEMENT
    // ---------------------------
    void LaneMovement()
    {
        float moveX = (targetX - rb.position.x) * laneChangeSpeed;

        Vector3 vel = rb.velocity;
        vel.x = moveX;
        rb.velocity = vel;
    }

    // ---------------------------
    // KEYBOARD INPUT (OLD SYSTEM)
    // ---------------------------
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            ChangeLane(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ChangeLane(+1);
    }

    // ---------------------------
    // SWIPE INPUT (OLD SYSTEM)
    // ---------------------------
    void HandleSwipeInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            swipeStart = touch.position;
            isSwiping = true;
        }
        else if (touch.phase == TouchPhase.Ended && isSwiping)
        {
            Vector2 delta = touch.position - swipeStart;

            if (Mathf.Abs(delta.x) > 50f)
            {
                if (delta.x > 0) ChangeLane(+1);
                else ChangeLane(-1);
            }

            isSwiping = false;
        }
    }

    // ---------------------------
    // CHANGE LANE
    // ---------------------------
    void ChangeLane(int direction)
    {
        currentLane += direction;
        currentLane = Mathf.Clamp(currentLane, -1, 1);

        targetX = currentLane * laneOffset;
    }

    // ---------------------------
    // JUMP
    // ---------------------------
    public void Jump()
    {
        if (!isGrounded || jumpLocked) return;

        Vector3 vel = rb.velocity;
        vel.y = jumpForce;
        rb.velocity = vel;

        jumpLocked = true;
    }

    public bool IsJumping => !isGrounded;

    // ---------------------------
    // GROUND CHECK
    // ---------------------------
    void GroundCheck()
    {
        isGrounded = Physics.Raycast(
            groundCheckPoint.position,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );

        if (isGrounded)
            jumpLocked = false;
    }
}
