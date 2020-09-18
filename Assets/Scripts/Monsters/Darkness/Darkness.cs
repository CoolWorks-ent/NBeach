using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;
using System;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */

namespace DarknessMinion
{
	public class Darkness : MonoBehaviour
	{

		public enum AggresionRating { Attacking = 1, Idling, Wandering }
		public enum NavTargetTag { Attack, Patrol, Neutral, Null, AttackStandby}
		[HideInInspector]
		public AggresionRating agRatingCurrent, agRatingPrevious;
		public Dark_State previousState, currentState;
		public NavigationTarget navTarget;
		public NavigationTarget[] PatrolPoints;

		[HideInInspector]
		public AIPath pather;

		[HideInInspector]
		public Seeker sekr;
		public Collider darkHitBox;

		public GameObject deathFX;
		public Dark_State DeathState;
		public Animator animeController;

		[HideInInspector]
		public int attackHash = Animator.StringToHash("Attack"),
					attackAfterHash = Animator.StringToHash("AfterAttack"),
					chaseHash = Animator.StringToHash("Chase"),
					idleHash = Animator.StringToHash("Idle"),
					deathHash = Animator.StringToHash("Death");
		public bool updateStates, attacked;
		public int creationID;
		public float playerDist, swtichDist, navTargetDist, stopDistance;

		private Dictionary<Dark_State.CooldownStatus, Dark_State.CooldownInfo> stateActionsOnCooldown;

		void Awake()
		{
			//attackInitiationRange = 2.5f;
			stopDistance = 3;
			swtichDist = 3; //attackInitiationRange*1.85f;
			creationID = 0;
			navTargetDist = -1;
			updateStates = true;
			agRatingCurrent = agRatingPrevious = AggresionRating.Idling;
			attacked = false;
			stateActionsOnCooldown = new Dictionary<Dark_State.CooldownStatus, Dark_State.CooldownInfo>();
		}

		void Start()
		{
			animeController = GetComponentInChildren<Animator>();
			pather = GetComponent<AIPath>();
			sekr = GetComponent<Seeker>();
			darkHitBox = GetComponent<CapsuleCollider>();
			Darkness_Manager.OnDarknessAdded(this);
			Darkness_Manager.UpdateDarkness += UpdateStates;
			//aIMovement = GetComponent<AI_Movement>();
			currentState.InitializeState(this);
			darkHitBox.enabled = false;
			pather.repathRate = 1;

			//aIMovement.target = Target;
		}

		/*public IEnumerator StateTransition(Dark_State nextState)
		{
			//if(nextState.stateType != currentState.stateType)
			if(nextState != currentState)
			{
				previousState = currentState;
				currentState = nextState;
				yield return new WaitForSeconds(previousState.transitionTime);
				previousState.ExitState(this);            
				currentState.InitializeState(this);
			}
			yield return null;
		}*/

		public void ChangeState(Dark_State nextState)
		{
			previousState = currentState;
			currentState = nextState;
			previousState.ExitState(this);
			currentState.InitializeState(this);
			/*if(nextState.stateType == Dark_State.StateType.DEATH)
			{
				previousState = currentState;
				currentState.ExitState(this);
				currentState = nextState;
				currentState.InitializeState(this);
			} 
			//else if (nextState.stateType == Dark_State.StateType.ATTACK && currentState.stateType == Dark_State.StateType.CHASING)
			else StartCoroutine(StateTransition(nextState));*/

		}

		void FixedUpdate()
		{
			if (currentState.stateType != Dark_State.StateType.REMAIN)
				currentState.MovementUpdate(this);
		}

		void Update()
		{
			if (stateActionsOnCooldown.Count > 0)
				UpdateCooldownTimers();
		}

		public void UpdateStates()
		{
			currentState.UpdateState(this);
		}

		public void DistanceEvaluation(Vector3 location)
		{
			playerDist = Vector3.Distance(transform.position, location);
			if (navTarget.navTargetTag != NavTargetTag.Neutral)
			{
				navTargetDist = Vector3.Distance(transform.position, navTarget.navPosition);
			}
			else navTargetDist = -1;
		}

		public void AggressionChanged(AggresionRating agR)
		{
			if (agR != agRatingCurrent)
				agRatingPrevious = agRatingCurrent;
			agRatingCurrent = agR;
		}

