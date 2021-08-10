using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  DarknessMinion
{
	[ExecuteInEditMode]
	public class DarknessAttackZoneManager : MonoBehaviour 
	{
		[SerializeField]
		private Transform player;

		[SerializeField]
		private DarknessAttackZone[] darknessAttackZones;

		void Awake()
		{
			if(darknessAttackZones.Length <= 0)
				darknessAttackZones = new DarknessAttackZone[4];
			
			foreach(DarknessAttackZone dTZ in darknessAttackZones)
			{
				dTZ.SetParameters(Vector3.zero, player);
			}
			
		}
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public void LateUpdate()
		{
			foreach(DarknessAttackZone dTZ in darknessAttackZones)
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
				foreach(DarknessAttackZone dTZ in darknessAttackZones)
				{
					dTZ.ZoneUpdate();
					UnityEditor.Handles.DrawWireDisc(dTZ.attackZoneOrigin, Vector3.up, dTZ.attackZoneRadius);
				}
			}
		}
		#endif
	}
}