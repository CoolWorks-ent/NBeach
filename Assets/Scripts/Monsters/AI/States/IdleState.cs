using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu (menuName = "AI/Darkness/State/IdleState")]
public class IdleState : Dark_State
{
    [Range(1, 5)]
    public float idleTime;

    protected override void Awake()
    {
        stateType = StateType.IDLE;
        base.Awake();
    }

    public override void InitializeState(Darkness controller)
    {
        controller.aIRichPath.canMove = false;
        controller.animeController.SetTrigger(controller.idleHash);
        
        float idleTime = controller.actionIdle;
        /*if(controller.animeController.GetBool(controller.attackAfterHash))
        {
            idleTime = idleTime/2;
            controller.animeController.SetBool(controller.attackAfterHash, true);
        }*/
        AI_Manager.Instance.StartCoroutine(IdleTime(controller, idleTime));
    }

    public override void UpdateState(Darkness controller)
    {
        
    }

    protected override void ExitState(Darkness controller)
    {
        AI_Manager.Instance.StopCoroutine(IdleTime(controller, idleTime));
    }

    protected IEnumerator IdleTime(Darkness controller, float idleTime)
    {
        yield return AI_Manager.Instance.WaitTimer(idleTime);
        CheckTransitions(controller);
        //AI_Manager.Instance.StartCoroutine(IdleTime(controller, idleTime));
    }
}