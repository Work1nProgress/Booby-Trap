using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearPickup : Pickup
{

    protected override void OnPickedUp()
    {
        base.OnPickedUp();
        SoundManager.Instance.Play("Upgrade_Spear", ControllerGame.Instance.player.transform);
        ControllerGame.Instance.PickupSpear();
    }


}
