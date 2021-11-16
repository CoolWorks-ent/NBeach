using System;
using System.Collections.Generic;
using Darkness;
using UnityEngine;

namespace Darkness.States
{
    public class DecisionMaker
    {
        public enum DecisionName { IsAggressive, IsIdling, PausedForNextCommand, IsWandering, InAttackRange, PlayerOutOfRange, AttackOnCooldown, IdleComplete, CloseToPlayer, NotInZone, InsideZone }
        Dictionary<DecisionName, Func<DarknessController, bool>> Decisions;

        public DecisionMaker()
        {
            Decisions = new Dictionary<DecisionName, Func<DarknessController, bool>>();
            Decisions.Add(DecisionName.IsAggressive, AggresiveCheck);
            Decisions.Add(DecisionName.PausedForNextCommand, PausedNextCommandCheck);
            Decisions.Add(DecisionName.IsWandering, WanderingCheck);
            Decisions.Add(DecisionName.PlayerOutOfRange, PlayerOutOfRangeCheck);
            Decisions.Add(DecisionName.InAttackRange, AttackRangeCheck);
            Decisions.Add(DecisionName.AttackOnCooldown, AttackOnCooldownCheck);
            Decisions.Add(DecisionName.IdleComplete, IdleOnCooldownCheck);
            Decisions.Add(DecisionName.IsIdling, IdlingCheck);
            Decisions.Add(DecisionName.NotInZone, NotInTheZone);
            Decisions.Add(DecisionName.InsideZone, AlreadyInZone);
        }

        public bool MakeDecision(DecisionName dName, DarknessController controller)
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

        private bool IdleOnCooldownCheck(DarknessController controller)
        {
            return !controller.CheckActionsOnCooldown(CooldownInfo.CooldownStatus.Idling);
        }

        private bool AggresiveCheck(DarknessController controller)
        {
           return controller.agRatingCurrent == DarknessController.AggresionRating.Attacking;
        }

        private bool AttackRangeCheck(DarknessController controller)
        {
            if (controller.PlayerDistance() < controller.attackSwitchRange)
                return true;
            else return false;
        }

        /*private bool AttackSuccessfullCheck(Darkness controller)
        {
            if (controller.attacked)
                return true;
            else return false;
        }*/

        private bool AttackOnCooldownCheck(DarknessController controller)
        {
            return controller.CheckActionsOnCooldown(CooldownInfo.CooldownStatus.Attacking);
        }

        private bool PausedNextCommandCheck(DarknessController controller)
        {
            if (controller.agRatingCurrent == DarknessController.AggresionRating.Idling)
                return true;
            else return false;
        }

        private bool WanderingCheck(DarknessController controller)
        {
            if (controller.agRatingCurrent == DarknessController.AggresionRating.Wandering)
                return true;
            else return false;
        }

        private bool IdlingCheck(DarknessController controller)
        {
            if(controller.agRatingCurrent == DarknessController.AggresionRating.Idling)
                return true;
            else return false;
        }

        private bool PlayerOutOfRangeCheck(DarknessController controller)
        {
            if (controller.PlayerDistance() > controller.attackSwitchRange)
                return true;
            else return false;
        }

        private bool NotInTheZone(DarknessController controller)
        {
            if(AttackZoneManager.Instance.playerAttackZone.InTheZone(controller.transform.position.ToVector2()))
            {
                return false;
            }
            return true;
        }

        private bool AlreadyInZone(DarknessController controller)
        {
            return !NotInTheZone(controller);
        }
    }
}