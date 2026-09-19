#!/usr/bin/env python3
"""Print properties, tags, pass render state, keywords and the Metal sources of a ripped (AssetRipper YAML) shader asset."""
import re,sys,struct,lz4.block,yaml
path=sys.argv[1]
text=open(path).read()
body=text.split('\n',3)[3] if text.startswith('%YAML') else text
doc=yaml.safe_load(body)
sh=doc['Shader']; pf=sh['m_ParsedForm']
print('NAME',pf['m_Name'])
print('PROPS')
for p in pf['m_PropInfo']['m_Props']:
    print('  ',p.get('m_Name'),'type',p.get('m_Type'),'def',p.get('m_DefValue0'),p.get('m_DefValue1'),p.get('m_DefValue2'),p.get('m_DefValue3'),'tex',p.get('m_DefTexture',{}).get('m_DefaultName'),'attrs',p.get('m_Attributes'),'desc',p.get('m_Description'),'flags',p.get('m_Flags'))
print('FALLBACK',pf.get('m_FallbackName'),'CUSTOMEDITOR',pf.get('m_CustomEditorName'))
for si,sub in enumerate(pf['m_SubShaders']):
    print('SUBSHADER',si,'tags',sub.get('m_Tags'),'LOD',sub.get('m_LOD'))
    for pi,ps in enumerate(sub['m_Passes']):
        st=ps.get('m_State',{})
        def V(d,k):
            v=d.get(k) if isinstance(d,dict) else None
            return v.get('val') if isinstance(v,dict) else v
        print('  PASS',pi,'name',ps.get('m_Name') or st.get('m_Name'),'type',ps.get('m_Type'),'tags',ps.get('m_Tags'),'useName',ps.get('m_UseName'))
        print('    pass tags',st.get('m_Tags'),'LOD',st.get('m_LOD'),'lighting',st.get('lighting'),'fogMode',st.get('fogMode'))
        print('    state: cull',V(st,'culling'),'zwrite',V(st,'zWrite'),'ztest',V(st,'zTest'),'zclip',V(st,'zClip'),'offset',V(st,'offsetFactor'),V(st,'offsetUnits'),'alphaToMask',V(st,'alphaToMask'),'conservative',V(st,'conservative'))
        rt0=st.get('rtBlend0',{})
        print('    blend0: src',V(rt0,'srcBlend'),'dst',V(rt0,'destBlend'),'srcA',V(rt0,'srcBlendAlpha'),'dstA',V(rt0,'destBlendAlpha'),'op',V(rt0,'blendOp'),'opA',V(rt0,'blendOpAlpha'),'colMask',V(rt0,'colMask'),'rtSeparate',st.get('rtSeparateBlend'))
        so=st.get('stencilOp',{})
        print('    stencil ref',V(st,'stencilRef'),'read',V(st,'stencilReadMask'),'write',V(st,'stencilWriteMask'),'comp',V(so,'comp'),'pass',V(so,'pass'),'fail',V(so,'fail'),'zfail',V(so,'zFail'))
        print('    keywords', ps.get('m_NameIndices'))
        for stage in ('progVertex','progFragment'):
            pr=ps.get(stage,{})
            sps=pr.get('m_SubPrograms') or []
            plays=pr.get('m_PlayerSubPrograms') or []
            print('   ',stage,'subprograms',len(sps),'player',len(plays),'commonParams',len(pr.get('m_CommonParameters') or []))
            for sp in sps:
                print('      blob',sp.get('m_BlobIndex'),'api',sp.get('m_GpuProgramType'),'keywords',sp.get('m_KeywordIndices'),'globalKeywords',sp.get('m_GlobalKeywordIndices'),'localKeywords',sp.get('m_LocalKeywordIndices'),'shaderRequirements',sp.get('m_ShaderRequirements'))
print('KEYWORD NAMES', pf.get('m_KeywordNames'))
# metal sources per platform blob
plats=sh.get('platforms'); print('PLATFORMS',plats)
blobs=re.findall(r'compressedBlob: ([0-9a-f]+)',text)
dls=re.findall(r'decompressedLengths:\n((?:\s+- [0-9a-f]+\n)+)',text)
for bi,b in enumerate(blobs):
    raw=bytes.fromhex(b)
    lens=[struct.unpack('<I',bytes.fromhex(x))[0] for x in re.findall(r'- ([0-9a-f]{8})',dls[bi])]
    off=0
    for n in lens:
        # each blob segment is separately compressed? compressedLengths give sizes
        pass
    cls=re.findall(r'compressedLengths:\n((?:\s+- [0-9a-f]+\n)+)',text)[bi]
    clens=[struct.unpack('<I',bytes.fromhex(x))[0] for x in re.findall(r'- ([0-9a-f]{8})',cls)]
    pos=0
    for n,c in zip(lens,clens):
        out=lz4.block.decompress(raw[pos:pos+c], uncompressed_size=n); pos+=c
        txt=out.decode('latin-1')
        k=0
        for m in re.finditer(r'#include <metal_stdlib>.*?(?=\x00)', txt, re.S):
            k+=1
            print(f'----- blob {bi} program {k} -----'); print(m.group(0))
