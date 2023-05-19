using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    #region variable declaration
    [Header("Player Stats")]
    public int startingHealth = 10;

    [Header("Player Input")]
    [SerializeField] string jumpButton = "Jump";
    [SerializeField] string basicAttackButton = "Fire1";
    [SerializeField] string dashingThrustButton = "Fire2";
    [SerializeField] string spearThrowControllerAxis = "9th";
    [SerializeField] KeyCode spearThrowKey = KeyCode.F;

    [Header("Basic Attack")]
    [SerializeField] float weaponRadius = 0.1f;
    [SerializeField] uint weaponDamage = 1;
    [SerializeField] float attackRange = 1.0f;
    [SerializeField]
    private Vector2 attackDirection = Vector2.right;    

    [Header("Spear Throw")]
    public GameObject spearPrefab;    
    [SerializeField] float spawnOffsetDistance;
    [Tooltip("How long the collider of the spehere will be disabled when dropping through it")]
    [SerializeField] float fallThroughTime = 1.0f;

    // private varialbes
    private bool canAttack = true;
    private int health;
    private GameObject thrownSpear = null;

    private bool onSpear = true;

    // components
    Animator animator;    

    // singleton classes
    HUD gui;
    PlayerSound sound;

    public UnityEvent onJumpInput, OnDash, onSpearThrow;

    private SpriteRenderer sprite;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        gui = HUD.Instance;
        sound = GetComponent<PlayerSound>();

        health = startingHealth;
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;

        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!gameManager.GameRunning)
            //return;

        UpdateInput();

        /*
        // up pressed while standing infront of a portal
        if (Input.GetAxis("Vertical") > 0 && portal)
        {
            sound.PlaySound(sound.enterPotalSound);
            portal.EnterPortal();
        }

        /*
        if (transform.position.y < gameManager.minYPosition)
        {
            gameManager.EndGame();
        }*/
    }
    
    private void UpdateInput()
    {    
        float verticalInput = Input.GetAxis("Vertical");
        animator.SetFloat("vInput", verticalInput);

        // drop through spear input
        if (verticalInput < 0 && Input.GetButtonDown(jumpButton))
        {
            if(onSpear)
            {
                Spear spear = thrownSpear.GetComponent<Spear>();
                StartCoroutine(spear.DisabaleCollider(fallThroughTime));
            }
        }
        else if (Input.GetButtonDown(jumpButton))
            onJumpInput.Invoke();

        // update attack direction
        if (verticalInput > 0)
        {
            attackDirection = Vector2.up;
            //animator.SetBool("verticalAttack", true);
        }
        else if (verticalInput < 0)
        {
            attackDirection = Vector2.down;
            //animator.SetBool("verticalAttack", true);
        }
        else
        {
            animator.SetBool("verticalAttack", false);

            if (sprite.flipX) // facing left
                attackDirection = Vector2.left;
            else
                attackDirection = Vector2.right;
        }

        if (canAttack)
        {
            if (Input.GetButtonDown(basicAttackButton))
            {
                animator.SetTrigger("attack");
                canAttack = false;
            }

            if (Input.GetKeyDown(spearThrowKey))
            {
                if (spearPrefab != null)
                {
                    onSpearThrow.Invoke();
                }
            }
        }

        if(Input.GetButtonDown(dashingThrustButton) || Input.GetKeyDown(KeyCode.K))
        {
            OnDash.Invoke();
        }

        
    }

    public void SpawnSpear()
    {
        if(thrownSpear != null)
            GameObject.Destroy(thrownSpear);

        animator.SetTrigger("throw"); // plays shooting animation
        Vector2 spawnPosition = (Vector2)transform.position + attackDirection * spawnOffsetDistance;
        thrownSpear = Instantiate(spearPrefab, spawnPosition, Quaternion.Euler(attackDirection));
        thrownSpear.GetComponent<Spear>().Direction = attackDirection;
    }

    // Invoked from animation even on attack animation
    public void PerformMeleeAttack()
    {
        if (thrownSpear != null)
            GameObject.Destroy(thrownSpear);

        var collisions = Physics2D.CircleCastAll(transform.position, weaponRadius, attackDirection, attackRange);

        foreach (var hit in collisions)
        {
            var enemy = hit.transform.GetComponent<EnemyController>();

            if (enemy != null)
            {
                enemy.TakeDamage(weaponDamage);
            }
        }        
    }

    // Invoked from animation even on attack animation
    public void AttackFinished()
    {
        canAttack = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == thrownSpear)
        {
            //Debug.Log("You have landed on top of your spear");
            onSpear = true;
        }

        if(collision.gameObject.layer == 9) // enemy projectile layer
        {
            TakeDamage(1);            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == thrownSpear)
        {
            //Debug.Log("You have left the spear");
            onSpear = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ammo")
        {
            //AddAmmo(ammoPerPickup);
            GameObject.Destroy(collision.gameObject);
        }

        if (collision.gameObject.layer == 9) // enemy projectile
        {
            TakeDamage(1);
        }

        /*
        if(collision.gameObject.layer == 12) // portal layer
        {           
            portal = collision.GetComponent<Portal>();
        }*/

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        /*
        if (collision.gameObject.layer == 12) // portal layer
        {            
            portal = null;
        }*/
    }

    private void TakeDamage(int amount)
    {
        health -= amount;

        //Debug.Log("Took " + amount + " damage. Health Left: " + health);
        gui.SetHealthText(health);
        sound.PlaySound(sound.damagedSound);

        if (health <=0)
        {
            //gameManager.EndGame();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + ((Vector3)attackDirection * attackRange), weaponRadius);
    }

}
