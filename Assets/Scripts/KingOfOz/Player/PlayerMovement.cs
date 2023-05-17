using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField][Range(1, 20)] 
    float runSpeed = 10;    

    [Header("Jumping and falling")]
    [SerializeField] //[Range(1, 100)]
    float maxJumpForce = 10;
    [SerializeField]//[Range(0, 10)] 

    float jumpIncreaseRate = 1;
        [SerializeField][Range(0, 10)] 
    float gravityMultiplier = 1;
    [SerializeField] ForceMode2D forceType = ForceMode2D.Impulse;     

    [Header("Dash Attack")]
    [SerializeField] float dashForce = 10;

    [Header("Vault Attack")]
    [SerializeField] float forwardForce = 10;
    [SerializeField] float upwardForce = 20;

    /*
    [Header("Climbing")]
    public float climbSpeed = 1;
    [Range(0, 0.5f)]
    public float toleranceFromCenter = 0.1f;*/

    [Header("Layers")]
    public LayerMask groundLayer;

    // raycasts
    [Header("Ground Checks")]
    [SerializeField]
    private Vector2 leftOffset = new Vector2(-0.4f, -1);
    [SerializeField]
    private Vector2 rightOffset = new Vector2(0.4f, -1);
    [SerializeField]
    private float castDistance = 0.2f;

    [Header("Debugging")]
    [SerializeField] Vector2 velocity;
    [SerializeField] float jumpForce = 0;

    // components
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;

    private PlayerController controller;
    private PlayerSound sound;    

    private bool onGround = false;
    private float inputX, inputY;
    private bool jumping = false;
    private bool jumpPressed = false;
    private Vector2 jumpPoint;

    private bool falling = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        controller = GetComponent<PlayerController>();
        sound = GetComponent<PlayerSound>();
    }

    // Update is called once per frame
    void Update()
    {
        jumpPressed = Input.GetButtonDown ("Jump");
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        // ground check
        onGround = PlayerOnGround();
   
        if (body.velocity.y <= 0)
        {
            jumping = false;

            if (!onGround)
            {
                jumpForce = 0;
                falling = true;
            }
        }

        /*
        if (onGround)
        {
            if (!jumping && PlayerOnEdge())
                body.gravityScale = 0;
            else
                body.gravityScale = gravityMultiplier;
        }*/

        HorizontalMovement();
        VerticalMovement();

        if (onGround && body.velocity.x != 0)
            animator.SetBool("running", true);
        else if (!onGround || body.velocity.x == 0)
            animator.SetBool("running", false);

        //Debug.Log("Velocity: " + body.velocity);

        // switch animation if falling
        animator.SetFloat("yVelocity", body.velocity.y);
        animator.SetBool("onGround", onGround);

        velocity = body.velocity;
    }

    private void HorizontalMovement()
    {
        // horizontal movement
        if (inputX < 0)
            body.velocity =new Vector2(-runSpeed, body.velocity.y);
        else if (inputX > 0)
            body.velocity = new Vector2(runSpeed, body.velocity.y);
        else if (onGround)
            body.velocity = new Vector2(0, body.velocity.y);
    }

    private void VerticalMovement()
    {             
        if (jumpPressed && onGround)
        {
            if (!jumping)
            {
                animator.SetTrigger("jump");
                //Debug.Log("Sent jump trigger");

                if(sound!= null)
                   sound.PlaySound(sound.jumpSound);
            }

            jumping = true;
            //jumpPoint = transform.position;
            body.gravityScale = gravityMultiplier;

            body.AddForce(Vector2.up * jumpForce, forceType);

            //body.velocity = new Vector2(body.velocity.x, jumpForce);
        }

        // build up force while jump is held down
        if (jumping && Input.GetButton("Jump"))
        {
            if (jumpForce < maxJumpForce)
                 jumpForce += jumpIncreaseRate;
        }


        /*
        if (jumping && Vector2.Distance(transform.position, jumpPoint) > maxJumpHeight)
        {
            Debug.Log("Hit max jump height");
            body.velocity = new Vector2(body.velocity.x, Physics.gravity.y);            
        }*/

    }


    private void LateUpdate()
    {
        // update the face direction
        if (body.velocity.x > 0) // facing right
            sprite.flipX = false;

        if (body.velocity.x < 0) // facing left
            sprite.flipX = true;        
    }

    private bool PlayerOnGround()
    {
        RaycastHit2D leftCheck = Physics2D.Raycast((Vector2)transform.position + leftOffset, Vector2.down, castDistance, groundLayer);
        RaycastHit2D rightCheck = Physics2D.Raycast((Vector2)transform.position + rightOffset, Vector2.down, castDistance, groundLayer);

        //Debug.DrawRay((Vector2)transform.position + rightOffset, Vector2.down * castDistance, Color.yellow);

        if (leftCheck || rightCheck)
            return true;

        return false;
    }
    
    private bool PlayerOnEdge()
    {        
        RaycastHit2D middleCheck = Physics2D.Raycast((Vector2)transform.position + Vector2.down, Vector2.down, 0.2f, groundLayer);

        //Debug.DrawRay((Vector2)transform.position + Vector2.down, Vector2.down * 0.2f, Color.yellow);
               
        if (!middleCheck)
            return true;        

        return false;            
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 && falling) // ground layer
        {
            if(sound != null)
                sound.PlaySound(sound.landSound);

            falling = false;
            jumping = false;
            jumpForce = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
            body.velocity = Vector2.zero;
    }

    /*
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11) // climbable layer
        {            
            var collider = collision.GetComponent<TilemapCollider2D>();
            Vector2 collisionPos = collider.ClosestPoint(transform.position);            

            // check if infront of the collider range
            if (transform.position.x <= collisionPos.x + toleranceFromCenter &&
                transform.position.x >= collisionPos.x - toleranceFromCenter)
            {
                if (inputX == 0)
                    body.velocity = new Vector2(0, body.velocity.y);

                body.gravityScale = 0;
                body.velocity = new Vector2(body.velocity.x, inputY * climbSpeed);
            }
        }
    }*/

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11) // climbable layer
        {
            body.gravityScale = gravityMultiplier;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay((Vector2) transform.position + leftOffset, Vector2.down * castDistance);
        Gizmos.DrawRay((Vector2) transform.position + rightOffset, Vector2.down * castDistance);
    }

}
