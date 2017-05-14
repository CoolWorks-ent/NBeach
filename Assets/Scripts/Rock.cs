using UnityEngine;
using System;
using System.Collections;
using VRStandardAssets.Utils;


public class Rock : MonoBehaviour {

    private Vector3 startingPosition;

    public Material inactiveMaterial;
    public Material gazedAtMaterial;
    [SerializeField]
    private VRInteractiveItem m_InteractiveItem;

    void Start()
    {
        startingPosition = transform.localPosition;
        SetGazedAt(false);
    }

    private void OnEnable() { 

    }

    public void SetGazedAt(bool gazedAt)
    {
        if (inactiveMaterial != null && gazedAtMaterial != null)
        {
            Debug.Log("rock looked at");
            //if statement to determine which which material should be displayed when gazed/not gazed
            GetComponent<MeshRenderer>().material = gazedAt ? gazedAtMaterial : inactiveMaterial;
            return;
        }
        GetComponent<MeshRenderer>().material.color = gazedAt ? Color.green : Color.red;
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
