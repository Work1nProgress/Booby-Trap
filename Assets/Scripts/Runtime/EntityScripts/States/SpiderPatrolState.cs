using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpiderPatrolState", menuName = "Entities/Spider Patrol")]
public class SpiderPatrolState : EntityState
{




    Vector2 m_BackCheckPoint = new Vector2(-0.26f, -0.2f);
    Vector2 m_FrontCheckPoint = new Vector2(0.39f, -0.2f);
    Vector2 m_WallCheckPoint = new Vector2(0.41f, -0.12f);

    Vector2 m_CachedPos, m_BackCheckPointActive, m_FrontCheckPointActive, m_WallCheckPointActive;
    float m_CheckSize = 0.08f;
    float m_CachedAngle, m_Direction, m_RotationSpeed;

    public override void EnterState()
    {
        base.EnterState();
        if (_controller.Stats.StartDirection != 1)
        _controller.Rigidbody.isKinematic = true;
        m_BackCheckPointActive = m_BackCheckPoint;
        m_FrontCheckPointActive = m_FrontCheckPoint;
        m_WallCheckPointActive = m_WallCheckPoint;

        m_Direction = 1;
        _controller.Rigidbody.gravityScale = 1;
        if(_controller.Stats.StartDirection != 1)
        {
            ChangeDirection();
        }
        //90 is a magic number, we can experiment with this
        m_RotationSpeed = _controller.Stats.MovementSpeed * 90f;
    }

   
    public override void FixedUpdateState(float deltaTime)
    {
        base.FixedUpdateState(deltaTime);
        m_CachedPos = _controller.Rigidbody.position;
        m_CachedAngle = _controller.Rigidbody.rotation;

       

        var backCR = Utils.Rotate2D(m_BackCheckPointActive, m_CachedAngle);
        var frontCR = Utils.Rotate2D(m_FrontCheckPointActive, m_CachedAngle);
        var wallCR = Utils.Rotate2D(m_WallCheckPointActive, m_CachedAngle);


        var bellyCheck = Physics2D.OverlapCircle(m_CachedPos, m_CheckSize, Utils.GroundLayer) != null;

        var backCheck = Physics2D.OverlapCircle(m_CachedPos + backCR, m_CheckSize, Utils.GroundLayer) != null;

        var frontCheck = Physics2D.OverlapCircle(m_CachedPos + frontCR, m_CheckSize, Utils.GroundLayer) != null;
        var wallCheck = Physics2D.OverlapCircle(m_CachedPos + wallCR, m_CheckSize, Utils.GroundLayer) != null;



        //move a bit up if sunk into the ground
        if (bellyCheck)
        {
            _controller.Rigidbody.MovePosition(m_CachedPos + 0.1f * Utils.Rotate2D(Vector2.up, m_CachedAngle));
        }

        //stick to the ground --- this can lead to the spider floating off
        if (!backCheck && !frontCheck && !wallCheck)
        {
            _controller.Rigidbody.MovePosition(m_CachedPos + _controller.Rigidbody.gravityScale * Utils.Rotate2D(Vector2.down, m_CachedAngle) * Time.fixedDeltaTime);
        }
        else
        {


            //hit wall rotate up
            if (wallCheck)
            {
                _controller.Rigidbody.MoveRotation(m_CachedAngle + m_RotationSpeed * m_Direction * deltaTime);
            }
            //at a descent wall rotate down
            else if (!frontCheck)
            {
                _controller.Rigidbody.MoveRotation(m_CachedAngle - m_RotationSpeed * m_Direction * deltaTime);
            }
            //touched ground with face, rotate up
            else if (!backCheck && frontCheck)
            {
                _controller.Rigidbody.MoveRotation(m_CachedAngle + m_RotationSpeed * m_Direction * deltaTime);
            }


            //just move forward
            if (!wallCheck)
            {
                _controller.Rigidbody.MovePosition(m_CachedPos + _controller.Stats.MovementSpeed * Utils.Rotate2D(Vector2.right, m_CachedAngle) * m_Direction * deltaTime);
            }
        }


        
    }

    //could be used to block escape into another room for example
    void ChangeDirection()
    {
        m_Direction *= -1;
        Vector2 scale = m_Direction == -1 ? new Vector2(-1, 1) : new Vector2(1, 1);

        m_BackCheckPointActive = Vector2.Scale(m_BackCheckPointActive, scale);
        m_FrontCheckPointActive = Vector2.Scale(m_FrontCheckPointActive, scale);
        m_WallCheckPointActive = Vector2.Scale(m_WallCheckPointActive, scale);

        _controller.Sprite.flipX = m_Direction == -1;
    }


}
