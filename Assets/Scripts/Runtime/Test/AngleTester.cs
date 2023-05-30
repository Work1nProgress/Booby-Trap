using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleTester : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D enemy, player;

    [SerializeField]
    bool _movingRight;

    // Update is called once per frame
    void FixedUpdate()
    {
        var signedAngle = Vector2.SignedAngle(_movingRight ? Vector3.right : Vector3.left,
                player.position - enemy.position);

        var angle = Vector2.Angle(_movingRight ? Vector3.right : Vector3.left,
               player.position - enemy.position);
        Debug.Log($"{signedAngle} {angle}");
    }
}
