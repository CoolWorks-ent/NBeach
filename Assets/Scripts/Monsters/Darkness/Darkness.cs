using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */

namespace DarknessMinion
{
	[RequireComponent(typeof(DarknessMovement))]
	public class Darkness : MonoBehaviour
	{
		public enum AggresionRating { Attacking = 1, Idling, Wandering }

		public enum DarkAnimationStates {Spawn, Idle, Chase, Attack, Death}

		[HideInInspector]
		public AggresionRating agRatingCurrent, agRatingPrevious;
		public Transform player;

		[HideInInspector]
		public Collider darkHitBox;

		[HideInInspector]
		public TextMesh textMesh;
		public LayerMask playerMask;
		[HideInInspector]
		public RaycastHit rayHitInfo;
		public int creationID { get; private set; }
		public float attackSwitchRange { get { return attackRange; } }

		[SerializeField, Range(0, 5)]
		private float attackRange;

		public DarknessMovement movement;

		[SerializeField] //Tooltip("Assign in Editor")
		private DarkState deathState, currentState;
		private DarkState previousState;

		private Animator animeController;

		private Dictionary<CooldownInfo.CooldownStatus, CooldownInfo> stateActionsOnCooldown;
		private string debugMessage {get; set;}

		private int animTriggerAttack, animTriggerIdle, animTriggerChase, animTriggerDeath;

		void Awake()
		{
			creationID = 0;
			agRatingCurrent = agRatingPrevious = AggresionRating.Idling;
			stateActionsOnCooldown = new Dictionary<CooldownInfo.CooldownStatus, CooldownInfo>();
		}

		void OnEnable() { DarkEventManager.UpdateDarknessStates += UpdateStates; }
		void OnDisable() { DarkEventManager.UpdateDarknessStates -= UpdateStates; }

		void Start()
		{
			movement = GetComponent<DarknessMovement>();
			movement.darkness = this;
			textMesh = GetComponentInChildren<TextMesh>(true);
			animeController = GetComponentInChildren<Animator>();
			animTriggerAttack =	Animator.StringToHash("Attack");
			animTriggerChase = Animator.StringToHash("Chase");
			animTriggerIdle = Animator.StringToHash("Idle");
			animTriggerDeath = Animator.StringToHash("Death");
			darkHitBox = GetComponent<CapsuleCollider>();
			previousState  = currentState;
			darkHitBox.enabled = false;
			DarkEventManager.OnDarknessAdded(this);
			currentState.InitializeState(this);
			movement.player = player;
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

		public void Spawn(int createID)
		{
			creationID = createID;
		}

		private void UpdateStates()
		{
			currentState.UpdateState(this);
		}

		public void ChangeAnimation(DarkAnimationStates anim)
		{
			//animeController.SetInteger(stateAnimID, playID);
			switch (anim)
			{
				case DarkAnimationStates.Attack:
					animeController.SetTrigger(animTriggerAttack);
					return;
				case DarkAnimationStates.Chase:
					animeController.SetTrigger(animTriggerChase);
					return;
				case DarkAnimationStates.Idle:
					animeController.SetTrigger(animTriggerIdle);
					return;
				case DarkAnimationStates.Death:
					animeController.SetTrigger(animTriggerDeath);
					return;
			}
		}

		public float CurrentAnimationLength()
		{
			//Debug.Log("Animation length: " + animeController.GetCurrentAnimatorStateInfo(0).length);
			return animeController.GetCurrentAnimatorStateInfo(0).length;
		}

		public bool IsAnimationPlaying(DarkAnimationStates anim)
		{
			//Debug.LogWarning("Animator is playing: " + animeController.GetCurrentAnimatorClipInfo(0)[0].clip.name);
			return animeController.GetCurrentAnimatorStateInfo(0).IsName(anim.ToString());
		}

		#region Frequently Ran 
		public void AggressionRatingUpdate(AggresionRating agR)
		{
			if (agR != agRatingCurrent)
				agRatingPrevious = agRatingCurrent;
			agRatingCurrent = agR;
		}

		public bool CheckActionsOnCooldown(CooldownInfo.CooldownStatus actType)
		{
			if (stateActionsOnCooldown.Count > 0)
				return stateActionsOnCooldown.ContainsKey(actType);
			return false;
		}
		#endregion

		public void AddCooldown(CooldownInfo actionCooldownInfo)
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

		public void SetAttackDistance(float atkValue)
		{
			attackRange = atkValue;
		}

		public float PlayerDistance()
		{
			if(movement)
				return movement.playerDist;
			return -1;
		}

		private void UpdateCooldownTimers()
		{
			List<CooldownInfo.CooldownStatus> deletedEntries = new List<CooldownInfo.CooldownStatus>();
			foreach (KeyValuePair<CooldownInfo.CooldownStatus, CooldownInfo> info in stateActionsOnCooldown)
			{
				if(!info.Value.TimeRemaining(Time.deltaTime))
					deletedEntries.Add(info.Key);
			}

			foreach (CooldownInfo.CooldownStatus cdStatus in deletedEntries)
			{
				CooldownInfo deletedInfo; // = stateActionsOnCooldown[cdStatus];
				if(stateActionsOnCooldown.TryGetValue(cdStatus, out deletedInfo))
                {
					stateActionsOnCooldown.Remove(cdStatus);
					deletedInfo.Callback.Invoke(this);
				}
			}
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
		}
		
		public void KillDarkness()
		{
			ChangeState(deathState);
		}


		public void UpdateDebugMessage(bool showAggresionRating, bool showStateInfo, bool showLocationInfo, bool showCooldownInfo)
		{
			debugMessage = "";
			if(showAggresionRating)
				debugMessage += String.Format("\n <b>Aggression Rating:</b> {0}", agRatingCurrent.ToString());
			if(showStateInfo)
				debugMessage += String.Format("\n <b>Current State:</b> {0} \n" + "Previous State: {1} \n", currentState.ToString(), previousState.ToString());
			if(showLocationInfo)	
				debugMessage += String.Format("\n <b>Player Distance:</b> {0} \n" + "<b>Darkness Position:</b> {1}", movement.playerDist, this.transform.position);
			if(showCooldownInfo)
				debugMessage += String.Format("\n<b>Active Cooldown Count: {0}</b>", stateActionsOnCooldown.Count());
			textMesh.text = debugMessage;
		}

		public void ToggleDebugMessage(bool active)
		{
			textMesh.gameObject.SetActive(active);
		}

	#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
			Debug.DrawRay(transform.position+new Vector3(0,1,0), transform.forward*2f, new Color(1f, 0.92f, 0.08f), 0.01f);
        }
	#endif
	}
}