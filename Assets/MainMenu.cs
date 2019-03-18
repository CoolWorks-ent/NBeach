using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject LevelSelectMenuObj;

    [SerializeField]
    GameObject MainMenuObj;

    void Start()
    {
        //1
        Application.targetFrameRate = 70;
        //hide other menus

    }
    //2
    //Function is Called when "Start Button" is pressed
    public void GoToGame()
    {
        //3
        ///SceneManager.LoadScene("Song2");
        EventManager.TriggerEvent("MenuSelect_StartGame", "startGame");
    }

    public void LevelSelect()
    {

    }

    public void LevelSelect_LoadOnClick(int level)
    {
        //loadingImage.SetActive(true);
        EventManager.TriggerEvent("MenuSelect_LevelSelectGame", "levelSelectGame");
        SceneManager.LoadScene(level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
