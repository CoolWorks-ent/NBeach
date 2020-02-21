using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dark_Transition
{
    public enum Transition_Priority {LOW = 1, MEDIUM = 3, HIGH = 5}
    //public Transition_Priority priority;
    public AI_DecisionMaker decision = new AI_DecisionMaker();

    public AI_DecisionMaker.DecisionName decisionChoice;
    public Dark_State trueState;
    public Dark_State falseState;
}