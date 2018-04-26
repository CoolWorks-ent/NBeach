using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour {

	public Transform player;
	public List<Darkness> ActiveDarkness;

	public int maxEnemyCount;
	private static AI_Manager instance;
	public static AI_Manager Instance
	{
		get {return instance; }
	}

	void Awake()
	{
		maxEnemyCount = 10;
		if(instance != null && instance != this)
		{
			//Debug.LogError("Instance of AI Manager already exist in this scene");
		}
		else instance = this;

		ActiveDarkness = new List<Darkness>();

		player = GameObject.FindGameObjectWithTag("PlayerCube").transform;
	}

	public void AddtoDarknessList(Darkness updatedDarkness)
	{
		ActiveDarkness.Add(updatedDarkness);
		updatedDarkness.target = player;
	}

	public IEnumerator WaitTimer(float timer)
	{
		yield return new WaitForSeconds(timer);
		Debug.LogWarning("Time is up for idle");
	}
}
