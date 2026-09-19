// PLACEHOLDER for the Asset Store shader "Shader Graphs/SunCore" (the ripped copy was a compiled Metal blob that
// cannot render in the editor or on other platforms). Same shader name, same properties and defaults, same
// queue / tags / blend state; the body is a simple approximation of the effect. Import the real package and
// run tools/remake/relink_stub_guids.py to put this file's GUID on the real shader, then delete this file.
Shader "Shader Graphs/SunCore"
{
    Properties
    {
        [HDR] Color_85C936A7 ("Base Noise Color", Color) = (1, 0.8440896, 0, 0)
        [HDR] Color_A488C782 ("Second Color", Color) = (0.4716981, 0.04829874, 0, 0)
        _NoiseScale ("NoiseScale", Float) = 5.1
        _NoisePow ("NoisePow", Float) = 1
        _Animation_Speed ("Animation_Speed", Range(0.001, 1)) = 0.34
        _RimBrightness ("RimBrightness", Range(0, 1000)) = 149.79
        _FinalBrightness ("FinalBrightness", Range(0.001, 10)) = 0.56
        _BlinkPeriod ("BlinkPeriod", Float) = 1.5
        [HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl ("_QueueControl", Float) = -1
        [NoScaleOffset] [HideInInspector] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "white" {}
        [NoScaleOffset] [HideInInspector] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "white" {}
        [NoScaleOffset] [HideInInspector] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "white" {}
    }
    SubShader
    {
        Tags { "DisableBatching" = "False" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        Pass
        {
            Cull Back
            ZWrite On
            Blend One Zero
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; float3 positionWS : TEXCOORD1; float3 normalWS : TEXCOORD2; float3 dirOS : TEXCOORD3; };
            Varyings vert(Attributes i)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                o.uv = i.uv; o.color = i.color;
                o.positionWS = TransformObjectToWorld(i.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(i.normalOS);
                o.dirOS = normalize(i.positionOS.xyz);
                return o;
            }
            half4 Color_85C936A7, Color_A488C782; float _NoiseScale, _NoisePow, _Animation_Speed, _RimBrightness, _FinalBrightness, _BlinkPeriod;
            half4 frag(Varyings i) : SV_Target
            {
                float3 v = normalize(_WorldSpaceCameraPos - i.positionWS);
                float rim = pow(1.0 - saturate(dot(normalize(i.normalWS), v)), 2.0) * _RimBrightness;
                float n = 0.5 + 0.5 * sin(dot(i.positionWS, float3(3.1, 5.7, 2.3)) * _NoiseScale + _Time.y * _Animation_Speed);
                float blink = 0.85 + 0.15 * sin(_Time.y * 6.2831853 / max(_BlinkPeriod, 0.01));
                half3 c = lerp(Color_85C936A7.rgb, Color_A488C782.rgb, pow(n, max(_NoisePow, 0.01))) + rim;
                return half4(c * _FinalBrightness * blink, 1);
            }

            ENDHLSL
        }
    }
}
