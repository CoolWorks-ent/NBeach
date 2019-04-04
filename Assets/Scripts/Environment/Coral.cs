using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class Coral : MonoBehaviour {

    private GVRInteractiveItem m_InteractiveItem;
    public GameObject bubbles;
    GameObject bubbleCopy;

    // Use this for initialization
    void Start () {
        m_InteractiveItem = GetComponent<GVRInteractiveItem>();
        m_InteractiveItem.OnClick += TouchCoral;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void TouchCoral()
    {
        Debug.Log("coral touched");
        //make object bounce slightly
        StartCoroutine(BounceEffect());
        //play bubble particle
        //may need to adjust the position the bubbles should appear
        bubbleCopy = Instantiate(bubbles, transform.position, bubbles.transform.rotation) as GameObject;
        bubbleCopy.GetComponentInChildren<ParticleSystem>().Play();
    }

    IEnumerator BounceEffect()
    {
        Vector3 baseScale = transform.localScale;
        Vector3 newScale = new Vector3(baseScale.x+.2f,baseScale.y+.2f, baseScale.z+.2f);
        float baseAmt = 1f, newAmt = 2f; //1.15f
        float t = 0f;
        float bounceTime = 1f;
        float time = 0f;
        while (time < bounceTime)
        {
            //increment timer once per frame
            t += Time.deltaTime;
            time = t / bounceTime;

            float value = EaseOutBounce(newAmt, baseAmt, time);
            transform.localScale = baseScale * value;
            yield return null;
        }

        yield return new WaitForSeconds(3); //play bubble effect for [x] seconds only
        bubbleCopy.GetComponentInChildren<ParticleSystem>().Stop();

        yield return null;
    }

    //ease functions from Copyright (c)2001 Robert Penner
    public static float EaseOutBounce(float start, float end, float value)
    {
        value /= 1f;
        end -= start;
        if (value < (1 / 2.75f))
        {
            return end * (7.5625f * value * value) + start;
        }
        else if (value < (2 / 2.75f))
        {
            value -= (1.5f / 2.75f);
            return end * (7.5625f * (value) * value + .75f) + start;
        }
        else if (value < (2.5 / 2.75))
        {
            value -= (2.25f / 2.75f);
            return end * (7.5625f * (value) * value + .9375f) + start;
        }
        else
        {
            value -= (2.625f / 2.75f);
            return end * (7.5625f * (value) * value + .984375f) + start;
        }
    }

    public static float EaseInOutElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0)
            return start;

        if ((value /= d / 2) == 2)
            return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        if (value < 1)
            return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
    }
}
