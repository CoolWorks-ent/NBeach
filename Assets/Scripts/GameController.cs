using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    [SerializeField]
    PlayerController playerControl;
    [SerializeField]
    SoundManager soundManager;
    [SerializeField]
    CameraPathAnimator pathControl;

    GameObject player;
    SpriteRenderer blackOverlay;
    GameObject[] waterParticles;



    List<string> s1Events;

    // Use this for initialization
    void Start () {
        //set default state to "paused"
        //splineControl.sSplineState = SplineState.Paused;
        blackOverlay = GameObject.Find("BlackOverlay").GetComponent<SpriteRenderer>();
        blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 1);
        StartCoroutine(Level1Start());
        player = GameObject.Find("PlayerCube").gameObject;
        pathControl.playOnStart = false;
    }

    void Awake()
    {
        waterParticles = GameObject.FindGameObjectsWithTag("WaterParticle");
        //there are 3 particles now, need to make this dynamic later
        //order particles are currently added to array: greastest to least = 1st item is the last item in the array
        waterParticles[0].SetActive(false);
        waterParticles[1].SetActive(false);

        //subscribe to Scene1Event event call
        EventManager.StartListening("Scene1Event", Scene1Events);
        s1Events = new List<string>{"waterBubbles",""};
        pathControl.AnimationPausedEvent += delegate { DebugFunc("Paused"); };
    }

    

    private void DebugFunc(string evt)
    {
        if (evt == "Paused")
            Debug.Log("[Camera Path]: Paused");
    }
	
	// Update is called once per frame
	void Update () {

        //if "P" pressed, pause spline
        if (Input.GetKeyDown(KeyCode.P))
        {
            /*if (splineControl.sSplineState == SplineState.Paused) //unpause
            {
                splineControl.sSplineState = SplineState.Resume;
                playerControl.playerState = PlayerState.MOVING;
            }
            else  //pause
            {
                splineControl.sSplineState = SplineState.Paused;
                playerControl.playerState = PlayerState.NOTMOVING;
            }*/
        }
        //if "O" pressed, continue spline
        if (Input.GetKeyDown(KeyCode.O))
        {
            //splineControl.sSplineState = SplineState.Resume;
            playerControl.playerState = PlayerState.MOVING;
        }
    }

    //function that contains what each event in song 1 should do
    void Scene1Events(string evt)
    { 
            switch (evt)
            {
                case "waterParticle":
                    Debug.Log("water particle activated");
                    waterParticles[1].SetActive(true);
                    break;
                case "Scene2":
                    StartCoroutine(Scene1_5());
                    break;
                default:
                    break;
            }
                
    }

    IEnumerator Scene1_5()
    {
        //pause the spline
        playerControl.CanMove = false;
        pathControl.Pause();

        //pause for x seconds and let player move around
        yield return new WaitForSeconds(3);

        //resume the spline
        playerControl.CanMove = true;
        pathControl.Play();

        yield return null;
    }

    IEnumerator Level1Start()
    {
        //set timer to show Title theme and begin spline
        float time = 0;
        float screenFadeOutTime = 1f;
        float headRotTime = 5f;
        Color baseColor = blackOverlay.color;

        //open eyes
        soundManager.PlayMusic(0);

        //fade sign out every second
        while (time < screenFadeOutTime)
        {
            Debug.Log("fading");
            blackOverlay.color = Color.Lerp(baseColor, new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 0f), time / screenFadeOutTime);
            time += Time.deltaTime;
            yield return null;
        }

        float t = 0;
        
        //rotate player to face ocean
        Vector3 playerRot = player.transform.rotation.eulerAngles; //rotation in euler angles
        Quaternion baseRot = player.transform.rotation;
        Quaternion nextRot = Quaternion.Euler(0, playerRot.y, playerRot.z);
        Debug.Log("rotate head");
        /*while (t < headRotTime)
        {
            player.transform.rotation = Quaternion.Lerp(baseRot, nextRot, (t / headRotTime));
            t += Time.deltaTime;
            yield return null;
        }*/

        //have fish swim up to player and begin to lead player

        yield return new WaitForSeconds(1);
        //begin spline
        pathControl.Play();
        playerControl.CanMove = true;

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
