// - specular lighting directions are approximated per vertex
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "MapBackground" {
Properties {
	_MainTex ("Nebula (RGB)", 2D) = "black" {}
	_DecalTex ("Stars Detail (RGB)", 2D) = "black" {}
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;
sampler2D _DecalTex;

struct Input {
	float2 uv_MainTex;
	float2 uv_DecalTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex)*0.5;
	fixed4 decal = tex2D(_DecalTex, IN.uv_DecalTex);
	c.rgb += decal.rgb;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
