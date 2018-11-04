using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedEffectAnimator : MonoBehaviour {

    [SerializeField]
    private int m_FrameRate = 30;                  // The number of times per second the image should change.
    [SerializeField]
    private MeshRenderer m_ScreenMesh;             // The mesh renderer who's texture will be changed.
    [SerializeField]
    private Texture[] m_AnimTextures;              // The textures that will be looped through.
    [SerializeField]
    private Image speedFxOverlay;
    [SerializeField]
    Animator animationControl;


    private WaitForSeconds m_FrameRateWait;                         // The delay between frames.
    private int m_CurrentTextureIndex;                              // The index of the textures array.
    private bool m_Playing;                                         // Whether the textures are currently being looped through.
    float fadeTime = 1f;
    Color baseColor;


    private void Awake()
    {
        // The delay between frames is the number of seconds (one) divided by the number of frames that should play during those seconds (frame rate).
        m_FrameRateWait = new WaitForSeconds(1f / m_FrameRate);
        EventManager.StartListening("Player_SpeedBoost", StartAnim);
        EventManager.StartListening("Player_SpeedBoostOff", StopAnim);
        //set to invisible
        speedFxOverlay.color = new Color(speedFxOverlay.color.r,speedFxOverlay.color.g,speedFxOverlay.color.b,0);
        baseColor = speedFxOverlay.color;

        animationControl = GetComponent<Animator>();
    }


    private void OnEnable()
    {

    }


    private void OnDisable()
    {

    }

    private void Update()
    {

    }

    private void Reset(string str)
    {
        if(m_Playing)
        {
            StopCoroutine(PlayTextures());
            speedFxOverlay.gameObject.SetActive(false);
        }
            
    }

    public void StartAnim(string str)
    {
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
        if (m_Playing)
        {
            speedFxOverlay.gameObject.SetActive(true);
            //fade in speed line textures
            float time = 0f;
            while (time < fadeTime)
            {
                speedFxOverlay.color = Color.Lerp(baseColor, new Color(speedFxOverlay.color.r, speedFxOverlay.color.g, speedFxOverlay.color.b, 1), time / fadeTime);
                time += Time.deltaTime;
                yield return null;
            }
        }
        else if(!m_Playing)
        {
            float time = 0f;
            baseColor = speedFxOverlay.color;
            //fade out speed line textures
            while (time < fadeTime)
            {
                speedFxOverlay.color = Color.Lerp(baseColor, new Color(speedFxOverlay.color.r, speedFxOverlay.color.g, speedFxOverlay.color.b, 0), time / fadeTime);
                time += Time.deltaTime;
                yield return null;
            }
            speedFxOverlay.gameObject.SetActive(false);
        }
        // So long as the textures should be playing...
        /*while (m_Playing)
        {
            // Set the texture of the mesh renderer to the texture indicated by the index of the textures array.
            m_ScreenMesh.material.mainTexture = m_AnimTextures[m_CurrentTextureIndex];

            // Then increment the texture index (looping once it reaches the length of the textures array.
            m_CurrentTextureIndex = (m_CurrentTextureIndex + 1) % m_AnimTextures.Length;

            // Wait for the next frame.
            yield return m_FrameRateWait;
        }*/
    }
}
