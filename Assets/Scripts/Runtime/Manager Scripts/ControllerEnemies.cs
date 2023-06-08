using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEnemies : MonoBehaviour
{
    [SerializeField]
    EnemyStats UnicornStats, CyclopsStats, SpiderStats, HarpyStats;


    Dictionary<string, HashSet<Transform>> AgressiveEnemies = new Dictionary<string, HashSet<Transform>>();
    public void Init()
    {
        var enemies = FindObjectsOfType<EntityController>();

        foreach (var enemy in enemies)
        {
            switch (enemy)
            {
                case UnicornBot unicorn:
                    unicorn.Init(UnicornStats);
                    break;

                case SpiderBot spider:
                    spider.Init(SpiderStats);
                    break;

                case HarpyBot harpy:
                    harpy.Init(HarpyStats);
                    break;

                case CyclopsBot cyclops:
                    cyclops.Init(CyclopsStats);
                    break;

            }

        }
    }

    public void AddAggresiveEnemy(EntityController enemy)
    {
        string typeName = EnemyToTypeString(enemy);
        if (string.IsNullOrEmpty(typeName))
        {
            return;
        }


        if (!AgressiveEnemies.ContainsKey(typeName))
        {
            AgressiveEnemies.Add(typeName, new HashSet<Transform>());
        }
        if (!AgressiveEnemies[typeName].Contains(enemy.transform))
        {
            AgressiveEnemies[typeName].Add(enemy.transform);
        }
       
        if (AgressiveEnemies[typeName].Count == 1)
        {
            SoundManager.Instance.PlayLooped(enemy.Sound.AgressiveLoop, gameObject, enemy.transform);
        }

    }



    public void RemoveAggresiveEnemy(EntityController enemy)
    {
        string typeName = EnemyToTypeString(enemy);

        if (string.IsNullOrEmpty(typeName))
        {
            return;
        }


        if (AgressiveEnemies.ContainsKey(typeName))
        {
            if (AgressiveEnemies[typeName].Contains(enemy.transform))
            {
                AgressiveEnemies[typeName].Remove(enemy.transform);
            }
         
            if (AgressiveEnemies[typeName].Count == 0)
            {
                SoundManager.Instance.CancelLoop(enemy.Sound.AgressiveLoop, gameObject);
            }
        }
    }

    private string EnemyToTypeString(EntityController enemyBase)
    {
        return enemyBase switch
        {
            HarpyBot harpy => harpy.GetType().ToString(),
            SpiderBot spider => spider.GetType().ToString(),
            UnicornBot unicorn => unicorn.GetType().ToString(),
            CyclopsBot cyclops => cyclops.GetType().ToString(),
            _ => string.Empty

        };

    }
       
    
    }
