using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessMinion
{
	[ExecuteInEditMode]
	public class AttackZoneManager : MonoBehaviour 
	{
		//Collection of AttackZones. Updates zone locations. Provides a valid zone target

		[SerializeField]
		private AttackZone[] attackZones;
		
		[SerializeField]
		private Transform playerTransform;

		[SerializeField]
		private LayerMask avoidanceMask;
		
		public static AttackZoneManager Instance { get; private set; }
		
		void Awake()
		{
			Instance = this;

			if(attackZones.Length <= 0)
				attackZones = new AttackZone[4];
			foreach(AttackZone dTZ in attackZones)
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
			for(int i = 0; i < attackZones.Length; i++)
			{
				attackZones[i].ZoneUpdate();
			}

			//check if the zone is mostly blocked. if so assign the next best zone
		}

		public AttackZone RequestZoneTarget(int iD)
		{
			//assign a random unoccupied zone 
			List<AttackZone> zones = UnOccupiedZones();

			if(zones.Count != 0)
			{
				if(zones.Count != 1)
				{
					int t = Random.Range(0, zones.Count);
					zones[t].OccupyZone(iD);
					return zones[t];
				}
				
				zones[0].OccupyZone(iD);
				return zones[0];
			}

			return null;
		}

		public List<AttackZone> UnOccupiedZones()
		{
			List<AttackZone> zones = new List<AttackZone>();

			for(int i = 0; i < attackZones.Length; i++)
			{
				if(attackZones[i].occupied)
					zones.Add(attackZones[i]);
			}

			return zones;
		}

		public void DeAllocateZone(int iD)
		{
			for(int i = attackZones.Length-1; i > 0; i--)
			{
				if(attackZones[i].occupierID == iD)
					attackZones[i].DeAllocateZone();
			}
		}
		

		public bool ZoneBlocked(int id)
		{
			RaycastHit hit;
			if(Physics.SphereCast(attackZones[id].attackZoneOrigin, attackZones[id].attackZoneRadius * 0.8f, Vector3.zero, out hit, avoidanceMask, 0, QueryTriggerInteraction.Collide))
				return true;
			return false;
		}

	#if UNITY_EDITOR
		void OnDrawGizmosSelected()
		{	
			if(attackZones.Length > 0)
			{
				UnityEditor.Handles.color = Color.green;

				foreach(AttackZone dTZ in attackZones)
				{
					if(dTZ == null)
						dTZ.SetParameters(Vector3.zero, DarknessManager.Instance.playerTransform);
					dTZ.ZoneUpdate();
					UnityEditor.Handles.DrawWireDisc(dTZ.attackZoneOrigin, Vector3.up, dTZ.attackZoneRadius);
				}
			}
		}
		#endif
	}
}