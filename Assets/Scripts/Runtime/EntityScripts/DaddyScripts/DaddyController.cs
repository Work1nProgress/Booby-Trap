using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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






    private void Awake()
    {
        _DaddyAttack = new DaddyAttack[Phases.Length];
        for (int i = 0; i < Phases[currentPhase].DaddyAttacks.Count;i++)
        {
            _DaddyAttack[i] = Instantiate(Phases[currentPhase].DaddyAttacks[i]);
            _DaddyAttack[i].Init(this);
        }
        FaceTowards(1);
        Init(new EntityStats
        {
            MaxHealth = DaddyMaxHealth

        });
    }


  

    public void EndAttack()
    {
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

        if (_CurrentAttack && _CurrentAttack.IsActive) {
            _CurrentAttack.UpdateAttack(Time.fixedDeltaTime);
            DebugText.text = $"HP: {Health}/{DaddyMaxHealth}\n"+_CurrentAttack.GetDebugMessage(); 
        } else if(_CurrentAttack == null){

            EndAttack();
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




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
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
        _health = _maxHealth;
    }
}

[System.Serializable]
public class DaddyPhase
{
    public float HealthPercent;
    public List<DaddyAttack> DaddyAttacks;

}





