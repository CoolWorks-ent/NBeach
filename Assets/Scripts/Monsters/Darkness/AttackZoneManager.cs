using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  DarknessMinion
{
	[ExecuteInEditMode]
	public class AttackZoneManager : MonoBehaviour 
	{
		//Collection of AttackZones. Updates zone locations. Provides a valid zone target

		[SerializeField]
		private AttackZone[] darknessAttackZones;
		
		[SerializeField]
		private Transform playerTransform;
		
		void Awake()
		{
			if(darknessAttackZones.Length <= 0)
				darknessAttackZones = new AttackZone[4];
			foreach(AttackZone dTZ in darknessAttackZones)
			{
				if(!playerTransform)
				{
					dTZ.SetParameters(Vector3.zero, DarknessManager.Instance.playerTransform);
				}
				else dTZ.SetParameters(Vector3.zero, playerTransform);
			}
		}

		public void LateUpdate()
		{
			foreach(AttackZone dTZ in darknessAttackZones)
			{
				dTZ.ZoneUpdate();
			}
			//attackZoneOrigin = playerLocation.position + playerRotationFacing.forward * attackZoneOffset;
			//DarkEventManager.OnUpdateZoneLocation(attackZoneOrigin);
		}

		#if UNITY_EDITOR
		void OnDrawGizmosSelected()
		{	
			if(darknessAttackZones.Length > 0)
			{
				UnityEditor.Handles.color = Color.green;

				foreach(AttackZone dTZ in darknessAttackZones)
				{
					dTZ.ZoneUpdate();
					UnityEditor.Handles.DrawWireDisc(dTZ.attackZoneOrigin, Vector3.up, dTZ.attackZoneRadius);
				}
			}
		}
		#endif
	}
}