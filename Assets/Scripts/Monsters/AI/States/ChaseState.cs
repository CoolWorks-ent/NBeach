using UnityEngine;
using UnityEditor;

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

    void OnGUI()
    {
        EditorGUILayout.MinMaxSlider(ref minRepathRate, ref maxRepathRate, 0.5f, 12.0f);
    }

    public override void InitializeState(Darkness controller)
    {
        controller.animeController.SetTrigger(controller.chaseHash);
        
        controller.aIRichPath.canMove = true;
        controller.aIRichPath.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.aIRichPath.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
    }

    public override void UpdateState(Darkness controller)
    {
        if(controller.ai != null) 
            controller.ai.SearchPath();
        else Debug.LogError("AI not set. Attach IAstar component to object");
        controller.ai.destination = controller.target.position;
        if(controller.TargetWithinDistance(controller.attackInitiationRange))
        {
            AI_Manager.OnAttackRequest(controller.queueID);
            ExitState(controller);
            controller.ChangeState(EnemyState.ATTACK);
        }
    }

    public override void ExitState(Darkness controller)
    {
        controller.aIRichPath.canMove = false;
    }
}