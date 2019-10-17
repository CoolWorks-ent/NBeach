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
	public float calculationTime;
	private bool paused;

	public Dark_State[] dark_States;

	public List<int> attackApprovalPriority; 
	//private Queue<Darkness> engagementQueue, approachQueue;
	private static AI_Manager instance;
	public static AI_Manager Instance
	{
		get {return instance; }
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
		// engagementQueue = new Queue<Darkness>();
		// approachQueue = new Queue<Darkness>();
		ActiveDarkness = new Dictionary<int, Darkness>();
		attackApprovalPriority = new List<int>();
		AddDarkness += AddtoDarknessList;
		RemoveDarkness += RemoveFromDarknessList;
		paused = false;
		calculationTime = 1.5f;
		StartCoroutine(CompareNeighbors());
		foreach(Dark_State d in dark_States)
        {
            d.Startup();
        }
	}

	/*public bool DarknessStateChange(Dark_State toState, Darkness fromController)
	{
		bool result = false;
		switch(toState.stateType)
		{
			case Dark_State.StateType.REMAIN:
				result = false;
				break;
			case Dark_State.StateType.IDLE:
				Debug.LogWarning(String.Format("<color=blue>AI Manager IDLE:</color> Requesting {0} -> {1}",fromController.currentState,toState), fromController);
				result = true;
				break;
			case Dark_State.StateType.CHASING:
				if(fromController.currentState.stateType != Dark_State.StateType.CHASING)
				{
					 //lets check for player movement here
					result = true;
				}
				else result = false; 
				break;
			case Dark_State.StateType.ATTACK:
				if(fromController.agRating == Darkness.AggresionRating.Attacking)
					result = true; 
				else result = false;
				break;
			case Dark_State.StateType.STANDBY: 
				if(fromController.agRating == Darkness.AggresionRating.StandingBy)
				{
					result = true;
				}
				else result = false;
				break;
		}
		return result;
	}*/

	private void AttackerSwap(int usurper, int deposed)
	{
		int temp = usurper;
		Debug.LogError("Starting swap of " + attackApprovalPriority[usurper] + " & " + attackApprovalPriority[deposed]);
		attackApprovalPriority[usurper] = attackApprovalPriority[deposed];
		attackApprovalPriority[deposed] = attackApprovalPriority[temp];
		Debug.LogError("Ending swap of " + attackApprovalPriority[usurper] + " & " + attackApprovalPriority[deposed]);
	}

	private IEnumerator CompareNeighbors() //TODO: Every n seconds go through the Darkness to see who is closer to the player. If found perform QueueSwap with the Darkness that is furthest away
	{
		while(!paused)
		{
			if(attackApprovalPriority.Count > 1)
			{
				//ActiveDarkness.Values.CopyTo(closestDarkness,0);
				foreach(KeyValuePair<int,Darkness> baba in ActiveDarkness)
				{
					baba.Value.DistanceEvaluation();
				}
				SortTheGoons();
				yield return new WaitForSeconds(calculationTime/3);
				ApproveGoonAttack();
				yield return new WaitForSeconds(calculationTime);
			}
			else yield return new WaitForSeconds(0.5f);
		}
		yield return null;
	}

	private void ApproveGoonAttack() //TODO: Add checking for Darkness being swapped to lower position. Preferrably during that swap they would get set to false.
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
				//ActiveDarkness[attackApprovalPriority[i]].agRatingPrevious = ActiveDarkness[attackApprovalPriority[i]].agRatingCurrent;
				//ActiveDarkness[attackApprovalPriority[i]].agRatingCurrent = Darkness.AggresionRating.Attacking;
				//ActiveDarkness[attackApprovalPriority[i]].target = player;
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
	public delegate void AIEvent<T>(T obj);
	public delegate void AIEvent<T1,T2>(T1 obj1, T2 obj2);
	//public delegate void AIEvent<T1,T2, T3>(T1 obj1, T2 obj2, T3 obj3);
	//public static event AIEvent<Dark_State, Darkness, Action<bool, Dark_State, Darkness>> ChangeState;
	public static event AIEvent<Darkness> AddDarkness;
	public static event AIEvent<Darkness> RemoveDarkness;

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
	#endregion
}