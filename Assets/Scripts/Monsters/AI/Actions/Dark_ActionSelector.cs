using System;
using System.Collections.Generic;
using UnityEngine;

public class Dark_ActionSelector : MonoBehaviour 
{
	public enum ActioName {CHECK_PLAYER_DISTANCE, ROTATE_TOWARDS, PAUSE_FOR_COMMAND, MOVE_AWAY, MOVE_TOWARDS}

	Dictionary<ActioName, Action<DarknessMinion>> Actions;

	public Dark_ActionSelector()
	{
		Actions = new Dictionary<ActioName, Action<DarknessMinion>>();
	}

	public void SelectAction(ActioName actioName, DarknessMinion controller)
	{
		if(controller != null)
		{
			Actions[actioName].Invoke(controller);
		}
	}

#region Movement Actions
	private void MoveTowards(DarknessMinion controller)
	{

	}

	private void RotateTowards(DarknessMinion controller)
	{
		
	}
#endregion

#region Utility Actions
#endregion

#region Attack Actions
#endregion
}
