using System;
using System.Collections.Generic;
using UnityEngine;


namespace DarknessMinion
{
    public class DarkDecisionMaker
    {
        public enum DecisionName { IsAggressive, IsIdling, PausedForNextCommand, IsWandering, InAttackRange, PlayerOutOfRange, AttackOnCooldown, NavTargetClose, IdleComplete, CloseToPlayer }
        Dictionary<DecisionName, Func<Darkness, bool>> Decisions;

        public DarkDecisionMaker()
        {
            Decisions = new Dictionary<DecisionName, Func<Darkness, bool>>();
            Decisions.Add(DecisionName.IsAggressive, AggresiveCheck);
            Decisions.Add(DecisionName.PausedForNextCommand, PausedNextCommandCheck);
            Decisions.Add(DecisionName.IsWandering, WanderingCheck);
            Decisions.Add(DecisionName.PlayerOutOfRange, PlayerOutOfRangeCheck);
            Decisions.Add(DecisionName.InAttackRange, AttackRangeCheck);
            Decisions.Add(DecisionName.AttackOnCooldown, AttackOnCooldownCheck);
            Decisions.Add(DecisionName.NavTargetClose, NavTargetCloseCheck);
            Decisions.Add(DecisionName.IdleComplete, IdleOnCooldownCheck);
            Decisions.Add(DecisionName.IsIdling, IdlingCheck);
            Decisions.Add(DecisionName.CloseToPlayer, CloseToPlayer);
        }

        public bool MakeDecision(DecisionName dName, Darkness controller)
        {
            try
            {
                if (controller != null)
                    return Decisions[dName].Invoke(controller);
                else return false;
            }
            catch(KeyNotFoundException k)
            {
                Debug.LogError("Key not found in DarkDecisionMaker: " + dName.ToString() + "Resulting in this error " + k);
                return false;
            }
        }

        private bool CloseToPlayer(Darkness controller)
        {
            return controller.movement.closeToPlayer;
        }

        private bool IdleOnCooldownCheck(Darkness controller)
        {
            return !controller.CheckActionsOnCooldown(CooldownInfo.CooldownStatus.Idling);
        }

        private bool AggresiveCheck(Darkness controller)
        {
           return controller.agRatingCurrent == Darkness.AggresionRating.Attacking;
        }

        private bool AttackRangeCheck(Darkness controller)
        {
            if (controller.movement.playerDist < controller.attackDist)
                return true;
            else return false;
        }

        /*private bool AttackSuccessfullCheck(Darkness controller)
        {
            if (controller.attacked)
                return true;
            else return false;
        }*/

        private bool AttackOnCooldownCheck(Darkness controller)
        {
            return controller.CheckActionsOnCooldown(CooldownInfo.CooldownStatus.Attacking);
        }

        private bool PausedNextCommandCheck(Darkness controller)
        {
            if (controller.agRatingCurrent == Darkness.AggresionRating.Idling)
                return true;
            else return false;
        }

        private bool WanderingCheck(Darkness controller)
        {
            if (controller.agRatingCurrent == Darkness.AggresionRating.Wandering)
                return true;
            else return false;
        }

        private bool IdlingCheck(Darkness controller)
        {
            if(controller.agRatingCurrent == Darkness.AggresionRating.Idling)
                return true;
            else return false;
        }

        private bool PlayerOutOfRangeCheck(Darkness controller)
        {
            if (controller.movement.playerDist > controller.attackDist)
                return true;
            else return false;
        }

        private bool NavTargetCloseCheck(Darkness controller)
        {
            if (controller.movement.navTargetDist < controller.attackDist)
                return true;
            else return false;
        }
    }
}