// Re-authored from the shipped build's compiled Metal programs (shader "ShadowRun/SolidColorSkybox":
// one pass, Cull Off, ZWrite Off, ZTest LEqual, Queue/RenderType Background, PreviewType Skybox;
// vertex: MVP; fragment: _Color). _Color is a plain uniform (not SRP-batched) in the shipped build.
Shader "ShadowRun/SolidColorSkybox"
{
    Properties
    {
        _Color ("Sky Color", Color) = (1, 0.03, 0.02, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Background" "Queue" = "Background" "PreviewType" = "Skybox" }
        Pass
        {
            Cull Off
            ZWrite Off
            ZTest LEqual
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            half4 _Color;

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }
}
