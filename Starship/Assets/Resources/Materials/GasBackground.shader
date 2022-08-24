// - specular lighting directions are approximated per vertex
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "GasBackground" {
Properties {
	_MainTex ("Ground (RGB)", 2D) = "black" {}
	_DecalTex ("Ground Detail (RGB)", 2D) = "black" {}
	_CloudsTex("Clouds (RGB)", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1)
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;
sampler2D _DecalTex;
//uniform float4 _DecalTex_ST;
sampler2D _CloudsTex;
//uniform float4 _CloudsTex_ST;
fixed4 _Color;

struct Input {
	float2 uv_MainTex;
	float2 uv_DecalTex;
	float2 uv_CloudsTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 ground = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 detail = tex2D(_DecalTex, IN.uv_DecalTex/*IN.uv_MainTex * _DecalTex_ST.xy*/);
	fixed4 clouds = tex2D(_CloudsTex, IN.uv_CloudsTex/*IN.uv_MainTex * _CloudsTex_ST.xy*/);

	o.Albedo = _Color*0.5 + (ground.rgb*0.5 + 0.33*detail.rgb*_Color + 0.33*clouds.rgb*_Color)*0.5;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
