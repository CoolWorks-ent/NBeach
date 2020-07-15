using System.Collections;
using UnityEngine;

[CreateAssetMenu (menuName = "Darkness/Action/IdleAction")]
public class IdleAction : Dark_Action
{
    [SerializeField, Range(0,5)]
    private float idleTime;

    public override void ExecuteAction(DarknessMinion controller)
    {
        controller.EndMovement();
        controller.StartCoroutine(IdleTime(controller, idleTime));
    }



    public override void TimedTransition(DarknessMinion controller)
    {
        
    }

    public override void ExitAction(DarknessMinion controller)
    {
        
    }

    protected IEnumerator IdleTime(DarknessMinion controller, float idleTime)
    {
        yield return controller.WaitTimer(idleTime);
    }
}