using System.Collections;
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
		public int maxEnemyCount, minEnemyCount;

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
			DarkEventManager.AddDarkness += AddtoDarknessList;
			DarkEventManager.RemoveDarkness += RemoveFromDarknessList;
			paused = false;
			calculationTime = 0.5f;
			//attackOffset = 2.75f;
			
			StartCoroutine(ManagedDarknessUpdate());
		}

		void Start()
		{
			List<Transform> playerAttackPoints = new List<Transform>();
			/*foreach(Transform t in attackPointHolder.GetComponentsInChildren<Transform>())
			{
				if(!t.gameObject.CompareTag("Attack Points"))
					playerAttackPoints.Add(t);
			}*/
		}

		void LateUpdate()
		{
			//PlayerPoint.UpdateLocation(player.position);
			/*foreach(NavigationTarget point in AttackPoints)
			{
				point.UpdateLocation(player.position);
			}*/
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
					//ActiveDarkness.Values.CopyTo(closestDarkness,0);
					/*foreach (KeyValuePair<int, Darkness> dark in ActiveDarkness)
					{
						dark.Value.DistanceEvaluation(player.position);
					}*/
					DarkEventManager.OnUpdateDarknessDistance();
					SortTheGoons();
					//yield return new WaitForSeconds(calculationTime / 3);
					UpdateDarknessAggresion();
					yield return new WaitForSeconds(calculationTime);
					DarkEventManager.OnUpdateDarknessStates();
				}
				else yield return new WaitForSeconds(calculationTime);
				maxEnemyCount = 4; //TODO removve once done testing
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
				}
				/*else if (i < darknessConcurrentAttackLimit + 2)
				{
					ActiveDarkness[attackApprovalPriority[i]].AggressionChanged(Darkness.AggresionRating.Wandering);
				}*/
				else
				{
					ActiveDarkness[attackApprovalPriority[i]].AggressionRatingUpdate(Darkness.AggresionRating.Idling);
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
			//updatedDarkness.StartCoroutine(updatedDarkness.ExecuteCurrentState());
		}

		///<summary>Removes Darkness from attack list if present. Also removes Darkness from active list and stops any relevant running funcitons</summary>
		public void RemoveFromDarknessList(Darkness updatedDarkness)
		{
			//updatedDarkness.StopCoroutine(updatedDarkness.ExecuteCurrentState());
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