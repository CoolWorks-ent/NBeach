using UnityEngine;
using UnityEditor;

[CreateAssetMenu (menuName = "AI/Darkness/State/ChaseState")]
public class ChaseState : Dark_State
{
    [Range(0.5f,12.0f)]
    public float minRepathRate, maxRepathRate;
    [Range(1.0f,10.0f)]
    public float minSpeedRange, maxSpeedRange;

    public override void InitializeState(Darkness controller)
    {
        controller.animeController.SetTrigger(controller.chaseHash);
        controller.aIRichPath.canMove = true;
        controller.aIRichPath.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.aIRichPath.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
    }

    public override void UpdateState(Darkness controller)
    {
        //controller.aIMovement.target = controller.target;
        //controller.aIMovement.UpdatePath();
        CheckTransitions(controller);
        /*if(controller.TargetWithinDistance(controller.attackInitiationRange))
        {
            AI_Manager.OnAttackRequest(controller.queueID);
            ExitState(controller);
            controller.ChangeState(EnemyState.ATTACK);
        }*/
    }

    protected override void ExitState(Darkness controller)
    {
        //controller.aIRichPath.canMove = false;
    }
}