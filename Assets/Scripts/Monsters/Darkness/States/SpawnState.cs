using UnityEngine;

namespace DarknessMinion
{
    [CreateAssetMenu(menuName = "Darkness/SpawnState")]
    public class SpawnState : DarkState
    {
		public GameObject spawnFX;
		//public SpawnState(Darkness dControl) : base(dControl){ }
		public override void InitializeState(Darkness darkController)
		{
			darkController.AddCooldown(new CooldownInfo(darkController.CurrentAnimationLength(), CooldownStatus.Spawn, CooldownCallback));
		}

		public override void UpdateState(Darkness darkController)
		{

		}

		public override void ExitState(Darkness darkController)
		{
			
		}

        public override void MovementUpdate(Darkness darkController)
        {

        }
		protected override void CooldownCallback(Darkness darkController)
        {
            //Destroy particles that were spawned
			CheckTransitions(darkController);
        }
    }
}