using System.Collections;
using UnityEngine;

namespace DarknessMinion
{
    [CreateAssetMenu(menuName = "Darkness/DeathState")]
    public class DeathState : DarkState
    {
        //public DeathState(Darkness dControl) : base(dControl){ }

        public GameObject deathFX;

        public override void InitializeState(Darkness darkController)
        {   
            darkController.StartCoroutine(DeathRoutine(darkController));
            //controller.aIMovement.EndMovement();
        }

        public override void UpdateState(Darkness darkController){ }

        public override void ExitState(Darkness darkController)
        { 
            base.ExitState(darkController);
        }

        public override void MovementUpdate(Darkness darkController) { }

        public IEnumerator DeathRoutine(Darkness darkController)
        {
            //controller.animeController.SetTrigger(controller.deathTrigHash);
            darkController.ChangeAnimation(Darkness.DarkAnimationStates.Death);

            GameObject newFX = Instantiate(deathFX.gameObject, darkController.transform.position, Quaternion.identity) as GameObject;
            newFX.transform.SetParent(DarknessManager.Instance.transform);
            
            //change darkness back to idle to state to prevent moving & set to Kinematic to prevent any Physics effects
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