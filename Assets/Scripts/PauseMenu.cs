using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// PauseMenu is shown when player presses the pause button.
/// </summary>

public class PauseMenu : MonoBehaviour {

    [SerializeField]
    GameObject pauseMenuPanel;
    [SerializeField]
    GameObject pauseMenuCanvas;

    Vector3 baseScale;
    Vector3 basePosition;
    Quaternion baseRotation;

    Transform m_UIElement;

    // Use this for initialization
    void Start()
    {

        EventManager.StartListening("PauseGame", OnGamePaused);
        EventManager.StartListening("LevelLoaded", ResetMenuPos);        
    }

    private void Awake()
    {
        EventManager.StartListening("PauseGame", OnGamePaused);
        //EventManager.StartListening("LevelLoaded", ResetMenuPos);
        m_UIElement = this.gameObject.transform;

        baseScale = this.gameObject.transform.localScale;
        basePosition = this.gameObject.transform.localPosition;
        baseRotation = this.gameObject.transform.localRotation;

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //ResetMenuPos("pos");
    }

    //set camera again
    public void ResetMenuPos(string pos)
    {
        Camera camera = GameController.instance.playerControl.mainCamera;
        //this.GetComponent<Canvas>().worldCamera = GameController.instance.playerControl.mainCamera;
        pauseMenuCanvas.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        //set the menu to be a parent of the current playerController object to center it on screen and make the position update properly
        //this.gameObject.transform.SetParent(GameController.instance.playerControl.transform);
       // m_UIElement.SetParent(GameController.instance.playerControl.mainCamera.transform);

        //Reset ALL TRANSFORMS to be in front of player
        m_UIElement.localScale = baseScale;
        m_UIElement.localPosition = basePosition;
        m_UIElement.localRotation = baseRotation;
    }

    // Update is called once per frame
    void Update()
    {
        float m_FollowSpeed = 2;
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        Transform m_Camera = Camera.main.transform; //GameController.instance.playerControl.mainCamera;
        Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward);
        //m_UIElement.position = rayOrigin + (fwd * 30f);
        //m_UIElement.rotation = new Quaternion(0, m_Camera.rotation.y, 0,m_Camera.rotation.w);
        //this.gameObject.transform.Translate(Vector3.forward * 2, Space.Self);
        //this.gameObject.transform.localRotation = camera.transform.localRotation;

        // Find the direction the camera is looking but on a flat plane.
        Vector3 targetDirection = Vector3.ProjectOnPlane(m_Camera.forward, Vector3.up).normalized;

        // Calculate a target position from the camera in the direction at the same distance from the camera as it was at Start.
        Vector3 targetPosition = m_Camera.position + targetDirection * 30f;
        Quaternion targetRotation = m_Camera.rotation;

        // Set the target position  to be an interpolation of itself and the UI's position.
        targetPosition = Vector3.Lerp(m_UIElement.position, targetPosition, m_FollowSpeed * Time.deltaTime);
        targetRotation = Quaternion.Lerp(m_UIElement.rotation, targetRotation, m_FollowSpeed * Time.deltaTime);

        // Since the UI is only following on the XZ plane, negate any y movement.
        targetPosition.y = m_UIElement.position.y;

        // Set the UI's position to the calculated target position.
        m_UIElement.position = targetPosition;
        m_UIElement.rotation = targetRotation;
        
    }

    //When game paused, display pause menu
    void OnGamePaused(string levelNum)
    {
        //ResetMenuPos("pos");

        pauseMenuPanel.SetActive(true);
    }

    public void LoadOnClick(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void LoadNextLevel()
    {
        //show any extra HUD stuff
        EventManager.TriggerEvent("MenuSelect_LoadNextLevel", "");
        //GameController.instance.lvlManager.LoadNextLevel();
    }

    public void ResumeGame()
    {
        EventManager.TriggerEvent("MenuSelect_ResumeGame", "");
        pauseMenuPanel.SetActive(false);
    }

    //Tell game to load main menu and quit level
    public void ReturnToMainMenu()
    {
        EventManager.TriggerEvent("MenuSelect_ReturnToTitle", "");
        SceneManager.LoadScene(0);
    }

}

