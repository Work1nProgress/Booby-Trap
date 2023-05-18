using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    #region variable declaration
    [Header("Player Stats")]
    public int startingHealth = 10;

    [Header("Basic Attack")]
    [SerializeField] float weaponRadius = 0.1f;
    [SerializeField] uint weaponDamage = 1;
    [SerializeField] float attackRange = 1.0f;
    [SerializeField]
    private Vector2 attackDirection = Vector2.right;    

    [Header("Spear Throw")]
    public GameObject spearPrefab;
    [SerializeField] KeyCode throwKey = KeyCode.F;
    [SerializeField] float spawnOffsetDistance;
    
    // private varialbes
    private bool canAttack = true;
    private int health;
    private GameObject thrownSpear = null;

    // components
    Animator animator;    

    // singleton classes
    //GameManager gameManager;
    HUD gui;
    PlayerSound sound;   

    private SpriteRenderer sprite;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //gameManager = GameManager.Instance;
        gui = HUD.Instance;
        sound = GetComponent<PlayerSound>();

        health = startingHealth;
        //ammo = startingAmmo;

        //aimingLine = GetComponentInChildren<LineRenderer>();
        animator = GetComponent<Animator>();

        //gunPivot.gameObject.SetActive(false);

        //if(!followCursorPosition)
        Cursor.lockState = CursorLockMode.Locked;

        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!gameManager.GameRunning)
            //return;

        UpdateAttackInput();

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
    
    private void UpdateAttackInput()
    {    
        float verticalInput = Input.GetAxis("Vertical");
        animator.SetFloat("vInput", verticalInput);

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
            if (Input.GetButtonDown("Fire1"))
            {
                animator.SetTrigger("attack");
                canAttack = false;
            }

            if (Input.GetKeyDown(throwKey))
            {
                if (spearPrefab != null)
                {
                    ThrowSpear(attackDirection);
                }
            }
        }
    }

    private void ThrowSpear(Vector2 direction)
    {
        if(thrownSpear != null)
            GameObject.Destroy(thrownSpear);

        animator.SetTrigger("Throw"); // plays shooting animation
        Vector2 spawnPosition = (Vector2)transform.position + direction * spawnOffsetDistance;
        thrownSpear = Instantiate(spearPrefab, spawnPosition, Quaternion.Euler(direction));
        thrownSpear.GetComponent<Spear>().Direction = direction;
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
        if(collision.gameObject.layer == 9) // enemy projectile layer
        {
            TakeDamage(1);            
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
