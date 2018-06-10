using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public abstract class DarkState : State<DarkStateController>
{
	public EnemyState stateType;

	public abstract void OnEnable();
}

public enum EnemyState { CHASING, IDLE, ATTACK }