		public void PopulatePatrolPoints(int size)
		{
			PatrolPoints = new NavigationTarget[size];
			for (int i = 0; i < PatrolPoints.Length; i++)
			{
				float xOffset = 0;
				//PatrolPoints[i].position.parent = this.transform;
				if (i % 2 == 0 || i == 0)
					xOffset = transform.position.x - UnityEngine.Random.Range(5 + i, 15);
				else xOffset = transform.position.x + UnityEngine.Random.Range(5 + i, 15);
				Vector3 offset = new Vector3(xOffset, transform.position.y, transform.position.z - UnityEngine.Random.Range(9, 9 + i));
				PatrolPoints[i] = new NavigationTarget(transform.position, offset, Darkness_Manager.Instance.oceanPlane.position.y, NavTargetTag.Patrol);
				//PatrolPoints[i].targetID = i;
			}
		}


		public bool CheckActionsOnCooldown(Dark_State.CooldownStatus actType)
		{
			if (stateActionsOnCooldown.Count > 0)
				return stateActionsOnCooldown.ContainsKey(actType);
			return false;
		}

		public void AddCooldown(Dark_State.CooldownInfo actionCooldownInfo)
		{
			if (!stateActionsOnCooldown.ContainsKey(actionCooldownInfo.acType))
			{
				Debug.LogWarning("Adding cooldown for " + actionCooldownInfo.acType);
				stateActionsOnCooldown.Add(actionCooldownInfo.acType, actionCooldownInfo);
			}
		}

		private void UpdateCooldownTimers()
		{
			List<Dark_State.CooldownStatus> deletedEntries = new List<Dark_State.CooldownStatus>();
			foreach (KeyValuePair<Dark_State.CooldownStatus, Dark_State.CooldownInfo> info in stateActionsOnCooldown)
			{
				info.Value.UpdateTime(Time.deltaTime);
				if (info.Value.CheckTimerComplete())
				{
					Debug.LogWarning("Executing callback using: " + info.Value.Callback.ToString());
					deletedEntries.Add(info.Key);
				}
			}

			foreach (Dark_State.CooldownStatus cdStatus in deletedEntries)
			{
				stateActionsOnCooldown[cdStatus].Callback.Invoke(this);
				stateActionsOnCooldown.Remove(cdStatus);
			}
		}

		public void ResetCooldowns()
		{
			stateActionsOnCooldown.Clear();
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (collider.gameObject.CompareTag("Projectile"))
			{
				if (collider.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
				{
					Debug.LogWarning("Darkness Destroyed");
					ChangeState(DeathState);
				}
			}
			else if (collider.gameObject.CompareTag("Player"))
			{
				//Debug.LogWarning("Darkness collided with Player");
			}
		}



		///<summary>NavigationTarget is used by Darkness for pathfinding purposes. </summary>
		public struct NavigationTarget
		{
			private int weight;
			private int claimedID;
			private bool claimed;
			private float groundElavation;
			private Vector3 position;
			private Vector3 positionOffset;
			private readonly NavTargetTag targetTag;
			

			public bool navTargetClaimed {  get { return claimed; } }
			public int navTargetWeight { get { return weight; } }
			public NavTargetTag navTargetTag { get { return targetTag; } }
			public Vector3 navPosition { get { return position + positionOffset; } }

			///<param name="iD">Used in AI_Manager to keep track of the Attack points. Arbitrary for the Patrol points.</param>
			///<parem name="offset">Only used on targets that will be used for attacking. If non-attack point set to Vector3.Zero</param>
			public NavigationTarget(Vector3 loc, Vector3 offset, float elavation, NavTargetTag ntTag)//, bool act)
			{
				position = loc;
				groundElavation = elavation;
				//if(parent != null)
				//	transform.parent = parent;
				positionOffset = offset;
				targetTag = ntTag;
				weight = 0;
				claimed = false;
				claimedID = 0;
				//active = false;
				//assignedDarknessIDs = new int[assignmentLimit];
			}

			public void ClaimTarget(int cID)
            {
				claimedID = cID;
				claimed = true;
            }

			public void ReleaseTarget()
            {
				if (weight - 1 < 0)
					weight = 0;

				claimedID = -1;
				claimed = false;

            }

			public void UpdateLocation(Vector3 loc)
			{
				position = new Vector3(loc.x, groundElavation, loc.y);
			}
		}
	}
}