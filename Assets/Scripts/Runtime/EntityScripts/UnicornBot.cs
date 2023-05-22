using UnityEngine;

public class UnicornBot : EntityBase
{
    [SerializeField] private float trotSpeed = 1f;
    [SerializeField] private float gallopSpeed = 2f;
    [SerializeField] private float enrageDuration = 2f;
    [SerializeField] private int groundLayer;
    private float _enrageTimer;
    private bool _enraged;
    private Rigidbody2D _rigidbody2D;

    void Awake()
    {
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
        
        var moveSpeed = _enraged ? gallopSpeed : trotSpeed;
        _rigidbody2D.velocity = IsFacingRight() ? new Vector2(moveSpeed, 0f) : new Vector2(-moveSpeed, 0f);
    }

    private bool IsFacingRight()
    {
        return transform.localScale.x > Mathf.Epsilon;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            transform.localScale = new Vector2((-Mathf.Sign(_rigidbody2D.velocity.x)), transform.localScale.y);
        }
    }

    public void Enrage()
    {
        _enrageTimer = enrageDuration;
        _enraged = true;
    }
}
