using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public enum GameState { IsPlaying, IsPaused, IsOver, InMenu, IsWorldPaused }
public enum GameLevelSelectionState {LevelSelected, Continue, NewGame}

public class GameController : MonoBehaviour {

    [SerializeField]
    public PlayerController playerControl;
    [SerializeField]
    public SoundManager soundManager;
    [SerializeField]
    public LevelManager lvlManager;
    [SerializeField]
    public Camera UICamera;
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    GameObject debugMenuPrefab;

    public CameraPathAnimator pathControl; //reads value from level script
    public TimeManager timeManager;
    GameObject player;
    GameObject wave;
    GameObject waveTunnel;
    GameObject[] waterParticles;
    GameObject darkRoom;
    float scene1Time = 5f;
    ParticleSystem HolyLight;

    public GameState gameState { get; set; }
    public GameLevelSelectionState gameLevelSelectionState { get; set; }
    public Image blackOverlay, dmgOverlay;


    MagicFish specialFish;
    bool showDebug=false;
    private GameObject debugMenuObj;
    List<string> s1Events;
    


    private static GameController gameController;

    public static GameController instance
    {
        get
        {
            if (!gameController)
            {
                gameController = FindObjectOfType(typeof(GameController)) as GameController;

                if (!gameController)
                {
                    Debug.LogError("There needs to be one active GameController script on a GameObject in your scene.");
                }
            }
            return gameController;
        }

    }


    

    // Use this for initialization
    void Start () {

        //set unity player to run even when video is not in focus
        Application.runInBackground = true;
        Input.backButtonLeavesApp = true;


        //set default state to "paused"
        //splineControl.sSplineState = SplineState.Paused;

    }

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (instance != gameController)
        {
            Destroy(gameObject);
            return;
        }

        /*
         * 
         * Load other managers
         *
         */
        uiManager = this.GetComponent<UIManager>(); //FindObjectOfType(typeof(UIManager)) as UIManager;
        soundManager = FindObjectOfType(typeof(SoundManager)) as SoundManager;            
        lvlManager = this.GetComponent<LevelManager>(); //FindObjectOfType(typeof(LevelManager)) as LevelManager;

        /*
         * LOAD RESOURCES on AWAKE
         * load different resources based upon the Scene loaded
         */
         
        EventManager.StartListening("Player_Stop", ResetPlayer);
        EventManager.StartListening("PauseWorld", OnWorldPaused);
        EventManager.StartListening("PauseGame", OnGamePaused);
        EventManager.StartListening("MenuSelect_ResumeGame", OnGameResume);
        EventManager.StartListening("MenuSelect_StartGame", OnStartGame);
        EventManager.StartListening("MenuSelect_LevelSelectGame", OnStartGameFromLevelSelect);


