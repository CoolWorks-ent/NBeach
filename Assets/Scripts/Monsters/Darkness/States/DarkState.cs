using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Darkness.States
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

		public abstract void InitializeState(DarknessController darkController);
		public virtual void UpdateState(DarknessController darkController){ }
		public virtual void ExitState(DarknessController darkController)
		{
			darkController.ClearCooldown();
			darkController.steering.ResetMovement();
		}
		public virtual void MovementUpdate(DarknessController darkController){}
		protected virtual void CooldownCallback(DarknessController darkController) { }
		protected void CheckTransitions(DarknessController darkController)
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