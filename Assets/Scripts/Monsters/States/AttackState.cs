using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Darkness/State/AttackState")]
public class AttackState : DarkState
{
    [Range(1, 3)]
    public int attackSpeedModifier;
    public override void OnEnable()
    {
        stateType = EnemyState.ATTACK;
    }
    public override void InitializeState(Darkness controller)
    {
        controller.aIRichPath.canMove = true;
        controller.aIRichPath.maxSpeed *= attackSpeedModifier;
        //ExitState(controller);
    }

    public override void UpdateState(Darkness controller)
    {
        controller.aIRichPath.canMove = false;
        controller.animeController.SetTrigger(controller.attackHash);
        controller.aIRichPath.maxSpeed /= attackSpeedModifier;
        ExitState(controller);
    }

    public override void ExitState(Darkness controller)
    {
        //AI_Manager.OnAttackResult();
        controller.animeController.SetBool(controller.attackAfterHash, true);
        controller.ChangeState(EnemyState.IDLE);
    }

}