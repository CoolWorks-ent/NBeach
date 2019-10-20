using System.Collections;
using System.Collections.Generic;
using VRStandardAssets.Utils;
using UnityEngine;

public class RockInteract : MonoBehaviour {
    private GVRInteractiveItem m_InteractiveItem;
    Vector3 originalPos;
    float shakeDuration;

    // Use this for initialization
    void Start () {
        m_InteractiveItem = GetComponent<GVRInteractiveItem>();
        m_InteractiveItem.OnClick += OnTouch;

        shakeDuration = .5f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTouch()
    {
        StartCoroutine(RockShake(1f, .5f));
    }

    public IEnumerator RockShake(float magnitude, float duration)
    {
        originalPos = transform.localPosition;
        if (duration > 0)
            shakeDuration = duration;
        float elapsed = 0.0f;
        float shakeDamper = 5f;
        while (elapsed < shakeDuration)
        {      

            elapsed += Time.deltaTime;
            //Mathf.PerlinNoise(elapsed / shakeDamper, 0f) * curve.Eval(elapsed);

            
            float percentComplete = elapsed / shakeDuration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.2f, 0.8f);
            // map value to [-1, 1]
            float x = Random.Range(originalPos.x - .1f, originalPos.x + .1f);
            float y = Random.Range(originalPos.y - .2f, originalPos.y + .2f);
            float z = Random.Range(originalPos.z - .1f, originalPos.z + .1f);
            x *= magnitude;// * damper;
            y *= magnitude;// * damper;
            z *= magnitude;// * damper;
            
            Debug.Log(transform.localPosition);
            transform.localPosition = new Vector3(x, y, z);
            
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
