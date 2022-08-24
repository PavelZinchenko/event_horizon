Shader "Sprites/Outline"
{
Properties{
	[PerRendererData] _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_Color("Tint", Color) = (1,1,1,1)
	_Thickness("Thickness", Float) = 5
}

SubShader{
	Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

	Cull Off
	Lighting Off
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha

	Pass{
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag			
		#include "UnityCG.cginc"

		struct appdata_t {
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 vertex  : SV_POSITION;
			half2 texcoord : TEXCOORD0;
		};

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		fixed4 _Color;
		half _Thickness;

		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.texcoord = v.texcoord;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
            half x = i.texcoord.x;
			half y = i.texcoord.y;
			half dx = _MainTex_TexelSize.x * _Thickness;
			half dy = _MainTex_TexelSize.y * _Thickness;

			half left = tex2D(_MainTex, half2(x - dx, y)).a;
			half right = tex2D(_MainTex, half2(x + dx, y)).a;
			half top = tex2D(_MainTex, half2(x, y - dx)).a;
			half bottom = tex2D(_MainTex, half2(x, y + dx)).a;
			half center = tex2D(_MainTex, half2(x, y)).a;

			half a1 = max(max(max(left, right), max(top, bottom)) - center, 0) * 0.5;
			half a2 = max(center - top*bottom*left*right, 0) * 0.9;

			half4 c = _Color;
			c.a *= max(a1,a2);
			return c;
		}

		ENDCG
	}
}
}
