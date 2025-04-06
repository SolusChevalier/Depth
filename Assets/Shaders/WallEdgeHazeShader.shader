Shader "Custom/WallEdgeHaze"
{
    Properties
    {
        _FadeDistance ("Fade Distance", Float) = 10.0
        _Color ("Color", Color) = (1,1,1,1)
        _EdgeFade ("Edge Fade Strength", Range(0,1)) = 0.5
        _EdgeSharpness ("Edge Sharpness", Range(0.01, 5)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            float _EdgeFade;
            float _EdgeSharpness;
            float4 _Color;
            float _FadeDistance;
             v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.pos = TransformWorldToHClip(worldPos);
                o.worldNormal = normalize(TransformObjectToWorldNormal(v.normal));
                o.worldPos = worldPos;
                return o;
            }

            half4 frag (v2f i) : SV_Target
{
    float3 camPos = GetCameraPositionWS();

    // Calculate horizontal distance (ignore vertical Y distance)
    float horizontalDist = length(float2(i.worldPos.x - camPos.x, i.worldPos.z - camPos.z));

    // Fade based on horizontal distance (clamped to _FadeDistance)
    float distToCamera = saturate(horizontalDist / _FadeDistance);

    // Edge factor (same idea as before but adjusted for top-down view)
    float edgeFactor = 1.0 - abs(i.worldNormal.x); // Focus on the X-axis (side view)
    edgeFactor = pow(edgeFactor, _EdgeSharpness);
    edgeFactor = smoothstep(0, 1, edgeFactor * _EdgeFade);

    // Combine distance fade and edge fade
    float distFade = saturate(1.0 - distToCamera); // We want more fade as we get farther
    float edgeInfluence = lerp(0.6, 1.0, edgeFactor); // Keeps at least some opacity

    // Final alpha, blending distance and edge influence
    float finalAlpha = distFade * edgeInfluence;

    return float4(_Color.rgb, finalAlpha);
}
            ENDHLSL
        }
    }
}