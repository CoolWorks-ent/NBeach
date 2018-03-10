using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public LevelManager()
    {

    }

    
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log("Mode: " + mode);
        switch(scene.name)
        {
            case "Song1_V2":
                currentLvl = GameObject.FindGameObjectWithTag("song1_obj").GetComponent<song1_lvl>();
                break;
            case "Song2":
                currentLvl = GameObject.FindGameObjectWithTag("song2_obj").GetComponent<song2_lvl>();
                break;
            default:
                break;
        }
    }
}
