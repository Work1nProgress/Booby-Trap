using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    Collider2D m_Confiner;

    public Collider2D Confiner => m_Confiner;

    [SerializeField]
    public List<EnemyStatsOverride> OverrideEnemyStats;

    [SerializeField]
    int m_RoomId;
    public int RoomId => m_RoomId;

    List<EnemyMapData> Enemies = new List<EnemyMapData>();

    List<EnemyBase> LiveEnemies = new List<EnemyBase>();


    UnityAction<Room> m_OnEchoExit;
    UnityAction<Room> m_OnEchoInRoom;



    bool isInitialized = false;

    public List<EnemyBase> Init(List<EnemyBase> enemies, UnityAction<Room> OnEchoExit, UnityAction<Room> OnEchoInRoom)
    {
        m_OnEchoExit = OnEchoExit;
        m_OnEchoInRoom = OnEchoInRoom;
        var enemiesInThisRoom = new List<EnemyBase>();
        m_Confiner = GetComponent<Collider2D>();

        foreach (var enemy in enemies)
        {
            var hit = Physics2D.OverlapPoint(enemy.Rigidbody.position, LayerMask.GetMask("Room"));
            if (hit != null && hit.gameObject == gameObject)
            {
                enemiesInThisRoom.Add(enemy);
                var data = new EnemyMapData
                {
                    position = enemy.transform.position,
                    EnemyType = enemy.EnemyType,
                    FaceRight = enemy.StartDirection == 1

                };

                if (enemy is HarpyBot)
                {
                    data.HarpyWaypoints = (enemy as HarpyBot).Waypoints;
                }
                Enemies.Add(data);

                enemiesInThisRoom.Add(enemy);
            }

        }


        isInitialized = true;
        return enemiesInThisRoom;

    }

    static Dictionary<EnemyType, string> TypeToPrefab = new Dictionary<EnemyType, string>
    {
        {EnemyType.Spider, "spiderBot" },
        {EnemyType.Unicorn, "Unicorn Bot" },
        {EnemyType.Harpy, "Harpy Bot" },
        {EnemyType.Cyclops, "MaybeACyclops" },
         {EnemyType.None, string.Empty },


    };
   
    public void Activate(List<EnemyStatsOverride> defaultStats) {


        foreach (var enemyInMap in Enemies)
        {
            string prefabName = TypeToPrefab[enemyInMap.EnemyType];
            EnemyBase enemy = default;
            switch (enemyInMap.EnemyType)
            {
                case EnemyType.None:
                    break;

                case EnemyType.Harpy:
                    enemy = PoolManager.Spawn<HarpyBot>(prefabName, null);
                    (enemy as HarpyBot).SetWaypoints(enemyInMap.HarpyWaypoints);
                    break;

                case EnemyType.Spider:
                    enemy = PoolManager.Spawn<SpiderBot>(prefabName, null);
                    break;

                case EnemyType.Cyclops:
                    enemy = PoolManager.Spawn<CyclopsBot>(prefabName, null);
                    break;

                case EnemyType.Unicorn:
                    enemy = PoolManager.Spawn<UnicornBot>(prefabName, null);
                    break;
            }

            if (enemy != default)
            {
                enemy.transform.position = new Vector3(enemyInMap.position.x, enemyInMap.position.y, 0);
                enemy.StartDirection = enemyInMap.FaceRight ? 1 : -1;

               var idx = OverrideEnemyStats.FindIndex(x => x.EnemyType == enemyInMap.EnemyType);
                EnemyStatsOverride stats = default;
                if (idx == -1)
                {
                    stats = defaultStats.Find(x => x.EnemyType == enemyInMap.EnemyType);
                }
                else
                {
                    stats = OverrideEnemyStats[idx];
                }
                enemy.Init(stats.EnemyStats);
                LiveEnemies.Add(enemy);


            }
            

        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isInitialized)
        {
            return;
        }
        if (collision.gameObject.layer == Utils.PlayerLayer) {
            m_OnEchoInRoom.Invoke(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInitialized)
        {
            return;
        }
        if (collision.gameObject.layer == Utils.PlayerLayer)
        {
            m_OnEchoInRoom.Invoke(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isInitialized)
        {
            return;
        }
        if (collision.gameObject.layer == Utils.PlayerLayer)
        {
            m_OnEchoExit.Invoke(this);
        }
    }

    public void Deactivate()
    {
        foreach (var enemy in LiveEnemies)
        {
            enemy.CancelSound();
            
            PoolManager.Despawn(enemy);
        }
        LiveEnemies.Clear();
    }
}





[System.Serializable]
public class EnemyStatsOverride
{
    public EnemyType EnemyType;
    public EnemyStats EnemyStats;
}



public class EnemyMapData
{
    public EnemyType EnemyType;
    public Vector3 position;
    public bool FaceRight;

    public Transform[] HarpyWaypoints;
}

public enum EnemyType
{
    None,
    Harpy,
    Spider,
    Cyclops,
    Unicorn,

}
