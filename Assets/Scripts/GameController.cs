using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    [SerializeField]
    SplineController splineControl;
    [SerializeField]
    PlayerController playerControl;

    GameObject player;
    SpriteRenderer blackOverlay;
    // Use this for initialization
    void Start () {
        //set default state to "paused"
        //splineControl.sSplineState = SplineState.Paused;
        blackOverlay = GameObject.Find("BlackOverlay").GetComponent<SpriteRenderer>();
        blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 1);
        StartCoroutine(Level1Start());
        player = GameObject.Find("Player Container").gameObject;
    }

    void Awake()
    {

    }
	
	// Update is called once per frame
	void Update () {

        //if "P" pressed, pause spline
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (splineControl.sSplineState == SplineState.Paused) //unpause
            {
                splineControl.sSplineState = SplineState.Resume;
                playerControl.playerState = PlayerState.MOVING;
            }
            else  //pause
            {
                splineControl.sSplineState = SplineState.Paused;
                playerControl.playerState = PlayerState.NOTMOVING;
            }
        }
        //if "O" pressed, continue spline
        if (Input.GetKeyDown(KeyCode.O))
        {
            splineControl.sSplineState = SplineState.Resume;
            playerControl.playerState = PlayerState.MOVING;
        }
    }

    IEnumerator Level1Start()
    {
        //set timer to show Title theme and begin spline
        float time = 0;
        float screenFadeOutTime = 1f;
        float headRotTime = 10f;
        Color baseColor = blackOverlay.color;
        
        //open eyes
        
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
        while (t < headRotTime)
        {
            player.transform.rotation = Quaternion.Lerp(baseRot, nextRot, (t / headRotTime));
            t += Time.deltaTime;
            yield return null;
        }

        //have fish swim up to player and begin to lead player

        yield return new WaitForSeconds(1);
        //begin spline
        splineControl.sSplineState = SplineState.Resume;
        playerControl.playerState = PlayerState.MOVING;
        Debug.Log("spline started");

        yield return null;
    }
}
