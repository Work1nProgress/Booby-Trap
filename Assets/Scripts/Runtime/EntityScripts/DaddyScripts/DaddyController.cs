using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
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

    [SerializeField]
    Light2D daddyLight;

    [SerializeField]
    Light2D roomLight;


    bool saidIntro;
    bool isDead;
    bool isActive;

    Vector3 startPos;
    bool startFlipSprite;


    private void Awake()
    {
        startPos = transform.position;
        startFlipSprite = SpriteRenderer.flipX;
        facingDirection = SpriteRenderer.flipX ? 1 : -1;
    }


    public void StartFight()
    {

      
      
        phaseChange = true;
        Init(new EntityStats
        {
            MaxHealth = DaddyMaxHealth

        });
        bossHealthBar.transform.parent.gameObject.SetActive(true);
        bossHealthBar.RerenderPips(_health, MaxHealth);
        isActive = true;
        DOVirtual.DelayedCall(3f,() =>GetComponent<DaddyMusic>().ResetMusic());

        DOVirtual.Float(0, 1, 1, AnimateLight).SetDelay(3);
    }

    public void CancelFight()
    {
        MusicPlayer.Instance.StopPlaying(0.5f);
        SoundManager.Instance.Play(Sound.EchoDie, transform);
        transform.position = startPos;
        SpriteRenderer.flipX = startFlipSprite;
        facingDirection = SpriteRenderer.flipX ? 1 : -1;
        isActive = false;
        bossHealthBar.transform.parent.gameObject.SetActive(false);
        var mines = FindObjectsOfType<DaddyMine>();
        foreach (var mine in mines)
        {
            mine.Remove();
        }
        _health = _maxHealth;
        currentPhase = 0;
        phaseChange = true;
        saidIntro = false;
        AnimateLight(0);

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
                SoundManager.Instance.Play(Sound.PhaseChange, transform);
                GetComponent<DaddyMusic>().ResetVolume(2f);
            }


            _CurrentAttack = null;
            return;
        }

        _CurrentAttack = ChooseAttack();
       
        switch (_CurrentAttack)
        {
            case DaddyBulldozerAttack daddyBulldozerAttack:
                _CurrentAttack.AnimatorTrigger = Bulldozer;
                ///_animator.SetTrigger(Bulldozer);
                break;
            case DaddyCollapsingWallAttack daddyCollapsingWallAttack:
                _CurrentAttack.AnimatorTrigger = Floating;
                //_animator.SetTrigger(Floating);
                break;
            case DaddyLightningAttack daddyLightningAttack:
                _CurrentAttack.AnimatorTrigger = Lightning;
                //_animator.SetTrigger(Lightning);
                break;
            case DaddyMineAttack daddyMineAttack:
                _CurrentAttack.AnimatorTrigger = Mines;
               // _animator.SetTrigger(Mines);
                break;
            case DaddySlashAttack daddySlashAttack:
                _CurrentAttack.AnimatorTrigger = Slash;
               // Invoke(nameof(SetSlashTrigger), 1.5f);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(_CurrentAttack));
        }
        _CurrentAttack.BeginAttack();
    }

    public void SetTrigger(int triggerHash)
    {
        _animator.SetTrigger(triggerHash);
    }

    void AnimateLight(float value)
    {
        roomLight.intensity = value * 3.18f;
        daddyLight.intensity = value;
    }

    private void SetSlashTrigger()
    {
        _animator.SetTrigger(Slash);
    }

    public void SetTeleportTrigger()
    {
        _animator.SetTrigger(Teleport);
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

        if (isDead || !isActive)
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
          //  DebugText.text = $"Entering Phase {currentPhase+1} in {string.Format("{0:0.00}", WaitTimer)}";
            return;
        }

        if (_CurrentAttack && _CurrentAttack.IsActive) {
            _CurrentAttack.UpdateAttack(Time.fixedDeltaTime);
          //  DebugText.text = $"HP: {Health}/{DaddyMaxHealth}\n"+_CurrentAttack.GetDebugMessage(); 
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
    private static readonly int Teleport = Animator.StringToHash("Teleport out");

    public override void Damage(int amount)
    {

       
      
        base.Damage(amount);
        bossHealthBar.RerenderPips(_health, MaxHealth);
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

        var mines = FindObjectsOfType<DaddyMine>();
        foreach (var mine in mines)
        {
            mine.Remove();
        }
        _health = _maxHealth;
        bossHealthBar.RerenderPips(_health, MaxHealth);
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





