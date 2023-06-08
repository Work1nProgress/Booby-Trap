using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticHazard : MonoBehaviour
{
    [SerializeField]
    int DamageToDeal;



    int EnemyLayer;
    private void Awake()
    {
        EnemyLayer = LayerMask.GetMask("Enemy");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Collide(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collide(collision.gameObject);
    }


    private void Collide(GameObject go)
    {
        EnemyBase enemy = go.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.Damage(10000);
        }

        if (go.layer == Utils.PlayerCollisionLayer)
        {
            ControllerGame.Instance.player.TeleportToLastGround();
            ControllerGame.Instance.player.Damage(DamageToDeal);
        }
       
    }
}

public enum HazardShape {

    Square,
    Circle,

}
