using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DarknessMinion
{
	public abstract class DarkState : ScriptableObject
	{
		//protected readonly Darkness darkController;
		public DarkTransition[] transitions;
		public List<DarkState> referencedBy;
		//protected Lookup<DarkTransition.TransitionPriority, DarkTransition> priorityTransitions;

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
	}
}