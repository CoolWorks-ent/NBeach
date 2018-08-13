using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/State")]
public  class AI_State : ScriptableObject
{
    public AI_Action[] actions;
    public AI_Transition[] transitions;

    public EnemyState stateType;
    public void UpdateState(Darkness controller)
    {
        ExecuteActions(controller);
        CheckTransitions(controller);
    }

    public void ExecuteActions(Darkness controller)
    {
        for(int i = 0; i < actions.Length; i++)
            actions[i].Act(controller);
    }

    public void CheckTransitions(Darkness controller)
    {
        for(int i = 0; i < transitions.Length; i++)
        {
            bool decisionSucceeded = transitions[i].decision.Decide(controller);
            if(decisionSucceeded)
                controller.TransitionToState(transitions[i].trueState);
            else controller.TransitionToState(transitions[i].falseState);
        }   
    }
}

public enum EnemyState { CHASING, WANDER, IDLE, ATTACK, DEATH }