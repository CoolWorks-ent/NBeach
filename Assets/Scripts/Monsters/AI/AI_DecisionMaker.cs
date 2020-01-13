using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_DecisionMaker
{
    public enum DecisionName {IS_AGGRESSIVE, PAUSED_FOR_NEXT_COMMAND, WANDER_NEAR, IN_ATTACK_RANGE, TARGET_MOVED_FAR}
    Dictionary<DecisionName,Func<Darkness,bool>> Decisions;

    public AI_DecisionMaker()
    {
        Decisions = new Dictionary<DecisionName,Func<Darkness,bool>>();
        Decisions.Add(DecisionName.IS_AGGRESSIVE,AggresiveCheck);
        Decisions.Add(DecisionName.PAUSED_FOR_NEXT_COMMAND, AwaitingNextCommand);
        Decisions.Add(DecisionName.WANDER_NEAR,ShouldWanderNear);
        //Decisions.Add(DecisionName.NEED_NEW_TARGET, EndOfPathing);
        //Decisions.Add(DecisionName.TARGET_OUT_OF_RANGE, TargetCheck);
        Decisions.Add(DecisionName.IN_ATTACK_RANGE, CommitToAttack);
    }

    public bool MakeDecision(DecisionName dName, Darkness controller)
    {   
        if(controller != null)
            return Decisions[dName].Invoke(controller);
        else return false;
    }

    private bool AggresiveCheck(Darkness controller)
    {
        if(controller.agRatingCurrent == Darkness.AggresionRating.Attacking) 
        {
            return true;
        }
        else return false;
    }

    private bool CommitToAttack(Darkness controller)
    {
        if(controller.targetDist < controller.attackInitiationRange)
        {
            return true;
        }
        else return false;
    }

    private bool AwaitingNextCommand(Darkness controller)
    {
        if(controller.agRatingCurrent == Darkness.AggresionRating.Idling) 
        {
            return true;
        }
        else return false;
    }

    private bool ShouldWanderNear(Darkness controller) 
    {
        if(controller.agRatingCurrent == Darkness.AggresionRating.Wandering) 
        {
            return true;
        }
        else return false;
    }

    /*private bool EndOfPathing(Darkness controller)
    {
        if(controller.aIMovement.reachedEndOfPath && controller.agRatingCurrent != Darkness.AggresionRating.Idling)
            return true;
        else return false;
    }*/

    private bool TargetCheck(Darkness controller)
    {
        if(controller.targetDist > controller.attackInitiationRange) //&& controller.agRatingPrevious == Darkness.AggresionRating.Attacking)
            return true;
        else return false;
    }
}