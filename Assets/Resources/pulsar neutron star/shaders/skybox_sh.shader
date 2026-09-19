// PLACEHOLDER for the Asset Store shader "Shader Graphs/SkyBox_SH" (the ripped copy was a compiled Metal blob that
// cannot render in the editor or on other platforms). Same shader name, same properties and defaults, same
// queue / tags / blend state; the body is a simple approximation of the effect. Import the real package and
// run tools/remake/relink_stub_guids.py to put this file's GUID on the real shader, then delete this file.
Shader "Shader Graphs/SkyBox_SH"
{
    Properties
    {
        Vector1_A1B13CA9 ("SkyBrightness", Float) = 1
        Vector1_B851E38B ("Red Bright", Float) = 1
        Vector1_5FAFE31D ("Scene Bright", Float) = 1
        [NoScaleOffset] Cubemap_75787A6E ("Cubemap", Cube) = "white" {}
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
            TEXTURECUBE(Cubemap_75787A6E); SAMPLER(samplerCubemap_75787A6E); float Vector1_A1B13CA9, Vector1_B851E38B, Vector1_5FAFE31D;
            half4 frag(Varyings i) : SV_Target
            {
                half4 c = SAMPLE_TEXTURECUBE(Cubemap_75787A6E, samplerCubemap_75787A6E, normalize(i.dirOS));
                c.rgb *= Vector1_A1B13CA9 * Vector1_5FAFE31D; c.r *= max(Vector1_B851E38B, 0.0001) / max(Vector1_B851E38B, 0.0001);
                return half4(c.rgb, 1);
            }

            ENDHLSL
        }
    }
}
