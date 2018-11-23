using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Dark_State : ScriptableObject
{
    public enum StateType { CHASING, WANDER, IDLE, ATTACK, DEATH }
    public StateType stateType;
    public AI_Transition[] transitions;
    protected Lookup<AI_Transition.Transition_Priority, AI_Transition> priorityTransitions;
    public abstract void InitializeState(Darkness controller);
    public abstract void UpdateState(Darkness controller);

    protected abstract void ExitState(Darkness controller);
        
    protected void CheckTransitions(Darkness controller)
    {
        for(int i = 0; i < transitions.Length; i++)
        {
            bool decisionSucceeded = transitions[i].decision.Decide(controller);
            if(decisionSucceeded)
                InitiateStateTransfer(transitions[i].trueState, controller);
            else InitiateStateTransfer(transitions[i].falseState, controller);
        }   
    }

    private void InitiateStateTransfer(Dark_State newState, Darkness controller)
    {
        if(newState != controller.currentState)
        {
            //ExitState(controller);
            //controller.ChangeState(newState);
            //Debug.LogWarning("<b><i>Changing states from</i></b> " + controller.currentState.name + " to new state ->" + newState.name);
            AI_Manager.OnChangeState(newState, controller, ProcessStateChange);
        }
    }

    private void ProcessStateChange(bool approved, Dark_State approvedState, Darkness controller)
    {
        if(approved)
        {
            ExitState(controller);
            controller.ChangeState(approvedState);
        }
    }
}

