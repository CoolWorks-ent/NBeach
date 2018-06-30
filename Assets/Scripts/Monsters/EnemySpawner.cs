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
    public float spawnRate = 10; //The greater the slower, 1 every x seconds
    [SerializeField]
    Transform[] spawnLocs;

    public bool spawningEnemies = false;
    float spawnWait = 0;
    int enemiesDestroyed = 0;

    public void Start()
    {
        //EventManager.StartListening("DarknessDeath", DarknessDeath);
    }

    //function called if an enemy dies to decrease from the total enemy count
    void DarknessDeath(string enemyName)
    {
        //remove enemy from the list
        Darkness enemyObj = GameObject.Find(enemyName).GetComponent<Darkness>();
        Destroy(enemyObj.gameObject);
    }

    public void Update()
    {
        //only spawn enemies if this bool tells it to
        if (spawningEnemies == true)
        {
            if (AI_Manager.Instance.ActiveDarkness.Count < AI_Manager.Instance.maxEnemyCount)
            {
                if (spawnWait >= spawnRate)
                {
                    //choose random spawn location in array and spawn there
                    Vector3 randomloc = Random.insideUnitCircle * 5;
                    Darkness enemy = Instantiate(darknessEnemy, spawnLocs[Random.Range(0, spawnLocs.Length)].position + new Vector3(randomloc.x, 0, randomloc.y), darknessEnemy.transform.rotation);
                    //reset timer
                    spawnWait = 0;
                    //add enemy to management list
                    AI_Manager.Instance.AddtoDarknessList(enemy);
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
