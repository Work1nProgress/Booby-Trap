using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DaddyAttackPhase : ScriptableObject
{

    [Header("State Times")]
    [SerializeField]
    protected float m_TelegraphTime;
    [SerializeField]
    protected float m_ActiveTime, m_CooldownTime;

    [SerializeField]
    protected int DamageToPlayer;


    [Header("Triggers")]

    [SerializeField]
    DaddyPhaseTrigger m_DaddyPhaseTrigger;

    [SerializeField]
    float m_DamageThreshold;

    [SerializeField]
    bool Teleport;

    [SerializeField]
    float TeleportTime;


    [Header("Conditions")]
    [SerializeField]
    DaddyPhaseCondition m_DaddyPhaseCondition;

    public DaddyPhaseCondition Conditions => m_DaddyPhaseCondition;

    [SerializeField]
    float m_PlayerCloserThanTiles;
    public float DistanceToPlayer => m_PlayerCloserThanTiles;

    [SerializeField]
    int m_Weight;

    [SerializeField]

    [Tooltip("Tile from where to start the attack. If empty starts at current tile otherwise teleports to a tile.")]
    int[] StartTile;

    public int Weight => m_Weight;




    int _currentPhaseDamage;

    protected DaddyPhaseState _State = DaddyPhaseState.None;


    CountdownTimer _StateCountdown;

    protected DaddyController _controller;

    protected float _currentTime;


    bool _IsActive;
    public bool IsActive => _IsActive;


    protected Vector2 TeleportPosition;



    #region core
    public virtual void Init(DaddyController daddyController)
    {
        _controller = daddyController;

    }

    public virtual void BeginPhase()
    {
        _waitOneFrame = true;
        _IsActive = true;
        _currentPhaseDamage = 0;
        _State = DaddyPhaseState.None;
        _teleported = false;
        TeleportPosition = default;
        if (StartTile.Length > 0 && !Teleport)
        {

            _controller.GoToTile(StartTile[Random.Range(0, StartTile.Length)]);
            TeleportPosition = _controller.Rigidbody.position;

        }
        else if(Teleport)
        {


            TeleportPosition = _controller.GetRoomPosition+ Utils.TileToWorldPosition(StartTile[Random.Range(0, StartTile.Length)], 1);
            if (Vector2.Distance(_controller.Rigidbody.position, TeleportPosition) < 1.1)
            {
                _teleported = true;
                OnTeleport();
            }
        }
    }
    public virtual void ExitPhase()
    {
        _IsActive = false;
        _controller.EndPhase();
    }

    bool _waitOneFrame;
    bool _teleported;
    public virtual void UpdatePhase(float deltaTime)
    {

        if (_waitOneFrame)
        {
            _waitOneFrame = false;
            return;
        }
        if (_State == DaddyPhaseState.None)
        {
          
            StartTelegraph();
            return;
        }


        if (_State == DaddyPhaseState.Telegraph && Teleport && !_teleported)
        {

            if (_currentTime > TeleportTime)
            {

                _waitOneFrame = true;
                _teleported = true;
            
                _controller.GoToTile(Utils.WorldPositionToTile(TeleportPosition.x -_controller.GetRoomPosition.x));
                OnTeleport();
            }
        }


        _currentTime += deltaTime;
        
    }

    protected virtual void OnTeleport()
    {


    }


    #endregion




    #region State changes

    protected virtual void StartTelegraph()
    {
        _currentTime = 0;
        _State = DaddyPhaseState.Telegraph;
       


        if (m_TelegraphTime > 0)
        {
            _StateCountdown = new CountdownTimer(m_TelegraphTime, false, false, OnEndTelegraph);
            if (Teleport && !_teleported)
            {
                var tp = PoolManager.Spawn<PoolObjectTimed>("teleportPortal", null, new Vector3(TeleportPosition.x, TeleportPosition.y, 0));
                tp.StartTicking(TeleportTime);
            }
        }
        else
        {
            OnEndTelegraph();

        }

    }



    protected virtual void OnEndTelegraph()
    {
        StartActive();
    }

    protected virtual void StartActive()
    {
        _currentTime = 0;
        _State = DaddyPhaseState.Active;

       
        if (m_ActiveTime < 0)
        {
           
        }
        else if (m_ActiveTime == 0)
        {
            OnEndActive();

        }
        else
        {
            _StateCountdown = new CountdownTimer(m_ActiveTime, false, false, OnEndActive);
        }

    }

    protected virtual void OnEndActive()
    {

        StartCooldown();
    }

    protected virtual void StartCooldown()
    {
        _currentTime = 0;
        _State = DaddyPhaseState.Cooldown;

        if (m_CooldownTime > 0)
        {
            _StateCountdown = new CountdownTimer(m_CooldownTime, false, false, OnEndCooldown);
        }
        else
        {
            OnEndCooldown();

        }

    }

    protected virtual void OnEndCooldown()
    {

       _State = DaddyPhaseState.None;
       ExitPhase();          
        

    }


    #endregion


    public void AddDamage(int damage)
    {

        _currentPhaseDamage += damage;
        if (m_DaddyPhaseTrigger.HasFlag(DaddyPhaseTrigger.DamageThreshold))
        {
            if (_currentPhaseDamage >= m_DamageThreshold)
            {
                StartCooldown();
            }
        }
    }

   

    #region DEBUG

    public string GetDebugMessage()
    {
        var msg = $"Phase: {name.Split("(")[0]}\n" +
            $"State: {_State}";

        if (m_DaddyPhaseTrigger.HasFlag(DaddyPhaseTrigger.DamageThreshold))
        {
            msg += $"\n Damage {_currentPhaseDamage}/{m_DamageThreshold}";
        }
        if (m_DaddyPhaseTrigger.HasFlag(DaddyPhaseTrigger.Timer))
        {
            msg += $"\n Time {string.Format("{0:0.00}", _currentTime)}/{GetTime()}";
        }
        return msg;

    }

    float GetTime() =>


        _State switch
        {
            DaddyPhaseState.Telegraph => m_TelegraphTime,
            DaddyPhaseState.Active => m_ActiveTime,
            DaddyPhaseState.Cooldown => m_CooldownTime,
            _ => 0
        };

    public virtual void DrawHitboxes() { Gizmos.color = Color.black; }
    #endregion










}


public enum DaddyPhaseState
{
    None,
    Telegraph,
    Active,
    Cooldown
}

[System.Flags]
public enum DaddyPhaseTrigger
{
    None,
    Timer,
    DamageThreshold
}

[System.Flags]
public enum DaddyPhaseCondition
{
    None,
    PlayerCloserThan
}
