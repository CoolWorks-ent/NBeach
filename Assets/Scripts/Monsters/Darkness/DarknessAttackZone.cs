﻿using System.Collections;
using UnityEngine;


namespace DarknessMinion
{
	[ExecuteInEditMode]
	public class DarknessAttackZone : MonoBehaviour
	{
		private Vector3 attackZoneOrigin;

		[SerializeField, Range(0, 5)]
		private float attackZoneRadius;
		[SerializeField, Range(0, 4)]
		private float attackZoneOffset;

		[SerializeField]
		private Transform playerRotationFacing, playerLocation;

		public static DarknessAttackZone Instance { get; private set; }


		//Have a zone offset towards the front of the player.
		//The center of the zone is x units in front of the player container's forward vector.
		//This should rotate the attack zone based on what should be their ideal position

		void Awake()
        {
			if (Instance != null && !Instance.gameObject.CompareTag("AI Manager"))
			{
				Debug.LogError("Instance of DarkAttackZone already exist in this scene");
				//Destroy(instance.gameObject.GetComponent<AI_Manager>());
			}
			Instance = this;
        }

		void Start()
		{
			attackZoneOrigin = playerLocation.position + playerRotationFacing.forward * attackZoneOffset;
		}

		void LateUpdate()
		{
			attackZoneOrigin = playerLocation.position + playerRotationFacing.forward * attackZoneOffset;
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



		#if UNITY_EDITOR
		void OnDrawGizmosSelected()
		{
			UnityEditor.Handles.color = Color.green;
			UnityEditor.Handles.DrawWireDisc(attackZoneOrigin, Vector3.up, attackZoneRadius);
		}
		#endif
	}
}