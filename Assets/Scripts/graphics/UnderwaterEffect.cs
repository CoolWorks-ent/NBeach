using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UnderwaterEffect : MonoBehaviour {

	public float m_UnderwaterColorFade = 0.125F;
	public Shader m_UnderwaterShader;
	//public Water3 m_Water;
	
	//public Color m_BlendColor = Color.blue;
	public Color m_BlendColor = new Color(0.086f, 0.216f, 0.714f);
	
	private Material m_UnderwaterMaterial;

	// Use this for initialization
	void Start () {
		if(m_UnderwaterShader) {
			m_UnderwaterMaterial = new Material(m_UnderwaterShader);
			//m_UnderwaterMaterial.hideFlags = HideFlags.HideAndDontSave;	
		}
	}
	
	// Update is called once per frame
	void OnRenderImage(RenderTexture source, RenderTexture destination) 
	{
		//m_BlendColor = Water3Manager.Instance().GetMaterialColor("_RefrColorDepth");
		
		RenderTexture temp = RenderTexture.GetTemporary(source.width,source.height);
		
		m_UnderwaterMaterial.SetColor("_DepthColor", m_BlendColor);
		m_UnderwaterMaterial.SetFloat("_UnderwaterColorFade", m_UnderwaterColorFade);
		
		m_UnderwaterMaterial.SetVector("offsets", new Vector4(1.0F,0.0F,0.0F,0.0F));
		Graphics.Blit(source, temp, m_UnderwaterMaterial,0);
		m_UnderwaterMaterial.SetVector("offsets", new Vector4(0.0F,1.0F,0.0F,0.0F));
		Graphics.Blit(temp, destination, m_UnderwaterMaterial,0);
		
		RenderTexture.ReleaseTemporary(temp);		
	}
}
