using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "AI/Darkness/State/AttackState")]
public class AttackState : Dark_State
{
    [Range(1, 3)]
    public int attackSpeedModifier;
    [Range(0,5)]
    public float attackCooldown;

    protected override void FirstTimeSetup()
    {
        stateType = StateType.ATTACK;
    }

    public override void InitializeState(Darkness controller)
    {
        AI_Manager.OnRequestNewTarget(controller.creationID);
        controller.pather.destination = controller.Target.location.position;
        controller.pather.repathRate = 1.75f;
        if(controller.playerDist > controller.attackInitiationRange)
            controller.pather.canMove = true;
        else controller.pather.canMove = false;
        controller.pather.canSearch = true;
        controller.pather.rotationSpeed = 270;
        controller.pather.endReachedDistance += 1.10f;
        //controller.aIRichPath.maxSpeed *= attackSpeedModifier;
        controller.pather.canMove = true;
        controller.pather.canSearch = true;
    }

    public override void UpdateState(Darkness controller)
    { 
        //TODO check if the darkness is facing the player. if not start rotating towards the player
        controller.pather.destination = controller.Target.location.position;
        if(controller.playerDist < controller.attackInitiationRange && !controller.attacked) 
        {
            controller.attacked = true;
            controller.animeController.SetTrigger(controller.attackHash);
            controller.pather.canMove = false;
            controller.StartCoroutine(controller.AttackCooldown(attackCooldown));
            //if(controller.animeController.animation.)
            //controller.StartCoroutine(controller.AttackCooldown(attackCooldown, controller.idleHash));
        }   
        /*else 
        {
            controller.pather.canMove = true;
        }*/
        CheckTransitions(controller);
    }

    public override void ExitState(Darkness controller)
    {
        controller.pather.endReachedDistance -= 1.0f;
        //controller.attacked = false;
        //controller.animeController.SetBool(controller.attackAfterHash, true);
    }
}