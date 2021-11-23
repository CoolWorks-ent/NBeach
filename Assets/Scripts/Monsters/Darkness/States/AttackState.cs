using UnityEngine;

namespace Darkness.States
{

    [CreateAssetMenu(menuName = "Darkness/AttackState")]
    public class AttackState : DarkState
    {

        public override void InitializeState(DarknessController darkController)
        {   
            darkController.steering.ResetMovement();
            darkController.ChangeAnimation(DarknessController.DarkAnimationStates.Idle);
        }

        public override void UpdateState(DarknessController darkController)
        {
            CheckTransitions(darkController);
        }

        public override void MovementUpdate(DarknessController darkController) 
        {
            if(!darkController.IsAnimationPlaying(DarknessController.DarkAnimationStates.Attack)) 
            {
                if(darkController.steering.IsFacingTarget(0.8f)) 
                {
                    if (!darkController.CheckActionsOnCooldown(CooldownInfo.CooldownStatus.Attacking))
                    {
                        darkController.ChangeAnimation(DarknessController.DarkAnimationStates.Attack);
                        darkController.AssignCooldown(new CooldownInfo(1.5f, CooldownInfo.CooldownStatus.Attacking, CooldownCallback));
                        darkController.darkHitBox.enabled = true;
                    }
                }
                else darkController.steering.movementController.RotateTowardsDirection(darkController.steering.Target.position); //TODO make sure this works still
            }
        }

        protected override void CooldownCallback(DarknessController darkController)
        {
            darkController.darkHitBox.enabled = false;
        }
    }
}