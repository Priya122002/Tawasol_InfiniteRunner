Shader "Custom/NeonDissolve"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (0.1, 0.6, 1, 1)
        _EdgeColor ("Glow Edge Color", Color) = (4, 8, 16, 1)

        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _EdgeWidth ("Edge Width", Range(0,0.2)) = 0.08

        _GlowIntensity ("Glow Intensity", Range(0,20)) = 8

        _NoiseTex ("Dissolve Noise", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend One One

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _MainColor;
            float4 _EdgeColor;

            float _DissolveAmount;
            float _EdgeWidth;
            float _GlowIntensity;

            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
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

            half4 frag(Varyings i) : SV_Target
            {
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.uv).r;

                float dissolve = noise - _DissolveAmount;

                // edge highlight
                float edge = smoothstep(0.0, _EdgeWidth, dissolve);

                float3 col = _MainColor.rgb * edge;

                float glowEdge = smoothstep(_EdgeWidth, 0.0, dissolve);
                col += _EdgeColor.rgb * glowEdge * _GlowIntensity;

                return float4(col, 1);
            }

            ENDHLSL
        }
    }
}
