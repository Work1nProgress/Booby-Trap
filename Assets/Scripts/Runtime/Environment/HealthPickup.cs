using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{



    protected override void OnPickedUp()
    {
        base.OnPickedUp();
        SoundManager.Instance.Play("Health_Bottle", ControllerGame.Instance.player.transform);
        ControllerGame.Instance.PickupHealth(PickupID);
    }
}
