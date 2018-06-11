using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DarkStateController : MonoBehaviour
{
    DarkState currentState; 

    public DarkState previousState;

    public DarkState[] darkStates;
    Dictionary<EnemyState, DarkState> States;

    public Darkness owner;

    public Animator animeController;
    public int attackHash = Animator.StringToHash("Attack"),
                attackAfterHash = Animator.StringToHash("AfterAttack"),
                chaseHash = Animator.StringToHash("Chase"),
                idleHash = Animator.StringToHash("Idle"),
                deathHash = Animator.StringToHash("Death");
    public int actionIdle;

    private void Awake()
    {
        actionIdle = 3;
        animeController = GetComponentInChildren<Animator>();
        States = new Dictionary<EnemyState, DarkState>();
        for(int i = 0; i < darkStates.Length; i++)
        {
            States.Add(darkStates[i].stateType,darkStates[i]);
        }
    }

    private void Start()
    {
        owner = GetComponent<Darkness>();
        ChangeState(EnemyState.IDLE);
    }

    public void ChangeState(EnemyState eState)
    {
        previousState = currentState;
        currentState = States[eState];
        currentState.InitializeState(this);
    }
    public void ExecuteCurrentState()
    {
        currentState.UpdateState(this);
    }

    public void RevertState(Darkness owner)
    {
        currentState = previousState;
    }

    public bool TargetWithinAttackDistance(int range)
    {
        if(Vector3.Distance(owner.target.position, owner.transform.position) <= range)
        {
            return true;
        }
        else return false;
    }
}