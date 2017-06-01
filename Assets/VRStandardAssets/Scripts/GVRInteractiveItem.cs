using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VRStandardAssets.Utils
{
    //Modified for functionality with GVR SDK
    // This class should be added to any gameobject in the scene
    // that should react to input based on the user's gaze.
    // It contains events that can be subscribed to by classes that
    // need to know about input specifics to this gameobject.
    public class GVRInteractiveItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
    {
        public event Action OnOver;             // Called when the gaze moves over this object
        public event Action OnOut;              // Called when the gaze leaves this object
        public event Action OnClick;            // Called when click input is detected whilst the gaze is over this object.
        public event Action OnDoubleClick;      // Called when double click input is detected whilst the gaze is over this object.
        public event Action OnUp;               // Called when Fire1 is released whilst the gaze is over this object.
        public event Action OnDown;             // Called when Fire1 is pressed whilst the gaze is over this object.


        protected bool m_IsOver;


        public bool IsOver
        {
            get { return m_IsOver; }              // Is the gaze currently over this object?
        }


        // The below functions are called by the VREyeRaycaster when the appropriate input is detected.
        // They in turn call the appropriate events should they have subscribers.
        public void OnPointerEnter(PointerEventData eventData)
        {
            m_IsOver = true;

            if (OnOver != null)
                OnOver();
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            m_IsOver = false;

            if (OnOut != null)
                OnOut();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null)
                OnClick();
        }


        public void DoubleClick()
        {
            if (OnDoubleClick != null)
                OnDoubleClick();
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            if (OnUp != null)
                OnUp();
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (OnDown != null)
                OnDown();
        }
    }
}