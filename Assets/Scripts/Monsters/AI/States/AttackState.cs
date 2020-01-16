using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "AI/Darkness/State/AttackState")]
public class AttackState : Dark_State
{
    [Range(1, 3)]
    public int attackSpeedModifier;

    protected override void FirstTimeSetup()
    {
        stateType = StateType.ATTACK;
    }

    public override void InitializeState(Darkness controller)
    {
        //controller.aIRichPath.canMove = false;
        //controller.aIRichPath.maxSpeed *= attackSpeedModifier;
        RequestNewTarget(controller.creationID);
        controller.animeController.SetTrigger(controller.attackHash);
        AI_Manager.Instance.StartCoroutine(IdleTime(controller, 2));
    }

    public override void UpdateState(Darkness controller)
    { //check for to see if the animation finished playing 
        //controller.aIMovement.UpdatePath();
    }

    protected override void ExitState(Darkness controller)
    {
        //controller.animeController.SetBool(controller.attackAfterHash, true);
    }

    protected IEnumerator IdleTime(Darkness controller, float idleTime)
    {
        yield return AI_Manager.Instance.WaitTimer(idleTime);
        CheckTransitions(controller);
    }
}