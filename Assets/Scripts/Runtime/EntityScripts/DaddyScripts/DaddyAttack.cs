using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class DaddyAttack : ScriptableObject
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
    DaddyAttackTrigger m_DaddyAttackTrigger;

    [SerializeField]
    float m_DamageThreshold;

    [SerializeField]
    bool Teleport;

    [SerializeField]
    float TeleportTime;

    public int AnimatorTrigger;


    [Header("Conditions")]
    [SerializeField]
    DaddyAttackCondition m_DaddyAttackCondition;

    public DaddyAttackCondition Conditions => m_DaddyAttackCondition;

    [SerializeField]
    float m_PlayerCloserThanTiles;
    public float DistanceToPlayer => m_PlayerCloserThanTiles;

    [SerializeField]
    int m_Weight;


    [Tooltip("Tile from where to start the attack. If empty starts at current tile otherwise teleports to a tile.")]
    [SerializeField]
    int[] StartTile;

    [Tooltip("Height from where to start the attack. If empty starts at current tile otherwise teleports to a tile.")]
    int[] StartHeight;

    public int Weight => m_Weight;








    int _currentAttackDamage;

    protected DaddyAttackState _State = DaddyAttackState.None;


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

    public virtual void BeginAttack()
    {
        _currentTime = 0;
        hasSentOnTeleport = false;
        _waitOneFrame = true;
        _IsActive = true;
        _currentAttackDamage = 0;
        _State = DaddyAttackState.None;
        _teleported = false;
        TeleportPosition = default;
        if (StartTile.Length > 0 && !Teleport)
        {

            _controller.GoToTile(StartTile[Random.Range(0, StartTile.Length)]);
            TeleportPosition = _controller.Rigidbody.position;
            _controller.SetTrigger(AnimatorTrigger);

        }
        else if (Teleport)
        {


            TeleportPosition = _controller.GetRoomPosition + Utils.TileToWorldPosition(StartTile[Random.Range(0, StartTile.Length)], 1);
            if (Vector2.Distance(_controller.Rigidbody.position, TeleportPosition) < 1.1)
            {
                _teleported = true;
                hasSentOnTeleport = true;
                OnTeleport();
            }
        }
        else
        {
            //edge case for slash attack
            DOVirtual.DelayedCall(1.5f, () => _controller.SetTrigger(AnimatorTrigger));
        }
    }
    public virtual void ExitAttack()
    {
        _IsActive = false;
        _controller.EndAttack();
    }

    bool _waitOneFrame;
    bool _teleported;
    bool hasSentOnTeleport;

    public virtual void UpdateAttack(float deltaTime)
    {

        if (_waitOneFrame)
        {
            _waitOneFrame = false;
            return;
        }
        if (_State == DaddyAttackState.None)
        {
          
            StartTelegraph();
            return;
        }


        if (_State == DaddyAttackState.Telegraph && Teleport && !_teleported)
        {

            if (_currentTime > TeleportTime)
            {


                _controller.GoToTile(Utils.WorldPositionToTile(TeleportPosition.x -_controller.GetRoomPosition.x));
                _waitOneFrame = true;
                
                _teleported = true;
            
               
            }
        }
        else if (_State == DaddyAttackState.Telegraph && Teleport && _teleported && !hasSentOnTeleport) {
            hasSentOnTeleport = true;
            SoundManager.Instance.Play(_controller.Sound.TeleportOut, _controller.transform);
            OnTeleport();
        }
        




            _currentTime += deltaTime;
        
    }

    protected virtual void OnTeleport()
    {
        _controller.SetTrigger(AnimatorTrigger);
    }


    #endregion




    #region State changes

    protected virtual void StartTelegraph()
    {
        _currentTime = 0;
        _State = DaddyAttackState.Telegraph;
       


        if (m_TelegraphTime > 0)
        {
            _StateCountdown = new CountdownTimer(m_TelegraphTime, false, false, OnEndTelegraph);
            if (Teleport && !_teleported)
            {
                var tp = PoolManager.Spawn<CollapsingWall>("teleportPortal", null, new Vector3(TeleportPosition.x, TeleportPosition.y-0.4f, 0));
                tp.StartTicking(TeleportTime+0.16f);
                SoundManager.Instance.Play(_controller.Sound.TeleportIn, tp.transform);
                _controller.SetTeleportTrigger();
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
        _State = DaddyAttackState.Active;

       
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
        _State = DaddyAttackState.Cooldown;
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

       _State = DaddyAttackState.None;
       ExitAttack();          
        

    }


    #endregion


    public void AddDamage(int damage)
    {

        _currentAttackDamage += damage;
        if (m_DaddyAttackTrigger.HasFlag(DaddyAttackTrigger.DamageThreshold))
        {
            if (_currentAttackDamage >= m_DamageThreshold)
            {
                StartCooldown();
            }
        }
    }

   

    #region DEBUG

    public string GetDebugMessage()
    {
        var msg = $"Attack: {name.Split("(")[0]}\n" +
            $"State: {_State}";

        if (m_DaddyAttackTrigger.HasFlag(DaddyAttackTrigger.DamageThreshold))
        {
            msg += $"\n Damage {_currentAttackDamage}/{m_DamageThreshold}";
        }
        if (m_DaddyAttackTrigger.HasFlag(DaddyAttackTrigger.Timer))
        {
            msg += $"\n Time {string.Format("{0:0.00}", _currentTime)}/{GetTime()}";
        }
        return msg;

    }

    float GetTime() =>


        _State switch
        {
            DaddyAttackState.Telegraph => m_TelegraphTime,
            DaddyAttackState.Active => m_ActiveTime,
            DaddyAttackState.Cooldown => m_CooldownTime,
            _ => 0
        };

    public virtual void DrawHitboxes() { Gizmos.color = Color.black; }
    #endregion










}


public enum DaddyAttackState
{
    None,
    Telegraph,
    Active,
    Cooldown
}

[System.Flags]
public enum DaddyAttackTrigger
{
    None,
    Timer,
    DamageThreshold
}

[System.Flags]
public enum DaddyAttackCondition
{
    None,
    PlayerCloserThan
}
