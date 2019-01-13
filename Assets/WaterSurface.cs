using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSurface : MonoBehaviour {
    [SerializeField]
    ParticleSystem rainDrop;
    [SerializeField]
    EnemySpawner enemySpawner;
    [SerializeField]
    public Material daytimeWaterMat;
    [SerializeField]
    public Material nighttimeWaterMat;

    private void Awake()
    {
        Debug.Log(daytimeWaterMat);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

}
