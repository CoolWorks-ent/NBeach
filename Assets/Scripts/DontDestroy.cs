using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// --DontDestroy Script--
/// The game object this sccript is attached to will not be destroyed on a scene load.
/// </summary>
/// 
public class DontDestroy : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
