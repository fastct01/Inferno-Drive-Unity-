import struct, hashlib, json, sys
d = open('Data/Managed/Metadata/global-metadata.dat','rb').read()
tabOff, tabBytes, count, dataOff = struct.unpack('<IIII', d[8:24])
print("tabOff",tabOff,"tabBytes",tabBytes,"count",count,"dataOff",dataOff, file=sys.stderr)
offs = struct.unpack('<%dI' % count, d[tabOff:tabOff+4*count])
table = {}
for i in range(count-1):
    s = d[dataOff+offs[i]:dataOff+offs[i+1]].decode('utf-8', errors='replace')
    table[hashlib.sha1(s.encode('utf-16-le')).hexdigest().upper()] = s
json.dump(table, open(sys.argv[1],'w'), ensure_ascii=False, indent=0)
print(len(table), file=sys.stderr)
