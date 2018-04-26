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
    public override void InitializeState(Darkness owner, DarkStateController controller)
    {
        dsStateController = controller;
        owner.aIPath.canMove = false;
        Debug.Log("Entering idle state");

        ExitState(owner);
    }

    public override void UpdateState(Darkness owner)
    {
    }

    public override void ExitState(Darkness owner)
    {
        AI_Manager.Instance.StartCoroutine(IdleTime(owner));
        Debug.Log("Exiting idle state");
        
    }

    private IEnumerator IdleTime(Darkness owner)
    {
        yield return AI_Manager.Instance.WaitTimer(idleTime);
        dsStateController.ChangeState(EnemyState.CHASING, owner);
    }
}