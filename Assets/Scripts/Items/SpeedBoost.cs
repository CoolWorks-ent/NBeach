using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour {

    public float speedTime = 4f, initBoostTime = 2f, boostAmt = .5f;
    float timeElapsed;
    float origSpeed;
    bool playSpeedAnim;
    SpeedEffectAnimator speedAnimator;
    GameController gameController;
    CameraPath pathController;

    // Use this for initialization
    void Start () {
        speedTime = 4f;
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        EventManager.StartListening("Player_SpeedBoostOff", OnDeboost);
        speedAnimator = Camera.main.GetComponent<SpeedEffectAnimator>();
        origSpeed = gameController.pathControl.pathSpeed;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Player")
        {
            EventManager.TriggerEvent("Player_SpeedBoost","Player_SpeedBoost");
            OnSpeedBoost("evt");
            //increase player speed over time
            StartCoroutine(GradualBoost());
            //destroy this object after boost is finished
        }
    }

    private void OnSpeedBoost(string str)
    {
        speedAnimator.StartAnim("anim");
        timeElapsed = Time.deltaTime;
        //EventManager.TriggerEvent("Player_SpeedBoost","evt");
        playSpeedAnim = true;
        
        //*optional: set the time of the speed boost here
    }

    private void OnDeboost(string str)
    {
        StartCoroutine(GradualDeboost());
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
        //fade sign out every second
        while (time < initBoostTime)
        {
            Debug.Log("boosting");
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
        while (time < decreaseTime)
        {
            Debug.Log("revert boost");
            //decrease speed back to normal speed
            gameController.pathControl.pathSpeed = Mathf.Lerp(baseSpeed, origSpeed, time / decreaseTime);
            time += Time.deltaTime;
            yield return null;
        }
        //set back to original speed
        gameController.pathControl.pathSpeed = origSpeed;
       yield return null;
    }
}
