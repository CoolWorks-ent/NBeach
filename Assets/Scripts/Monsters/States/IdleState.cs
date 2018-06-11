using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu (menuName = "Darkness/State/IdleState")]
public class IdleState : DarkState
{
    public float idleTime;
    public override void OnEnable()
    {
        stateType = EnemyState.IDLE;
    }
    public override void InitializeState(DarkStateController controller)
    {
        controller.owner.aIRichPath.canMove = false;
        //Debug.Log("Entering idle state");
        controller.animeController.SetTrigger(controller.idleHash);
        
        ExitState(controller);
    }

    public override void UpdateState(DarkStateController controller)
    {
    }

    public override void ExitState(DarkStateController controller)
    {
        float idleTime = controller.actionIdle;
        if(controller.animeController.GetBool(controller.attackAfterHash))
        {
            idleTime = idleTime/2;
            controller.animeController.SetBool(controller.attackAfterHash, true);
        }
        AI_Manager.Instance.StartCoroutine(IdleTime(controller, idleTime));
    }

    private IEnumerator IdleTime(DarkStateController controller, float idleTime)
    {
        yield return AI_Manager.Instance.WaitTimer(idleTime);
        controller.owner.aIRichPath.canMove = true;
        controller.ChangeState(EnemyState.CHASING);
    }
}