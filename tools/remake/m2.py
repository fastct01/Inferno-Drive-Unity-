#!/usr/bin/env python3
"""Dense IL2CPP -> pseudo-C# viewer.  m2.py <MethodName|substring> [--raw] [--all]"""
import json, re, sys, os
SP = os.path.dirname(os.path.abspath(__file__))
BASE = '/Users/fast-ct/Downloads/car точно финал/Il2CppOutputProject/Source/il2cppOutput'
idx = json.load(open(SP+'/methods.json'))
strs = json.load(open(SP+'/strings.json'))
args = [a for a in sys.argv[1:] if not a.startswith('--')]
raw = '--raw' in sys.argv
cache = {}
def lines_of(f):
    if f not in cache: cache[f] = open(BASE+'/'+f, encoding='utf-8', errors='replace').read().split('\n')
    return cache[f]
lit = re.compile(r'_stringLiteral([0-9A-F]{40})')
def sublit(m):
    s = strs.get(m.group(1))
    return json.dumps(s, ensure_ascii=False) if s is not None else m.group(0)

TYPE_DECL = re.compile(r'^\s*(?:const\s+)?(?!return\b|goto\b|throw\b|delete\b|new\b|else\b|case\b)(?:[A-Za-z_][\w:<>]*\*?\s+)(?:const\s+)?\*?\s*([lvgLVG]_?[A-Za-z0-9_]*)\s*(=\s*(.*))?;\s*$')

