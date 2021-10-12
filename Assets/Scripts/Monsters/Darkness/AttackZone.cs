using System.Collections.Generic;
using UnityEngine;

namespace DarknessMinion
{
	[System.Serializable]
	public class AttackZone
	{
		public Vector3 attackZoneOrigin;

		[SerializeField, Range(0, 25)]
		public float zoneRadius;

		[HideInInspector]
		public int occupierID;
		[HideInInspector]
		public bool occupied;
		
		[SerializeField, Range(-25, 25)]
		private float zoneOffsetForward, zoneOffsetRight;
		[SerializeField, Range(-5, 10)]
		private float attackPointOffsetForward;
		private Transform playerLocation;

		//TODO bool occupied, take in RemovedDarkness, with that data I can against ID to unassign
		//TODO test to make sure when the player is far away from the zone the darkness 

		public void SetPlayerLocationOrigin(Transform player)
		{
			playerLocation = player;
			attackZoneOrigin = playerLocation.position + playerLocation.forward * zoneOffsetForward + playerLocation.right * zoneOffsetRight;
		}

		public void ZoneUpdate()
		{
			attackZoneOrigin = playerLocation.position + playerLocation.forward * zoneOffsetForward + playerLocation.right * zoneOffsetRight;
			DarkEventManager.OnUpdateZoneLocation(attackZoneOrigin);
		}

		public Vector3 AttackPoint()
		{
			return playerLocation.position + playerLocation.forward * attackPointOffsetForward;
		}

		public NavigationTarget RequestPointInsideZone(float height)
        {
			return new NavigationTarget((Random.insideUnitSphere * zoneRadius), attackZoneOrigin, height);
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
			if (Vector2.Distance(v, location) < zoneRadius)
				return true;
			return false;
		}
	}
}
