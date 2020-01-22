using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Dark_State : ScriptableObject
{
    public enum StateType { CHASING, IDLE, ATTACK, DEATH, PAUSE, REMAIN, WANDER }
    public StateType stateType;
    public AI_Transition[] transitions;
    public List<Dark_State> ReferencedBy;
    //protected Lookup<AI_Transition.Transition_Priority, AI_Transition> priorityTransitions;
    public abstract void InitializeState(Darkness controller);
    public abstract void UpdateState(Darkness controller);
    public abstract void ExitState(Darkness controller);
    public float transitionTime = 0.75f;

    public virtual void Startup()
    {
        AI_Manager.RemoveDarkness += RemoveDarkness;
        if(ReferencedBy == null || ReferencedBy.Count < 1)
            ReferencedBy = new List<Dark_State>();
        foreach(AI_Transition ai in transitions)
        {
            if(!ai.trueState.ReferencedBy.Contains(this))
                ai.trueState.ReferencedBy.Add(this);
            if(!ai.falseState.ReferencedBy.Contains(this))
                ai.falseState.ReferencedBy.Add(this);
        }
    }

    protected virtual void FirstTimeSetup()
    {
        stateType = StateType.REMAIN;
    }

    protected void RequestNewTarget(int darkID)
    {
        AI_Manager.OnRequestNewTarget(darkID);
    }

    protected void CheckTransitions(Darkness controller)
    {
        for(int i = 0; i < transitions.Length; i++)
        {
            bool decisionResult = transitions[i].decision.MakeDecision(transitions[i].decisionChoice,controller);
            if(decisionResult) 
            {
                if(transitions[i].trueState.stateType == StateType.REMAIN)
                    continue;
                else ProcessStateChange(transitions[i].trueState, controller); 
            }
            else if(!decisionResult) 
            {
                if(transitions[i].falseState.stateType == StateType.REMAIN)
                    continue;
                else ProcessStateChange(transitions[i].falseState, controller);
            }
        }   
    }

    protected void ProcessStateChange(Dark_State approvedState, Darkness controller) //TODO Have Darkness start a coroutine to begin transitioning. 
    {
        controller.ChangeState(approvedState, transitionTime);
    }

    protected void RemoveDarkness(Darkness controller)
    {
        if(stateType != Dark_State.StateType.DEATH)
        {
            this.ExitState(controller);
            controller.updateStates = false;
            //controller.ChangeState(controller.DeathState);
        }
    }
}