#!/usr/bin/env python3
"""IL2CPP global-metadata (v39) reader.  meta.py type <Name> | meta.py attrs <TypeName>"""
import struct,sys
d=open('/Users/fast-ct/Downloads/car точно финал/Data/Managed/Metadata/global-metadata.dat','rb').read()
names=['stringLiterals','stringLiteralData','strings','events','properties','methods','parameterDefaultValues','fieldDefaultValues','fieldAndParameterDefaultValueData','fieldMarshaledSizes','parameters','fields','genericParameters','genericParameterConstraints','genericContainers','nestedTypes','interfaces','vtableMethods','interfaceOffsets','typeDefinitions','images','assemblies','fieldRefs','referencedAssemblies','attributeData','attributeDataRanges','unresolvedIndirectCallParameterTypes','unresolvedIndirectCallParameterRanges','windowsRuntimeTypeNames','windowsRuntimeStrings','exportedTypeDefinitions']
sec={}
for i,n in enumerate(names):
    off,size,count=struct.unpack_from('<iii',d,8+12*i)
    sec[n]=(off,size,count)
def cstr(idx):
    base=sec['strings'][0]+idx
    e=d.index(b'\0',base)
    return d[base:e].decode('utf-8','replace')
TD=struct.Struct('<iihhhhIiiiiiiiiHHHHHHHHII')
assert TD.size==sec['typeDefinitions'][1]//sec['typeDefinitions'][2], (TD.size, sec['typeDefinitions'])
types=[]
for i in range(sec['typeDefinitions'][2]):
    t=TD.unpack_from(d,sec['typeDefinitions'][0]+i*TD.size)
    types.append(t)
MD=struct.Struct('<ihhIihIHHHH')
methods=[MD.unpack_from(d,sec['methods'][0]+i*MD.size) for i in range(sec['methods'][2])]
PD=struct.Struct('<iIh')  # nameIndex, token, typeIndex
params=[PD.unpack_from(d,sec['parameters'][0]+i*PD.size) for i in range(sec['parameters'][2])]
FD=struct.Struct('<ihI')
fields=[FD.unpack_from(d,sec['fields'][0]+i*FD.size) for i in range(sec['fields'][2])]
PRD=struct.Struct('<iiiII') # name, get, set, attrs, token
props=[PRD.unpack_from(d,sec['properties'][0]+i*PRD.size) for i in range(sec['properties'][2])]
def tname(i):
    t=types[i]; ns=cstr(t[1]); n=cstr(t[0])
    return (ns+'.' if ns else '')+n
def find_type(name):
    return [i for i,t in enumerate(types) if cstr(t[0])==name or tname(i)==name]
cmd=sys.argv[1]
if cmd=='type':
    for ti in find_type(sys.argv[2]):
        t=types[ti]
        print('==',tname(ti),'token',hex(t[24]),'methods',t[15],'fields',t[17],'props',t[16])
        mstart=t[8]
        for k in range(t[15]):
            m=methods[mstart+k]
            pnames=[cstr(params[m[4]+p][0]) for p in range(m[10])]
            print(f'  slot={m[9] if m[9]!=0xffff else "-"} {cstr(m[0])}({", ".join(pnames)}) token={hex(m[6])}')
        fstart=t[7]
        for k in range(t[17]):
            f=fields[fstart+k]
            print('  field',cstr(f[0]))
elif cmd=='attrs':
    ranges=[struct.unpack_from('<II',d,sec['attributeDataRanges'][0]+i*8) for i in range(sec['attributeDataRanges'][2])]
    bytok={r[0]:i for i,r in enumerate(ranges)}
    def rd(pos):
        b=d[pos]
        if b<0x80: return b,pos+1
        if (b&0xC0)==0x80: return ((b&0x3f)<<8)|d[pos+1],pos+2
        if (b&0xE0)==0xC0: return ((b&0x1f)<<24)|(d[pos+1]<<16)|(d[pos+2]<<8)|d[pos+3],pos+4
        if b==0xF0: return struct.unpack_from('<I',d,pos+1)[0],pos+5
        return 0xffffffff,pos+1
    def show(tok,label):
        if tok not in bytok: return
        i=bytok[tok]; start=ranges[i][1]; end=ranges[i+1][1] if i+1<len(ranges) else sec['attributeData'][1]
        pos=sec['attributeData'][0]+start
        cnt,pos=rd(pos); ctors=[]
        for _ in range(cnt):
            c,pos=rd(pos); ctors.append(c)
        blob=d[pos:sec['attributeData'][0]+end]
        print(label,[tname(methods[c][1]) for c in ctors],blob)
    for ti in find_type(sys.argv[2]):
        t=types[ti]
        show(t[24],'type '+tname(ti))
        for k in range(t[15]):
            m=methods[t[8]+k]; show(m[6],'  method '+cstr(m[0]))
        for k in range(t[17]):
            f=fields[t[7]+k]; show(f[2],'  field '+cstr(f[0]))
        for k in range(t[16]):
            p=props[t[10]+k]; show(p[4],'  prop '+cstr(p[0]))
elif cmd=='vtable':
    # interfaces: TypeIndex entries (2 bytes); interfaceOffsets: (TypeIndex 2 bytes, int32 offset) = 6 bytes; vtableMethods: uint32 encoded
    for ti in find_type(sys.argv[2]):
        t=types[ti]
        print('==',tname(ti),'vtable_count',t[20],'interfaces',t[21],'interfaceOffsets',t[22])
        ioffs=[]
        for k in range(t[22]):
            o=sec['interfaceOffsets'][0]+(t[14]+k)*6
            tix,off=struct.unpack_from('<hi',d,o)
            ioffs.append((tix,off))
        print('  interfaceOffsets (typeIndex, slotOffset):',ioffs)
        for k in range(t[20]):
            enc,=struct.unpack_from('<I',d,sec['vtableMethods'][0]+(t[13]+k)*4)
            kind=enc>>29; idx=enc&0x1fffffff
            if kind==3 and idx<len(methods):
                m=methods[idx]; print(f'  vslot {k}: {tname(m[1])}.{cstr(m[0])} (defslot {m[9]})')
            else:
                print(f'  vslot {k}: kind={kind} idx={idx}')
