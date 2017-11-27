using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class UnderwaterScript : MonoBehaviour {

	public GameObject waterPlane;
	public bool isUnderwater;
	Color normalColor;
    //set in unity editor
	public Color underwaterColor;
    public Color underwaterFogColor;
    Color underwaterLighting;
    Camera mainCamera;
    private bool defaultFog;
    private Color defaultFogColor;
    private float defaultFogDensity;
    private Material defaultSkybox;
    private Color defaultLighting;
    private Material noSkybox;
    GlobalFog myFog;
    ScreenOverlay camTint;

    bool canSwim = false;
	bool underGround = false;
	float groundLevel;
    float camTintUnderwater = 0.3f;

	void Start() 
	{
        //Set the background color
        mainCamera = GetComponent<Camera>();

        //use these in the future for full game, these settings represent whatever the fog settings are before entering water
        defaultFog = RenderSettings.fog;
        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;
        defaultSkybox = RenderSettings.skybox;
        defaultLighting = RenderSettings.ambientLight;
        normalColor = new Color (0.5f, 0.5f, 0.5f, 0.5f);
        underwaterLighting = new Color(0.1544117f, 0.5645437f, 0.7f);
        myFog = mainCamera.GetComponent<GlobalFog>();
        camTint = mainCamera.GetComponent<ScreenOverlay>();

    }

    void Update()
    {
        //if player under water plane, display water effects
        //if not already underwater, check if player under the water plane.  if in water, check if player above water plane.
        /*if ((transform.position.y > waterPlane.transform.position.y) != isUnderwater) 
		{
			isUnderwater = transform.position.y < waterPlane.transform.position.y;
			if (isUnderwater) SetUnderwater ();
			if (!isUnderwater) SetNormal ();
		}*/

         if (transform.position.y < waterPlane.transform.position.y)
         {
             isUnderwater = true;
             SetUnderwater();
            Debug.Log("underwater");
         }
        else
        {
            isUnderwater = false;
            SetNormal();
            Debug.Log("above water");
        }
                
        /*if(transform.position.y < groundLevel)
		{
			canSwim = true;
			underGround = true;
		}

		else
		{
			underGround = false;
		}*/

        if (isUnderwater && canSwim == true && underGround == false && Input.GetKey(KeyCode.E))
		{
            //GetComponent<ConstantForce>().relativeForce = new Vector3(0,-200, 0);
		}
		else
		{
            //GetComponent<ConstantForce>().relativeForce = new Vector3(0, 0, 0);
		}

		if(isUnderwater && canSwim == true && Input.GetKey(KeyCode.Q))
		{
           // GetComponent<ConstantForce>().relativeForce = new Vector3(0, 200, 0);
		}
	}

	void SetNormal() 
	{
        myFog.enabled = false;
        RenderSettings.fogColor = defaultFogColor;
        RenderSettings.fogDensity = defaultFogDensity;
        RenderSettings.ambientLight = defaultLighting;
        RenderSettings.skybox = defaultSkybox;
        mainCamera.backgroundColor = normalColor;

        //set skybox to show
        mainCamera.clearFlags = CameraClearFlags.Skybox;
        //set gravity and speed when player is underwater
        canSwim = false;

        //set opacity intensity higher to remove underwater tint on camera
        camTint.intensity = 0;
	}

	void SetUnderwater() 
	{
        //RenderSettings.fog = true;
        myFog.enabled = true;
        RenderSettings.fogColor = underwaterFogColor;
        RenderSettings.ambientLight = underwaterLighting;
        RenderSettings.skybox = null;
        //RenderSettings.fogDensity = 0.04f;
        
        //set skybox to not show
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = underwaterFogColor;

        //set opacity intensity higher to show tint on camera while in water
        camTint.intensity = camTintUnderwater;
        //set gravity and speed when player is underwater
    }
}
