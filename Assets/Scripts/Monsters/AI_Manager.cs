using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour {

	public Transform player;
	private List<Darkness> ActiveDarkness;

	void Start () {
		ActiveDarkness = new List<Darkness>();
		//Pass darkness object into list using an event 
	}
	
	void Update () {
		
	}
}
