using UnityEngine;
using System.Collections;

public class Rock : MonoBehaviour {

    private Vector3 startingPosition;

    public Material inactiveMaterial;
    public Material gazedAtMaterial;

    void Start()
    {
        startingPosition = transform.localPosition;
        SetGazedAt(false);
    }

    public void SetGazedAt(bool gazedAt)
    {
        if (inactiveMaterial != null && gazedAtMaterial != null)
        {
            Debug.Log("rock looked at");
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
