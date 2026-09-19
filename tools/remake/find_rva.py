import hashlib, struct, sys, re
d = open('Data/Managed/Metadata/global-metadata.dat','rb').read()
targets = {}
for line in open(sys.argv[1]):
    m = re.match(r'__StaticArrayInitTypeSizeU3D(\d+)_t\w+ ___([0-9A-F]{64})', line.strip())
    if m: targets[m.group(2)] = int(m.group(1))
# Field default data lives in the fieldAndParameterDefaultValueData blob; scan the whole file with each size (aligned to 4 first, then unaligned)
found = {}
for h, size in targets.items():
    for align in (4, 1):
        for off in range(0, len(d)-size+1, align):
            if hashlib.sha256(d[off:off+size]).hexdigest().upper() == h:
                found[h] = (off, d[off:off+size]); break
        if h in found: break
for h, size in targets.items():
    if h not in found: print('NOT FOUND', h, size); continue
    off, b = found[h]
    print(f'{h[:8]} size={size} off={off}')
    if size % 4 == 0 and size <= 64:
        print('  floats:', struct.unpack('<%df' % (size//4), b))
        print('  ints:  ', struct.unpack('<%di' % (size//4), b))
    open(f'{sys.argv[2]}/rva_{h[:8]}.bin','wb').write(b)
