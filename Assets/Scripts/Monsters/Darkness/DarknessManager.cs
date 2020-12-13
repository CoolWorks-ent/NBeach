using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DarknessMinion
{
	public class DarknessManager : MonoBehaviour
	{

		public Transform player, attackPointHolder;
		public Dictionary<int, Darkness> ActiveDarkness;

		[SerializeField]
		private int darknessIDCounter, darknessConcurrentAttackLimit;
		public int maxEnemyCount, minEnemyCount;

		private float calculationTime, attackOffset;
		private bool paused;
		
		[SerializeField]
		private NavigationTarget[] AttackPoints;
		[SerializeField]
		//private NavigationTarget PlayerPoint; //StartPoint,

		public List<int> attackApprovalPriority;

		public float groundLevel {get { return player.position.y; }}

		public static DarknessManager Instance { get; private set; }

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
			DarkEventManager.RequestNewTarget += ApproveRequestedTarget;
			paused = false;
			calculationTime = 0.5f;
			attackOffset = 2.75f;
			
			StartCoroutine(ManagedDarknessUpdate());
		}

		void Start()
		{
			//StartPoint = new Darkness.NavigationTarget(this.transform.position, Vector3.zero, oceanPlane.position.y, Darkness.NavTargetTag.Neutral);
			List<Transform> playerAttackPoints = new List<Transform>();
			

			foreach(Transform t in attackPointHolder.GetComponentsInChildren<Transform>())
			{
				if(!t.gameObject.CompareTag("Attack Points"))
					playerAttackPoints.Add(t);
			}
			AttackPoints = new NavigationTarget[playerAttackPoints.Count];

			for (int i = 0; i < AttackPoints.Length; i++)
			{
				AttackPoints[i] = new NavigationTarget(playerAttackPoints[i], NavigationTarget.NavTargetTag.Attack);
				//Debug.Log("Attack point locaiton " + AttackPoints[i].objectTransform.position);
				/*
				Vector3 vector;
				if(offsets[i].x < 0)
					vector = offsets[i] + new Vector3(1.5f, 0, 1.75f);
				else if(offsets[i].x > 0) 
					vector = offsets[i] - new Vector3(1.5f, 0, -1.75f);
				else vector = offsets[i] - new Vector3(0, 0, -1.65f);
				AttackPoints[i] = new NavigationTarget(player.transform.position, offsets[i], vector, oceanPlane.position.y, NavigationTarget.NavTargetTag.Attack);
				*/
				//Vector3 t = AttackPoints[i].position+offsets[i];
				//Debug.LogWarning(string.Format("Attack point location AttackPoint[{0}]" + t, i));
			}
		}

		void LateUpdate()
		{
			//PlayerPoint.UpdateLocation(player.position);
			/*foreach(NavigationTarget point in AttackPoints)
			{
				point.UpdateLocation(player.position);
			}*/
		}
		#region Helpers
		public Vector3 PlayerToDirection(Vector3 start)
		{
			return player.position - start;
		}
		#endregion

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
					DarkEventManager.OnUpdateDarknessDistance(player.position);
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
				return ActiveDarkness[a].playerDist.CompareTo(ActiveDarkness[b].playerDist);
			});
		}
		#endregion

		#region NavTargetHandling

		///<summary>Returns index of unclaimed attack Navigation Target</summary>
		public int LeastRequestedAttackTarget() 
		{
			int lowest = 0;
			List<int> evenCount = new List<int>(); 
			for (int i = 0; i < AttackPoints.Length; i++)
			{
				if (AttackPoints[i].navTargetClaimed)
					continue;
				else evenCount.Add(i); 
			}

			if (evenCount.Count >= 2)
			{
				lowest = evenCount[Random.Range(0, evenCount.Count - 1)];
				return lowest;
			}
			else if (evenCount.Count < 2 && evenCount.Count > 0)
				return evenCount[0];
			else return -1;
		}

		///<summary>Check the Darkness for current NavTarget. If the target is an attack Target the target will be set to the starting NavTarget.</summary>
		private void RemoveFromNavTargets(int darkID)
		{
			Darkness darkness;
			if (ActiveDarkness.TryGetValue(darkID, out darkness))
			{
				if (darkness.navTarget.navTargetTag != NavigationTarget.NavTargetTag.Neutral)
				{
					darkness.navTarget.ReleaseTarget();
				}
			}
		}

		///<summary>Processes Darkness request for a  NavTarget. Assign a new target to the requestor Darkness if a valid request</summary> //--Work in Progress--
		private void ApproveRequestedTarget(int darkID) //TODO Darkness will make request for new Navigation Targets based on their status
		{
			Darkness darkness;
			if (ActiveDarkness.TryGetValue(darkID, out darkness))
			{	
				if (darkness.agRatingCurrent == Darkness.AggresionRating.Attacking)
				{
					int index = LeastRequestedAttackTarget();
					if (index != -1)
					{
						AttackPoints[index].ClaimTarget(darkID);
						darkness.navTarget = AttackPoints[index]; 
					}
				}
				else 
				{
					if(darkness.navTarget.navTargetTag == NavigationTarget.NavTargetTag.Attack)
					{
						RemoveFromNavTargets(darkID);
						darkness.CreateDummyNavTarget(player.position.y);
					}
				}
			}
		}
		#endregion

		#region DarknessCollectionUpdates
		///<summary> Notified by the AddDarkness event. Initializes Darkness parameters and adds to ActiveDakness </summary>
		private void AddtoDarknessList(Darkness updatedDarkness)
		{
			updatedDarkness.transform.SetParent(Instance.transform);
			darknessIDCounter++;
			
			updatedDarkness.Spawn(darknessIDCounter, groundLevel);

			ActiveDarkness.Add(updatedDarkness.creationID, updatedDarkness);
			attackApprovalPriority.Add(updatedDarkness.creationID);
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

		private void OnDrawGizmos()
        {
			foreach(NavigationTarget n in AttackPoints)
			{
				if(n.navTargetClaimed)
				{
					Gizmos.color = Color.red;
				}
				else Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(n.objectTransform.position, 1);
				//Gizmos.color = Color.green;
				//Gizmos.DrawSphere(n.closeToSrcPosition, 0.5f);
			}
        }
	}
}