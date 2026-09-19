#!/usr/bin/env python3
"""Generate placeholder source shaders for third-party shaders that the ripper could only export as
compiled Metal blobs: same name, same properties (names, types, defaults), same queue/tags/blend, a
simple visible approximation per shader. Replaced by the real package shader on import
(tools/remake/relink_stub_guids.py copies the ripped GUID onto it)."""
import re,sys,os,yaml

BLEND={0:'Zero',1:'One',2:'DstColor',3:'SrcColor',4:'OneMinusDstColor',5:'SrcAlpha',6:'OneMinusSrcColor',7:'DstAlpha',8:'OneMinusDstAlpha',9:'SrcAlphaSaturate',10:'OneMinusSrcAlpha'}
CULL={0:'Off',1:'Front',2:'Back'}

def props_block(props):
    out=[]
    for p in props:
        name=p['m_Name']; desc=p.get('m_Description') or name; t=p['m_Type']; d=[p.get('m_DefValue[%d]'%i,0) for i in range(4)]
        attrs=''.join('[%s] '%a for a in (p.get('m_Attributes') or []) if not a.startswith('KeywordEnumNoPrefix'))
        flags=p.get('m_Flags',0)
        if flags & 1: attrs='[HideInInspector] '+attrs
        if flags & 16: attrs='[HDR] '+attrs
        if flags & 2: attrs='[PerRendererData] '+attrs
        if t==0: out.append(f'        {attrs}{name} ("{desc}", Color) = ({d[0]}, {d[1]}, {d[2]}, {d[3]})')
        elif t==1: out.append(f'        {attrs}{name} ("{desc}", Vector) = ({d[0]}, {d[1]}, {d[2]}, {d[3]})')
        elif t==2: out.append(f'        {attrs}{name} ("{desc}", Float) = {d[0]}')
        elif t==3: out.append(f'        {attrs}{name} ("{desc}", Range({d[1]}, {d[2]})) = {d[0]}')
        elif t==4:
            dim=p.get('m_DefTexture',{}).get('m_TexDim',2); tex=p.get('m_DefTexture',{}).get('m_DefaultName') or 'white'
            kind={2:'2D',3:'3D',4:'Cube',5:'2DArray'}.get(dim,'2D')
            if flags & 4: attrs='[NoScaleOffset] '+attrs
            out.append(f'        {attrs}{name} ("{desc}", {kind}) = "{tex}" {{}}')
        elif t==5: out.append(f'        {attrs}{name} ("{desc}", Int) = {int(d[0])}')
    return '\n'.join(out)

def load(path):
    text=open(path).read(); body=text.split('\n',3)[3]
    doc=yaml.safe_load(body); pf=doc['Shader']['m_ParsedForm']
    subs=pf['m_SubShaders']
    sub=subs[0] if subs else {'m_Passes':[],'m_Tags':{'tags':{'QUEUE':'Transparent','RenderType':'Transparent'}}}
    ps=sub['m_Passes'][0] if sub['m_Passes'] else {'m_State':{'culling':{'val':0},'zWrite':{'val':0},'rtBlend0':{'srcBlend':{'val':5},'destBlend':{'val':10}}}}
    st=ps.get('m_State',{})
    V=lambda d,k: (d.get(k) or {}).get('val') if isinstance(d.get(k),dict) else d.get(k)
    rt=st.get('rtBlend0',{})
    tags=(sub.get('m_Tags') or {}).get('tags') or {}
    state=dict(cull=V(st,'culling'),zwrite=V(st,'zWrite'),src=V(rt,'srcBlend'),dst=V(rt,'destBlend'),srcA=V(rt,'srcBlendAlpha'),dstA=V(rt,'destBlendAlpha'),tags=tags,name=pf['m_Name'],props=pf['m_PropInfo']['m_Props'])
    return state

def tags_line(tags):
    m={'QUEUE':'Queue','RenderType':'RenderType','IGNOREPROJECTOR':'IgnoreProjector','PreviewType':'PreviewType','RenderPipeline':'RenderPipeline','CanUseSpriteAtlas':'CanUseSpriteAtlas','DisableBatching':'DisableBatching'}
    parts=[]
    for k,v in tags.items():
        if k in m: parts.append('"%s" = "%s"'%(m[k], 'True' if v is True else ('False' if v is False else v)))
    return ' '.join(parts)

