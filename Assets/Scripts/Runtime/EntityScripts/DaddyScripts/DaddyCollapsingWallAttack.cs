using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[CreateAssetMenu(fileName = "CollapsingWallAttack", menuName = "Entities/Daddy/Collapsing Wall Attack")]
public class DaddyCollapsingWallAttack : DaddyAttack
{
    [SerializeField]
    Vector2 WallStartSize, WallEndSize;

    [SerializeField]
    float WallStartPositionOffset;

    [SerializeField]
    float WallEndPositionOffset;

    [SerializeField]
    Ease WallMovementEase = Ease.Linear;
    [SerializeField]
    Ease WallSizeEase = Ease.Linear;

    [SerializeField]
    Ease AscentMovementEase = Ease.Linear;
    [SerializeField]
    Ease DescentMovementEase = Ease.Linear;


    [SerializeField]
    float HoverHeight;


    [SerializeField]
    Vector2 HoverAmplitude;
    [SerializeField]
    Vector2 HoverFrequency;

    //0 none
    //1 rising
    //2 hovering
    //3 descending
    int hoverState;
    float _hoverTimer;
    float _hoverDuration;

    Vector2 startPos;
    Vector2 hoverPos;

    PoolObjectTimed wallLeft;
    PoolObjectTimed wallRight;

    Vector3 wallLeftPos;
    Vector3 wallRightPos;


    public override void BeginAttack()
    {
        hoverState = 0;
        base.BeginAttack();
        
    }

    public override void UpdateAttack(float deltaTime)
    {
        base.UpdateAttack(deltaTime);

        if (_State == DaddyAttackState.Telegraph && hoverState == 1)
        {

            var pos = Vector2.Lerp(startPos, startPos + new Vector2(0, HoverHeight), DOVirtual.EasedValue(0, 1, _hoverTimer / _hoverDuration, AscentMovementEase));
            _controller.Rigidbody.MovePosition(pos);
            _hoverTimer += deltaTime;
        }

        if (_State == DaddyAttackState.Active && hoverState == 2)
        {

            var t = _hoverTimer / _hoverDuration;
            _controller.Rigidbody.MovePosition(hoverPos + new Vector2(HoverAmplitude.x * Mathf.Sin(t * HoverFrequency.x), HoverAmplitude.y * Mathf.Sin(t * HoverFrequency.y)));
            _hoverTimer += deltaTime;
        }

        if (_State == DaddyAttackState.Cooldown && hoverState == 3)
        {

            var pos = Vector2.Lerp(hoverPos, startPos, DOVirtual.EasedValue(0, 1, _hoverTimer / _hoverDuration, DescentMovementEase));
            _controller.Rigidbody.MovePosition(pos);
            _hoverTimer += deltaTime;
        }

        if (_State == DaddyAttackState.Active)
        {
            MoveWall(wallLeft.transform, _currentTime / m_ActiveTime, leftStartX, leftEndX);
            MoveWall(wallRight.transform, _currentTime / m_ActiveTime, rightStartX, rightEndX);
            if (Physics2D.OverlapBox(new Vector3(wallLeft.transform.position.x, _controller.GetRoomPosition.y + currentSize.y / 2, 0), currentSize, 0, Utils.PlayerLayerMask) || Physics2D.OverlapBox(new Vector3(wallRight.transform.position.x, _controller.GetRoomPosition.y + currentSize.y / 2, 0), currentSize, 0, Utils.PlayerLayerMask))
            {
                ControllerGame.Instance.player.Damage(DamageToPlayer);
            }

            if (wallLeft != null)
            {
                wallLeftPos = wallLeft.transform.position;
            }

            if (wallRight != null)
            {
                wallRightPos = wallRight.transform.position;
            }


        }

    }

    protected override void OnTeleport()
    {
        base.OnTeleport();
        hoverState = 1;
        _hoverDuration = m_TelegraphTime - _currentTime;
        _hoverTimer = 0;
        startPos = _controller.Rigidbody.position;

        wallLeft = SpawnWall(leftStartX);
        wallRight = SpawnWall(rightStartX);

    }

    float leftStartX => _controller.GetRoomPosition.x + WallStartPositionOffset;
    float rightStartX => _controller.GetRoomPosition.x + _controller.GetRoomSize.x - WallStartPositionOffset;

    float leftEndX => _controller.GetRoomPosition.x + WallEndPositionOffset;
    float rightEndX => _controller.GetRoomPosition.x + _controller.GetRoomSize.x - WallEndPositionOffset;
    Vector2 currentSize;
    void MoveWall(Transform wall, float value, float startX, float endX)
    {
        currentSize = Vector2.Lerp(WallStartSize, WallEndSize, DOVirtual.EasedValue(0,1, value, WallSizeEase));
        var y = DOVirtual.EasedValue(_controller.GetRoomPosition.y/1.5f, -6.74f, value, Ease.Linear);
        var x = DOVirtual.EasedValue(startX, endX, value, WallMovementEase);
        wall.transform.position = new Vector3(x, y, 0);

    }

    PoolObjectTimed SpawnWall(float posX)
    {
        var slash = PoolManager.Spawn<CollapsingWall>("CollapsingWall", null, new Vector3(posX, _controller.GetRoomPosition.y/1.5f, 0));


        slash.StartTicking(m_TelegraphTime - _currentTime + m_ActiveTime);

        return slash;
    }

    protected override void StartActive()
    {
        hoverState = 2;
        _hoverDuration = m_ActiveTime;
        _hoverTimer = 0;
        hoverPos = _controller.Rigidbody.position;
        SoundManager.Instance.PlayLooped(_controller.Sound.WallsStart,_controller.gameObject, _controller.transform);
        base.StartActive();
    }

    PoolObjectTimed SpawnVanishDrone(Vector3 position, int direction)
    {
        var droneVanish = PoolManager.Spawn<CollapsingWall>("LigtningDroneVanish", null, position);


        droneVanish.StartTicking(0.4f);
        droneVanish.transform.DOLocalMove(droneVanish.transform.position + new Vector3(1.5f*direction, 0.5f,0), 0.4f).SetEase(Ease.OutCubic);

        return droneVanish;

    }

    protected override void OnEndActive()
    {
        SpawnVanishDrone(wallRightPos + new Vector3(0, 3.014f, 0f), -1);
        SpawnVanishDrone(wallLeftPos+ new Vector3(0, 3.014f, 0f), 1);
        base.OnEndActive();
    }

    protected override void StartCooldown()
    {

        _hoverDuration = m_CooldownTime;
        _hoverTimer = 0;
        hoverState = 3;
        hoverPos = _controller.Rigidbody.position;
        SoundManager.Instance.CancelLoop(_controller.gameObject);
        SoundManager.Instance.Play(_controller.Sound.WallsEnd, _controller.transform);
        base.StartCooldown();
    }


    public override void DrawHitboxes()
    {
        base.DrawHitboxes();
        if (_State == DaddyAttackState.Active)
        {
            Gizmos.DrawWireCube(new Vector3(wallLeft.transform.position.x, _controller.GetRoomPosition.y + currentSize.y/2, 0), new Vector3(currentSize.x, currentSize.y,0));
            Gizmos.DrawWireCube(new Vector3(wallRight.transform.position.x, _controller.GetRoomPosition.y + currentSize.y/2, 0), new Vector3(currentSize.x, currentSize.y, 0));
        }
    }


}
