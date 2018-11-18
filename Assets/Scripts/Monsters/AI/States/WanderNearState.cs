using UnityEngine;

[CreateAssetMenu (menuName = "AI/Darkness/State/WanderNearState")]
public class WanderNearState : Dark_State
{

    [Range(0.5f,12.0f)]
    public float minRepathRate, maxRepathRate;
    [Range(1.0f,10.0f)]
    public float minSpeedRange, maxSpeedRange;

    [Range(2.05f, 14.0f)]
    public float wanderRange;


    public override void InitializeState(Darkness controller)
    {
        controller.animeController.SetTrigger(controller.chaseHash);
        
        //controller.aIRichPath.canMove = true;
        //controller.aIRichPath.repathRate = Random.Range(minRepathRate, maxRepathRate);
        //controller.aIRichPath.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
    }

    public override void UpdateState(Darkness controller)
    {
        /* if(controller.aIMovement != null) 
            controller.aIMovement.aI.SearchPath();
        else Debug.LogError("AI not set. Attach IAstar component to object");*/
        /* if(controller.TargetWithinDistance(controller.attackInitiationRange*2))
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

    protected override void ExitState(Darkness controller)
    {
        //controller.aIRichPath.canMove = false;
        //controller.ChangeState(EnemyState.IDLE);
    }
}