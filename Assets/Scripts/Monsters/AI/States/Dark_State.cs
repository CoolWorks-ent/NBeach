using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Dark_State : ScriptableObject
{
    public enum StateType {PASSIVE, AGGRESSIVE, DEATH, REMAIN}

    public StateType stateType;
    public Dark_Transition[] transitions;
    public Dark_Action[] actions;
    public List<Dark_State> ReferencedBy;
    //protected Lookup<AI_Transition.Transition_Priority, AI_Transition> priorityTransitions;


    [SerializeField, Range(0, 3)]
    public float speedModifier;

    [SerializeField, Range(0, 15)]
    protected float stopDist;

    [SerializeField, Range(0,360)]
    protected int rotationSpeed;

    [SerializeField, Range(0, 5)]
    protected float pathUpdateRate;

    [SerializeField]
    protected bool hasExitTimer;

    [Range(0, 5)]
    public float exitTime;

    public virtual void Startup()
    {
        Darkness_Manager.RemoveDarkness += RemoveDarkness;
        if(ReferencedBy == null || ReferencedBy.Count < 1)
            ReferencedBy = new List<Dark_State>();
        foreach(Dark_Transition ai in transitions)
        {
            if(!ai.trueState.ReferencedBy.Contains(this))
                ai.trueState.ReferencedBy.Add(this);
            if(!ai.falseState.ReferencedBy.Contains(this))
                ai.falseState.ReferencedBy.Add(this);
        }
    }

    public virtual void InitializeState(Darkness controller)
    {
        /*controller.pather.rotationSpeed = rotationSpeed;
        controller.pather.endReachedDistance = stopDist;
        controller.pather.maxSpeed = speedModifier;
        controller.pather.repathRate = pathUpdateRate;*/
    }
    public abstract void UpdateState(Darkness controller);
    public abstract void ExitState(Darkness controller);
    /*{
        if(hasExitTimer)
        {
            controller.timedStateExiting = true;
            controller.StartCoroutine(controller.WaitTimer(exitTime));
            controller.timedStateExiting = false;
        } 
    }*/

    protected virtual void FirstTimeSetup()
    {
        stateType = StateType.REMAIN;
    }

    protected void ActionSelector() //check if action has proper flags checked for 
    {

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

    protected Vector3 RandomPoint(Vector3 center, float radiusLower, float radiusUpper)
    {
        Vector2 point = UnityEngine.Random.insideUnitCircle * Mathf.Sqrt(UnityEngine.Random.Range(radiusLower, radiusUpper));
        return new Vector3(point.x + center.x, center.y, point.y + center.z);
    }

    protected void ProcessStateChange(Dark_State approvedState, Darkness controller)
    {
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