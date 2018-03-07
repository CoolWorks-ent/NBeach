using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum GameState { IsPlaying, IsPaused, IsOver, InMenu}

public class GameController : MonoBehaviour {

    [SerializeField]
    PlayerController playerControl;
    [SerializeField]
    public SoundManager soundManager;
    [SerializeField]
    public LevelManager lvlManager;
    [SerializeField]
    Camera UICamera;
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    public CameraPathAnimator pathControl;
    [SerializeField]
    GameObject debugMenuPrefab;

    GameObject player;
    GameObject wave;
    GameObject waveTunnel;
    GameObject[] waterParticles;
    GameObject darkRoom;
    float scene1Time = 5f;
    ParticleSystem HolyLight;

    public GameState gameState { get; set; }
    public Image blackOverlay, dmgOverlay;


    MagicFish specialFish;
    bool showDebug=false;
    private GameObject debugMenuObj;

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


    List<string> s1Events;

    // Use this for initialization
    void Start () {

        //set gamestate
        gameState = GameState.IsPlaying;

        //set unity player to run even when video is not in focus
        Application.runInBackground = true;
        Input.backButtonLeavesApp = true;

        //set default state to "paused"
        //splineControl.sSplineState = SplineState.Paused;

        /********************************
         * Song 1
         * ********************************/
        if (SceneManager.GetActiveScene().name == "Song1_V2")
        {
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

            specialFish = GameObject.Find("ClownFish_Special").GetComponent<MagicFish>();
            specialFish.gameObject.SetActive(false);

            pathControl.playOnStart = false;
            StartCoroutine(Level1Start());
        }
        if (SceneManager.GetActiveScene().name == "Song1_V2")
        {
            dmgOverlay = GameObject.Find("DmgOverlay").GetComponent<Image>();
            dmgOverlay.color = new Color(dmgOverlay.color.r, dmgOverlay.color.g, dmgOverlay.color.b, 0);
            dmgOverlay.gameObject.SetActive(false);
        }

        /********************************
         * Song 2
         * ********************************/
        if (SceneManager.GetActiveScene().name == "Song2")
        {
            song2_lvl lvl2 = (song2_lvl)lvlManager.levelList[0];
            lvl2.Initialize();
        }
    }

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Song1_V2")
        {
            waterParticles = GameObject.FindGameObjectsWithTag("WaterParticle");
            //there are 3 particles now, need to make this dynamic later
            //order particles are currently added to array: greastest to least = 1st item is the last item in the array
            waterParticles[0].SetActive(false);
            waterParticles[1].SetActive(true);
            waterParticles[2].SetActive(true);

			darkRoom = GameObject.Find ("DarkRoom");

            //subscribe to Scene1Event event call
            EventManager.StartListening("Scene1Event", Scene1Events);
            EventManager.StartListening("Player_Stop", ResetPlayer);
            s1Events = new List<string> { "waterBubbles", "" };
        }

        pathControl.AnimationPausedEvent += delegate { DebugFunc("Paused"); };
        pathControl.AnimationFinishedEvent += delegate { DebugFunc("Finished"); };
        EventManager.StartListening("StopAllTempAudio", delegate { DebugFunc("StopAudio"); });
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
        // Exit when (X) is tapped.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        //if "P" pressed, pause spline
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pathControl.pPathState == PathState.Paused) //unpause
            {
                pathControl.pPathState = PathState.Play;
                playerControl.CanMove = true;
            }
            else  //pause
            {
                pathControl.pPathState = PathState.Paused;
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
                case "SceneEnd":
                    break;
                default:
                    break;
            }
        Debug.Log("[EVENT] " + evt);
    }

    void Song2Events(string evt)
    {
        switch(evt)
        {
            case "Song2_Opening":
                break;
            case "Pause":
                lvlManager.currentLvl.GetComponent<song2_lvl>().PauseSpline();
                break;
            default:
                break;
        }
    }

	IEnumerator Scene1()
	{
		float pt1Time = 35f;
		float pt2Time = 10f;
		float fadeTime = .5f;
        float scene1Time = 60f;
		float time = 0;

        //restrict player movement
        playerControl.CanMove = false;
        //pathControl.pPathState = PathState.Paused;

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
        //pathControl.pPathState = PathState.Play;
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
        //pause the spline
        playerControl.CanMove = false;
        pathControl.pPathState = PathState.Paused;

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
        pathControl.topSpeed = 5;
        pathControl.pPathState = PathState.Play;

        //pause to allow camera to look towards wave and then open eyes
        yield return new WaitForSeconds(1.2f);

        //open eyes, and fade out black overlay
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
        pathControl.Play();
        playerControl.CanMove = true;
        playerControl.playerState = PlayerState.MOVING;

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
