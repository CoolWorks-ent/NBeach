using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmThrow : MonoBehaviour {

    Projectile_Shell projectile;
	// Use this for initialization
	void Start () {
        GetComponent<Animator>().StopPlayback();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void ThrowProjectile()
    {
        print("throw event");
        EventManager.TriggerEvent("FireProjectile","projectileThrown");
    }
}
