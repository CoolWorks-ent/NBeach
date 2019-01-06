using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Dark_State : ScriptableObject
{
    public enum StateType { CHASING, WANDER, IDLE, ATTACK, DEATH }
    public StateType stateType;
    public AI_Transition[] transitions;
    //protected Lookup<AI_Transition.Transition_Priority, AI_Transition> priorityTransitions;
    public abstract void InitializeState(Darkness controller);
    public abstract void UpdateState(Darkness controller);
    protected abstract void ExitState(Darkness controller);

    protected virtual void Awake()
    {
        AI_Manager.RemoveDarkness += RemoveDarkness;
        //if(transitions.Count() > 1)
           //Array.Sort(transitions,delegate(AI_Transition n, AI_Transition m){return m.priority.CompareTo(n.priority);});
    }

    /* private void SortTransitions()
    {
        Array.Sort(transitions,delegate(AI_Transition n, AI_Transition m){return n.priority.CompareTo(m.priority);});
    }*/
        
    protected void CheckTransitions(Darkness controller) //check here for possibly not changing to the proper state
    {
        for(int i = 0; i < transitions.Length; i++)
        {
            bool decisionResult = transitions[i].decision.Decide(controller);
            if(decisionResult) //&& transitions[i].trueState.stateType != controller.currentState.stateType)
                InitiateStateTransfer(transitions[i].trueState, controller);
            else if(!decisionResult) //&& transitions[i].falseState.stateType != controller.currentState.stateType)
                InitiateStateTransfer(transitions[i].falseState, controller);
            //else continue;
        }   
    }

    protected void InitiateStateTransfer(Dark_State newState, Darkness controller)
    {
        //if(newState != controller.currentState)
        //{
            //ExitState(controller);
            //controller.ChangeState(newState);
            //Debug.LogWarning("<b><i>Changing states from</i></b> " + controller.currentState.name + " to new state ->" + newState.name);
            AI_Manager.OnChangeState(newState, controller, ProcessStateChange);
        //}
    }

    protected void ProcessStateChange(bool approved, Dark_State approvedState, Darkness controller)
    {
        if(approved)
        {
            ExitState(controller);
            controller.ChangeState(approvedState);
        }
    }

    protected void RemoveDarkness(Darkness controller)
    {
        if(stateType != Dark_State.StateType.DEATH)
        {
            this.ExitState(controller);
            controller.ChangeState(controller.DeathState);
        }
    }
}

