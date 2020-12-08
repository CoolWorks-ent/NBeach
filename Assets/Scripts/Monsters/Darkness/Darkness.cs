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

		public enum DarkAnimationStates {Spawn, Idle, Chase, Attack, Death}

		[HideInInspector]
		public AggresionRating agRatingCurrent, agRatingPrevious;
		public NavigationTarget navTarget;

		//public NavigationTarget[] patrolPoints;

		[HideInInspector]
		public AIPath pather;

		[HideInInspector]
		public Seeker sekr;

		[HideInInspector]
		public Collider darkHitBox;

		[HideInInspector]
		public TextMesh textMesh;
		public bool updateStates, attacked;
		public float playerDist, swtichDist, navTargetDist;
		public LayerMask mask;
		public int creationID { get; private set;}

		[SerializeField] //Tooltip("Assign in Editor"),
		private DarkState deathState, spawnState, currentState;
		private DarkState previousState;

		private Animator animeController;

		private Dictionary<DarkState.CooldownStatus, DarkState.CooldownInfo> stateActionsOnCooldown;
		private List<GameObject> stateCache;
		private string debugMessage {get; set;}
		private int stateAnimID;

		void Awake()
		{
			swtichDist = 4.6f; 
			creationID = 0;
			navTargetDist = -1;
			updateStates = true;
			agRatingCurrent = agRatingPrevious = AggresionRating.Idling;
			attacked = false;
			stateActionsOnCooldown = new Dictionary<DarkState.CooldownStatus, DarkState.CooldownInfo>();
			stateCache = new List<GameObject>();
		}

		void Start()
		{
			textMesh = GetComponentInChildren<TextMesh>(true);
			animeController = GetComponentInChildren<Animator>();
			pather = GetComponent<AIPath>();
			sekr = GetComponent<Seeker>();
			darkHitBox = GetComponent<CapsuleCollider>();
			//aIMovement = GetComponent<AI_Movement>();
			currentState = spawnState;
			currentState.InitializeState(this);
			darkHitBox.enabled = false;
			pather.repathRate = 0.85f;
			mask = LayerMask.GetMask("Player");
			stateAnimID = Animator.StringToHash("StateID");
			DarkEventManager.OnDarknessAdded(this);
			DarkEventManager.UpdateDarknessStates += UpdateStates;
			DarkEventManager.UpdateDarknessDistance += DistanceEvaluation;
			//aIMovement.target = Target;
			//ChangeAnimation(DarkAnimationStates.Spawn);
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

		public void ChangeState(DarkState nextState)
		{
			previousState = currentState;
			currentState = nextState;
			previousState.ExitState(this);
			currentState.InitializeState(this);
		}

		public void Spawn(int createID, float spawnHeight)
		{
			creationID = createID;
			CreateDummyNavTarget(spawnHeight);
		}

		public void UpdateStates()
		{
			currentState.UpdateState(this);
		}

		public void ChangeAnimation(DarkAnimationStates anim)
		{
			//animeController.SetInteger(stateAnimID, playID);
			animeController.Play(anim.ToString());
		}

		public float CurrentAnimationLength()
		{
			return animeController.GetCurrentAnimatorStateInfo(0).length;
		}

		public bool IsAnimationPlaying(DarkAnimationStates anim)
		{
			Debug.LogWarning("Animator is playing: " + animeController.GetCurrentAnimatorClipInfo(0)[0].clip.name);
			return animeController.GetCurrentAnimatorStateInfo(0).IsName(anim.ToString());
		}

		#region Frequently Ran 
		public void DistanceEvaluation(Vector3 location)
		{
			playerDist = Vector3.Distance(transform.position, location);
			if (navTarget != null)
			{
				navTargetDist = Vector3.Distance(transform.position, navTarget.navPosition);
			}
			else navTargetDist = -1;
		}

		public void AggressionRatingUpdate(AggresionRating agR)
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
		#endregion

		public void AddCooldown(DarkState.CooldownInfo actionCooldownInfo)
		{
			if (!stateActionsOnCooldown.ContainsKey(actionCooldownInfo.acType))
			{
				//Debug.LogWarning("Adding cooldown for " + actionCooldownInfo.acType);
				stateActionsOnCooldown.Add(actionCooldownInfo.acType, actionCooldownInfo);
			}
		}

		public void ClearCooldowns()
		{
			stateActionsOnCooldown.Clear();
		}

		private void UpdateCooldownTimers()
		{
			List<DarkState.CooldownStatus> deletedEntries = new List<DarkState.CooldownStatus>();
			foreach (KeyValuePair<DarkState.CooldownStatus, DarkState.CooldownInfo> info in stateActionsOnCooldown)
			{
				if(info.Value.UpdateTime(Time.deltaTime))
					deletedEntries.Add(info.Key);
			}

			foreach (DarkState.CooldownStatus cdStatus in deletedEntries)
			{
				stateActionsOnCooldown[cdStatus].Callback.Invoke(this);
				stateActionsOnCooldown.Remove(cdStatus);
			}
		}

		public void AddToStateCache(GameObject obj)
		{
			stateCache.Add(obj);
		}

		public GameObject GetLastObjectFromCache()
		{
			return stateCache[stateCache.Count-1];
		}

		public void RemoveFromStateCache(GameObject obj)
		{
			if(stateCache.Contains(obj))
				stateCache.Remove(obj);
		}

		public void ClearStateCache()
		{
			stateCache.Clear();
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
		
		public void KillDarkness()
		{
			ChangeState(deathState);
		}

		public void CreateDummyNavTarget(float elavation)
		{
			Vector3 randloc = new Vector3(UnityEngine.Random.Range(-10,10) + transform.position.x, elavation, UnityEngine.Random.Range(-5,5));
			navTarget = new NavigationTarget(randloc, elavation, NavigationTarget.NavTargetTag.Neutral);
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
				navTarget.navTargetTag, navTarget.navPosition, currentState.ToString(), previousState.ToString(), playerDist, navTargetDist, this.transform.position);

				textMesh.text = debugMessage;
			}
		}

		public void ToggleDebugMessage(bool active)
		{
			textMesh.gameObject.SetActive(active);
		}

        private void OnDrawGizmos()
        {
			Debug.DrawRay(transform.position+new Vector3(0,1,0), transform.forward*2f, Color.red, 0.01f);
			/*if(navTarget != null)
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
			}*/
        }

	}
}