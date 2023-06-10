using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEnemies : MonoBehaviour
{
    [SerializeField]
    List<EnemyStatsOverride> DefaultEnemyStats;


    Dictionary<EnemyType, HashSet<Transform>> AgressiveEnemies = new Dictionary<EnemyType, HashSet<Transform>>();
    public void Init()
    {
        var enemies = FindObjectsOfType<EntityController>();

        foreach (var enemy in enemies)
        {
            enemy.Init(DefaultEnemyStats.Find(x => x.EnemyType == enemy.EnemyType).EnemyStats);
        }
    }

    public void AddAggresiveEnemy(EntityController enemy)
    {
        if (enemy.EnemyType == EnemyType.None)
        {
            return;
        }


        if (!AgressiveEnemies.ContainsKey(enemy.EnemyType))
        {
            AgressiveEnemies.Add(enemy.EnemyType, new HashSet<Transform>());
        }
        if (!AgressiveEnemies[enemy.EnemyType].Contains(enemy.transform))
        {
            AgressiveEnemies[enemy.EnemyType].Add(enemy.transform);
        }
       
        if (AgressiveEnemies[enemy.EnemyType].Count == 1)
        {
            SoundManager.Instance.PlayLooped(enemy.Sound.AgressiveLoop, gameObject, enemy.transform);
        }

    }



    public void RemoveAggresiveEnemy(EntityController enemy)
    {
        if (enemy.EnemyType == EnemyType.None)
        {
            return;
        }


        if (AgressiveEnemies.ContainsKey(enemy.EnemyType))
        {
            if (AgressiveEnemies[enemy.EnemyType].Contains(enemy.transform))
            {
                AgressiveEnemies[enemy.EnemyType].Remove(enemy.transform);
            }
         
            if (AgressiveEnemies[enemy.EnemyType].Count == 0)
            {
                SoundManager.Instance.CancelLoop(enemy.Sound.AgressiveLoop, gameObject);
            }
        }
    }

       
    
    }
