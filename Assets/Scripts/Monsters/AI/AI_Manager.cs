using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour {

	public Transform player;
	public Transform oceanPlane;
	public Dictionary<int, Darkness> ActiveDarkness;

	[SerializeField]
	private int darknessIDCounter, darknessConcurrentAttackLimit;
	public int maxEnemyCount, minEnemyCount, darkTotalCount, darkAttackCount, darkStandbyCount;
	public float calculationTime, attackOffset;
	
	private bool paused;

	[SerializeField]
	private Dark_State[] dark_States;
	private Darkness.NavigationTarget[] AttackPoints;
	private Darkness.NavigationTarget StartPoint, PlayerPoint;

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
		ActiveDarkness = new Dictionary<int, Darkness>();
		attackApprovalPriority = new List<int>();
		AddDarkness += AddtoDarknessList;
		RemoveDarkness += RemoveFromDarknessList;
		RequestNewTarget += ApproveDarknessTarget;
		paused = false;
		calculationTime = 0.5f;
		attackOffset = 3.5f;
		AttackPoints = new Darkness.NavigationTarget[4]; 
		StartCoroutine(ManagedDarknessUpdate());
		foreach(Dark_State d in dark_States)
        {
            d.Startup();
        }
	}

	void Start()
	{
		StartPoint = new Darkness.NavigationTarget(this.transform.position, Vector3.zero, oceanPlane.position.y, Darkness.NavTargetTag.Neutral);
		PlayerPoint = new Darkness.NavigationTarget(player.position, Vector3.zero, oceanPlane.position.y, Darkness.NavTargetTag.Attack);
		List<Vector3> offsets = new List<Vector3>();
		offsets.Add(new Vector3(attackOffset, 0, -2));
		offsets.Add(new Vector3(-attackOffset, 0, -2));
		offsets.Add(new Vector3(-attackOffset / 2, 0, 0));
		offsets.Add(new Vector3(attackOffset / 2, 0, 0));
		for (int i = 0; i < AttackPoints.Length; i++)
		{
			AttackPoints[i] = new Darkness.NavigationTarget(player.transform.position, offsets[i], oceanPlane.position.y, Darkness.NavTargetTag.Attack);
			//Vector3 t = AttackPoints[i].position+offsets[i];
			//Debug.LogWarning(string.Format("Attack point location AttackPoint[{0}]" + t, i));
		}

		//AttackPoints[0].position = new Vector3(player.position.x + attackOffset, player.position.y-0.5f, player.position.z);//right of player
		//AttackPoints[1].position = new Vector3(player.position.x - attackOffset, player.position.y-0.5f, player.position.z);//left of player
		//AttackPoints[2].position = new Vector3(player.position.x - attackOffset/2, player.position.y-0.5f, player.position.z);
		//AttackPoints[3].position = new Vector3(player.position.x + attackOffset/2, player.position.y-0.5f, player.position.z);
	}

#region DarknessUpdateLoop

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
					dark.Value.PlayerDistanceEvaluation(player.position);
				}
				SortTheGoons();
				yield return new WaitForSeconds(calculationTime/3);
				UpdateDarknessAggresion();
				yield return new WaitForSeconds(calculationTime);
				OnUpdateDarkness();
			}
			else yield return new WaitForSeconds(0.5f);
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

	///<summary>Sorts the Darkness in ActiveDarkness based on their distance to target values</summary>
	private void SortTheGoons() 
	{
		attackApprovalPriority.Sort(delegate(int a, int b)
		{
			return ActiveDarkness[a].playerDist.CompareTo(ActiveDarkness[b].playerDist);
		});
	}	
	#endregion

