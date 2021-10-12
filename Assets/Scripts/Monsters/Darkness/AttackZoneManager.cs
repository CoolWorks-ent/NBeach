using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DarknessMinion
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

		[SerializeField]
		private LayerMask avoidanceMask;
		
		public AttackZone playerAttackZone { get { return attackZone; } }
		
		public static AttackZoneManager Instance { get; private set; }
		
		void Awake()
		{
			Instance = this;

			/*if(attackZones.Length <= 0)
				attackZones = new AttackZone[4];
			foreach(AttackZone dTZ in attackZones)
			{
				if(!playerTransform)
				{
					dTZ.SetPlayerLocationOrigin(playerTransform);
				}
				else dTZ.SetPlayerLocationOrigin(playerTransform);
			}*/
			
			attackZone.SetPlayerLocationOrigin(playerTransform);
		}

		public void LateUpdate()
		{
			/*for(int i = 0; i < attackZones.Length; i++)
			{
				attackZones[i].ZoneUpdate();
			}*/
			
		}
		
		/*public AttackZone RequestZoneTarget(int iD)
		{
			//assign a random unoccupied zone 
			List<AttackZone> zones = UnOccupiedZones();

			if (zones.Count == 0) 
				return null;
			if(zones.Count != 1)
			{
				int t = Random.Range(0, zones.Count);
				zones[t].OccupyZone(iD);
				return zones[t];
			}
				
			zones[0].OccupyZone(iD);
			return zones[0];
		}

		public List<AttackZone> UnOccupiedZones()
		{
			List<AttackZone> zones = new List<AttackZone>();

			foreach (AttackZone atkZone in attackZones)
			{
				if(!atkZone.occupied)
					zones.Add(atkZone);
			}

			return zones;
		}

		public void DeAllocateZone(int iD)
		{
			for(int i = attackZones.Length-1; i > 0; i--)
			{
				if(attackZones[i].occupierID == iD)
					attackZones[i].RemoveOccupier();
			}
		}
		

		public bool ZoneBlocked(int id)
		{
			RaycastHit hit;
			if(Physics.SphereCast(attackZones[id].attackZoneOrigin, attackZones[id].attackZoneRadius * 0.8f, Vector3.zero, out hit, avoidanceMask, 0, QueryTriggerInteraction.Collide))
				return true;
			return false;
		}*/

	#if UNITY_EDITOR
		void OnDrawGizmosSelected()
		{	
			attackZone.SetPlayerLocationOrigin(playerTransform);
			UnityEditor.Handles.DrawWireDisc(attackZone.attackZoneOrigin, Vector3.up, attackZone.zoneRadius);
			UnityEditor.Handles.color = Color.yellow;
			UnityEditor.Handles.DrawWireDisc(attackZone.AttackPoint(), Vector3.up, 0.5f);
			/*if(attackZones.Length > 0)
			{
				foreach(AttackZone dTZ in attackZones)
				{
					if(dTZ == null)
						dTZ.SetPlayerLocationOrigin(playerTransform);
					dTZ.ZoneUpdate();
					//TODO change color for occupied zones
					if(dTZ.occupied)
						UnityEditor.Handles.color = Color.red;
					else UnityEditor.Handles.color = Color.green;
					UnityEditor.Handles.DrawWireDisc(dTZ.attackZoneOrigin, Vector3.up, dTZ.attackZoneRadius);
				}
			}*/
		}
		#endif
	}
}