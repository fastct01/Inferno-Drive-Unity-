#!/usr/bin/env python3
"""Async state-machine simplifier: python3 asy.py <MoveNextName>  (runs m2.py and rewrites awaits)."""
import sys,re,subprocess
name=sys.argv[1]
out=subprocess.run(['python3',sys.argv[0].replace('asy.py','m2.py'),name],capture_output=True,text=True).stdout
lines=out.split('\n')
awaiters={}   # vK -> expr
res=[]
i=0
skip_patterns=[
 r'^\s*il2cpp::utils::ExceptionSupportStack',
 r'^\s*v0 = this\.U3CU3E1__state;$',
 r'^\s*v0 = 0;$',
 r'^\s*l\d+ = \d+;$',
 r'^\s*v0 = l\d+;$',
 r'^\s*this\.U3CU3E1__state = l\d+;$',
 r'^\s*this\.U3CU3E1__state = \(-2\);$',
 r'^\s*this\.U3CU3Eu__\d+ = v\d+;$',
 r'^\s*l\d+ = \(Async\w+MethodBuilder(_1)?\)\(&this\.U3CU3Et__builder\);$',
 r'^\s*Async\w+MethodBuilder(_1)?_AwaitUnsafeOnCompleted',
 r'^\s*Async\w+MethodBuilder(_1)?_SetResult\(',
 r'^\s*Async\w+MethodBuilder(_1)?_SetException\(',
 r'^\s*v\d+ = this\.U3CU3Eu__\d+;$',
 r'^\s*l\d+ = \(TaskAwaiter(_1)?\)\(&this\.U3CU3Eu__\d+\);$',
 r'^\s*l\d+ = \(YieldAwaiter\)\(&this\.U3CU3Eu__\d+\);$',
 r'^\s*initobj\(l\d+, sizeof\((TaskAwaiter(_1)?|YieldAwaiter)\)\);$',
 r'^\s*l\d+ = \(-1\);$',
 r'^\s*IL2CPP_PUSH_ACTIVE_EXCEPTION',
 r'^\s*IL2CPP_POP_ACTIVE_EXCEPTION',
 r'^\s*throw e;$',
 r'^\s*if\(class_is_assignable_from',
 r'^\s*catch\(Il2CppExceptionWrapper& e\)$',
 r'^\s*__finallyBlock\.StoreException',
 r'^\s*auto __finallyBlock = il2cpp::utils::Finally\(\[&\]$',
 r'^\s*FINALLY_[0-9a-f_]+:$',
 r'^\s*l\d+ = TaskAwaiter(_1)?_get_IsCompleted\(ref v\d+.*\);$',
 r'^\s*l\d+ = YieldAwaiter_get_IsCompleted\(ref v\d+\);$',
 r'^\s*v\d+ = 0;$',
]
skip=[re.compile(p) for p in skip_patterns]
n=len(lines)
while i<n:
    l=lines[i]
    s=l.strip()
    # awaiter capture: lN = Task.1_GetAwaiter(EXPR, ...); vK = lN;   or lN = Task.GetAwaiter(EXPR); vK = lN;
    m=re.match(r'^\s*(l\d+) = Task\.(?:1_)?GetAwaiter\((.*?)(?:, Task_1_GetAwaiter)?\);$',l)
    if m and i+1<n:
        m2=re.match(r'^\s*(v\d+) = '+m.group(1)+';$',lines[i+1])
        if m2:
            awaiters[m2.group(1)]=m.group(2); i+=2; continue
    m=re.match(r'^\s*(l\d+) = Task\.Yield\(\);$',l)
    if m and i+3<n:
        awaiters['__yield']='Task.Yield()'
        # lines: v3 = l7; l8 = YieldAwaitable_GetAwaiter(ref v3); v2 = l8;
        j=i+1
        mv=re.match(r'^\s*(v\d+) = '+m.group(1)+';$',lines[j])
        if mv:
            j+=1
            mg=re.match(r'^\s*(l\d+) = YieldAwaitable_GetAwaiter\(ref '+mv.group(1)+r'\);$',lines[j])
            if mg:
                j+=1
                mv2=re.match(r'^\s*(v\d+) = '+mg.group(1)+';$',lines[j])
                if mv2:
                    awaiters[mv2.group(1)]='Task.Yield()'; i=j+1; continue
    m=re.match(r'^(\s*)(l\d+) = TaskAwaiter(?:_1)?_GetResult\(ref (v\d+)(?:, TaskAwaiter_1_GetResult)?\);$',l)
    if m:
        res.append(f'{m.group(1)}{m.group(2)} = await {awaiters.get(m.group(3),"?")};'); i+=1; continue
    m=re.match(r'^(\s*)TaskAwaiter_GetResult\(ref (v\d+)\);$',l)
    if m:
        res.append(f'{m.group(1)}await {awaiters.get(m.group(2),"?")};'); i+=1; continue
    m=re.match(r'^(\s*)YieldAwaiter_GetResult\(ref (v\d+)\);$',l)
    if m:
        res.append(f'{m.group(1)}await Task.Yield();'); i+=1; continue
    # 'if (lX) goto IL_..;' right after an IsCompleted line was already skipped: drop 'if (lN) goto' when lN undefined
    if any(p.match(l) for p in skip): i+=1; continue
    # drop state dispatch: 'if (!v0) goto', 'if ((v0 == 1)) goto', switch(v0) blocks, 'if ((!(v0 > 1))) goto'
    if re.match(r'^\s*if \(\(?!?\(?v0( == \d+)?\)?\)?\)? goto IL_',l) or re.match(r'^\s*if \(\(!\(v0 > \d+\)\)\) goto',l) or re.match(r'^\s*if \(\(!\(\(\(\(v0 - 1\)\)\) > \d+\)\)\) goto',l): i+=1; continue
    if re.match(r'^\s*switch \(\(?\(?v0( - 1)?\)?\)?\)$',l):
        # skip until closing brace of switch
        depth=0; j=i+1
        while j<n:
            if lines[j].strip()=='{': depth+=1
            elif lines[j].strip()=='}':
                depth-=1
                if depth==0: break
            j+=1
        i=j+1; continue
    if re.match(r'^\s*(if \((v0 >= 0)\)) goto IL_',l): i+=1; continue
    # goto to resume label / dispatch (IL_xxxx_n: labels of resume points) - keep, hard to tell
    s2=re.sub(r'this\.U3C(\w+)U3E5__\d+',r'\1',l)
    s2=re.sub(r'this\.U3CU3E8__1->___',r'closure.',s2)
    s2=re.sub(r'this\.U3CU3E4__this->___','this.',s2)
    s2=s2.replace('this.U3CU3E4__this','this')
    s2=re.sub(r'\bv1->___','this.',s2)
    res.append(s2)
    i+=1
# remove 'if (lN) goto' where lN never defined
defined=set(re.findall(r'\b(l\d+) = ',' '.join(res)))
final=[]
for l in res:
    m=re.match(r'^\s*if \(!?(l\d+)\) goto IL_',l)
    if m and m.group(1) not in defined: continue
    final.append(l)
print('\n'.join(final))
