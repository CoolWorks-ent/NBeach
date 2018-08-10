using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu (menuName = "AI/Darkness/State/IdleState")]
public class IdleState : AI_State
{
    public float idleTime;
    public void InitializeState(Darkness controller)
    {
        controller.aIRichPath.canMove = false;
        controller.animeController.SetTrigger(controller.idleHash);
        
        ExitState(controller);
    }

    public void UpdateState(Darkness controller)
    {
    }

    public void ExitState(Darkness controller)
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