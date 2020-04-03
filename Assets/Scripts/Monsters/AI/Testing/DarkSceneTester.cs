using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSceneTester : MonoBehaviour {

	public Transform darkPrefab;

	public bool spawnDarkness;
	public int spawnRate;

	void Start () {
		spawnDarkness = false;
	}

	public void ToggleSpawner()
	{
		spawnDarkness = !spawnDarkness;
		if(!spawnDarkness)
			StopCoroutine(DarknessSpawnTimer(spawnRate));
		else StartCoroutine(DarknessSpawnTimer(spawnRate));
	}

	private IEnumerator DarknessSpawnTimer(int rate)
	{
		Instantiate(darkPrefab, this.transform.position, Quaternion.identity, this.transform);
		yield return new WaitForSeconds(rate);
	}
}
