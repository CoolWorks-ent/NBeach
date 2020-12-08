using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DarknessMinion
{
	public abstract class DarkState : ScriptableObject
	{
		//protected readonly Darkness darkController;
		public enum CooldownStatus { Attacking, Patrolling, Idling, Moving, Spawn}
		public DarkTransition[] transitions;
		public List<DarkState> referencedBy;
		protected Lookup<DarkTransition.TransitionPriority, DarkTransition> priorityTransitions;

		public void SortTransitionsByPriority()
		{
			//DarkEventManager.RemoveDarkness += RemoveDarkness;
			if(transitions != null || transitions.Count() > 0)
				Array.Sort(transitions, ((t, p) => {return t.priorityLevel.CompareTo(p.priorityLevel);}));
		}

		public void UpdateReferences()
		{
			referencedBy = new List<DarkState>();
			foreach (DarkTransition t in transitions)
			{
				if(t.trueState != null)
				{
					//if (!t.trueState.referencedBy.Contains(this))
					t.trueState.referencedBy.Add(this);
				}
			}
		}

		public abstract void InitializeState(Darkness darkController);
		public abstract void UpdateState(Darkness darkController);
		public virtual void ExitState(Darkness darkController)
		{
			darkController.ClearCooldowns();
		}
		public virtual void MovementUpdate(Darkness darkController){}
		protected abstract void CooldownCallback(Darkness darkController);
		protected void CheckTransitions(Darkness darkController)
		{
			foreach (DarkTransition darkTran in transitions)
			{
				bool decisionResult = darkTran.decision.MakeDecision(darkTran.decisionChoice, darkController);
				if (decisionResult)
				{
					darkController.ChangeState(darkTran.trueState);
				}
			}
		}

		/*protected void RemoveDarkness()
		{
			this.ExitState(); //fire this if not in the Death state already
			darkController.updateStates = false;
		}*/

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

			public bool UpdateTime(float time)
			{
				remainingTime = Mathf.Max(remainingTime - time, 0);
				if(remainingTime == 0)
					return false;
				else return true;
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