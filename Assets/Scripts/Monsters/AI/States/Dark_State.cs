using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Dark_State : ScriptableObject
{
	public enum StateType { CHASING, IDLE, ATTACK, DEATH, PAUSE, REMAIN, WANDER }
	public enum CooldownStatus { Attacking, Patrolling, Idling, Moving }

	public enum TargetType { DIRECT_PLAYER, FLANK_PLAYER, PATROL}
	public StateType stateType;
	public Dark_Transition[] transitions;
	public List<Dark_State> ReferencedBy;
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
		Darkness_Manager.RemoveDarkness += RemoveDarkness;
		if(ReferencedBy == null || ReferencedBy.Count < 1)
			ReferencedBy = new List<Dark_State>();
		foreach(Dark_Transition ai in transitions)
		{
			if(!ai.trueState.ReferencedBy.Contains(this))
				ai.trueState.ReferencedBy.Add(this);
			if(!ai.falseState.ReferencedBy.Contains(this))
				ai.falseState.ReferencedBy.Add(this);
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
		for(int i = 0; i < transitions.Length; i++)
		{
			bool decisionResult = transitions[i].decision.MakeDecision(transitions[i].decisionChoice,controller);
			if(decisionResult) 
			{
				if(transitions[i].trueState.stateType == StateType.REMAIN)
					continue;
				else ProcessStateChange(transitions[i].trueState, controller); 
			}
			else if(!decisionResult) 
			{
				if(transitions[i].falseState.stateType == StateType.REMAIN)
					continue;
				else ProcessStateChange(transitions[i].falseState, controller);
			}
		}   
	}

	protected void ProcessStateChange(Dark_State approvedState, Darkness controller) //TODO Have Darkness start a coroutine to begin transitioning. 
	{
		controller.ChangeState(approvedState);
	}

	protected void RemoveDarkness(Darkness controller)
	{
		if(stateType != Dark_State.StateType.DEATH)
		{
			this.ExitState(controller);
			controller.updateStates = false;
			//controller.ChangeState(controller.DeathState);
		}
	}

	public class CooldownInfo
	{
		public CooldownStatus acType { get; private set; }
		public float remainingTime;
		public float coolDownTime;
		public Action<Darkness> Callback;
		//public Coroutine durationRoutine;

		public CooldownInfo(float cdTime, CooldownStatus acT, Action<Darkness> cback)
		{
			remainingTime = cdTime;
			coolDownTime = cdTime;
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