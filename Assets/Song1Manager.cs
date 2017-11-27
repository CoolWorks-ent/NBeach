using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Song1Manager : MonoBehaviour {

	[SerializeField]
	PlayerController playerControl;
	[SerializeField]
	SoundManager soundManager;
	[SerializeField]
	GameObject holyLight;

	Image blackOverlay;
	GameObject wave;
	GameObject[] waterParticles;
	GameObject darkRoom;
	float scene1Time = 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
