// PLACEHOLDER for the Asset Store shader "Shader Graphs/HaloPulse" (the ripped copy was a compiled Metal blob that
// cannot render in the editor or on other platforms). Same shader name, same properties and defaults, same
// queue / tags / blend state; the body is a simple approximation of the effect. Import the real package and
// run tools/remake/relink_stub_guids.py to put this file's GUID on the real shader, then delete this file.
Shader "Shader Graphs/HaloPulse"
{
    Properties
    {
        _PulsePeriod ("PulsePeriod", Float) = 1
        _Color ("Color", Color) = (0.02207178, 0.6518165, 0.7798742, 0)
        _Brightness ("Brightness", Float) = 1
        _AttenuationLimit ("AttenuationLimit", Range(0, 10)) = 1.17
        [NoScaleOffset] _SampleTexture2D_9dbf5f3320a441a1b91d160dee71bcee_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
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
            TEXTURE2D(_SampleTexture2D_9dbf5f3320a441a1b91d160dee71bcee_Texture_1_Texture2D); SAMPLER(sampler_SampleTexture2D_9dbf5f3320a441a1b91d160dee71bcee_Texture_1_Texture2D);
            half4 _Color; float _PulsePeriod, _Brightness, _AttenuationLimit;
            half4 frag(Varyings i) : SV_Target
            {
                half4 t = SAMPLE_TEXTURE2D(_SampleTexture2D_9dbf5f3320a441a1b91d160dee71bcee_Texture_1_Texture2D, sampler_SampleTexture2D_9dbf5f3320a441a1b91d160dee71bcee_Texture_1_Texture2D, i.uv);
                float pulse = 0.75 + 0.25 * sin(_Time.y * 6.2831853 / max(_PulsePeriod, 0.01));
                return half4(t.rgb * _Color.rgb * _Brightness * pulse, t.a * _Color.a);
            }

            ENDHLSL
        }
    }
}
