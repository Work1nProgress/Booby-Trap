using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Cinemachine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class ControllerRooms : MonoBehaviour
{

    Room[] m_Rooms;


    [SerializeField]
    List<EnemyStatsOverride> DefaultEnemyStats;


    [SerializeField]
    Color EndColor;

    [SerializeField]
    float timeFadeIn = 0.3f;

    [SerializeField]
    float timeFadeOut = 0.3f;


    CinemachineVirtualCamera m_VCam;
    CinemachineConfiner2D m_Confiner2D;
    ColorAdjustments m_ColorAdjustments;

    Room m_CurrentRoom;
    Room m_NextRoom;

    bool firstRoomEntered = false;

    public Room GetRoom(int roomId)
    {
        return Array.Find(m_Rooms, x => x.RoomId == roomId);
    }

    public void Init(CinemachineVirtualCamera cinemachineVirtualCamera, ColorAdjustments colorAdjustments)
    {
        m_VCam = cinemachineVirtualCamera;
        Debug.Log(m_VCam);
        m_Confiner2D = m_VCam.GetComponent<CinemachineConfiner2D>();
        m_ColorAdjustments = colorAdjustments;

        var enemies = FindObjectsOfType<EnemyBase>().ToList();
        m_Rooms = FindObjectsOfType<Room>();
        foreach (var room in m_Rooms)
        {
            var toRemove = room.Init(enemies, EchoExitRoom, EchoInRoom);
            foreach (var enemyToRemove in toRemove) {
                
                enemies.Remove(enemyToRemove);
                Destroy(enemyToRemove.gameObject);
            }
        }

        for (int i = enemies.Count-1; i >=0; i--)
        {
            Debug.Log($"enemy {enemies[i]} at {enemies[i].Rigidbody.position} is not in a room", enemies[i]);
        }

    }

    public void EnterRoom(Room room)
    {
        room.Activate(DefaultEnemyStats);

    }

    public void ExitRoom(Room room)
    {
        room.Deactivate();
    }

    void EchoExitRoom(Room room)
    {
        if (m_CurrentRoom == room)
        {
            if (m_NextRoom != null)
            {
                if (m_CurrentRoom != null)
                {
                    AnimateRoomTransition(m_CurrentRoom, m_NextRoom);
                }
            }
            else
            {
                Debug.LogWarning($"Echo not in any room! (from room {m_CurrentRoom})");
            }
        }
    }

    void EchoInRoom(Room room)
    {

        if (m_CurrentRoom == null)
        {
            if (firstRoomEntered)
            {
                AnimateRoomTransition(null, room);

            }
            else
            {
                firstRoomEntered = true;
                EnterRoom(room);
            }
            m_CurrentRoom = room;
        }
        if (m_CurrentRoom == room)
        {
            
            return;
        }
        else
        {
            m_NextRoom = room;
        }

    }

    void ChangeCamera(Room toRoom)
    {
        m_Confiner2D.m_BoundingShape2D = toRoom.Confiner;
       

    }

    void AnimateRoomTransition(Room fromRoom, Room toRoom)
    {
        ControllerGame.Instance.player.FreezeOnTransition(true);

       var sequence = DOTween.Sequence();

        sequence.Append(DOVirtual.Float(0, 1, timeFadeIn, DoColorAdjustment));
        sequence.AppendCallback(() => ExitRoom(fromRoom));
        sequence.AppendCallback(() => EnterRoom(toRoom));
        sequence.AppendCallback(() => ChangeCamera(toRoom));
        sequence.Append(DOVirtual.Float(1, 0, timeFadeIn, DoColorAdjustment));

        sequence.AppendCallback(() => {
            m_CurrentRoom = m_NextRoom;
            m_NextRoom = null;
            ControllerGame.Instance.player.FreezeOnTransition(false);
        });
        
        sequence.Play();

    }

    void DoColorAdjustment(float value) {

       var c = Color.Lerp(Color.white, EndColor, value);
        m_ColorAdjustments.colorFilter.Override(c);
    }




}
