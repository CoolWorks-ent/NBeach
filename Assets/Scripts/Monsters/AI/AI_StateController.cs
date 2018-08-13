using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI_StateController : MonoBehaviour
{
	public abstract void InitializeAI();
	public abstract void TransitionToState(AI_State state);
	public abstract void OnExitState();

	//protected abstract IEnumerator UpdateState();
}
