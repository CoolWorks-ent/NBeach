using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public int levelNum;
    [SerializeField]
    public CameraPathAnimator pathControl;
    
	// Use this for initialization
	void Start () {
		
	}

    public Level()
    {
    
    }

    //Trigger event that level is finished
    public void OnLevelFinished(string levelNum)
    {
        EventManager.TriggerEvent("LevelFinished",levelNum);

    }



	// Update is called once per frame
	void Update () {
		
	}
}
