using UnityEngine;
using System.Collections;

public class FishMovements : MonoBehaviour {
	
	public Vector3 startingPoint;
	public float maxDist;
	public Vector3 rndShift;
	
	// Use this for initialization
	void Start () {
		
		startingPoint = transform.position;
		rndShift = startingPoint + maxDist * Random.insideUnitSphere;
		rndShift.y = startingPoint.y;
		
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if ( Vector3.Distance ( transform.position, rndShift ) > 0.1f )
		{
			transform.position = Vector3.MoveTowards( transform.position, rndShift, Time.deltaTime );			
			Quaternion targRot = Quaternion.LookRotation( rndShift  - transform.position );
			transform.rotation = Quaternion.Lerp( transform.rotation, targRot, Time.deltaTime );			
		}
		else
		{
			rndShift = startingPoint + maxDist * Random.insideUnitSphere;
			rndShift.y = startingPoint.y;
		}
	
	}
		
	 void OnCollisionEnter(Collision collision) {
	
		Debug.Log("Collision");
		
		foreach (ContactPoint contact in collision.contacts)
		{
			rndShift = maxDist * contact.normal;
			rndShift.y = startingPoint.y;        
        }
		
		
		
	}	

}
