using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public GameObject enemyPrefab;
    [Range(1, 60)]
    public float respawnTime = 5;
    [Tooltip("Uncheck this is you want to use timer for initial spawn")]
    public bool spawnOnLevelBegin = true;

    private GameObject enemyInstance;
    private bool setTimer = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;

        if(spawnOnLevelBegin)
            enemyInstance = Instantiate(enemyPrefab, transform.position, Quaternion.identity);        
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyInstance == null && !setTimer)
        {
            //Debug.Log("Enemy destroyed");
            setTimer = true;
            Invoke("SpawnEnemy", respawnTime);
        }
    }

    private void SpawnEnemy()
    {
        enemyInstance = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        setTimer = false;
    }
}
