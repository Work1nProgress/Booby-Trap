using UnityEngine;

public class Player : EntityBase
{
    [Header("Spear")]

    //these settings will be moved somwhere else later (hopefully)
    [SerializeField]
    [Tooltip("Amount of spears in scene before the previous ones dissapear (0 for infinite)")]
    int m_MaxNumberOfSpearsInGame;

    [SerializeField]
    [Tooltip("Amount of spear ammo on player (-1 for infinite)")]
    int m_MaxSpearsOnPlayer;

    [SerializeField]
    [Tooltip("How long until you get spear \"ammo\" (0 for instant)")]
    float m_SpearRegenTime;

    [SerializeField]
    [Tooltip("Need to collect spears with attack")]
    bool SpearCollector;

    [SerializeField]
    [Tooltip("Throw speed (0 for instant)")]
    float m_ThrowCooldown;

    [SerializeField]
    [Tooltip("Spear speed")]
    float m_SpearFlySpeed;

    [SerializeField]
    [Tooltip("How long a spear will survive after being stuck in a wall (big numbers for infinite)")]
    float m_SpearLifetime;

    [SerializeField]
    [Tooltip("Max range (0 for infinite)")]
    float m_SpearRange;

    [SerializeField]
    [Tooltip("Amount of speed the spear should inherit (0 to 1)")]
    [Range(0,1f)]
    float m_InheritSpeed;

    [SerializeField]
    float chanceToGainHeartRanged;
    public float ChanceToGainHeartRanged => chanceToGainHeartRanged;

    [SerializeField]
    PlayerMovementController m_MovementController;


    [Header("Attack")]
    [SerializeField]
    Vector2 m_HorizontalRange;

    [SerializeField]
    Vector2 m_UpRange;

    [SerializeField]
    Vector2 m_DownRange;

    [SerializeField]
    [Tooltip("1/Attack Speed")]
    float m_ReloadTime;

    [SerializeField]
    [Tooltip("Normal Attack Damage")]
    int m_Damage;

    [SerializeField]
    float chanceToGainHeartMelee;

    [SerializeField]
    float offsetHorizontal, offsetUp, offsetDown;

    [SerializeField]
    [Tooltip("IFrames")]
    float InvulTime = 1f;

    [SerializeField]
    float StunEnemyTime = 2f;

    [SerializeField]
    float StunEnemyForce= 20f;





    bool m_SpearButtonPressed = false;


    float m_AttackTimer;

    float m_ThrowTimer;
    float m_RegenTimer;
    float m_InvulTimer;
    int m_CurrentSpearAmount;


    float inputY;

    [SerializeField]
    PlayerMovementController PlayerMovementController;

    public Rigidbody2D RigidBody => PlayerMovementController.RigidBody;


    bool CanThrowSpear => (m_CurrentSpearAmount > 0 || m_MaxSpearsOnPlayer < 0) && (m_ThrowTimer < 0 || m_ThrowCooldown == 0);
    [Header("Sound")]
    [SerializeField]
    string Attack;
    [SerializeField]
    string AttackHit;
    [SerializeField]
    string Throw;
    [SerializeField]
    string ThrowHit;


    private void OnEnable()
    {
        ControllerInput.Instance.Attack.AddListener(OnAttack);
        ControllerInput.Instance.Throw.AddListener(OnThrow);
        ControllerInput.Instance.Vertical.AddListener(OnVertical);
    }

    private void OnDisable()
    {
        ControllerInput.Instance.Attack.RemoveListener(OnAttack);
        ControllerInput.Instance.Throw.RemoveListener(OnThrow);
        ControllerInput.Instance.Vertical.RemoveListener(OnVertical);
    }

    private void Start()
    {
        m_CurrentSpearAmount = m_MaxSpearsOnPlayer;
    }

    private void Update()
    {
        m_AttackTimer -= Time.deltaTime;
        m_ThrowTimer -= Time.deltaTime;
        m_InvulTimer -= Time.deltaTime;

        if (!SpearCollector)
        {
            m_RegenTimer -= Time.deltaTime;
            if (m_RegenTimer <= 0)
            {
                GainSpear();
            }
        }

    }

