using UnityEngine;

public class UnicornBot : EnemyBase, ILineOfSightEntity
{
    [SerializeField] private float trotSpeed = 1f;
    [SerializeField] private float gallopSpeed = 2f;
    [SerializeField] private float enrageDuration = 2f;
    private int groundLayer;

    [SerializeField]
    Vector2 groundCheckPoint;

    [SerializeField]
    Vector2 wallCheckPoint;
    private float _enrageTimer;
    private bool _enraged;
    private Rigidbody2D _rigidbody2D;




    void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (_enraged)
        {
            _enrageTimer -= Time.deltaTime;
            if (_enrageTimer <= 0)
            {
                _enraged = false;
            }
        }  
    }


    protected override void FixedUpdate()
    {
        var pos2d = new Vector2(transform.position.x, transform.position.y);
        float direction = Mathf.Sign(transform.localScale.x);

        // rotate direction of X compontnet
        var groundCheck = new Vector2(groundCheckPoint.x * direction, groundCheckPoint.y);
        var wallCheck = new Vector2(wallCheckPoint.x * direction, wallCheckPoint.y);



        if (!Physics2D.Raycast(pos2d + groundCheck, Vector2.down, 1f, groundLayer))
        {
            transform.localScale = new Vector2((-Mathf.Sign(_rigidbody2D.velocity.x)), transform.localScale.y);
        }
        else if (Physics2D.Raycast(pos2d + wallCheck * direction, Vector2.right* direction, 0.5f, groundLayer))
        {
            transform.localScale = new Vector2((-Mathf.Sign(_rigidbody2D.velocity.x)), transform.localScale.y);
        }
        var moveSpeed = _enraged ? gallopSpeed : trotSpeed;
        _rigidbody2D.velocity = IsFacingRight() ? new Vector2(moveSpeed, 0f) : new Vector2(-moveSpeed, 0f);
        base.FixedUpdate();
    }

    protected override void OnDrawGizmosSelected()
    {
        float direction = Mathf.Sign(transform.localScale.x);
        var gcp3d = new Vector3(groundCheckPoint.x* direction, groundCheckPoint.y, 0);
        var wcp3d = new Vector3(wallCheckPoint.x* direction, wallCheckPoint.y, 0);
        Gizmos.DrawLine(transform.position + gcp3d, transform.position + gcp3d + Vector3.down);
        Gizmos.DrawLine(transform.position + wcp3d, transform.position + wcp3d + direction*Vector3.right*0.5f);
        base.OnDrawGizmosSelected();
    }

    private bool IsFacingRight()
    {
        return transform.localScale.x > Mathf.Epsilon;
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.layer == groundLayer)
    //    {
    //        transform.localScale = new Vector2((-Mathf.Sign(_rigidbody2D.velocity.x)), transform.localScale.y);
    //    }
    //}
    public void EnteredLOS()
    {
        _enrageTimer = enrageDuration;
        _enraged = true;
    }
}