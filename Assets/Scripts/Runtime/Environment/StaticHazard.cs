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
    float AttackForce;

    [SerializeField]
    HazardShape Shape;


    int PlayerLayer;
    private void Awake()
    {
        PlayerLayer = LayerMask.GetMask("Player");
    }


    private void FixedUpdate()
    {
        bool isInRange = false;
        switch(Shape)
        {
            case HazardShape.Square:
                isInRange = Physics2D.OverlapBox(transform.position, Vector2.one * Range, 0, PlayerLayer) != null;
                break;

            case HazardShape.Circle:
                isInRange = Vector3.Distance(ControllerGame.Instance.player.transform.position, transform.position) < Range;
                break;


        }
        if (isInRange)
        {
            ControllerGame.Instance.player.AttackForce(transform.position, AttackForce);

            ControllerGame.Instance.player.Damage(DamageToDeal);
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
