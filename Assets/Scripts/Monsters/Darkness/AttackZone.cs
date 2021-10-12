using System.Collections.Generic;
using UnityEngine;

namespace DarknessMinion
{
	[System.Serializable]
	public class AttackZone
	{
		public Vector3 attackZoneOrigin;

		[SerializeField, Range(0, 25)]
		public float attackZoneRadius;

		[HideInInspector]
		public int occupierID;
		[HideInInspector]
		public bool occupied;
		
		[SerializeField, Range(-25, 25)]
		private float attackZoneOffsetForward, attackZoneOffsetRight;
		[SerializeField, Range(-5, 10)]
		private float attackPointOffsetForward;
		private Transform playerLocation;

		//TODO bool occupied, take in RemovedDarkness, with that data I can against ID to unassign
		//TODO test to make sure when the player is far away from the zone the darkness 

		public void SetPlayerLocationOrigin(Transform player)
		{
			playerLocation = player;
			attackZoneOrigin = playerLocation.position + playerLocation.forward * attackZoneOffsetForward + playerLocation.right * attackZoneOffsetRight;
		}

		public void ZoneUpdate()
		{
			attackZoneOrigin = playerLocation.position + playerLocation.forward * attackZoneOffsetForward + playerLocation.right * attackZoneOffsetRight;
			DarkEventManager.OnUpdateZoneLocation(attackZoneOrigin);
		}

		public Vector3 AttackPoint()
		{
			return playerLocation.position + playerLocation.forward * attackPointOffsetForward;
		}

		public NavigationTarget RequestPointInsideZone(float height)
        {
			return new NavigationTarget((Random.insideUnitSphere * attackZoneRadius), attackZoneOrigin, height);
		}

		public void OccupyZone(int iD)
		{
			occupied = true;
			occupierID = iD;
		}

		public void RemoveOccupier()
		{
			occupierID = 2000;
			occupied = false;
		}

		public bool InTheZone(Vector2 location)
		{
			Vector2 v = new Vector2(attackZoneOrigin.x, attackZoneOrigin.z);
			if (Vector2.Distance(v, location) < attackZoneRadius)
				return true;
			return false;
		}
	}
}
