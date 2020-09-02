using System;
using System.Collections.Generic;
using UnityEngine;

public class Dark_DecisionMaker
{
    public enum DecisionName {IS_AGGRESSIVE, PAUSED_FOR_NEXT_COMMAND, WANDER_NEAR, IN_ATTACK_RANGE, PLAYER_OUT_OF_RANGE, ATTACK_SUCCESSFULL, NAV_TARGET_CLOSE, IDLE_COMPLETE}
    Dictionary<DecisionName,Func<Darkness,bool>> Decisions;

    public Dark_DecisionMaker()
    {
        Decisions = new Dictionary<DecisionName,Func<Darkness,bool>>();
        Decisions.Add(DecisionName.IS_AGGRESSIVE,AggresiveCheck);
        Decisions.Add(DecisionName.PAUSED_FOR_NEXT_COMMAND, AwaitingNextCommand);
        Decisions.Add(DecisionName.WANDER_NEAR, ShouldWanderNear);
        Decisions.Add(DecisionName.PLAYER_OUT_OF_RANGE, PlayerDistCheck);
        Decisions.Add(DecisionName.IN_ATTACK_RANGE, CommitToAttack);
        Decisions.Add(DecisionName.ATTACK_SUCCESSFULL, AttackSuccessfull);
        Decisions.Add(DecisionName.NAV_TARGET_CLOSE, TargetDistClose);
        Decisions.Add(DecisionName.IDLE_COMPLETE, IdleOnCooldown);
    }



    public bool MakeDecision(DecisionName dName, Darkness controller)
    {   
        if(controller != null)
            return Decisions[dName].Invoke(controller);
        else return false;
    }

    private bool IdleOnCooldown(Darkness controller)
    {
        return !controller.CheckActionsOnCooldown(Dark_State.CooldownStatus.Idling);
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
        if(AggresiveCheck(controller) && (controller.navTargetDist <= controller.swtichDist || controller.playerDist < controller.swtichDist)) 
        {
            return true;
        }
        else return false;
    }

    private bool AttackSuccessfull(Darkness controller)
    {
        if(controller.attacked)
        {
            return true;
        }
        else return false;
    }

    private bool AttackOnCooldown(Darkness controller)
    {
        return controller.CheckActionsOnCooldown(Dark_State.CooldownStatus.Attacking);
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

    private bool PlayerDistCheck(Darkness controller)
    {
        if(controller.playerDist > controller.swtichDist) 
            return true;
        else return false;
    }

    private bool TargetDistClose(Darkness controller)
    {
        if(controller.navTargetDist < controller.swtichDist)
        {
            return true;
        }
        else return false;
    }
}