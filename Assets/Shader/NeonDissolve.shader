Shader "Custom/NeonDissolve"
{
    Properties
    {
        // ===== BASE NEON =====
        _BaseColor ("Base Color", Color) = (0.15, 0.05, 0.05, 1)
        _GlowColor ("Glow Color (HDR)", Color) = (2.5, 0.2, 0.2, 1)
        _GlowIntensity ("Glow Intensity", Range(0,10)) = 4

        _PulseSpeed ("Pulse Speed", Range(0,5)) = 2
        _PulseAmount ("Pulse Amount", Range(0,2)) = 0.5

        // ===== DISSOLVE =====
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _EdgeWidth ("Dissolve Edge Width", Range(0,0.2)) = 0.08
        _DissolveEdgeColor ("Dissolve Edge Color (HDR)", Color) = (4, 8, 16, 1)
        _DissolveGlow ("Dissolve Glow", Range(0,20)) = 8

        _NoiseTex ("Dissolve Noise", 2D) = "white" {}
    }

    SubShader
    {
        // ✅ IMPORTANT FIX
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
        Cull Off
        ZWrite On

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _BaseColor;
            float4 _GlowColor;
            float _GlowIntensity;
            float _PulseSpeed;
            float _PulseAmount;

            float _DissolveAmount;
            float _EdgeWidth;
            float4 _DissolveEdgeColor;
            float _DissolveGlow;

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float2 uv          : TEXCOORD1;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // ===== BASE NEON LOOK =====
                float edge = 1 - abs(normalize(i.normalWS).y);
                float pulse = sin(_Time.y * _PulseSpeed) * _PulseAmount + 1;

                float3 color =
                    _BaseColor.rgb +
                    (_GlowColor.rgb * edge * _GlowIntensity * pulse);

                // ===== DISSOLVE =====
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.uv).r;
                float dissolve = noise - _DissolveAmount;

                // ✅ REAL dissolve
                clip(dissolve);

                // glowing dissolve edge
                float dissolveEdge = smoothstep(_EdgeWidth, 0.0, dissolve);
                color += _DissolveEdgeColor.rgb * dissolveEdge * _DissolveGlow;

                return float4(color, 1);
            }
            ENDHLSL
        }
    }
}
