using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LightningAttack", menuName = "Entities/Daddy/Lightning Attack")]
public class DaddyLightningPhase : DaddyAttackPhase
{

    [Header("Lightning Phase")]

    [SerializeField]
    int LightningWidth;

    [SerializeField]
    int LightningSpacing;

    [SerializeField]
    int LightningStartTile;

    [SerializeField]
    int LightningStartTileAlternate;

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



    public override void BeginPhase()
    {
        base.BeginPhase();
        _loopSegment = 0;
        _loopCounter = 0;
        _previousLoopSegment = -1;
        _loopTimer = _LoopDuration;
    }

    public override void UpdatePhase(float deltaTime)
    {


        base.UpdatePhase(deltaTime);

        if (_State != DaddyPhaseState.Active)
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
            int currentIndex = _loopCounter % 2 == 0 ? LightningStartTile : LightningStartTileAlternate;
            while (currentIndex < _controller.GetRoomSize.x)
            {

                //spawn at every other tile
                var telegraph = PoolManager.Spawn<PoolObjectTimed>("LightningTelegraph",
                    null,
                    Utils.TileToWorldPosition(
                        (int)_controller.GetRoomPosition.x + currentIndex,
                        (int)(_controller.GetRoomSize.y + _controller.GetRoomPosition.y)
                        ));
                telegraph.transform.localScale = new Vector3(LightningWidth, 1, 0);
                telegraph.StartTicking(TelegraphLightningDuration);
                currentIndex += LightningSpacing + LightningWidth;
            }

        }
        //shoot
        else if (_previousLoopSegment == 1 && _loopSegment == 2)
        {
            int currentIndex = _loopCounter % 2 == 0 ? LightningStartTile : LightningStartTileAlternate;
            while (currentIndex < _controller.GetRoomSize.x)
            {
                var lightning = PoolManager.Spawn<PoolObjectTimed>("Lightning", null, Utils.TileToWorldPosition((int)_controller.GetRoomPosition.x + currentIndex, (int)(_controller.GetRoomSize.y /2+ _controller.GetRoomPosition.y)));
                lightning.transform.localScale = new Vector3(LightningWidth, _controller.GetRoomSize.y, 0);
                lightning.StartTicking(LightningDuration);
                currentIndex += LightningSpacing+ LightningWidth;
            }
        }

        if (_loopSegment == 2)
        {
            //TODO check if echo is standing on the tiles that are zapped by lightning and damage her
        }
        _previousLoopSegment = _loopSegment;
    }




}
