using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarknessMinion;

public interface IDarkDebug  
{
	string debugMessage {get; set;}

	void UpdateDebugMessage();

	void ToggleDebugMessage(bool active);
}
