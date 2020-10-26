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
	public class Darkness : MonoBehaviour, IDarkDebug
	{

		public enum AggresionRating { Attacking = 1, Idling, Wandering }
		[HideInInspector]
		public AggresionRating agRatingCurrent, agRatingPrevious;
		public DarkState previousState, currentState;
		public NavigationTarget navTarget;
		public NavigationTarget[] patrolPoints;

		[HideInInspector]
		public AIPath pather;

		[HideInInspector]
		public Seeker sekr;
		public Collider darkHitBox;

		[HideInInspector]
		public string debugMessage {get; set;}

		public TextMesh textMesh;

		public GameObject deathFX;
		public DarkState deathState;
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

		private Dictionary<DarkState.CooldownStatus, DarkState.CooldownInfo> stateActionsOnCooldown;

		void Awake()
		{
			//attackInitiationRange = 2.5f;
			stopDistance = 3;
			swtichDist = 4.6f; //attackInitiationRange*1.85f;
			creationID = 0;
			navTargetDist = -1;
			updateStates = true;
			agRatingCurrent = agRatingPrevious = AggresionRating.Idling;
			attacked = false;
			stateActionsOnCooldown = new Dictionary<DarkState.CooldownStatus, DarkState.CooldownInfo>();
		}

		void Start()
		{
			textMesh = GetComponentInChildren<TextMesh>(true);
			animeController = GetComponentInChildren<Animator>();
			pather = GetComponent<AIPath>();
			sekr = GetComponent<Seeker>();
			darkHitBox = GetComponent<CapsuleCollider>();
			DarkEventManager.OnDarknessAdded(this);
			DarkEventManager.UpdateDarkness += UpdateStates;
			//aIMovement = GetComponent<AI_Movement>();
			currentState.InitializeState(this);
			darkHitBox.enabled = false;
			pather.repathRate = 0.85f;

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

		public void ChangeState(DarkState nextState)
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
			if (currentState != null)
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
			if (navTarget != null)
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


		public bool CheckActionsOnCooldown(DarkState.CooldownStatus actType)
		{
			if (stateActionsOnCooldown.Count > 0)
				return stateActionsOnCooldown.ContainsKey(actType);
			return false;
		}

		public void AddCooldown(DarkState.CooldownInfo actionCooldownInfo)
		{
			if (!stateActionsOnCooldown.ContainsKey(actionCooldownInfo.acType))
			{
				//Debug.LogWarning("Adding cooldown for " + actionCooldownInfo.acType);
				stateActionsOnCooldown.Add(actionCooldownInfo.acType, actionCooldownInfo);
			}
		}

		private void UpdateCooldownTimers()
		{
			List<DarkState.CooldownStatus> deletedEntries = new List<DarkState.CooldownStatus>();
			foreach (KeyValuePair<DarkState.CooldownStatus, DarkState.CooldownInfo> info in stateActionsOnCooldown)
			{
				info.Value.UpdateTime(Time.deltaTime);
				if (info.Value.CheckTimerComplete())
				{
					Debug.LogWarning("Executing callback using: " + info.Value.acType);
					deletedEntries.Add(info.Key);
				}
			}

			foreach (DarkState.CooldownStatus cdStatus in deletedEntries)
			{
				stateActionsOnCooldown[cdStatus].Callback.Invoke(this);
				stateActionsOnCooldown.Remove(cdStatus);
			}
		}

		public void ResetCooldowns()
		{
			stateActionsOnCooldown.Clear();
		}

		private void OnTriggerEnter(Collider col)
		{
			if (col.gameObject.CompareTag("Projectile"))
			{
				if (col.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
				{
					Debug.LogWarning("Darkness Destroyed");
					ChangeState(deathState);
				}
			}
			else if (col.gameObject.CompareTag("Player"))
			{
				//Debug.LogWarning("Darkness collided with Player");
			}
		}

		public void CreateDummyNavTarget(float elavation)
		{
			Vector3 randloc = new Vector3(UnityEngine.Random.Range(-10,10) + transform.position.x, elavation, UnityEngine.Random.Range(-5,5));
			navTarget = new NavigationTarget(randloc, Vector3.zero, elavation, NavigationTarget.NavTargetTag.Neutral);
		}

		public void UpdateDebugMessage()
		{
			if(navTarget != null)
			{
				debugMessage = String.Format(
				"<b>NavTarget:</b> Tag = {0} Position = {1} \n" +
				"<b>Current State:</b> {2} \n" +
				"Previous State: {3} \n" +
				"<b>Player Distance:</b> {4} \n" +
				"<b>NavTarget Distance:</b> {5} \n" +
				"<b>Darkness Position:</b> {6}",
				navTarget.navTargetTag, navTarget.navPosition, currentState.name, previousState.name, playerDist, navTargetDist, this.transform.position);

				textMesh.text = debugMessage;
			}
		}

		public void ToggleDebugMessage(bool active)
		{
			textMesh.gameObject.SetActive(active);
		}

        private void OnDrawGizmos()
        {
			if(navTarget != null)
            {
				switch (navTarget.navTargetTag)
				{
					case NavigationTarget.NavTargetTag.Attack:
						Gizmos.color = Color.red; //new Color(1f, 0.43f, 0.24f);
						Gizmos.DrawCube(navTarget.navPosition, Vector3.one * 2);

						Gizmos.color = Color.white;
						Gizmos.DrawSphere(navTarget.srcPosition, 2);
						break;
					case NavigationTarget.NavTargetTag.AttackStandby:
						Gizmos.color = Color.yellow;
						Gizmos.DrawCube(navTarget.navPosition, Vector3.one * 1.5f);
						break;
					case NavigationTarget.NavTargetTag.Neutral:
						Gizmos.color = Color.white;
						Gizmos.DrawCube(navTarget.navPosition, Vector3.one * 1f);
						break;
					case NavigationTarget.NavTargetTag.Patrol:
						Gizmos.color = Color.cyan;
						Gizmos.DrawCube(navTarget.navPosition, Vector3.one * 1f);
						break;
				}
			}
        }

	}
}