def clean(s):
    s = lit.sub(sublit, s)
    s = re.sub(r'_t[0-9A-F]{40}', '', s)
    s = re.sub(r'_m[0-9A-F]{40}(_RuntimeMethod_var)?', '', s)
    s = re.sub(r'_[0-9A-F]{40}_RuntimeMethod_var', '', s)
    s = s.replace('/*hidden argument*/', '')
    s = re.sub(r'Object_op_Inequality\(([^,]+), \(\w+\*\)NULL\)', r'(\1 != null)', s)
    s = re.sub(r'Object_op_Equality\(([^,]+), \(\w+\*\)NULL\)', r'(\1 == null)', s)
    s = re.sub(r'ShadowRunPrototype_get_(\w+)\(__this\)', r'this.\1', s)
    s = re.sub(r',\s*NULL\)', ')', s)
    s = re.sub(r'\(NULL\)', '()', s)
    s = re.sub(r'\(\s*const RuntimeMethod\*\s*method\s*\)', '()', s)
    s = re.sub(r'\(RuntimeObject\*\)', '', s)
    s = re.sub(r'\bil2cpp_codegen_multiply\(', 'MUL(', s)
    s = re.sub(r'\bil2cpp_codegen_add\(', 'ADD(', s)
    s = re.sub(r'\bil2cpp_codegen_subtract\(', 'SUB(', s)
    s = re.sub(r'\bil2cpp_codegen_object_new\([A-Za-z0-9_]+_il2cpp_TypeInfo_var\)', 'NEW', s)
    s = re.sub(r'\(([A-Za-z0-9_]+)\*\)\s*NEW', r'new \1', s)
    s = re.sub(r'\(([A-Za-z0-9_]+U5BU5D)\*\)\(\1\*\)SZArrayNew\(\1_il2cpp_TypeInfo_var, \(uint32_t\)(\w+)\)', lambda m: f'new {m.group(1).replace("U5BU5D","")}[{m.group(2)}]', s)
    s = re.sub(r'\(([A-Za-z0-9_]+U5BU5D)\*\)SZArrayNew\(\1_il2cpp_TypeInfo_var, \(uint32_t\)(\w+)\)', lambda m: f'new {m.group(1).replace("U5BU5D","")}[{m.group(2)}]', s)
    s = re.sub(r'U5BU5D', '[]', s)
    s = re.sub(r'__this->___', 'this.', s)
    s = re.sub(r'\(\((\w+)_StaticFields\)static_fields_for\(\1_il2cpp_TypeInfo_var\)\)->___', r'\1.', s)
    s = re.sub(r'\(\((\w+)_StaticFields\*\)il2cpp_codegen_static_fields_for\(\1_il2cpp_TypeInfo_var\)\)->___', r'\1.', s)
    s = re.sub(r'\(__this\)', '()', s)
    s = re.sub(r'\(__this, ', '(', s)
    s = re.sub(r'\b___(\d+)_(\w+)', r'\2', s)
    s = re.sub(r'\bV_(\d+)\b', r'v\1', s)
    s = re.sub(r'\bL_(\d+)\b', r'l\1', s)
    s = re.sub(r'\bG_B(\d+)_(\d+)\b', r'g\1_\2', s)
    s = re.sub(r'\(\((int32_t|float|double|uint32_t|int64_t|uint8_t|int16_t)\)(\w+)\)', r'\2', s)
    s = re.sub(r'\((int32_t|uint32_t)\)', '', s)
    s = re.sub(r'\(bool\)1\b', 'true', s); s = re.sub(r'\(bool\)0\b', 'false', s)
    s = re.sub(r'\(\s*\(?(?:Object|RuntimeObject|GameObject|Transform|Component|String_t|[A-Za-z0-9_]+)\*\)?\s*NULL\)', 'null', s)
    s = re.sub(r'\(([A-Za-z0-9_]+)\*\)NULL', 'null', s)
    s = re.sub(r'\bNULL\b', 'null', s)
    s = re.sub(r'\(([A-Za-z0-9_]+)\*\)\s*(\w)', r'(\1)\2', s)
    s = re.sub(r'\(([A-Za-z0-9_]+)\*\)', r'(\1)', s)
    s = re.sub(r'\bMUL\(([^()]*(?:\([^()]*\))*[^()]*), ([^()]*(?:\([^()]*\))*[^()]*)\)', r'(\1 * \2)', s)
    s = re.sub(r'\bADD\(([^()]*(?:\([^()]*\))*[^()]*), ([^()]*(?:\([^()]*\))*[^()]*)\)', r'(\1 + \2)', s)
    s = re.sub(r'\bSUB\(([^()]*(?:\([^()]*\))*[^()]*), ([^()]*(?:\([^()]*\))*[^()]*)\)', r'(\1 - \2)', s)
    s = re.sub(r'\(\(float\)(\(.*?\))\)', r'\1', s)
    s = re.sub(r'\(\(float\)\(', '((', s)
    s = re.sub(r'\(float\)\(', '(', s)
    s = s.replace('_inline(', '(')
    s = re.sub(r'\(\(\((\w+)\) < \((\w+)\)\)\)', r'(\1 < \2)', s)
    for op in ['<', '>', '<=', '>=', '==', '!=']:
        s = re.sub(r'\(\(\((\w[\w\.]*)\) '+re.escape(op)+r' \((\w[\w\.]*)\)\)\)', r'(\1 '+op+r' \2)', s)
        s = re.sub(r'\(\(\((\w[\w\.]*)\) '+re.escape(op)+r' \((\w[\w\.]*)\)\)\)', r'(\1 '+op+r' \2)', s)
    s = re.sub(r'\(!\(\(\((\w+)\) == \(\(uint32_t\)(\w+)\)\)\)\)', r'(\1 != \2)', s)
    s = re.sub(r'\(!\(\(\((\w+)\) == \((\w+)\)\)\)\)', r'(\1 != \2)', s)
    s = re.sub(r'\(\(\((\w+)\) == \(\(uint32_t\)(\w+)\)\)\)', r'(\1 == \2)', s)
    s = re.sub(r'\(\(uint32_t\)(\w+)\)', r'\1', s)
    s = re.sub(r'\(\((\w+)\) (==|!=|<|>|<=|>=) \((\w+)\)\)', r'(\1 \2 \3)', s)
    s = re.sub(r'Object_op_Inequality\(([^,]+), null\)', r'(\1 != null)', s)
    s = re.sub(r'Object_op_Equality\(([^,]+), null\)', r'(\1 == null)', s)
    s = re.sub(r'Object_op_Inequality\((\w+), (\w+)\)', r'(\1 != \2)', s)
    s = re.sub(r'Object_op_Equality\((\w+), (\w+)\)', r'(\1 == \2)', s)
    s = re.sub(r'Object_op_Implicit\((\w+)\)', r'(bool)\1', s)
    s = re.sub(r'String_op_Equality\(([^,]+), ([^)]+)\)', r'(\1 == \2)', s)
    s = re.sub(r'String_op_Inequality\(([^,]+), ([^)]+)\)', r'(\1 != \2)', s)
    s = re.sub(r'String_IsNullOrEmpty\(', 'string.IsNullOrEmpty(', s)
    s = re.sub(r'String_Concat\(', 'string.Concat(', s)
    s = re.sub(r'String_Format\(', 'string.Format(', s)
    s = re.sub(r'\bMathf_(\w+)\(', r'Mathf.\1(', s)
    s = re.sub(r'\b(\w+)__ctor\(\(&(\w+)\), ', r'\2 = new \1(', s)
    s = re.sub(r'\b(\w+)__ctor\((\w+), ', r'\2.ctor(', s)
    s = re.sub(r'\b(\w+)__ctor\((\w+)\)', r'\2.ctor()', s)
    s = re.sub(r'\b(\w+)_set_(\w+)\((\w[\w\.]*), ', r'\3.\2 = (', s)
    s = re.sub(r'\b(\w+)_get_(\w+)\((\w[\w\.]*)\)', r'\3.\2', s)
    s = re.sub(r'\b(\w+)_get_(\w+)\(\)', r'\1.\2', s)
    s = re.sub(r'\b(\w+)_set_(\w+)\(', r'\1.\2 = (', s)
    s = re.sub(r'\bShadowRunPrototype_(\w+)\(', r'\1(', s)
    s = re.sub(r'\b(Object|GameObject|Component|Transform|Resources|Debug|PlayerPrefs|Application|QualitySettings|Screen|Time|Physics|Random|Color|Vector2|Vector3|Vector4|Quaternion|Rect|Material|Texture2D|Shader|Sprite|Font|RenderSettings|Camera|Light|Canvas|Graphic|Image|RawImage|Text|Button|RectTransform|CanvasGroup|AudioSource|AudioListener|Renderer|MeshRenderer|MeshFilter|Mesh|ParticleSystem|Enumerable|Array|String|List_1|Dictionary_2|HashSet_1|Math|Device|Handheld|Input|SystemInfo|Enum|Int32|Single|Boolean|Char|DateTime|TimeSpan|Task|Path|Environment|Convert|Keyboard|Mouse|Touchscreen|EventSystem|Gizmos|Bounds|Matrix4x4|Mathf|LayerMask|SceneManager|Gradient|AnimationCurve|Keyframe|Exception|StringBuilder|CultureInfo|Guid|Nullable_1|Func_\d|Action(?:_\d)?|UnityEvent|UnityAction|ColorUtility|JsonUtility|Sprite|TextureUtil|ReflectionProbe|Volume|Enumerator|KeyValuePair_2|Il2CppCodeGenWriteBarrier|TransformU5BU5D|Behaviour|MonoBehaviour|ScriptableObject|UnityEngine)_(\w+)\(', r'\1.\2(', s)
    s = re.sub(r'\b([A-Z]\w+)_(Tis\w+?)\(', r'\1<\2>(', s)
    s = re.sub(r'<Tis(\w+)>', r'<\1>', s)
    s = re.sub(r'VirtualActionInvoker(\d)(?:<[^>]*>)?::Invoke\((\d+), (\w[\w\.]*), ', r'\3.VIRT\1[\2](', s)
    s = re.sub(r'VirtualActionInvoker0::Invoke\((\d+), (\w[\w\.]*)\)', r'\2.VIRT0[\1]()', s)
    s = re.sub(r'VirtualFuncInvoker(\d)<([^>]*)>::Invoke\((\d+), (\w[\w\.]*), ', r'\4.VIRTF\1[\3](', s)
    s = re.sub(r'VirtualFuncInvoker0<([^>]*)>::Invoke\((\d+), (\w[\w\.]*)\)', r'\3.VIRTF0[\2]()', s)
    s = re.sub(r'InterfaceActionInvoker(\d)(?:<[^>]*>)?::Invoke\((\d+), (\w+)_il2cpp_TypeInfo_var, (\w[\w\.]*), ', r'\4.IFACE\3.\1[\2](', s)
    s = re.sub(r'InterfaceFuncInvoker(\d)<([^>]*)>::Invoke\((\d+), (\w+)_il2cpp_TypeInfo_var, (\w[\w\.]*), ', r'\5.IFACE\4.\1[\3](', s)
    s = re.sub(r'InterfaceFuncInvoker0<([^>]*)>::Invoke\((\d+), (\w+)_il2cpp_TypeInfo_var, (\w[\w\.]*)\)', r'\4.IFACE\3.0[\2]()', s)
    s = re.sub(r'GenericVirtual\w*Invoker\d?<?[^>]*>?::Invoke\(', 'GVIRT(', s)
    s = re.sub(r'\(&(\w+)\)', r'ref \1', s)
    s = re.sub(r'\bref (\w+)\.', r'\1.', s) if False else s
    s = re.sub(r'\(\w+\)\s*SetAt\(', 'SetAt(', s)
    s = re.sub(r'\((\w+)\)->SetAt\(static_cast<il2cpp_array_size_t>\((\w+)\), (\(\w+\))?\s*', r'\1[\2] = (', s)
    s = re.sub(r'\((\w+)\)->GetAt\(static_cast<il2cpp_array_size_t>\((\w+)\)\)', r'\1[\2]', s)
    s = re.sub(r'\((\w+)\)->max_length', r'\1.Length', s)
    s = re.sub(r'\bil2cpp_codegen_is_fake_boxed_object_null\(([^)]*)\)', r'((\1) == null)', s)
    s = re.sub(r'\(\((\w[\w\.\[\]]*)\) (\+|-|\*|/) \((\w[\w\.\[\]]*)\)\)', r'(\1 \2 \3)', s)
    s = re.sub(r'\bil2cpp_codegen_', '', s)
    s = re.sub(r'\bfabsf\(', 'Mathf.Abs(', s)
    s = re.sub(r'\bsqrtf\(', 'Mathf.Sqrt(', s)
    s = s.replace('__ctor', '.ctor')
    return s

