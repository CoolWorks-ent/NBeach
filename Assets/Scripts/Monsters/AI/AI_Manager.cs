using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour {

	public Transform player;
	public Dictionary<int, Darkness> ActiveDarkness;

	[SerializeField]
	private int darknessIDCounter, darknessConcurrentAttackLimit, attacksCurrentlyProcessed;
	public int maxEnemyCount;
    public int minEnemyCount;

	private List<int> enemyEngagement;
	private static AI_Manager instance;
	public static AI_Manager Instance
	{
		get {return instance; }
	}

	void Awake()
	{
		darknessConcurrentAttackLimit = 2;
		darknessIDCounter = 0;
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
		RemoveDarkness += RemoveFromDarknessList;
		AttackRequest += ProcessAttackRequest;
		ChangeState += DarknessStateChange;
		//player = GameObject.FindGameObjectWithTag("PlayerCube").transform;
	}

	void Update()
	{
		/*if(AttackQueue.Count > 0)
			ProcessAttackRequest();*/
	}

	private void DarknessStateChange(Dark_State toState, Darkness fromController, Action<bool, Dark_State, Darkness> callback)
	{
		switch(toState.stateType)
		{
			case Dark_State.StateType.CHASING:
				if(fromController.canAttack && enemyEngagement.Count <= darknessConcurrentAttackLimit)
				{
					callback(true, toState, fromController);
				}
				else callback(false, toState, fromController);
				break;
			/*case Dark_State.StateType.WANDER:
				callback(true, toState, fromController);
				break;*/

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
					//AttackQueue.Enqueue(iD); 
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
		ActiveDarkness.Add(darknessIDCounter, updatedDarkness);
		updatedDarkness.queueID = darknessIDCounter;
		updatedDarkness.target = player;
		//updatedDarkness.GetComponent<AI_Movement>().target = player;
	}

    private void RemoveFromDarknessList(Darkness updatedDarkness)
    {
		if(ActiveDarkness[updatedDarkness.queueID].canAttack)
			enemyEngagement.Remove(updatedDarkness.queueID);
        ActiveDarkness.Remove(updatedDarkness.queueID);
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