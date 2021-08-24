using System.Collections;
using UnityEngine;


namespace DarknessMinion
{
	[System.Serializable]
	public class AttackZone
	{
		public Vector3 attackZoneOrigin;

		[SerializeField, Range(0, 5)]
		public float attackZoneRadius;
		[SerializeField, Range(-5, 5)]
		public float attackZoneOffsetForward, attackZoneOffsetRight;
		private Transform playerLocation;


		//Have a zone offset towards the front of the player.
		//The center of the zone is x units in front of the player container's forward vector.
		//This should rotate the attack zone based on what should be their ideal position

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

		public bool InTheZone(Vector2 location)
        {
			if (Vector2.Distance(new Vector2(attackZoneOrigin.x, attackZoneOrigin.z), location) < attackZoneRadius)
				return true;
			return false;
        }

		public NavigationTarget RequestPointInsideZone(float height)
        {
			return new NavigationTarget((Random.insideUnitSphere * attackZoneRadius), attackZoneOrigin, height);
		}
	}
}