using System.Collections;
using UnityEditor;
using UnityEngine;

namespace DarknessMinion
{
	public class DarknessAttackZone : MonoBehaviour
	{
		private Vector3 attackZoneCircle;

		[SerializeField, Range(0, 5)]
		private float attackZoneRadius;
		[SerializeField, Range(0, 4)]
		private float attackZoneOffset;

		[SerializeField]
		private Transform playerRotationFacing, playerLocation;



		//Have a zone offset towards the front of the player.
		//The center of the zone is x units in front of the player container's forward vector.
		//This should rotate the attack zone based on what should be their ideal position

		void Start()
		{
			attackZoneCircle = playerLocation.position + playerRotationFacing.forward * attackZoneOffset;
		}

		void LateUpdate()
		{
			attackZoneCircle = playerLocation.position + playerRotationFacing.forward * attackZoneOffset;
		}

		public bool InTheZone(Vector2 location)
        {
			if (Vector2.Distance(new Vector2(attackZoneCircle.x, attackZoneCircle.z), location) < attackZoneRadius)
				return true;
			return false;
        }

		#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			Handles.color = Color.green;
			Handles.DrawWireDisc(attackZoneCircle, Vector3.up, attackZoneRadius);
		}
		#endif
	}
}