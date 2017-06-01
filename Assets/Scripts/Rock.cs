using UnityEngine;
using System;
using System.Collections;
using VRStandardAssets.Utils;


public class Rock : MonoBehaviour {

    private Vector3 startingPosition;

    public Material inactiveMaterial;
    public Material gazedAtMaterial;
    private GVRInteractiveItem m_InteractiveItem;

    void Start()
    {
        startingPosition = transform.localPosition;
        m_InteractiveItem = GetComponent<GVRInteractiveItem>();
        m_InteractiveItem.OnOver += LookedAt;
        m_InteractiveItem.OnOut += LookedAway;
    }

    private void OnEnable() { 

    }

    public void LookedAt()
    {
        Debug.Log("rock looked at");
        //if statement to determine which which material should be displayed when gazed/not gazed
        GetComponent<MeshRenderer>().material = gazedAtMaterial;
        //GetComponent<MeshRenderer>().material.color = Color.green;
    }

    public void LookedAway()
    {
        GetComponent<MeshRenderer>().material =  inactiveMaterial;
        //GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void Reset()
    {
    }

    public void TeleportRandomly()
    {
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
