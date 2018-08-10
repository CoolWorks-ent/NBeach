using UnityEngine;

[CreateAssetMenu (menuName = "AI/Darkness/State/WanderNearState")]
public class WanderNearState : AI_State
{

    [Range(0.5f,12.0f)]
    public float minRepathRate, maxRepathRate;
    [Range(1.0f,10.0f)]
    public float minSpeedRange, maxSpeedRange;

    [Range(2.05f, 14.0f)]
    public float wanderRange;


    public void InitializeState(Darkness controller)
    {
        controller.animeController.SetTrigger(controller.wanderHash);
        
        controller.aIRichPath.canMove = true;
        controller.aIRichPath.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.aIRichPath.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
    }

    public void UpdateState(Darkness controller)
    {
        if(controller.ai != null) 
            controller.ai.SearchPath();
        else Debug.LogError("AI not set. Attach IAstar component to object");
        controller.ai.destination = controller.target.position;
        /*if(controller.TargetWithinDistance(controller.attackInitiationRange*2))
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
        }*/
    }

    public void ExitState(Darkness controller)
    {
        controller.aIRichPath.canMove = false;
        //controller.ChangeState(EnemyState.IDLE);
    }
}