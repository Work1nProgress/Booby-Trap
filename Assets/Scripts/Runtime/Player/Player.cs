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
    float offsetHorizontal, offsetUp, offsetDown;



    bool m_SpearButtonPressed = false;


    float m_AttackTimer;

    float m_ThrowTimer;
    float m_RegenTimer;
    int m_CurrentSpearAmount;


    float inputY;


    bool CanThrowSpear => (m_CurrentSpearAmount > 0 || m_MaxSpearsOnPlayer < 0) && (m_ThrowTimer < 0 || m_ThrowCooldown == 0);



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


            var slash = PoolManager.Spawn<Slash>("Slash", null, transform.position + new Vector3(offsetHorizontal * direction, 0, 0), Quaternion.Euler(0, 0, -90));
            slash.transform.localScale = new Vector3(1, direction, 1);

        }
        else if (inputY > 0)
        {
            int direction = m_MovementController.FacingRight ? 1 : -1;
            hit = Physics2D.OverlapBox(transform.position + new Vector3(0, offsetUp, 0), m_UpRange, 0, LayerMask.GetMask("Enemy"));
           

            var slash = PoolManager.Spawn<Slash>("Slash", null, transform.position + new Vector3(0,offsetUp, 0),  Quaternion.Euler(0,0,0));
            slash.transform.localScale = new Vector3(direction, 1, 1);
        }
        if (hit != null)
        {
            var entity = hit.gameObject.GetComponent<EntityBase>();
            entity.Damage(m_Damage);
        }
        m_AttackTimer = m_ReloadTime;
    }

    void OnVertical(float value)
    {
        inputY = value;
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

   
}
