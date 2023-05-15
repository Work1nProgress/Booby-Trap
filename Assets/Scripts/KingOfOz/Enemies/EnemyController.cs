using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyController : MonoBehaviour
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
        if (GetComponent<BoxCollider2D>())
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), player.GetComponent<CapsuleCollider2D>());

        if(GetComponent<CapsuleCollider2D>())
            Physics2D.IgnoreCollision(GetComponent<CapsuleCollider2D>(), player.GetComponent<CapsuleCollider2D>());
    }

    void Update()
    {        

    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            gameManager.PlaySound(sound.dieSound);
            GameObject.Destroy(gameObject);            
        }
        else
        {
            sound.PlaySound(sound.damagedSound);
        }
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRaidus);        
    }

}