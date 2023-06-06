using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticHazard : MonoBehaviour
{
    [SerializeField]
    int DamageToDeal;

    [SerializeField]
    float Range;


    [SerializeField]
    HazardShape Shape;


    int PlayerLayer;
    int EnemyLayer;
    private void Awake()
    {
        EnemyLayer = LayerMask.GetMask("Enemy");
    }


    private void FixedUpdate()
    {
        bool isInRange = false;
        EnemyBase enemy;
        switch(Shape)
        {
            case HazardShape.Square:
                isInRange = Physics2D.OverlapBox(transform.position, Vector2.one * Range, 0, Utils.PlayerLayerMask) != null;
                var hit  = Physics2D.OverlapBox(transform.position, Vector2.one * Range, 0, EnemyLayer);
                if (hit)
                {
                   enemy = hit.GetComponent<EnemyBase>();
                   enemy.Damage(10000);
                }
                break;

            case HazardShape.Circle:
                isInRange = Vector3.Distance(ControllerGame.Instance.player.transform.position, transform.position) < Range;
                break;


        }
        if (isInRange)
        {

            ControllerGame.Instance.player.Damage(DamageToDeal);
            ControllerGame.Instance.player.TeleportToLastGround();
        }
       
    }
    protected virtual void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.green;

        switch (Shape)
        {
            case HazardShape.Square:
                UnityEditor.Handles.DrawWireCube(transform.position, Vector3.one*Range);
                break;

            case HazardShape.Circle:
                UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, Range);
                break;


        }
       

#endif

    }


}

public enum HazardShape {

    Square,
    Circle,

}
