using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    Collider2D Confiner;

    [SerializeField]
    public List<EnemyStatsOverride> OverrideEnemyStats;

    List<EnemyMapData> Enemies = new List<EnemyMapData>();


    public List<EnemyBase> Init(List<EnemyBase> enemies)
    {
        var enemiesInThisRoom = new List<EnemyBase>();
        Confiner = GetComponent<Collider2D>();

        foreach (var enemy in enemies)
        {
            var hit = Physics2D.OverlapPoint(enemy.Rigidbody.position, LayerMask.GetMask("Room"));
            if (hit != null && hit.gameObject == gameObject)
            {
                enemiesInThisRoom.Add(enemy);
                Enemies.Add(new EnemyMapData
                {
                    position = enemy.transform.position,


                });

            }

        }
        return enemiesInThisRoom;

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
}

public enum EnemyType
{
    None,
    Harpy,
    Spider,
    Cyclops,
    Unicorn,

}
