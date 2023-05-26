using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBot : EnemyBase
{


    [SerializeField]
    float CheckSize;

    [SerializeField]
    Vector2 backCheckPoint, frontCheckPoint, wallCheckPoint;

    Vector2 cachedPos, backCheckPointActive, frontCheckPointActive, wallCheckPointActive;
    float cachedAngle;

    int GroundLayer;


    [SerializeField]
    Rigidbody2D Rigidbody2D;

    [SerializeField]
    float moveSpeed, direction;

    [SerializeField]
    bool startRight;

    float rotationSpeed;
    public override void Awake()
    {
        base.Awake();
        Rigidbody2D.isKinematic = true;
        GroundLayer = LayerMask.GetMask("Ground");
        backCheckPointActive = backCheckPoint;
        frontCheckPointActive = frontCheckPoint;
        wallCheckPointActive = wallCheckPoint;
        if (!startRight)
        {
            ChangeDirection();
        }

        //90 is a magic number, we can experiment with this
        rotationSpeed = moveSpeed * 90f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        cachedPos = new Vector2(transform.position.x, transform.position.y);
        cachedAngle = Rigidbody2D.rotation;

        var backCR = Utils.Rotate2D(backCheckPointActive, cachedAngle);
        var frontCR = Utils.Rotate2D(frontCheckPointActive, cachedAngle);
        var wallCR = Utils.Rotate2D(wallCheckPointActive, cachedAngle);


        var bellyCheck = Physics2D.OverlapCircle(cachedPos, CheckSize, GroundLayer) != null;

        var backCheck = Physics2D.OverlapCircle(cachedPos + backCR, CheckSize, GroundLayer) != null;
     
        var frontCheck = Physics2D.OverlapCircle(cachedPos + frontCR, CheckSize, GroundLayer)  != null;
        var wallCheck = Physics2D.OverlapCircle(cachedPos + wallCR, CheckSize, GroundLayer) != null;


        //move a bit up if sunk into the ground
        if (bellyCheck)
        {
            Rigidbody2D.MovePosition(cachedPos + 0.1f*Utils.Rotate2D(Vector2.up, cachedAngle));
        }

        //stick to the ground --- this can lead to the spider floating off
        if (!backCheck && !frontCheck && !wallCheck)
        {
            Rigidbody2D.MovePosition(cachedPos + Rigidbody2D.gravityScale * Utils.Rotate2D(Vector2.down, cachedAngle) * Time.fixedDeltaTime);
        }
        else
        {


            //hit wall rotate up
            if (wallCheck)
            {
                Rigidbody2D.MoveRotation(cachedAngle + rotationSpeed * direction * Time.fixedDeltaTime);
            }
            //at a descent wall rotate down
            else if (!frontCheck)
            {
                Rigidbody2D.MoveRotation(cachedAngle - rotationSpeed * direction * Time.fixedDeltaTime);
            }
            //touched ground with face, rotate up
            else if (!backCheck && frontCheck)
            {
                Rigidbody2D.MoveRotation(cachedAngle + rotationSpeed * direction * Time.fixedDeltaTime);
            }


            //just move forward
            if (!wallCheck)
            {
                Rigidbody2D.MovePosition(cachedPos + moveSpeed * Utils.Rotate2D(Vector2.right, cachedAngle) * direction * Time.fixedDeltaTime);
            }
        }
        
    }

    //could be used to block escape into another room for example
    void ChangeDirection()
    {
        direction *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        backCheckPointActive = new Vector2(backCheckPointActive.x * -1, backCheckPointActive.y);
        frontCheckPointActive= new Vector2(frontCheckPointActive.x * -1, frontCheckPointActive.y);
        wallCheckPointActive = new Vector2(wallCheckPointActive.x * -1, wallCheckPointActive.y);
    }


    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
#if UNITY_EDITOR

        if (!UnityEditor.EditorApplication.isPlaying)
        {
            cachedPos = new Vector2(transform.position.x, transform.position.y);
            backCheckPointActive = backCheckPoint;
            frontCheckPointActive = frontCheckPoint;
            wallCheckPointActive = wallCheckPoint;
        }




        var backCR = Utils.Rotate2D(backCheckPointActive, cachedAngle);
        var frontCR = Utils.Rotate2D(frontCheckPointActive, cachedAngle);
        var wallCR = Utils.Rotate2D(wallCheckPointActive, cachedAngle);

        UnityEditor.Handles.DrawWireDisc(cachedPos + backCR, Vector3.forward, CheckSize);
        UnityEditor.Handles.DrawWireDisc(cachedPos, Vector3.forward, CheckSize);
        UnityEditor.Handles.DrawWireDisc(cachedPos + frontCR, Vector3.forward, CheckSize);
        UnityEditor.Handles.DrawWireDisc(cachedPos + wallCR, Vector3.forward, CheckSize);

#endif


    }
}
