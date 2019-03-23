using UnityEngine;
using UnityEditor;

[CreateAssetMenu (menuName = "AI/Darkness/State/ChaseState")]
public class ChaseState : Dark_State
{

    [Range(0, 5)]
    public float stopDist;
    [Range(1,8)]
    public float minSpeedRange, maxSpeedRange;
    [Range(0,5)]
    public float minRepathRate, maxRepathRate;

    private float lowRepathCap, highRepathCap, lowSpeedCap, highSpeedCap;

    protected override void Awake()
    {
        stateType = StateType.CHASING;
        lowRepathCap = 0.5f;
        lowSpeedCap = 1.0f;
        highSpeedCap = 10.0f;
        highRepathCap = 12.0f;
        base.Awake();

    }

    public override void InitializeState(Darkness controller)
    {
        controller.animeController.SetTrigger(controller.chaseHash);
        controller.aIRichPath.canMove = true;
        controller.aIRichPath.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.aIRichPath.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
        controller.aIRichPath.endReachedDistance = stopDist;
    }

    public override void UpdateState(Darkness controller)
    {
        //controller.aIMovement.target = controller.target;
        //controller.aIMovement.UpdatePath();
        CheckTransitions(controller);
    }

    protected override void ExitState(Darkness controller)
    {
        controller.aIRichPath.canMove = false;
    }

    /* void OnGUI()
    {
        EditorGUILayout.MinMaxSlider(ref minRepathRate, ref maxRepathRate, lowRepathCap, highRepathCap);
        EditorGUILayout.MinMaxSlider(ref minSpeedRange, ref maxSpeedRange, lowSpeedCap, highSpeedCap);
    }*/
}