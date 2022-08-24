Shader "Mobile/Diffuse Detail" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Detail ("Detail (RGB)", 2D) = "gray" {}
	}
	SubShader {
		
		Pass{
			
			Material{
				Diffuse [_Color]
			}
			
			Lighting Off
			Fog { Mode Off }
			SetTexture [_MainTex] { combine texture * primary Double, texture * primary}
			SetTexture [_Detail] { combine previous * texture Double, previous}
		}

	} 
	FallBack "VertexLit", 2
}
