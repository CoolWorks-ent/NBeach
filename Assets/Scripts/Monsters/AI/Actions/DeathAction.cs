using System.Collections;
using UnityEngine;

namespace Darkness
{
    [CreateAssetMenu (menuName = "Darkness/Action/DeathAction")]
    public class DeathAction : Dark_Action {
        public GameObject deathFX;

        public void OnEnable()
        {
            actionType = ActionType.Death;
        }

        public override void ExecuteAction(DarknessMinion controller)
        {
            GameObject newFX = Instantiate(deathFX.gameObject, controller.transform.position, Quaternion.identity) as GameObject;
            //gameObject.GetComponent<MeshRenderer>().material.SetColor(Color.white);
            
            //change darkness back to idle to state to prevent moving & set to Kinematic to prevent any Physics effects
            controller.gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
            Darkness_Manager.Instance.StartCoroutine(deathRoutine(controller));
            //controller.aIMovement.EndMovement();
            Darkness_Manager.OnDarknessRemoved(controller);
        }


        IEnumerator deathRoutine(DarknessMinion controller)
        {
            float fxTime = 1;
            //Slowly increase texture power over the FX lifetime to show the Darkness "Glowing" and explode!
            int maxPower = 10;
            SkinnedMeshRenderer renderer = controller.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            float curPower = renderer.material.GetFloat("_MainTexturePower");
            float curTime = 0;
            while(curTime < fxTime)
            {
                curPower = curTime * maxPower;
                renderer.material.SetFloat("_MainTexturePower", curPower);
                curTime += Time.deltaTime;
                yield return 0;
            }
        
            //yield return new WaitForSeconds(fxTime);
            //AI_Manager.Instance.RemoveFromDarknessList(controller);
            controller.gameObject.SetActive(false);
            Darkness_Manager.OnDarknessRemoved(controller);
            //Destroy(controller.animeController);
            Destroy(controller.gameObject);
            yield return 0;
        }
    }
}