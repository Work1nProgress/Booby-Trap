using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DaddyController : EntityBase
{
    [SerializeField]
    Rigidbody2D Rigidbody2D;

    public Rigidbody2D Rigidbody => Rigidbody2D;

    [SerializeField]
    SpriteRenderer SpriteRenderer;

    [SerializeField]
    Vector2 RoomSize;

    [SerializeField]
    Vector2 RoomPosition;

    [SerializeField] private BossHealthBar bossHealthBar;


    public Vector2 GetRoomSize => RoomSize;
    public Vector2 GetRoomPosition => RoomPosition;



    [Header("Damage")]
    [SerializeField]
    int ContactDamage = 1;

    [SerializeField] private Animator _animator;


    [SerializeField]
    DaddyPhase[] Phases;

    int currentPhase = 0;



    DaddyAttack[] _DaddyAttacks;


    DaddyAttack _CurrentAttack;

    int directionToPlayer;
    public int facingDirection = 1;

    [SerializeField]
    TMP_Text DebugText;

    public int DaddyMaxHealth = 150;
    float WaitTimer = 2;
    
    private DaddyAttack _previousAttack;

    [SerializeField]
    private DaddySound m_Sound;

    public DaddySound Sound => m_Sound;


    bool saidIntro;
    bool isDead;


    private void Awake()
    {
        phaseChange = true;
        Init(new EntityStats
        {
            MaxHealth = DaddyMaxHealth

        });
        bossHealthBar.setMaxHealth(DaddyMaxHealth);
    }



  

    public void EndAttack()
    {

        if (phaseChange)
        {
            phaseChange = false;
            WaitTimer = Phases[currentPhase].BeginPhaseWaitTime;
            InitCurrentPhase();
            FaceTowardsEcho();
            //do some in-between phases stuff here

            if (currentPhase > 0)
            {
                Debug.Log("phase change");
                SoundManager.Instance.Play(Sound.PhaseChange, transform);
            }


            _CurrentAttack = null;
            return;
        }

        _CurrentAttack = ChooseAttack();
        _CurrentAttack.BeginAttack();
        switch (_CurrentAttack)
        {
            case DaddyBulldozerAttack daddyBulldozerAttack:
                _animator.SetTrigger(Bulldozer);
                break;
            case DaddyCollapsingWallAttack daddyCollapsingWallAttack:
                _animator.SetTrigger(Floating);
                break;
            case DaddyLightningAttack daddyLightningAttack:
                _animator.SetTrigger(Lightning);
                break;
            case DaddyMineAttack daddyMineAttack:
                _animator.SetTrigger(Mines);
                break;
            case DaddySlashAttack daddySlashAttack:
                Invoke(nameof(SetSlashTrigger), 2f);
                break;
                
            default:
                throw new ArgumentOutOfRangeException(nameof(_CurrentAttack));
        }
    }

    private void SetSlashTrigger()
    {
        _animator.SetTrigger(Slash);
    }


    DaddyAttack ChooseAttack()
    {
        var possibleAttacks = _DaddyAttacks.ToList();
        possibleAttacks.RemoveAll(at => at == _previousAttack);
        
        var tmpAttacks = new List<DaddyAttack>();
        var weights = new List<int>();
        int weightAll = 0;
        
        foreach (var attack in possibleAttacks)
        {
            if (CanStartAttack(attack))
            {
                tmpAttacks.Add(attack);
                weightAll += attack.Weight;
                weights.Add(weightAll);
            }
        }

        var roll = Random.Range(0, weightAll);
        for (int i = 0; i < weights.Count; i++)
        {
            if (roll < weights[i])
            {
                _previousAttack = possibleAttacks[i];
                return possibleAttacks[i];
            }
        }

        return _DaddyAttacks[_DaddyAttacks.Length - 1];

    }

    private void FixedUpdate()
    {

        if (isDead)
        {
            return;
        }
        WaitTimer -= Time.fixedDeltaTime;

        if (WaitTimer > 0)
        {
            if (currentPhase == 0 && !saidIntro)
            {
                saidIntro = true;
                SoundManager.Instance.Play(Sound.StartBattle, transform);
            }
            DebugText.text = $"Entering Phase {currentPhase+1} in {string.Format("{0:0.00}", WaitTimer)}";
            return;
        }

        if (_CurrentAttack && _CurrentAttack.IsActive) {
            _CurrentAttack.UpdateAttack(Time.fixedDeltaTime);
            DebugText.text = $"HP: {Health}/{DaddyMaxHealth}\n"+_CurrentAttack.GetDebugMessage(); 
        } else if(_CurrentAttack == null){

            EndAttack();
        }
    }
    bool phaseChange =  false;
    private static readonly int Floating = Animator.StringToHash("Floating");
    private static readonly int Bulldozer = Animator.StringToHash("Bulldozer");
    private static readonly int Mines = Animator.StringToHash("Mines");
    private static readonly int Slash = Animator.StringToHash("Slash");
    private static readonly int Lightning = Animator.StringToHash("Lightning");

    public override void Damage(int amount)
    {

       
        bossHealthBar.updateHealth(-amount);
        base.Damage(amount);
        if (currentPhase >= Phases.Length - 1)
        {
            return;
        }
        var currentHealthPercent = (float)Health / MaxHealth;
        if (currentHealthPercent*100 <= Phases[currentPhase + 1].HealthPercent)
        {

            //go to next phase
            currentPhase++;
            phaseChange = true;

        }
        


    }

    protected override void OnKill()
    {
        if (!isDead)
        {
             isDead = true;
            SoundManager.Instance.Play(Sound.Death, transform);
        }

    }


    public void GoToTile(int x)
    {
        Rigidbody2D.MovePosition(Utils.TileToWorldPosition((int)RoomPosition.x + x, (int)RoomPosition.y)+ new Vector2(0,0.5f));

    }

    public int GetTilePosition()
    {
        return Utils.WorldPositionToTile(Rigidbody2D.position.x);

    }

    public void FaceTowards(int direction)
    {
        facingDirection = direction;
        SpriteRenderer.flipX = direction == 1;

    }

    public void FaceTowardsEcho()
    {
 
        facingDirection = (int)Mathf.Sign(ControllerGame.Instance.player.transform.position.x - transform.position.x);
        SpriteRenderer.flipX = facingDirection == 1;

    }


    bool CanStartAttack(DaddyAttack daddyAttackPhase)
    {
        if (daddyAttackPhase.Conditions.HasFlag(DaddyAttackCondition.PlayerCloserThan))
        {
            return Vector2.Distance(Rigidbody.position, ControllerGame.Instance.player.RigidBody.position) <= daddyAttackPhase.DistanceToPlayer;
        }

        return true;

    }

    void InitCurrentPhase()
    {

       
        _DaddyAttacks = new DaddyAttack[Phases[currentPhase].DaddyAttacks.Count];
        for (int i = 0; i < Phases[currentPhase].DaddyAttacks.Count; i++)
        {
            _DaddyAttacks[i] = Instantiate(Phases[currentPhase].DaddyAttacks[i]);
            _DaddyAttacks[i].Init(this);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && ContactDamage > 0)
        {
            ControllerGame.Instance.player.Damage(ContactDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_CurrentAttack != null)
        {
            _CurrentAttack.DrawHitboxes();
        }
    }

    public void ResetDadsHp()
    {
        bossHealthBar.resetHealth();
        _health = _maxHealth;
        currentPhase = 0;
        phaseChange = true;
        saidIntro = false;
    }
}

[System.Serializable]
public class DaddyPhase
{
    public float HealthPercent;
    public float BeginPhaseWaitTime;
    public List<DaddyAttack> DaddyAttacks;

}





