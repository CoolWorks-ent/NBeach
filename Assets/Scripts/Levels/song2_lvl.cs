using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class song2_lvl : Level {

    public EnemySpawner enemySpawners;
    [SerializeField]
    GameController gController;
    [SerializeField]
    ShellPickup[] pickupShells;
    [SerializeField]
    int shellSpawnRate = 10; //The greater the slower, 1 every x seconds
    [SerializeField]
    Transform[] shellLocs;
    [SerializeField]
    DarknessBoss darkBoss;
    [SerializeField]
    ParticleSystem RainFX;
    [SerializeField]
    GameObject enemyStageObj;
    GameObject darkBossObj; //use this variable for any movement related code for the darkBoss.  This is it's parent container

    CameraPathAnimator pathControl;
    public Image blackOverlay;
    int enemiesDestroyed = 0;
    float curSongTime = 0;
    float songStartTime = 0;
    public int stageNum = 0;
    float stage1StartTime = 120; //in seconds
    float stage2StartTime = 180; //3min in seconds
    float stage3StartTime = 240; //4min in seconds
    int darkSpawnRate_Stage1 = 10;
    int darkSpawnRate_Stage2 = 7;
    int darkSpawnRate_Stage3 = 4;

    int numOfShells = 0;
    int maxShellCount = 2;
    int rainEmissionRateDefault = 8;
    int rainEmissionRateMax = 40;
    Material nightSkybox;


    // Use this for initialization
    void Start () {
        
    }

    public new void Initialize()
    {
        EventManager.StartListening("DarknessDeath", DarknessDestroyed);
        EventManager.StartListening("PickUpShell", Evt_ShellPickedUp);
        EventManager.StartListening("Stage1Start", delegate { DebugFunc("Stage1Start"); });
        EventManager.StartListening("Stage2Start", delegate { DebugFunc("Stage2Start"); });
        EventManager.StartListening("Stage3Start", delegate { DebugFunc("Stage3Start"); });
        EventManager.StartListening("Stage4Start", delegate { DebugFunc("Stage4Start"); });

        gController = GameController.instance;
        pathControl = gController.pathControl;
        pathControl.topSpeed = 4;

        blackOverlay = GameObject.Find("BlackOverlay").GetComponent<Image>();
        blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 0);
        blackOverlay.gameObject.SetActive(false);
        nightSkybox = (Material)Resources.Load("Skyboxes/Night 01A", typeof(Material));
        //start Intro First
        darkBoss.gameObject.SetActive(false);
        darkBossObj = darkBoss.transform.parent.gameObject;
        RainFX.Stop();

        //Start Stage0 of battle
        Stage0();
   }


	
	// Update is called once per frame
	void Update () {

        //get current time based upon when stage0 started
        curSongTime = Time.time - songStartTime;

        //call function to handle spawning of throwable shells
        ShellSpawner();

    }

    void StartNextStage()
    {
        if (stageNum == 1) //&& curSongTime >= stage1StartTime)
        {
            //start Stage1 of battle
            Stage1();
        }
        //Logic for Stages of Battle
        //minimum 10 enemies Destroyed
        if (stageNum == 2) //&& curSongTime >= stage2StartTime)
        {
            //start Stage2 of battle
            Stage2();
        }
        //minimu 30 enemies destroyed
        else if (stageNum == 3 )//&& curSongTime >= stage3StartTime)
        {
            //start Stage2 of battle
            Stage3();
        }
    }

    private void DebugFunc(string evt)
    {
        if (evt == "Paused")
            Debug.Log("[Camera Path]: Paused");
        else if (evt == "Resume")
            Debug.Log("[Camera Path]: Resume");
        else if (evt == "Finished")
            Debug.Log("[Camera Path]: Finished");
        else if (evt == "StopAudio")
            Debug.Log("[Sound Manager]: Audio Stopped");
    }

    void DarknessDestroyed(string evt)
    {
        enemiesDestroyed += 1;
    }

    /**********
     **********Pick-Up Spawning Functions***********
     **********/
    void ShellSpawner()
    {
        /*Logic for managing shell projectiles in scene
        */
        //Only spawn shells if the player is in a battle sequence. Ie. NotMoving and Not in a cutscene
        if (gController.playerControl.playerState != PlayerState.MOVING)
        { 
            //only spawn shells if this bool tells it to
            if (numOfShells < maxShellCount)
            {
                StartCoroutine(SpawnShellPickUp());
                numOfShells += 1;
            }
        }
            
    }

    IEnumerator SpawnShellPickUp()
    {
        float shellSpawnRate = 2;
        float time = 0;
        while (time <= shellSpawnRate)
        {
            time += Time.deltaTime;
            yield return null;
        }
        //choose random spawn location in array and spawn there
        ShellPickup tempShell = pickupShells[Random.Range(0, pickupShells.Length)];
        ShellPickup reloadShell = Instantiate(tempShell, shellLocs[Random.Range(0, shellLocs.Length)].position, tempShell.transform.rotation);
        Debug.Log("pickup shell spawned");
        
        yield return 0;
    }

    //event function for when a shell is pickedup
    void Evt_ShellPickedUp(string str)
    {
        numOfShells -= 1;
    }


    /***************
     *****Level Stage Functions ****************
     ************/

    void Stage0()
    {
        stageNum = 0;
        songStartTime = Time.time;
        darkBoss.gameObject.SetActive(true);

        StartCoroutine(Stage0Routine());
        
    }
    void Stage1()
    {
        stageNum = 1;
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

    //function to make player run to next rock when old rock is destroyed Via Spline
    IEnumerator RunToNewRock(int rockNum)
    {
        //begin spline
        pathControl.Play();
        //playerControl.CanMove = true;
        gController.playerControl.playerState = PlayerState.MOVING;
        yield return 0;
    }

    /*
     * Function that Pauses Spline for when the player reaches a new rock ONLY.
     */
    public void PauseSplineEndStage()
    {
        //pathControl.Pause();
        pathControl.pPathState = CamPathState.Paused;
        gController.playerControl.playerState = PlayerState.NOTMOVING;

        //increment state number because player has reach next rock
        stageNum += 1; 
        Debug.Log("Rock " + stageNum + " Reached.");
        StartNextStage();
        //remove speed boost and other FX
        //gController.playerControl.Reset();
    }

    IEnumerator Stage0Routine()
    {
        Debug.Log("Stage 0 Start");
        //start rain
        RainFX.Play();
        float rainIncreaseTime = 5f;
        float t = 0;

        yield return new WaitForSeconds(5);
        //display overlay briefly to be eye-blink and block the change of the skybox
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 1f);
        yield return new WaitForSeconds(0.1f);

        //change skybox to night
        print("night sky");
        RenderSettings.skybox = nightSkybox;
        yield return new WaitForSeconds(0.1f);
        blackOverlay.gameObject.SetActive(false);

        //increase the rains RateOverTime gradually
        while (t < rainIncreaseTime )
        {
            UnityEngine.ParticleSystem.EmissionModule em = RainFX.emission;
           em.rateOverTime  = Mathf.Lerp(rainEmissionRateDefault, rainEmissionRateMax, t / rainIncreaseTime);
            t += Time.deltaTime;
            yield return null;
        }

        //begin spawning enemies
        enemySpawners.spawningEnemies = true;

        //move dark boss to the top of the water plane
        float moveTime = 4;
        t = 0;
        Vector3 startPos = darkBossObj.transform.position;
        Vector3 endPos = new Vector3(startPos.x, GameObject.Find("OceanSurfaceQuad").transform.position.y, startPos.z);

        while (t <= moveTime)
        {
            //slowly move darkboss to above the water and begin its attack
            darkBossObj.transform.position = Vector3.Lerp(startPos, endPos, t / moveTime);
            t += Time.deltaTime;
            yield return null;
        }
        //start the dark boss' attack sequence
        darkBoss.stage = DarknessBoss.BossStage.stage1;
        darkBoss.status = DarknessBoss.BossStatus.start;

        float waitTime = stage1StartTime - curSongTime;
        Debug.Log("time till next stage = " + waitTime);
        float tempWaitTime = 15;
        //This is the length of time the rest of the stage should play out
        yield return new WaitForSeconds(tempWaitTime);
        //darkBoss.DoRockSmash_Interrupt();

        //wait for an event for when the rock is destroyed by the dark boss.
        //After dark boss as destroyed the rock, Trigger the RunToNewRock CoRoutine...
       StartCoroutine(RunToNewRock(1));

        yield return 0;
    }

    IEnumerator Stage1Routine()
    {
        Debug.Log("Stage 1 Start");
        //Set Enemy Stage to face the Player Container Position
        enemyStageObj.transform.LookAt(gController.playerControl.GetComponent<NFPSController>().playerContainer.transform);

        enemySpawners.spawnRate = darkSpawnRate_Stage1;
        //start playing bg song
        //gController.soundManager.FadeInMusic(1);

        float tempWaitTime = 15;
        //This is the length of time the rest of the stage should play out
        yield return new WaitForSeconds(tempWaitTime);
        //darkBoss.DoRockSmash_Interrupt();

        //wait for an event for when the rock is destroyed by the dark boss.
        //After dark boss as destroyed the rock, Trigger the RunToNewRock CoRoutine...
        StartCoroutine(RunToNewRock(2));
        yield return 0;
    }

    IEnumerator Stage2Routine()
    {
        Debug.Log("Stage 2 Start");
        enemySpawners.spawnRate = darkSpawnRate_Stage2;

        while (curSongTime < 10f)
        {
            //track and increment total time passed since beginning of level
            curSongTime += Time.deltaTime;
            yield return null;
        }
        //start playing bg song
        //gController.soundManager.FadeInMusic(1);

    }

    IEnumerator Stage3Routine()
    {
        Debug.Log("Stage 3 Start");
        enemySpawners.spawnRate = darkSpawnRate_Stage3;

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
