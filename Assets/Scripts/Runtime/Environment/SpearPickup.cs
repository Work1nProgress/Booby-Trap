using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearPickup : Pickup
{

    protected override void OnPickedUp()
    {
        base.OnPickedUp();
        ControllerGame.Instance.PickupSpear();
    }


}
