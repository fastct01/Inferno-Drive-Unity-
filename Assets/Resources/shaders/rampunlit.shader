// Re-authored from the shipped build's compiled Metal programs (shader "ShadowRun/RampUnlit": one
// UniversalForward pass "ForwardUnlit", Cull Off, ZWrite On, ZTest LEqual, opaque; vertex: uv *
// baseColorTexture_ST; fragment: baseColorTexture(uv) * baseColorFactor). Property names follow the
// glTF material the ramp mesh was imported with (baseColorTexture / baseColorFactor).
Shader "ShadowRun/RampUnlit"
{
    Properties
    {
        baseColorTexture ("Base Color", 2D) = "white" {}
        baseColorFactor ("Base Color Factor", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode" = "UniversalForward" }
            Cull Off
            ZWrite On
            ZTest LEqual
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(baseColorTexture);
            SAMPLER(sampler_baseColorTexture);

            CBUFFER_START(UnityPerMaterial)
                float4 baseColorTexture_ST;
                half4 baseColorFactor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv * baseColorTexture_ST.xy + baseColorTexture_ST.zw;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                return SAMPLE_TEXTURE2D(baseColorTexture, sampler_baseColorTexture, input.uv) * baseColorFactor;
            }
            ENDHLSL
        }
    }
}
