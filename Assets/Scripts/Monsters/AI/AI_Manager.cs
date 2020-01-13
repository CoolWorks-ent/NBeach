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
	private NavigationTarget StartPoint;

	public List<int> attackApprovalPriority; 
	//private Queue<Darkness> engagementQueue, approachQueue;
	private static AI_Manager instance;
	public static AI_Manager Instance
	{
		get {return instance; }
	}

	public enum NavTargetTag {Attack, Patrol, Neutral, Chase}
	public class NavigationTarget
	{
		public int targetID, 
					weight, 
					weightCap;
		public Transform location;

		private NavTargetTag targetTag;
		public NavTargetTag navTargetTag { get{ return targetTag; }}

		public NavigationTarget(Transform loc, int iD, NavTargetTag ntTag)//, int assignmentLimit)
		{
			location = loc;
			targetID = iD;
			targetTag = ntTag;
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
		StartCoroutine(ExecuteDarknessStates());
		foreach(Dark_State d in dark_States)
        {
            d.Startup();
        }
	}

	void Start()
	{
		StartPoint = new NavigationTarget(this.transform, 0, NavTargetTag.Neutral);
		for(int i = 0; i < AttackPoints.Length; i++)
		{
			AttackPoints[i] = new NavigationTarget(new GameObject("attackPoint" + i).transform, i, NavTargetTag.Attack);
			AttackPoints[i].location.parent = player;
			AttackPoints[i].targetID = i;
		}

		for(int i = 0; i < PatrolPoints.Length; i++)
		{
			PatrolPoints[i] = new NavigationTarget(new GameObject("patrolPoint" + i).transform, i, NavTargetTag.Patrol);
			float xOffset = 0;
			PatrolPoints[i].location.parent = player; //TODO make this locations unique and not trash
			if(i % 2 == 0 || i == 0)
				xOffset = player.position.x - Random.Range(5+i, 15);
			else xOffset = player.position.x + Random.Range(5+i, 15);
			PatrolPoints[i].location.position = new Vector3(xOffset, player.position.y, player.position.z - Random.Range(9, 9+i));
			PatrolPoints[i].targetID = i;
		}

		AttackPoints[0].location.position = new Vector3(player.position.x + attackOffset, player.position.y, player.position.z);//right of player
		AttackPoints[1].location.position = new Vector3(player.position.x - attackOffset, player.position.y, player.position.z);//left of player
		AttackPoints[2].location.position = new Vector3(player.position.x, player.position.y, player.position.z - attackOffset);//front of player
	}

	///<summary>Contols the update loop for Darkness objects. Calls Darkness sorting and Darkness approval functions </summary>
	private IEnumerator ManagedDarknessUpdate() 
	{
		while(!paused)
		{
			if(attackApprovalPriority.Count > 0)
			{
				//ActiveDarkness.Values.CopyTo(closestDarkness,0);
				foreach(KeyValuePair<int,Darkness> dark in ActiveDarkness)
				{
					dark.Value.DistanceEvaluation(player.position);
				}
				SortTheGoons();
				yield return new WaitForSeconds(calculationTime/3);
				UpdateDarknessAggresion();
				yield return new WaitForSeconds(calculationTime);

			}
			else yield return new WaitForSeconds(0.5f);
		}
		yield return null;
	}

	public IEnumerator ExecuteDarknessStates()
    {
        while(!paused)
        {
			foreach(KeyValuePair<int,Darkness> dark in ActiveDarkness)
			{
				dark.Value.currentState.UpdateState(dark.Value);
			}
            yield return new WaitForSeconds(calculationTime);
        }
        yield return null;
    }

	///<summary>Sets the closest Darkness to attack state. Darkness that are runners up are set to patrol nearby. Furtheset Darkness are set to idle priority</summary>
	private void UpdateDarknessAggresion() 
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

	///<summary>Processes Darkness request for a  NavTarget. Assign a new target to the requestor Darkness if a valid request</summary> //--Work in Progress--
	public void ApproveDarknessTarget(int ID, NavTargetTag type) //TODO Darkness will make request for new Navigation Targets based on their status
	{
		Darkness darkness;
		if(ActiveDarkness.TryGetValue(ID, out darkness))
		{
			if(type == NavTargetTag.Attack && darkness.Target.navTargetTag != NavTargetTag.Attack && darkness.agRatingCurrent == Darkness.AggresionRating.Attacking)
			{
				NavigationTarget nT = AssignNavigationTarget(darkness.creationID, true); 
				if(nT != null)
				{
					RemoveFromNavTargets(ID);
					darkness.Target = nT;
				}
			}
			if(darkness.Target.navTargetTag != NavTargetTag.Patrol && darkness.agRatingCurrent != Darkness.AggresionRating.Attacking)
			{
				NavigationTarget nT = AssignNavigationTarget(darkness.creationID, false);
				if(nT != null)
				{
					darkness.Target = nT;
				}
			}
		}
	}

	///<summary>Returns index of the attack Navigation Target with the lowest weight</summary>
	public int LeastRequestedNavigationTarget(NavigationTarget[] navTargets) //TODO Create checking for if all targets are at capacity
	{
		int lowest = 0;
		List<int> evenCount = new List<int>(); //In case there are entries at the same levels
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

	///<summary>Returns an attack or patrol Navigation Target. Returns a null object if Darkness is not found in active list. </summary>
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
			else return PatrolPoints[Random.Range(0, PatrolPoints.Length)];
		}
		else 
		{
			Debug.LogError(string.Format("Darkness {0} does not exist", darkID));
			return null;	
		}
	}

	///<summary>Check the Darkness for current NavTarget. If the target is an attack Target the target will be set to the starting NavTarget.</summary>
	private void RemoveFromNavTargets(int darkID)
	{
		Darkness darkness;
		if(ActiveDarkness.TryGetValue(darkID, out darkness))
		{
			if(darkness.Target.navTargetTag != NavTargetTag.Neutral)
			{
				darkness.Target.weight--;
				darkness.Target = StartPoint;
			}
		}
	}

	///<summary>Sorts the Darkness in ActiveDarkness based on their distance to target values</summary>
	private void SortTheGoons() 
	{
		attackApprovalPriority.Sort(delegate(int a, int b)
		{
			return ActiveDarkness[a].targetDist.CompareTo(ActiveDarkness[b].targetDist);
		});
	}	

	public IEnumerator WaitTimer(float timer)
	{
		yield return new WaitForSeconds(timer);
	}

