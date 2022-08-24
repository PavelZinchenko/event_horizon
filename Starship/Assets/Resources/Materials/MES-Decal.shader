// - specular lighting directions are approximated per vertex
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "MES/Decal" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_DecalTex ("Decal (RGBA)", 2D) = "black" {}
	//_Color ("Main Color", Color) = (1,1,1,1)
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;
sampler2D _DecalTex;
//fixed4 _Color;

struct Input {
	float2 uv_MainTex;
	float2 uv_DecalTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 decal = tex2D(_DecalTex, IN.uv_DecalTex);
	c.rgb = lerp(c.rgb, decal.rgb, decal.a);// * _Color.rgb;
	o.Albedo = c.rgb;// * _Color.a;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
