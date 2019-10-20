using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoralTubeSingle : MonoBehaviour {
    [SerializeField]
    ParticleSystem fxObj;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayInteract()
    {
        StartCoroutine(BubbleEffect());
        //GetComponent<Animation>().Play();
    }

    IEnumerator BubbleEffect()
    {
        fxObj.Play();
        yield return new WaitForSeconds(3); //play bubble effect for [x] seconds only
        fxObj.Stop();
    }

}
