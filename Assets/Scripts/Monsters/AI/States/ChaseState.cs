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
        if(controller.ai != null) 
            controller.ai.onSearchPath += controller.ExecuteCurrentState;
        else Debug.LogError("AI not set. Attach IAstar component to object");
        controller.aIRichPath.canMove = true;
        controller.aIRichPath.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.aIRichPath.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
    }

    public override void UpdateState(Darkness controller)
    {
        controller.ai.destination = controller.target.position;

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
        controller.ai.onSearchPath -= controller.ExecuteCurrentState;
        //controller.aIRichPath.canMove = false;
    }
}