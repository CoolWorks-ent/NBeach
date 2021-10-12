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
            Vector3 direction = (AttackZoneManager.Instance.playerAttackZone.attackZoneOrigin - darkController.transform.position).normalized;
            darkController.movement.UpdatePathDestination(direction);
            darkController.AddCooldown(new CooldownInfo(pathUpdateRateFar, CooldownInfo.CooldownStatus.Moving, CooldownCallback));
        }
    }
}