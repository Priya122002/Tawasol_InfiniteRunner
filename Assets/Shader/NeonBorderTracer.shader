Shader "Custom/NeonBorderTracer"
{
    Properties
    {
        _BorderColor ("Border Color (HDR)", Color) = (0, 1.5, 3, 1)
        _BorderThickness ("Border Thickness", Range(0.001, 0.05)) = 0.01
        _GlowIntensity ("Glow Intensity", Range(1, 30)) = 10
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend One One
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
            float _GlowIntensity;

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

            float BorderMask(float2 uv)
            {
                float2 d = min(uv, 1.0 - uv);
                float edge = min(d.x, d.y);
                return smoothstep(_BorderThickness, 0.0, edge);
            }

            half4 frag (Varyings i) : SV_Target
            {
                float border = BorderMask(i.uv);

                float3 glow = _BorderColor.rgb * border * _GlowIntensity;

                return float4(glow, border);
            }

            ENDHLSL
        }
    }
}