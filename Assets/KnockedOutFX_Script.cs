using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockedOutFX_Script : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float spinAmt = 180 * Time.deltaTime;
        transform.Rotate(new Vector3(30, 0, 90), spinAmt, Space.Self);
	}
}
