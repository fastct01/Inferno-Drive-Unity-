// Re-authored from the shipped build's compiled Metal programs (shader "ShadowRun/VehicleSmokeMobile":
// one pass "Vehicle Smoke", Cull Off, ZWrite Off, ZTest LEqual, Blend SrcAlpha OneMinusSrcAlpha,
// Queue Transparent+100, IgnoreProjector). Vertex: uv * _BaseMap_ST, colour = vertex colour * _BaseColor.
// Fragment (decompiled): tex = _BaseMap(uv); t = saturate((max(tex.rgb) - 0.008) * 6.57894754)
// (= (x - 0.008) / 0.152); s = t*t*(3 - 2t); rgb = tex.rgb * colour.rgb; a = tex.a * s * colour.a;
// clip(a - 0.002).
Shader "ShadowRun/VehicleSmokeMobile"
{
    Properties
    {
        [MainTexture] _BaseMap ("Smoke Texture", 2D) = "white" {}
        [MainColor] _BaseColor ("Tint", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+100" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "Vehicle Smoke"
            Cull Off
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv * _BaseMap_ST.xy + _BaseMap_ST.zw;
                output.color = input.color * _BaseColor;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half brightest = max(tex.r, max(tex.g, tex.b));
                float t = saturate((brightest - 0.008) * 6.57894754);
                t = t * t * (3.0 - 2.0 * t);
                half alpha = tex.a * t;
                half4 result;
                result.rgb = tex.rgb * input.color.rgb;
                alpha *= input.color.a;
                result.a = alpha;
                clip(alpha - 0.002);
                return result;
            }
            ENDHLSL
        }
    }
}
