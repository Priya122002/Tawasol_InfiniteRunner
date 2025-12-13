using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 10f;
    [Header("Jump Tuning")]
    public float jumpForce = 5f;              // base force (keep)
    public float jumpExtraBoost = 2.5f;       // height control
    public float fallGravityMultiplier = 2f;  // makes landing fast


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
    void FixedUpdate()
    {
        if (!canMove) return;

        ForwardMovement();
        LaneMovement();
        ApplyBetterGravity();
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

    void ApplyBetterGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y *
                           fallGravityMultiplier * Time.fixedDeltaTime;
        }
    }

  


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

    public void Jump()
    {
        if (groundContacts == 0 || jumpLocked) return;

        // reset vertical velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // base jump
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // extra height (one-frame only)
        rb.AddForce(Vector3.up * jumpExtraBoost, ForceMode.VelocityChange);

        if (jumpSound != null && audioSource != null)
            audioSource.PlayOneShot(jumpSound);

        jumpLocked = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerWorld"))
        {
            groundContacts++;
            jumpLocked = false;
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerWorld"))
        {
            if (rb.velocity.y < 0)
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
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
