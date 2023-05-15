using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyController
{    
    [Header("Movement")]
    public float moveSpeed = 1;
    [Tooltip("The rate in which the ship slows down when above player")]
    [Range(0f, 1f)]
    public float dampening = 0.9f;

    [Header("Shooting")]
    public float attackRange = 5;
    public Vector2 bulletSpawnOffset = Vector2.down;

    [Header("Player Detection")]
    public LayerMask playerLayer;
    public float detectionRadius = 5;
    public Vector2 raycastOffset = Vector2.down * 0.5f; 

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if(AbovePlayer())
        {  
            rb.velocity *= dampening;

            if (!hasFired)
            {
                Invoke("FireShot", timeBetweenShots);
                hasFired = true;
            }
        }

        if (transform.position.x > player.position.x + offsetDistance)
        {
            rb.AddForce(Vector2.left * moveSpeed);
        }
        else if (transform.position.x < player.position.x - offsetDistance)
        {
            rb.AddForce(Vector2.right * moveSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        
    }

    private bool AbovePlayer()
    {
        var raycastHit = Physics2D.Raycast((Vector2)transform.position + raycastOffset,
                          Vector2.down, attackRange, playerLayer);

        //raycastHit.point;

        return raycastHit;
    }

    private void FireShot()
    {
        var bullet = Instantiate(bulletPrefab, (Vector2)transform.position + bulletSpawnOffset, Quaternion.identity);
        bullet.GetComponent<Bullet>().Direction = Vector2.down;

        hasFired = false;

        sound.PlaySound(sound.fireSound);
    }

    private new void OnDrawGizmos()
    {
        if (AbovePlayer())
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.white;

        Gizmos.DrawLine((Vector2)transform.position + raycastOffset,
                        (Vector2)transform.position + raycastOffset + Vector2.down * attackRange);
    }
}