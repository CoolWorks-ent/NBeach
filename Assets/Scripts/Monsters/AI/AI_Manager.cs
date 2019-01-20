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
	private Queue<Darkness> engagementQueue;
	private static AI_Manager instance;
	public static AI_Manager Instance
	{
		get {return instance; }
	}

	void Awake()
	{
		darknessConcurrentAttackLimit = 2;
		darknessConcurrentMovingLimit = darknessConcurrentAttackLimit*2;
		darknessIDCounter = darknessMoveCounter = 0;
		indexNull = 0;
		maxEnemyCount = 6;
        minEnemyCount = 1;
		if(instance != null && instance != this)
		{
			//Debug.LogError("Instance of AI Manager already exist in this scene");
		}
		else instance = this;
		engagementQueue = new Queue<Darkness>();
		ActiveDarkness = new Dictionary<int, Darkness>();
		enemyEngagement = new int[3];
		AddDarkness += AddtoDarknessList;
		RemoveDarkness += RemoveFromDarknessList;
		//AttackRequest += ProcessAttackRequest;
		//ChangeState += DarknessStateChange;
		//player = GameObject.FindGameObjectWithTag("PlayerCube").transform;
	}

	void Update()
	{
		/*if(AttackQueue.Count > 0)
			ProcessAttackRequest();*/
	}

	public bool DarknessStateChange(Dark_State.StateType toState, Darkness fromController)//Dark_State toState, Darkness fromController, Action<bool, Dark_State, Darkness> callback)
	{
		//Debug.LogWarning(String.Format("Darkness {0} received request to change from {2} state to {1} state ",fromController.queueID,toState.stateType,fromController.currentState.stateType));
		switch(toState)//.stateType)
		{
			case Dark_State.StateType.CHASING:
				if(fromController.currentState.stateType != Dark_State.StateType.CHASING)
				{
					//callback(true, toState, fromController);
					darknessMoveCounter++;
					//Debug.LogWarning(String.Format("Darkness {0} transitioning to {1} state",fromController.queueID,toState.stateType));
					return true;
				}
				else return false; //callback(false, toState, fromController); Debug.LogWarning(String.Format("Darkness {0} staying in {1} state",fromController.queueID,fromController.currentState));
				break;
			/*	case Dark_State.StateType.WANDER:
				if((fromController.currentState.stateType != Dark_State.StateType.WANDER || fromController.currentState.stateType != Dark_State.StateType.CHASING) && darknessMoveCounter <= darknessConcurrentMovingLimit)
				{
					Debug.LogWarning(String.Format("Darkness {0} transitioning to {1} state",fromController.queueID,toState.stateType));
					darknessMoveCounter++;
					callback(true, toState, fromController);
				}
				else callback(false, toState, fromController); Debug.LogWarning(String.Format("Darkness {0} staying in {1} state",fromController.queueID,fromController.currentState));
				break;*/
			case Dark_State.StateType.ATTACK:
				if(fromController.canAttack)
					return true; //Debug.LogWarning(String.Format("Darkness {0} transitioning to {1} state",fromController.queueID,toState.stateType)); //check to see if they can attack
				else return false;
				break;
		}
		return false;
	}

	///<summary>Check the current engagement collection. If the darkness is not in the list and the list is not full add it to the list</summary>
	public bool CheckAttackRequest(int iD) //Search for an empty spot in the array. After checking all spots if none of them are empty or contain the item I'm looking for return false. If their is an empty spot add the item to that spot.
	{
		for(int i = 0; i < enemyEngagement.Length; i++)
		{
			if(enemyEngagement[i] == iD)
			{
				Debug.Log("Darkness already in engagment list");
				ActiveDarkness[iD].canAttack = true;
				ActiveDarkness[iD].engIndex = i;
				return true;
			}
			else if(enemyEngagement[i] == indexNull)
			{
				Debug.Log("Added darkness to engagment list");
				enemyEngagement[i] = iD;
				ActiveDarkness[iD].canAttack = true;
				ActiveDarkness[iD].engIndex = i;
				return true;
			}
		}
		ActiveDarkness[iD].canAttack = false;
		return false;
	}

	private void AddtoDarknessList(Darkness updatedDarkness)
	{
		darknessIDCounter++;
		updatedDarkness.creationID = darknessIDCounter;
		Debug.Log("New Darkness added. ID#" + updatedDarkness.creationID);
		ActiveDarkness.Add(updatedDarkness.creationID, updatedDarkness);
		updatedDarkness.target = player;
		if(!CheckAttackRequest(updatedDarkness.creationID))
			engagementQueue.Enqueue(updatedDarkness);
		//updatedDarkness.GetComponent<AI_Movement>().target = player;
	}

    public void RemoveFromDarknessList(Darkness updatedDarkness)
    {
		darknessMoveCounter--;
		if(updatedDarkness.engIndex < enemyEngagement.Length)
			enemyEngagement[updatedDarkness.engIndex] = indexNull;
		if (engagementQueue.Count >= 1 && engagementQueue.Peek() != null)
		{
			if(CheckAttackRequest(engagementQueue.Peek().creationID))
			{
				engagementQueue.Dequeue();
			}
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
	public delegate void AIEvent<T1,T2, T3>(T1 obj1, T2 obj2, T3 obj3);
	public static event AIEvent<Dark_State, Darkness, Action<bool, Dark_State, Darkness>> ChangeState;
	public static event AIEvent<Darkness> AddDarkness;
	public static event AIEvent<Darkness> RemoveDarkness;

	public static void OnChangeState(Dark_State to, Darkness from, Action<bool, Dark_State, Darkness> approved)
	{
		if(ChangeState != null)
			ChangeState(to, from, approved);
	}

	public static void OnDarknessAdded(Darkness d)
	{
		if(AddDarkness != null)
			AddDarkness(d);
	}
	public static void OnDarknessRemoved(Darkness d)
	{
		if(RemoveDarkness != null)
			RemoveDarkness(d);
	}
	#endregion
}