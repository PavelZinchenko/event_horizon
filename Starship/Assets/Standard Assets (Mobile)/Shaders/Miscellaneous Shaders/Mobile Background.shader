Shader "Mobile/Background" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
    Pass {
    	Tags {"Queue" = "Background" }
        Material { }
        Lighting Off
        Zwrite off
        Fog { Mode Off }
        SetTexture [_MainTex] {
            Combine texture 
        }

    }
}
}