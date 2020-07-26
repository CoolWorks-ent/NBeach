using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darkness
{
    [System.Serializable]
    public class Dark_Transition //repurpose for transtitioning between passive and aggressive states
    {
        
        public AI_DecisionMaker decision = new AI_DecisionMaker();

        ///<summary>Provides dropdown in the inspector using the DecisionName enum. Used in Dark_State Update</summary>
        public AI_DecisionMaker.DecisionName decisionChoice;

        //trueState and falseState are both assigned in the inspector
        //These are the states that execute based on the results of the decisionChoice. Check inspector if execution needs to be changed.
        public Dark_State trueState;
        public Dark_State falseState;
    }
}