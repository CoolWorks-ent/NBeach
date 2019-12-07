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

	[SerializeField]
	private Dark_State[] dark_States;
	private NavigationTarget[] PatrolPoints;
	private NavigationTarget[] AttackPoints;

	public List<int> attackApprovalPriority; 
	//private Queue<Darkness> engagementQueue, approachQueue;
	private static AI_Manager instance;
	public static AI_Manager Instance
	{
		get {return instance; }
	}
	public class NavigationTarget
	{
		public int targetID, 
					weight, 
					weightCap;
		public Transform location;

		public NavigationTarget(Transform loc, int iD)//, int assignmentLimit)
		{
			location = loc;
			targetID = iD;
			//assignedDarknessIDs = new int[assignmentLimit];
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
		PatrolPoints = new NavigationTarget[4]; 
		AttackPoints = new NavigationTarget[3]; 
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
			AttackPoints[i] = new NavigationTarget(new GameObject("attackPoint" + i).transform, i);
			AttackPoints[i].location.parent = player;
			AttackPoints[i].targetID = i;
		}

		for(int i = 0; i < PatrolPoints.Length; i++)
		{
			PatrolPoints[i] = new NavigationTarget(new GameObject("patrolPoint" + i).transform, i);
			float xOffset = 0;
			PatrolPoints[i].location.parent = this.transform; //TODO make this locations unique and not trash
			if(i % 2 == 0 || i == 0)
				xOffset = this.transform.position.x - Random.Range(5+i, 15);
			else xOffset = this.transform.position.x + Random.Range(5+i, 15);
			PatrolPoints[i].location.position = new Vector3(xOffset, player.position.y, this.transform.position.z + Random.Range(-9, 9+i));
			PatrolPoints[i].targetID = i;
		}

		AttackPoints[0].location.position = new Vector3(player.position.x + attackOffset, 0f, player.position.z);//right of player
		AttackPoints[1].location.position = new Vector3(player.position.x - attackOffset, 0f, player.position.z);//left of player
		AttackPoints[2].location.position = new Vector3(player.position.x, 0f, player.position.z - attackOffset);//front of player
	}

	private IEnumerator ManagedDarknessUpdate() 
	{
		while(!paused)
		{
			if(attackApprovalPriority.Count > 0)
			{
				//ActiveDarkness.Values.CopyTo(closestDarkness,0);
				foreach(KeyValuePair<int,Darkness> dark in ActiveDarkness)
				{
					dark.Value.DistanceEvaluation();
				}
				SortTheGoons();
				yield return new WaitForSeconds(calculationTime/3);
				ApproveDarknessAttack();
				yield return new WaitForSeconds(calculationTime);
			}
			else yield return new WaitForSeconds(0.5f);
		}
		yield return null;
	}

	private void ApproveDarknessAttack() 
	{
		darkStandbyCount = 0;
		darkAttackCount = 0;
		darkTotalCount = ActiveDarkness.Count;
		for(int i = 0; i < attackApprovalPriority.Count; i++)
		{
			if(i < darknessConcurrentAttackLimit)
			{ //TODO check for current NavTarget. Decrement weight of current if changing to new 
				darkAttackCount++;
				ActiveDarkness[attackApprovalPriority[i]].AggressionChanged(Darkness.AggresionRating.Attacking);
				NavigationTarget nT = AssignNavigationTarget(ActiveDarkness[attackApprovalPriority[i]].creationID, true);
				if(nT != null)
				{
					ActiveDarkness[attackApprovalPriority[i]].Target = nT;
				}
			}
			else if(i < darknessConcurrentAttackLimit+2)
			{
				if(ActiveDarkness[attackApprovalPriority[i]].agRatingCurrent != Darkness.AggresionRating.CatchingUp)
				{
					darkStandbyCount++;//remove previous navTarget
					ActiveDarkness[attackApprovalPriority[i]].AggressionChanged(Darkness.AggresionRating.Wandering);
					NavigationTarget nT = AssignNavigationTarget(ActiveDarkness[attackApprovalPriority[i]].creationID, false);
					if(nT != null)
					{
						ActiveDarkness[attackApprovalPriority[i]].Target = nT;
					}
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

	public int LeastRequestedNavigationTarget(NavigationTarget[] navTargets) //TODO Create checking for if all targets are at capacity
	{
		int lowest = 0;
		List<int> evenCount = new List<int>();
		for(int i = 0; i < navTargets.Length; i++)
		{
			if(navTargets[i].weight < navTargets[lowest].weight)
				lowest = i;
			else if(navTargets[i].weight == navTargets[lowest].weight)
				evenCount.Add(i);
		}

		if(evenCount.Count >= 2)
			lowest = evenCount[Random.Range(0, evenCount.Count-1)];
		return lowest;
	}

	private NavigationTarget AssignNavigationTarget(int darkID, bool attack) 
	{
		//Find if Darkness is in the collection
		Darkness darkness;
		if(ActiveDarkness.TryGetValue(darkID, out darkness))
		{
			if(attack)
			{
				int index = LeastRequestedNavigationTarget(AttackPoints);
				AttackPoints[index].weight++;
				return AttackPoints[index];
			}
			else 
			{
				int index = LeastRequestedNavigationTarget(PatrolPoints);
				PatrolPoints[index].weight++;
				return PatrolPoints[index];
			}
		}
		else 
		{
			Debug.LogError(string.Format("Darkness {0} does not exist", darkID));
			return null;	
		}
	}


	private void RemoveFromNavTargets(int darkID)
	{

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
		updatedDarkness.Target.location = player;
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