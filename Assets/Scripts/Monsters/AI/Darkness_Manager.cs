using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darkness_Manager : MonoBehaviour {

	public Transform player;
	public Dictionary<int, Darkness> ActiveDarkness;

	[SerializeField]
	private int darknessIDCounter, darknessConcurrentAttackLimit;
	public int maxEnemyCount, minEnemyCount, darkTotalCount, darkAttackCount, darkStandbyCount;
	public float calculationTime, attackOffset, ground;
	private bool updatePaused, gamePaused;

	[SerializeField]
	private Dark_State[] dark_States;
	//private NavigationTarget[] PatrolPoints;
	private NavigationTarget[] AttackPoints;
	private NavigationTarget StartPoint, PlayerPoint;

	public List<int> attackApprovalPriority; 
	//private Queue<Darkness> engagementQueue, approachQueue;
	private static Darkness_Manager instance;
	public static Darkness_Manager Instance
	{
		get {return instance; }
	}

	public enum NavTargetTag {Attack, Patrol}

	///<summary>NavigationTarget is used by Darkness for pathfinding purposes. </summary>
	public struct NavigationTarget
	{
		public int targetID, weight;
		public bool active;
		private float groundElavation;

		public Vector3 position;
		private Vector3 positionOffset;
		//public Transform locationInfo { 
		//	get {return transform; }}

		private NavTargetTag targetTag;
		public NavTargetTag navTargetTag { get{ return targetTag; }}

		///<param name="iD">Used in AI_Manager to keep track of the Attack points. Arbitrary for the Patrol points.</param>
		///<parem name="offset">Only used on targets that will be used for attacking. If non-attack point set to Vector3.Zero</param>
		public NavigationTarget(Vector3 loc, Vector3 offset, float elavation, int iD, NavTargetTag ntTag)//, bool act)
		{
			position = loc;
			groundElavation = elavation;
			//if(parent != null)
			//	transform.parent = parent;
			positionOffset = offset;
			targetID = iD;
			targetTag = ntTag;
			weight = 0;
			active = false;
			//assignedDarknessIDs = new int[assignmentLimit];
		}

		public void UpdateLocation(Vector3 loc, bool applyOffset)
		{
			if(!applyOffset)
				position = new Vector3(loc.x, groundElavation, loc.y);
			else position = new Vector3(loc.x, groundElavation, loc.y) + positionOffset;
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
		RequestNewTarget += ApproveDarknessTarget;
		updatePaused = true;
		gamePaused = false;
		calculationTime = 0.25f;
		attackOffset = 3.5f;
		//PatrolPoints = new NavigationTarget[4]; 
		AttackPoints = new NavigationTarget[4]; 
		//StartCoroutine(ExecuteDarknessStates());
		foreach(Dark_State d in dark_States)
        {
            d.Startup();
        }
	}

	void Start()
	{
		//StartPoint = new NavigationTarget(this.transform, 0, NavTargetTag.Neutral);
		ground = GameObject.FindGameObjectWithTag("Water").transform.position.y;
		player = GameObject.FindGameObjectWithTag("Player").transform;
		PlayerPoint = new NavigationTarget(player.transform.position, Vector3.zero, ground, 0, NavTargetTag.Attack);
		List<Vector3> offsets = new List<Vector3>();
		offsets.Add(new Vector3(attackOffset, 0, 0));
		offsets.Add(new Vector3(-attackOffset, 0, 0));
		offsets.Add(new Vector3(-attackOffset/2, 0, 0));
		offsets.Add(new Vector3(attackOffset/2, 0, 0));
		for(int i = 0; i < AttackPoints.Length; i++)
		{
			AttackPoints[i] = new NavigationTarget(PlayerPoint.position, offsets[i], ground, i, NavTargetTag.Attack);
		}
		
		/*for(int i = 0; i < PatrolPoints.Length; i++)
		{
			PatrolPoints[i] = new NavigationTarget(new GameObject("patrolPoint" + i).transform, i, NavTargetTag.Patrol);
			float xOffset = 0;
			PatrolPoints[i].location.parent = player; 
			if(i % 2 == 0 || i == 0)
				xOffset = player.position.x - Random.Range(5+i, 15);
			else xOffset = player.position.x + Random.Range(5+i, 15);
			PatrolPoints[i].location.position = new Vector3(xOffset, player.position.y, player.position.z - Random.Range(9, 9+i));
			PatrolPoints[i].targetID = i;
		}*/
	}

	void LateUpdate()
	{
		foreach(NavigationTarget n in AttackPoints) //update the location of the attack points
		{
			n.UpdateLocation(player.position, true);
		}
	}

#region DarknessUpdateLoop

	///<summary>Controls the update loop for Darkness objects. Calls Darkness sorting and Darkness approval functions </summary>
	private IEnumerator ManagedDarknessUpdate() 
	{
		Debug.LogWarning("[Darkness_Manager] Started ManagedUpdate");
		while(!updatePaused)
		{
			if(ActiveDarkness.Count > 0)
			{
				Debug.LogWarning("[Darkness_Manager] Executing ManagedUpdate");
				/*foreach(KeyValuePair<int,Darkness> dark in ActiveDarkness)
				{
					dark.Value.UpdateDistanceEvaluation(player.position);
				}*/

				OnDistanceUpdate(player.position);

				UpdateDarknessAggresionStatus();
				//yield return new WaitForSeconds(calculationTime);
				OnUpdateDarkStates();
				//SortTheGoons();

				/*foreach(KeyValuePair<int,Darkness> dark in ActiveDarkness)
				{
					dark.Value.currentState.UpdateState(dark.Value);
				}*/
				yield return new WaitForSeconds(calculationTime);
			} else yield return new WaitForSeconds(calculationTime*4);
		}
		yield return null;
	}

	///<summary>Sets the closest Darkness to attack state. Darkness that are runners up are set to patrol nearby. Furtheset Darkness are set to idle priority</summary>
	private void UpdateDarknessAggresionStatus() 
	{
		attackApprovalPriority.Sort(delegate(int a, int b)
		{
			return ActiveDarkness[a].playerDist.CompareTo(ActiveDarkness[b].playerDist);
		});

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

	/*///<summary>Sorts the Darkness in ActiveDarkness based on their distance to target values</summary>
	private void SortTheGoons() 
	{
		attackApprovalPriority.Sort(delegate(int a, int b)
		{
			return ActiveDarkness[a].playerDist.CompareTo(ActiveDarkness[b].playerDist);
		});
	}	*/
	#endregion

#region NavTargetHandling

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
		}
		return lowest;
	}

	///<summary>Returns an attack Navigation Target. Returns the StartPoint object if Darkness is not found or if they should not be attacking. </summary>
	private NavigationTarget AssignAttackNavTarget(int darkID) 
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
					if(darkness.navTarget.navTargetTag == NavTargetTag.Patrol)
					{
						if(darkness.navTarget.targetID+1 < PatrolPoints.Length)
						{
							patrol = PatrolPoints[darkness.navTarget.targetID+1];
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
			return StartPoint;
		}
	}

	///<summary></summary>
	private void DeactivateNavTarget(int darkID, bool aggresive)
	{
		Darkness darkness;
		if(ActiveDarkness.TryGetValue(darkID, out darkness))
		{
			if(!aggresive)
			{
				darkness.attackNavTarget.weight--;
				darkness.attackNavTarget.active = false;
			}
			else
			{
				darkness.patrolNavTarget.active = false;
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
				if(darkness.targetDistance <= darkness.swtichDist+0.25f)
					darkness.attackNavTarget = PlayerPoint;
				else
				{
					NavigationTarget nT = AssignAttackNavTarget(darkness.creationID); 
					if(nT.active)
					{
						darkness.attackNavTarget = nT;
						DeactivateNavTarget(darkID, true);
					}
				}
			}
			else if(darkness.agRatingCurrent == Darkness.AggresionRating.CatchingUp)
			{
				DeactivateNavTarget(darkID, true);
				darkness.attackNavTarget = PlayerPoint;
			}
			else //if(darkness.agRatingCurrent == Darkness.AggresionRating.Idling)
			{
				DeactivateNavTarget(darkID, false);
				darkness.attackNavTarget = StartPoint;
			}
		}
	}
	#endregion

	public IEnumerator WaitTimer(float timer)
	{
		yield return new WaitForSeconds(timer);
	}

#region DarknessCollectionUpdates
	///<summary> Notified by the AddDarkness event. Initializes Darkness parameters and adds to ActiveDakness </summary>
	private void AddtoDarknessList(Darkness updatedDarkness)
	{
		updatedDarkness.transform.SetParent(Instance.transform);
		darknessIDCounter++;
		updatedDarkness.creationID = darknessIDCounter;
		updatedDarkness.attackNavTarget = StartPoint;

		ActiveDarkness.Add(updatedDarkness.creationID, updatedDarkness);
		attackApprovalPriority.Add(updatedDarkness.creationID);
		if(updatePaused && !gamePaused)
		{
			updatePaused = false;
			StartCoroutine(ManagedDarknessUpdate());
		}
			
		//updatedDarkness.StartCoroutine(updatedDarkness.ExecuteCurrentState());
	}

	///<summary>Removes Darkness from attack list if present. Also removes Darkness from active list and stops any relevant running funcitons</summary>
    public void RemoveFromDarknessList(Darkness updatedDarkness)
    {
		//updatedDarkness.StopCoroutine(updatedDarkness.ExecuteCurrentState());

		attackApprovalPriority.Remove(updatedDarkness.creationID);
        ActiveDarkness.Remove(updatedDarkness.creationID);
		if(ActiveDarkness.Count == 0)
		{
			updatePaused = true;
		}
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

	public static event AIEvent UpdateDarkStates;
	public static event AIEvent<Vector3> DistanceUpdate;
	public static event AIEvent<Darkness> AddDarkness;
	public static event AIEvent<Darkness> RemoveDarkness;
	public static event AIEvent<int> RequestNewTarget;

	///<summary>Executes logic update for Darkness</summary>
	public static void OnUpdateDarkStates() //Called by Darkness_Manager. Subsribed by Darkness. Darkness will run the update for their current state.
	{
		if(UpdateDarkStates != null)
			UpdateDarkStates();
	}

	///<summary>Executes distance update for Darkness</summary>
	public static void OnDistanceUpdate(Vector3 pos) //Called by Darkness_Manager. Subsribed by Darkness
	{
		if(DistanceUpdate != null)
			DistanceUpdate(pos);
	}

	///<summary>Adds Darkness to tracking list. Only call this for after initializing Darkness variables.</summary> //Called by Darkness. Subscribed by Darkness_Manager
	public static void OnDarknessAdded(Darkness d) //Called by Darkness. Subscribed by Darkness_Manager
	{
		if(AddDarkness != null)
			AddDarkness(d);
	}

	///<summary>Removes Darkness from tracking list.</summary>
	public static void OnDarknessRemoved(Darkness d) //Called by Darkness. Subscribed by Darkness_Manager
	{
		if(RemoveDarkness != null)
			RemoveDarkness(d);
	}

	///<summary>Request a new navigation target be assigned to the identified Darkness.</summary>
	public static void OnRequestNewTarget(int ID) //Called by Dark_States. Subscribed by Darkness_Manager
	{
		if(RequestNewTarget != null)
			RequestNewTarget(ID);
	}
	#endregion
	
}