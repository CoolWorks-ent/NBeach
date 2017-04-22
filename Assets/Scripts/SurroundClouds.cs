using UnityEngine;
using System.Collections;

public class SurroundClouds : MonoBehaviour {
	
	private GameObject theGod;
	
	// Use this for initialization
	void Start () {
		
		// In a days when I found god, yeahh yeahh 
		theGod = GameObject.Find ( "God" );
	
	}
	
	// Update is called once per frame
	void Update () {
	
		// Change the position of a clouds accordinly
		transform.position = new Vector3 ( theGod.transform.position.x,transform.position.y,theGod.transform.position.z);
		
	}
}
