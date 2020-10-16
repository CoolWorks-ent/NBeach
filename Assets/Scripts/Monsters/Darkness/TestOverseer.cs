using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarknessMinion;
public class TestOverseer : MonoBehaviour 
{
	public Darkness darknessPrefab;

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
		if(DarknessManager.Instance.ActiveDarkness.TryGetValue(DarknessManager.Instance.ActiveDarkness.Count, out d))
		{
			d.ChangeState(d.deathState);
		}
	}
}