for k in ([args[0]] if args[0] in idx else ([kk for kk in idx if re.fullmatch(r'ShadowRunPrototype_'+re.escape(args[0])+r'_m[0-9A-F]{40}', kk)] or [kk for kk in idx if args[0] in kk])) if args else []:
    v = idx[k]
    L = lines_of(v['file'])[v['start']-1:v['end']]
    print(f'// ===== {k}  [{v["file"]}:{v["start"]}-{v["end"]}, {v["lines"]} lines]')
    if raw:
        print('\n'.join(L)); continue
    # pass 1: strip boilerplate
    out = []
    for ln in L[1:]:
        s = ln.rstrip()
        st = s.strip()
        if not st: continue
        if st.startswith('static bool s_Il2CppMethodInitialized') or st.startswith('if (!s_Il2CppMethodInitialized)') or st.startswith('il2cpp_codegen_initialize_runtime_metadata') or st == 's_Il2CppMethodInitialized = true;' or st.startswith('il2cpp_codegen_runtime_class_init_inline') or st.startswith('NullCheck(') or st.startswith('Il2CppCodeGenWriteBarrier') or st.startswith('IL2CPP_RUNTIME_CLASS_INIT') or st.startswith('IL2CPP_RAISE_MANAGED_EXCEPTION') and False:
            continue
        if re.match(r'^memset\(', st): continue
        out.append(st)
    # pass 2: clean and merge if/goto
    res = []
    depth_stack = []
    i = 0
    while i < len(out):
        st = out[i]
        # if (...) { goto IL_x; }  -> spread over 4 lines? Since braces removed, pattern is: if (cond) / goto IL_x;
        if st.startswith('if (') and i+3 < len(out) and out[i+1] == '{' and out[i+2].startswith('goto ') and out[i+3] == '}':
            cond = clean(st[3:].strip())
            res.append(f'if {cond} {out[i+2]}')
            i += 4; continue
        if st in ('{', '}'):
            # drop the redundant top-level block braces: a '{' directly following a label or at start, and its '}' — keep only when preceded by if/else/try/catch/for/while
            prev = res[-1] if res else ''
            if st == '{' and not re.match(r'^(if|else|try|catch|for|while|do|switch)\b', prev) and not prev.startswith('auto __finallyBlock'):
                depth_stack.append('drop'); i += 1; continue
            if st == '}' and depth_stack and depth_stack[-1] == 'drop':
                depth_stack.pop(); i += 1; continue
            if st == '{': depth_stack.append('keep')
            elif depth_stack: depth_stack.pop()
            res.append(st); i += 1; continue
        c = clean(st)
        m = TYPE_DECL.match(c)
        if m:
            if m.group(2):
                c = f'{m.group(1)} = {m.group(3)};'
            else:
                i += 1; continue  # bare declaration
        res.append(c)
        i += 1
    # pass 3: inline single-use SSA temporaries that are simple copies
    simple = re.compile(r'^(l\d+) = ([\w\.\[\]"]+|-?[\d\.]+f?|this\.\w+|v\d+|g\d+_\d+|[a-zA-Z_]\w*);$')
    text = res
    changed = True
    while changed:
        changed = False
        for j, c in enumerate(text):
            m = simple.match(c)
            if not m: continue
            name, val = m.group(1), m.group(2)
            # count uses after j
            uses = [kx for kx in range(j+1, len(text)) if re.search(r'\b'+name+r'\b', text[kx])]
            if len(uses) == 1 and not re.match(r'^'+name+r' = ', text[uses[0]]):
                # do not fold a field/local load across a later write to the same field/local
                # (captures like `var a = field; field = null; a?.Invoke()` must keep the temp)
                blocked = False
                if re.match(r'^(this\.\w+|[A-Za-z_]\w*\.\w+|v\d+|g\d+_\d+|[a-zA-Z_]\w*)$', val) and not re.match(r'^-?[\d\.]+f?$', val):
                    for kx in range(j+1, uses[0]):
                        if re.match(r'^'+re.escape(val)+r' = ', text[kx]) or re.match(r'^ref '+re.escape(val)+r'->', text[kx]):
                            blocked = True; break
                if blocked:
                    continue
                text[uses[0]] = re.sub(r'\b'+name+r'\b', val, text[uses[0]])
                del text[j]
                changed = True
                break
            elif len(uses) == 0:
                del text[j]; changed = True; break
    # pass 4: label formatting
    for c in text:
        if re.match(r'^IL_[0-9a-f]+:$', c): print(c)
        else: print('  ' + c)
