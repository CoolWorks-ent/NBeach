using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    void Start()
    {
        //1
        Application.targetFrameRate = 70;
    }
    //2
    public void GoToGame()
    {
        //3
        SceneManager.LoadScene("Song2");
    }

    public void LevelSelect()
    {

    }
}
