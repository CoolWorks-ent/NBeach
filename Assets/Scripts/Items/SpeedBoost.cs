﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour {

    public float speedTime = 4f, initBoostTime = 2f, boostAmt = 1f;
    float timeElapsed;
    float origSpeed;
    bool playSpeedAnim;
    bool playDeboost;
    SpeedEffectAnimator speedAnimator;
    GameController gameController;
    CameraPath pathController;

    // Use this for initialization
    void Start () {
        speedTime = 4f;
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        EventManager.StartListening("Player_SpeedBoostOff", OnDeboost);
        EventManager.StartListening("CancelPowerUps", OnCancel);
        speedAnimator = Camera.main.GetComponent<SpeedEffectAnimator>();
        origSpeed = gameController.pathControl.pathSpeed;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Player")
        {
            EventManager.TriggerEvent("Player_SpeedBoost","Player_SpeedBoost");
            OnSpeedBoost("evt");
            //destroy this object after boost is finished
        }
    }

    private void OnSpeedBoost(string str)
    {
        //start FX animation
        speedAnimator.StartAnim("anim");
        timeElapsed = Time.deltaTime;
        playSpeedAnim = true;

        //increase player speed over time
        StartCoroutine(GradualBoost());
        //*optional: set the time of the speed boost here
    }

    private void OnDeboost(string str)
    {
        playDeboost = true;
        StartCoroutine(GradualDeboost());
    }

    private void OnCancel(string str)
    {
        //reset camera path speed and stop speed animation
        speedAnimator.StopAnim("anim");

        //stop speedBoost routine & reset player's path speed
        if (playDeboost == true)
            StopCoroutine(GradualDeboost());
        if (playSpeedAnim == true)
        {
            StopCoroutine(GradualBoost());
            gameController.pathControl.pathSpeed = origSpeed;
            Debug.Log(gameController.pathControl.pathSpeed);
            //destroy this game object, that was playing the animation
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        //plays the speed effect for x seconds
        /*if (playSpeedAnim == true)
        {
            if (timeElapsed > speedTime)
            {
                EventManager.TriggerEvent("Player_SpeedBoostOff", "evt");
                playSpeedAnim = false;
            }
            timeElapsed += Time.deltaTime;
        }*/
	}

    //coroutine for increase speed of the player.  
    //Not sure if using yet or not
    IEnumerator GradualBoost()
    {
        float time = 0;
        float baseSpeed = gameController.pathControl.pathSpeed;
        Debug.Log("boosting");
        //fade sign out every second
        while (time < initBoostTime)
        {
            //increase speed over time and hold that speed
            gameController.pathControl.pathSpeed = Mathf.Lerp(baseSpeed, gameController.pathControl.pathSpeed + boostAmt, time/initBoostTime);
            time += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    IEnumerator GradualDeboost()
    {
        // decrease speed over time
         float baseSpeed = gameController.pathControl.pathSpeed;
        float decreaseTime = 1f;
        //reset time
        float time = 0;
        Debug.Log("revert boost");
        while (time < decreaseTime)
        {   
            //decrease speed back to normal speed
            gameController.pathControl.pathSpeed = Mathf.Lerp(baseSpeed, origSpeed, time / decreaseTime);
            time += Time.deltaTime;
            yield return null;
        }
        //set back to original speed
        gameController.pathControl.pathSpeed = origSpeed;
        //destroy object
        Destroy(this);
        yield return null;
    }
}
