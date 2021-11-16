using System.Collections;
using UnityEngine;

namespace Darkness.States
{
    [CreateAssetMenu(menuName = "Darkness/DeathState")]
    public class DeathState : DarkState
    {
        public GameObject deathFX;

        public override void InitializeState(DarknessController darkController)
        {
            darkController.StartCoroutine(DeathRoutine(darkController));
        }

        public IEnumerator DeathRoutine(DarknessController darkController)
        {
            //controller.animeController.SetTrigger(controller.deathTrigHash);
            darkController.ChangeAnimation(DarknessController.DarkAnimationStates.Death);

            GameObject newFX = Instantiate(deathFX.gameObject, darkController.transform.position, Quaternion.identity) as GameObject;
            //newFX.transform.SetParent(DarknessManager.Instance.transform);
            
            //set to Kinematic to prevent any Physics effects
            darkController.gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;

            float fxTime = 1;
            //Slowly increase texture power over the FX lifetime to show the Darkness "Glowing" and explode!
            int maxPower = 10;
            SkinnedMeshRenderer renderer = darkController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            float curPower = renderer.material.GetFloat("_MainTexturePower");
            float curTime = 0;
            while (curTime < fxTime)
            {
                curPower = curTime * maxPower;
                renderer.material.SetFloat("_MainTexturePower", curPower);
                curTime += Time.deltaTime;
                yield return 0;
            }

            //yield return new WaitForSeconds(fxTime);
            //AI_Manager.Instance.RemoveFromDarknessList(controller);
            darkController.gameObject.SetActive(false);
            DarkEventManager.OnDarknessRemoved(darkController);
            //Destroy(controller.animeController);
            Destroy(darkController.gameObject);
            Destroy(newFX, 3.1f);
            yield return 0;
        }
    }
}