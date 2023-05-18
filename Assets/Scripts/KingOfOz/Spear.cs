using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    public float speed = 1;
    [Tooltip("The distance a spear can travel before being destroyed from the scene")]
    public float maxDistance = 100;
    [Tooltip("The sound to play when the the spear collides with something")]
    public AudioClip collisionSound;

    //private GameManager gameManager;
    private Rigidbody2D rb;
    private float distanceTraveled;
    private Vector2 origin;

    public Vector2 Direction { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        origin = transform.position;

        //gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(Direction * speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        //transform.Translate(Direction * speed * Time.fixedDeltaTime);

        distanceTraveled = Vector2.Distance(origin, transform.position);

        if (distanceTraveled > maxDistance)
            GameObject.Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3) // ground layer
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
            gameObject.layer = 3;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int collisionLayer = collision.gameObject.layer;

        if (this.gameObject.layer == 8 && // player projectile layer
            collisionLayer == 7) // enemy layer
        {
            collision.GetComponent<EnemyController>().TakeDamage(1);
            GameObject.Destroy(gameObject);
        }

        //if (collisionLayer == 3) // ground layer
            //gameManager.PlaySound(destroyedSound);

        // destuction layers
        //if(collisionLayer == 3 || collisionLayer == 6 || collisionLayer == 7) 
           // GameObject.Destroy(gameObject);
    }

}