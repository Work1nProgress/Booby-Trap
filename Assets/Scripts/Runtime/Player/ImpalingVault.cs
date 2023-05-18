using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpalingVault : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField][Range (1, 100)]
    float upwardForce = 20;
    [SerializeField][Range(1, 100)]
    float forwardForce = 0;

    [Header("Damage")]
    float damageRadius = 5;
    float damageDealt = 1;

    private PlayerMovement movement;
    private Rigidbody2D body;

    private bool canPerformAttack = false;
    private bool performingVault = false;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire3") || Input.GetKeyDown(KeyCode.E))
        {
            if (movement.OnGround)
            {
                movement.Jump(upwardForce);

                //body.AddForce(movement.FaceDirection * forwardForce, ForceMode2D.Impulse);

                performingVault = true;
            }
        }
            
    }

}
