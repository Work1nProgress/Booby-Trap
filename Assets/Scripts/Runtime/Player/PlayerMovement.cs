using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    enum MovementType
    {
        Standing,
        Running,
        Jumping,
        Falling,
        Dashing
    }

    #region variable declaration
    [Header("Running")]
    [SerializeField]
    [Tooltip("Top speed")] [Range(0f, 50f)]
    float runSpeed = 10;
    [SerializeField] [Range(0f, 10f)]
    [Tooltip("Horizontal movement acceleration")]
    float runAcceleration = 5;
    [SerializeField] [Range(0f, 10f)]
    [Tooltip("Horizontal movement decceleration")]
    float runDecceleration = 5;

    [Header("Vertical Movement")]
    [Header("Jumping")]
    [Tooltip("Jumping force happens once")]
    [SerializeField][Range(0f, 20f)]
    float jumpForce = 10;
    
    [SerializeField][Range(0f, 5f)]
    [Tooltip("Multiplier for horizontal movement acceleration in air")]
    float jumpAcceleration = 1;
    [SerializeField][Range(0f, 5f)]
    [Tooltip("Multiplier for horizontal movement decceleration in air")]
    float jumpDecceleration = 1;

    [Header("Gravity")]
    [Tooltip("Overall gravity multiplier")]
    [SerializeField]
    [Range(0, 10)]
    float gravityMultiplier = 1;
    [SerializeField][Range(0f, 5f)]
    [Tooltip("Gravity multiplier while jumping and holding jump")]
    float jumpGravityJumpHeld;
    [SerializeField][Range(0f, 5f)]
    [Tooltip("Gravity multiplier while jumping and not holding jump")]
    float jumpGravity;
    [SerializeField][Range(0f, 5f)]
    [Tooltip("Gravity multiplier while falling and holding jump")]
    float fallingGravityJumpHeld;
    [SerializeField][Range(0f, 5f)]
    [Tooltip("Gravity multiplier while falling and not holding jump")]
    float fallingGravity;
    [SerializeField]
    [Range(0f, 5f)]
    [Tooltip("Gravity multiplier while falling after dashing")]
    float fallingGravityAfterDash;

    [SerializeField]

    [Tooltip("Gravity multiplier while falling after dashing")]
    float maximumFallSpeed;

    [Header("Hang Times")]
    [SerializeField][Range(0f, 5f)]
    [Tooltip("Vertical velocity threshold at apex of jump to toggle better manouverability")]
    float hangTimeThreshold = 0.1f;
    [SerializeField][Range(0f, 5f)]
    [Tooltip("Extra acceleration at apex")]
    float hangTimeAccelerationMult = 1.1f;
    [SerializeField][Range(0f, 5f)]
    [Tooltip("Extra top speed at apex")]
    float hangTimeSpeedMult = 1.1f;

    [SerializeField][Range(0f, 5f)]
    [Tooltip("Meep Meep")]
    float coyoteTime;

    [Header("Dash")]
    [SerializeField]
    [Range(1, 1000)]
    float dashSpeed = 500;
    [SerializeField]
    [Range(0, 10)]
    float dashAcceleration = 10;
    [SerializeField]
    [Range(0, 10)]
    float dashCooldown = 3.0f;
    [SerializeField]
    [Range(0, 10)]
    float dashDuration = 0.1f;
    [SerializeField] private Vector2 dashCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private Vector3 dashCheckPointLeft;
    [SerializeField] private Vector3 dashCheckPointRight;

    [Header("Layers")]
    public LayerMask groundLayer;

    // raycasts
    [Header("Ground Checks")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private Vector3 groundCheckPoint;

    [Header("Debugging")]
    [SerializeField] Vector2 velocity;

    // components
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;
    private PlayerSound sound;

    //flags
    private bool fallingFromDash = false;

    //input
    private float inputX, inputY;
    private bool jumpHeld = false;

    //timers
    float lastOnGroundTime;
    float lastDashDuration;

    // dash timers
    float timeSpentDashing;
    float timeSinceLastDash;
    float lastDashTime;
    float dashStartTime;

    //private variables
    float dashDirection;
    float canDashDirection;
    bool hasDashed = false;
    bool onGround;
    float accelerationRate;
    float targetSpeed;

    [SerializeField]
    private MovementType state;
    #endregion

    #region public functions and properties
    // properties
    public bool OnGround => lastOnGroundTime > 0;
    //public bool Dashing => LastDashDurationTime > 0;

    public Vector2 FaceDirection {
        get
        {
            if(sprite.flipX)
                return Vector2.left;
            else
                return Vector2.right;
        }
    }

    // functions
    public void SetJumpState() { ChangeState(MovementType.Jumping); }
    public void SetDashState() 
    { 
        if(timeSinceLastDash >= dashCooldown || !hasDashed)
            ChangeState(MovementType.Dashing);
    }
    #endregion

    #region start and update
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        sound = GetComponent<PlayerSound>();

        state = MovementType.Standing;
    }

    // Update is called once per frame
    void Update()
    {        
        UpdateCooldowns();

        jumpHeld = Input.GetButton("Jump");
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        GroundCheck();

        if(inputX != 0 && onGround && state != MovementType.Running)
            ChangeState(MovementType.Running);

        switch(state) 
        {
            case MovementType.Running: UpdateRun();  break;
            case MovementType.Jumping: UpdateJump(); break;
            case MovementType.Dashing: UpdateDash(); break;
            case MovementType.Falling: UpdateFall(); break;
        }

        // switch animation if falling
        animator.SetFloat("yVelocity", body.velocity.y);
        animator.SetBool("onGround", OnGround);

        velocity = body.velocity;
    }

    private void FixedUpdate()
    {
        if (state != MovementType.Standing)
            HorizontalMovement();
        else
            body.velocity = new Vector2(0, body.velocity.y); // prevents player from sliding backwards
    }

    private void LateUpdate()
    {
        // update the face direction
        if (inputX > 0 && body.velocity.x > 0)// && Mathf.Abs(body.velocity.x) > 0.1f) // facing right
            sprite.flipX = false;

        if (inputX < 0 && body.velocity.x < 0)// && Mathf.Abs(body.velocity.x) > 0.1f) // facing left
            sprite.flipX = true;
    }
    #endregion

    #region private functions
    private void ChangeState(MovementType newState)
    {
        if (state == newState)
            return;

        state = newState;

        switch (state)
        {
            case MovementType.Standing:
                animator.SetBool("running", false);
                animator.SetBool("dashing", false);
                body.velocity = Vector2.zero;
            break;

            case MovementType.Running:
                animator.SetBool("running", true);
            break;

            case MovementType.Jumping:
                StartJump();
            break;

            case MovementType.Dashing:
                hasDashed = true;
                dashStartTime = Time.time;
                animator.SetBool("dashing", true);
                body.gravityScale = 0;
            break;
        }
    }

    private void UpdateCooldowns()
    {
        lastOnGroundTime -= Time.deltaTime;
        //LastDashTime -= Time.deltaTime;
        //LastDashDurationTime -= Time.deltaTime;
        timeSinceLastDash = Time.time - lastDashTime;
    }

    private void StartJump()
    {
        if (lastOnGroundTime > 0)
        {
            lastOnGroundTime = 0;

            body.gravityScale = gravityMultiplier * jumpGravityJumpHeld;
            animator.SetTrigger("jump");

            if (sound != null)
                sound.PlaySound(sound.jumpSound);            

            body.AddForce(Vector2.up * (jumpForce - body.velocity.y), ForceMode2D.Impulse);
        }
    }

    private void UpdateJump()
    {
        if (body.velocity.y <= 0)
        {
            ChangeState(MovementType.Falling);
        }

        // higher gravity if let go of jump
        if (!Input.GetButton("Jump"))
        {
            body.gravityScale = gravityMultiplier * jumpGravity;
        }
    }
    
    private void UpdateFall()
    {
        if (!OnGround)
        {
            if (jumpHeld)
            {
                body.gravityScale = gravityMultiplier * fallingGravityJumpHeld;

            }
            else if (fallingFromDash)
            {
                body.gravityScale = gravityMultiplier * fallingGravityAfterDash;
            }
            else
            {
                body.gravityScale = gravityMultiplier * fallingGravity;
            }

            body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -maximumFallSpeed, 0));
        }
    }

    private void HorizontalMovement()
    {
        //Calculate the direction we want to move in and our desired velocity
        if (state == MovementType.Dashing)
        {
            targetSpeed = dashDirection * dashSpeed;
        }
        else
        {
            targetSpeed = inputX * runSpeed;
        }

        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(body.velocity.x, targetSpeed, 1);
        
        if (state == MovementType.Dashing)
        {
            accelerationRate = dashAcceleration;
        }
        else if (OnGround)
        {
            if (Mathf.Abs(targetSpeed) > 0.01f)
                accelerationRate = runAcceleration;
            else
                accelerationRate = runDecceleration;
        }
        else
        {
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration * jumpAcceleration : runDecceleration * jumpDecceleration;
        }
        
        if (lastDashDuration <= 0 && state == MovementType.Jumping || state == MovementType.Falling &&
            Mathf.Abs(body.velocity.y) < hangTimeThreshold)
        {
            accelerationRate *= hangTimeAccelerationMult;
            targetSpeed *= hangTimeSpeedMult;
        }

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed -body.velocity.x;
        //Calculate force along x-axis to apply to thr player
        float movement = speedDif * accelerationRate;
        
        //Convert this to a vector and apply to rigidbody
        body.AddForce(movement * Vector2.right, ForceMode2D.Force);

        if (OnGround && inputX == 0 && state != MovementType.Dashing)
            ChangeState(MovementType.Standing);
    }

    private void UpdateRun()
    {
        if (OnGround)
        {
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration : runDecceleration;
        }
        else
        {
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration * jumpAcceleration : runDecceleration * jumpDecceleration;
        }        
    }
   
    private void UpdateDash()
    {
        //dash cooldown ended and dash input pressed
        if (!fallingFromDash)
        {
            //if player is holding a direction
            if (Mathf.Abs(inputX) > 0)
            {
                dashDirection = inputX;
            }
            //use the current facing direction
            else
            {
                dashDirection = FaceDirection.x;
            }
            //check if player is already colliding with a wall
            if (canDashDirection == 0 || dashDirection != canDashDirection)
            {
                lastDashDuration = dashDuration;
               // LastDashTime = dashCooldown;

                body.velocity = new Vector2(body.velocity.x, 0);                
            }
            else
            {
                dashDirection = 0;
            }
        }

        // check if there is a collision with a wall on the left
        canDashDirection = 0;
        var dashCanceledLeft = Physics2D.OverlapBox(transform.position + dashCheckPointLeft, dashCheckSize, 0, groundLayer);
        if (dashCanceledLeft)
        {
            if (dashDirection < 0)
            {
                EndDash();
            }
            canDashDirection -= 1;
        }

        // check if there is a collision with a wall on the right
        var dashCanceledRight = Physics2D.OverlapBox(transform.position + dashCheckPointRight, dashCheckSize, 0, groundLayer);
        if (dashCanceledRight)
        {
            if (dashDirection > 0)
            {
                EndDash();
            }
            canDashDirection += 1;
        }

        timeSpentDashing = Time.time - dashStartTime;

        if (timeSpentDashing >= dashDuration)
            EndDash();

        /*
        if (wasDashing && !Dashing)
        {
            EndDash();
        }
        wasDashing = Dashing;*/
    }

    private void GroundCheck()
    {
        onGround = Physics2D.OverlapBox(transform.position + groundCheckPoint, groundCheckSize, 0, groundLayer) && 
                   state != MovementType.Jumping;

        if (onGround)
        {
            lastOnGroundTime = coyoteTime;
            fallingFromDash = false;
            body.gravityScale = gravityMultiplier * fallingGravity;

            if (state == MovementType.Jumping)
                state = MovementType.Standing;
        }

    }

    void EndDash()
    {
        lastDashDuration = 0;
        //LastDashTime = dashCooldown;
        dashDirection = 0;
        lastOnGroundTime = 0;        

        if (!OnGround)
        {
            fallingFromDash = true;

            ChangeState(MovementType.Falling);
            body.gravityScale = gravityMultiplier* fallingGravityAfterDash;
        }
        else 
        {
            ChangeState(MovementType.Standing);
        }

        lastDashTime = Time.time;
        animator.SetBool("dashing", false);
    }

    /*
    private bool CanJump()
    {
        return OnGround && !jumping;
    }*/

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(transform.position + groundCheckPoint, groundCheckSize);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + dashCheckPointLeft, dashCheckSize);
        Gizmos.DrawWireCube(transform.position + dashCheckPointRight, dashCheckSize);
    }
    #endregion

}
