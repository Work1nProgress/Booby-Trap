using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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


    public Vector2 GetRoomSize => RoomSize;
    public Vector2 GetRoomPosition => RoomPosition;



    [Header("Damage")]
    [SerializeField]
    int ContactDamage = 1;


    [SerializeField]
    DaddyPhase[] Phases;

    int currentPhase = 0;



    DaddyAttack[] _DaddyAttack;


    DaddyAttack _CurrentAttack;

    int directionToPlayer;
    public int facingDirection = 1;

    [SerializeField]
    TMP_Text DebugText;

    public int DaddyMaxHealth = 150;
    float WaitTimer = 2;

    



    private void Start()
    {
        phaseChange = true;
        Init(new EntityStats
        {
            MaxHealth = DaddyMaxHealth

        });
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



            _CurrentAttack = null;
            return;
        }
        _CurrentAttack = ChoseNextAttack();
        _CurrentAttack.BeginAttack();

    }


    DaddyAttack ChoseNextAttack()
    {
        var tmpAttacks = new List<DaddyAttack>();
        var weights = new List<int>();
        int weightAll = 0;
        foreach (var attack in _DaddyAttack)
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
                return _DaddyAttack[i];
            }
        }
        return _DaddyAttack[_DaddyAttack.Length - 1];

    }

    private void FixedUpdate()
    {

        WaitTimer -= Time.fixedDeltaTime;

        if (WaitTimer > 0)
        {
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
    public override void Damage(int ammount)
    {

        
        base.Damage(ammount);
        if (currentPhase >= Phases.Length - 1)
        {
            return;
        }
        var currentHealthPercent = (float)Health / MaxHealth;
        if (currentHealthPercent <= Phases[currentPhase + 1].HealthPercent)
        {

            //go to next phase
            currentPhase++;
            phaseChange = true;
          
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
        Debug.Log(facingDirection);
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
        _DaddyAttack = new DaddyAttack[Phases[currentPhase].DaddyAttacks.Count];
        for (int i = 0; i < Phases[currentPhase].DaddyAttacks.Count; i++)
        {
            _DaddyAttack[i] = Instantiate(Phases[currentPhase].DaddyAttacks[i]);
            _DaddyAttack[i].Init(this);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == Utils.PlayerLayer)
        {
            Debug.Log("Daddy hugged you");
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
        _health = _maxHealth;
        currentPhase = 0;
        phaseChange = true;
    }
}

[System.Serializable]
public class DaddyPhase
{
    public float HealthPercent;
    public float BeginPhaseWaitTime;
    public List<DaddyAttack> DaddyAttacks;

}





