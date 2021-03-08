using UnityEngine;

namespace DarknessMinion
{

    [CreateAssetMenu(menuName = "Darkness/AttackState")]
    public class AttackState : DarkState
    {

        public override void InitializeState(Darkness darkController)
        {   
            darkController.movement.StopMovement();
            darkController.ChangeAnimation(Darkness.DarkAnimationStates.Idle);
        }

        public override void UpdateState(Darkness darkController)
        {
            CheckTransitions(darkController);
        }

        public override void MovementUpdate(Darkness darkController) 
        {
            if(!darkController.IsAnimationPlaying(Darkness.DarkAnimationStates.Attack)) 
            {
                if(darkController.movement.IsFacingPlayer(0.8f))
                {
                    if (!darkController.CheckActionsOnCooldown(CooldownInfo.CooldownStatus.Attacking))
                    {
                        darkController.ChangeAnimation(Darkness.DarkAnimationStates.Attack);

                        darkController.AddCooldown(new CooldownInfo(1.5f, CooldownInfo.CooldownStatus.Attacking, CooldownCallback));
                        darkController.darkHitBox.enabled = true;
                    }
                }
                else darkController.movement.RotateTowardsPlayer();
            }
        }

        public override void ExitState(Darkness darkController)
        {

        }

        protected override void CooldownCallback(Darkness darkController)
        {
            darkController.darkHitBox.enabled = false;
            darkController.movement.StopMovement();
        }
    }
}