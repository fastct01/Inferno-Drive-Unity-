// PLACEHOLDER for the Asset Store shader "Cartoon FX/Remaster/Particle Procedural Glow" (the ripped copy was a compiled Metal blob that
// cannot render in the editor or on other platforms). Same shader name, same properties and defaults, same
// queue / tags / blend state; the body is a simple approximation of the effect. Import the real package and
// run tools/remake/relink_stub_guids.py to put this file's GUID on the real shader, then delete this file.
Shader "Cartoon FX/Remaster/Particle Procedural Glow"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Blend Source", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Blend Destination", Float) = 10
        [Toggle(_CFXR_DISSOLVE)] _UseDissolve ("Enable Dissolve", Float) = 0
        [NoScaleOffset] _DissolveTex ("Dissolve Texture", 2D) = "gray" {}
        _DissolveSmooth ("Dissolve Smoothing", Range(0.0001, 0.5)) = 0.1
        [ToggleNoKeyword] _InvertDissolveTex ("Invert Dissolve Texture", Float) = 0
        [KeywordEnum(P0, P2, P4, P8)] _CFXR_GLOW_POW ("Apply Power of", Float) = 0
        _GlowMin ("Circle Min", Float) = 0
        _GlowMax ("Circle Max", Float) = 1
        _MaxValue ("Max Value", Float) = 10
        _HdrMultiply ("HDR Multiplier", Float) = 2
        [Toggle(_FADING_ON)] _UseSP ("Soft Particles", Float) = 0
        _SoftParticlesFadeDistanceNear ("Near Fade", Float) = 0
        _SoftParticlesFadeDistanceFar ("Far Fade", Float) = 1
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
            ZWrite Off
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
            float _GlowMin, _GlowMax, _MaxValue, _HdrMultiply;
            half4 frag(Varyings i) : SV_Target
            {
                float r = length(i.uv - 0.5) * 2.0;
                float g = saturate((_GlowMax - r) / max(_GlowMax - _GlowMin, 0.001));
                g = min(g * g, _MaxValue);
                half4 c = i.color; c.rgb *= g * max(_HdrMultiply, 1.0); c.a *= g;
                return c;
            }

            ENDHLSL
        }
    }
}
