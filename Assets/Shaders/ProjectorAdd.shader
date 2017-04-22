Shader "Mage Fight/ProjectorAdditive" { 
   Properties { 
      _ShadowTex ("Cookie", 2D) = "" { TexGen ObjectLinear } 
   } 
   Subshader { 
      Pass { 
         ZWrite off 
         Fog { Color (0, 0, 0) } 		
         //ColorMask RGB 
         Blend One One 
         SetTexture [_ShadowTex] { 
            combine texture, ONE - texture 
            Matrix [_Projector] 
         }
      }
   }
}