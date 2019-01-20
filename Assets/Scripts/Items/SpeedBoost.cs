using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour {

    [SerializeField]
    string boostType;

    public float speedTime = 1f; //length of time player keeps speedBoost
    public float initBoostTime = .5f, boostAmt =1f;
    float timeElapsed;
    float baseSpeed;
    bool playSpeedAnim;
    bool playDeboost;
    GameObject speedBoostObj;
    SpeedEffectAnimator speedAnimator;
    GameController gameController;
    CameraPath pathController;

    // Use this for initialization
    void Start () {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        EventManager.StartListening("Player_SpeedBoostOff", OnDeboost);
        EventManager.StartListening("Player_SpeedBoost", OnSpeedBoost);
        EventManager.StartListening("CancelPowerUps", OnCancel);
        speedAnimator = Camera.main.GetComponent<SpeedEffectAnimator>();
        baseSpeed = gameController.pathControl._basePathSpeed;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (boostType == "PlayerOnly")
        {
            if (collider.tag == "Player")
            {
                EventManager.TriggerEvent("Player_SpeedBoost", "Player_SpeedBoost");
                //OnSpeedBoost("evt");
                speedBoostObj = this.gameObject;
                //destroy this object after boost is finished
            }
        }
    else
        {
            if(collider.tag == "PlayerContainer")
            {
                EventManager.TriggerEvent("Player_SpeedBoost", "Player_SpeedBoost");
                //OnSpeedBoost("evt");
                speedBoostObj = this.gameObject;
                //destroy this object after boost is finished
            }
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
        if (speedBoostObj != null)
        {
            //reset camera path speed and stop speed animation
            speedAnimator.StopAnim("anim");

            //stop speedBoost routine & reset player's path speed
            if (playDeboost == true)
                StopCoroutine(GradualDeboost());
            if (playSpeedAnim == true)
            {
                StopCoroutine(GradualBoost());
                gameController.pathControl.pathSpeed = baseSpeed;
                Debug.Log(gameController.pathControl.pathSpeed);
                //destroy this game object, that was playing the animation
                Destroy(speedBoostObj);
            }
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
        float startSpeed = gameController.pathControl.pathSpeed;
        Debug.Log("boosting");
        //fade sign out every second
        while (time < initBoostTime)
        {
            //increase speed over time and hold that speed
            gameController.pathControl.pathSpeed = Mathf.Lerp(startSpeed, gameController.pathControl.pathSpeed + boostAmt, time/initBoostTime);
            time += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(speedTime);
        //turn off boost
        EventManager.TriggerEvent("Player_SpeedBoostOff", "Player_SpeedBoost");

        yield return null;
    }

    IEnumerator GradualDeboost()
    {
        //if (speedBoostObj != null)
        //{ 
                // decrease speed over time
            float startSpeed = gameController.pathControl.pathSpeed;
            float decreaseTime = initBoostTime;
            //reset time
            float time = 0;
            Debug.Log("revert boost");
            while (time < decreaseTime)
            {
                //decrease speed back to normal speed
                gameController.pathControl.pathSpeed = Mathf.Lerp(startSpeed, baseSpeed, time / decreaseTime);
                time += Time.deltaTime;
                yield return null;
            }
            //set back to original speed
            gameController.pathControl.pathSpeed = baseSpeed;
        //destroy object
        if (speedBoostObj != null)
            Destroy(speedBoostObj);
        //}
        yield return null;
    }
}
