using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class to manage enemies in the scene & the spawning of enemies
 */

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    Darkness darknessEnemy;
    [SerializeField]
    int spawnRate = 10; //The greater the slower, 1 every x seconds
    [SerializeField]
    Transform[] spawnLocs;

    public List<Darkness> enemyList;
    public bool spawningEnemies = false;
    int MaxEnemyCount = 10;
    float spawnWait = 0;
    int enemiesDestroyed = 0;

    public void Start()
    {
        enemyList = new List<Darkness>();
        //EventManager.StartListening("DarknessDeath", DarknessDeath);
    }

    //function called if an enemy dies to decrease from the total enemy count
    void DarknessDeath(string enemyName)
    {
        //remove enemy from the list
        Darkness enemyObj = GameObject.Find(enemyName).GetComponent<Darkness>();
        enemyList.Remove(enemyObj);
        Destroy(enemyObj.gameObject);
    }

    public void Update()
    {
        //only spawn enemies if this bool tells it to
        if (spawningEnemies == true)
        {
            if (enemyList.Count < MaxEnemyCount)
            {
                if (spawnWait >= spawnRate)
                {
                    //choose random spawn location in array and spawn there
                    Darkness enemy = Instantiate(darknessEnemy, spawnLocs[Random.Range(0, spawnLocs.Length)].position, darknessEnemy.transform.rotation);
                    enemy.enemySpawner = this;
                    //reset timer
                    spawnWait = 0;
                    //add enemy to management list
                    enemyList.Add(enemy);
                    Debug.Log("darkness spawned");
                }
                else
                {
                    //increase timer
                    spawnWait += Time.deltaTime;
                }

            }
        }

        
    }
}
