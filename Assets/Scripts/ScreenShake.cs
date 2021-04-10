using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script ScreenShake uses Pernel Noise to shake the camera.
/// </summary>
/// 
public class ScreenShake : MonoBehaviour {

    float shakeDuration;
    Vector3 originalCamPos;
    GameObject camObj;

    public ScreenShake()
    {
        shakeDuration = 0.6f;
        camObj = GameController.instance.playerControl.gameObject;
        //print(camObj);
    }

	// Use this for initialization
	void Start () {
        shakeDuration = .6f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //set duration to 0, if you want to use default
    public void Play(float mag, float duration)
    {
        StartCoroutine(Shake(mag, duration));
    }

    //Uses Pernel Noise
    public IEnumerator Shake(float magnitude, float duration)
    {
        originalCamPos = camObj.transform.localPosition;
        if (duration > 0)
            shakeDuration = duration;
        float elapsed = 0.0f;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float percentComplete = elapsed / shakeDuration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
            // map value to [-1, 1]
            float x = Random.value * 2.0f - 1.0f;
            float z = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= magnitude * damper;
            y *= magnitude * damper / 2;
            z *= magnitude * damper / 2;

            camObj.transform.localPosition = new Vector3(x, y, z);
            yield return null;
        }
        camObj.transform.localPosition = originalCamPos;
    }
}
