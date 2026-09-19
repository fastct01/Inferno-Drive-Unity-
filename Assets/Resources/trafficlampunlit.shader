// Re-authored from the shipped build's compiled Metal programs (Data/data.unity3d, shader
// "ShadowRun/TrafficLampUnlit": one UniversalForward pass, Cull Back, ZWrite On, ZTest LEqual,
// no blending, Queue Geometry+12; vertex = MVP only, fragment = _BaseColor from UnityPerMaterial).
// The property flag 16 (HDR) and the description "Lamp Color" come from the serialized shader.
Shader "ShadowRun/TrafficLampUnlit"
{
    Properties
    {
        [HDR] _BaseColor ("Lamp Color", Color) = (4, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry+12" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "TrafficLampUnlit"
            Tags { "LightMode" = "UniversalForward" }
            Cull Back
            ZWrite On
            ZTest LEqual
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                return _BaseColor;
            }
            ENDHLSL
        }
    }
}
