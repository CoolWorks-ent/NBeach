using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Darkness/State/AttackState")]
public class AttackState : DarkState
{
    [Range(1,3)]
    public int attackSpeedModifier;
    public override void OnEnable()
    {
        stateType = EnemyState.ATTACK;
    }
    public override void InitializeState(DarkStateController controller)
    {   
        controller.owner.aIRichPath.canMove = true;
        controller.owner.aIRichPath.maxSpeed *= attackSpeedModifier;
        //ExitState(controller);
    }

    public override void UpdateState(DarkStateController controller)
    {
        controller.owner.aIRichPath.canMove = false;
        controller.animeController.SetTrigger(controller.attackHash);
        controller.owner.aIRichPath.maxSpeed /= attackSpeedModifier;
        ExitState(controller);
        /*if(Quaternion.Angle(Quaternion.LookRotation(controller.owner.target.position, controller.owner.transform.position), controller.owner.transform.rotation) <= 10)
            controller.animeController.SetTrigger(controller.attackHash);
        else
        {
            Quaternion.Slerp(controller.owner.transform.rotation, Quaternion.LookRotation(controller.owner.target.position, controller.owner.transform.position), Time.deltaTime*1.5f);
        }*/
    }

    public override void ExitState(DarkStateController controller)
    {
        controller.animeController.SetBool(controller.attackAfterHash, true);
        controller.ChangeState(EnemyState.IDLE, controller.owner);
    }

}