using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_DecisionMaker
{
    public enum DecisionName {CHASE_PLAYER, ATTACK_PLAYER, PAUSE_FOR_COMMAND, WANDER_NEAR}
    Dictionary<DecisionName,Func<Darkness,bool>> Decisions;

    public AI_DecisionMaker()
    {
        Decisions = new Dictionary<DecisionName,Func<Darkness,bool>>();
        Decisions.Add(DecisionName.CHASE_PLAYER,ShouldChasePlayer);
        Decisions.Add(DecisionName.ATTACK_PLAYER,ShouldAttack);
        Decisions.Add(DecisionName.PAUSE_FOR_COMMAND, ShouldPauseForCommand);
        Decisions.Add(DecisionName.WANDER_NEAR,ShouldWanderNear);
    }

    public bool MakeDecision(DecisionName dName, Darkness controller)
    {
        return Decisions[dName].Invoke(controller);
    }

    private bool ShouldChasePlayer(Darkness controller)
    {
        if(controller.canAttack)
            return true;
        else return false;
    }

    private bool ShouldAttack(Darkness controller)
    {
        if(controller.TargetWithinDistance(controller.attackInitiationRange) && controller.canAttack) 
        {
            return true;
        }
        else return false;
    }

    private bool ShouldPauseForCommand(Darkness controller)
    {
        if(controller.aIRichPath.canMove) 
        {
            return true;
        }
        else return false;
    }

    private bool ShouldWanderNear(Darkness controller)
    {
        if(controller.standBy && Vector3.Distance(controller.target.position,controller.transform.position) > controller.waitRange) 
        {
            return true;
        }
        else return false;
    }
}