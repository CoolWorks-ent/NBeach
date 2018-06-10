using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Darkness/StateController")]
public class DarkStateController : ScriptableObject
{
    DarkState currentState; 

    public DarkState previousState;

    public DarkState[] darkStates;
    Dictionary<EnemyState, DarkState> States;

    public void OnEnable()
    {
        States = new Dictionary<EnemyState, DarkState>();
        for(int i = 0; i < darkStates.Length; i++)
        {
            States.Add(darkStates[i].stateType,darkStates[i]);
        }
    }

    public void ChangeState(EnemyState eState, Darkness owner)
    {
        previousState = currentState;
        currentState = States[eState];
        currentState.InitializeState(owner, this);
    }
    public void ExecuteCurrentState(Darkness owner)
    {
        currentState.UpdateState(owner);
    }

    public void RevertState(Darkness owner)
    {
        currentState = previousState;
    }
}