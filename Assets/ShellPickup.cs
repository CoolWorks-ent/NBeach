using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellPickup : MonoBehaviour {
    private GVRInteractiveItem m_InteractiveItem;
    public GameObject bubbles;
    NFPSController player;
    int ammoCount;

    // Use this for initialization
    void Start () {
        m_InteractiveItem = GetComponent<GVRInteractiveItem>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<NFPSController>();
        m_InteractiveItem.OnDown += PickUpShell;
        ammoCount = 10;
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    void PickUpShell()
    {
        
        //only pick up shell is player has less than max ammo
        if (player.playerAmmo == 0)
        {
            Debug.Log("shell pickedup");
            player.playerAmmo += ammoCount;
            
            StartCoroutine(moveItemToPlayer());
        }
        //play bubble particle
        //GameObject bubbleCopy = Instantiate(bubbles, transform.position, Quaternion.identity, transform) as GameObject;
        //bubbleCopy.GetComponentInChildren<ParticleSystem>().Play();
    }

    IEnumerator BounceEffect()
    {
        Vector3 baseScale = transform.localScale;
        Vector3 newScale = new Vector3(baseScale.x + .2f, baseScale.y + .2f, baseScale.z + .2f);
        float baseAmt = 1f, newAmt = 1.15f;
        float t = 0f;
        float bounceTime = 1f;
        float time = 0f;
        while (time < bounceTime)
        {
            //increment timer once per frame
            t += Time.deltaTime;
            time = t / bounceTime;

            float value = EaseOutBounce(baseAmt, newAmt, time);
            transform.localScale = baseScale * value;
            yield return null;
        }

        yield return null;
    }

    //Moves the shell into the player on pickup.  once returned, shells are added to player's inventory 
    IEnumerator moveItemToPlayer()
    {
        Vector3 hitPos = transform.position;
        //this position will be on the player but lower than the camera
        Vector3 finalPos = GameObject.Find("ItemReturnPoint").transform.position;

        float moveTime = .5f, bounceTime = .5f;
        float t = 0f, time = 0f;
        Vector3 baseScale = transform.localScale;
        float baseAmt = 1f, newAmt = 1.2f;
        /*
        float bounceAmt = 2.2f;
        Vector3 baseSize = transform.localScale;
        Vector3 newSize = new Vector3(baseSize.x*bounceAmt, baseSize.y* bounceAmt, baseSize.z* bounceAmt);
        //bounce
        while(time < bounceTime)
        {
            //sine wave
            //float theta = time / .3f;
            //float distance = bounceAmt * Mathf.Sin(theta);
            //transform.localScale = baseSize + new Vector3(baseSize.x * distance, baseSize.y*distance, baseSize.z*distance);

            //transform.localScale = Bounce(transform.localScale);
            //print(transform.localScale);
            //value = Bounce(bounceAmt);
            //transform.localScale = Berp(baseSize,newSize,time);
            time += Time.deltaTime;
            //transform.localScale = baseSize * value;
            yield return null;
        }
        */
        while (time < bounceTime)
        {
            //increment timer once per frame
            t += Time.deltaTime;
            time = t / bounceTime;

            float value = EaseOutBounce(baseAmt, newAmt, time);
            transform.localScale = baseScale * value;
            yield return null;
        }

        time = 0f;
        t = 0f;
        float scaleAmt = 0.1f;
        baseScale = transform.localScale;
        Vector3 newScale = new Vector3(transform.localScale.x* scaleAmt, transform.localScale.y * scaleAmt, transform.localScale.z * scaleAmt);
        //move
        while(time < moveTime)
        {
            t = time / moveTime;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(hitPos, finalPos, t);

            //decrease size over time too, to make it look like it's joining the player
            transform.localScale = Vector3.Lerp(baseScale, newScale,time/2);

            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(.5f);
        Destroy(this.gameObject);
        yield return 0;
    }

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

    //Boing
    public static float Berp(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

    public static Vector2 Berp(Vector2 start, Vector2 end, float value)
    {
        return new Vector2(Berp(start.x, end.x, value), Berp(start.y, end.y, value));
    }

    public static Vector3 Berp(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Berp(start.x, end.x, value), Berp(start.y, end.y, value), Berp(start.z, end.z, value));
    }

    //Bounce
    public static float Bounce(float x)
    {
        return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
    }

    public static Vector3 Bounce(Vector3 vec)
    {
        return new Vector3(Bounce(vec.x), Bounce(vec.y), Bounce(vec.z));
    }
}
