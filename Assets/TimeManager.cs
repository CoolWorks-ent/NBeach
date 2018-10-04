using UnityEngine;

/*****************************
 * TIME MANAGER SCRIPT
 * This script contains function calls to adjust in-game time
 * Primarily used for a slow-motion effect in game. 
 *****************************/ 
public class TimeManager : MonoBehaviour {


    float slowDownFactor = 0.08f; //the smaller, the slower
    float slowDownLength = 10; //seconds for time slow down
    float recoverTime = 1; //timer it takes to return to normal time
    public bool slowMoOn = false;
    float slowDownTimer = 0f;

    private void Update()
    {
        //Slowly adds time back to the timescale for the scaleDownLength to return time to normal
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1); //clamp the timeScale value to prevent it from becoming to high
        if (slowMoOn)
        {
            slowDownTimer += Time.unscaledDeltaTime;
            if (slowDownTimer > slowDownLength)
            {                
                Time.timeScale += (1 / recoverTime) * Time.unscaledDeltaTime; //unscaled is not affected by the timescale, so the equation stays consistent           
                if (Time.timeScale >= 1)
                {
                    Time.fixedDeltaTime = Time.timeScale * .02f;
                    slowMoOn = false;
                    Debug.Log("SlowMo Off!");
                }
            }
        }
    }
    public void DoSlowMo(float? slowMoLength)
    {
        //if timescale = .5, time will move 2 times faster than real time.
        Time.timeScale = slowDownFactor; //the higher the timescale, the slower time is in-game
        Debug.Log("SlowMo On!");

        //Adjust FixedDeltaTime to match the timeScale change. we want 50 frames per second for our FixedDeltaTime
        Time.fixedDeltaTime = Time.timeScale * .02f; //1/50
        slowMoOn = true;
        slowDownTimer = 0;
        if (slowMoLength != null)
            slowDownLength = (float)slowMoLength;
    }
}
