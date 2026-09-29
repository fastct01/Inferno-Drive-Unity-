// [user request 2026-09-24: neon night cycle] Two latitude-longitude panoramas (night = _MainTex, day = _DayTex)
// mixed by _Blend, with the mapping, tint and exposure maths of Unity's Skybox/Panoramic (360 degrees, no mirror),
// so a map whose sky fades between night and day keeps its night look exactly at _Blend 0. Used by
// ShadowRunPrototype.InfernoMaps.DayNightMaterial through Resources/Inferno/SkyBlend.mat.
Shader "ShadowRun/PanoramicBlend"
{
    Properties
    {
        _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
        [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
        _Rotation ("Rotation", Range(0, 360)) = 0
        [NoScaleOffset] _MainTex ("Night panorama", 2D) = "grey" {}
        [NoScaleOffset] _DayTex ("Day panorama", 2D) = "grey" {}
        _Blend ("Day blend", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DayTex;
            half4 _Tint;
            half _Exposure;
            float _Rotation;
            half _Blend;

            float3 RotateAroundYInDegrees(float3 vertex, float degrees)
            {
                float alpha = degrees * UNITY_PI / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(m, vertex.xz), vertex.y).xzy;
            }

            inline float2 ToRadialCoords(float3 coords)
            {
                float3 normalizedCoords = normalize(coords);
                float latitude = acos(normalizedCoords.y);
                float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
                float2 sphereCoords = float2(longitude, latitude) * float2(0.5 / UNITY_PI, 1.0 / UNITY_PI);
                return float2(0.5, 1.0) - sphereCoords;
            }

            struct appdata_t { float4 vertex : POSITION; };
            struct v2f { float4 vertex : SV_POSITION; float3 texcoord : TEXCOORD0; };

            v2f vert(appdata_t v)
            {
                v2f o;
                float3 rotated = RotateAroundYInDegrees(v.vertex.xyz, _Rotation);
                o.vertex = UnityObjectToClipPos(rotated);
                o.texcoord = v.vertex.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 tc = ToRadialCoords(i.texcoord);
                half3 night = tex2D(_MainTex, tc).rgb;
                half3 day = tex2D(_DayTex, tc).rgb;
                half3 c = lerp(night, day, _Blend);
                c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb * _Exposure;
                return half4(c, 1);
            }
            ENDCG
        }
    }
    Fallback Off
}
