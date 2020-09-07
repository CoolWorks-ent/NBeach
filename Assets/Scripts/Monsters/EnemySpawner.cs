using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarknessMinion;

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
    [SerializeField]
    ParticleSystem rainParticle;


    public bool spawningEnemies = false;
    public bool pauseSpawning = false;
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

    public void DarknessGruntSpawnCheck(Vector3 spawnPos)
    {
        //only spawn enemies if this bool tells it to
        if (spawningEnemies == true)
        {
            if (pauseSpawning == false)
            {
                if (Darkness_Manager.Instance.ActiveDarkness.Count < Darkness_Manager.Instance.maxEnemyCount)
                {
                    if (spawnWait >= spawnRate)
                    {
                        Vector3 randomloc = Random.insideUnitCircle * 5;
                        Darkness enemy = Instantiate(darknessEnemy, new Vector3(spawnPos.x, spawnPos.y+0.5f, spawnPos.z), darknessEnemy.transform.rotation);
                        //reset timer
                        spawnWait = 0;
                        //add enemy to management list
                        //AI_Manager.OnDarknessAdded(enemy);
                        Debug.Log("darkness spawned");
                    }

                }
            }
        }
    }

    void DarknessGruntWaveSpawn()
    {

    }

    public void Update()
    {
        if (spawningEnemies == true)
        {
            if (pauseSpawning == false)
            {
                //increase timer
                spawnWait += Time.deltaTime;
            }
            else
            {
                if (Darkness_Manager.Instance.ActiveDarkness.Count <= Darkness_Manager.Instance.minEnemyCount)
                {
                    //resume spawning of enemies
                    pauseSpawning = false;
                }
            } 
         }
        /*
        //only spawn enemies if this bool tells it to
        if (spawningEnemies == true)
        {
            if (pauseSpawning == false)
            {
                if (AI_Manager.Instance.ActiveDarkness.Count < AI_Manager.Instance.maxEnemyCount)
                {
                    if (spawnWait >= spawnRate)
                    {                        
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
                else
                {
                    pauseSpawning = true;
                    Debug.Log("[AI] Pause Spawning");
                }
            }

            else
            {
                if (AI_Manager.Instance.ActiveDarkness.Count <= AI_Manager.Instance.minEnemyCount)
                {
                    //resume spawning of enemies
                    pauseSpawning = false;
                }
            }
        }*/

    }
}
