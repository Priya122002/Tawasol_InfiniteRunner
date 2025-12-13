using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 10f;
    public float jumpForce = 10f;

    [Header("Speed Increase")]
    public float speedIncreaseRate = 0.1f;
    public float maxSpeed = 25f;

    [Header("Lane Settings")]
    public float laneOffset = 1.5f;
    public float laneChangeSpeed = 10f;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private bool jumpLocked;
    public bool canMove = true;

    private int currentLane = 0;     // -1 left, 0 middle, 1 right
    private float targetX = 0f;

    // Swipe handling
    private bool isSwiping = false;
    private Vector2 swipeStart;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

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

        IncreaseSpeedOverTime();  // ⭐ NEW FEATURE
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        ForwardMovement();
        LaneMovement();
    }

    // ---------------------------
    // SPEED INCREASE SYSTEM
    // ---------------------------
    void IncreaseSpeedOverTime()
    {
        forwardSpeed += speedIncreaseRate * Time.deltaTime;

        if (forwardSpeed > maxSpeed)
            forwardSpeed = maxSpeed;
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
        float newX = Mathf.MoveTowards(rb.position.x, targetX, laneChangeSpeed * Time.fixedDeltaTime);

        rb.MovePosition(new Vector3(
            newX,
            rb.position.y,
            rb.position.z
        ));
    }

    // ---------------------------
    // INPUT
    // ---------------------------
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            ChangeLane(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ChangeLane(+1);
    }

    void HandleSwipeInput()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began)
        {
            swipeStart = t.position;
            isSwiping = true;
        }
        else if (t.phase == TouchPhase.Ended && isSwiping)
        {
            Vector2 delta = t.position - swipeStart;

            if (Mathf.Abs(delta.x) > 50)
            {
                if (delta.x > 0) ChangeLane(+1);
                else ChangeLane(-1);
            }

            isSwiping = false;
        }
    }

    void ChangeLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, -1, 1);
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
        // 🔊 PLAY JUMP SOUND
        if (jumpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
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
