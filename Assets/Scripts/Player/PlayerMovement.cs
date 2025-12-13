using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 10f;
    [Header("Jump Tuning")]
    public float jumpForce = 5f;             
    public float jumpExtraBoost = 2.5f;      
    public float fallGravityMultiplier = 2f; 


    [Header("Speed Increase")]
    public float maxSpeed = 25f;

    [Header("Lane Settings")]
    public float laneOffset = 1.5f;
    public float laneChangeSpeed = 10f;

    [Header("Speed Interval Settings")]
    public float speedIncreaseInterval = 10f;   
    public float speedBoostAmount = 2f;       

   

    private Rigidbody rb;
    public bool canMove = true;

    private int currentLane = 0;    
    private float targetX = 0f;

    private float speedTimer = 0f;
    [Header("Speed Effect Control")]
    public int speedEffectEvery = 5;

    private int speedIncreaseCount = 0;
    private Vector2 swipeStartPos;

    private int groundContacts = 0;
    private bool jumpLocked = false;
    [Header("Swipe Settings")]
    public float minSwipeDistance = 50f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

      

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

        speedIncreaseCount++;

        if (speedIncreaseCount % speedEffectEvery == 0)
        {
            UIManager.Instance?.ShowSpeedEffect();
        }
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
            swipeStartPos = t.position;
        }
        else if (t.phase == TouchPhase.Ended)
        {
            Vector2 swipeDelta = t.position - swipeStartPos;

            if (swipeDelta.magnitude < minSwipeDistance)
                return;

            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                if (swipeDelta.x > 0)
                    ChangeLane(+1);
                else
                    ChangeLane(-1);
            }
            else
            {
                if (swipeDelta.y > 0)
                    Jump();
            }
        }
    }


    void ChangeLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, -1, 1);
        targetX = currentLane * laneOffset;
    }
    public void StopMovement()
    {
        Debug.Log("PLAYER HARD STOP");

        canMove = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;    
    }

    public void ResumeMovement()
    {
        Debug.Log("PLAYER RESUME");

        rb.isKinematic = false;
        canMove = true;
    }



    public void Jump()
    {
        if (groundContacts == 0 || jumpLocked) return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        rb.AddForce(Vector3.up * jumpExtraBoost, ForceMode.VelocityChange);

        SoundManager.Instance.Play("jump");

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
