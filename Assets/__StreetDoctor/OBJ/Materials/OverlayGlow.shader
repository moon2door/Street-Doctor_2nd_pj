Shader "Custom/OverlayGlow"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _OverlayColor ("Glow Color", Color) = (0, 1, 0.6, 0.5) // 형광 청록, 반투명
        _OverlayStrength ("Overlay Strength", Range(0, 2)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _OverlayColor;
            float _OverlayStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.uv);
                fixed4 overlay = _OverlayColor;
                
                // Overlay 알파값과 strength를 적용해 혼합
                baseColor.rgb = lerp(baseColor.rgb, overlay.rgb, overlay.a * _OverlayStrength);
                return baseColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

