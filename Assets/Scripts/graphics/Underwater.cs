using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class Underwater : MonoBehaviour {
	
	#region public data
	public float m_UnderwaterCheckOffset = 0.001F;	
	public Color envFogColor;
	public Color underwaterFogColor = Color.blue;
	public GameObject underwaterStuff;
	public Material waterBubblesMat;
	
	public float skyFogDensity = 0.005f;
	public float waterFogDensity = 0.05f;
	
	public Color mUnderWaterBubblesColor = new Color(0.27f,0.27f,0.27f,1f); 
	public Color mUpWaterBubblesColor = new Color(0.019607843f,0.019607843f,0.019607843f,1f);
	#endregion
	
	
	#region private data
	private bool wasUnderwater = false;
	#endregion
	
	public bool IsUnderwater(Camera cam) {
		return cam.transform.position.y + m_UnderwaterCheckOffset < transform.position.y ? true : false;	
	}
			
	public void OnWillRenderObject()
	{
		
		Camera cam = Camera.current;
		
		if(IsUnderwater(cam)) 
		{
			if(Camera.main == cam && !cam.gameObject.GetComponent(typeof(UnderwaterEffect)) ) {
					cam.gameObject.AddComponent(typeof(UnderwaterEffect));	
				}
				
				UnderwaterEffect effect = (UnderwaterEffect)cam.gameObject.GetComponent(typeof(UnderwaterEffect));				
				if(effect) {
					effect.enabled = true;					
				}
				
				//Ok some HACK's here 
				GetComponent<Renderer>().sharedMaterial.shader.maximumLOD = 50;	
				
			
				if(!wasUnderwater){
					
					wasUnderwater = true;
							
					//Change fog a little
					RenderSettings.fogDensity = waterFogDensity;
					RenderSettings.fogColor = underwaterFogColor;		
					
					//Change reflection mode					
					WaterMirrorReflection reflScript = (WaterMirrorReflection) GetComponent(typeof(WaterMirrorReflection)); 
					reflScript.m_BackSide = true;
					
					//Enable caustic
					underwaterStuff.SetActiveRecursively(true);
					
					//Enable bubbles
					waterBubblesMat.SetVector("_TintColor", mUnderWaterBubblesColor );													
				}
			
		}
		else{
			
			UnderwaterEffect effect = (UnderwaterEffect)cam.gameObject.GetComponent(typeof(UnderwaterEffect));
				if(effect && effect.enabled) {
					effect.enabled = false;
				}

				//Ok some HACK's here 
				GetComponent<Renderer>().sharedMaterial.shader.maximumLOD = 100;	
				
				if(wasUnderwater){
				
					
					//Change fog a little
					RenderSettings.fogDensity = skyFogDensity;
					RenderSettings.fogColor = envFogColor;
					wasUnderwater = false;
					
					//Change reflection mode					
					WaterMirrorReflection reflScript = (WaterMirrorReflection) GetComponent(typeof(WaterMirrorReflection)); 
					reflScript.m_BackSide = false;
					
					//Disable caustic
					underwaterStuff.SetActiveRecursively(false);
					
					//Disable bubbles				
					waterBubblesMat.SetVector("_TintColor", mUpWaterBubblesColor );
				
				}
			
		}
		
	}
}
