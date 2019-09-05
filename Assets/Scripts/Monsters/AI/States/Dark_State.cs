using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Dark_State : ScriptableObject
{
    public enum StateType { CHASING, WANDER, IDLE, ATTACK, DEATH, PAUSE, REMAIN }
    public StateType stateType;
    public AI_Transition[] transitions;
    public List<Dark_State> ReferencedBy;
    //protected Lookup<AI_Transition.Transition_Priority, AI_Transition> priorityTransitions;
    public abstract void InitializeState(Darkness controller);
    public abstract void UpdateState(Darkness controller);
    protected abstract void ExitState(Darkness controller);

    protected virtual void Awake()
    {
        //AI_Manager.RemoveDarkness += RemoveDarkness;
    }

    public void Startup()
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

    protected void CheckTransitions(Darkness controller)
    {
        for(int i = 0; i < transitions.Length; i++)
        {
            bool decisionResult = transitions[i].decision.MakeDecision(transitions[i].decisionChoice,controller);
            if(decisionResult) 
            {
                if(transitions[i].trueState.stateType == StateType.REMAIN)
                    continue;
                if(AI_Manager.Instance.DarknessStateChange(transitions[i].trueState, controller))
                    ProcessStateChange(transitions[i].trueState, controller);
            }
            else if(!decisionResult) 
            {
                if(transitions[i].falseState.stateType == StateType.REMAIN)
                    continue;
                if(AI_Manager.Instance.DarknessStateChange(transitions[i].falseState, controller))
                    ProcessStateChange(transitions[i].falseState, controller);
            }
        }   
    }

    protected void ProcessStateChange(Dark_State approvedState, Darkness controller)
    {
        ExitState(controller);
        controller.ChangeState(approvedState);
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