// PLACEHOLDER for the Asset Store shader "GapperGames/Skybox/Fast_Skybox" (the ripped copy was a compiled Metal blob that
// cannot render in the editor or on other platforms). Same shader name, same properties and defaults, same
// queue / tags / blend state; the body is a simple approximation of the effect. Import the real package and
// run tools/remake/relink_stub_guids.py to put this file's GUID on the real shader, then delete this file.
Shader "GapperGames/Skybox/Fast_Skybox"
{
    Properties
    {
        _SunSize ("SunSize", Range(0, 1)) = 0.05
        _SunIntensity ("SunIntensity", Float) = 30
        _DayColour ("DayColour", Color) = (1, 1, 1, 1)
        _EveningColour ("EveningColour", Color) = (1, 0.3537736, 0.5057102, 1)
        _Saturation ("Saturation", Range(0, 2)) = 1
        _Wavelengths ("Wavelengths", Vector) = (680, 440, 550, 0)
        _DensityPower ("DensityPower", Float) = 8
        _DayScatterStrength ("DayScatterStrength", Float) = 1
        _EveningScatterStrength ("EveningScatterStrength", Float) = 7.5
        _DayBrightness ("DayBrightness", Float) = 1
        _EveningBrightness ("EveningBrightness", Float) = 0.5
        _EveningThreshold ("EveningThreshold", Float) = 3
        _StarColour ("StarColour", Color) = (0.740566, 0.9334841, 1, 1)
        _StarBrightness ("StarBrightness", Float) = 10
        _StarSize ("StarSize", Range(0, 1)) = 0.5
        _StarScale ("StarScale", Float) = 5
        _StarSpeed ("StarSpeed", Float) = 1
        _StarThreshold ("StarThreshold", Float) = 8
        _CloudColour ("CloudColour", Color) = (1, 1, 1, 1)
        _CloudBrightness ("CloudBrightness", Float) = 1
        _CloudAnisotropy ("CloudAnisotropy", Range(0, 1)) = 0.76
        _CloudDensity ("CloudDensity", Float) = 1
        _CloudHeight ("CloudHeight", Range(0, 1)) = 0.4
        _CloudThickness ("CloudThickness", Range(0, 1)) = 0.5
        _CloudSoftness ("CloudSoftness", Range(0, 1)) = 0.5
        _CloudSpeed ("CloudSpeed", Float) = 1
        _CloudTwirl ("CloudTwirl", Range(0, 5)) = 0.5
        _CloudScale ("CloudScale", Float) = 1
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
            half4 _DayColour, _EveningColour, _StarColour, _CloudColour; float _DayBrightness, _EveningBrightness, _StarBrightness;
            half4 frag(Varyings i) : SV_Target
            {
                float3 d = normalize(i.dirOS);
                float horizon = pow(1.0 - saturate(d.y), 3.0);
                half3 sky = lerp(_DayColour.rgb * _DayBrightness, _EveningColour.rgb * _EveningBrightness, horizon);
                return half4(sky, 1);
            }

            ENDHLSL
        }
    }
}
