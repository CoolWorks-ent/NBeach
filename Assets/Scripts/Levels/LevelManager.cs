using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*********************
 * Base Level Script for any level in the game
 *********************/


public class LevelManager : MonoBehaviour {
    [SerializeField]
    public Level[] levelList;

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
}
