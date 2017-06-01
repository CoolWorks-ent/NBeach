using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    [SerializeField]
    SplineController splineControl;
    [SerializeField]
    PlayerController playerControl;

    SpriteRenderer blackOverlay;
    float time = 0f;
    // Use this for initialization
    void Start () {
        //set default state to "paused"
        splineControl.sSplineState = "Paused";
        blackOverlay = GameObject.Find("BlackOverlay").GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {

        //if "P" pressed, pause spline
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (splineControl.sSplineState == "Paused")
            {
                splineControl.sSplineState = "Start";
                playerControl.playerState = PlayerState.MOVING;
            }
            else
            {
                splineControl.sSplineState = "Paused";
                playerControl.playerState = PlayerState.NOTMOVING;
            }
        }
        //if "O" pressed, continue spline
        if (Input.GetKeyDown(KeyCode.O))
        {
            splineControl.sSplineState = "Resume";
            playerControl.playerState = PlayerState.MOVING;
        }
    }

    IEnumerator Level1Start()
    {
        //set timer to show Title theme and begin spline
        float screenFadeOutTime = 3f;
        //open eyes
        blackOverlay.color = Color.Lerp(blackOverlay.color, new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 0f), time);
            //fade sign every second
        if (time < 1)
        {
            time += Time.deltaTime / screenFadeOutTime;
        }

        //rotate player to face ocean
        yield return null;
    }
}
