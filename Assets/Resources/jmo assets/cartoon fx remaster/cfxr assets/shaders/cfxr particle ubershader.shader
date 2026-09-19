// PLACEHOLDER for the Asset Store shader "Cartoon FX/Remaster/Particle Ubershader" (the ripped copy was a compiled Metal blob that
// cannot render in the editor or on other platforms). Same shader name, same properties and defaults, same
// queue / tags / blend state; the body is a simple approximation of the effect. Import the real package and
// run tools/remake/relink_stub_guids.py to put this file's GUID on the real shader, then delete this file.
Shader "Cartoon FX/Remaster/Particle Ubershader"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Blend Source", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Blend Destination", Float) = 10
        _BlendingType ("Blending Type", Float) = 0
        [ToggleNoKeyword] _ZWrite ("Depth Write", Float) = 0
        [Toggle(_ALPHATEST_ON)] _UseAlphaClip ("Alpha Clipping (Cutout)", Float) = 0
        _Cutoff ("Cutoff Threshold", Range(0.001, 1)) = 0.1
        [Toggle(_FADING_ON)] _UseSP ("Soft Particles", Float) = 0
        _SoftParticlesFadeDistanceNear ("Near Fade", Float) = 0
        _SoftParticlesFadeDistanceFar ("Far Fade", Float) = 1
        [Toggle(_CFXR_EDGE_FADING)] _UseEF ("Edge Fade", Float) = 0
        _EdgeFadePow ("Edge Fade Power", Float) = 1
        [Toggle(_CFXR_DISSOLVE)] _UseDissolve ("Enable Dissolve", Float) = 0
        _DissolveTex ("Dissolve Texture", 2D) = "gray" {}
        _DissolveSmooth ("Dissolve Smoothing", Range(0.0001, 0.5)) = 0.1
        [ToggleNoKeyword] _InvertDissolveTex ("Invert Dissolve Texture", Float) = 0
        [ToggleNoKeyword] _DoubleDissolve ("Double Dissolve", Float) = 0
        [Toggle] _UseDissolveOffsetUV ("Dissolve offset along X", Float) = 0
        _DissolveScroll ("UV Scrolling", Vector) = (0, 0, 0, 0)
        [Toggle(_CFXR_UV_DISTORTION)] _UseUVDistortion ("Enable UV Distortion", Float) = 0
        [NoScaleOffset] _DistortTex ("Distortion Texture", 2D) = "gray" {}
        _DistortScrolling ("Scroll (XY) Tile (ZW)", Vector) = (0, 0, 1, 1)
        [Toggle] _UseUV2Distortion ("Use UV2", Float) = 0
        _Distort ("Distortion Strength", Range(0, 2)) = 0.1
        [ToggleNoKeyword] _FadeAlongU ("Fade along Y", Float) = 0
        [Toggle] _UVDistortionAdd ("Add to base UV", Float) = 0
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        [Toggle] _SingleChannel ("Single Channel Texture", Float) = 0
        [KeywordEnum(Off,1x,2x)] _CFXR_OVERLAYTEX ("Enable Overlay Texture", Float) = 0
        [Enum(RGBA,0,RGB,1,A,2)] _CFXR_OVERLAYBLEND ("Overlay Blend Channels", Float) = 0
        [NoScaleOffset] _OverlayTex ("Overlay Texture", 2D) = "white" {}
        _OverlayTex_Scroll ("Overlay Scrolling / Scale", Vector) = (0.1, 0.1, 1, 1)
        [Toggle(_FLIPBOOK_BLENDING)] _UseFB ("Flipbook Blending", Float) = 0
        [Toggle(_CFXR_SECONDCOLOR_LERP)] _UseSecondColor ("Secondary Vertex Color (TEXCOORD2)", Float) = 0
        [NoScaleOffset] _SecondColorTex ("Second Color Map", 2D) = "black" {}
        _SecondColorSmooth ("Second Color Smoothing", Range(0.0001, 0.5)) = 0.2
        [Toggle(_CFXR_FONT_COLORS)] _UseFontColor ("Use Font Colors", Float) = 0
        _HdrMultiply ("HDR Multiplier", Float) = 1
        _UseLighting ("Mode", Float) = 0
        _DirectLightingRamp ("Direct Lighting Ramp", Range(0, 1)) = 1
        [Toggle(_NORMALMAP)] _UseNormalMap ("Enable Normal Map", Float) = 0
        [NoScaleOffset] _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Range(-1, 1)) = 1
        [Toggle(_EMISSION)] _UseEmission ("Enable Emission (TEXCOORD2)", Float) = 0
        [Toggle(_CFXR_LIGHTING_WPOS_OFFSET)] _UseLightingWorldPosOffset ("Enable World Pos. Offset", Float) = 0
        _LightingWorldPosStrength ("Offset Strength", Range(0, 1)) = 0.2
        [Toggle(_CFXR_LIGHTING_BACK)] _UseBackLighting ("Enable Backlighting", Float) = 0
        _DirLightScreenAtten ("Dir. Light Screen Attenuation", Range(0, 5)) = 1
        _BacklightTransmittance ("Backlight Transmittance", Range(0, 2)) = 1
        _IndirectLightingMix ("Indirect Lighting Mix", Range(0, 1)) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
        [KeywordEnum(Off,On,CustomTexture)] _CFXR_DITHERED_SHADOWS ("Dithered Shadows", Float) = 0
        _ShadowStrength ("Shadows Strength Max", Range(0, 1)) = 1
        _DitherCustom ("Dithering 3D Texture", 3D) = "black" {}
    }
    SubShader
    {
        Tags { "IgnoreProjector" = "True" "PreviewType" = "Plane" "Queue" = "Transparent" "RenderType" = "Transparent" }
        Pass
        {
            Cull Off
            ZWrite [_ZWrite]
            Blend [_SrcBlend] [_DstBlend]
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
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_ST; float _HdrMultiply, _SingleChannel, _Cutoff, _UseAlphaClip;
            half4 frag(Varyings i) : SV_Target
            {
                half4 t = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);
                if (_SingleChannel > 0.5) t = half4(1, 1, 1, t.r);
                half4 c = t * i.color;
                c.rgb *= max(_HdrMultiply, 1.0);
                if (_UseAlphaClip > 0.5) clip(c.a - _Cutoff);
                return c;
            }

            ENDHLSL
        }
    }
}
