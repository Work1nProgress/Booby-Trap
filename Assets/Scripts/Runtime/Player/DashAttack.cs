using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class DashAttack : MonoBehaviour
{
    [SerializeField][Range(1, 1000)]
    float dashSpeed = 500;
    [SerializeField][Range(0, 10)]
    float dashDistance = 10;
    [SerializeField]
    bool useCooldown = true;
    [SerializeField][Range(0, 10)]
    float coolDownTime = 3.0f;
    [SerializeField]
    bool dashReady = true;

    [Header("Wall Detection")]
    [SerializeField] LayerMask detectionLayer;
    [SerializeField] float boxOffsetDistance = 0.5f;
    [SerializeField] Vector2 boxSize = Vector2.one;

    private Vector2 initialPoint;
    private bool dashing = false;
    private Vector2 attackDirection = Vector2.right;
    private float lastDashTime;  

    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (useCooldown)
            dashReady = ((Time.time - lastDashTime) > coolDownTime);
        else
            dashReady = true;

        if (sprite.flipX)
            attackDirection = Vector2.left;
        else
            attackDirection = Vector2.right;

        if (Input.GetButtonDown("Fire2") && dashReady && !dashing)
        {
            initialPoint = transform.position;
            dashing = true;
            lastDashTime = Time.time;
        }

        if (dashing)
        {
            PerformDaskAttack();
        }

        animator.SetBool("dashing", dashing);
    }

    private void PerformDaskAttack()
    {
        var wallCollision = Physics2D.OverlapBox((Vector2)transform.position + attackDirection * boxOffsetDistance, 
                                                 boxSize, 0, detectionLayer);
        var currentDistance = Vector2.Distance(transform.position, initialPoint);

        if (wallCollision != null || currentDistance >= dashDistance)
        {
            dashing = false;
            return;
        }

        body.AddForce(attackDirection * dashSpeed * Time.deltaTime, ForceMode2D.Impulse);      
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2) transform.position + attackDirection * boxOffsetDistance, boxSize);
    }
}
