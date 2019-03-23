using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenUICanvas : MonoBehaviour {

    GraphicRaycaster raycaster;
    [SerializeField]
    GameObject UIPanel;

    // Use this for initialization
    void Start () {
		
	}

    private void Awake()
    {
        this.raycaster = GetComponent<GraphicRaycaster>();
    }

	
	// Update is called once per frame
	void Update () {
        //show canvas if gamestate is playing
        if (GameController.instance.gameState == GameState.IsPlaying && UIPanel.activeSelf == false)
            UIPanel.gameObject.SetActive(true);
        else if (GameController.instance.gameState == GameState.InMenu && UIPanel.activeSelf == true)
            UIPanel.gameObject.SetActive(false);

        if (Input.GetMouseButtonDown(0))
        {
            //Set up the new Pointer Event
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            pointerData.position = Input.mousePosition;
            this.raycaster.Raycast(pointerData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                if(result.gameObject.name == "PauseButton")
                {
                    Debug.Log("pause button hit");
                    EventManager.TriggerEvent("PauseGame", "PauseGame");
                }
               // Debug.Log("Hit " + result.gameObject.name);

            }
        }
    }
}
