Shader "Custom/NeonObstacle"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.1, 0, 0, 1)
        _GlowColor ("Glow Color (HDR)", Color) = (2.5, 0.2, 0.2, 1)
        _GlowIntensity ("Glow Intensity", Range(0,10)) = 4

        _PulseSpeed ("Pulse Speed", Range(0,5)) = 2
        _PulseAmount ("Pulse Amount", Range(0,2)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Off   // ✅ FRONT + BACK visible

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _BaseColor;
            float4 _GlowColor;
            float _GlowIntensity;
            float _PulseSpeed;
            float _PulseAmount;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = normalize(v.normal);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // Edge glow based on normal
                float edge = 1 - abs(i.normal.y);

                // Pulse animation
                float pulse = sin(_Time.y * _PulseSpeed) * _PulseAmount + 1;

                float3 glow = _GlowColor.rgb * edge * _GlowIntensity * pulse;
                float3 baseCol = _BaseColor.rgb;

                return half4(baseCol + glow, 1);
            }
            ENDHLSL
        }
    }
}
