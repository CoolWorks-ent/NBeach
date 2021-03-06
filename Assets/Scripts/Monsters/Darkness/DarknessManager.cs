﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DarknessMinion
{
	public class DarknessManager : MonoBehaviour
	{
		public Transform playerTransform { get { return player; } }
		public Vector3 playerVector { get { return player.position; } }
		public Dictionary<int, Darkness> ActiveDarkness;

		public List<int> attackApprovalPriority;

		public float groundLevel { get { return player.position.y; } }

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
			ActiveDarkness = new Dictionary<int, Darkness>();
			attackApprovalPriority = new List<int>();
			
			paused = false;
			calculationTime = 0.5f;
			//attackOffset = 2.75f;

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
					ActiveDarkness[attackApprovalPriority[i]].AggressionRatingUpdate(Darkness.AggresionRating.Attacking);
					//if (i + 1 <= attackApprovalPriority.Count - 1)
					//	ActiveDarkness[attackApprovalPriority[i + 1]].movement.UpdateHighAvoidancePoint(ActiveDarkness[attackApprovalPriority[i]].transform.position); //update the avoidance points of the next Darkness sorted in the list
				}
				else
				{
					ActiveDarkness[attackApprovalPriority[i]].AggressionRatingUpdate(Darkness.AggresionRating.Idling);
					//ActiveDarkness[attackApprovalPriority[i]].movement.UpdateHighAvoidancePoint(Vector3.zero);
				}
			}
		}

		///<summary>Sorts the Darkness in ActiveDarkness based on their distance to target values</summary>
		private void SortTheGoons()
		{
			attackApprovalPriority.Sort(delegate (int a, int b)
			{
				return ActiveDarkness[a].movement.playerDist.CompareTo(ActiveDarkness[b].movement.playerDist);
			});
		}
		#endregion

		#region DarknessCollectionUpdates
		///<summary> Notified by the AddDarkness event. Initializes Darkness parameters and adds to ActiveDakness </summary>
		private void AddtoDarknessList(Darkness updatedDarkness)
		{
			updatedDarkness.transform.SetParent(Instance.transform);
			darknessIDCounter++;
			
			updatedDarkness.Spawn(darknessIDCounter);

			ActiveDarkness.Add(updatedDarkness.creationID, updatedDarkness);
			attackApprovalPriority.Add(updatedDarkness.creationID);
			updatedDarkness.movement.player = player;
			Vector3 pDir = player.position - updatedDarkness.transform.position;
			updatedDarkness.transform.Rotate(Vector3.RotateTowards(updatedDarkness.transform.forward, pDir, 180, 0.0f));
		}

		///<summary>Removes Darkness from attack list if present. Also removes Darkness from active list and stops any relevant running funcitons</summary>
		public void RemoveFromDarknessList(Darkness updatedDarkness)
		{
			attackApprovalPriority.Remove(updatedDarkness.creationID);
			ActiveDarkness.Remove(updatedDarkness.creationID);
		}

		[ContextMenu("Test KillAllDarkness")]
		public void KillAllDarkness()
		{
			Debug.Log("[AI] All Darkness AI kill call");
			foreach (KeyValuePair<int, Darkness> dark in ActiveDarkness)
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