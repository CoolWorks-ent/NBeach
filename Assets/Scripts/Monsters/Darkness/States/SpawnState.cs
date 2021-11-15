using UnityEngine;

namespace DarknessMinion
{
    [CreateAssetMenu(menuName = "Darkness/SpawnState")]
    public class SpawnState : DarkState
    {
		public GameObject spawnFX;
		[Range(0, 3)]
		public float spawnDelay;

		public override void InitializeState(Darkness darkController)
		{	
			GameObject newFX = Instantiate(spawnFX.gameObject, darkController.transform.position, Quaternion.identity) as GameObject;
            newFX.transform.SetParent(darkController.transform);
			Destroy(newFX, 3);
			darkController.AssignCooldown(new CooldownInfo(darkController.CurrentAnimationLength(), CooldownInfo.CooldownStatus.Spawn, CooldownCallback));
		}

		protected override void CooldownCallback(Darkness darkController)
        {
			CheckTransitions(darkController);
        }
    }
}