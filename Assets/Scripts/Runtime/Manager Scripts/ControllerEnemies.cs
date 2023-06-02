using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEnemies : MonoBehaviour
{
    [SerializeField]
    EnemyStats UnicornStats, CyclopsStats, SpiderStats, HarpyStats;


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
}
