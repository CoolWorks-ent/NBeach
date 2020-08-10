using System;
using System.Collections.Generic;

namespace Darkness
{
    ///<summary>DecisionMaker provides a way conditions to be set as requisite to transition to the next state.</summary>
    public class AI_DecisionMaker
    {
        public enum DecisionName {IS_AGGRESSIVE, IN_PATROL_DISTANCE, ACTIONS_COMPLETE}//IDLE_FINISHED, ATTACK_FINISHED,PLAYER_WITHIN_RANGE, ATTACK_SUCCESSFULL, NAV_TARGET_CLOSE, EXIT_TIMER_EXPIRED, PAUSED_FOR_NEXT_COMMAND}
        
        ///<summary>Holds all the function calls that are called in MakeDecision</summary>
        Dictionary<DecisionName,Func<DarknessMinion,bool>> Decisions;

        public AI_DecisionMaker() //Should these choices be reduced 
        {
            Decisions = new Dictionary<DecisionName,Func<DarknessMinion,bool>>();
            Decisions.Add(DecisionName.IS_AGGRESSIVE,AggresiveCheck);
            //Decisions.Add(DecisionName.ATTACK_FINISHED, AttackComplete);
            //Decisions.Add(DecisionName.IDLE_FINISHED, IdleComplete);
            Decisions.Add(DecisionName.IN_PATROL_DISTANCE, WithinPatrolPerimeter);
            Decisions.Add(DecisionName.ACTIONS_COMPLETE, ActionsCompleted);
        }

        public bool MakeDecision(DecisionName dName, DarknessMinion controller)
        {   
            if(controller != null)
                return Decisions[dName].Invoke(controller);
            else return false;
        }

        private bool AggresiveCheck(DarknessMinion controller) //Should this be checked for state transitions here?
        {
            if(controller.agRatingCurrent == DarknessMinion.AggresionRating.Aggressive) 
            {
                return true;
            }
            else return false;
        }
        
        private bool ActionsCompleted(DarknessMinion controller)
        {
            return controller.activeCooldownsComplete;
        }

        private bool WithinPatrolPerimeter(DarknessMinion controller)
        {
            return controller.playerDist < controller.patrolDistance;
        }

        /*private bool AttackComplete(DarknessMinion controller) //Should this be checked for state transitions here?
        {
            return controller.attackOnCooldown;
        }

        private bool MovementCooldownComplete(DarknessMinion controller)
        {
            return controller.movementOnCooldown;
        }

        private bool IdleComplete(DarknessMinion controller)
        {
            return controller.idleActive;
        }*/
    }
}