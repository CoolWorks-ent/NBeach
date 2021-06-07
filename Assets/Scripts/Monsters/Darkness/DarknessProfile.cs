using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessProfile : ScriptableObject 
{
	[Range(0, 10)]
	public float movementCheckDistance;

	[Range(0, 10)]
	public int pathSetDistance; 

	[Range(0, 5)]
	public float attackRange;
}
