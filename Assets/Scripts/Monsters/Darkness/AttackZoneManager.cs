using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Darkness
{
	[ExecuteInEditMode]
	public class AttackZoneManager : MonoBehaviour 
	{
		//Collection of AttackZones. Updates zone locations. Provides a valid zone target

		//[SerializeField]
		//private AttackZone[] attackZones;

		[SerializeField]
		private AttackZone attackZone;

		[SerializeField]
		private Transform playerTransform;

		public AttackZone playerAttackZone { get { return attackZone; } }
		
		public static AttackZoneManager Instance { get; private set; }
		
		void Awake()
		{
			Instance = this;
			attackZone.SetPlayerLocationOrigin(playerTransform);
		}

		private void LateUpdate()
		{
			if(attackZone != null)
				attackZone.ZoneUpdate();
		}

#if UNITY_EDITOR
		void OnDrawGizmosSelected()
		{	
			attackZone.SetPlayerLocationOrigin(playerTransform);
			UnityEditor.Handles.DrawWireDisc(attackZone.attackZoneOrigin, Vector3.up, attackZone.zoneRadius);
			UnityEditor.Handles.color = Color.yellow;
			UnityEditor.Handles.DrawWireDisc(attackZone.AttackPoint(), Vector3.up, 0.5f);
		}
		#endif
	}
}