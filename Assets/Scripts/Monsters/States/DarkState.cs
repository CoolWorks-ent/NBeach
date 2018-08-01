using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public abstract class DarkState : State<Darkness>
{
	public EnemyState stateType;

	public abstract void OnEnable();
}

public enum EnemyState { CHASING, WANDER, IDLE, ATTACK, DEATH }