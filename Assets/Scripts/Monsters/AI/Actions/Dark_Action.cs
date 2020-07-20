using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Dark_Action : ScriptableObject
{
    public enum AnimationType {Chase, Idle, None, Attack}

    [SerializeField]
    ///<summary>Assigned in inspector. Determines the animation clip used to play during the action</summary> 
    protected AnimationType animationType; 
    public bool hasTimer; 

    public enum ActionFlags {PlayerInAttackRange, AttackSuccessfull, NavTargetDistClose, PlayerOutOfRange, EndOfPath}

	public abstract void ExecuteAction(DarknessMinion controller);
	public abstract void TimedTransition(DarknessMinion controller);
    public abstract void ExitAction(DarknessMinion controller);
    
	private ActionFlags[] Conditions;

    private Dictionary<ActionFlags,Func<DarknessMinion,bool>> ActionFlagCheck = new Dictionary<ActionFlags, Func<DarknessMinion, bool>>
    {
        //{ActionFlags.PlayerInAttackRange, PlayerInAttackRange},
        {ActionFlags.NavTargetDistClose, NavTargetDistClose},
        {ActionFlags.AttackSuccessfull, AttackSuccessfull},
        //{ActionFlags.PlayerOutOfRange, PlayerOutOfRange}
    };


	

    public bool ConditionsMet(DarknessMinion controller)
    {
        foreach(ActionFlags actCond in Conditions)
        {
            if(!CheckFlag(actCond, controller))
                return false;
        }

        return true;
    } 

    private bool CheckFlag(ActionFlags fName, DarknessMinion controller)
    {   
        if(controller != null)
            return ActionFlagCheck[fName].Invoke(controller);
        else return false;
    }

    private static bool EndOfPath(DarknessMinion controller)
    {
        return controller.reachedEndOfPath;
    }

    /*private static bool PlayerInAttackRange(DarknessMinion controller) 
    {
        if(controller.playerDist <= controller.attackRange) 
        {
            return true;
        }
        else return false;
    }*/

    private static bool AttackSuccessfull(DarknessMinion controller) 
    {
        if(controller.attacked)
        {
            return true;
        }
        else return false;
    }

    private static bool NavTargetDistClose(DarknessMinion controller) 
    {
        if(controller.targetDistance < controller.switchTargetDistance || EndOfPath(controller))
        {
            return true;
        }
        else return false;
    }

    /*private static bool PlayerOutOfRange(DarknessMinion controller) 
    {
        if(controller.playerDist > controller.attackRange)
        {
            return true;
        }
        else return false;
    }*/
}
