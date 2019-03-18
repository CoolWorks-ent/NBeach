using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*********************
 * Base Level Script for any level in the game
 *********************/


public class LevelManager : MonoBehaviour {
    [SerializeField]
    public Level[] levelList; //container for all levels
    public Level currentLvl;

    // Use this for initialization
    void Awake ()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
        EventManager.StartListening("MenuSelect_LoadNextLevel", LoadNextLevel);
    }

    //FUTURE FUNCTIONALITY, keep track of player's last played level.  Level manager will decide to load the next level after the player has finished a level. 
    //..on startGame select
    public LevelManager()
    {

    }

    
	
	// Update is called once per frame
	void Update () {

	}

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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

    //Load next level
    public void LoadNextLevel(string evt)
    {
        //if there is another scene, then load it
        if (SceneManager.GetSceneAt(currentLvl.levelNum + 1) != null)
        {
            currentLvl.levelNum = currentLvl.levelNum + 1;
            SceneManager.LoadScene(currentLvl.levelNum + 1);
        }
        else
        {
            print("end of game, returning to main menu");
            SceneManager.LoadScene(0);
        }
    }

    public void LoadLevel(int levelNum)
    {
        if(levelNum == 1)
            SceneManager.LoadScene(2);
        else
            currentLvl.levelNum = levelNum;
            SceneManager.LoadScene(levelNum);
        
    }
}
