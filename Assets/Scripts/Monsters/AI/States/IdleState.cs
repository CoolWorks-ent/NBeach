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
        //controller.aIMovement.EndMovement();
        controller.pather.canMove = false;
        controller.animeController.SetTrigger(controller.idleHash);
        AI_Manager.Instance.StartCoroutine(IdleTime(controller, idleTime));
        controller.sekr.CancelCurrentPathRequest();
        controller.pather.repathRate = 5.0f;
    }

    public override void UpdateState(Darkness controller)
    {
        // Vector3 dir = Vector3.RotateTowards(controller.transform.position, controller.target.position,0f,0f);
        // controller.transform.rotation = Quaternion.LookRotation(dir);
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