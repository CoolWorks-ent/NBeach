using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessMinion
{ 
    [System.Serializable]
    public class Dark_Transition
    {
        public enum Transition_Priority { LOW = 1, MEDIUM = 3, HIGH = 5 }
        //public Transition_Priority priority;
        public Dark_DecisionMaker decision = new Dark_DecisionMaker();

        public Dark_DecisionMaker.DecisionName decisionChoice;
        public Dark_State trueState;
        public Dark_State falseState;
    }
}