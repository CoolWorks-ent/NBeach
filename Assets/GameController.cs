using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    [SerializeField]
    SplineController splineControl;
    [SerializeField]
    PlayerController playerControl;
    // Use this for initialization
    void Start () {
        //set default state to "paused"
        splineControl.sSplineState = "Paused";
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
        //set timer to show Title them begin spline
        yield return null;
    }
}
