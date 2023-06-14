using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{


    [SerializeField]
    protected string PickupID;

    public string GetPickupID => PickupID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == Utils.PlayerLayer)
        {
            OnPickedUp();
            Destroy(gameObject);

        }
    }
   

    protected virtual void OnPickedUp()
    {
      
    }

   


}
