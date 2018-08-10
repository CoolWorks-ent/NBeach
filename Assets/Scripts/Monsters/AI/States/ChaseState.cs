using UnityEngine;
using UnityEditor;

[CreateAssetMenu (menuName = "AI/Darkness/State/ChaseState")]
public class ChaseState : AI_State
{

    public void InitializeState(Darkness controller)
    {
        /*controller.animeController.SetTrigger(controller.chaseHash);
        
        controller.aIRichPath.canMove = true;
        controller.aIRichPath.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.aIRichPath.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);*/
    }

    public void UpdateState(Darkness controller)
    {
        if(controller.ai != null) 
            controller.ai.SearchPath();
        else Debug.LogError("AI not set. Attach IAstar component to object");
        controller.ai.destination = controller.target.position;
        /*if(controller.TargetWithinDistance(controller.attackInitiationRange))
        {
            AI_Manager.OnAttackRequest(controller.queueID);
            ExitState(controller);
            controller.ChangeState(EnemyState.ATTACK);
        }*/
    }

    public void ExitState(Darkness controller)
    {
        controller.aIRichPath.canMove = false;
    }
}