using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Dark_Action : ScriptableObject
{
	[SerializeField, Range(0, 3)]
    public float speedModifier;

    [SerializeField, Range(0, 15)]
    protected float stopDist;

    [SerializeField, Range(0,360)]
    protected int rotationSpeed;

    [SerializeField, Range(0, 5)]
    public float pathUpdateRate;

    [SerializeField]
    protected bool hasExitTimer;

    [Range(0, 5)]
    public float exitTime;

    public enum AnimationType {Chase, Idle, None, Attack}

	//[SerializeField]
	//public bool parallelAction; //can this action run in parallel with other actions or should it take precedent i.e the attack action should not be parallel

	//TODO add action type - Universal can be used by all while aggresive actions are reserved for aggressive states
	public bool hasTransition, overrideTransition;

	public Dark_ActionConditions.ActionFlag[] Conditions;

	public abstract void ExecuteAction(DarknessMinion controller);
	public abstract void TimedTransition(DarknessMinion controller);

	//public AI_DecisionMaker decision = new AI_DecisionMaker();

	//public AI_DecisionMaker.DecisionName decisionChoice;
}
