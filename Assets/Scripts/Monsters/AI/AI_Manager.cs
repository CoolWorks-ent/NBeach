using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour {

	public Transform player;
	public Dictionary<int, Darkness> ActiveDarkness;
	private Queue<int> AttackQueue;
	[SerializeField]
	private int darknessQueueID, darknessConcurrentAttackLimit, attacksCurrentlyProcessed;
	public int maxEnemyCount;
    public int minEnemyCount;
	private static AI_Manager instance;
	public static AI_Manager Instance
	{
		get {return instance; }
	}

	void Awake()
	{
		darknessConcurrentAttackLimit = 2;
		darknessQueueID = 0;
		maxEnemyCount = 2;
        minEnemyCount = 3;
		if(instance != null && instance != this)
		{
			//Debug.LogError("Instance of AI Manager already exist in this scene");
		}
		else instance = this;

		ActiveDarkness = new Dictionary<int, Darkness>();
		AddDarkness += AddtoDarknessList;
		RemoveDarkness += RemoveFromDarknessList;
		AttackRequest += UpdateAttackRequest;
		AttackQueue = new Queue<int>();
		//player = GameObject.FindGameObjectWithTag("PlayerCube").transform;
	}

	void Update()
	{
		/*if(AttackQueue.Count > 0)
			ProcessAttackRequest();*/
	}

	///<summary>Notify AI manager to add a Darkness unit to the queue to decide which units can attack next</summary>
	private void UpdateAttackRequest(int iD, bool requested)
	{
		if(!requested)
		{
			AttackQueue.Enqueue(iD); 
			Debug.Log("<b><color=green>AI Manager:</color></b> Darkness #" + iD + " request has been processed");
		}
		else
		{
			Debug.LogWarning("<b><color=red>AI Manager:</color></b> Darkness #" + iD + " has already requested to attack");
			ProcessAttackRequest();
		} 
	}

	///<summary>
	///Checks the status of the AI on the top of the queue to see if the attack has finished or was interrupted. 
	///Once a result has been sent the units will be dequeued.
	///</summary>
	private void ProcessAttackRequest()
	{
		//TODO check if current darkness 
		if(attacksCurrentlyProcessed <= darknessConcurrentAttackLimit)
		{
			if(ActiveDarkness.ContainsKey(AttackQueue.Peek()))
			{
				ActiveDarkness[AttackQueue.Peek()].canAttack = true;
				attacksCurrentlyProcessed++;
				AttackQueue.Dequeue();
				ProcessAttackRequest();
			}
			else 
			{
				AttackQueue.Dequeue();
			}
		}
		// if(AttackQueue.Count >= 0 && attackConfirmations <= attackPriorityCount)
		// {
		// 	for(int i = 0; i <= darknessAttackCount || i <= AttackQueue.Count; i++)
		// 	{
		// 		attackConfirmations++;	
		// 	}
		// }
	}

	private void AddtoDarknessList(Darkness updatedDarkness)
	{
		darknessQueueID++;
		ActiveDarkness.Add(darknessQueueID, updatedDarkness);
		updatedDarkness.queueID = darknessQueueID;
		updatedDarkness.target = player;
	}

    private void RemoveFromDarknessList(int updatedDarkness)
    {
        ActiveDarkness.Remove(updatedDarkness);
    }

    public IEnumerator WaitTimer(float timer)
	{
		yield return new WaitForSeconds(timer);
	}

	public delegate void AIEvent<T>(T obj);
	public delegate void AIEvent<T1,T2>(T1 obj1, T2 obj2);
	public static event AIEvent<int, bool> AttackRequest;
	public static event AIEvent<int> AttackEnded;
	public static event AIEvent<Darkness> AddDarkness;
	public static event AIEvent<int> RemoveDarkness;

	public static void OnAttackRequest(int iD, bool request)
	{
		if(AttackRequest != null)
			AttackRequest(iD, request);			
	}

	public static void OnAttackEnded(int d)
	{
		if(AttackEnded != null)
			AttackEnded(d);
	}

	public static void OnDarknessAdded(Darkness d)
	{
		if(AddDarkness != null)
			AddDarkness(d);
	}
	public static void OnDarknessRemoved(int d)
	{
		if(RemoveDarkness != null)
			RemoveDarkness(d);
	}
}
