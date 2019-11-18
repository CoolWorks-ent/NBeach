using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour {

	public Transform player;
	public Dictionary<int, Darkness> ActiveDarkness;

	[SerializeField]
	private int darknessIDCounter, darknessConcurrentAttackLimit;
	public int maxEnemyCount, minEnemyCount, darkTotalCount, darkAttackCount, darkStandbyCount;
	public float calculationTime, attackOffset;
	private bool paused;

	public Dark_State[] dark_States;
	private NavigationTarget[] PatrolPoints;
	private NavigationTarget[] AttackPoints;

	public List<int> attackApprovalPriority; 
	//private Queue<Darkness> engagementQueue, approachQueue;
	private static AI_Manager instance;
	public static AI_Manager Instance
	{
		get {return instance; }
	}
	public struct NavigationTarget
	{
		public bool occupied;
		public int assignedDarknessID;
		public Transform location;

		public NavigationTarget(Transform loc, bool occ, int asID)
		{
			occupied = occ;
			location = loc;
			assignedDarknessID = asID;
		}
	}

	void Awake()
	{
		dark_States = Resources.LoadAll<Dark_State>("States");
		darknessConcurrentAttackLimit = 2;
		darknessIDCounter = darkTotalCount = darkAttackCount = darkStandbyCount = 0;
		maxEnemyCount = 6;
        minEnemyCount = 1;
		if(instance != null && !instance.gameObject.CompareTag("AI Manager"))
		{
			Debug.LogError("Instance of AI Manager already exist in this scene");
			//Destroy(instance.gameObject.GetComponent<AI_Manager>());
		}
		else instance = this;
		ActiveDarkness = new Dictionary<int, Darkness>();
		attackApprovalPriority = new List<int>();
		AddDarkness += AddtoDarknessList;
		RemoveDarkness += RemoveFromDarknessList;
		paused = false;
		calculationTime = 1.5f;
		attackOffset = 3.5f;
		PatrolPoints = new NavigationTarget[4]; //Declare Patrol points in start
		AttackPoints = new NavigationTarget[3]; //Declare Attack points in start
		StartCoroutine(ManagedDarknessUpdate());
		foreach(Dark_State d in dark_States)
        {
            d.Startup();
        }
	}

	void Start()
	{
		for(int i = 0; i < AttackPoints.Length; i++)
		{
			AttackPoints[i] = new NavigationTarget(new GameObject("attackPoint" + i).transform, false, 0);
			AttackPoints[i].location.parent = player;
		}

		for(int i = 0; i < PatrolPoints.Length; i++)
		{
			PatrolPoints[i] = new NavigationTarget(new GameObject("patrolPoint" + i).transform, false, 0);
			PatrolPoints[i].location.parent = this.transform;
		}

		/*PatrolPoints[0].location.position = ; //TODO Choose random points within a radial range to path to
		PatrolPoints[1].location.position = ;
		PatrolPoints[2].location.position = ;
		PatrolPoints[3].location.position = ;*/

		AttackPoints[0].location.position = new Vector3(player.position.x + attackOffset, player.position.y, player.position.z);//right of player
		AttackPoints[1].location.position = new Vector3(player.position.x - attackOffset, player.position.y, player.position.z);//left of player
		AttackPoints[2].location.position = new Vector3(player.position.x, player.position.y, player.position.z + attackOffset);//front of player
	}

	private Vector3 GeneratePatrolPoint()
	{

		return new Vector3();
	}

	private void AttackerSwap(int usurper, int deposed)
	{
		int temp = usurper;
		Debug.LogError("Starting swap of " + attackApprovalPriority[usurper] + " & " + attackApprovalPriority[deposed]);
		attackApprovalPriority[usurper] = attackApprovalPriority[deposed];
		attackApprovalPriority[deposed] = attackApprovalPriority[temp];
		Debug.LogError("Ending swap of " + attackApprovalPriority[usurper] + " & " + attackApprovalPriority[deposed]);
	}

	private IEnumerator ManagedDarknessUpdate() //TODO: Every n seconds go through the Darkness to see who is closer to the player. If found perform QueueSwap with the Darkness that is furthest away
	{
		while(!paused)
		{
			if(attackApprovalPriority.Count > 0)
			{
				//ActiveDarkness.Values.CopyTo(closestDarkness,0);
				foreach(KeyValuePair<int,Darkness> baba in ActiveDarkness)
				{
					baba.Value.DistanceEvaluation();
				}
				SortTheGoons();
				yield return new WaitForSeconds(calculationTime/3);
				ApproveDarknessAttack();
				yield return new WaitForSeconds(calculationTime);
			}
			else yield return new WaitForSeconds(0.5f);
			//OnMovementUpdate();
		}
		yield return null;
	}

	private void ApproveDarknessAttack() //TODO: Add checking for Darkness being swapped to lower position. Preferrably during that swap they would get set to false.
	{
		darkStandbyCount = 0;
		darkAttackCount = 0;
		darkTotalCount = ActiveDarkness.Count;
		for(int i = 0; i < attackApprovalPriority.Count; i++)
		{
			if(i < darknessConcurrentAttackLimit)
			{
				darkAttackCount++;
				ActiveDarkness[attackApprovalPriority[i]].AggressionChanged(Darkness.AggresionRating.Attacking);
			}
			else if(i < darknessConcurrentAttackLimit+2)
			{
				if(ActiveDarkness[attackApprovalPriority[i]].agRatingCurrent != Darkness.AggresionRating.CatchingUp)
				{
					darkStandbyCount++;
					ActiveDarkness[attackApprovalPriority[i]].AggressionChanged(Darkness.AggresionRating.Wandering);
				}
			}
			else 
			{
				if(ActiveDarkness[attackApprovalPriority[i]].agRatingCurrent != Darkness.AggresionRating.CatchingUp)
				{
					ActiveDarkness[attackApprovalPriority[i]].AggressionChanged(Darkness.AggresionRating.Idling);
				}
			}
		}
	}

	//TODO Add function to assign Darkness to the approved attack locations. Otherwise the target locations are null

	private void SortTheGoons() 
	{
		attackApprovalPriority.Sort(delegate(int a, int b)
		{
			return ActiveDarkness[a].targetDist.CompareTo(ActiveDarkness[b].targetDist);
		});
	}	

	///<summary> Notified by the AddDarkness event. If the engagement count is not at cap add the Darkness. Otherwise assign to wait queue.</summary>
	private void AddtoDarknessList(Darkness updatedDarkness)
	{
		updatedDarkness.transform.SetParent(Instance.transform);
		darknessIDCounter++;
		updatedDarkness.creationID = darknessIDCounter;

		ActiveDarkness.Add(updatedDarkness.creationID, updatedDarkness);
		attackApprovalPriority.Add(updatedDarkness.creationID);
		updatedDarkness.Target = player;
	}

	///<summary></summary>
    public void RemoveFromDarknessList(Darkness updatedDarkness)
    {
		attackApprovalPriority.Remove(updatedDarkness.creationID);
        ActiveDarkness.Remove(updatedDarkness.creationID);
    }

	public void KillAllDarkness()
    {
        Debug.Log("[AI] All Darkness AI kill call");
        foreach(KeyValuePair<int, Darkness>dark in ActiveDarkness)
        {
			OnDarknessRemoved(dark.Value);
            ActiveDarkness.Remove(dark.Key);
        }
    }

    public IEnumerator WaitTimer(float timer)
	{
		yield return new WaitForSeconds(timer);
	}

#region AIManagerEvents
	public delegate void AIEvent();
	public delegate void AIEvent<T>(T obj);
	public static event AIEvent<Darkness> AddDarkness;
	public static event AIEvent<Darkness> RemoveDarkness;
	public static event AIEvent UpdateMovement;

	///<summary>
	///Adds Darkness to list. Darkness will be assigned their spot in the queue and set to attack if queue is not full.
	///</summary>
	public static void OnDarknessAdded(Darkness d)
	{
		if(AddDarkness != null)
			AddDarkness(d);
	}
	///<summary>
	///Removes Darkness from list. If in active queue, Darkness will be removed.
	///</summary>
	public static void OnDarknessRemoved(Darkness d)
	{
		if(RemoveDarkness != null)
			RemoveDarkness(d);
	}

	/*public static void OnMovementUpdate()
	{
		if(UpdateMovement != null)
			UpdateMovement();
	}*/

	#endregion
	
}