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

    int bossRoom;

    Elevator[] elevators;

    

    public Room GetRoom(int roomId)
    {
        return Array.Find(m_Rooms, x => x.RoomId == roomId);
    }

    public void Init(CinemachineVirtualCamera cinemachineVirtualCamera, ColorAdjustments colorAdjustments)
    {
        elevators = FindObjectsOfType<Elevator>();
        var boss = FindObjectOfType<DaddyController>();
        var hit = Physics2D.OverlapPoint(boss.Rigidbody.position, LayerMask.GetMask("Room"));
        bossRoom = hit.GetComponent<Room>().RoomId;
        m_VCam = cinemachineVirtualCamera;
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

    void ToggleElevators(bool isPaused)
    {
        foreach (var elevator in elevators)
        {
            elevator.Pause(isPaused);

        }

    }

    public void EnterRoom(Room room)
    {
        if (room == null)
        {
            return;
        }
        room.Activate(DefaultEnemyStats);
        if (room.RoomId == bossRoom)
        {
            SoundManager.Instance.CancelLoop("Ambience", gameObject);
            MusicPlayer.Instance.StopPlaying(0.5f);
            ControllerGame.Instance.player.transform.position = new Vector3(-34.8f, -39.35f);
            ControllerGame.Instance.player.RigidBody.velocity = default;
            ControllerGame.Instance.Daddy.StartFight();
        }

    }

    public void ExitRoom(Room room)
    {
        if (room == null)
        {
            return;
        }

        room.Deactivate();
        if (room.RoomId == bossRoom)
        {
            SoundManager.Instance.PlayLooped("Ambience", gameObject);
            ControllerGame.Instance.Daddy.CancelFight();
            DOVirtual.DelayedCall(3f, () =>MusicPlayer.Instance.PlayPlaylist("GardenPlaylist", 2));
        }
    }

    void EchoExitRoom(Room room)
    {
        if (m_CurrentRoom == room)
        {
            if (m_NextRoom != null)
            {
                if (m_CurrentRoom != null && ControllerGame.Instance.player.Health > 0)
                {
                    AnimateRoomTransition(m_CurrentRoom, m_NextRoom);
                }
            }
            else
            {
                //Debug.LogWarning($"Echo not in any room! (from room {m_CurrentRoom})");
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
                if (room.RoomId != 3)
                {
                    MusicPlayer.Instance.PlayPlaylist("GardenPlaylist", 2);
                    SoundManager.Instance.PlayLooped("Ambience", gameObject);
                }
                firstRoomEntered = true;
                EnterRoom(room);
                ChangeCamera(room);
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

    public void OnDeathAnimation()
    {

        m_NextRoom = null;
        roomTransition.Kill();
        ControllerGame.Instance.player.FreezeOnTransition(true);
        ToggleElevators(true);
        var sequence = DOTween.Sequence();

        sequence.Append(DOVirtual.Float(0, 1, timeFadeIn*5, DoColorAdjustment));
        sequence.AppendCallback(() => ControllerGame.Instance.ResetPlayer());
        sequence.AppendCallback(() => ExitRoom(m_CurrentRoom));
        sequence.AppendCallback(() => m_CurrentRoom = GetRoom(ControllerGame.Instance.SavedRoom));
        sequence.AppendCallback(() => EnterRoom(GetRoom(ControllerGame.Instance.SavedRoom)));
        sequence.AppendCallback(() => ChangeCamera(GetRoom(ControllerGame.Instance.SavedRoom)));
        sequence.AppendInterval(0.4f);
        sequence.AppendCallback(() => ControllerGame.Instance.player.FreezeOnTransition(false));
        sequence.AppendCallback(() => ToggleElevators(false));
        sequence.Append(DOVirtual.Float(1, 0, timeFadeIn*3, DoColorAdjustment));
        
        sequence.Play();
        



    }
    Sequence roomTransition;
    void AnimateRoomTransition(Room fromRoom, Room toRoom)
    {
        ControllerGame.Instance.player.FreezeOnTransition(true);
        ToggleElevators(true);
        roomTransition = DOTween.Sequence();

        roomTransition.Append(DOVirtual.Float(0, 1, timeFadeIn, DoColorAdjustment));
        roomTransition.AppendCallback(() => ExitRoom(fromRoom));
        roomTransition.AppendCallback(() => EnterRoom(toRoom));
        roomTransition.AppendCallback(() => ChangeCamera(toRoom));
        roomTransition.AppendInterval(0.3f);
        roomTransition.Append(DOVirtual.Float(1, 0, timeFadeIn, DoColorAdjustment));

        roomTransition.AppendCallback(() => {
            m_CurrentRoom = m_NextRoom;
            m_NextRoom = null;
            ControllerGame.Instance.player.FreezeOnTransition(false);
            ToggleElevators(false);
        });

        roomTransition.Play();

    }

    void DoColorAdjustment(float value) {

       var c = Color.Lerp(Color.white, EndColor, value);
        m_ColorAdjustments.colorFilter.Override(c);
    }




}
