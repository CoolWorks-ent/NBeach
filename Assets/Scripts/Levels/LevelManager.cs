using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*********************
 * Base Level Script for any level in the game
 *********************/


public class LevelManager : MonoBehaviour {
    [SerializeField]
    public Level[] levelList;
    public Level currentLvl;

    // Use this for initialization
    void Awake ()
    {

	}

    public LevelManager()
    {

    }

    
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnLevelWasLoaded(int level)
    {
        if(level == 2)
        {
            currentLvl = GameObject.FindGameObjectWithTag("song2_obj").GetComponent<song2_lvl>();
        }   
    }
}
