using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DarknessMinion
{

	public abstract class DarkState : ScriptableObject
	{
		public enum StateType { CHASING, IDLE, ATTACK, DEATH, REMAIN, WANDER }
		public enum CooldownStatus { Attacking, Patrolling, Idling, Moving }

		//public enum TargetType { DIRECT_PLAYER, FLANK_PLAYER, PATROL }
		public StateType stateType;
		public DarkTransition[] transitions;
		public List<DarkState> referencedBy;
		//protected Lookup<AI_Transition.Transition_Priority, AI_Transition> priorityTransitions;
		//[SerializeField, Range(0, 3)]
		//public float speedModifier;

		//[SerializeField, Range(0, 15)]
		//protected float stopDist;

		//[SerializeField, Range(0,360)]
		//protected int rotationSpeed;

		//[SerializeField, Range(0, 5)]
		//protected float pathUpdateRate;



		public virtual void Startup()
		{
			DarkEventManager.RemoveDarkness += RemoveDarkness;
			//if (referencedBy == null || referencedBy.Count < 1)
			referencedBy = new List<DarkState>();
			foreach (DarkTransition ai in transitions)
			{
				if (!ai.trueState.referencedBy.Contains(this))
					ai.trueState.referencedBy.Add(this);
				if (!ai.falseState.referencedBy.Contains(this))
					ai.falseState.referencedBy.Add(this);
			}
		}

		public abstract void InitializeState(Darkness controller);
		public abstract void UpdateState(Darkness controller);
		public abstract void ExitState(Darkness controller);
		public abstract void MovementUpdate(Darkness controller);
		protected abstract void CooldownCallback(Darkness controller);



		protected virtual void FirstTimeSetup()
		{
			stateType = StateType.REMAIN;
		}

		protected void CheckTransitions(Darkness controller)
		{
			foreach (DarkTransition darkTran in transitions)
			{
				bool decisionResult = darkTran.decision.MakeDecision(darkTran.decisionChoice, controller);
				if (decisionResult)
				{
					if (darkTran.trueState.stateType != StateType.REMAIN)
						controller.ChangeState(darkTran.trueState);
				}
				else 
				{
					if (darkTran.falseState.stateType != StateType.REMAIN) 
						controller.ChangeState(darkTran.falseState);
				}
			}
		}

		protected void RemoveDarkness(Darkness controller)
		{
			if (stateType != DarkState.StateType.DEATH)
			{
				this.ExitState(controller);
				controller.updateStates = false;
				//controller.ChangeState(controller.DeathState);
			}
		}

		public class CooldownInfo
		{
			public CooldownStatus acType { get; private set; }
			private float remainingTime;
			//public float coolDownTime;
			public Action<Darkness> Callback;
			//public Coroutine durationRoutine;

			public CooldownInfo(float cdTime, CooldownStatus acT, Action<Darkness> cback)
			{
				remainingTime = cdTime;
				//coolDownTime = cdTime;
				acType = acT;
				Callback = cback;
			}

			public void UpdateTime(float time)
			{
				remainingTime = Mathf.Max(remainingTime - time, 0);
			}

			public bool CheckTimerComplete()
			{
				if (remainingTime == 0)
					return true;
				else return false;
			}
		}
	}
}