#region NavTargetHandling

	///<summary>Returns index of the attack Navigation Target with the lowest weight</summary>
	public int LeastRequestedNavigationTarget(Darkness.NavigationTarget[] navTargets) //TODO Create checking for if all targets are at capacity
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
		{
			int t = 0;
			for(int x = 0; x <= 5; x++)
			{	
				t = evenCount[Random.Range(0, evenCount.Count-1)];
				if(t == lowest)
					continue;
				else 
				{
					lowest = t;
					break;
				}

			}
			/*if(+1 < AttackPoints.Length)
			{
				patrol = PatrolPoints[darkness.Target.targetID+1];
			}
			else patrol = PatrolPoints[0];*/
		}
		return lowest;
	}

	///<summary>Returns an attack or patrol Navigation Target. Returns a null object if Darkness is not found in active list. </summary>
	private Darkness.NavigationTarget AssignAttackNavigationTarget(int darkID) 
	{
		//Find if Darkness is in the collection
		Darkness darkness;
		if(ActiveDarkness.TryGetValue(darkID, out darkness)) 
		{
			switch(darkness.agRatingCurrent)
			{
				case Darkness.AggresionRating.Attacking:
					int index = LeastRequestedNavigationTarget(AttackPoints);
					AttackPoints[index].weight++;
					return AttackPoints[index];
				/*case Darkness.AggresionRating.Wandering:
					NavigationTarget patrol = PatrolPoints[Random.Range(0, PatrolPoints.Length)]; 
					if(darkness.Target.navTargetTag == NavTargetTag.Patrol)
					{
						if(darkness.Target.targetID+1 < PatrolPoints.Length)
						{
							patrol = PatrolPoints[darkness.Target.targetID+1];
						}
						else patrol = PatrolPoints[0];
					}
					return patrol;*/
				default:
					return StartPoint;
			}
		}
		else 
		{
			Debug.LogError(string.Format("Darkness {0} does not exist", darkID));
			return new Darkness.NavigationTarget(Vector3.zero, Vector3.zero, 0, Darkness.NavTargetTag.Null);	
		}
	}

	///<summary>Check the Darkness for current NavTarget. If the target is an attack Target the target will be set to the starting NavTarget.</summary>
	private void RemoveFromNavTargets(int darkID)
	{
		Darkness darkness;
		if(ActiveDarkness.TryGetValue(darkID, out darkness))
		{
			if(darkness.Target.navTargetTag != Darkness.NavTargetTag.Neutral)
			{
				darkness.Target.weight--;
			}
		}
	}

	///<summary>Processes Darkness request for a  NavTarget. Assign a new target to the requestor Darkness if a valid request</summary> //--Work in Progress--
	public void ApproveDarknessTarget(int darkID) //TODO Darkness will make request for new Navigation Targets based on their status
	{
		Darkness darkness;
		if(ActiveDarkness.TryGetValue(darkID, out darkness))
		{
			if(darkness.agRatingCurrent == Darkness.AggresionRating.Attacking)
			{ 
				/*if(darkness.navTargetDist <= darkness.swtichDist+0.25f)
					darkness.Target = PlayerPoint;
				else
				{*/
				Darkness.NavigationTarget nT = AssignAttackNavigationTarget(darkness.creationID); 
				if(nT.navTargetTag != Darkness.NavTargetTag.Neutral)
				{
					RemoveFromNavTargets(darkID);
					darkness.Target = nT;
				}
				//}
			}
			/*else if(darkness.agRatingCurrent == Darkness.AggresionRating.Wandering)
			{
				NavigationTarget nT = AssignAttackNavigationTarget(darkness.creationID); 
					if(nT != null)
					{
						RemoveFromNavTargets(darkID);
						darkness.Target = nT;
					}
			}*/
			else if(darkness.agRatingCurrent == Darkness.AggresionRating.CatchingUp)
			{
				RemoveFromNavTargets(darkID);
				darkness.Target = PlayerPoint;
			}
			else //if(darkness.agRatingCurrent == Darkness.AggresionRating.Idling)
			{
				RemoveFromNavTargets(darkID);
				darkness.Target = StartPoint;
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

	public static event AIEvent<int> RequestNewTarget;
	public static event AIEvent UpdateDarkness;

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

	public static void OnRequestNewTarget(int ID)
	{
		if(RequestNewTarget != null)
			RequestNewTarget(ID);
	}

	public static void OnUpdateDarkness()
	{
		if(UpdateDarkness != null)
			UpdateDarkness();
	}

	#endregion
	
}