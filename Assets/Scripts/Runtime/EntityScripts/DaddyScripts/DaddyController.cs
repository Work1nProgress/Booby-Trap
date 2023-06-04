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
    DaddyAttackPhase[] Phases;



    DaddyAttackPhase[] _Phases;


    DaddyAttackPhase _CurrentPhase;

    int directionToPlayer;
    public int facingDirection = 1;

    [SerializeField]
    TMP_Text DebugText;

    [SerializeField]
    int DaddyMaxHealth;




    private void Start()
    {
        _Phases = new DaddyAttackPhase[Phases.Length];
        for (int i = 0; i < _Phases.Length;i++)
        {
            _Phases[i] = Instantiate(Phases[i]);
            _Phases[i].Init(this);
        }
        FaceTowards(1);
        Init(new EntityStats
        {
            MaxHealth = DaddyMaxHealth

        });
    }


  

    public void EndPhase()
    {
        _CurrentPhase = ChoseNextPhase();
        _CurrentPhase.BeginPhase();

    }


    DaddyAttackPhase ChoseNextPhase()
    {
        var tmpPhases = new List<DaddyAttackPhase>();
        var weights = new List<int>();
        int weightAll = 0;
        foreach (var phase in _Phases)
        {
            if (CanStartPhase(phase))
            {
                tmpPhases.Add(phase);
                weightAll += phase.Weight;
                weights.Add(weightAll);
            }
        }

        var roll = Random.Range(0, weightAll);
        for (int i = 0; i < weights.Count; i++)
        {
            if (roll < weights[i])
            {
                return _Phases[i];
            }
        }
        return _Phases[_Phases.Length - 1];

    }

    private void FixedUpdate()
    {

        if (_CurrentPhase && _CurrentPhase.IsActive) {
            _CurrentPhase.UpdatePhase(Time.fixedDeltaTime);
            DebugText.text = $"HP: {Health}/{DaddyMaxHealth}\n"+_CurrentPhase.GetDebugMessage(); 
        } else if(_CurrentPhase == null){

            EndPhase();
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


    bool CanStartPhase(DaddyAttackPhase daddyAttackPhase)
    {
        if (daddyAttackPhase.Conditions.HasFlag(DaddyPhaseCondition.PlayerCloserThan))
        {


            return Vector2.Distance(Rigidbody.position, ControllerGame.Instance.player.RigidBody.position) <= daddyAttackPhase.DistanceToPlayer;

        }

        return true;

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
        if (_CurrentPhase != null)
        {
            _CurrentPhase.DrawHitboxes();
        }
    }


}





