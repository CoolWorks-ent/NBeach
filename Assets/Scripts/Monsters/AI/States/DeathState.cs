using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Darkness/State/DeathState")]
public class DeathState : Dark_State
{
    public override void InitializeState(Darkness controller)
    {
        //controller.aIRichPath.canMove = false;
        controller.animeController.SetTrigger(controller.deathHash);
        
        GameObject newFX = Instantiate(controller.deathFX.gameObject, controller.transform.position, Quaternion.identity) as GameObject;
        //gameObject.GetComponent<MeshRenderer>().material.SetColor(Color.white);
        
        //change darkness back to idle to state to prevent moving & set to Kinematic to prevent any Physics effects
        controller.gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
        AI_Manager.Instance.StartCoroutine(deathRoutine(controller));
    }

    public override void UpdateState(Darkness controller)
    {
    }

    protected override void ExitState(Darkness controller)
    {
        AI_Manager.OnDarknessRemoved(controller.queueID);
    }

    
    IEnumerator deathRoutine(Darkness controller)
    {
        float fxTime = 1;
        //Slowly increase texture power over the FX lifetime to show the Darkness "Glowing" and explode!
        int maxPower = 10;
        MeshRenderer renderer = controller.gameObject.GetComponentInChildren<MeshRenderer>();
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
        Destroy(controller.gameObject);
        yield return 0;
    }
}
