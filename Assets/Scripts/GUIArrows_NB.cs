using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
    // This class fades in and out arrows which indicate to
    // the player which direction they should be facing.
    public class GUIArrows_NB : MonoBehaviour
    {
        [SerializeField] private float m_FadeDuration = 0.5f;       // How long it takes for the arrows to appear and disappear.
        [SerializeField] private float m_ShowAngle = 60f;           // How far from the desired facing direction the player must be facing for the arrows to appear.
        [SerializeField] private Transform m_DesiredDirection;      // Indicates which direction the player should be facing (uses world space forward if null).
        [SerializeField] private Transform m_Camera;                // Reference to the camera to determine which way the player is facing.
        [SerializeField] private Image[] m_ArrowRenderers;       // Reference to the renderers of the arrows used to fade them in and out.


        private float m_CurrentAlpha;                               // The alpha the arrows currently have.
        private float m_TargetAlpha;                                // The alpha the arrows are fading towards.
        private float m_FadeSpeed;                                  // How much the alpha should change per second (calculated from the fade duration).
        private GameObject player;
        private GameObject incomingObject;
        private List<bool> activeArrows;

        private const string k_MaterialPropertyName = "_Alpha";     // The name of the alpha property on the shader being used to fade the arrows.

    public void Start()
    {

    }



	    public void Load ()
	    {
            // Speed is distance (zero alpha to one alpha) divided by time (duration).
            m_FadeSpeed = 1f / m_FadeDuration;
            player = GameObject.Find("PlayerCube").gameObject;
            activeArrows = new List<bool> { false, false };
            m_ArrowRenderers[0].color = new Color(m_ArrowRenderers[0].color.r, m_ArrowRenderers[0].color.g, m_ArrowRenderers[0].color.b, 0);//left
            m_ArrowRenderers[1].color = new Color(m_ArrowRenderers[0].color.r, m_ArrowRenderers[0].color.g, m_ArrowRenderers[0].color.b, 0);//right

    }


        private void Update()
        {
            if(gameObject.activeSelf)
            {
                //check if incoming object is within player's field of view
                Vector3 screenPoint = Camera.main.WorldToViewportPoint(incomingObject.transform.position); //convert object position to viewport space
                //Vector3 fullScreen = Camera.main.ViewportToWorldPoint(new Vector3(Screen.width, Screen.height, 10));
            bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.z > 0 && screenPoint.y > 0 && screenPoint.y < 1; //incomingObject.transform.position.y > screenPoint.y && incomingObject.transform.position.z < screenPoint.z;

            Debug.Log("point - " + screenPoint);
                //after the player sees the object once, turn off the arrows for that object. The player now knows the object exists
                if (onScreen)
                {
                    //measure distance of object to player and increase animation speed as it gets closer
                    //Debug.Log("offscreen");
                    if (screenPoint.x > 0.5) //right
                        Hide(1);
                    else if (screenPoint.x < 0.5)
                        Hide(0); //left
                    else
                    {
                        Hide(1);
                        Hide(0);
                    }
                }
                    


        }

            /*// The vector in which the player should be facing is the forward direction of the transform specified or world space.
            Vector3 desiredForward = m_DesiredDirection == null ? Vector3.forward : m_DesiredDirection.forward;

            // The forward vector of the camera as it would be on a flat plane.
            Vector3 flatCamForward = Vector3.ProjectOnPlane(m_Camera.forward, Vector3.up).normalized;

            // The difference angle between the desired facing and the current facing of the player.
            float angleDelta = Vector3.Angle (desiredForward, flatCamForward);

            // If the difference is greater than the angle at which the arrows are shown, their target alpha is one otherwise it is zero.
            m_TargetAlpha = angleDelta > m_ShowAngle ? 1f : 0f;

            // Increment the current alpha value towards the now chosen target alpha and the calculated speed.
            m_CurrentAlpha = Mathf.MoveTowards (m_CurrentAlpha, m_TargetAlpha, m_FadeSpeed * Time.deltaTime);

            // Go through all the arrow renderers and set the given property of their material to the current alpha.
            for (int i = 0; i < m_ArrowRenderers.Length; i++)
            {
                m_ArrowRenderers[i].material.SetFloat(k_MaterialPropertyName, m_CurrentAlpha);
            }*/
        }


        // Turn off the arrows entirely.
        public void Hide(int arrow)
        {
            if(activeArrows[arrow])
            {
                activeArrows[arrow] = false;
                FadeOut(arrow); //fade out
            }
            

        //check if both arrows are false, then deactivate gameobject
            gameObject.SetActive(false);
        }


        // Turn the arrows on.
        public void Show (int arrow, GameObject iObject)
        {
            
            gameObject.SetActive(true);
            //call function for left or right arrow, fade in, start animation
            //set this object to active if it is not already
            if (activeArrows[arrow] == false)
            {
                activeArrows[arrow] = true;
                FadeIn(arrow);
            }
            incomingObject = iObject;
            
        }

        //fade in specified arrow
        void FadeIn(int arr)
        {
            // If the difference is greater than the angle at which the arrows are shown, their target alpha is one otherwise it is zero.
            m_TargetAlpha = 1;
            m_CurrentAlpha = m_ArrowRenderers[arr].color.a;
            float time = 0;
            
            while (time < m_FadeDuration)
            {
                // Increment the current alpha value towards the now chosen target alpha and the calculated speed.
                m_CurrentAlpha = Mathf.MoveTowards(m_CurrentAlpha, m_TargetAlpha, m_FadeSpeed * Time.deltaTime);
                m_ArrowRenderers[arr].color = new Color(m_ArrowRenderers[arr].color.r, m_ArrowRenderers[arr].color.g, m_ArrowRenderers[arr].color.b, m_CurrentAlpha);
                time += Time.deltaTime;
            }
        }

        //fade out specified arrow
        void FadeOut(int arr)
        {
            // If the difference is greater than the angle at which the arrows are shown, their target alpha is one otherwise it is zero.
            m_TargetAlpha = 1;
            m_CurrentAlpha = m_ArrowRenderers[arr].color.a;
            float time = 0;

            while (time < m_FadeDuration)
            {
                // Increment the current alpha value towards the now chosen target alpha and the calculated speed.
                m_CurrentAlpha = Mathf.MoveTowards(m_CurrentAlpha, m_TargetAlpha, m_FadeSpeed * Time.deltaTime);
                m_ArrowRenderers[arr].color = new Color(m_ArrowRenderers[arr].color.r, m_ArrowRenderers[arr].color.g, m_ArrowRenderers[arr].color.b, m_CurrentAlpha);
                time += Time.deltaTime;
            }
        }
}