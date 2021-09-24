using System.Collections.Generic;
using UnityEngine;

namespace DarknessMinion
{
	[System.Serializable]
	public class AttackZone
	{
		public Vector3 attackZoneOrigin;

		[SerializeField, Range(0, 5)]
		public float attackZoneRadius;

		[HideInInspector]
		public int occupierID;
		[HideInInspector]
		public bool occupied;
		
		[SerializeField, Range(-5, 5)]
		private float attackZoneOffsetForward, attackZoneOffsetRight;
		private Transform playerLocation;



		//TODO bool occupied, take in RemovedDarkness, with that data I can against ID to unassign
		//TODO test to make sure when the player is far away from the zone the darkness 

		//Have a zone offset towards the front of the player.
		//The center of the zone is x units in front of the player container's forward vector.
		//This should rotate the attack zone based on what should be their ideal position

		public AttackZone()
		{
			occupierID = -1;
		}

		public void SetParameters(Vector3 origin, Transform player)
		{
			playerLocation = player;
			attackZoneOrigin = playerLocation.position + playerLocation.forward * attackZoneOffsetForward + playerLocation.right * attackZoneOffsetRight;
		}

		public void ZoneUpdate()
		{
			attackZoneOrigin = playerLocation.position + playerLocation.forward * attackZoneOffsetForward + playerLocation.right * attackZoneOffsetRight;
			DarkEventManager.OnUpdateZoneLocation(attackZoneOrigin);
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

		public void DeAllocateZone()
		{
			occupierID = 2000;
			occupied = false;
		}

		public bool InTheZone(Vector2 location)
		{
			Vector2 v = new Vector2(attackZoneOrigin.x, attackZoneOrigin.y);
			if (Vector2.Distance(v, location) < attackZoneRadius)
				return true;
			return false;
		}
	}
}