using UnityEngine;

namespace DarknessMinion
{
    [CreateAssetMenu(menuName = "Darkness/StayInZoneState")]
    public class StayInZoneState : DarkState
    {
        public override void InitializeState(Darkness darkController)
        {
            darkController.ChangeAnimation(Darkness.DarkAnimationStates.Chase);
            darkController.movement.StartMovement();
        }

        public override void UpdateState(Darkness darkController)
        {
            CheckTransitions(darkController);
        }

        public override void MovementUpdate(Darkness darkController)
        {
            Vector3 direction = (AttackZoneManager.Instance.playerAttackZone.attackZoneOrigin - darkController.transform.position).normalized;
            darkController.movement.MoveBody(darkController.movement.UpdatePathDestination(direction));
        }

        public override void ExitState(Darkness darkController)
        {
            darkController.movement.StopMovement();
            base.ExitState(darkController);
        }
    }
}