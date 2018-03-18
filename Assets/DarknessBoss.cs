using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessBoss : MonoBehaviour {

	// Use this for initialization
	void Start () {
        EventManager.StartListening("Stage1Start",Stage1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void DarkBallAttack()
    {

    }

    void Stage1(string str)
    {

    }
}
