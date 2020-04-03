using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu (menuName = "AI/Darkness/State/IdleState")]
public class IdleState : Dark_State
{
    [Range(0, 5)]
    public float idleTime;
    
    protected override void FirstTimeSetup()
    {
        stateType = StateType.IDLE;
    }

    public override void InitializeState(Darkness controller)
    {
        controller.EndMovement();
        controller.animeController.SetTrigger(controller.idleHash);
        AI_Manager.Instance.StartCoroutine(IdleTime(controller, idleTime));
        base.InitializeState(controller);
    }

    public override void UpdateState(Darkness controller)
    {
        Vector3 pDir = AI_Manager.Instance.player.position - controller.transform.position; 
        Vector3 dir = Vector3.RotateTowards(controller.transform.forward, pDir, 2.0f * Time.deltaTime, 0.1f);
        controller.transform.rotation = Quaternion.LookRotation(dir);
        controller.UpdateAnimator();
    }

    public override void ExitState(Darkness controller)
    {
        AI_Manager.Instance.StopCoroutine(IdleTime(controller, idleTime));
        if(controller.animeController != null)
            controller.animeController.ResetTrigger(controller.idleHash);
    }

    protected IEnumerator IdleTime(Darkness controller, float idleTime)
    {
        yield return AI_Manager.Instance.WaitTimer(idleTime);
        CheckTransitions(controller);
        //AI_Manager.Instance.StartCoroutine(IdleTime(controller,idleTime));
        //AI_Manager.Instance.StartCoroutine(IdleTime(controller, idleTime));
    }
}