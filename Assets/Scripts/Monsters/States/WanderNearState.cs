using UnityEngine;

[CreateAssetMenu (menuName = "Darkness/State/WanderNearState")]
public class WanderNearState : DarkState
{

    [Range(0.5f,12.0f)]
    public float minRepathRate, maxRepathRate;
    [Range(1.0f,10.0f)]
    public float minSpeedRange, maxSpeedRange;

    [Range(2.05f, 14.0f)]
    public float wanderRange;

    public override void OnEnable()
    {
        stateType = EnemyState.WANDER;
    }

    public override void InitializeState(Darkness controller)
    {
        controller.animeController.SetTrigger(controller.wanderHash);
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
        if(controller.TargetWithinDistance(controller.attackInitiationRange*2))
        {
            AI_Manager.OnAttackRequest(controller.queueID);
            if(controller.canAttack)
            {
                ExitState(controller);
                controller.ChangeState(EnemyState.CHASING);
            }
            else 
            {
                ExitState(controller); 
                controller.ChangeState(EnemyState.IDLE);
            }
        }
    }

    public override void ExitState(Darkness controller)
    {
        controller.aIRichPath.canMove = false;
        controller.ai.onSearchPath -= controller.ExecuteCurrentState;
        //controller.ChangeState(EnemyState.IDLE);
    }
}