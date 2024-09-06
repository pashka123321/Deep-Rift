Shader "Hidden/UnderwaterEffect"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.1
        _TintColor ("Tint Color", Color) = (0.0, 0.4, 0.7, 1.0)
    }
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _DistortionStrength;
            float4 _TintColor;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Добавляем небольшое смещение для эффекта искажения
                float2 distortion = (tex2D(_MainTex, uv + _MainTex_TexelSize.xy * _DistortionStrength) - 0.5) * _DistortionStrength;
                uv += distortion;

                fixed4 col = tex2D(_MainTex, uv);

                // Применяем цветовой оттенок
                col.rgb = lerp(col.rgb, _TintColor.rgb, 0.5);

                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
