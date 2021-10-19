using UnityEngine;

namespace DarknessMinion
{
    [CreateAssetMenu(menuName = "Darkness/StayInZoneState")]
    public class StayInZoneState : ChaseState
    {
        public override void UpdateState(Darkness darkController)
        {
            CheckTransitions(darkController);
        }

        protected override void CooldownCallback(Darkness darkController)
        {
            darkController.movement.DetermineBestDirection(AttackZoneManager.Instance.playerAttackZone.attackZoneOrigin);
            darkController.AssignCooldown(new CooldownInfo(pathUpdateRateFar, CooldownInfo.CooldownStatus.Moving, CooldownCallback));
        }
    }
}