using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour {

	public Transform player;
	public Dictionary<int, Darkness> ActiveDarkness;

	[SerializeField]
	private int darknessIDCounter, darknessMoveCounter, darknessConcurrentAttackLimit, attacksCurrentlyProcessed, darknessConcurrentMovingLimit;
	public int maxEnemyCount;
    public int minEnemyCount;
	private int indexNull;

	private int[] enemyEngagement; 
	private Queue<Darkness> engagementQueue, approachQueue;
	private static AI_Manager instance;
	public static AI_Manager Instance
	{
		get {return instance; }
	}

	void Awake()
	{
		darknessConcurrentAttackLimit = 2;
		darknessConcurrentMovingLimit = 3;
		darknessIDCounter = darknessMoveCounter = 0;
		indexNull = 0;
		maxEnemyCount = 6;
        minEnemyCount = 1;
		if(instance != null && !instance.gameObject.CompareTag("AI Manager"))
		{
			Debug.LogError("Instance of AI Manager already exist in this scene");
			//Destroy(instance.gameObject.GetComponent<AI_Manager>());
		}
		else instance = this;
		engagementQueue = new Queue<Darkness>();
		approachQueue = new Queue<Darkness>();
		ActiveDarkness = new Dictionary<int, Darkness>();
		enemyEngagement = new int[3];
		AddDarkness += AddtoDarknessList;
		RemoveDarkness += RemoveFromDarknessList;
	}

	void Update()
	{
		/*if(AttackQueue.Count > 0)
			ProcessAttackRequest();*/
	}

	public bool DarknessStateChange(Dark_State toState, Darkness fromController)
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
					darknessMoveCounter++; //lets check for player movement here
					result = true;
				}
				else result = false; 
				break;
			case Dark_State.StateType.WANDER:
				if((fromController.currentState.stateType != Dark_State.StateType.CHASING))
				{
					Debug.LogWarning(String.Format("<color=green>AI Manager Wander:</color> Transitioning from {0}",fromController.currentState), fromController);
					darknessMoveCounter++;
					result = true;
				}
				else{ result = false; Debug.LogWarning(String.Format("<color=yellow>AI Manager Wander:</color> Transitioning from {0}",fromController.currentState), fromController);}
				break;
			case Dark_State.StateType.ATTACK:
				if(fromController.canAttack)
					result = true; 
				else result = false;
				break;
		}
		return result;
	}

	///<summary>Check the current engagement collection. If the darkness is not in the list and the list is not full add it to the list</summary>
	public bool CheckAttackRequest(int iD) //Search for an empty spot in the array. After checking all spots if none of them are empty or contain the item I'm looking for return false. If there is an empty spot add the item to that spot.
	{
		if(Array.Exists(enemyEngagement, i => i.Equals(indexNull)))
		{
			if(Array.Exists(enemyEngagement, i => i.Equals(iD)))
			{
				Debug.LogWarning(String.Format("<color=purple>Darkness <color=red>({0})</color> already in engagment list</color>", ActiveDarkness[iD].creationID),ActiveDarkness[iD]);
				return false;	
			}
			for(int i = 0; i < enemyEngagement.Length; i++)
			{
				if(enemyEngagement[i] == indexNull && enemyEngagement[i] != iD)
				{
					Debug.LogWarning(String.Format("<color=white>Added darkness <color=green>({0})</color> to engagment list</color>", ActiveDarkness[iD].creationID),ActiveDarkness[iD]);
					enemyEngagement[i] = iD;
					ActiveDarkness[iD].canAttack = true;
					ActiveDarkness[iD].standBy = false;
					ActiveDarkness[iD].engIndex = i;
					return true;
				}
			}
		}
		ActiveDarkness[iD].canAttack = false;
		return false;
	}

	///<summary>Called when adding a new Darkness to the queue. Checks the first few in the queue and sets them to the standby status.</summary>
	private void UpdateStandbyDarkness()
	{
		int i = 0;
		if(engagementQueue.Count > 0)
		{
			Darkness[] darkContainer = engagementQueue.ToArray();
			while(i < darknessConcurrentMovingLimit && i < darkContainer.Length)
			{
				if(ActiveDarkness[darkContainer[i].creationID].standBy)
					i++;
				else
				{
					ActiveDarkness[darkContainer[i].creationID].standBy = true;
					i++;
				}
			}
		}
	}

	///<summary> Notified by the AddDarkness event. If the engagement count is not at cap add the Darkness. Otherwise assign to wait queue.</summary>
	private void AddtoDarknessList(Darkness updatedDarkness)
	{
		updatedDarkness.transform.SetParent(Instance.transform);
		darknessIDCounter++;
		updatedDarkness.creationID = darknessIDCounter;
		Debug.Log("New Darkness added. ID#" + updatedDarkness.creationID);
		ActiveDarkness.Add(updatedDarkness.creationID, updatedDarkness);
		updatedDarkness.target = player;
		/* if(!CheckAttackRequest(updatedDarkness.creationID)) //check to see if the queue has grunts in it first
		{
		}*/
		engagementQueue.Enqueue(updatedDarkness);
		if(!CheckAttackRequest(engagementQueue.Peek().creationID))
			UpdateStandbyDarkness();
		else engagementQueue.Dequeue();
		//updatedDarkness.GetComponent<AI_Movement>().target = player;
	}

	///<summary></summary>
    public void RemoveFromDarknessList(Darkness updatedDarkness)
    {
		darknessMoveCounter--;
		if(enemyEngagement[updatedDarkness.engIndex] != indexNull && updatedDarkness.engIndex < enemyEngagement.Length)
			enemyEngagement[updatedDarkness.engIndex] = indexNull;
		if (engagementQueue.Count >= 1)
		{
			if(engagementQueue.Peek() != null)
			{
				if(CheckAttackRequest(engagementQueue.Peek().creationID)) //Log ID number is being added number 
				{
					//Debug.LogWarning(String.Format("<color=gray>AI Darkness Removed:</color> Adding this darkness {0}", engagementQueue.Peek()), engagementQueue.Peek());
					engagementQueue.Dequeue();
					UpdateStandbyDarkness();
				}
			}
			else engagementQueue.Dequeue();
		}
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
	
	/* public static void OnChangeState(Dark_State to, Darkness from, Action<bool, Dark_State, Darkness> approved)
	{
		if(ChangeState != null)
			ChangeState(to, from, approved);
	}*/
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