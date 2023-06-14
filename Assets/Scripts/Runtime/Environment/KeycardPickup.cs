using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardPickup : Pickup
{
    protected override void OnPickedUp()
    {
        base.OnPickedUp();

        ControllerGame.Instance.PickupKeycard(PickupID);
        
    }
}
