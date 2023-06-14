using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SavePoint : MonoBehaviour
{

    [SerializeField]
    int RoomID;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    SpriteAnimator animator;

    [SerializeField]
    Sprite staticSprite;

    private void Start()
    {
        if (ControllerGame.Instance.SavedRoom != RoomID)
        {
            spriteRenderer.sprite = staticSprite;

        }
        else
        {
            animator.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (ControllerGame.Instance.SavedRoom != RoomID)
        {
            if (collision.gameObject.layer == Utils.PlayerLayer)
            {
                SoundManager.Instance.Play("Save_Station", ControllerGame.Instance.player.transform);
                ControllerGame.Instance.SaveRoom(RoomID);
                ControllerGame.Instance.player.FreezeOnTransition(true);
                ControllerGame.Instance.player.transform.position = transform.position - new Vector3(0, 0.123f, 0);
                ControllerGame.Instance.player.TriggerTyping();
                DOVirtual.DelayedCall(3, () => ControllerGame.Instance.player.FreezeOnTransition(false));
                animator.enabled = true;
            }
        }
    }

}
