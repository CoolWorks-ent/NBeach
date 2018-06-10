using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public abstract class DarkState : ScriptableObject
{
	public EnemyState stateType;
	protected DarkStateController dsStateController;

	public abstract void OnEnable();
	public abstract void InitializeState(Darkness owner, DarkStateController controller);
	public abstract void UpdateState(Darkness owner);
	public abstract void ExitState(Darkness owner);
}

public enum EnemyState { CHASING, IDLE, ATTACK }