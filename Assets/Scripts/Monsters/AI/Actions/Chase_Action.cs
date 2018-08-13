using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Chase")]
public class Chase_Action : AI_Action {

	[Range(0.5f,12.0f)]
    public float minRepathRate, maxRepathRate;
    [Range(1.0f,10.0f)]
    public float minSpeedRange, maxSpeedRange;
	
	public override void Act(Darkness controller)
	{
        if(!controller.aIRichPath.canMove)
		    controller.aIRichPath.canMove = true;
        controller.aIRichPath.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.aIRichPath.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);

        if(controller.ai != null) 
            controller.ai.SearchPath();
        else Debug.LogError("AI not set. Attach IAstar component to object");
        controller.ai.destination = controller.target.position;
	}
}
