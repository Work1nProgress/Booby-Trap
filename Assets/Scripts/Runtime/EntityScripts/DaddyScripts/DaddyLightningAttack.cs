using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LightningAttack", menuName = "Entities/Daddy/Lightning Attack")]
public class DaddyLightningAttack : DaddyAttack
{

    [Header("Lightning Phase")]

    [SerializeField]
    float LightningWidth;

    [SerializeField]
    float LightningSpacing;

    [SerializeField]
    float LightningStartPosition;

    [SerializeField]
    float LightningStartPositionAlternate;

    [SerializeField]
    float TelegraphLightningDuration;

    [SerializeField]
    float LightningDuration;

    [SerializeField]
    float LightningDelay;



    float _loopTimer;
    float _LoopDuration => LightningDelay + TelegraphLightningDuration + LightningDuration;


    //0 delay
    //1 telegraphing
    //2 shooting lightning
    int _loopSegment;
    int _previousLoopSegment;

    //track odd or even loops
    int _loopCounter;

    List<Vector2> lightningCollision = new List<Vector2>();
    Vector2 lightningArea;

    public override void Init(DaddyController daddyController)
    {
        base.Init(daddyController);
        lightningArea =  new Vector2(LightningWidth, _controller.GetRoomSize.y);
    }

    public override void BeginAttack()
    {
        base.BeginAttack();
        _loopSegment = 0;
        _loopCounter = 0;
        _previousLoopSegment = -1;
        _loopTimer = _LoopDuration;
    }

    public override void UpdateAttack(float deltaTime)
    {


        base.UpdateAttack(deltaTime);

        if (_State != DaddyAttackState.Active)
        {
            return;
        }

        _loopTimer -= deltaTime;


        if (_loopTimer > TelegraphLightningDuration + LightningDuration)
        {
            _loopSegment = 0;
        }
        else if (_loopTimer > LightningDuration)
        {
            _loopSegment = 1;
        }
        else if (_loopTimer > 0)
        {
            _loopSegment = 2;
        }
        else
        {
            _loopSegment = 0;
            _loopCounter++;
            _loopTimer = _LoopDuration;
            return;
        }



        //start telegraphing
        if (_previousLoopSegment == 0 && _loopSegment == 1)
        {
            float currentPosition = (_loopCounter % 2 == 0 ? LightningStartPosition : LightningStartPositionAlternate) + _controller.GetRoomPosition.x;
            while (currentPosition < _controller.GetRoomPosition.x +_controller.GetRoomSize.x)
            {

                //spawn at every other tile
                var telegraph = PoolManager.Spawn<PoolObjectTimed>("LightningTelegraph",
                    null,
                    new Vector3(
                        currentPosition,
                        _controller.GetRoomSize.y + _controller.GetRoomPosition.y,
                        0)
                    );
                telegraph.transform.localScale = new Vector3(LightningWidth, 1, 0);
                telegraph.StartTicking(TelegraphLightningDuration);
                currentPosition += LightningSpacing + LightningWidth;
            }

        }
        //shoot
        else if (_previousLoopSegment == 1 && _loopSegment == 2)
        {
            lightningCollision.Clear();
            float currentPosition = (_loopCounter % 2 == 0 ? LightningStartPosition : LightningStartPositionAlternate) + _controller.GetRoomPosition.x;
            while (currentPosition < _controller.GetRoomSize.x)
            {
                var lightning = PoolManager.Spawn<PoolObjectTimed>("Lightning",
                    null,
                    new Vector3(
                        currentPosition,
                        _controller.GetRoomSize.y / 2+ _controller.GetRoomPosition.y,
                        0)
                    );
                lightning.transform.localScale = new Vector3(LightningWidth, _controller.GetRoomSize.y, 0);
                lightning.StartTicking(LightningDuration);
                lightningCollision.Add(new Vector2(currentPosition, _controller.GetRoomSize.y / 2 + _controller.GetRoomPosition.y));
                currentPosition += LightningSpacing+ LightningWidth;
            }
        }

        if (_loopSegment == 2)
        {
         
            foreach (var lightning in lightningCollision)
            {
                if (Physics2D.OverlapBox(lightning, lightningArea, 0, Utils.PlayerLayerMask))
                {
                    ControllerGame.Instance.player.Damage(DamageToPlayer);
                    break;
                }
            }
        }
        _previousLoopSegment = _loopSegment;
    }

    public override void DrawHitboxes()
    {
        base.DrawHitboxes();
        if (_loopSegment == 2 && _State == DaddyAttackState.Active)
        {

            foreach (var lightning in lightningCollision)
            {
                Gizmos.DrawWireCube(lightning, lightningArea);
            }
            
        }
    }




}
