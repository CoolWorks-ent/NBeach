using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "AI/Darkness/State/AttackState")]
public class AttackState : Dark_State
{
    [Range(1, 3)]
    public int attackSpeedModifier;

    public override void InitializeState(Darkness controller)
    {
        //controller.aIRichPath.canMove = false;
        //controller.aIRichPath.maxSpeed *= attackSpeedModifier;
        //controller.animeController.SetTrigger(controller.attackHash);
        AI_Manager.Instance.StartCoroutine(IdleTime(controller, 2));
    }

    public override void UpdateState(Darkness controller)
    {
        //controller.animeController.SetTrigger(controller.attackHash);
        //controller.aIRichPath.maxSpeed /= attackSpeedModifier;
        
    }

    protected override void ExitState(Darkness controller)
    {
        controller.animeController.SetBool(controller.attackAfterHash, true);   
    }

    protected IEnumerator IdleTime(Darkness controller, float idleTime)
    {
        yield return AI_Manager.Instance.WaitTimer(idleTime);
        CheckTransitions(controller);
    }

}