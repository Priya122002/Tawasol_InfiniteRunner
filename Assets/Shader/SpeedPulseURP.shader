Shader "Custom/SpeedPulseURP"
{
    Properties
    {
        _Intensity ("Intensity", Range(0,1)) = 0
        _Color ("Speed Color", Color) = (0, 0.8, 1, 1)

        _LineDensity ("Line Density", Range(10,50)) = 25
        _FlowSpeed ("Flow Speed", Range(0,10)) = 4
        _CenterBoost ("Center Boost", Range(0,2)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Overlay"
            "RenderType"="Transparent"
        }

        Pass
        {
            ZWrite Off
            ZTest Always
            Blend SrcAlpha One

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

            float _Intensity;
            float4 _Color;
            float _LineDensity;
            float _FlowSpeed;
            float _CenterBoost;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv - 0.5;   // Centered UV
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                float dist = length(uv);

                // Direction from center
                float2 dir = normalize(uv + 0.0001);

                // Radial speed lines (center → outward)
                float flow =
                    sin(dot(dir, float2(1, 0)) * _LineDensity
                    + _Time.y * _FlowSpeed);

                flow = saturate(flow);

                // Stronger in center
                float centerFade = 1.0 - smoothstep(0.0, 0.7, dist);
                centerFade = pow(centerFade, 1.5) * _CenterBoost;

                // Fade near edges
                float edgeFade = smoothstep(0.1, 0.8, dist);

                float alpha =
                    flow *
                    centerFade *
                    edgeFade *
                    _Intensity;

                return half4(_Color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
