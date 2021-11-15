using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DarknessMinion
{
	public abstract class DarkState : ScriptableObject
	{
		public Transition[] transitions;
		public List<DarkState> referencedBy;

		public void SortTransitionsByPriority()
		{
			if(transitions != null || transitions.Count() > 0)
				Array.Sort(transitions, (t, p) => t.priorityLevel.CompareTo(p.priorityLevel));
		}

		public void UpdateReferences()
		{
			referencedBy = new List<DarkState>();
			foreach (Transition t in transitions)
			{
				if(t.trueState != null)
				{
					//if (!t.trueState.referencedBy.Contains(this))
					t.trueState.referencedBy.Add(this);
				}
			}
		}

		public abstract void InitializeState(Darkness darkController);
		public virtual void UpdateState(Darkness darkController){ }
		public virtual void ExitState(Darkness darkController)
		{
			darkController.ClearCooldown();
		}
		public virtual void MovementUpdate(Darkness darkController){}
		protected virtual void CooldownCallback(Darkness darkController) { }
		protected void CheckTransitions(Darkness darkController)
		{
			foreach (Transition darkTran in transitions)
			{
				bool decisionResult = darkTran.decision.MakeDecision(darkTran.decisionChoice, darkController);
				if (decisionResult)
				{
					darkController.ChangeState(darkTran.trueState);
				}
			}
		}
	}
}