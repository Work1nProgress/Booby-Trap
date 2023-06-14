using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{



    protected override void OnPickedUp()
    {
        base.OnPickedUp();
        ControllerGame.Instance.PickupHealth(PickupID);
    }
}
