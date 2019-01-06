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
    SpeedBoost speedBoostPowerUp;
    [SerializeField]
    public GameObject[] coverRocks;
    [SerializeField]
    DarknessBoss darkBoss;
    [SerializeField]
    GameObject[] lightningBolts;
    [SerializeField]
    ParticleSystem RainFX;
    [SerializeField]
    ParticleSystem RainFX_2;
    [SerializeField]
    ParticleSystem RainFX_3;
    [SerializeField]
    ParticleSystem RainFX_Wide;
    [SerializeField]
    GameObject dirtImpactFX;
    [SerializeField]
    GameObject enemyStageObj;
    [SerializeField]
    GameObject playerStageObj;
    [SerializeField]
    CameraPathAnimator pathControl_EndScene;
    [SerializeField]
    SimpleAnimator2D animator2D;
    [SerializeField]
    int bossAttackSpeed_stage1 = 4, bossAttackSpeed_stage2 = 5, bossAttackSpeed_stage3 = 6;
    [SerializeField]
    float bossTimeBtwAttacks_stage1 = 1, bossTimeBtwAttacks_stage2 = 1.5f, bossTimeBtwAttacks_stage3 = 1.5f;
    [SerializeField]
    Transform darkBossEndPos;
    [SerializeField]
    Material rockMaterial;
    [SerializeField]
    Material rockMaterial_2;
    [SerializeField]
    Material cloudMaterial;

    GameObject darkBossObj; //use this variable for any movement related code for the darkBoss.  This is it's parent container

    CameraPathAnimator pathControl;
    public Image blackOverlay;
    int enemiesDestroyed = 0;
    float curSongTime = 0;
    float songStartTime = 0;
    public int stageNum = 0;
    float stage1StartTime = 120; //120; //in seconds
    float stage2StartTime = 180; //180; //3min in seconds
    float stage3StartTime = 230; //240; //4min in seconds
    float stage3EndTime = 250; //320 //5+min in seconds 
    float finalStageEndTime = 320; //(current total song time = 5:15)
    float darkSpawnRate_Stage0 = 6;
    float darkSpawnRate_Stage0_1 = 5;
    float darkSpawnRate_Stage1 = 4;
    float darkSpawnRate_Stage2 = 2f;
    float darkSpawnRate_Stage3 = 1f;
    int rainEmissionRate_stage1 = 0, rainEmissionRate_stage2 = 0, rainEmissionRate_stage3 = 0, rainEmissionRate_stage3End = 0;

    int numOfShells = 0;
    int maxShellCount = 5;
    float shellSpawnTime = 0;
    List<ShellPickup> shellArray;
    int rainEmissionRateDefault = 8;
    int rainEmissionRateMax = 40;
    Material nightSkybox;
    Material daySkybox;
   

    bool stagePlaying = true;

    // Use this for initialization
    void Start() {

    }

    public new void Initialize()
    {
        EventManager.StartListening("DarknessDeath", DarknessDestroyed);
        EventManager.StartListening("PickUpShell", Evt_ShellPickedUp);
        EventManager.StartListening("Stage1Start", delegate { DebugFunc("Stage1Start"); });
        EventManager.StartListening("Stage2Start", delegate { DebugFunc("Stage2Start"); });
        EventManager.StartListening("Stage3Start", delegate { DebugFunc("Stage3Start"); });
        EventManager.StartListening("Stage4Start", delegate { DebugFunc("Stage4Start"); });
        EventManager.StartListening("Song2_End_Cutscene_Start", delegate { StartCoroutine(Song2_EndCutscene()); });
        EventManager.StartListening("OnPlayerHitIsland", OnPlayerHitIsland);
        EventManager.StartListening("PauseWorld", OnStagePaused);

        gController = GameController.instance;
        pathControl = gController.pathControl;
        pathControl.topSpeed = 4;

        blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 0);
        blackOverlay.gameObject.SetActive(false);
        nightSkybox = (Material)Resources.Load("Skyboxes/Night 01A", typeof(Material));
        daySkybox = (Material)Resources.Load("Skyboxes/Day 01A", typeof(Material));

        //start Intro First
        darkBoss.gameObject.SetActive(false);
        darkBossObj = darkBoss.transform.parent.gameObject;
        RainFX.Stop();
        RainFX_2.Stop();
        RainFX_3.Stop();
        RainFX_Wide.Stop();

        shellArray = new List<ShellPickup>();
        gController.playerControl.playerStatus = PLAYER_STATUS.ALIVE;
        gController.playerControl.playerState = PlayerState.NOTMOVING;
        //Start Stage0 of battle
        Stage0();
        //StartCoroutine(StageTestRoutine());


        rainEmissionRate_stage1 = rainEmissionRateMax + 10;
        rainEmissionRate_stage2 = rainEmissionRateMax + 20;
        rainEmissionRate_stage3 = rainEmissionRateMax + 30;
        rainEmissionRate_stage3End = rainEmissionRateMax + 30;
    }



    // Update is called once per frame
    void Update()
    {

        //get current time based upon when stage0 started and the music started
        curSongTime = Time.time - songStartTime;

        //call function to handle spawning of throwable shells
        ShellSpawner();

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            DebugFunc("Song2_Stage1");
            gController.SceneSkip("Song2_Stage1", stage1StartTime);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DebugFunc("Song2_Stage2");
            gController.SceneSkip("Song2_Stage2", stage2StartTime);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DebugFunc("Song2_Stage3");
            gController.SceneSkip("Song2_Stage3", stage3StartTime);
        }
    }

    /// <summary>
    /// World Pause. Pauses all actions in the world except for the player's and the player's input. 
    /// Used primarily for the player's HURT state
    /// </summary>
    /// <param name="evt"></param>
         
    void OnStagePaused(string evt)
    {
        //Set timescale to 0
        //Time.timeScale = 0;
        stagePlaying = false;
    }

    void OnStageResumed(string evt)
    {
        //Set timescale to 1
        //Time.timeScale = 1;
        stagePlaying = true;
    }

    /// <summary>
    /// Game Pause.  Pauses the entire game state and brings up the pause menu
    /// </summary>
    /// <param name="evt"></param>
    void OnGamePaused(string evt)
    {

    }

    void OnGameResumed(string evt)
    { }

    /// <summary>
    /// Function called to start the next sequential stage in the level.
    /// Condition for Starting the Next Stage is: the player needs to reach the next Cover Rock. 
    /// </summary>
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
        else if (stageNum == 3)//&& curSongTime >= stage3StartTime)
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
        else if (evt == "Song2_Stage3")
            Debug.Log("[Level Manager]: Skip to Song2 Stage 3");
        else
        {
            Debug.Log(evt);
        }
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
        float spawnRate = 2;
        /*Logic for managing shell projectiles in scene
        */
        //Only spawn shells if the player is in a battle sequence. Ie. NotMoving and Not in a cutscene
        if (gController.playerControl.playerState != PlayerState.MOVING)
        {
            //StartCoroutine(SpawnShellPickUp(spawnRate));

            if (shellSpawnTime >= spawnRate && shellArray.Count < maxShellCount)
            {
                ShellPickup tempShell = pickupShells[Random.Range(0, pickupShells.Length)];
                ShellPickup reloadShell = Instantiate(tempShell, shellLocs[Random.Range(0, shellLocs.Length)].position, tempShell.transform.rotation);
                shellArray.Add(reloadShell);
                Debug.Log("pickup shell spawned");
                shellSpawnTime = 0;
            }
            else
            {
                shellSpawnTime += Time.deltaTime;
            }

            //only spawn shells if this bool tells it to
            /*if (shellArray.Count < maxShellCount)
            {
                StartCoroutine(SpawnShellPickUp(spawnRate));
                //numOfShells += 1;
            }*/
        }

    }

    IEnumerator SpawnShellPickUp(float shellSpawnRate)
    {
        if (shellArray.Count < maxShellCount)
        {
            float time = 0;
            while (time <= shellSpawnRate)
            {
                time += Time.deltaTime;
                yield return null;
            }
            //choose random spawn location in array and spawn there
            ShellPickup tempShell = pickupShells[Random.Range(0, pickupShells.Length)];
            ShellPickup reloadShell = Instantiate(tempShell, shellLocs[Random.Range(0, shellLocs.Length)].position, tempShell.transform.rotation);
            shellArray.Add(reloadShell);
            Debug.Log("pickup shell spawned");
        }

        yield return 0;
    }

    //event function for when a shell is pickedup
    void Evt_ShellPickedUp(string str)
    {
        for (int i = 0; i < shellArray.Count; i++)
        {
            //add a object == nil check?
            if (shellArray[i].name == str)
                shellArray.RemoveAt(i);
        }
        //numOfShells -= 1;
    }


    /***************
     *****Level Stage Functions ****************
     ************/

    public void Stage0()
    {
        stageNum = 0;
        songStartTime = Time.time;
        darkBoss.gameObject.SetActive(true);

        StartCoroutine(Stage0Routine());

    }
    public void Stage1()
    {
        stageNum = 1;
        songStartTime = stage1StartTime;
        StartCoroutine(Stage1Routine());
    }

    public void Stage2()
    {
        stageNum = 2;
        songStartTime = stage2StartTime;
        StartCoroutine(Stage2Routine());
    }

    public void Stage3()
    {
        stageNum = 3;
        songStartTime = stage3StartTime;
        StartCoroutine(Stage3Routine());
    }

    /// <summary>
    /// Cover Destroyed Function. Is called once player's rock cover is destroyed.
    /// </summary>
    /// <returns></returns>
    public IEnumerator OnPlayerCoverDestroyed()
    {
        //slight delay after rock is destroyed  before player should run
        yield return new WaitForSeconds(.5f);
        if (stageNum < 2) //only run to new rock if not on last stage of battle
            StartCoroutine(RunToNewRock(1));
        yield return 0;
    }

    //function to make player run to next rock when old rock is destroyed Via Cam pathSpline
    public IEnumerator RunToNewRock(int rockNum)
    {
        /*
        * Prevent Enemies from Attacking Player
        */
        darkBoss.status = DarknessBoss.BossStatus.rest;
        //gController.playerControl.GetComponent<BoxCollider>().enabled = false;

        //begin spline
        pathControl.Play();
        gController.playerControl.CanMove = false;
        gController.playerControl.playerState = PlayerState.MOVING;

        yield return new WaitForSeconds(1);
        Transform playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        //SpeedBoost spdBoost = new SpeedBoost();
        SpeedBoost spdBoost = Instantiate(speedBoostPowerUp, playerPos.position, playerPos.rotation);
        //EventManager.TriggerEvent("Player_SpeedBoost", "Player_SpeedBoost");
        yield return 0;
    }

    /*
     * Function that Pauses Spline for when the player reaches a new rock ONLY.
     */
    public void PauseSplineEndStage()
    {
        //pathControl.Pause();
        pathControl.pPathState = CamPathState.Paused;
        gController.playerControl.CanMove = true;
        gController.playerControl.playerState = PlayerState.NOTMOVING;

        //remove old shells from previous area
        foreach (ShellPickup s in shellArray)
        {
            //shellArray.Remove(s);
            if(s)
                Destroy(s.gameObject);
        }
        shellArray.Clear();
        //destroy ALL darkness previously spawned to start over
        //AI_Manager.Instance.KillAllDarkness();

        //increment state number because player has reach next rock
        stageNum += 1;
        Debug.Log("Rock " + stageNum + " Reached.");

        /*
         * Allow Enemies To Attack Player
         */
        darkBoss.status = DarknessBoss.BossStatus.charging;
        //enabled Player's box collider
        //gController.playerControl.GetComponent<BoxCollider>().enabled = false;

        StartNextStage();
        //remove speed boost and other FX
        //gController.playerControl.Reset();
    }

    void StageReset()
    {
        //change skybox & lighting to night
        Color lightingColor_Night;
        Color lightingColor_Day;  //"B0B0B0"
        ColorUtility.TryParseHtmlString("#56577A", out lightingColor_Night);
        ColorUtility.TryParseHtmlString("#B0B0B0", out lightingColor_Day);

        print("day sky");
        RenderSettings.skybox = nightSkybox;
        RenderSettings.ambientLight = lightingColor_Day;

        //change lighting on rocks and environment objects
        float dayColor_rocks = 0f;
        float nightColor_rocks = 0.6f;
        float dayColor_clouds = 0f;
        float nightColor_clouds = 2f;
        rockMaterial.SetFloat("_LMPower", dayColor_rocks);
        rockMaterial_2.SetFloat("_LMPower", dayColor_rocks);
        cloudMaterial.SetFloat("_LMPower", dayColor_clouds);
    }

    /*
     * Stage Test Routine
     * Function for ONLY testing specific Stages of the battler
     */
    IEnumerator StageTestRoutine()
    {
        stageNum = 0;
        songStartTime = Time.time;
        darkBoss.gameObject.SetActive(true);
        Debug.Log("Stage 0 Start");
        //start rain
        RainFX.Play();
        RainFX_Wide.Play();
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



        //increase rain FX amount & intensity
        StartCoroutine(AdjustRainAmt(RainFX, rainEmissionRateMax));

        //begin spawning enemies
        enemySpawners.spawningEnemies = true;

        //move dark boss to the top of the water plane
        float moveTime = 4;
        t = 0;
        Vector3 startPos = darkBossObj.transform.position;
        Vector3 endPos = new Vector3(startPos.x, GameObject.Find("OceanSurfaceQuad").transform.position.y, startPos.z);

        /*
         * [PLAY BG MUSIC] start playing bg song
         */ 
        gController.soundManager.FadeInMusic(1);

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
        // tempWaitTime = 5;
        //This is the length of time the rest of the stage should play out
        yield return new WaitForSeconds(waitTime);
        //wait for an event for when the rock is destroyed by the dark boss.
        stageNum = 2;
        darkBoss.DoRockSmash_Interrupt(coverRocks[0]);

        yield return new WaitForSeconds(10);
        stageNum = 3;
        Debug.Log("Rock " + stageNum + " Reached.");
        StartNextStage();
        yield return 0;

    }
    IEnumerator Stage0Routine()
    {
        Debug.Log("Stage 0 Start");
        //start rain
        RainFX.Play();
        RainFX_Wide.Play();
        float rainIncreaseTime = 5f;
        float t = 0;

        yield return new WaitForSeconds(5);
        //display eye-blink overrlay and block the change of the skybox
        animator2D.Play("eyeClose");
        yield return new WaitForSeconds(0.1f);
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 1f);
        yield return new WaitForSeconds(0.1f);

        //change skybox & lighting to night
        Color lightingColor_Night;
        Color lightingColor_Day;  //"B0B0B0"
        ColorUtility.TryParseHtmlString("#56577A", out lightingColor_Night);
        ColorUtility.TryParseHtmlString("#B0B0B0", out lightingColor_Day);

        print("night sky");
        RenderSettings.skybox = nightSkybox;
        RenderSettings.ambientLight = lightingColor_Night;

        //change lighting on rocks and environment objects
        float dayColor_rocks = 0f;
        float nightColor_rocks = 0.6f;
        float dayColor_clouds = 0f;
        float nightColor_clouds = 2f;
        rockMaterial.SetFloat("_LMPower", nightColor_rocks);
        rockMaterial_2.SetFloat("_LMPower", nightColor_rocks);
        cloudMaterial.SetFloat("_LMPower", nightColor_clouds);

        yield return new WaitForSeconds(0.3f);

        //Display eye-open animation and show world. disable black overlay too
        animator2D.Play("eyeOpen");
        blackOverlay.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);

        foreach (GameObject bolt in lightningBolts)
        {
            bolt.SetActive(true);
            bolt.GetComponent<LightningBoltScript>().Trigger();
        }
        
        //TEST LIGHTNING FLASH
        GameObject flashSphere = GameObject.Find("FlashSphere");
        Renderer renderer = flashSphere.GetComponent<Renderer>();
        Material mat = flashSphere.GetComponent<Renderer>().material;
        Color alpha1 = new Color(mat.color.r, mat.color.g, mat.color.b, 0f);
        Color alpha2 = new Color(mat.color.r, mat.color.g, mat.color.b, .2f);
        Color alpha3 = new Color(mat.color.r, mat.color.g, mat.color.b, .5f);
        float elapsedTime = 0;
        float time = .02f;
        int flashCount = 0;
        int maxFlashCount = 12;
        Debug.Log(renderer.material.HasProperty("_Color"));

        renderer.material.SetColor("_Color",alpha2);
        while (flashCount < maxFlashCount)
        {
            if (renderer.material.color == alpha2)
            {
                while (elapsedTime < time)
                {
                    renderer.material.color = Color.Lerp(renderer.material.color, alpha3, elapsedTime / time);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                renderer.material.color = alpha3;
            }
            else if (renderer.material.color == alpha3)
            {
                while (elapsedTime < time)
                {
                    renderer.material.color = Color.Lerp(renderer.material.color, alpha2, elapsedTime / time);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                renderer.material.color = alpha2;
            }
            else
            {
                while (elapsedTime < time)
                {
                    renderer.material.color = Color.Lerp(renderer.material.color, alpha1, elapsedTime / time);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                renderer.material.color = alpha1;
            }
            elapsedTime = 0;
            flashCount += 1;
            yield return null;
        }

        //destroy lightning bolts
        foreach (GameObject bolt in lightningBolts)
        {
            //bolt.SetActive(false);
        }

        //reset color to transparent while (elapsedTime < time)
        while (elapsedTime < time)
        {
            renderer.material.color = Color.Lerp(renderer.material.color, alpha1, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        renderer.material.color = alpha1;
        //renderer.material.SetColor("_Color", alpha1);

        //increase rain FX amount & intensity
        StartCoroutine(AdjustRainAmt(RainFX, rainEmissionRate_stage1));

        //begin spawning enemies
        enemySpawners.spawningEnemies = true;

        //move dark boss to the top of the water plane
        float moveTime = 4;
        t = 0;
        Vector3 startPos = darkBossObj.transform.position;
        Vector3 endPos = new Vector3(startPos.x, GameObject.Find("OceanSurfaceQuad").transform.position.y, startPos.z);

        /*
         * [PLAY BG MUSIC] start playing bg song
         */
        gController.soundManager.FadeInMusic(2);
        //BG song now playing, set startTime
        songStartTime = Time.time;

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
        darkBoss.ballMoveSpeed = darkBoss.ballMoveSpeed + bossAttackSpeed_stage1;

        enemySpawners.spawnRate = darkSpawnRate_Stage0;
        float waitTime = stage1StartTime - curSongTime;
        Debug.Log("time till next stage = " + (waitTime * (.5f)));

        /*
         * TIME LOOP to replace WaitForSeconds
         */

        //create custom internal timer
        //only iterates through loop while the world state is not paused. so this should help control coroutine better
        float savedSongTime = curSongTime;
        float tempWaitTime = waitTime * .5f;
        float timer = 0f;
        Debug.Log("wait time " + tempWaitTime);
        while ((timer < tempWaitTime))
        {
            if (gController.gameState != GameState.IsWorldPaused)
            {
                timer += Time.deltaTime;
                //savedSongTime += Time.deltaTime; //count by seconds passed
                //curWaitTime = tempWaitTime - savedSongTime;                
            }
            //MUST INCLUDE WAITFORSECONDS FOR while loops. so wait for 1 millisecond
            yield return null;
        }

        //This is the length of time the rest of the stage should play out
        //yield return new WaitForSeconds(waitTime * (.5f));

        //Pause coroutine until player is not in HURT state
        /*while (gController.playerControl.playerStatus == PLAYER_STATUS.HURT)
            yield return null; */

        /*
         * Stage1.2 - wave 2
        */
        print("Enemy Wave 2");
        enemySpawners.spawnRate = darkSpawnRate_Stage0_1;
        AI_Manager.Instance.maxEnemyCount = 7;

        Debug.Log("time till next stage = " + (waitTime * (.5f)));
        //yield return new WaitForSeconds(waitTime * .5f);
        tempWaitTime = waitTime * .5f;
        timer = 0f;
        Debug.Log("wait time " + tempWaitTime);
        while ((timer < tempWaitTime))
        {
            if (gController.gameState != GameState.IsWorldPaused)
            {
                timer += Time.deltaTime;               
            }
            //MUST INCLUDE WAITFORSECONDS FOR while loops. so wait for 1 millisecond
            yield return null;
        }

        //Pause coroutine until player is not in HURT state
        while (gController.playerControl.playerStatus == PLAYER_STATUS.HURT)
            yield return null;

        //start attack to destroy the player's cover rock
        //wait for an event for when the rock is destroyed by the dark boss.
        darkBoss.DoRockSmash_Interrupt(coverRocks[0]);

        //After dark boss as destroyed the rock, Trigger the RunToNewRock CoRoutine...

        yield return 0;
    }


    IEnumerator Stage1Routine()
    {
        Debug.Log("Stage 1 Start");
        //Set Enemy Stage to face the Player Container Position
        RainFX.GetComponent<rainSpawnScript>().enabled = false;
        RainFX_2.Play();
        //increase rain FX of stage 2 
        StartCoroutine(AdjustRainAmt(RainFX_2, rainEmissionRate_stage2));
        //decrease rainFX of stage1 to 0
        StartCoroutine(AdjustRainAmt(RainFX, 0));

        Transform target = gController.playerControl.GetComponent<NFPSController>().playerContainer.transform;
        float rotateTime = 2f;
        float curTime = 0;

        while (curTime <= rotateTime)
        {
            //find the vector pointing from our position to the target
            Vector3 direction = (target.position - enemyStageObj.transform.position).normalized;
            Quaternion lookDirection = Quaternion.LookRotation(direction);

            //rotate us over time according to speed until we are in the required rotation
            enemyStageObj.transform.rotation = Quaternion.Slerp(enemyStageObj.transform.rotation, lookDirection, Time.deltaTime * rotateTime);
            curTime += Time.deltaTime;
            yield return null;
        }
        //enemyStageObj.transform.LookAt(gController.playerControl.GetComponent<NFPSController>().playerContainer.transform,);
        //Reset Dark Boss rotation
        darkBossObj.transform.rotation = new Quaternion(0, 0, 0, 0);

        //snap player transform to face dark boss after reaching the new stage position
        gController.playerStage.transform.LookAt(darkBoss.transform);

        enemySpawners.spawnRate = darkSpawnRate_Stage1;
        
        AI_Manager.Instance.maxEnemyCount = 15;
        //Decrease Time between attacks
        darkBoss.maxAttackTimer = darkBoss.baseMaxAttackTimer - bossTimeBtwAttacks_stage1;
        //Increase speed of dark ball attack
        darkBoss.ballMoveSpeed = darkBoss.ballMoveSpeed + bossAttackSpeed_stage1;
        //resume spawning AI for next wave
        enemySpawners.pauseSpawning = false;

        float waitTime = stage2StartTime - songStartTime;
        Debug.Log("time till next stage = " + waitTime);
       // float tempWaitTime = 5;
        //This is the length of time the rest of the stage should play out
        yield return new WaitForSeconds(waitTime);
        //darkBoss.DoRockSmash_Interrupt();

        //Pause coroutine until player is not in HURT state
        while (gController.playerControl.playerStatus == PLAYER_STATUS.HURT)
            yield return null;

        //wait for an event for when the rock is destroyed by the dark boss.
        darkBoss.DoRockSmash_Interrupt(coverRocks[1]);

        //After dark boss as destroyed the rock, Trigger the RunToNewRock CoRoutine...
        yield return 0;
    }

    IEnumerator Stage2Routine()
    {
        Debug.Log("Stage 2 Start");
        RainFX_2.GetComponent<rainSpawnScript>().enabled = false;
        RainFX_3.Play();
        //increase rain FX of stage 2 
        StartCoroutine(AdjustRainAmt(RainFX_3, rainEmissionRate_stage3));
        //decrease rainFX of stage1 to 0
        StartCoroutine(AdjustRainAmt(RainFX_2, 0));

        //Set Enemy Stage to face the Player Container Position
        //Transform target = gController.playerControl.GetComponent<NFPSController>().transform;
        Transform target = coverRocks[2].transform;
        float rotateTime = 2f;
        float curTime = 0;

        while (curTime <= rotateTime)
        {
            //find the vector pointing from our position to the target
            Vector3 direction = (target.position - enemyStageObj.transform.position).normalized;
            Quaternion lookDirection = Quaternion.LookRotation(direction);

            //rotate us over time according to speed until we are in the required rotation
            enemyStageObj.transform.rotation = Quaternion.Slerp(enemyStageObj.transform.rotation, lookDirection, Time.deltaTime * rotateTime);
            curTime += Time.deltaTime;
            yield return null;
        }

        //snap player transform to face dark boss after reaching the new stage position
        gController.playerStage.transform.LookAt(darkBoss.transform);

        //increase enemy count and enemy spawn rate
        enemySpawners.spawnRate = darkSpawnRate_Stage2;
        AI_Manager.Instance.maxEnemyCount = 15;
        //Decrease Time between attacks
        darkBoss.maxAttackTimer = darkBoss.baseMaxAttackTimer - bossTimeBtwAttacks_stage2;
        //Increase speed of dark ball attack
        darkBoss.ballMoveSpeed = darkBoss.ballMoveSpeed + bossAttackSpeed_stage2;

        float waitTime = stage3StartTime - songStartTime;
        Debug.Log("time till next stage = " + waitTime);
        //float tempWaitTime = 5;
        //This is the length of time the rest of the stage should play out
        yield return new WaitForSeconds(waitTime);

        //move dark boss towards the player
        float moveTime = 2;
        float t = 0;
        Vector3 startPos = darkBossObj.transform.position;
        Vector3 endPos = gController.playerStage.transform.position;

        while (t <= moveTime)
        {
            //slowly move darkboss to above the water and begin its attack
            darkBossObj.transform.position = Vector3.Lerp(startPos, endPos, t / 10);
            t += Time.deltaTime;
            yield return null;
        }
        //darkBossObj.transform.position = Vector3.MoveTowards(darkBossObj.transform.position, gController.playerStage.transform.position, 2f);


        //wait for an event for when the rock is destroyed by the dark boss.
        //tell boss to destroy the player's last cover rock
        darkBoss.DoRockSmash_Interrupt(coverRocks[2]);
        yield return new WaitForSeconds(4);

        //!Dark Boss moves closer!

        //!!Dark Boss Grows larger...!!

        stageNum += 1;
        Debug.Log("Rock " + stageNum + " Reached.");
        StartNextStage();
        //yield return new WaitForSeconds(10);

        //darkBoss.DoRockSmash_Interrupt(gController.playerControl.gameObject);

        //start playing bg song
        //gController.soundManager.FadeInMusic(1);

    }

    //[Stage3] Final Stand, no more cover during this stage
    IEnumerator Stage3Routine()
    {
        Debug.Log("Stage 3 Start");

        //Set Enemy Stage to face the Player Container Position
        //Transform target = gController.playerControl.GetComponent<NFPSController>().transform;
        Transform target = gController.playerControl.gameObject.transform;
        float rotateTime = 2f;
        float curTime = 0;

        while (curTime <= rotateTime)
        {
            //find the vector pointing from our position to the target
            Vector3 direction = (target.position - enemyStageObj.transform.position).normalized;
            Quaternion lookDirection = Quaternion.LookRotation(direction);

            //rotate us over time according to speed until we are in the required rotation
            enemyStageObj.transform.rotation = Quaternion.Slerp(enemyStageObj.transform.rotation, lookDirection, Time.deltaTime * rotateTime);
            curTime += Time.deltaTime;
            yield return null;
        }

        //snap player transform to face dark boss after reaching the new stage position
        gController.playerStage.transform.LookAt(darkBoss.transform);
        enemyStageObj.transform.LookAt(gController.playerControl.gameObject.transform);

        //The Darkness should overwhelm player in 20 seconds, increase spawn rate
        enemySpawners.spawnRate = darkSpawnRate_Stage3;
        AI_Manager.Instance.maxEnemyCount = 20;
        StartCoroutine(AdjustRainAmt(RainFX_3, rainEmissionRate_stage3End));

        //Decrease Time between attacks
        darkBoss.maxAttackTimer = darkBoss.baseMaxAttackTimer - bossTimeBtwAttacks_stage3; 
        //Increase speed of dark ball attack
        darkBoss.ballMoveSpeed = darkBoss.ballMoveSpeed + bossAttackSpeed_stage3;

        float waitTime = stage3EndTime - songStartTime;
        Debug.Log("time till end = " + waitTime);
        //float tempWaitTime = 5;
        yield return new WaitForSeconds(waitTime);

        //ijncrease size of hand to guarantee collision with player
        Vector3 boxSize = darkBoss.darkHand.GetComponent<BoxCollider>().size;
        darkBoss.darkHand.GetComponent<BoxCollider>().size = new Vector3(boxSize.x, .1f, .1f);

        darkBoss.DoRockSmash_Interrupt(gController.playerControl.gameObject);
        //yield return new WaitForSeconds(4f);
        yield return new WaitUntil(() => darkBoss.GetComponent<Animator>().GetBool("DoRockSmashAttack") == true);
        yield return new WaitForSeconds(1f);
        gController.timeManager.DoSlowMo(15f);
        yield return new WaitUntil(() => gController.timeManager.slowMoOn == false);


        //StartCoroutine(Song2_EndCutscene());

        //start playing bg song
        //gController.soundManager.FadeInMusic(1);

    }

    /*
     * Function is called by "Rock Smash" collision event in NFPSController.cs
     **/ 
    IEnumerator Song2_EndCutscene()
    {
        //player flies towards the beach
        Transform playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        speedBoostPowerUp.boostAmt = 2f;
        speedBoostPowerUp.speedTime = pathControl_EndScene.animationTime;

        //begin spline
        pathControl_EndScene.Play();
        SpeedBoost spdBoost = Instantiate(speedBoostPowerUp, playerPos.position, playerPos.rotation);
        //EventManager.TriggerEvent("Player_SpeedBoost", "Player_SpeedBoost");
        gController.playerControl.CanMove = false;
        gController.playerControl.playerState = PlayerState.MOVING;
        yield return 0;
    }

    void OnPlayerHitIsland(string evt)
    {
        StartCoroutine(PlayerHitIslandRoutine());
    }

    IEnumerator PlayerHitIslandRoutine()
    {
        float time = 0;
        float screenFadeInTime = 1f;
        Rigidbody pContainerBody = gController.playerControl.GetComponent<NFPSController>().playerContainer.GetComponent<Rigidbody>();
        pContainerBody.isKinematic = true;
        pContainerBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        //play "THUD" sound + VFX when player hits the island + camera shake
        GameObject impactFX = Instantiate(dirtImpactFX, gController.playerControl.transform.position, Quaternion.identity);
        //Cancel speed FX when player hits island
        EventManager.TriggerEvent("CancelPowerUps", "CancelPowerUps");
        EventManager.TriggerEvent("Player_SpeedBoostOff", "Player_SpeedBoost");
        gController.playerControl.CanMove = false;
        gController.playerControl.playerState = PlayerState.NOTMOVING;

        /* [CREEP Ending]
         * MOVE Dark Boss to the Island and slowly creep into player's view
         */ 
        float tempSpeed = 3;
        //Teleport Boss close to player's position
        darkBossObj.transform.position = new Vector3(darkBossEndPos.transform.position.x-5, darkBossEndPos.transform.position.y, darkBossEndPos.transform.position.z-5);
        //set boss to look in player's direction on the island
        darkBossObj.transform.LookAt(gController.playerControl.GetComponent<NFPSController>().playerContainer.transform);
        //rotate boss to face downwards and in player's view
        darkBossObj.transform.localRotation = new Quaternion(23.365f, darkBossObj.transform.rotation.y, darkBossObj.transform.rotation.z, darkBossObj.transform.rotation.w);

        //slowly move boss towards player's position within their view
        yield return new WaitForSeconds(1);
        float t = 0;
        float moveTime = 3;
        while (t < moveTime)
        {
            darkBossObj.transform.position = Vector3.MoveTowards(darkBossObj.transform.position, darkBossEndPos.transform.position, Time.deltaTime * tempSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(3);
        
        Destroy(impactFX);
        
        /*
         *IMPROVE EYE CLOSE - Passout animation.  make the eye-close slower and more dramatic! 
         * 
         */
          
        //scene fades to black as player's eyes close
        if (blackOverlay != null)
        {
                //display eye-blink overrlay
                animator2D.Play("eyeClose");
                yield return new WaitForSeconds(0.1f);
                blackOverlay.gameObject.SetActive(true);
                blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 1f);
                yield return new WaitForSeconds(0.1f);
        }
        //FADE OUT Music
        gController.soundManager.StopBGAudio();
        Debug.Log("song finished");
    }

    /*
     *Adjust Rain Effects to be more/less intense by amount and speed 
     */ 
    IEnumerator AdjustRainAmt(ParticleSystem rainFX, int newRainEmissionRate)
    {
        float t = 0;
        float rainIncreaseTime = 5;
        //increase the rains RateOverTime gradually - MORE INTENSE & Faster
        while (t < rainIncreaseTime)
        {
            UnityEngine.ParticleSystem.EmissionModule em = rainFX.emission;
            UnityEngine.ParticleSystem.EmissionModule em2 = RainFX_Wide.emission;
            float currentRate = em.rateOverTime.constant;
            em.rateOverTime = Mathf.Lerp(currentRate, newRainEmissionRate, t / rainIncreaseTime);

            float currentRate2 = em2.rateOverTime.constant;
            em2.rateOverTime = Mathf.Lerp(currentRate2, newRainEmissionRate-2, t / rainIncreaseTime);

            t += Time.deltaTime;
            yield return null;
        }
    }

    void SmoothLookAtRotate(GameObject targetObj, float speed)
    {
        var targetRotation = Quaternion.LookRotation(targetObj.transform.position - transform.position);

        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
    }
}
