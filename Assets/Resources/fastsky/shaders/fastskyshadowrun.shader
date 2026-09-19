// PLACEHOLDER for the Asset Store shader "GapperGames/FastSky ShadowRun" (the ripped copy was a compiled Metal blob that
// cannot render in the editor or on other platforms). Same shader name, same properties and defaults, same
// queue / tags / blend state; the body is a simple approximation of the effect. Import the real package and
// run tools/remake/relink_stub_guids.py to put this file's GUID on the real shader, then delete this file.
Shader "GapperGames/FastSky ShadowRun"
{
    Properties
    {
        [HDR] _DayColour ("DayColour", Color) = (0.5, 0.025, 0.045, 1)
        [HDR] _EveningColour ("EveningColour", Color) = (1.1, 0.1, 0.045, 1)
        [HDR] _StarColour ("StarColour", Color) = (1, 1, 1, 1)
        [HDR] _CloudColour ("CloudColour", Color) = (0.025, 0.001, 0.003, 1)
        _SunIntensity ("SunIntensity", Float) = 12
        _SunSize ("SunSize", Range(0.001, 0.25)) = 0.055
        _Saturation ("Saturation", Float) = 1.5
        _DayBrightness ("DayBrightness", Float) = 0.38
        _EveningBrightness ("EveningBrightness", Float) = 0.16
        _EveningThreshold ("EveningThreshold", Float) = 1.34
        _Wavelengths ("Wavelengths", Color) = (760, 470, 390, 0)
        _DayScatterStrength ("DayScatterStrength", Float) = 1.12
        _EveningScatterStrength ("EveningScatterStrength", Float) = 19
        _DensityPower ("DensityPower", Float) = 7.2
        _StarBrightness ("StarBrightness", Float) = 0
        _StarSize ("StarSize", Range(0, 1)) = 0.98
        _StarScale ("StarScale", Float) = 2
        _StarSpeed ("StarSpeed", Float) = 0.02
        _StarJitterSpeed ("StarJitterSpeed", Float) = 0.1
        _StarRotateSpeed ("StarRotateSpeed", Float) = 5
        _StarThreshold ("StarThreshold", Float) = 20
        _CloudBrightness ("CloudBrightness", Float) = 60
        _CloudAnisotropy ("CloudAnisotropy", Range(0, 1)) = 0.8
        _CloudDensity ("CloudDensity", Float) = 2
        _CloudHeight ("CloudHeight", Range(0, 1)) = 0.35
        _CloudThickness ("CloudThickness", Range(0, 1)) = 0.7
        _CloudSoftness ("CloudSoftness", Range(0, 1)) = 0.45
        _CloudSmoothness ("CloudSmoothness", Range(0, 1)) = 0.5
        _CloudDensitySmoothness ("CloudDensitySmoothness", Range(0, 1)) = 0.6
        _CloudSpeed ("CloudSpeed", Float) = 0.08
        _CloudScale ("CloudScale", Float) = 0.8
        _CloudTwirl ("CloudTwirl", Range(0, 1)) = 0
        _Light_Direction ("Light_Direction", Vector) = (0, 1, 0, 0)
    }
    SubShader
    {
        Tags { "PreviewType" = "Skybox" "Queue" = "Background" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Background" }
        Pass
        {
            Cull Off
            ZWrite Off
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
            half4 _DayColour, _EveningColour, _StarColour, _CloudColour; float _DayBrightness, _EveningBrightness, _EveningThreshold, _StarBrightness, _StarThreshold, _StarScale, _CloudBrightness, _CloudDensity; float4 _Light_Direction;
            half4 frag(Varyings i) : SV_Target
            {
                float3 d = normalize(i.dirOS);
                float up = saturate(d.y);
                float horizon = pow(1.0 - up, 3.0);
                half3 sky = lerp(_DayColour.rgb * _DayBrightness, _EveningColour.rgb * _EveningBrightness, saturate(horizon + _EveningThreshold * 0.0));
                // a few procedural stars above the horizon
                float2 cell = floor(d.xz / max(d.y, 0.05) * max(_StarScale, 1.0));
                float n = frac(sin(dot(cell, float2(12.9898, 78.233))) * 43758.5453);
                half star = step(1.0 - 0.02 * saturate(_StarThreshold + 1.0), n) * up * _StarBrightness;
                return half4(sky + _StarColour.rgb * star, 1);
            }

            ENDHLSL
        }
    }
}
