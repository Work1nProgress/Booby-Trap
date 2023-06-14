using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MineAttack", menuName = "Entities/Daddy/Mine Attack")]
public class DaddyMineAttack : DaddyAttack
{
    [SerializeField]
    Vector2 GridSpacing;

    [SerializeField]
    Vector2 VerticalOffset;

    [SerializeField]
    Vector2 HorizontalOffset;

    [SerializeField]
    float MineTelegraphTime;

    [SerializeField]
    Vector2 MineHitBox;


    [SerializeField]
    int MinMines;

    [SerializeField]
    int MaxMines;

    [SerializeField]
    int MinimumMineDistance;

    [SerializeField]
    float maxDistanceToPlayer = 2f;



    [Header("Debug room data")]

    [Tooltip("Just for visual purposes, should be same in daddy controller")]
    [SerializeField]
    Vector2 RoomSize;

    [SerializeField]
    Vector2 RoomPosition;

    List<Vector2> MinePositions;

    List<Vector2> PossiblePositions;


    public override void Init(DaddyController daddyController)
    {
        MinePositions = new List<Vector2>();
        base.Init(daddyController);
    }

    protected override void StartActive()
    {
        base.StartActive();
        PossiblePositions = new List<Vector2>();


        float xMin = RoomPosition.x + Mathf.Max(0, HorizontalOffset.x) + GridSpacing.x/2;
        float yMin = RoomPosition.y + Mathf.Max(0, VerticalOffset.x) + GridSpacing.y/2;

        float xMax = RoomPosition.x + RoomSize.x - Mathf.Max(0, HorizontalOffset.y);
        float yMax = RoomPosition.y + RoomSize.y - Mathf.Max(0, VerticalOffset.y);

        float x = xMin;
      

        int breakCondition = 0;
        
        while (x < xMax && breakCondition < 100)
        {

            int breakCondition2 = 0;
            float y = yMin;
            while (y < yMax && breakCondition2 < 100)
            {
                var pos = new Vector2(x, y);
                var distance = Vector2.Distance(ControllerGame.Instance.player.RigidBody.position, pos);
                if (!MinePositions.Contains(pos) && distance > maxDistanceToPlayer)
                {
                    PossiblePositions.Add(new Vector2(x, y));
                }
                y += GridSpacing.y;

                breakCondition2++;
            }
            x += GridSpacing.x;

            breakCondition++;
        }

        int Mines = Random.Range(MinMines, MaxMines+1);

        bool soundPlayed = false;
        for (int i = 0; i < Mines; i++)
        {
            if (PossiblePositions.Count > 0) {


                var nextMinePos = PossiblePositions[Random.Range(0, PossiblePositions.Count)];
                var mine = PoolManager.Spawn<MineTelegraph>("DaddyMineTelegraph", null, new Vector3(nextMinePos.x, nextMinePos.y, 0)).Init(new MineData
                {
                    MineTelegraphDuration = MineTelegraphTime,
                    MinePosition = nextMinePos,
                    MineHitbox = MineHitBox,
                    MineCallback = OnMineDestroyed,
                    Damage = DamageToPlayer
                });
                if (!soundPlayed)
                {
                    soundPlayed = true;
                    SoundManager.Instance.Play(_controller.Sound.MineArmed, mine.transform);
                }
                MinePositions.Add(nextMinePos);
                for (int j = PossiblePositions.Count - 1; j >= 0; j--)
                {
                    if (Mathf.Abs(nextMinePos.x - PossiblePositions[j].x) <= GridSpacing.x * MinimumMineDistance)
                    {

                        if (Mathf.Abs(nextMinePos.y - PossiblePositions[j].y) <= GridSpacing.y*MinimumMineDistance)
                        {
                            PossiblePositions.RemoveAt(j);
                        }
                    }
                }
            }
        }
        


        //var slash = PoolManager.Spawn<PoolObjectTimed>("SlashDaddy", _controller.transform, new Vector3(SlashPosition.x * _controller.facingDirection, SlashPosition.y, 0));
        //slash.StartTicking(m_ActiveTime);
    }

    void OnMineDestroyed(Vector2 minePosition)
    {
        if (MinePositions.Contains(minePosition))
        {
            MinePositions.Remove(minePosition);
        }


    }
    

#if UNITY_EDITOR
    private void OnEnable()
    {
        UnityEditor.SceneView.duringSceneGui += DrawMyGizmos;
    }

    private void OnDisable()
    {
        UnityEditor.SceneView.duringSceneGui -= DrawMyGizmos;
      
    }

    private void DrawMyGizmos(UnityEditor.SceneView sceneView)
    {

        if (!UnityEditor.Selection.Contains(this))
            return;

        
        float xMin = RoomPosition.x + Mathf.Max(0, HorizontalOffset.x);
        float yMin = RoomPosition.y + Mathf.Max(0, VerticalOffset.x);

        float xMax = RoomPosition.x  + RoomSize.x - Mathf.Max(0, HorizontalOffset.y);
        float yMax = RoomPosition.y  + RoomSize.y - Mathf.Max(0, VerticalOffset.y);

        float x = xMin;

        int breakCondition = 0;
        while (x < xMax && breakCondition < 100)
        {

            UnityEditor.Handles.DrawLine(new Vector3(x, yMin), new Vector3(x, yMax));
            x += GridSpacing.x;

            breakCondition++;
        }
        UnityEditor.Handles.DrawLine(new Vector3(xMax, yMin), new Vector3(xMax, yMax));

        float y = yMin;
        breakCondition = 0;
        while (y < yMax && breakCondition < 100)
        {

            UnityEditor.Handles.DrawLine(new Vector3(xMin, y), new Vector3(x, y));
            y += GridSpacing.y;

            breakCondition++;
        }
        UnityEditor.Handles.DrawLine(new Vector3(xMin, yMax), new Vector3(xMax, yMax));
    }
#endif


}
