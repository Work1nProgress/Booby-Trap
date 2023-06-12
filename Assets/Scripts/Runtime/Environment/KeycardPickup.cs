using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardPickup : MonoBehaviour
{
    [SerializeField] string _keyString;

    private void FixedUpdate()
    {
        CheckForEcho();
    }

    private void CheckForEcho()
    {
        Collider2D collider;
        collider = Physics2D.OverlapCircle(transform.position, 0.2f, LayerMask.GetMask("EchoCollisionBox"));

        if(collider != null)
        {
            PlayerKeycardContainer kkc = collider.GetComponent<PlayerKeycardContainer>();
            if (kkc != null)
            {
                kkc.Add(_keyString);
                Destroy(gameObject);
            }
        }
    }
}
