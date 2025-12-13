Shader "Custom/NeonScreenBorder"
{
    Properties
    {
        _BorderColor ("Border Color (HDR)", Color) = (0, 1, 2, 1)
        _BorderThickness ("Border Thickness", Range(0.001, 0.2)) = 0.02
        _GlowStrength ("Glow Strength", Range(1, 100)) = 25
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend One One    // Additive glow
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _BorderColor;
            float _BorderThickness;
            float _GlowStrength;

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

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            // Perfect uniformly thick border
            float borderMask(float2 uv, float thickness)
            {
                float left   = step(uv.x, thickness);
                float right  = step(1.0 - uv.x, thickness);
                float top    = step(1.0 - uv.y, thickness);
                float bottom = step(uv.y, thickness);

                return max(max(left, right), max(top, bottom));
            }

            half4 frag(Varyings i) : SV_Target
            {
                float border = borderMask(i.uv, _BorderThickness);

                float glow = border * _GlowStrength;

                return float4(_BorderColor.rgb * glow, glow);
            }

            ENDHLSL
        }
    }
}