    void OnThrow()
    {
        if (CanThrowSpear)
        {
           
            // will change this when we add the throwing the spear up and down
            float speed = m_MovementController.RigidBody.velocity.x;
             int direction = m_MovementController.FacingRight ? 1 : -1;

            var hit = Physics2D.Raycast(transform.position, direction * Vector2.right, 0.5f, LayerMask.GetMask("Ground"));


            m_ThrowTimer = m_ThrowCooldown;
            Spear spear;
            if (hit)
            {
                SoundManager.Instance.Play(Throw, transform);
                SoundManager.Instance.Play(ThrowHit, transform);
                var stuckSpear = PoolManager.Spawn<StuckSpear>("StuckSpear", null, new Vector3(hit.point.x, hit.point.y, 0) - direction * Spear.StuckOffset, Quaternion.Euler(0, 0, 90));
                stuckSpear.Init(m_SpearLifetime,
                   direction,  //direction                       
                    ReturnSpear);
                spear = stuckSpear;
            }
            else
            {
               
                var thrownSpear = PoolManager.Spawn<FlyingSpear>("FlyingSpear", null, transform.position + direction * new Vector3(0.5f, 0, 0), Quaternion.Euler(0, 0, 90));
                thrownSpear.Init(m_SpearFlySpeed,
                    m_SpearLifetime,
                    m_SpearRange,
                   speed, //speed of echo
                   direction,  //direction
                    m_InheritSpeed,//amount of inherited speed   
                    ReturnSpear);
                SoundManager.Instance.Play(Throw, thrownSpear.transform);
                spear = thrownSpear;
            }




            if (m_MaxNumberOfSpearsInGame > 0 && ControllerGame.Instance.Spears.Count + 1 >= m_MaxNumberOfSpearsInGame)
            {
                ControllerGame.Instance.RemoveSpear(0);
            }

            ControllerGame.Instance.Spears.Add(spear);
            m_CurrentSpearAmount--;
            m_CurrentSpearAmount = Mathf.Min(m_CurrentSpearAmount + 1, m_MaxSpearsOnPlayer);
        }
    }

    public void AttackForce(Vector3 position, float force)
    {
        if (m_InvulTimer > 0)
        {
            return;
        }
        PlayerMovementController.RigidBody.AddForce((transform.position -position).normalized * force, ForceMode2D.Impulse);
    }

    public override void Damage(int ammount)
    {
        if (m_InvulTimer > 0)
        {
            return;
        }
        m_InvulTimer = InvulTime;
        base.Damage(ammount);
    }

    public void TeleportToLastGround()
    {
        m_MovementController.TeleportToLastGround();
    }

    void OnAttack()
    {
        if (m_AttackTimer > 0)
        {
            return; 
        }
        Collider2D hit = null;
        if (inputY == 0)
        {
            int direction = m_MovementController.FacingRight ? 1 : -1;
            hit = Physics2D.OverlapBox(transform.position + new Vector3(offsetHorizontal * direction, 0, 0), m_HorizontalRange, 0, LayerMask.GetMask("Enemy"));


            var slash = PoolManager.Spawn<Slash>("Slash", transform,default, Quaternion.Euler(0, 0, -90));
            slash.transform.localPosition = new Vector3(offsetHorizontal * direction, 0, 0);
            slash.transform.localScale = new Vector3(1, direction, 1);
            SoundManager.Instance.Play(Attack, slash.transform);

        }
        else if (inputY > 0)
        {
            int direction = m_MovementController.FacingRight ? 1 : -1;
            hit = Physics2D.OverlapBox(transform.position + new Vector3(0, offsetUp, 0), m_UpRange, 0, LayerMask.GetMask("Enemy"));
           

            var slash = PoolManager.Spawn<Slash>("Slash", transform,default,  Quaternion.Euler(0,0,0));
            slash.transform.localPosition = new Vector3(0, offsetUp, 0);
            slash.transform.localScale = new Vector3(direction, 1, 1);
            SoundManager.Instance.Play(Attack, slash.transform);
        }

       

        if (hit != null)
        {
            var entity = hit.gameObject.GetComponent<EntityBase>();
            if (entity != null)
            {
                SoundManager.Instance.Play(AttackHit, entity.transform);
                entity.Damage(m_Damage);
                if (Random.value <= ChanceToGainHeartRanged)
                {
                    Heal(1);
                }

                var enemy = entity as EnemyBase;
                if (enemy != null)
                {
                    enemy.KnockBackAndStun((enemy.Rigidbody.position - RigidBody.position).normalized * StunEnemyForce, StunEnemyTime);
                }
            }
            
        }
        m_AttackTimer = m_ReloadTime;
    }

    void OnVertical(float value)
    {
        inputY = value;
    }

    protected override void OnKill()
    {
       
    }




    void ReturnSpear() {
        if (SpearCollector)
        {
            GainSpear();
        }

    }

    void GainSpear()
    {
        m_CurrentSpearAmount = Mathf.Min(m_CurrentSpearAmount + 1, m_MaxSpearsOnPlayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        int direction = m_MovementController.FacingRight ? 1 : -1;
        Gizmos.DrawWireCube(transform.position + new Vector3(offsetHorizontal * direction, 0, 0), m_HorizontalRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, offsetUp, 0), m_UpRange);
    }


}
