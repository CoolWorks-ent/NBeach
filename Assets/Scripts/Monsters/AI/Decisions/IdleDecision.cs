using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Decision/IdleDecision")]
public class IdleDecision : AI_Decision {

	public override bool Decide(Darkness controller)
	{
		if(controller.idleFinished)
		{
			controller.idleFinished = false;
			return true;
		}
		else return false;
	}

}