#region AIEventHandlers
	///<summary> Notified by the AddDarkness event. Initializes Darkness parameters and adds to ActiveDakness </summary>
	private void AddtoDarknessList(Darkness updatedDarkness)
	{
		updatedDarkness.transform.SetParent(Instance.transform);
		darknessIDCounter++;
		updatedDarkness.creationID = darknessIDCounter;
		updatedDarkness.Target = StartPoint;

		ActiveDarkness.Add(updatedDarkness.creationID, updatedDarkness);
		attackApprovalPriority.Add(updatedDarkness.creationID);
		//updatedDarkness.StartCoroutine(updatedDarkness.ExecuteCurrentState());
	}

	///<summary>Removes Darkness from attack list if present. Also removes Darkness from active list and stops any relevant running funcitons</summary>
    public void RemoveFromDarknessList(Darkness updatedDarkness)
    {
		//updatedDarkness.StopCoroutine(updatedDarkness.ExecuteCurrentState());
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
	#endregion

#region AIManagerEvents
	public delegate void AIEvent();
	public delegate void AIEvent<T>(T obj);
	public static event AIEvent<Darkness> AddDarkness;
	public static event AIEvent<Darkness> RemoveDarkness;
	public static event AIEvent UpdateMovement;


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

	/*public static void OnMovementUpdate()
	{
		if(UpdateMovement != null)
			UpdateMovement();
	}*/

	#endregion
	
}