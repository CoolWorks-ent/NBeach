using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBlinkScript : MonoBehaviour
{

    public RectTransform upperBox;
    public RectTransform lowerBox;
    public float speed = 0.70f;
    public int blinkTimes = 3;
    public bool endClosing = false;


    private Vector3 originalUpperPosition;
    private Vector3 originalLowerPosition;
    private Vector3 endUpper;
    private Vector3 endLower;

    private int currentBlink = 1;
    int mFrameCount = 5;

    public enum Action
    {
        Open,
        Close
    };

    void Awake()
    {
        originalUpperPosition = upperBox.position;
        originalLowerPosition = lowerBox.position;
        //EventManager.StartListening("EyeBlink_Open", delegate { Action.Open; });
    }


    void OnEnable()
    {
        //StartCoroutine(blink());
    }

    private IEnumerator BlinkV2()
    {
        float speed = 0.3f;

        while(gameObject.activeSelf)
        {
            for (int i =0; i< mFrameCount; i++)
            {
                yield return new WaitForSeconds(speed * Time.deltaTime);
                SetNewFrame(i);
            }
            //Vector2 eyeShift = new Vector2(Random.Range(-0.04f, 0.04f), Random.Range(-0.04f, 0.04f));
            //eyeRenderer.material.SetTextureOffset("_MainTex", eyeShift);

            yield return new WaitForSeconds(0.25f);

            for (int j = mFrameCount - 1; j >= 0;j--)
            {
                yield return new WaitForSeconds(speed * Time.deltaTime);
                SetNewFrame(j);
            }
        }

        yield return 0;
    }

    private void SetNewFrame(int frameindex)
    {
        Vector2 newOffset = new Vector2(frameindex * (1.0f / mFrameCount), 0);
        //mLidRenderer.material.setTextureOffset("_MainTex", newOffset);
    }

    private IEnumerator blink()
    {
        while (currentBlink <= blinkTimes)
        {
            endUpper = originalUpperPosition;
            endLower = originalLowerPosition;

            endUpper.y += (50 * currentBlink);
            endLower.y -= (50 * currentBlink);

            // open eyelids
            yield return moveEyelids(endUpper, endLower, Action.Open);

            // check if we want to end the blink closing the eyes
            if (currentBlink == blinkTimes && !endClosing)
            {
                originalUpperPosition.y = Screen.height * 2;
                originalLowerPosition.y = -Screen.height;
            }

            // close eyelids
            yield return moveEyelids(originalUpperPosition, originalLowerPosition, Action.Close);

            currentBlink++;
        }
    }

    private IEnumerator moveEyelids(Vector3 upperLid, Vector3 lowerLid, Action action)
    {
        float elapsedTime = 0;

        while (elapsedTime < speed)
        {
            float duration = (elapsedTime / speed);

            if (action == Action.Open)
            {
                upperBox.position = Vector3.Lerp(originalUpperPosition, upperLid, duration);
                lowerBox.position = Vector3.Lerp(originalLowerPosition, lowerLid, duration);
            }
            else
            {
                upperBox.position = Vector3.Lerp(endUpper, upperLid, duration);
                lowerBox.position = Vector3.Lerp(endLower, lowerLid, duration);
            }

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
