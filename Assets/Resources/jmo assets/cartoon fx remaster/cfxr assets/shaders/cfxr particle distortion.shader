// PLACEHOLDER for the Asset Store shader "Cartoon FX/Remaster/Particle Screen Distortion" (the ripped copy was a compiled Metal blob that
// cannot render in the editor or on other platforms). Same shader name, same properties and defaults, same
// queue / tags / blend state; the body is a simple approximation of the effect. Import the real package and
// run tools/remake/relink_stub_guids.py to put this file's GUID on the real shader, then delete this file.
Shader "Cartoon FX/Remaster/Particle Screen Distortion"
{
    Properties
    {
        [Toggle(_ALPHATEST_ON)] _UseAlphaClip ("Alpha Clipping (Cutout)", Float) = 0
        _Cutoff ("Cutoff Threshold", Range(0.001, 1)) = 0.1
        [Toggle(_FADING_ON)] _UseSP ("Soft Particles", Float) = 0
        _SoftParticlesFadeDistanceNear ("Near Fade", Float) = 0
        _SoftParticlesFadeDistanceFar ("Far Fade", Float) = 1
        [Toggle(_CFXR_EDGE_FADING)] _UseEF ("Edge Fade", Float) = 0
        _EdgeFadePow ("Edge Fade Power", Float) = 1
        [NoScaleOffset] _ScreenDistortionTex ("Distortion Texture", 2D) = "bump" {}
        _ScreenDistortionScale ("Distortion Scale", Range(-0.5, 0.5)) = 0.1
        [Toggle(_DEBUG_VISUALIZE_DISTORTION)] _DebugVisualize ("Visualize Distortion Particles", Float) = 0
    }
    SubShader
    {
        Tags { "IgnoreProjector" = "True" "PreviewType" = "Plane" "Queue" = "Transparent" "RenderType" = "Transparent" }
        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
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
            half4 frag(Varyings i) : SV_Target
            {
                // screen distortion needs the opaque texture; the placeholder is invisible
                return half4(0, 0, 0, 0);
            }

            ENDHLSL
        }
    }
}
