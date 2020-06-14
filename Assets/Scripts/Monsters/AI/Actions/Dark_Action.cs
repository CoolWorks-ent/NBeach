using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dark_Action {

	//TODO add action type - Universal can be used by all while aggresive actions are reserved for aggressive states
	public bool hasTransition, overrideTransition;

	public List<Dark_ActionConditions> Conditions;

	public bool ConditionEvaluator() //checks the condition flags that need to be met for this action to execute
	{
		
		return false;
	}
	//public AI_DecisionMaker decision = new AI_DecisionMaker();

	//public AI_DecisionMaker.DecisionName decisionChoice;
}
