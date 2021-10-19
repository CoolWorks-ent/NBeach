using System;
using System.Collections.Generic;
using UnityEngine;


namespace DarknessMinion
{
    public class DecisionMaker
    {
        public enum DecisionName { IsAggressive, IsIdling, PausedForNextCommand, IsWandering, InAttackRange, PlayerOutOfRange, AttackOnCooldown, IdleComplete, CloseToPlayer, NotInZone, InsideZone }
        Dictionary<DecisionName, Func<Darkness, bool>> Decisions;

        public DecisionMaker()
        {
            Decisions = new Dictionary<DecisionName, Func<Darkness, bool>>();
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
            if (controller.PlayerDistance() > controller.attackSwitchRange)
                return true;
            else return false;
        }

        private bool NotInTheZone(Darkness controller)
        {
            if(AttackZoneManager.Instance.playerAttackZone.InTheZone(
                controller.movement.ConvertToVec2(controller.transform.position)))
            {
                return false;
            }
            return true;
        }

        private bool AlreadyInZone(Darkness controller)
        {
            return !NotInTheZone(controller);
        }
    }
}