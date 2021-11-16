﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Darkness
{
	public class DarknessManager : MonoBehaviour
	{
		/* 
		*	Contains references to all Darkness. Approves them for attacking based on distance. Provides attack target when attacking
		*/
		public Transform playerTransform { get { return player; } }
		public Vector3 playerVector { get { return player.position; } }
		public Dictionary<int, DarknessController> ActiveDarkness;

		public List<int> attackApprovalPriority;

		public static DarknessManager Instance { get; private set; }

		[SerializeField]
		private int darknessIDCounter, darknessConcurrentAttackLimit;

		private float calculationTime;
		private bool paused;

		[SerializeField, Header("Assign in Inspector")]
		private Transform player;
		public int maxEnemyCount;

		void Awake()
		{
			Instance = this;
			
			darknessConcurrentAttackLimit = 2;
			darknessIDCounter = 0;
			if (Instance != null && !Instance.gameObject.CompareTag("AI Manager"))
			{
				Debug.LogError("Instance of AI Manager already exist in this scene");
				//Destroy(instance.gameObject.GetComponent<AI_Manager>());
			}
			else Instance = this;
			ActiveDarkness = new Dictionary<int, DarknessController>();
			attackApprovalPriority = new List<int>();
			
			paused = false;
			calculationTime = 0.5f;

			StartCoroutine(ManagedDarknessUpdate());
		}

		void OnEnable()
		{
			DarkEventManager.AddDarkness += AddtoDarknessList;
			DarkEventManager.RemoveDarkness += RemoveFromDarknessList;
		}

		void OnDisable()
		{
			DarkEventManager.AddDarkness -= AddtoDarknessList;
			DarkEventManager.RemoveDarkness -= RemoveFromDarknessList;
		}

		public Vector3 DirectionToPlayer(Vector3 start)
		{
			return (player.position - start).normalized;
		}

		#region DarknessUpdateLoop

		///<summary>Contols the update loop for Darkness objects. Calls Darkness sorting and Darkness approval functions </summary>
		private IEnumerator ManagedDarknessUpdate()
		{
			while (!paused)
			{
				if (attackApprovalPriority.Count > 0)
				{
					DarkEventManager.OnUpdateDarknessDistance();
					SortTheGoons();

					UpdateDarknessAggresion();
					
					yield return new WaitForSeconds(calculationTime);
					DarkEventManager.OnUpdateDarknessStates();
				}
				else yield return new WaitForSeconds(calculationTime);
			}
			yield return null;
		}

		///<summary>Sets the closest Darkness to attack state. Darkness that are runners up are set to patrol nearby. 
		///Furtheset Darkness are set to idle priority</summary>
		private void UpdateDarknessAggresion()
		{
			for (int i = 0; i < attackApprovalPriority.Count; i++)
			{
				if (i < darknessConcurrentAttackLimit)
				{
					ActiveDarkness[attackApprovalPriority[i]].AggressionRatingUpdate(DarknessController.AggresionRating.Attacking);
					//TODO Set attackZone on DarknessMovement, check if zone if already assigned and not blocked
					//if (i + 1 <= attackApprovalPriority.Count - 1)
					//	ActiveDarkness[attackApprovalPriority[i + 1]].movement.UpdateHighAvoidancePoint(ActiveDarkness[attackApprovalPriority[i]].transform.position); //update the avoidance points of the next Darkness sorted in the list
				}
				else
				{
					ActiveDarkness[attackApprovalPriority[i]].AggressionRatingUpdate(DarknessController.AggresionRating.Idling);
					//ActiveDarkness[attackApprovalPriority[i]].movement.UpdateHighAvoidancePoint(Vector3.zero);
				}
			}
		}

		///<summary>Sorts the Darkness in ActiveDarkness based on their distance to target values</summary>
		private void SortTheGoons()
		{
			attackApprovalPriority.Sort(delegate (int a, int b)
			{
				return ActiveDarkness[a].PlayerDistance().CompareTo(ActiveDarkness[b].PlayerDistance());
			});
		}
		#endregion

		#region DarknessCollectionUpdates
		///<summary> Notified by the AddDarkness event. Initializes Darkness parameters and adds to ActiveDakness </summary>
		private void AddtoDarknessList(DarknessController updatedDarknessController)
		{
			updatedDarknessController.transform.SetParent(this.transform);
			darknessIDCounter++;
			
			updatedDarknessController.Spawn(darknessIDCounter);

			ActiveDarkness.Add(updatedDarknessController.creationID, updatedDarknessController);
			attackApprovalPriority.Add(updatedDarknessController.creationID);
			Vector3 pDir = player.position - updatedDarknessController.transform.position;
			updatedDarknessController.transform.Rotate(Vector3.RotateTowards(updatedDarknessController.transform.forward, pDir, 180, 0.0f));
		}

		///<summary>Removes Darkness from attack list if present. Also removes Darkness from active list and stops any relevant running funcitons</summary>
		public void RemoveFromDarknessList(DarknessController updatedDarknessController)
		{
			attackApprovalPriority.Remove(updatedDarknessController.creationID);
			ActiveDarkness.Remove(updatedDarknessController.creationID);
		}

		[ContextMenu("Test KillAllDarkness")]
		public void KillAllDarkness()
		{
			Debug.Log("[AI] All Darkness AI kill call");
			foreach (KeyValuePair<int, DarknessController> dark in ActiveDarkness)
			{
				dark.Value.KillDarkness();
				RemoveFromDarknessList(dark.Value);
				ActiveDarkness.Remove(dark.Key);
			}
		}

		public bool CheckIfSpawnFull()
		{
			return ActiveDarkness.Count > maxEnemyCount;
		}
		#endregion
	}
}