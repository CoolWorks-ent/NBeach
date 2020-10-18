using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DarknessMinion;
public class TestOverseer : MonoBehaviour 
{
	public Darkness darknessPrefab;
	public static TestOverseer Instance { get; private set; }

	public bool showDebugInfo;

	void Awake()
	{
		Instance = this;
		showDebugInfo = false;
	}

	public void SpawnDarkness()
	{
		if (DarknessManager.Instance.ActiveDarkness.Count < DarknessManager.Instance.maxEnemyCount)
		{
			Vector3 sLoc = Random.insideUnitCircle * 5;
			Darkness enemy = Instantiate(darknessPrefab, new Vector3(sLoc.x, DarknessManager.Instance.oceanPlane.position.y+0.5f, sLoc.z), darknessPrefab.transform.rotation);
		}
	}

	public void KillDarkness()
	{
		Darkness d;
		d = DarknessManager.Instance.ActiveDarkness.Values.LastOrDefault();
		if(d != null)
			d.ChangeState(d.deathState);
	}

	void LateUpdate()
	{
		foreach(Darkness dark in DarknessManager.Instance.ActiveDarkness.Values)
		{
			dark.UpdateDebugMessage();
		}
	}

	public void DisplayDebugInfo()
	{
		foreach(Darkness dark in DarknessManager.Instance.ActiveDarkness.Values)
		{
			dark.ToggleDebugMessage(true);
		}
	}

	public void DeactivateDebugInfo()
	{
		foreach(Darkness dark in DarknessManager.Instance.ActiveDarkness.Values)
		{
			dark.ToggleDebugMessage(false);
		}
	}

	public void MovePlayerObject(Vector3 direction)
	{
		Transform player = DarknessManager.Instance.player;

		player.position += direction;
	}
}
