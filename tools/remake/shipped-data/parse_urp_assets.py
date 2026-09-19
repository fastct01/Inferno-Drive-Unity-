import struct,sys
class R:
    def __init__(s,b): s.b=b; s.o=0
    def i32(s): v=struct.unpack_from('<i',s.b,s.o)[0]; s.o+=4; return v
    def u32(s): v=struct.unpack_from('<I',s.b,s.o)[0]; s.o+=4; return v
    def i64(s): v=struct.unpack_from('<q',s.b,s.o)[0]; s.o+=8; return v
    def f32(s): v=round(struct.unpack_from('<f',s.b,s.o)[0],6); s.o+=4; return v
    def bool(s): v=s.b[s.o]; s.o+=1; s.align(); return bool(v)
    def u8(s): v=s.b[s.o]; s.o+=1; s.align(); return v
    def align(s): s.o=(s.o+3)&~3
    def pptr(s): return (s.i32(), s.i64())
    def str(s):
        n=s.i32(); v=s.b[s.o:s.o+n].decode('utf-8'); s.o+=n; s.align(); return v
    def header(s):
        go=s.pptr(); en=s.b[s.o]; s.o+=1; s.align(); sc=s.pptr(); name=s.str(); return go,en,sc,name

def renderer(path):
    r=R(open(path,'rb').read()); print('==',path, r.header())
    print(' debugShaders',[r.pptr() for _ in range(3)])
    print(' probeVolumeResources',[r.pptr() for _ in range(6)])
    n=r.i32(); print(' m_RendererFeatures',[r.pptr() for _ in range(n)])
    n=r.i32(); print(' m_RendererFeatureMap',[r.i64() for _ in range(n)])
    print(' m_UseNativeRenderPass',r.bool())
    print(' xrSystemData',r.pptr(),'postProcessData',r.pptr())
    print(' m_AssetVersion',r.i32())
    print(' prepass/opaque/transparent masks',r.i32(),r.i32(),r.i32())
    print(' stencil override',r.bool(),'ref',r.i32(),'cmp',r.i32(),'pass',r.i32(),'fail',r.i32(),'zfail',r.i32())
    print(' m_ShadowTransparentReceive',r.bool())
    print(' m_RenderingMode',r.i32(),'m_DepthPrimingMode',r.i32(),'m_CopyDepthMode',r.i32())
    print(' m_DepthAttachmentFormat',r.i32(),'m_DepthTextureFormat',r.i32())
    print(' m_AccurateGbufferNormals',r.bool(),'m_IntermediateTextureMode',r.i32())
    print(' consumed',r.o,'of',len(r.b),'rest',r.b[r.o:].hex())

def rpasset(path):
    r=R(open(path,'rb').read()); print('==',path, r.header())
    P=lambda k,v: print(' ',k,v)
    P('k_AssetVersion',r.i32()); P('k_AssetPreviousVersion',r.i32()); P('m_RendererType',r.i32()); P('m_RendererData',r.pptr())
    n=r.i32(); P('m_RendererDataList',[r.pptr() for _ in range(n)]); P('m_DefaultRendererIndex',r.i32())
    P('m_RequireDepthTexture',r.bool()); P('m_RequireOpaqueTexture',r.bool()); P('m_OpaqueDownsampling',r.i32()); P('m_SupportsTerrainHoles',r.bool())
    P('m_SupportsHDR',r.bool()); P('m_HDRColorBufferPrecision',r.i32()); P('m_MSAA',r.i32()); P('m_RenderScale',r.f32())
    P('m_UpscalingFilter',r.i32()); P('m_FsrOverrideSharpness',r.bool()); P('m_FsrSharpness',r.f32())
    P('m_EnableLODCrossFade',r.bool()); P('m_LODCrossFadeDitheringType',r.i32())
    P('m_ShEvalMode',r.i32()); P('m_LightProbeSystem',r.i32()); P('m_ProbeVolumeMemoryBudget',r.i32()); P('m_ProbeVolumeBlendingMemoryBudget',r.i32())
    P('m_SupportProbeVolumeGPUStreaming',r.bool()); P('m_SupportProbeVolumeDiskStreaming',r.bool()); P('m_SupportProbeVolumeScenarios',r.bool()); P('m_SupportProbeVolumeScenarioBlending',r.bool())
    P('m_ProbeVolumeSHBands',r.i32())
    P('m_MainLightRenderingMode',r.i32()); P('m_MainLightShadowsSupported',r.bool()); P('m_MainLightShadowmapResolution',r.i32())
    P('m_AdditionalLightsRenderingMode',r.i32()); P('m_AdditionalLightsPerObjectLimit',r.i32()); P('m_AdditionalLightShadowsSupported',r.bool()); P('m_AdditionalLightsShadowmapResolution',r.i32())
    P('m_AdditionalLightsShadowResolutionTierLow',r.i32()); P('..Medium',r.i32()); P('..High',r.i32())
    P('m_ReflectionProbeBlending',r.bool()); P('m_ReflectionProbeBoxProjection',r.bool()); P('m_ReflectionProbeAtlas',r.bool())
    P('m_ShadowDistance',r.f32()); P('m_ShadowCascadeCount',r.i32()); P('m_Cascade2Split',r.f32()); P('m_Cascade3Split',(r.f32(),r.f32())); P('m_Cascade4Split',(r.f32(),r.f32(),r.f32()))
    P('m_CascadeBorder',r.f32()); P('m_ShadowDepthBias',r.f32()); P('m_ShadowNormalBias',r.f32())
    P('m_SoftShadowsSupported',r.bool()); P('m_ConservativeEnclosingSphere',r.bool()); P('m_NumIterationsEnclosingSphere',r.i32()); P('m_SoftShadowQuality',r.i32())
    P('m_AdditionalLightsCookieResolution',r.i32()); P('m_AdditionalLightsCookieFormat',r.i32())
    P('m_UseSRPBatcher',r.bool()); P('m_SupportsDynamicBatching',r.bool()); P('m_MixedLightingSupported',r.bool()); P('m_SupportsLightCookies',r.bool()); P('m_SupportsLightLayers',r.bool())
    P('m_DebugLevel',r.i32()); P('m_StoreActionsOptimization',r.i32()); P('m_UseAdaptivePerformance',r.bool()); P('m_ColorGradingMode',r.i32()); P('m_ColorGradingLutSize',r.i32())
    P('m_AllowPostProcessAlphaOutput',r.bool()); P('m_UseFastSRGBLinearConversion',r.bool()); P('m_SupportDataDrivenLensFlare',r.bool()); P('m_SupportScreenSpaceLensFlare',r.bool())
    P('m_GPUResidentDrawerMode',r.u8()); P('m_SmallMeshScreenPercentage',r.f32())
    P('m_GPUResidentDrawerEnableOcclusionCullingInCameras',r.bool())
    P('m_ShadowType',r.i32()); P('m_LocalShadowsSupported',r.bool()); P('m_LocalShadowsAtlasResolution',r.i32()); P('m_MaxPixelLights',r.i32()); P('m_ShadowAtlasResolution',r.i32())
    P('m_VolumeFrameworkUpdateMode',r.i32()); P('m_VolumeProfile',r.pptr())
    print('  consumed',r.o,'of',len(r.b),'rest',r.b[r.o:].hex())

for p in sys.argv[1:]:
    (renderer if 'Renderer' in p else rpasset)(p)
