using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    //UI Objects used during game
    [SerializeField]
    GUIArrows_NB guiArrows;
	// Use this for initialization
	void Start () {
        guiArrows.Load();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /*
     * function that will re-scale UI based upon mobile vs. PC
     * 
     */
     public void RescaleUI()
    {

    }

    //when notified to show Arrow, show and blink
    public void showArrow(int side, GameObject obj)
    {
        guiArrows.Show(side, obj);
        //side = 0 = left, side = 1 = right
    }

    public void hideArrow(int side)
    {
        guiArrows.Hide(side);
    }
}
