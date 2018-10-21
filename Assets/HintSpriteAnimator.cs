using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintSpriteAnimator : MonoBehaviour {

    [SerializeField]
    private int m_FrameRate = 20;                  // The number of times per second the image should change.
    [SerializeField]
    private Texture[] m_AnimTextures;              // The textures that will be looped through.
    [SerializeField]
    private Image hintOverlay;
    [SerializeField]
    Animator animationControl;

    private WaitForSeconds m_FrameRateWait;                         // The delay between frames.
    private int m_CurrentTextureIndex;                              // The index of the textures array.
    private bool m_Playing;                                         // Whether the textures are currently being looped through.
    float fadeTime = .5f;
    Color baseColor;

    private void Awake()
    {
        // The delay between frames is the number of seconds (one) divided by the number of frames that should play during those seconds (frame rate).
        m_FrameRateWait = new WaitForSeconds(1f / m_FrameRate);
        EventManager.StartListening("Player_TiltHintOn", StartAnim);
        EventManager.StartListening("Player_TiltHintOff", StopAnim);
        hintOverlay.color = new Color(hintOverlay.color.r, hintOverlay.color.g, hintOverlay.color.b, 0);
        baseColor = hintOverlay.color;
        animationControl = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Reset(string str)
    {
        if (m_Playing)
        {
            StopCoroutine(PlayTextures());
            hintOverlay.gameObject.SetActive(false);
        }

    }

    public void StartAnim(string str)
    {
        hintOverlay.gameObject.SetActive(true);
        m_Playing = true;
        StartCoroutine(PlayTextures());
        animationControl.SetBool("IsPlaying", true);
    }


    public void StopAnim(string str)
    {
        m_Playing = false;
        StartCoroutine(PlayTextures());
        animationControl.SetBool("IsPlaying", false);
    }


    private IEnumerator PlayTextures()
    {
        //m_Playing determines if animation should play or stop playing
        //fadeIn
        if (m_Playing)
        {
            hintOverlay.gameObject.SetActive(true);
            //fade in speed line textures
            float time = 0f;
            while (time < fadeTime)
            {
                hintOverlay.color = Color.Lerp(baseColor, new Color(hintOverlay.color.r, hintOverlay.color.g, hintOverlay.color.b, 1), time / fadeTime);
                time += Time.deltaTime;
                yield return null;
            }
        }
        //fadeout
        else if (!m_Playing)
        {
            float time = 0f;
            baseColor = hintOverlay.color;
            //fade out speed line textures
            while (time < fadeTime)
            {
                hintOverlay.color = Color.Lerp(baseColor, new Color(hintOverlay.color.r, hintOverlay.color.g, hintOverlay.color.b, 0), time / fadeTime);
                time += Time.deltaTime;
                yield return null;
            }
            //hintOverlay.gameObject.SetActive(false);
        }
    }
}
