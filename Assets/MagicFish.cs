using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicFish : MonoBehaviour {

    GVRInteractiveItem m_InteractiveItem;
    GameObject player;
    public float animTime = 5f;
    public bool activated = false;
    // Use this for initialization
    void Start () {

        player = GameObject.FindGameObjectWithTag("Player");
        m_InteractiveItem = GetComponent<GVRInteractiveItem>();
        m_InteractiveItem.OnOver += OnHover;

    }
    //function for clicking on fish
    public void OnHover()
    {
        activated = true;
        StartCoroutine(Rotate());
    }


    // Update is called once per frame
    void Update () {
		
	}

    IEnumerator Rotate()
    {
        //rotate to face forward
        Quaternion curRot = transform.rotation;
        Quaternion nextRot = Quaternion.Euler(curRot.x, 180, curRot.z);
        float time = 0;
        while (time < animTime)
        {
            transform.rotation = Quaternion.Lerp(curRot, nextRot, (time / animTime));
            time += Time.deltaTime;
            yield return null;
        }
        //make dissapear
        gameObject.SetActive(false);
        yield return null;
    }
}
