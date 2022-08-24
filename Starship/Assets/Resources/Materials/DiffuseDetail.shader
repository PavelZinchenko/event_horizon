// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Sprites/Detail"
{
Properties {
	_Color ("Tint", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Detail ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags
	{ 
		"Queue"="Transparent" 
		"IgnoreProjector"="True" 
		"RenderType"="Transparent" 
		"PreviewType"="Plane"
		"CanUseSpriteAtlas"="True"
	}
	
	Cull Off
	Lighting Off
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD1;
			};

			struct v2f {
				float4 vertex  : SV_POSITION;
				float4 color   : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _Detail;
			fixed4 _Color;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color*_Color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.texcoord) * i.color;
				//c.rgb *= tex2D(_Detail,i.texcoord).rgb * unity_ColorSpaceDouble.r;
				c *= tex2D(_Detail,i.texcoord).r * unity_ColorSpaceDouble.r;
				return c;
			}
		ENDCG
	}
}

}
