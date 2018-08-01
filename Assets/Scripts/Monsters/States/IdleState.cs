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
    public override void InitializeState(Darkness controller)
    {
        controller.aIRichPath.canMove = false;
        controller.animeController.SetTrigger(controller.idleHash);
        
        ExitState(controller);
    }

    public override void UpdateState(Darkness controller)
    {
    }

    public override void ExitState(Darkness controller)
    {
        float idleTime = controller.actionIdle;
        if(controller.animeController.GetBool(controller.attackAfterHash))
        {
            idleTime = idleTime/2;
            controller.animeController.SetBool(controller.attackAfterHash, true);
        }
        AI_Manager.Instance.StartCoroutine(IdleTime(controller, idleTime));
    }

    private IEnumerator IdleTime(Darkness controller, float idleTime)
    {
        yield return AI_Manager.Instance.WaitTimer(idleTime);
        controller.aIRichPath.canMove = true;
        if(!controller.canAttack)
            controller.ChangeState(EnemyState.WANDER);
        else controller.ChangeState(EnemyState.CHASING);
    }
}