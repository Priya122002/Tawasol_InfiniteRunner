using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 10f;
    public float jumpForce = 10f;

    [Header("Speed Increase")]
    public float maxSpeed = 25f;

    [Header("Lane Settings")]
    public float laneOffset = 1.5f;
    public float laneChangeSpeed = 10f;

    [Header("Speed Interval Settings")]
    public float speedIncreaseInterval = 10f;   // every 10 seconds
    public float speedBoostAmount = 2f;         // how much speed increases

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;

    private Rigidbody rb;
    public bool canMove = true;

    private int currentLane = 0;     // -1 left, 0 middle, 1 right
    private float targetX = 0f;

    private float speedTimer = 0f;

    // ⭐ COLLISION BASED GROUND CHECK
    private int groundContacts = 0;
    private bool jumpLocked = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = true;
    }

    void Update()
    {
        if (!canMove) return;

        HandleKeyboardInput();
        HandleSwipeInput();
        HandleSpeedInterval();

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
    // SPEED INTERVAL
    // ---------------------------
    void HandleSpeedInterval()
    {
        speedTimer += Time.deltaTime;

        if (speedTimer >= speedIncreaseInterval)
        {
            speedTimer = 0f;
            IncreaseSpeed();
        }
    }

    void IncreaseSpeed()
    {
        forwardSpeed += speedBoostAmount;
        if (forwardSpeed > maxSpeed)
            forwardSpeed = maxSpeed;

        UIManager.Instance?.ShowSpeedEffect();
    }

    // ---------------------------
    // MOVEMENT
    // ---------------------------
    void ForwardMovement()
    {
        rb.velocity = new Vector3(
            rb.velocity.x,
            rb.velocity.y,
            forwardSpeed
        );
    }

    void LaneMovement()
    {
        float newX = Mathf.MoveTowards(
            rb.position.x,
            targetX,
            laneChangeSpeed * Time.fixedDeltaTime
        );

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
            // nothing needed here
        }
        else if (t.phase == TouchPhase.Ended)
        {
            Vector2 delta = t.deltaPosition;

            if (Mathf.Abs(delta.x) > 50)
            {
                if (delta.x > 0) ChangeLane(+1);
                else ChangeLane(-1);
            }
        }
    }

    void ChangeLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, -1, 1);
        targetX = currentLane * laneOffset;
    }

    // ---------------------------
    // JUMP (COLLISION BASED)
    // ---------------------------
    public void Jump()
    {
        if (groundContacts == 0 || jumpLocked) return;

        rb.velocity = new Vector3(
            rb.velocity.x,
            jumpForce,
            rb.velocity.z
        );

        if (jumpSound != null && audioSource != null)
            audioSource.PlayOneShot(jumpSound);

        jumpLocked = true;
    }

    // ---------------------------
    // COLLISION GROUND DETECTION
    // ---------------------------
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerWorld"))
        {
            groundContacts++;
            jumpLocked = false;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerWorld"))
        {
            groundContacts--;
            if (groundContacts < 0)
                groundContacts = 0;
        }
    }
}
