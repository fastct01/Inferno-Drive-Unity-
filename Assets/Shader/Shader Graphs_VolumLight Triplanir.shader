// PLACEHOLDER for the Asset Store shader "Shader Graphs/VolumLight Triplanir" (the ripped copy was a compiled Metal blob that
// cannot render in the editor or on other platforms). Same shader name, same properties and defaults, same
// queue / tags / blend state; the body is a simple approximation of the effect. Import the real package and
// run tools/remake/relink_stub_guids.py to put this file's GUID on the real shader, then delete this file.
Shader "Shader Graphs/VolumLight Triplanir"
{
    Properties
    {
        _Opacity ("Opacity", Float) = 0.2
        _Color ("Color", Color) = (0, 0, 0, 0)
        _Metalic ("Metalic", Float) = 0
        _Emission ("Emission", Float) = 0
        _Ambient_Occlusion ("Ambient Occlusion", Float) = 1
        _Smoothness ("Smoothness", Float) = 0
        _Edg ("Edg", Float) = 0
        [ToggleUI] _Texture ("Texture?", Float) = 0
        _Power ("Power", Float) = 1
        [NoScaleOffset] _SampleTexture2D_cb642e350d5e4cc0962783f41619d5e8_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
        [HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl ("_QueueControl", Float) = -1
        [NoScaleOffset] [HideInInspector] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "white" {}
        [NoScaleOffset] [HideInInspector] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "white" {}
        [NoScaleOffset] [HideInInspector] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "white" {}
    }
    SubShader
    {
        Tags { "DisableBatching" = "False" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" }
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
            half4 _Color; float _Opacity, _Power, _Edg, _Emission;
            half4 frag(Varyings i) : SV_Target
            {
                float3 v = normalize(_WorldSpaceCameraPos - i.positionWS);
                float facing = saturate(abs(dot(normalize(i.normalWS), v)));
                float a = pow(facing, max(_Edg, 0.01)) * _Opacity;
                return half4(_Color.rgb * (1.0 + _Emission) * max(_Power, 0.0), saturate(a) * _Color.a);
            }

            ENDHLSL
        }
    }
}
