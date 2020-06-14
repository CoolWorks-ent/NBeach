using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dark_ActionConditions {
	public enum ActionFlag {PlayerInAttackRange, AttackSuccessfull, NavTargetDistClose}

    Dictionary<ActionFlag,Func<DarknessMinion,bool>> ActionFlags;

    public Dark_ActionConditions() 
    {
        ActionFlags = new Dictionary<ActionFlag, Func<DarknessMinion, bool>>();
        ActionFlags.Add(ActionFlag.PlayerInAttackRange, PlayerInAttackRange);
        ActionFlags.Add(ActionFlag.NavTargetDistClose, NavTargetDistClose);
        ActionFlags.Add(ActionFlag.AttackSuccessfull, AttackSuccessfull);
    }

    public bool CheckFlag(ActionFlag fName, DarknessMinion controller)
    {   
        if(controller != null)
            return ActionFlags[fName].Invoke(controller);
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
}