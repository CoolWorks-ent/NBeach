using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    Darkness darknessEnemy;
    [SerializeField]
    Transform[] spawnLocs;
    [SerializeField]
    int spawnRate = 4; //The greater the slower, 1 every x seconds
    int MaxEnemyCount = 10;
    int enemyCount = 0;
    float spawnWait = 0;

    public void Start()
    {
        enemyCount = 0;
    }

    //function called if an enemy dies to decrease from the total enemy count
    public void DarknessDeath()
    {
        enemyCount -= 1;
    }

    public void Update()
    {
        
        if (enemyCount < MaxEnemyCount)
        {
            if (spawnWait >= spawnRate)
            {
                //choose random spawn location in array and spawn there
                Darkness enemy = Instantiate(darknessEnemy, spawnLocs[Random.Range(0, spawnLocs.Length)].position, darknessEnemy.transform.rotation);
                //reset timer
                spawnWait = 0;
                enemyCount += 1;
                Debug.Log("darkness spawned");
            }
            
        }

        //increase timer
        spawnWait = spawnWait + Time.deltaTime;
    }
}
