using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Idle")]
public class Idle_Action : AI_Action {

	public float idleTime;

	public override void Act(Darkness controller)
	{
        controller.aIRichPath.canMove = false;
		AI_Manager.Instance.StartCoroutine(IdleTime(controller, idleTime));
	}

	private IEnumerator IdleTime(Darkness controller, float idleTime)
    {
        yield return AI_Manager.Instance.WaitTimer(idleTime);
        controller.aIRichPath.canMove = true;
        controller.idleFinished = true;
        /*if(!controller.canAttack)
            controller.ChangeState(EnemyState.WANDER);
        else controller.ChangeState(EnemyState.CHASING);*/
    }
}
