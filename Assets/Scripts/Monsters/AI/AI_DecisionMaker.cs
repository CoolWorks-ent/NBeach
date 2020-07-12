using System;
using System.Collections.Generic;

///<summary>DecisionMaker provides a way conditions to be set as requisite to transition to the next state.</summary>
public class AI_DecisionMaker
{
    
    public enum DecisionName {IN_ATTACK_RANGE, PLAYER_WITHIN_RANGE, ATTACK_SUCCESSFULL, NAV_TARGET_CLOSE, EXIT_TIMER_EXPIRED, PAUSED_FOR_NEXT_COMMAND, IS_AGGRESSIVE}
    
    ///<summary>Holds all the function calls that are called in MakeDecision</summary>
    Dictionary<DecisionName,Func<DarknessMinion,bool>> Decisions;

    public AI_DecisionMaker() //Should these choices be reduced 
    {
        Decisions = new Dictionary<DecisionName,Func<DarknessMinion,bool>>();
        //Decisions.Add(DecisionName.IS_AGGRESSIVE,AggresiveCheck);
        //Decisions.Add(DecisionName.PAUSED_FOR_NEXT_COMMAND, AwaitingNextCommand);
        //Decisions.Add(DecisionName.WANDER_NEAR, ShouldWanderNear);
        Decisions.Add(DecisionName.PLAYER_WITHIN_RANGE, PlayerWithinRange);
        Decisions.Add(DecisionName.IN_ATTACK_RANGE, InAttackRange);
        Decisions.Add(DecisionName.ATTACK_SUCCESSFULL, AttackSuccessfull);
        Decisions.Add(DecisionName.NAV_TARGET_CLOSE, NavTargetDistClose);
        //Decisions.Add(DecisionName.EXIT_TIMER_EXPIRED, ExitTimerExpired);
    }

    public bool MakeDecision(DecisionName dName, DarknessMinion controller)
    {   
        if(controller != null)
            return Decisions[dName].Invoke(controller);
        else return false;
    }

    /*private bool AggresiveCheck(DarknessMinion controller) //Should this be checked for state transitions here?
    {
        if(controller.agRatingCurrent == Darkness.AggresionRating.Attacking) 
        {
            return true;
        }
        else return false;
    }*/
    
    /*public bool ExitTimerExpired(Darkness controller)
    {
        if(!controller.timedState)
        {
            return true;
        }
        else return false;
    }*/

    private bool InAttackRange(DarknessMinion controller) //Should this be checked for state transitions here?
    {
        if(controller.playerDist <= controller.swtichDist) 
        {
            return true;
        }
        else return false;
    }

    private bool AttackSuccessfull(DarknessMinion controller) //Should this be checked for state transitions here?
    {
        if(controller.attacked)
        {
            return true;
        }
        else return false;
    }

    /*private bool AwaitingNextCommand(DarknessMinion controller) 
    {
        if(controller.agRatingCurrent == Darkness.AggresionRating.Idling) 
        {
            return true;
        }
        else return false;
    }*/

    /*private bool ShouldWanderNear(Darkness controller) 
    {
        if(controller.agRatingCurrent == Darkness.AggresionRating.Wandering) 
        {
            return true;
        }
        else return false;
    }*/

    private bool PlayerWithinRange(DarknessMinion controller) //Should this be checked for state transitions here?
    {
        if(controller.playerDist < controller.swtichDist) 
            return true;
        else return false;
    }

    private bool NavTargetDistClose(DarknessMinion controller) //Should this be checked for state transitions here?
    {
        if(controller.targetDistance < controller.swtichDist)
        {
            return true;
        }
        else return false;
    }
}