using UnityEngine;

namespace DarknessMinion
{
    [CreateAssetMenu(menuName = "Darkness/SpawnState")]
    public class SpawnState : DarkState
    {
		public GameObject spawnFX;
		[Range(0, 3)]
		public float spawnDelay;
		//public SpawnState(Darkness dControl) : base(dControl){ }
		public override void InitializeState(Darkness darkController)
		{	
			GameObject newFX = Instantiate(spawnFX.gameObject, darkController.transform.position, Quaternion.identity) as GameObject;
            newFX.transform.SetParent(darkController.transform);
			Destroy(newFX, 3);
			darkController.AddCooldown(new CooldownInfo(darkController.CurrentAnimationLength(), CooldownInfo.CooldownStatus.Spawn, CooldownCallback));
			//darkController.movement.StopMovement();
			//Debug.Log(this.name + " cooldown added at: " + Time.time);
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
			//Destroy(darkController.GetLastObjectFromCache());
			//Debug.Log(this.name + " Check Transitions fired at: " + Time.time);
			CheckTransitions(darkController);
        }
    }
}