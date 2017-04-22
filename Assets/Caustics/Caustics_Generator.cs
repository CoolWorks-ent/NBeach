using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Caustics_Renderer {
	
	public static List<Texture> causticTextures;
	public static List<RenderTexture> renderTextures;
	public static List<Material> causticsMaterials;
	public static List<bool> updates;
	
	
	public static void Setup ( Texture CT,  RenderTexture RT, Material CM ) {
		
		//Debug.Log ( "Setup Caustics Caustics_Renderer" );
		
		if( renderTextures != null ){
			foreach( RenderTexture existingRT in renderTextures ){
				if (RT == existingRT ){
					return;
				}
			}
			renderTextures.Add ( RT );
			causticTextures.Add ( CT );
			causticsMaterials.Add ( CM );
			updates.Add ( false );
		}else{
			renderTextures = new List<RenderTexture>();
			causticTextures = new List<Texture>();
			causticsMaterials = new List<Material>();
			updates = new List<bool>();
			
			renderTextures.Add ( RT );
			causticTextures.Add ( CT );
			causticsMaterials.Add ( CM );
			updates.Add ( false );
		}
		
	}
	
	public static void UpdateTexture ( RenderTexture RT ) {
	
		//Debug.Log ( "Update Caustics Caustics_Renderer" );
		
		int i = 0;
		for( i = 0; i < renderTextures.Count; i++  ){
			if( RT == renderTextures[i] ){
				if( updates[i] == false ){
					Graphics.Blit( causticTextures[i], renderTextures[i], causticsMaterials[i], -1 );
					updates[i] = true;	
				}else{
					continue;
				}
			}
		}
		
	}
	
	public static void Reset () {
	
		//Debug.Log ( "Reset Caustics Caustics_Renderer" );
		
		int i = 0;
		for( i = 0; i < updates.Count; i++  ){
			updates[i] = false;
		}
		
	}

}

public class Caustics_Generator : MonoBehaviour {

	public Texture causticTexture;
	public RenderTexture renderTexture;
	public Material causticsMaterial;

	// Use this for initialization
	void Start () {
		if ( !causticTexture || !renderTexture ){
			//Debug.LogError("A texture or a render texture are missing, assign them.");
		}else{
			//Debug.Log ( "Setup Caustics Caustics_Generator" );
			Caustics_Renderer.Setup( causticTexture, renderTexture, causticsMaterial );
			this.GetComponent<Light>().cookie = renderTexture;
		}
	}
	
	// Update is called once per frame
	void Update () {	
		//Debug.Log ( "Updating Texture Caustics_Generator" );
		Caustics_Renderer.UpdateTexture( renderTexture );
	}
	
	void LateUpdate () {	
		//Debug.Log ( "Reseting Generator Caustics_Generator" );
		Caustics_Renderer.Reset();
	}
}
