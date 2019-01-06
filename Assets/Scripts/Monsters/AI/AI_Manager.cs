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

	private List<int> enemyEngagement;
	//private Queue<Darkness> engagementQueue;
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
		maxEnemyCount = 5;
        minEnemyCount = 3;
		if(instance != null && instance != this)
		{
			//Debug.LogError("Instance of AI Manager already exist in this scene");
		}
		else instance = this;

		ActiveDarkness = new Dictionary<int, Darkness>();
		enemyEngagement = new List<int>();
		AddDarkness += AddtoDarknessList;
		//RemoveDarkness += RemoveFromDarknessList;
		AttackRequest += ProcessAttackRequest;
		ChangeState += DarknessStateChange;
		//player = GameObject.FindGameObjectWithTag("PlayerCube").transform;
	}

	void Update()
	{
		/*if(AttackQueue.Count > 0)
			ProcessAttackRequest();*/
	}

	public bool DarknessStateChange(Dark_State.StateType toState, Dark_State fromState, Darkness fromController)//Dark_State toState, Darkness fromController, Action<bool, Dark_State, Darkness> callback)
	{
		//Debug.LogWarning(String.Format("Darkness {0} received request to change from {2} state to {1} state ",fromController.queueID,toState.stateType,fromController.currentState.stateType));
		switch(toState)//.stateType)
		{
			case Dark_State.StateType.CHASING:
				if(CanMove(fromController.queueID) && fromController.currentState.stateType != Dark_State.StateType.CHASING)
				{
					//callback(true, toState, fromController);
					darknessMoveCounter++;
					//Debug.LogWarning(String.Format("Darkness {0} transitioning to {1} state",fromController.queueID,toState.stateType));
					return true;
				}
				else return false; //callback(false, toState, fromController); Debug.LogWarning(String.Format("Darkness {0} staying in {1} state",fromController.queueID,fromController.currentState));
				break;
			/* case Dark_State.StateType.WANDER:
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
	}

	///<summary>Notify AI manager to add a Darkness unit to the queue to decide which units can attack next</summary>
	private void ProcessAttackRequest(int iD, bool requested)
	{
		if(!requested)
		{
			if(enemyEngagement.Count < 3)
			{
				Darkness dark;
				if(ActiveDarkness.TryGetValue(iD, out dark))
				{
					dark.attackRequested = true;
					enemyEngagement.Add(iD);
				}
				Debug.Log("<b><color=green>AI Manager:</color></b> Darkness #" + iD + " request has been processed");
			}
		}
		else
		{
			Debug.LogWarning("<b><color=red>AI Manager:</color></b> Darkness #" + iD + " has already requested to attack");
			//ProcessAttackRequest();
		} 
	}

	private void AddtoDarknessList(Darkness updatedDarkness)
	{
		darknessIDCounter++;
		updatedDarkness.queueID = darknessIDCounter;
		Debug.Log("New Darkness added. ID#" + updatedDarkness.queueID);
		ActiveDarkness.Add(updatedDarkness.queueID, updatedDarkness);
		updatedDarkness.target = player;
		//updatedDarkness.GetComponent<AI_Movement>().target = player;
	}

    public void RemoveFromDarknessList(Darkness updatedDarkness)
    {
		if(ActiveDarkness[updatedDarkness.queueID].canAttack)
		{
			enemyEngagement.Remove(updatedDarkness.queueID);
			darknessMoveCounter--;
		}
        ActiveDarkness.Remove(updatedDarkness.queueID);
    }

	public bool CanMove(int ID)
	{
		if(enemyEngagement.Contains(ID))
			return true;
		else if(enemyEngagement.Count <= 3)
		{
			ProcessAttackRequest(ID, false);
			return true;
		}
		else return false;
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
	public static event AIEvent<int, bool> AttackRequest;
	public static event AIEvent<Dark_State, Darkness, Action<bool, Dark_State, Darkness>> ChangeState;
	public static event AIEvent<Darkness> AddDarkness;
	public static event AIEvent<Darkness> RemoveDarkness;

	public static void OnAttackRequest(int queueID, bool attackRequested)
	{
		if(AttackRequest != null)
			AttackRequest(queueID, attackRequested);
	}

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