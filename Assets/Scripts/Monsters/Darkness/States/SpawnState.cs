using UnityEngine;

namespace DarknessMinion
{
    [CreateAssetMenu(menuName = "Darkness/SpawnState")]
    public class SpawnState : DarkState
    {
		public GameObject spawnFX;
		[Range(0, 10)]
		public float spawnAnimTime;
		//public SpawnState(Darkness dControl) : base(dControl){ }
		public override void InitializeState(Darkness darkController)
		{
			GameObject newFX = Instantiate(spawnFX.gameObject, darkController.transform.position, Quaternion.identity) as GameObject;
            newFX.transform.SetParent(darkController.transform);
			darkController.AddToStateCache(newFX);
			darkController.AddCooldown(new CooldownInfo(spawnAnimTime, CooldownStatus.Spawn, CooldownCallback)); //darkController.CurrentAnimationLength()
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
			CheckTransitions(darkController);
			darkController.ClearStateCache();
        }
    }
}