def shader(path, body, blend_from_props=False):
    s=load(path)
    if blend_from_props: blend='Blend [_SrcBlend] [_DstBlend]'
    else: blend=f"Blend {BLEND.get(s['src'],'One')} {BLEND.get(s['dst'],'Zero')}"
    zwrite='ZWrite [_ZWrite]' if blend_from_props and any(p['m_Name']=='_ZWrite' for p in s['props']) else ('ZWrite On' if s['zwrite'] else 'ZWrite Off')
    text=f'''// PLACEHOLDER for the Asset Store shader "{s['name']}" (the ripped copy was a compiled Metal blob that
// cannot render in the editor or on other platforms). Same shader name, same properties and defaults, same
// queue / tags / blend state; the body is a simple approximation of the effect. Import the real package and
// run tools/remake/relink_stub_guids.py to put this file's GUID on the real shader, then delete this file.
Shader "{s['name']}"
{{
    Properties
    {{
{props_block(s['props'])}
    }}
    SubShader
    {{
        Tags {{ {tags_line(s['tags'])} }}
        Pass
        {{
            Cull {CULL.get(s['cull'],'Off')}
            {zwrite}
            {blend}
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
{body}
            ENDHLSL
        }}
    }}
}}
'''
    return text

COMMON_V='''            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; float3 normalOS : NORMAL; };
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
'''
OUT=os.path.expanduser('~/Downloads/InfernoDrive-Unity/Assets/')
jobs={
 'Resources/fastsky/shaders/fastskyshadowrun': (False, COMMON_V+'''            half4 _DayColour, _EveningColour, _StarColour, _CloudColour; float _DayBrightness, _EveningBrightness, _EveningThreshold, _StarBrightness, _StarThreshold, _StarScale, _CloudBrightness, _CloudDensity; float4 _Light_Direction;
            half4 frag(Varyings i) : SV_Target
            {
                float3 d = normalize(i.dirOS);
                float up = saturate(d.y);
                float horizon = pow(1.0 - up, 3.0);
                half3 sky = lerp(_DayColour.rgb * _DayBrightness, _EveningColour.rgb * _EveningBrightness, saturate(horizon + _EveningThreshold * 0.0));
                // a few procedural stars above the horizon
                float2 cell = floor(d.xz / max(d.y, 0.05) * max(_StarScale, 1.0));
                float n = frac(sin(dot(cell, float2(12.9898, 78.233))) * 43758.5453);
                half star = step(1.0 - 0.02 * saturate(_StarThreshold + 1.0), n) * up * _StarBrightness;
                return half4(sky + _StarColour.rgb * star, 1);
            }
'''),
 'Resources/fastsky/shaders/fast_skybox': (False, COMMON_V+'''            half4 _DayColour, _EveningColour, _StarColour, _CloudColour; float _DayBrightness, _EveningBrightness, _StarBrightness;
            half4 frag(Varyings i) : SV_Target
            {
                float3 d = normalize(i.dirOS);
                float horizon = pow(1.0 - saturate(d.y), 3.0);
                half3 sky = lerp(_DayColour.rgb * _DayBrightness, _EveningColour.rgb * _EveningBrightness, horizon);
                return half4(sky, 1);
            }
'''),
 'Resources/pulsar neutron star/shaders/skybox_sh': (False, COMMON_V+'''            TEXTURECUBE(Cubemap_75787A6E); SAMPLER(samplerCubemap_75787A6E); float Vector1_A1B13CA9, Vector1_B851E38B, Vector1_5FAFE31D;
            half4 frag(Varyings i) : SV_Target
            {
                half4 c = SAMPLE_TEXTURECUBE(Cubemap_75787A6E, samplerCubemap_75787A6E, normalize(i.dirOS));
                c.rgb *= Vector1_A1B13CA9 * Vector1_5FAFE31D; c.r *= max(Vector1_B851E38B, 0.0001) / max(Vector1_B851E38B, 0.0001);
                return half4(c.rgb, 1);
            }
'''),
 'Resources/pulsar neutron star/shaders/suncore': (False, COMMON_V+'''            half4 Color_85C936A7, Color_A488C782; float _NoiseScale, _NoisePow, _Animation_Speed, _RimBrightness, _FinalBrightness, _BlinkPeriod;
            half4 frag(Varyings i) : SV_Target
            {
                float3 v = normalize(_WorldSpaceCameraPos - i.positionWS);
                float rim = pow(1.0 - saturate(dot(normalize(i.normalWS), v)), 2.0) * _RimBrightness;
                float n = 0.5 + 0.5 * sin(dot(i.positionWS, float3(3.1, 5.7, 2.3)) * _NoiseScale + _Time.y * _Animation_Speed);
                float blink = 0.85 + 0.15 * sin(_Time.y * 6.2831853 / max(_BlinkPeriod, 0.01));
                half3 c = lerp(Color_85C936A7.rgb, Color_A488C782.rgb, pow(n, max(_NoisePow, 0.01))) + rim;
                return half4(c * _FinalBrightness * blink, 1);
            }
'''),
 'Resources/pulsar neutron star/shaders/halopulse': (False, COMMON_V+'''            TEXTURE2D(_SampleTexture2D_9dbf5f3320a441a1b91d160dee71bcee_Texture_1_Texture2D); SAMPLER(sampler_SampleTexture2D_9dbf5f3320a441a1b91d160dee71bcee_Texture_1_Texture2D);
            half4 _Color; float _PulsePeriod, _Brightness, _AttenuationLimit;
            half4 frag(Varyings i) : SV_Target
            {
                half4 t = SAMPLE_TEXTURE2D(_SampleTexture2D_9dbf5f3320a441a1b91d160dee71bcee_Texture_1_Texture2D, sampler_SampleTexture2D_9dbf5f3320a441a1b91d160dee71bcee_Texture_1_Texture2D, i.uv);
                float pulse = 0.75 + 0.25 * sin(_Time.y * 6.2831853 / max(_PulsePeriod, 0.01));
                return half4(t.rgb * _Color.rgb * _Brightness * pulse, t.a * _Color.a);
            }
'''),
 'Resources/jmo assets/cartoon fx remaster/cfxr assets/shaders/cfxr particle ubershader': (True, COMMON_V+'''            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_ST; float _HdrMultiply, _SingleChannel, _Cutoff, _UseAlphaClip;
            half4 frag(Varyings i) : SV_Target
            {
                half4 t = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);
                if (_SingleChannel > 0.5) t = half4(1, 1, 1, t.r);
                half4 c = t * i.color;
                c.rgb *= max(_HdrMultiply, 1.0);
                if (_UseAlphaClip > 0.5) clip(c.a - _Cutoff);
                return c;
            }
'''),
 'Resources/jmo assets/cartoon fx remaster/cfxr assets/shaders/cfxr particle glow': (True, COMMON_V+'''            float _GlowMin, _GlowMax, _MaxValue, _HdrMultiply;
            half4 frag(Varyings i) : SV_Target
            {
                float r = length(i.uv - 0.5) * 2.0;
                float g = saturate((_GlowMax - r) / max(_GlowMax - _GlowMin, 0.001));
                g = min(g * g, _MaxValue);
                half4 c = i.color; c.rgb *= g * max(_HdrMultiply, 1.0); c.a *= g;
                return c;
            }
'''),
 'Resources/jmo assets/cartoon fx remaster/cfxr assets/shaders/cfxr particle procedural ring': (True, COMMON_V+'''            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_ST; float _HdrMultiply, _SingleChannel, _RingTopOffset;
            half4 frag(Varyings i) : SV_Target
            {
                half4 t = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);
                if (_SingleChannel > 0.5) t = half4(1, 1, 1, t.r);
                half4 c = t * i.color; c.rgb *= max(_HdrMultiply, 1.0);
                return c;
            }
'''),
 'Resources/jmo assets/cartoon fx remaster/cfxr assets/shaders/cfxr particle distortion': (False, COMMON_V+'''            half4 frag(Varyings i) : SV_Target
            {
                // screen distortion needs the opaque texture; the placeholder is invisible
                return half4(0, 0, 0, 0);
            }
'''),
 'Shader/Shader Graphs_VolumLight': (False, COMMON_V+'''            half4 _Color; float _Opacity, _Power, _Edg, _Emission;
            half4 frag(Varyings i) : SV_Target
            {
                float3 v = normalize(_WorldSpaceCameraPos - i.positionWS);
                float facing = saturate(abs(dot(normalize(i.normalWS), v)));
                float a = pow(facing, max(_Edg, 0.01)) * _Opacity;
                return half4(_Color.rgb * (1.0 + _Emission) * max(_Power, 0.0), saturate(a) * _Color.a);
            }
'''),
 'Shader/Shader Graphs_VolumLight Triplanir': (False, COMMON_V+'''            half4 _Color; float _Opacity, _Power, _Edg, _Emission;
            half4 frag(Varyings i) : SV_Target
            {
                float3 v = normalize(_WorldSpaceCameraPos - i.positionWS);
                float facing = saturate(abs(dot(normalize(i.normalWS), v)));
                float a = pow(facing, max(_Edg, 0.01)) * _Opacity;
                return half4(_Color.rgb * (1.0 + _Emission) * max(_Power, 0.0), saturate(a) * _Color.a);
            }
'''),
}
for rel,(blendProps,body) in jobs.items():
    asset=OUT+rel+'.asset'
    text=shader(asset, body, blendProps)
    target=OUT+rel+'.shader'
    open(target,'w').write(text)
    guid=re.search(r'^guid: ([0-9a-f]{32})',open(asset+'.meta').read(),re.M).group(1)
    open(target+'.meta','w').write('fileFormatVersion: 2\nguid: %s\nShaderImporter:\n  externalObjects: {}\n  defaultTextures: []\n  nonModifiableTextures: []\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'%guid)
    print('placeholder', rel, guid)
