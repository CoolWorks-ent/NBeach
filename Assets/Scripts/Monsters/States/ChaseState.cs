using UnityEngine;

[CreateAssetMenu (menuName = "Darkness/State/ChaseState")]
public class ChaseState : DarkState
{

    [Range(0.5f,12.0f)]
    public float minRepathRate, maxRepathRate;
    [Range(1.0f,10.0f)]
    public float minSpeedRange, maxSpeedRange;
    
    public override void OnEnable()
    {
        stateType = EnemyState.CHASING;
    }

    public override void InitializeState(DarkStateController controller)
    {        
        controller.animeController.SetTrigger(controller.chaseHash);  
        controller.owner.aIRichPath.canMove = true; 
        //Vector3 pointNearTarget = new Vector3(controller.owner.target.position.x + Random.Range(-5,5), controller.owner.target.position.y, controller.owner.target.position.z + Random.Range(-5,5));
        //controller.owner.target.position = pointNearTarget;
        controller.owner.aIRichPath.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.owner.aIRichPath.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
    }

    public override void UpdateState(DarkStateController controller)
    {
        if(controller.TargetWithinAttackDistance(controller.owner.attackRange))
        {
            ExitState(controller);
            controller.ChangeState(EnemyState.ATTACK, controller.owner);
        }
    }

    public override void ExitState(DarkStateController controller)
    {
        controller.owner.aIRichPath.canMove = false;

        Debug.LogWarning("Exiting chase sequence");
    }
}