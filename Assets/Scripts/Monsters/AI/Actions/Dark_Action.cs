using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Dark_Action : ScriptableObject
{
	[SerializeField, Range(0, 3)]
    public float speedModifier;

    [SerializeField, Range(0, 15)]
    protected float stopDist;

    [SerializeField, Range(0,360)]
    protected int rotationSpeed;

    [SerializeField, Range(0, 5)]
    public float pathUpdateRate;

    [SerializeField]
    protected bool hasExitTimer;

    [Range(0, 5)]
    public float exitTime;

    public enum AnimationType {Chase, Idle, None, Attack}
    public enum ActionFlags {PlayerInAttackRange, AttackSuccessfull, NavTargetDistClose}

	//[SerializeField]
	//public bool parallelAction; //can this action run in parallel with other actions or should it take precedent i.e the attack action should not be parallel

	//TODO add action type - Universal can be used by all while aggresive actions are reserved for aggressive states
	public bool hasTransition, overrideTransition;

	public ActionFlags[] Conditions;

    private Dictionary<ActionFlags,Func<DarknessMinion,bool>> ActionFlagCheck;

	public abstract void ExecuteAction(DarknessMinion controller);
	public abstract void TimedTransition(DarknessMinion controller);

    public bool ConditionsMet(DarknessMinion controller)
    {
        foreach(ActionFlags actCond in Conditions)
        {
            if(!CheckFlag(actCond, controller))
                return false;
        }

        return true;
    }  

    void Awake() 
    {
        ActionFlagCheck = new Dictionary<ActionFlags, Func<DarknessMinion, bool>>();
        ActionFlagCheck.Add(ActionFlags.PlayerInAttackRange, PlayerInAttackRange);
        ActionFlagCheck.Add(ActionFlags.NavTargetDistClose, NavTargetDistClose);
        ActionFlagCheck.Add(ActionFlags.AttackSuccessfull, AttackSuccessfull);
    }

    private bool CheckFlag(ActionFlags fName, DarknessMinion controller)
    {   
        if(controller != null)
            return ActionFlagCheck[fName].Invoke(controller);
        else return false;
    }

    private bool PlayerInAttackRange(DarknessMinion controller) 
    {
        if(controller.playerDist <= controller.swtichDist) 
        {
            return true;
        }
        else return false;
    }

    private bool AttackSuccessfull(DarknessMinion controller) 
    {
        if(controller.attacked)
        {
            return true;
        }
        else return false;
    }

    private bool NavTargetDistClose(DarknessMinion controller) 
    {
        if(controller.targetDistance < controller.swtichDist)
        {
            return true;
        }
        else return false;
    }

	//public AI_DecisionMaker decision = new AI_DecisionMaker();

	//public AI_DecisionMaker.DecisionName decisionChoice;
}
