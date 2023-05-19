using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
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

    [Header("Vault")]

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
    private bool jumping = false;
    private bool falling = false;
    private bool fallingFromDash = false;

    //input
    private float inputX, inputY;
    //private bool jumpPressed = false;
    private bool jumpHeld = false;
    //private bool dashPressed = false;
    private bool dashing;

    //timers
    float LastOnGroundTime;
    float LastDashTime;
    float LastDashDurationTime;

    //private fields
    float dashDirection;
    float canDashDirection;
    bool wasDashing;
    #endregion

    #region public functions and properties
    // properties
    public bool OnGround => LastOnGroundTime > 0;
    public bool Dashing => LastDashDurationTime > 0;

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
    public void Jump()
    {
        if (LastOnGroundTime > 0)
        {
            LastOnGroundTime = 0;
            if (!jumping)
            {
                body.gravityScale = gravityMultiplier * jumpGravityJumpHeld;
                animator.SetTrigger("jump");

                if (sound != null)
                    sound.PlaySound(sound.jumpSound);
            }

            jumping = true;

            body.AddForce(Vector2.up * (jumpForce - body.velocity.y), ForceMode2D.Impulse);
        }
    }

    public void StartDash()
    {
        dashing = true;
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
    }

    // Update is called once per frame
    void Update()
    {
        LastOnGroundTime -= Time.deltaTime;
        LastDashTime -= Time.deltaTime;
        LastDashDurationTime -= Time.deltaTime;
        //jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        //dashPressed = Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.K);

        GroundCheck();

        UpdateJump();
        UpdateDash();
       
        VerticalMovement();

        if (LastOnGroundTime > 0 && Mathf.Abs(body.velocity.x) > 0.2f && inputX != 0)
            animator.SetBool("running", true);
        else if (LastOnGroundTime <= 0 || Mathf.Abs(body.velocity.x) < 0.2f)
            animator.SetBool("running", false);

        //Debug.Log("Velocity: " + body.velocity);

        // switch animation if falling
        animator.SetFloat("yVelocity", body.velocity.y);
        animator.SetBool("onGround", OnGround);

        velocity = body.velocity;
    }

    private void FixedUpdate()
    {
        HorizontalMovement();
    }

    private void LateUpdate()
    {
        // update the face direction
        if (body.velocity.x > 0 && Mathf.Abs(body.velocity.x) > 0.1f) // facing right
            sprite.flipX = false;

        if (body.velocity.x < 0 && Mathf.Abs(body.velocity.x) > 0.1f) // facing left
            sprite.flipX = true;
    }
    #endregion

    #region private functions
    private void UpdateJump()
    {
        if (body.velocity.y <= 0)
        {
            jumping = false;

            if (!OnGround && !falling)
            {

                falling = true;
            }

            if (OnGround && falling)
            {
                falling = false;
                jumping = false;
            }
            if (!OnGround && falling)
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
    }

    

    private void HorizontalMovement()
    {
        float targetSpeed;

        //Calculate the direction we want to move in and our desired velocity
        if (Dashing)
        {
            targetSpeed = dashDirection * dashSpeed;
        }
        else
        {
            targetSpeed = inputX * runSpeed;
        }
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(body.velocity.x, targetSpeed, 1);

        float accelerationRate;

        if (Dashing)
        {
            accelerationRate = dashAcceleration;
        }
        else if (OnGround)
        {
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration : runDecceleration;
        }
        else
        {
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration * jumpAcceleration : runDecceleration * jumpDecceleration;
        }

        if (!Dashing && (jumping || falling) && Mathf.Abs(body.velocity.y) < hangTimeThreshold)
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
    }

    private void VerticalMovement()
    {   if (Dashing)
        {
            body.gravityScale = 0;
            return;
        }        

        // higher gravity if let go of jump
        if (jumping && !Input.GetButton("Jump"))
        {
            body.gravityScale = gravityMultiplier * jumpGravity;
        }
    }
   
    private void UpdateDash()
    {
        //dash cooldown ended and dash input pressed
        if (LastDashTime <= 0 && dashing && !fallingFromDash)
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
                LastDashDurationTime = dashDuration;
                LastDashTime = dashCooldown;

                body.velocity = new Vector2(body.velocity.x, 0);
                falling = false;
                jumping = false;
            }
            else
            {
                dashDirection = 0;

            }
        }

        if (wasDashing && !Dashing)
        {
            EndDash();
        }
        wasDashing = Dashing;
    }

    private void GroundCheck()
    {
        canDashDirection = 0;
        var dashCanceledLeft = Physics2D.OverlapBox(transform.position + dashCheckPointLeft, dashCheckSize, 0, groundLayer);
        if (dashCanceledLeft)
        {
            if (Dashing && dashDirection < 0)
            {
                EndDash();
            }
            canDashDirection -= 1;
        }

        var dashCanceledRight = Physics2D.OverlapBox(transform.position + dashCheckPointRight, dashCheckSize, 0, groundLayer);
        if (dashCanceledRight)
        {
            if (Dashing && dashDirection > 0)
            {
                EndDash();
            }
            canDashDirection += 1;
        }


        var onGround = Physics2D.OverlapBox(transform.position + groundCheckPoint, groundCheckSize, 0, groundLayer) && !jumping;
        if (onGround)
        {
            LastOnGroundTime = coyoteTime;
            fallingFromDash = false;
            body.gravityScale = gravityMultiplier * fallingGravity;
        }

    }

    void EndDash()
    {
        LastDashDurationTime = 0;
        LastDashTime = dashCooldown;
        dashDirection = 0;
        LastOnGroundTime = 0;
        wasDashing = false;
        if (!OnGround)
        {
            falling = true;
            fallingFromDash = true;
            body.gravityScale = gravityMultiplier* fallingGravityAfterDash;
        }

        dashing = false;
    }

    private bool CanJump()
    {
        return OnGround && !jumping;
    }   

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
