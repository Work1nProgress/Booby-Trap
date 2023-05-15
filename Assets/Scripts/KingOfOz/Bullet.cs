using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1;    
    public float maxDistance = 100;
    [Tooltip("The sound to play when the the bullet hits something other and a character")]
    public AudioClip destroyedSound;

    private GameManager gameManager;
    private Rigidbody2D rb;
    private float distanceTraveled;
    private Vector2 origin;

    public Vector2 Direction { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        origin = transform.position;

        gameManager = GameManager.Instance;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int collisionLayer = collision.gameObject.layer;

        if (this.gameObject.layer == 8 && // player projectile layer
            collisionLayer == 7) // enemy layer
        {
            collision.GetComponent<EnemyController>().TakeDamage(1);            
        }

        if (collisionLayer == 3)
            gameManager.PlaySound(destroyedSound);

        // destuction layers
        if(collisionLayer == 3 || collisionLayer == 6 || collisionLayer == 7) 
            GameObject.Destroy(gameObject);
    }

}