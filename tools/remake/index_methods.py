# Index every ShadowRunPrototype_* (and other Assembly-CSharp) method definition: file, start line, end line
import re, json, sys, os
files = ['Assembly-CSharp.cpp','Assembly-CSharp__1.cpp','Assembly-CSharp__2.cpp']
pat = re.compile(r'^IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR .*? ([A-Za-z0-9_]+_m[0-9A-F]{40}) \((.*)$')
index = {}
for f in files:
    lines = open(f, encoding='utf-8', errors='replace').read().split('\n')
    i = 0
    n = len(lines)
    while i < n:
        m = pat.match(lines[i])
        if m:
            name = m.group(1)
            # find opening brace line (may be same line or following)
            j = i
            isdef = False
            while j < min(i+3, n):
                if lines[j].rstrip().endswith('{'):
                    isdef = True; break
                if lines[j].rstrip().endswith(';'):
                    break
                j += 1
            if isdef:
                # end at next line exactly '}'
                k = j+1
                while k < n and lines[k] != '}':
                    k += 1
                index[name] = {'file': f, 'start': i+1, 'end': k+1, 'lines': k - i + 1}
                i = k
        i += 1
json.dump(index, open(sys.argv[1],'w'), indent=0)
srp = {k:v for k,v in index.items() if k.startswith('ShadowRunPrototype_')}
print('total defs', len(index), 'ShadowRunPrototype defs', len(srp))
tot = sum(v['lines'] for v in srp.values())
print('ShadowRunPrototype total lines', tot)
big = sorted(srp.items(), key=lambda kv: -kv[1]['lines'])[:25]
for k,v in big: print(v['lines'], k)
