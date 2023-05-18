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
    [Tooltip("Top speed")][Range(0f, 50f)]
    float runSpeed = 10;
    [SerializeField][Range(0f, 10f)]
    [Tooltip("Horizontal movement acceleration")]
    float runAcceleration = 5;
    [SerializeField][Range(0f, 10f)]
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

    [Header("Layers")]
    public LayerMask _groundLayer;

    // raycasts
    [Header("Ground Checks")]
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private Vector3 _groundCheckPoint;

    [Header("Debugging")]
    [SerializeField] Vector2 velocity;

    // components
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;
    private PlayerSound sound;    

    private float inputX, inputY;
    private bool jumping = false;
    private bool jumpPressed = false;
    private bool jumpHeld = false;

    //fields
    float LastOnGroundTime;
    private bool falling = false;
    #endregion

    // properties
    public bool OnGround { get; set; }

    public Vector2 FaceDirection {
        get
        {
            if(sprite.flipX)
                return Vector2.left;
            else
                return Vector2.right;
        }
    }

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
        jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        GroundCheck();

        UpdateJump();
        
        HorizontalMovement();
        VerticalMovement();

        if (LastOnGroundTime > 0 && Mathf.Abs(body.velocity.x) > 0.2f && inputX != 0)
            animator.SetBool("running", true);
        else if (LastOnGroundTime <= 0 || Mathf.Abs(body.velocity.x) < 0.2f)
            animator.SetBool("running", false);

        //Debug.Log("Velocity: " + body.velocity);

        // switch animation if falling
        animator.SetFloat("yVelocity", body.velocity.y);
        animator.SetBool("onGround", LastOnGroundTime > 0);

        velocity = body.velocity;
    }

    private void LateUpdate()
    {
        // update the face direction
        if (body.velocity.x > 0 && Mathf.Abs(body.velocity.x) > 0.1f) // facing right
            sprite.flipX = false;

        if (body.velocity.x < 0 && Mathf.Abs(body.velocity.x) > 0.1f) // facing left
            sprite.flipX = true;
    }

    public void Jump(float liftForce)
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

        body.AddForce(Vector2.up * (liftForce - body.velocity.y), ForceMode2D.Impulse);
    }

    private void UpdateJump()
    {
        if (body.velocity.y <= 0)
        {
            jumping = false;

            if (LastOnGroundTime <= 0 && !falling)
            {

                falling = true;
            }

            if (LastOnGroundTime > 0 && falling)
            {
                falling = false;
                jumping = false;
            }
            if (LastOnGroundTime <= 0 && falling)
            {
                if (jumpHeld)
                {
                    body.gravityScale = gravityMultiplier * fallingGravityJumpHeld;

                }
                else
                {
                    body.gravityScale = gravityMultiplier * fallingGravity;
                }
            }
        }
    }

    private void HorizontalMovement()
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = inputX * runSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(body.velocity.x, targetSpeed, 1);

        float accelerationRate;
        if (LastOnGroundTime > 0)
        {

            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration : runDecceleration;

        }
        else
        {
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration * jumpAcceleration : runDecceleration * jumpDecceleration;
        }

        if ((jumping || falling) && Mathf.Abs(body.velocity.y) < hangTimeThreshold)
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
    {             
        if (jumpPressed && LastOnGroundTime > 0)
        {
            Jump(jumpForce);
        }

        // higher gravity if let go of jump
        if (jumping && !Input.GetButton("Jump"))
        {
            body.gravityScale = gravityMultiplier * jumpGravity;
        }
    }

    private void GroundCheck()
    {
        OnGround = Physics2D.OverlapBox(transform.position + _groundCheckPoint, _groundCheckSize, 0, _groundLayer) && !jumping;
        if (OnGround) {
            LastOnGroundTime = coyoteTime;
        }
        
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !jumping;
    }   

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(transform.position + _groundCheckPoint, _groundCheckSize);
    }

}
