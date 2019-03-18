using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script used to load a scene while another scene is loaded (Unity Pro Only)
/// </summary>
/// 
public class ClickToLoadAsync : MonoBehaviour {

    //public Slider loadingBar;
    public GameObject loadingImage;


    private AsyncOperation async;


    public void ClickAsync(int level)
    {
        loadingImage.SetActive(true);
        StartCoroutine(LoadLevelWithBar(level));
    }


    IEnumerator LoadLevelWithBar(int level)
    {
        async = SceneManager.LoadSceneAsync(level);
        while (!async.isDone)
        {
            //loadingBar.value = async.progress;
            yield return null;
        }
    }
}
