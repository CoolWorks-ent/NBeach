using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Decision/IdleDecision")]
public class IdleDecision : AI_Decision {

	public override bool Decide(Darkness controller)
	{
		bool withinDistance = controller.TargetWithinDistance();
		if(withinDistance && controller.canAttack)
			return false;
		else return true;
	}
}