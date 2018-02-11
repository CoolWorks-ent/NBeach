using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class song2_lvl : Level {

    public EnemySpawner enemySpawners;
    [SerializeField]
    GameController gController;

    int enemiesDestroyed = 0;
    float curSongTime = 0;
    int stageNum = 1;
    float stage1StartTime = 120; //in seconds
    float stage2StartTime = 180; //3min in seconds
    float stage3StartTime = 240; //4min in seconds

    // Use this for initialization
    void Start () {
        
    }

    public new void Initialize()
    {
        EventManager.StartListening("DarknessDeath", DarknessDestroyed);
        //start Intro First

        //Start Stage1 of battle
        Stage1();
   }


	
	// Update is called once per frame
	void Update () {
        
              

        //Logic for Stages of Battle
        if (enemiesDestroyed > 10 && stageNum == 1 && curSongTime >= stage2StartTime)
        {
            //start Stage2 of battle
            Stage2();
        }
        else if(enemiesDestroyed > 30 && stageNum == 2 && curSongTime >= stage3StartTime)
        {
            //start Stage2 of battle
            Stage3();
        }
	}

    void DarknessDestroyed(string evt)
    {
        enemiesDestroyed += 1;
    }

    void Stage1()
    {
        enemySpawners.spawningEnemies = true;
        StartCoroutine(Stage1Routine());
    }

    void Stage2()
    {
        stageNum = 2;
    }

    void Stage3()
    {
        stageNum = 3;
    }

    //functions to turn enemy spawners on or off

    IEnumerator Stage1Routine()
    {
        
        while (curSongTime < 10f)
        {
            //track and increment total time passed since beginning of level
            curSongTime += Time.deltaTime;
            yield return null;
        }
        //start playing bg song
        //gController.soundManager.FadeInMusic(1);

    }
}
