using UnityEngine;
using System.Collections;

public class AttackState : AI_State
{
    [Range(1, 3)]
    public int attackSpeedModifier;

    public void InitializeState(Darkness controller)
    {
        controller.aIRichPath.canMove = true;
        controller.aIRichPath.maxSpeed *= attackSpeedModifier;
        //ExitState(controller);
    }

    public void UpdateState(Darkness controller)
    {
        controller.aIRichPath.canMove = false;
        controller.animeController.SetTrigger(controller.attackHash);
        controller.aIRichPath.maxSpeed /= attackSpeedModifier;
        ExitState(controller);
    }

    public void ExitState(Darkness controller)
    {
        //AI_Manager.OnAttackResult();
        controller.animeController.SetBool(controller.attackAfterHash, true);
        //controller.ChangeState(EnemyState.IDLE);
    }

}