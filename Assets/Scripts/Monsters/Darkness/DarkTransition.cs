using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessMinion
{ 
    [System.Serializable]
    public class DarkTransition
    {
        //public enum TransitionPriority { LOW = 1, MEDIUM = 3, HIGH = 5 }
        //public Transition_Priority priority;
        public DarkDecisionMaker decision = new DarkDecisionMaker();

        public DarkDecisionMaker.DecisionName decisionChoice;
        public DarkState trueState;
        public DarkState falseState;
    }
}