        //timeManager = GetComponent<TimeManager>();
        Time.fixedDeltaTime = Time.timeScale * .02f;
        EventManager.StartListening("StopAllTempAudio", delegate { DebugFunc("StopAudio"); });

      

    }

    //SCRIPT runs right after a level is loaded.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            Debug.Log("MainMenu");
            //set gamestate
            gameState = GameState.InMenu;
        }
        else
        {
            gameState = GameState.IsPlaying;
            lvlManager.OnSceneLoaded(scene, mode);

            //Assign Variables from new Scene
            playerControl = GameObject.FindGameObjectWithTag("PlayerCube").GetComponent<PlayerController>();
            UICamera = playerControl.UICamera;
            //uiManager = playerControl.UICamera.GetComponent<UIManager>();
            //get Camera Path Controller from current level
            pathControl = lvlManager.currentLvl.pathControl;

            pathControl.AnimationPausedEvent += delegate { DebugFunc("Paused"); };
            pathControl.AnimationFinishedEvent += delegate { DebugFunc("Finished"); };

            LoadSceneObjects();
            Debug.Log("Level Loaded: " + lvlManager.currentLvl.levelNum);
            EventManager.TriggerEvent("LevelLoaded", "level loaded");

            
        }
    }

    /// <summary>
    /// LoadSceneObjects -Function determines what objects to load based upon the scene
    /// </summary>
    private void LoadSceneObjects()
    {
        if (SceneManager.GetActiveScene().name == "Song1_V2")
        {
            waterParticles = GameObject.FindGameObjectsWithTag("WaterParticle");
            //there are 3 particles now, need to make this dynamic later
            //order particles are currently added to array: greastest to least = 1st item is the last item in the array
            waterParticles[0].SetActive(false);
            waterParticles[1].SetActive(true);
            waterParticles[2].SetActive(true);

            darkRoom = GameObject.Find("DarkRoom");

            //subscribe to Scene1Event event call
            EventManager.StartListening("Scene1Event", Scene1Events);
            s1Events = new List<string> { "waterBubbles", "" };

            blackOverlay = GameObject.Find("BlackOverlay").GetComponent<Image>();
            blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 0);
            blackOverlay.gameObject.SetActive(false);

            player = GameObject.Find("PlayerCube").gameObject;
            wave = GameObject.Find("Wave").gameObject;
            waveTunnel = GameObject.Find("WaveTunnel").gameObject;
            waveTunnel.SetActive(false);
            //hide wave asset
            Color waveColor = wave.GetComponent<Renderer>().material.color;
            waveColor = new Color(waveColor.r, waveColor.g, waveColor.b, 0f);
            wave.GetComponent<Renderer>().material.color = waveColor;

            /*DISABLE SPECIAL FISH TEMPORARILY
            specialFish = GameObject.Find("ClownFish_Special").GetComponent<MagicFish>();
            specialFish.gameObject.SetActive(false);
            */


            //START LEVEL LOGIC
            pathControl.playOnStart = false;
            StartCoroutine(Level1Start());
        }
        else if (SceneManager.GetActiveScene().name == "Song2")
        {

            dmgOverlay = GameObject.Find("DmgOverlay").GetComponent<Image>();
            blackOverlay = GameObject.Find("BlackOverlay").GetComponent<Image>();

            song2_lvl lvl2 = GameObject.Find("Song2_LevelObj").GetComponent<song2_lvl>();
            //song2_lvl lvl2 = (song2_lvl)lvlManager.levelList[0];
            lvl2.Initialize();
        }

        uiManager.Initialize();
        pathControl.AnimationPausedEvent += delegate { DebugFunc("Paused"); };
        pathControl.AnimationFinishedEvent += delegate { DebugFunc("Finished"); };
        EventManager.StartListening("StopAllTempAudio", delegate { DebugFunc("StopAudio"); });
    }


    /// <summary>
    /// OnGameStart -> Handles loading of game assets, choosing of level to load, and starting game logic
    /// </summary>
    private void OnStartGame(string evt)
    {
        //load last level from save state
        lvlManager.LoadLevel(1); //temp, load 1st level
        gameLevelSelectionState = GameLevelSelectionState.Continue;
    }

    /// <summary>
    /// OnStartGame From LevelSelect Menu
    /// </summary>
    /// <param name="evt"></param>
    private void OnStartGameFromLevelSelect(string evt)
    {
        gameLevelSelectionState = GameLevelSelectionState.LevelSelected;
    }

    /// <summary>
    /// OnLevelFinished Function called when Level script declares level as finished.  Event Call
    /// </summary>
    /// <param name="evt"></param>
    private void OnLevelFinished(string evt)
    {
        if(gameLevelSelectionState == GameLevelSelectionState.Continue || gameLevelSelectionState == GameLevelSelectionState.NewGame)
        {
            lvlManager.LoadNextLevel(evt);
        }
    }

    public void OnLoadNextLevel(string evt)
    {

    }

    public void OnGameResume(string evt)
    {
        Time.timeScale = 1;
        Debug.Log("Game UnPaused");
        gameState = GameState.IsPlaying;
        soundManager.ResumeBGAudio();
    }

    public void OnGamePaused(string evt)
    {
        Time.timeScale = 0;
        Debug.Log("Game Paused");
        gameState = GameState.IsPaused;
        soundManager.PauseBGAudio();
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

    // Update is called once per frame
    void Update()
    {
        //testing mouse input code for Pause Button
        if (Input.GetMouseButtonDown(0))
        {
            /*
            const float maxDistance = 100f;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(GetRayForDistance(distance));
            Ray finalRay = Camera.main.ScreenPointToRay(screenPoint);

            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance).OrderBy(h => h.distance).ToArray();
            Debug.Log(hits.Length);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                Debug.Log(hit.transform.name);
                if (hit.collider.gameObject.CompareTag("Pause Button"))
                {
                    Debug.Log("pause button hit");
                    // do something with this object
                }
            }
            */
        }

        // Exit when (X) is tapped.
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            EventManager.TriggerEvent("PauseGame", "PauseGame");
        }

        //Press 'W' to pause the game's world state and update. Causes everything to stop moving except for the player.
        if(Input.GetKeyDown(KeyCode.W))
        {
            if (gameState == GameState.IsPlaying)
            {
                gameState = GameState.IsWorldPaused;
                EventManager.TriggerEvent("PauseWorld", "PauseWorld");
            }
            else if (gameState == GameState.IsWorldPaused)
            {
                gameState = GameState.IsPlaying;
                EventManager.TriggerEvent("ResumeWorld", "ResumeWorld");
            }
        }

        //if "P" pressed, pause spline
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pathControl.pPathState == CamPathState.Paused) //unpause
            {
                pathControl.pPathState = CamPathState.Play;
                playerControl.CanMove = true;
            }
            else  //pause
            {
                pathControl.pPathState = CamPathState.Paused;
                playerControl.CanMove = false;
            }
        }
        //if "O" pressed, continue spline
        if (Input.GetKeyDown(KeyCode.O))
        {
            //splineControl.sSplineState = SplineState.Resume;
            playerControl.playerState = PlayerState.MOVING;
        }

        ///<Debug Stuff>
        /// Anything related to onscreen Debug Mode
        ///</Debug>

        if (Input.GetKeyDown(KeyCode.BackQuote)) //tilda
        {
            //show Debug Menu
            if (ShowDebug)
                ShowDebug = false;
            else
                ShowDebug = true;
        }
    }

    private bool ShowDebug
    {
        set
        {
            showDebug = value;
            if (showDebug == true && debugMenuObj == null)
            {
                debugMenuObj = Instantiate(debugMenuPrefab);
                
            }
            else
                Destroy(debugMenuObj);
        }
        get {
            return showDebug; }
    }

    //return player and associated effects back to default state
    private void ResetPlayer(string evt)
    {
        //cancel forces
        playerControl.playerState = PlayerState.NOTMOVING;
        playerControl.CanMove = false;
        playerControl.Reset();
        //cancel FX
        //cancel powerUps
    }

    private void OnWorldPaused(string evt)
    {
        if (gameState == GameState.IsWorldPaused)
        {
            Debug.Log("World State UnPaused");
            gameState = GameState.IsPlaying;
            soundManager.ResumeBGAudio();
        }
        else
        { 
            Debug.Log("World State Paused");
            gameState = GameState.IsWorldPaused;
            //Also PAUSE BG AUDIO
            soundManager.PauseBGAudio();
        }
    }
    /// <summary>
    ///  Scene Skip Implementation
    ///  1. Determine the new time to skip the scene too
    ///  2. Turn on player's camSpline until they are in the proper position for that stage or time in the level. (unless there is a way to skip the spline to the player's point...)
    /// (possibly use the start percentage and change it to match the new time
    ///  3. Change the BGM playback position to the new time 
    ///  4. Call function that start this section of the level
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="newBgmPlaybackTime"></param>
    public void SceneSkip(string evt, float newBgmPlaybackTime)
    {
        Debug.Log("[Scene Skip] Skip to " + evt);
        switch (evt)
        {
            case "Song2_Opening":
                break;
            case "Song2_Stage1":
                //change start percentage for camera path
                pathControl.startPercent = .3f;
                soundManager.ChangeBgmPlaybackPos(newBgmPlaybackTime);

                pathControl.Play();
                StartCoroutine(SceneSkipCoroutine(1));
                break;
            case "Song2_Stage2":
                //change start percentage for camera path
                pathControl.startPercent = .6f;
                soundManager.ChangeBgmPlaybackPos(newBgmPlaybackTime);

                pathControl.Play();
                StartCoroutine(SceneSkipCoroutine(2));
                break;
            case "Song2_Stage3":
                //change start percentage for camera path
                //pathControl.Play();
                //pathControl.startPercent = .6f;
                pathControl.Seek(.8f);                
                soundManager.ChangeBgmPlaybackPos(newBgmPlaybackTime);

                pathControl.Play();
                StartCoroutine(SceneSkipCoroutine(3));
                break;
        }
    }

    IEnumerator SceneSkipCoroutine(int section)
    {
        song2_lvl level = lvlManager.currentLvl.GetComponent<song2_lvl>();
        //start stage 1
        level.StopAllCoroutines(); //stop all current running coroutines in song2, which are stage coroutines
        switch (section)
        {
            case 1:
                level.Stage1();
                break;
            case 2:
                level.Stage2();
                break;
            case 3:
                level.Stage3();
                break;
        }
        yield return 0;
    }

    //function that contains what each event in song 1 should do
    void Scene1Events(string evt)
    { 
            switch (evt)
            {
				case "Song1_Opening":
					//StartCoroutine (Scene1());
                    break;
				case "waterParticle":
                    Debug.Log("water particle activated");
                    waterParticles[1].SetActive(true);
                    break;
                case "Song1_pt2":
                    StartCoroutine(Scene1_5());
                    break;
                /*case "Song1_pt3":
                    StartCoroutine(Scene1_6());
                    break;*/
                case "SceneEnd":
                    break;
                default:
                    break;
            }
        Debug.Log("[EVENT] " + evt);
    }


	IEnumerator Scene1()
	{
		float pt1Time = 10f; //35f
		float pt2Time = 10f;
		float fadeTime = .5f;
        float scene1Time = 60f;
		float time = 0;

        //restrict player movement
        playerControl.CanMove = false;
        //pathControl.pPathState = CamPathState.Paused;

        //dark room should rotate until scene is finished
        IEnumerator darkRoomThread = (DarkRoom(scene1Time));
        StartCoroutine (darkRoomThread);
        yield return new WaitForSeconds(pt1Time);

        //show a small light and slowly glows brighter
        HolyLight = GameObject.Find("HolyLight").GetComponent<ParticleSystem>();
        HolyLight.Play();
		yield return new WaitForSeconds (pt2Time);
        StopCoroutine(darkRoomThread);
		//Open dark room (fade away) & reveal ocean
		MeshRenderer roomMat = darkRoom.GetComponent<MeshRenderer>();
        Color initColor = roomMat.material.GetColor("_TintColor");
        Color fadeColor = new Color(initColor.r, initColor.g, initColor.b, 0);
        while (time < fadeTime) {
			roomMat.material.SetColor("_TintColor", Color.Lerp(initColor, fadeColor, time / fadeTime));
			time += Time.deltaTime;
			yield return null;
		}
        darkRoom.SetActive(false);

        //begin player movement
        //pathControl.pPathState = CamPathState.Play;
        //playerControl.playerState = PlayerState.MOVING;
        //playerControl.CanMove = true;

        //begin spline
        pathControl.Play();
        playerControl.CanMove = true;
        playerControl.playerState = PlayerState.MOVING;
    }

	IEnumerator DarkRoom(float rotTime)
	{
		float time = 0;
		float rotSpeed = 100;

        //get list of particles and activate them
        GameObject[] particles = GameObject.FindGameObjectsWithTag("particle_light");
        /*foreach( GameObject particle in particles)
        {
            particle.GetComponent<ParticleSystem>().Play();
        }*/

		//rotate until 1st part of scene is finished
		while(time < rotTime) 
		{
			darkRoom.transform.Rotate(Vector3.up * (rotSpeed * Time.deltaTime));
			time += Time.deltaTime;
			yield return null;
		}
		yield return null;
	}

    //player hits bursts from water and hits wave @3:00min
    IEnumerator Scene1_5()
    {
        //tell FishInteract scripts to stop following the player, fish will stop immediately
        EventManager.TriggerEvent("FishFollowStop", "FishFollowStop");
        //pause the spline
        playerControl.CanMove = false;
        pathControl.pPathState = CamPathState.Paused;

        //pause for x seconds and let player move around
        yield return new WaitForSeconds(3);
        //wave shows and hits player
        Debug.Log("Wave rushing to player");
        Color waveColor = wave.GetComponent<Renderer>().material.color;
        Color baseColor = waveColor;
        float time = 0;
        float fadeTime = 1f;
        //play waveincoming sound
        soundManager.PlayFoleySound(0,.8f);
        while (time < fadeTime)
        {
            wave.GetComponent<Renderer>().material.color = Color.Lerp(baseColor, new Color(waveColor.r, waveColor.g, waveColor.b, 1), time / fadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        Vector3 w_basePosition = wave.transform.position;
        time = 0f;
        float waveCrashTime = 3f;

        //show left arrow to warn player to look to the left at the wave
        uiManager.showArrow(0, wave);
        //set the impact position to be slightly further from the camera
        Vector3 crashPos = player.transform.position;

        while (time < waveCrashTime)
        {
            wave.transform.position = Vector3.Lerp(w_basePosition, crashPos, time / waveCrashTime);
            if((time/waveCrashTime) > .8f)
            {
                //cut to black
                wave.SetActive(false); //deactivate wave so can't be seen
                blackOverlay.gameObject.SetActive(true);
                blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 1f);
            }

            time += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Player hit by wave");
        //play waveHit sound
        soundManager.PlayFoleySound(1,1f);
        yield return new WaitForSeconds(.5f);
        //stop all extra audio playing, except BGMusic
        soundManager.StopAllTempAudio();           
        //make wave tunnel active for next event
        waveTunnel.SetActive(true);
        //turn off arrows manually
        uiManager.hideArrow(0);

        yield return new WaitForSeconds(3);
        //reset player rotation and position in the PlayerContainer
        //player.transform.rotation = Quaternion.Euler(player.transform.rotation.x,-90,player.transform.rotation.z);
        player.transform.localPosition = new Vector3(0, 0, 0);
        
        //show special fish
        /*Special Fish Evt - NOT USING ANYMORE
         * .gameObject.SetActive(true);

        time = 0f;
        float headRotTime = 3f;
        //rotate player to face ocean
        Vector3 playerRot = player.transform.rotation.eulerAngles; //rotation in euler angles
        Quaternion baseRot = player.transform.rotation;
        Quaternion nextRot = Quaternion.Euler(0, playerRot.y, playerRot.z);
        while (time < headRotTime)
        {
            player.transform.rotation = Quaternion.Lerp(baseRot, nextRot, (time / headRotTime));
            time += Time.deltaTime;
            yield return null;
        }
        bool playerFacingForward = false;
        //show right arrow to notify player to look forward
        //when player looks at special fish, continue
        //i am doing this, to reset the camera
        uiManager.showArrow(1, specialFish.gameObject);


        //pause until player looks forward at the special fish
        yield return new WaitUntil(() => (specialFish.activated == true));
        //wait until fish has faced player and dissapeared, then continue
        yield return new WaitForSeconds(specialFish.animTime+1);
        */
        
        //resume the spline, player should be beginning to move once they open their eyes
        playerControl.CanMove = true;
        //pathControl.pathSpeed = 7;
        //pathControl.topSpeed = 5;
        pathControl.pPathState = CamPathState.Play;

        //pause to allow camera to look towards wave and then open eyes
        yield return new WaitForSeconds(1.2f);

        //open eyes, and fade out black overlay
        playerControl.animator_EyeBlink.Play("eyeOpen");
        blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 0f);
        blackOverlay.gameObject.SetActive(false);

        yield return null;
    }

    IEnumerator Level1Start()
    {
        //set timer to show Title theme and begin spline
        float time = 0;
        float screenFadeOutTime = 1f;
        float headRotTime = 5f;


        //open eyes
        soundManager.PlayMusic(0);
        yield return new WaitForSeconds(3);
       

        /* Disable overlay for test purposes
        if (blackOverlay != null)
        {
            Color baseColor = blackOverlay.color;
            //fade sign out every second
            while (time < screenFadeOutTime)
            {
                Debug.Log("fading");
                blackOverlay.color = Color.Lerp(baseColor, new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 0f), time / screenFadeOutTime);
                time += Time.deltaTime;
                yield return null;
            }
        }*/

        float t = 0;
        
        //rotate player to face ocean
        Vector3 playerRot = player.transform.rotation.eulerAngles; //rotation in euler angles
        Quaternion baseRot = player.transform.rotation;
        Quaternion nextRot = Quaternion.Euler(0, playerRot.y, playerRot.z);
        Debug.Log("rotate head");
        while (t < headRotTime)
        {
            player.transform.rotation = Quaternion.Lerp(baseRot, nextRot, (t / headRotTime));
            t += Time.deltaTime;
            yield return null;
        }

        //have fish swim up to player and begin to lead player

        yield return new WaitForSeconds(1);

        //begin spline
        //pathControl.Play();
        //playerControl.CanMove = true;
        //playerControl.playerState = PlayerState.MOVING;

        StartCoroutine(Scene1());

        /*
         * Old code to begin spline
        //begin spline
        splineControl.sSplineState = SplineState.Start;
        playerControl.playerState = PlayerState.MOVING;
        Debug.Log("spline started");

        //get array of all spline nodes attached to the current spline root
        SplineNodeProperties[] splineNodes = splineControl.GetNodes();
        //find the nodes that have "cutscene" tags on them, these nodes do special things to the player and pause spline mvmt
        int splineNum = splineControl.getSplineIdx();

        //activate water particles as the player moves through the water
        while (splineNodes[splineNum].Name != "Scene1_waterParticle")
        {
            splineNum = splineControl.getSplineIdx();
            yield return null;
        }
        Debug.Log("water particle activated");
        waterParticles[1].SetActive(true);

        //At Scene 2, pause player movement and spline.  Player can only look around
        while (splineNodes[splineNum].Name != "Scene_2")
        {
            splineNum = splineControl.getSplineIdx();
            yield return null;
        }
        Debug.Log("spline paused");
        playerControl.CanMove = false;
        splineControl.sSplineState = SplineState.Stopped;

        //pause for x seconds and let player move around
        yield return new WaitForSeconds(3);
        playerControl.CanMove = true;
        splineControl.sSplineState = SplineState.Resume;

        yield return null;
        */
    }

}
