using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public int maxHealth = 5;
    public float visionRaidus = 5.0f;    

    [Header("Enemy Fire")]
    public GameObject bulletPrefab;
    public float timeBetweenShots = 1.0f;
    public float offsetDistance = 1.0f;

    protected int health;
    protected Transform player;
    protected Rigidbody2D rb;
    protected bool hasFired = false;

    protected EnemySound sound;

    private GameManager gameManager;

    protected void Start()
    {
        health = maxHealth;
        player = GameObject.FindWithTag("Player").transform;

        rb = GetComponent<Rigidbody2D>();
        sound = GetComponent<EnemySound>();
        gameManager = GameManager.Instance;

        // ignore collisions between player and enemy
        /*if (GetComponent<BoxCollider2D>())
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), player.GetComponent<CapsuleCollider2D>());

        if(GetComponent<CapsuleCollider2D>())
            Physics2D.IgnoreCollision(GetComponent<CapsuleCollider2D>(), player.GetComponent<CapsuleCollider2D>());*/
    }

    void Update()
    {        

    }

    public void TakeDamage(uint amount)
    {
        health -= (int) amount;

        Debug.Log("enemy took " + amount + " damage. " + health + " health left.");

        if (health <= 0)
        {
            Debug.Log("Enemy is dead");
            //gameManager.PlaySound(sound.dieSound);
            GameObject.Destroy(gameObject);            
        }
        else
        {
            StartCoroutine(FlashDamage());

            if(sound != null)
                sound.PlaySound(sound.damagedSound);
        }        
        
    }

    IEnumerator FlashDamage()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, visionRaidus);        
    }

}