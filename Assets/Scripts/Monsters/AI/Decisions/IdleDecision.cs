using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Decision/IdleDecision")]
public class IdleDecision : AI_Decision {

	public override bool Decide(Darkness controller)
	{
		if(AI_Manager.Instance.CanMove(controller.queueID))
			return true;
		else return false;
		/* bool withinDistance = controller.TargetWithinDistance();
		if(withinDistance && controller.canAttack)
			return false;
		else return true;*/
	}
}