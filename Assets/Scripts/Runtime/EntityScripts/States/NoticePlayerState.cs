using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NoticePlayer", menuName = "Entities/States/Notice Player")]
public class NoticePlayerState : EntityState
{


    public override void EnterState()
    {
        SoundManager.Instance.Play(_controller.Sound.NoticePlayer, _controller.transform);
        base.EnterState();
    }
}
