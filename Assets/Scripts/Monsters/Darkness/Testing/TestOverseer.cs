using System.Linq;
using UnityEngine;

namespace DarknessMinion
{
	public class TestOverseer : MonoBehaviour 
	{
		public Darkness darknessPrefab;
		public static TestOverseer Instance { get; private set; }

		[Header("Show Info")]
		public bool showDebugInfo;

		[Header("Debug Info to Show")]
		public bool targetInfo;
		public bool agroRating;
		public bool stateInfo;
		public bool locationInfo;
		public bool cooldownInfo;

		void Awake()
		{
			Instance = this;
			//showDebugInfo = false;
		}

		public void SpawnDarkness()
		{
			
			if (DarknessManager.Instance.ActiveDarkness.Count < DarknessManager.Instance.maxEnemyCount)
			{
				//Vector3 sLoc = Random.insideUnitCircle * 5;
				Darkness enemy = Instantiate(darknessPrefab, new Vector3(Random.Range(-15, 15), DarknessManager.Instance.groundLevel + 0.5f, Random.Range(0, 10)), darknessPrefab.transform.rotation);
			}
		}

		public void KillDarkness()
		{
			if(DarknessManager.Instance.ActiveDarkness.Count > 0)
			{
				Darkness d;
				d = DarknessManager.Instance.ActiveDarkness.Values.LastOrDefault();
				if(d != null)
					d.KillDarkness();
			}
		}

		void LateUpdate()
		{
			foreach(Darkness dark in DarknessManager.Instance.ActiveDarkness.Values)
			{
				dark.UpdateDebugMessage(targetInfo, agroRating, stateInfo, locationInfo, cooldownInfo);
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
}