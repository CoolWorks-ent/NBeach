using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// InGameMenu is shown after a level is finished. and only if the player had chosen "Level Select"
/// </summary>

public class InGameMenu : MonoBehaviour {

    [SerializeField]
    GameObject gameMenuPanel;

    Vector3 baseScale;
    Vector3 basePosition;

    private void Awake()
    {

        baseScale = this.gameObject.transform.localScale;
        basePosition = this.gameObject.transform.localPosition;
    }
    // Use this for initialization
    void Start () {

        EventManager.StartListening("LevelFinished", OnLevelFinished);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //set camera again
    public void ResetMenuPos(string pos)
    {
        //this.GetComponent<Canvas>().worldCamera = GameController.instance.playerControl.mainCamera;
        this.gameObject.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        //set the menu to be a parent of the current playerController object to center it on screen and make the position update properly
        this.gameObject.transform.SetParent(GameController.instance.playerControl.playerContainer.transform);
        this.gameObject.transform.localScale = baseScale;
        this.gameObject.transform.localPosition = basePosition;
    }

    void OnLevelFinished(string levelNum)
    {
        // "Level Select" Start Only: Only set the menu active after the level is finished and the player started the game via "LevelSelect"
        if(GameController.instance.gameLevelSelectionState == GameLevelSelectionState.LevelSelected)
            gameMenuPanel.SetActive(true);
    }

    public void LoadOnClick(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void LoadNextLevel()
    {
        //show any extra HUD stuff
        EventManager.TriggerEvent("MenuSelect_LoadNextLevel","");
        //GameController.instance.lvlManager.LoadNextLevel();
    }

    public void ResumeGame()
    {
        EventManager.TriggerEvent("MenuSelect_ResumeGame", "");
    }

    //Tell game to load main menu and quit level
    public void ReturnToMainMenu()
    {
        EventManager.TriggerEvent("MenuSelect_ReturnToTitle","");
        SceneManager.LoadScene(0);
    }

}
