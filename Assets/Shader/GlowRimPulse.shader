Shader "Custom/GlowRimPulse"
{
    Properties
    {
        _ColorTop ("Top Fill Color (HDR)", Color) = (0.2, 0.9, 1, 1)
        _ColorBottom ("Bottom Fill Color (HDR)", Color) = (0, 0.4, 1, 1)

        _EdgeColor ("Edge Color (HDR)", Color) = (6, 12, 25, 1)

        _EdgeThickness ("Edge Thickness", Range(0.001, 0.1)) = 0.02
        _EdgeIntensity ("Edge Glow Intensity", Range(1,50)) = 25

        _FillIntensity ("Fill Glow Intensity", Range(1,40)) = 12
    }

    SubShader
    {
        Tags{ "Queue"="Transparent+10" "RenderType"="Transparent" }
        Blend One One
        ZWrite On

        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _ColorTop;
            float4 _ColorBottom;

            float4 _EdgeColor;

            float _EdgeThickness;
            float _EdgeIntensity;

            float _FillIntensity;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // -------------------------
                // FILL COLOR (GRADIENT)
                // -------------------------

                float fillLerp = i.uv.y;
                float3 fillBase = lerp(_ColorBottom.rgb, _ColorTop.rgb, fillLerp);

                float3 fillGlow = fillBase * _FillIntensity;

                // -------------------------
                // OUTLINE DETECTION
                // -------------------------

                float edgeX = min(i.uv.x, 1 - i.uv.x);
                float edgeY = min(i.uv.y, 1 - i.uv.y);

                float edgeDist = min(edgeX, edgeY);

                float outline = smoothstep(_EdgeThickness, 0.0, edgeDist);

                float3 borderGlow = _EdgeColor.rgb * outline * _EdgeIntensity;

                // -------------------------
                // FINAL COLOR (Additive)
                // -------------------------

                float3 finalColor = fillGlow + borderGlow;

                return float4(finalColor, 1);
            }

            ENDHLSL
        }
    }
}