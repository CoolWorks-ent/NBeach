using UnityEngine;

namespace Darkness.States
{
    [CreateAssetMenu(menuName = "Darkness/StayInZoneState")]
    public class StayInZoneState : ChaseState
    {
        public override void UpdateState(DarknessController darkController)
        {
            CheckTransitions(darkController);
        }

        protected override void CooldownCallback(DarknessController darkController)
        {
            //arrive and obstacle avoid
            Vector2 direction = Vector2.zero;
            Vector2 pos = AttackZoneManager.Instance.playerAttackZone.attackZoneOrigin.ToVector2();

            direction += darkController.steering.Seek(pos);
            direction += darkController.steering.ObstacleAvoidance(1.5f, 1.25f) * 1.2f;
            darkController.steering.SetMovementDirection(direction);
            darkController.AssignCooldown(new CooldownInfo(pathUpdateRateFar, CooldownInfo.CooldownStatus.Moving, CooldownCallback));
        }
    }
}