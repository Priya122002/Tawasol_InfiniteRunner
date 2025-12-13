Shader "Custom/NeonLaneGlow"
{
    Properties
    {
        _GlowColor ("Glow Color (HDR)", Color) = (0.2, 0.8, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(1,300)) = 150

        _FillIntensity ("Inner Fill Intensity", Range(0,100)) = 20

        _LineWidth ("Line Width", Range(0.001, 0.5)) = 0.10
        _Smoothness ("Edge Smoothness", Range(0.001, 0.5)) = 0.08

        _LeftOffset ("Left Line Center", Range(0,0.5)) = 0.15
        _LeftSpacing ("Left Pair Spacing", Range(0,0.2)) = 0.04

        _RightOffset ("Right Line Center", Range(0.5,1)) = 0.85
        _RightSpacing ("Right Pair Spacing", Range(0,0.2)) = 0.04

        _VerticalFade ("Fade Into Horizon", Range(0,2)) = 1
    }

    SubShader
    {
        Tags{ "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend One One
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _GlowColor;
            float _GlowIntensity;
            float _FillIntensity;

            float _LineWidth;
            float _Smoothness;

            float _LeftOffset;
            float _LeftSpacing;

            float _RightOffset;
            float _RightSpacing;

            float _VerticalFade;

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings  { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float Glow(float x, float center, float width, float smooth)
            {
                float dist = abs(x - center);
                return 1 - smoothstep(width, width - smooth, dist);
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv;

                float L1 = Glow(uv.x, _LeftOffset - _LeftSpacing, _LineWidth, _Smoothness);
                float L2 = Glow(uv.x, _LeftOffset + _LeftSpacing, _LineWidth, _Smoothness);

                float R1 = Glow(uv.x, _RightOffset - _RightSpacing, _LineWidth, _Smoothness);
                float R2 = Glow(uv.x, _RightOffset + _RightSpacing, _LineWidth, _Smoothness);

                float lineGlow = (L1 + L2 + R1 + R2);

                float innerFill = pow(lineGlow, 2.0) * _FillIntensity;

                float horizon = lerp(1.0, 0.2, uv.y * _VerticalFade);

                float totalGlow = (lineGlow * _GlowIntensity + innerFill) * horizon;

                float3 finalColor = _GlowColor.rgb * totalGlow;

                return float4(finalColor, 1);
            }
            ENDHLSL
        }
    }
}