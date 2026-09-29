using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// [user request 2026-09-21: maps] The six lobby maps as data-driven environment profiles.
// Until now a map was only an index into the (stubbed) Dynamic Weather System, so every map rendered the
// same city, the same gray road and a solid dark-red sky. Each profile here owns the sky (a procedural
// 1024x512 panorama on Skybox/Panoramic, or the pulsar nebula cubemap), fog, key light and ambient, the road
// surface (asphalt / wet / sand / snow / glass grid), lane and edge lines, the verge, the shipped buildings,
// neon fences and street lamps (on-off + tint), a scenery set built from primitives under the city scroll
// root (registered in cityObjects, so it loops with the city) and a particle set that follows the camera.
// Everything is Unity-native URP; nothing depends on the weather stub. Neon Night keeps the shipped
// per-frame red light pulse (OwnsLighting is false there); every other map owns its lighting.
// /* [user request 2026-09-24: neon night cycle] */ Neon Night also fades between its night and a daytime
// look during a run (Profile.Day + the Cycle state machine under "day / night cycle"), the way the shipped
// build's weather presets changed on a timer through the Dynamic Weather System that is a stub here.
public sealed partial class ShadowRunPrototype
{
	internal static partial class InfernoMaps
	{
		public const int Count = 6;

		public enum Road { Asphalt, Wet, Sand, Snow, Grid }
		public enum Sky { Day, Night, Overcast, Dust, Aurora, Nebula }
		public enum Scenery { Trees, Totems, GuardRails, Desert, Pines, Crystals }
		public enum Particles { None, Rain, Dust, Snow, Motes }

		// ---- profile schema ----------------------------------------------------------------
		private sealed class Profile
		{
			public string Name;
			public Sky SkyKind; public Road RoadKind; public Scenery SceneryKind; public Particles ParticleKind;
			public int Seed;
			// sky panorama: vertical gradient, stars, sun disc + glow, aurora curtain, cloud band
			public Color SkyTop, SkyHorizon, SkyBottom;
			public float Stars; public Color StarColor;
			public int SkyDetail = 1;                                    // panorama size multiplier: 1 = 1024x512, 2 = 2048x1024 /* [user request 2026-09-26: night drive] */
			public float Galaxy; public Color GalaxyColor = new Color(0.55f, 0.3f, 0.9f);   // faint galactic band: haze + a dense field of small stars
			public float Wisps; public Color WispColor = new Color(0.32f, 0.1f, 0.45f);     // thin high clouds lit from below (Sky.Night)
			public Color HorizonGlow = new Color(0.28f, 0.05f, 0.34f);                      // the city's light pollution above the horizon (Sky.Night)
			public bool Sun; public Vector3 SunDir; public Color SunColor; public float SunSize;
			public bool Aurora; public bool Clouds; public Color CloudColor;
			public Color CloudEdge = new Color(0.74f, 0.78f, 0.86f);   // the lit rim of a Sky.Day cloud /* [user request 2026-09-24: neon night cycle] */
			public float SkyExposure = 1f; public float SkyDrift;
			// fog / light
			public Color Fog; public float FogStart, FogEnd;
			public Color Ambient; public Color KeyColor; public float KeyIntensity; public Vector3 KeyEuler;
			public bool Pulse;       // keep the shipped per-frame red pulse (UpdateSceneLighting)
			public bool Lightning;
			// road
			public Color RoadTint; public float RoadSmoothness, RoadMetallic; public Color RoadEmission;
			public bool Lanes; public Color LaneColor, LaneEmission, EdgeColor, EdgeEmission;
			public Color Verge; public float VergeSmoothness;
			// shipped city pieces
			public bool Buildings; public Color BuildingTint = Color.white;
			public bool Fences; public Color FenceColor; public float FenceGlow;
			public bool Lamps; public Color LampEmission;
			// extras
			public bool RainAudio; public float RainVolume;
			public bool Pulsar; public bool FallingStars; public bool Planet; public bool Moon;
			public Profile Day;      // /* [user request 2026-09-24: neon night cycle] */ the daytime look a run fades to and from (null = a static map)
			public bool NightHazards;   // /* [user request 2026-09-26: night drive] */ blackouts, road-works barriers, police checkpoints and debris (InfernoNightHazards)
		}

		// Shipped values kept as the Neon Night profile: key (1, 0.4, 0.34) x1.85 @ (25, -165, 0) and ambient
		// (0.12, 0.05, 0.065) from MakeLights, road tint / emission from ApplyRoadTexture, lineMat and
		// neonFenceMat / street-lamp emission from MakeMaterials / ApplyStreetLampRedEmission.
		/* [user request 2026-09-24: neon night cycle] */
		// Neon Night by day: the shipped Day weather preset (BuildDayPreset / ApplyFastSkyDayProfile) - the red
		// inferno sky (day colour (0.5, 0.025, 0.045), evening (1.1, 0.1, 0.045), red-tinted clouds), red fog
		// (1, 0.09, 0.055), key (1, 0.42, 0.3) from (18, -32, 0), ambient (0.18, 0.08, 0.09) - lifted like the
		// remake's other lights. Only the sky, fog, camera background, key light, ambient, street lamps and fence
		// glow are taken from it; road, lines, verge, buildings and scenery stay the night profile's.
		private static readonly Profile NeonDay = new Profile
		{
			Name = "NEON NIGHT - DAY", SkyKind = Sky.Day, RoadKind = Road.Asphalt, SceneryKind = Scenery.Totems, ParticleKind = Particles.None, Seed = 2002,
			SkyTop = new Color(0.42f, 0.03f, 0.05f), SkyHorizon = new Color(1.0f, 0.17f, 0.06f), SkyBottom = new Color(0.36f, 0.05f, 0.04f),
			Stars = 0f, StarColor = Color.white,
			Sun = true, SunDir = new Vector3(0.136f, 0.225f, -0.965f), SunColor = new Color(1f, 0.62f, 0.3f), SunSize = 2.6f,
			Clouds = true, CloudColor = new Color(0.32f, 0.03f, 0.03f), CloudEdge = new Color(0.95f, 0.42f, 0.22f), SkyExposure = 1f, SkyDetail = 2,   /* [user request 2026-09-26: night drive] */
			Fog = new Color(0.86f, 0.13f, 0.07f), FogStart = 110f, FogEnd = 440f,
			Ambient = new Color(0.22f, 0.10f, 0.11f), KeyColor = new Color(1f, 0.5f, 0.36f), KeyIntensity = 1.75f, KeyEuler = new Vector3(18f, -32f, 0f),
			Lamps = true, LampEmission = Color.black, Fences = true, FenceColor = new Color(0.02f, 0.92f, 1f), FenceGlow = 2.2f,
		};

		private static readonly Profile[] Profiles =
		{
			new Profile
			{
				Name = "DAYLIGHT CITY", SkyKind = Sky.Day, RoadKind = Road.Asphalt, SceneryKind = Scenery.Trees, ParticleKind = Particles.None, Seed = 1001,
				SkyTop = new Color(0.36f, 0.62f, 0.96f), SkyHorizon = new Color(0.82f, 0.9f, 1f), SkyBottom = new Color(0.6f, 0.68f, 0.78f),
				Stars = 0f, StarColor = Color.white,
				Sun = true, SunDir = new Vector3(0.136f, 0.225f, -0.965f), SunColor = new Color(1f, 0.96f, 0.84f), SunSize = 2.4f,
				Clouds = true, CloudColor = Color.white, SkyExposure = 1f,
				Fog = new Color(0.78f, 0.85f, 0.95f), FogStart = 120f, FogEnd = 420f,
				Ambient = new Color(0.34f, 0.38f, 0.44f), KeyColor = new Color(1f, 0.95f, 0.85f), KeyIntensity = 1.6f, KeyEuler = new Vector3(48f, -30f, 0f),
				RoadTint = new Color(0.36f, 0.38f, 0.42f), RoadSmoothness = 0.35f, RoadMetallic = 0f, RoadEmission = new Color(0.18f, 0.2f, 0.23f),
				Lanes = true, LaneColor = new Color(0.92f, 0.92f, 0.9f), LaneEmission = new Color(0.5f, 0.5f, 0.48f), EdgeColor = new Color(0.95f, 0.78f, 0.2f), EdgeEmission = new Color(0.6f, 0.45f, 0.08f),
				Verge = new Color(0.22f, 0.4f, 0.16f), VergeSmoothness = 0.05f,
				Buildings = true, BuildingTint = new Color(1.28f, 1.25f, 1.2f), Fences = false, FenceColor = new Color(0.02f, 0.92f, 1f), FenceGlow = 6.2f,
				Lamps = true, LampEmission = Color.black,
			},
			new Profile
			{
				Name = "NEON NIGHT", SkyKind = Sky.Night, RoadKind = Road.Asphalt, SceneryKind = Scenery.Totems, ParticleKind = Particles.None, Seed = 2002,
				SkyTop = new Color(0.02f, 0.01f, 0.06f), SkyHorizon = new Color(0.16f, 0.03f, 0.22f), SkyBottom = new Color(0.08f, 0.02f, 0.12f),
				Stars = 0.9f, StarColor = new Color(0.85f, 0.9f, 1f), SkyExposure = 1f,   /* [user request 2026-09-26: night drive] */ // original: Stars = 0.6f
				SkyDetail = 2, Galaxy = 0.55f, GalaxyColor = new Color(0.55f, 0.3f, 0.9f), Wisps = 0.42f, WispColor = new Color(0.3f, 0.09f, 0.42f), HorizonGlow = new Color(0.28f, 0.05f, 0.34f),
				Moon = true, NightHazards = true,
				Fog = new Color(0.08f, 0.02f, 0.12f), FogStart = 160f, FogEnd = 520f,
				Ambient = new Color(0.12f, 0.05f, 0.065f), KeyColor = new Color(1f, 0.4f, 0.34f), KeyIntensity = 1.85f, KeyEuler = new Vector3(25f, -165f, 0f), Pulse = true,
				RoadTint = new Color(0.12f, 0.12f, 0.16f), RoadSmoothness = 0.5f, RoadMetallic = 0f, RoadEmission = new Color(0.4f, 0.48f, 0.58f),
				Lanes = true, LaneColor = new Color(0.18f, 0.9f, 1f), LaneEmission = new Color(0.08f, 0.78f, 1.15f) * 4.4f, EdgeColor = new Color(0.18f, 0.9f, 1f), EdgeEmission = new Color(0.08f, 0.78f, 1.15f) * 4.4f,
				Verge = new Color(0.035f, 0.026f, 0.026f), VergeSmoothness = 0f,
				Buildings = true, BuildingTint = Color.white, Fences = true, FenceColor = new Color(0.02f, 0.92f, 1f), FenceGlow = 6.2f,
				Lamps = true, LampEmission = new Color(1f, 0.015f, 0.008f) * 3.2f,
				Day = NeonDay,   /* [user request 2026-09-24: neon night cycle] */
			},
			new Profile
			{
				Name = "RAIN RUN", SkyKind = Sky.Overcast, RoadKind = Road.Wet, SceneryKind = Scenery.GuardRails, ParticleKind = Particles.Rain, Seed = 3003,
				SkyTop = new Color(0.1f, 0.11f, 0.15f), SkyHorizon = new Color(0.32f, 0.34f, 0.4f), SkyBottom = new Color(0.26f, 0.28f, 0.34f),
				Stars = 0f, StarColor = Color.white, Clouds = true, CloudColor = new Color(0.2f, 0.21f, 0.26f), SkyExposure = 1f,
				Fog = new Color(0.28f, 0.3f, 0.36f), FogStart = 60f, FogEnd = 300f,
				Ambient = new Color(0.16f, 0.18f, 0.22f), KeyColor = new Color(0.66f, 0.74f, 0.9f), KeyIntensity = 0.9f, KeyEuler = new Vector3(40f, -40f, 0f),
				RoadTint = new Color(0.08f, 0.09f, 0.12f), RoadSmoothness = 0.92f, RoadMetallic = 0.15f, RoadEmission = new Color(0.12f, 0.14f, 0.17f),
				Lanes = true, LaneColor = new Color(0.7f, 0.72f, 0.76f), LaneEmission = new Color(0.34f, 0.36f, 0.4f), EdgeColor = new Color(0.7f, 0.72f, 0.76f), EdgeEmission = new Color(0.34f, 0.36f, 0.4f),
				Verge = new Color(0.1f, 0.11f, 0.13f), VergeSmoothness = 0.6f,
				Buildings = true, BuildingTint = new Color(0.62f, 0.68f, 0.8f), Fences = false, FenceColor = new Color(0.02f, 0.92f, 1f), FenceGlow = 6.2f,
				Lamps = true, LampEmission = new Color(0.6f, 0.8f, 1f) * 2.4f,
				RainAudio = true, RainVolume = 0.5f,
			},
			new Profile
			{
				Name = "STORM CHASE", SkyKind = Sky.Dust, RoadKind = Road.Sand, SceneryKind = Scenery.Desert, ParticleKind = Particles.Dust, Seed = 4004,
				SkyTop = new Color(0.55f, 0.3f, 0.12f), SkyHorizon = new Color(0.95f, 0.72f, 0.45f), SkyBottom = new Color(0.78f, 0.62f, 0.42f),
				Stars = 0f, StarColor = Color.white,
				Sun = true, SunDir = new Vector3(-0.17f, 0.29f, -0.94f), SunColor = new Color(1f, 0.82f, 0.55f), SunSize = 3f,
				Clouds = true, CloudColor = new Color(0.3f, 0.17f, 0.1f), SkyExposure = 1f,
				Fog = new Color(0.78f, 0.62f, 0.42f), FogStart = 80f, FogEnd = 380f,
				Ambient = new Color(0.36f, 0.28f, 0.2f), KeyColor = new Color(1f, 0.8f, 0.55f), KeyIntensity = 1.3f, KeyEuler = new Vector3(35f, -20f, 0f), Lightning = true,
				RoadTint = new Color(0.78f, 0.7f, 0.58f), RoadSmoothness = 0.25f, RoadMetallic = 0f, RoadEmission = new Color(0.2f, 0.16f, 0.1f),
				Lanes = true, LaneColor = new Color(0.75f, 0.62f, 0.3f), LaneEmission = new Color(0.22f, 0.18f, 0.08f), EdgeColor = new Color(0.75f, 0.62f, 0.3f), EdgeEmission = new Color(0.22f, 0.18f, 0.08f),
				Verge = new Color(0.6f, 0.48f, 0.32f), VergeSmoothness = 0.05f,
				Buildings = false, BuildingTint = Color.white, Fences = false, FenceColor = new Color(0.02f, 0.92f, 1f), FenceGlow = 6.2f,
				Lamps = false, LampEmission = Color.black,
				RainAudio = true, RainVolume = 0.42f,
			},
			new Profile
			{
				Name = "FALLEN SKY", SkyKind = Sky.Aurora, RoadKind = Road.Snow, SceneryKind = Scenery.Pines, ParticleKind = Particles.Snow, Seed = 5005,
				SkyTop = new Color(0.02f, 0.04f, 0.1f), SkyHorizon = new Color(0.1f, 0.18f, 0.3f), SkyBottom = new Color(0.55f, 0.62f, 0.75f),
				Stars = 0.9f, StarColor = new Color(0.9f, 0.95f, 1f), Aurora = true, SkyExposure = 1f, SkyDrift = 0.35f,
				Fog = new Color(0.55f, 0.62f, 0.75f), FogStart = 120f, FogEnd = 460f,
				Ambient = new Color(0.18f, 0.22f, 0.3f), KeyColor = new Color(0.7f, 0.8f, 1f), KeyIntensity = 1f, KeyEuler = new Vector3(52f, -140f, 0f),
				RoadTint = new Color(0.9f, 0.93f, 1f), RoadSmoothness = 0.55f, RoadMetallic = 0f, RoadEmission = new Color(0.15f, 0.17f, 0.22f),
				Lanes = false, LaneColor = new Color(0.7f, 0.72f, 0.76f), LaneEmission = Color.black, EdgeColor = new Color(0.7f, 0.72f, 0.76f), EdgeEmission = Color.black,
				Verge = new Color(0.86f, 0.9f, 0.98f), VergeSmoothness = 0.3f,
				Buildings = true, BuildingTint = new Color(0.7f, 0.8f, 1f), Fences = false, FenceColor = new Color(0.02f, 0.92f, 1f), FenceGlow = 6.2f,
				Lamps = true, LampEmission = new Color(1f, 0.7f, 0.4f) * 2.6f,
				FallingStars = true, Moon = true,
			},
			new Profile
			{
				Name = "PLANET HORIZON", SkyKind = Sky.Nebula, RoadKind = Road.Grid, SceneryKind = Scenery.Crystals, ParticleKind = Particles.Motes, Seed = 6006,
				SkyTop = new Color(0.06f, 0.02f, 0.12f), SkyHorizon = new Color(0.3f, 0.1f, 0.45f), SkyBottom = new Color(0.12f, 0.04f, 0.2f),
				Stars = 0.8f, StarColor = new Color(1f, 0.9f, 1f), SkyExposure = 1.2f, SkyDrift = 0.25f,
				Fog = new Color(0.25f, 0.08f, 0.4f), FogStart = 100f, FogEnd = 420f,
				Ambient = new Color(0.16f, 0.06f, 0.24f), KeyColor = new Color(1f, 0.4f, 0.9f), KeyIntensity = 1.4f, KeyEuler = new Vector3(30f, -150f, 0f),
				RoadTint = new Color(0.05f, 0.02f, 0.1f), RoadSmoothness = 0.85f, RoadMetallic = 0.1f, RoadEmission = new Color(0.55f, 0.2f, 1f) * 3f,
				Lanes = true, LaneColor = new Color(1f, 0.3f, 0.9f), LaneEmission = new Color(1f, 0.25f, 0.9f) * 3.5f, EdgeColor = new Color(1f, 0.3f, 0.9f), EdgeEmission = new Color(1f, 0.25f, 0.9f) * 3.5f,
				Verge = new Color(0.08f, 0.03f, 0.14f), VergeSmoothness = 0.5f,
				Buildings = false, BuildingTint = Color.white, Fences = true, FenceColor = new Color(1f, 0.2f, 0.85f), FenceGlow = 5.5f,
				Lamps = false, LampEmission = Color.black,
				Pulsar = true, Planet = true,
			},
		};

		// ---- public state ------------------------------------------------------------------
		private static ShadowRunPrototype s_game;
		private static int s_current = -1;

		public static int Current { get { return s_current; } }
		public static bool OwnsLighting { get { return s_current >= 0 && (!Profiles[s_current].Pulse || s_cycle.Blend > 0.001f || s_blackout > 0.001f); } }   /* [user request 2026-09-24: neon night cycle] */ // the day blend owns the light unless it is pure night
		public static bool OwnsSky { get { return s_current >= 0; } }
		public static bool UsesRoadReflectionProbe { get { return s_current >= 0 && (Profiles[s_current].RoadKind == Road.Wet || Profiles[s_current].RoadKind == Road.Grid); } }

		private static int Clamp(int index) { return Mathf.Clamp(index, 0, Count - 1); }
		public static string Name(int index) { return Profiles[Clamp(index)].Name; }
		public static string Slug(int index) { return InfernoLobby.MapSlugs[Clamp(index)]; }
		public static string RoadKind(int index) { return Profiles[Clamp(index)].RoadKind.ToString(); }
		public static string SkyKind(int index) { return Profiles[Clamp(index)].SkyKind.ToString(); }
		public static string SceneryKind(int index) { return Profiles[Clamp(index)].SceneryKind.ToString(); }
		public static string ParticleKind(int index) { return Profiles[Clamp(index)].ParticleKind.ToString(); }
		public static bool HasLightning(int index) { return Profiles[Clamp(index)].Lightning; }
		public static bool KeepsShippedPulse(int index) { return Profiles[Clamp(index)].Pulse; }
		public static bool HasDayNightCycle(int index) { return Profiles[Clamp(index)].Day != null; }
		public static bool HasNightHazards(int index) { return Profiles[Clamp(index)].NightHazards; }   /* [user request 2026-09-26: night drive] */   /* [user request 2026-09-24: neon night cycle] */
		public static Camera WorldCamera(ShadowRunPrototype game) { return game != null ? game.cam : null; }

		// ---- runtime caches (reset per game instance) -----------------------------------------
		private static readonly Texture2D[] s_skyTextures = new Texture2D[Count];
		private static Material s_skyMat;          // Skybox/Panoramic (Resources/Inferno/SkyPanoramic copy)
		private static Material s_nebulaMat;       // Skybox/Cubemap copy of the pulsar nebula material
		private static Material s_solidSkyMat;     // last resort: ShadowRun/SolidColorSkybox
		private static Material s_blendSkyMat;     // ShadowRun/PanoramicBlend: night + day panoramas, _Blend /* [user request 2026-09-24: neon night cycle] */
		private static Texture2D s_daySkyTexture; private static int s_daySkyIndex = -1;
		private static Material s_skyActive;
		private static float s_skyBaseExposure = 1f, s_skyRotation;
		private static Texture2D s_sandTexture, s_snowTexture, s_gridTexture, s_dotTexture, s_boltTexture;
		private static readonly Dictionary<string, Material> s_materials = new Dictionary<string, Material>();
		private static readonly Dictionary<string, Mesh> s_meshes = new Dictionary<string, Mesh>();
		private static readonly List<Transform> s_buildings = new List<Transform>();
		private static readonly List<Transform> s_fences = new List<Transform>();
		private static readonly List<Transform> s_lamps = new List<Transform>();
		private static readonly Dictionary<Material, Color> s_buildingBaseColors = new Dictionary<Material, Color>();
		private static readonly List<Renderer> s_laneRenderers = new List<Renderer>();
		private static readonly List<Renderer> s_edgeRenderers = new List<Renderer>();
		private static Renderer s_roadSurface;
		private static Material s_edgeMat;
		private static Transform s_roadRoot;
		private static bool s_cityCollected;
		private static ReflectionProbe s_roadProbe;
		private static Transform s_sceneryRoot, s_particleRoot, s_propsRoot, s_boltQuad;
		private static Material s_boltMat;
		private static AudioSource s_rainSource;
		private static float s_rainVolume;
		private static bool s_rainMuted = true, s_rainMuteKnown;
		private static System.Random s_rng = new System.Random(1);
		private static float s_lightningTimer, s_flashTime;
		private static bool s_flashing;
		private static float s_keyBaseIntensity;
		private static float s_cityZ;

		private struct Floater { public Transform T; public float BaseY, Phase, Speed, Amp, Spin; }
		private static readonly List<Floater> s_floaters = new List<Floater>();
		private struct Scroller { public ParticleSystem System; public float Factor; }
		private static readonly List<Scroller> s_scrollers = new List<Scroller>();
		private static readonly List<ParticleSystem> s_systems = new List<ParticleSystem>();

		private static readonly int RotationId = Shader.PropertyToID("_Rotation");
		private static readonly int ExposureId = Shader.PropertyToID("_Exposure");
		private static readonly int TintId = Shader.PropertyToID("_Tint");
		private static readonly int SmoothnessId = Shader.PropertyToID("_Smoothness");
		private static readonly int MetallicId = Shader.PropertyToID("_Metallic");
		private static readonly int BlendId = Shader.PropertyToID("_Blend"), DayTexId = Shader.PropertyToID("_DayTex"), MainTexId = Shader.PropertyToID("_MainTex");   /* [user request 2026-09-24: neon night cycle] */

		private const float FlashDuration = 0.1f;

		private static void Reset(ShadowRunPrototype game)
		{
			s_game = game;
			s_current = -1;
			for (int i = 0; i < s_skyTextures.Length; i++) s_skyTextures[i] = null;
			s_skyMat = null; s_nebulaMat = null; s_solidSkyMat = null; s_skyActive = null;
			s_blendSkyMat = null; s_daySkyTexture = null; s_daySkyIndex = -1; s_cycle = ResetCycle(); s_appliedBlend = -1f;   /* [user request 2026-09-24: neon night cycle] */
			s_blackout = 0f; s_appliedBlackout = -1f;   /* [user request 2026-09-26: night drive] */
			s_sandTexture = null; s_snowTexture = null; s_gridTexture = null; s_dotTexture = null; s_boltTexture = null;
			s_materials.Clear(); s_meshes.Clear(); s_textures.Clear();
			s_buildings.Clear(); s_fences.Clear(); s_lamps.Clear(); s_buildingBaseColors.Clear();
			s_laneRenderers.Clear(); s_edgeRenderers.Clear();
			s_roadSurface = null; s_edgeMat = null; s_roadRoot = null; s_cityCollected = false; s_roadProbe = null;
			s_sceneryRoot = null; s_particleRoot = null; s_propsRoot = null; s_boltQuad = null; s_boltMat = null;
			s_rainSource = null; s_rainMuted = true; s_rainMuteKnown = false;
			s_floaters.Clear(); s_scrollers.Clear(); s_systems.Clear();
			s_flashing = false; s_flashTime = 0f; s_skyRotation = 0f;
		}

		// ---- Apply ---------------------------------------------------------------------------
		// Idempotent; called at the end of Awake (after MakeCar), from InfernoLobby.ApplySelectedMap on every
		// map step / run start, and by the thumbnail capture tool.
		public static void Apply(ShadowRunPrototype game, int index)
		{
			if (game == null) return;
			index = Clamp(index);
			if (s_game != game) Reset(game);
			Profile p = Profiles[index];
			bool same = s_current == index && s_sceneryRoot != null;
			s_current = index;
			s_keyBaseIntensity = p.KeyIntensity;
			CollectCity(game);
			ApplySky(game, p, index);
			ApplyLighting(game, p);
			ApplyRoad(game, p);
			ApplyCity(game, p);
			if (!same)
			{
				BuildScenery(game, p);
				BuildParticles(game, p);
				BuildSkyProps(game, p);
			}
			ApplyExtras(game, p);
			s_cycle = ResetCycle();   /* [user request 2026-09-24: neon night cycle] */ // every run starts at night; the first dawn comes PhaseDuration later
			s_appliedBlend = -1f;
			s_blackout = 0f; s_appliedBlackout = -1f;   /* [user request 2026-09-26: night drive] */ // the power is back on whenever a map is (re)applied
			if (p.Day != null) ApplyCycle(game, p);
			s_rng = new System.Random(p.Seed ^ 0x5A17);
			s_lightningTimer = NextLightningDelay();
			s_flashTime = 0f;
			s_flashing = false;
		}

		// ---- Tick (per frame while a run is in progress; called from UpdateWorld) --------------
		public static void Tick(ShadowRunPrototype game, float dt)
		{
			if (s_current < 0 || game == null || s_game != game) return;
			Profile p = Profiles[s_current];
			Camera cam = game.cam;
			if (s_particleRoot != null && cam != null)
			{
				Vector3 c = cam.transform.position;
				s_particleRoot.position = new Vector3(c.x, 0f, c.z - 11.1f);
			}
			float scroll = game.speed;
			for (int i = 0; i < s_scrollers.Count; i++)
			{
				ParticleSystem system = s_scrollers[i].System;
				if (system == null) continue;
				ParticleSystem.VelocityOverLifetimeModule velocity = system.velocityOverLifetime;
				float vz = scroll * s_scrollers[i].Factor;
				velocity.z = new ParticleSystem.MinMaxCurve(vz, vz);   // same mode (TwoConstants) as x / y, see SetVelocity
			}
			if (p.Lightning) TickLightning(game, p, dt);
			if (p.SkyDrift != 0f && s_skyActive != null && s_skyActive.HasProperty(RotationId))
			{
				s_skyRotation = Mathf.Repeat(s_skyRotation + p.SkyDrift * dt, 360f);
				s_skyActive.SetFloat(RotationId, s_skyRotation);
			}
			float time = Time.time;
			for (int i = 0; i < s_floaters.Count; i++)
			{
				Floater f = s_floaters[i];
				if (f.T == null) continue;
				Vector3 local = f.T.localPosition;
				local.y = f.BaseY + Mathf.Sin(time * f.Speed + f.Phase) * f.Amp;
				f.T.localPosition = local;
				f.T.localRotation = Quaternion.Euler(0f, time * f.Spin + f.Phase * 57.3f, 0f);
			}
			if (p.Pulsar && game.pulsarNightRoot != null && game.pulsarNightRoot.gameObject.activeSelf)
			{
				game.pulsarNightVisibility = 1f;
				game.AnimatePulsarNightSystem(1f);
			}
			if (p.Day != null || s_blackout > 0f)   /* [user request 2026-09-24: neon night cycle] */
			{
				if (p.Day != null && game.running && !game.paused) Advance(ref s_cycle, dt);
				ApplyCycle(game, p);
			}
		}

		// ---- day / night cycle (Neon Night) ----------------------------------------------------------
		/* [user request 2026-09-24: neon night cycle] */
		// The shipped build changed its weather during a run through the Dynamic Weather System: a new preset
		// every WeatherPhaseDuration (35 s), faded over WeatherTransitionDuration (4.25 s; 4 s from night to
		// day). That package is a stub in the remake, so Neon Night never left the night. The map now carries a
		// Day profile (NeonDay) and this state machine fades between the two while a run is in progress: the sky
		// (ShadowRun/PanoramicBlend mixes the two panoramas), fog, camera background, key light (colour,
		// intensity, direction), ambient, street lamps and the fence glow. At pure night nothing is touched and
		// the shipped red pulse (UpdateSceneLighting) runs as before; while the blend is up, OwnsLighting is true
		// and the pulse's values (ShippedLightPulse) are mixed with the day's here instead. Pure arithmetic apart
		// from ApplyCycle, so the EditMode tests cover the timing.
		public struct Cycle { public float Timer; public float Blend; public float From, To; public float Duration, Elapsed; public bool Transitioning; }
		public const float PhaseDuration = 35f;       // = ShadowRunPrototype.WeatherPhaseDuration
		public const float ToNightDuration = 4.25f;   // = WeatherTransitionDuration
		public const float ToDayDuration = 4f;        // = NightToDayTransitionDuration
		private static Cycle s_cycle = ResetCycle();
		private static float s_appliedBlend = -1f;
		/* [user request 2026-09-26: night drive] */ // a blackout (InfernoNightHazards) darkens the city on top of the day / night blend: lamps, fences, lane
		// lines and building windows go out, the fog closes in, ambient and key light drop. 0 = power on, 1 = fully dark.
		private static float s_blackout, s_appliedBlackout = -1f;
		public static float Blackout { get { return s_blackout; } }

		public static void SetBlackout(ShadowRunPrototype game, float darkness)
		{
			darkness = Mathf.Clamp01(darkness);
			if (Mathf.Abs(darkness - s_blackout) < 0.0005f) return;
			s_blackout = darkness;
			if (s_current >= 0 && s_game == game && game != null) ApplyCycle(game, Profiles[s_current]);
		}

		public static float DayBlend { get { return s_cycle.Blend; } }

		public static Cycle ResetCycle() { return new Cycle { Timer = PhaseDuration, Blend = 0f, From = 0f, To = 0f }; }

		public static void Advance(ref Cycle c, float dt)
		{
			if (dt <= 0f) return;
			if (c.Transitioning)
			{
				c.Elapsed += dt;
				float t = c.Duration > 0f ? Mathf.Clamp01(c.Elapsed / c.Duration) : 1f;
				c.Blend = Mathf.Lerp(c.From, c.To, t * t * (3f - 2f * t));
				if (t >= 1f) { c.Transitioning = false; c.Blend = c.To; c.Timer = PhaseDuration; }
				return;
			}
			c.Timer -= dt;
			if (c.Timer > 0f) return;
			c.From = c.Blend;
			c.To = c.Blend < 0.5f ? 1f : 0f;
			c.Duration = c.To > 0.5f ? ToDayDuration : ToNightDuration;
			c.Elapsed = 0f;
			c.Transitioning = true;
		}

		// Editor / test hook: skips ahead in the cycle (ShadowRunPrototype.DebugAdvanceNeonCycle forwards here).
		public static void DebugAdvanceCycle(float seconds) { Advance(ref s_cycle, seconds); }

		// The environment for the current day blend b and blackout k (both usually 0): sky blend, fog, camera
		// background, key light, ambient, street lamps, fences, lane lines and building windows. Lighting follows
		// the frame (the pulse moves) whenever b or k is up; the material writes only happen when b or k changed.
		private static void ApplyCycle(ShadowRunPrototype game, Profile p)
		{
			if (game == null) return;
			Profile d = p.Day ?? p;
			float b = p.Day != null ? s_cycle.Blend : 0f;
			float k = s_blackout;   /* [user request 2026-09-26: night drive] */
			bool night = b <= 0.001f;
			bool dark = k > 0.001f;
			bool changed = Mathf.Abs(b - s_appliedBlend) > 0.0005f || Mathf.Abs(k - s_appliedBlackout) > 0.0005f;
			if (night && !dark && s_appliedBlend >= 0f && s_appliedBlend <= 0.001f && s_appliedBlackout >= 0f && s_appliedBlackout <= 0.001f) return;   // settled at night with the power on: the profile values stand, the shipped pulse runs
			s_appliedBlend = b; s_appliedBlackout = k;
			if (s_skyActive != null && s_skyActive.HasProperty(BlendId)) s_skyActive.SetFloat(BlendId, b);
			if (s_skyActive != null && s_skyActive.HasProperty(ExposureId)) s_skyActive.SetFloat(ExposureId, p.SkyExposure * (1f - 0.35f * k));   // the sky glow dims a little with the city lights out
			Color fog = Color.Lerp(p.Fog, d.Fog, b);
			RenderSettings.fogColor = Color.Lerp(fog, fog * 0.35f, k);
			RenderSettings.fogStartDistance = Mathf.Lerp(Mathf.Lerp(p.FogStart, d.FogStart, b), 40f, k);
			RenderSettings.fogEndDistance = Mathf.Lerp(Mathf.Lerp(Mathf.Max(p.FogStart + 1f, p.FogEnd), Mathf.Max(d.FogStart + 1f, d.FogEnd), b), 190f, k);
			if (game.cam != null) game.cam.backgroundColor = Color.Lerp(Color.Lerp(p.SkyHorizon, d.SkyHorizon, b), Color.black, k);
			if (!night || dark)
			{
				Color ambient, key; float intensity;
				if (p.Pulse) game.ShippedLightPulse(out ambient, out key, out intensity);
				else { ambient = p.Ambient; key = p.KeyColor; intensity = p.KeyIntensity; }
				ambient = Color.Lerp(ambient, d.Ambient, b);
				key = Color.Lerp(key, d.KeyColor, b);
				intensity = Mathf.Lerp(intensity, d.KeyIntensity, b);
				RenderSettings.ambientLight = Color.Lerp(ambient, ambient * 0.4f, k);
				if (game.sceneKeyLight != null)
				{
					game.sceneKeyLight.color = key;
					game.sceneKeyLight.intensity = intensity * (1f - 0.55f * k);
				}
			}
			else
			{
				RenderSettings.ambientLight = p.Ambient;   // the pulse takes over from the next UpdateSceneLighting on
				if (game.sceneKeyLight != null) { game.sceneKeyLight.color = p.KeyColor; game.sceneKeyLight.intensity = p.KeyIntensity; }
			}
			if (game.sceneKeyLight != null) game.sceneKeyLight.transform.rotation = Quaternion.Slerp(Quaternion.Euler(p.KeyEuler), Quaternion.Euler(d.KeyEuler), b);
			if (!changed) return;
			// street lamps go out and the neon fences dim as the day comes up - or the power fails
			float power = 1f - k;
			Material lamp = game.streetLampRedEmissiveMat;
			if (lamp != null)
			{
				Color e = Color.Lerp(p.LampEmission, d.LampEmission, b) * power;
				if (lamp.HasProperty("_EmissionColor")) lamp.SetColor("_EmissionColor", e);
				if (lamp.HasProperty("_EmissiveColor")) lamp.SetColor("_EmissiveColor", e);
				if (lamp.HasProperty("_EmissiveFactor")) lamp.SetColor("_EmissiveFactor", e);
			}
			SetLit(game, game.neonFenceMat, Color.Lerp(p.FenceColor, d.FenceColor, b) * Mathf.Lerp(1f, 0.25f, k), Color.Lerp(p.FenceColor * p.FenceGlow, d.FenceColor * d.FenceGlow, b) * power);
			SetLit(game, game.lineMat, p.LaneColor * Mathf.Lerp(1f, 0.45f, k), p.LaneEmission * power);
			SetLit(game, s_edgeMat, p.EdgeColor * Mathf.Lerp(1f, 0.45f, k), p.EdgeEmission * power);
			// the buildings' lit windows go dark with the grid
			foreach (KeyValuePair<Material, Color> pair in s_buildingBaseColors)
			{
				Material mat = pair.Key;
				if (mat == null) continue;
				Color c = pair.Value;
				Color tinted = new Color(c.r * p.BuildingTint.r, c.g * p.BuildingTint.g, c.b * p.BuildingTint.b, c.a) * Mathf.Lerp(1f, 0.3f, k);
				tinted.a = c.a;
				if (mat.HasProperty(BaseColorProperty)) mat.SetColor(BaseColorProperty, tinted);
				if (mat.HasProperty(ColorProperty)) mat.SetColor(ColorProperty, tinted);
			}
		}

		// Pause / resume the map particles with the run (PauseRun / ResumeRun / StartRun).
		public static void Freeze(ShadowRunPrototype game, bool frozen)
		{
			if (s_game != game) return;
			for (int i = 0; i < s_systems.Count; i++)
			{
				ParticleSystem system = s_systems[i];
				if (system == null) continue;
				if (frozen) system.Pause(true);
				else if (system.isPaused) system.Play(true);
			}
		}

		// ---- sky -----------------------------------------------------------------------------
		private static void ApplySky(ShadowRunPrototype game, Profile p, int index)
		{
			Material sky = null;
			if (p.Day != null) sky = DayNightMaterial(p, index);   /* [user request 2026-09-24: neon night cycle] */ // the two-panorama blend; null = shader missing, plain night panorama below
			if (sky == null && p.SkyKind == Sky.Nebula) sky = NebulaMaterial(p);
			if (sky == null)
			{
				sky = PanoramicMaterial();
				if (sky != null)
				{
					Texture2D texture = s_skyTextures[index];
					if (texture == null)
					{
						texture = GenerateSky(p);
						s_skyTextures[index] = texture;
					}
					sky.mainTexture = texture;
					if (sky.HasProperty(TintId)) sky.SetColor(TintId, new Color(0.5f, 0.5f, 0.5f, 0.5f));
					if (sky.HasProperty(ExposureId)) sky.SetFloat(ExposureId, p.SkyExposure);
					if (sky.HasProperty(RotationId)) sky.SetFloat(RotationId, 0f);
				}
			}
			if (sky == null) sky = SolidSky(p.SkyHorizon);
			s_skyActive = sky;
			s_skyBaseExposure = p.SkyExposure;
			s_skyRotation = 0f;
			if (sky != null && RenderSettings.skybox != sky) RenderSettings.skybox = sky;
			if (game.cam != null)
			{
				game.cam.clearFlags = sky != null ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
				game.cam.backgroundColor = p.SkyHorizon;
			}
			RenderSettings.fog = true;
			RenderSettings.fogMode = FogMode.Linear;
			RenderSettings.fogColor = p.Fog;
			RenderSettings.fogStartDistance = p.FogStart;
			RenderSettings.fogEndDistance = Mathf.Max(p.FogStart + 1f, p.FogEnd);
			DynamicGI.UpdateEnvironment();
		}

		private static Material PanoramicMaterial()
		{
			if (s_skyMat != null) return s_skyMat;
			Material source = Resources.Load<Material>("Inferno/SkyPanoramic");
			Shader shader = source != null ? source.shader : Shader.Find("Skybox/Panoramic");
			if (shader == null) return null;
			s_skyMat = source != null ? new Material(source) : new Material(shader);
			s_skyMat.name = "Runtime Map Panoramic Skybox";
			return s_skyMat;
		}

		private static Material NebulaMaterial(Profile p)
		{
			if (s_nebulaMat == null)
			{
				Material source = Resources.Load<Material>("Pulsar Neutron Star/Materials/NEbula_SkyBox");
				if (source == null || source.shader == null) return null;
				s_nebulaMat = new Material(source);
				s_nebulaMat.name = "Runtime Map Nebula Skybox";
			}
			if (s_nebulaMat.HasProperty(TintId)) s_nebulaMat.SetColor(TintId, new Color(0.5f, 0.5f, 0.5f, 0.5f));
			if (s_nebulaMat.HasProperty(ExposureId)) s_nebulaMat.SetFloat(ExposureId, p.SkyExposure);
			if (s_nebulaMat.HasProperty(RotationId)) s_nebulaMat.SetFloat(RotationId, 0f);
			return s_nebulaMat;
		}

		/* [user request 2026-09-24: neon night cycle] */
		// The two-panorama skybox of a map with a Day profile: _MainTex = the map's night panorama, _DayTex = the
		// Day profile's, _Blend = Cycle.Blend (ApplyCycle drives it). Same tint / exposure maths as Skybox/Panoramic,
		// so blend 0 is the night sky exactly as before. Ships through Resources/Inferno/SkyBlend.mat.
		private static Material DayNightMaterial(Profile p, int index)
		{
			if (s_blendSkyMat == null)
			{
				Material source = Resources.Load<Material>("Inferno/SkyBlend");
				Shader shader = source != null ? source.shader : Shader.Find("ShadowRun/PanoramicBlend");
				if (shader == null) return null;
				s_blendSkyMat = source != null ? new Material(source) : new Material(shader);
				s_blendSkyMat.name = "Runtime Map Day-Night Skybox";
			}
			Texture2D night = s_skyTextures[index];
			if (night == null) { night = GenerateSky(p); s_skyTextures[index] = night; }
			if (s_daySkyTexture == null || s_daySkyIndex != index) { s_daySkyTexture = GenerateSky(p.Day); s_daySkyIndex = index; }
			s_blendSkyMat.SetTexture(MainTexId, night);
			s_blendSkyMat.SetTexture(DayTexId, s_daySkyTexture);
			if (s_blendSkyMat.HasProperty(TintId)) s_blendSkyMat.SetColor(TintId, new Color(0.5f, 0.5f, 0.5f, 0.5f));
			if (s_blendSkyMat.HasProperty(ExposureId)) s_blendSkyMat.SetFloat(ExposureId, p.SkyExposure);
			if (s_blendSkyMat.HasProperty(RotationId)) s_blendSkyMat.SetFloat(RotationId, 0f);
			s_blendSkyMat.SetFloat(BlendId, s_cycle.Blend);
			return s_blendSkyMat;
		}

		private static Material SolidSky(Color color)
		{
			if (s_solidSkyMat == null)
			{
				Shader shader = Shader.Find("ShadowRun/SolidColorSkybox");
				if (shader == null) return null;
				s_solidSkyMat = new Material(shader);
				s_solidSkyMat.name = "Runtime Map Solid Skybox";
			}
			if (s_solidSkyMat.HasProperty("_Color")) s_solidSkyMat.SetColor("_Color", color);
			return s_solidSkyMat;
		}

		// A wrapped (in u) value-noise lattice for clouds / aurora / nebula haze.
		private struct Lattice
		{
			private readonly float[] values;
			private readonly int w, h;

			public Lattice(System.Random rng, int width, int height)
			{
				w = width; h = height;
				values = new float[w * h];
				for (int i = 0; i < values.Length; i++) values[i] = (float)rng.NextDouble();
			}

			public float Sample(float u, float v)
			{
				float fx = u * w, fy = Mathf.Clamp01(v) * (h - 1);
				int x0 = Mathf.FloorToInt(fx), y0 = Mathf.FloorToInt(fy);
				float tx = fx - x0, ty = fy - y0;
				tx = tx * tx * (3f - 2f * tx);
				ty = ty * ty * (3f - 2f * ty);
				int xa = ((x0 % w) + w) % w, xb = (xa + 1) % w;
				int ya = Mathf.Clamp(y0, 0, h - 1), yb = Mathf.Clamp(y0 + 1, 0, h - 1);
				float top = Mathf.Lerp(values[xa + ya * w], values[xb + ya * w], tx);
				float bottom = Mathf.Lerp(values[xa + yb * w], values[xb + yb * w], tx);
				return Mathf.Lerp(top, bottom, ty);
			}
		}

		private static float Smooth(float edge0, float edge1, float x)
		{
			float t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
			return t * t * (3f - 2f * t);
		}

		private const int SkyWidth = 1024, SkyHeight = 512;

		// 1024x512 latitude-longitude panorama for Skybox/Panoramic. Row v = 0.5 is the horizon; the camera
		// sees roughly the band 0..24 degrees above it, so the gradient does most of its work there.
		private static Texture2D GenerateSky(Profile p)
		{
			System.Diagnostics.Stopwatch clock = System.Diagnostics.Stopwatch.StartNew();
			int detail = Mathf.Clamp(p.SkyDetail, 1, 4);   /* [user request 2026-09-26: night drive] */ // Neon Night renders at 2048x1024 (original: 1024x512 for every map)
			int w = SkyWidth * detail, h = SkyHeight * detail;
			Color[] pixels = new Color[w * h];
			float[] rowSin = new float[h], rowCos = new float[h], rowElev = new float[h];
			for (int y = 0; y < h; y++)
			{
				float v = (y + 0.5f) / h;
				float latitude = (1f - v) * Mathf.PI;
				rowSin[y] = Mathf.Sin(latitude);
				rowCos[y] = Mathf.Cos(latitude);
				rowElev[y] = (v - 0.5f) * 180f;
			}
			float[] colCos = new float[w], colSin = new float[w];
			for (int x = 0; x < w; x++)
			{
				float longitude = (0.5f - (x + 0.5f) / w) * Mathf.PI * 2f;
				colCos[x] = Mathf.Cos(longitude);
				colSin[x] = Mathf.Sin(longitude);
			}
			System.Random rng = new System.Random(p.Seed * 31 + 7);
			Lattice n1 = new Lattice(rng, 16, 8), n2 = new Lattice(rng, 32, 16), n3 = new Lattice(rng, 64, 32);
			Vector3 sun = p.Sun ? p.SunDir.normalized : Vector3.up;
			bool nebula = p.SkyKind == Sky.Nebula;
			for (int y = 0; y < h; y++)
			{
				float e = rowElev[y];
				float v = (y + 0.5f) / h;
				float dirY = rowCos[y];
				float s = rowSin[y];
				Color baseColor;
				if (e >= 0f) baseColor = Color.Lerp(p.SkyHorizon, p.SkyTop, 1f - Mathf.Exp(-e / 14f));
				else baseColor = Color.Lerp(p.SkyHorizon, p.SkyBottom, 1f - Mathf.Exp(e / 8f));
				for (int x = 0; x < w; x++)
				{
					float u = (x + 0.5f) / w;
					Color col = baseColor;
					float dirX = s * colCos[x], dirZ = s * colSin[x];
					/* [user request 2026-09-26: night drive] */ // the noise is shared by the clouds, the night wisps and the galactic haze; nothing is sampled well below the horizon
					float n = 0f;
					if ((p.Clouds || nebula || p.Wisps > 0f || p.Galaxy > 0f) && e > -3f)
						n = 0.55f * n1.Sample(u, v) + 0.3f * n2.Sample(u, v) + 0.15f * n3.Sample(u, v);
					if (p.Clouds || nebula)
					{
						if (p.SkyKind == Sky.Day)
						{
							float band = Smooth(1.5f, 7f, e) * (1f - Smooth(30f, 62f, e));
							float cloud = Smooth(0.5f, 0.72f, n) * band;
							Color cloudColor = Color.Lerp(p.CloudEdge, p.CloudColor, Smooth(0.5f, 0.9f, n));   /* [user request 2026-09-24: neon night cycle] */ // original: a fixed (0.74, 0.78, 0.86) rim
							col = Color.Lerp(col, cloudColor, cloud * 0.92f);
						}
						else if (p.SkyKind == Sky.Dust)
						{
							float band = Smooth(1f, 9f, e) * (1f - Smooth(26f, 50f, e));
							float cloud = Smooth(0.4f, 0.68f, n);
							col = Color.Lerp(col, p.CloudColor, cloud * band * 0.88f);
							float rim = cloud * (1f - cloud) * 4f * band;
							col += new Color(0.95f, 0.55f, 0.25f) * (rim * 0.35f);
							float haze = Mathf.Exp(-Mathf.Abs(e) / 5f) * 0.35f;
							col = Color.Lerp(col, new Color(0.9f, 0.7f, 0.5f), haze);
						}
						else if (p.SkyKind == Sky.Overcast)
						{
							float band = e >= 0f ? 1f : 0.3f;
							col *= 1f + (n - 0.5f) * 0.42f * band;
							float dark = Smooth(0.58f, 0.8f, n) * Smooth(0f, 8f, e);
							col = Color.Lerp(col, p.CloudColor, dark * 0.6f);
						}
						else if (nebula)
						{
							float band = Smooth(-4f, 10f, e);
							float cloud = Smooth(0.42f, 0.78f, n);
							col += new Color(0.35f, 0.1f, 0.5f) * (cloud * band * 0.7f);
							col += new Color(0.1f, 0.3f, 0.55f) * (Smooth(0.6f, 0.85f, n3.Sample(u * 2f, v)) * band * 0.35f);
						}
					}
					if (p.SkyKind == Sky.Night && e >= 0f)
					{
						/* [user request 2026-09-26: night drive] */ // the city's glow: the shipped violet haze plus a skyline-modulated band right above the horizon
						// original: col += new Color(0.28f, 0.05f, 0.34f) * Mathf.Exp(-e / 7f);
						col += p.HorizonGlow * Mathf.Exp(-e / 7f);
						col += p.HorizonGlow * (0.55f * Mathf.Exp(-e / 2.6f) * (0.55f + 0.45f * n3.Sample(u * 3f, 0.35f)));
						if (p.Galaxy > 0f)
						{
							// a faint galactic band: haze here, its dense small stars in SplatStars
							float centre = 26f + 16f * Mathf.Sin(u * Mathf.PI * 2f + 0.8f);
							float width = 9f + 3.5f * Mathf.Sin(u * Mathf.PI * 4f + 2.1f);
							float d = (e - centre) / width;
							float band = Mathf.Exp(-d * d);
							col += p.GalaxyColor * (band * (0.06f + 0.16f * n2.Sample(u * 2f, v)) * p.Galaxy);
						}
						if (p.Wisps > 0f)
						{
							// thin high clouds, lit from below by the city: dark violet bodies with a warmer underside
							float band = Smooth(1.5f, 6f, e) * (1f - Smooth(20f, 38f, e));
							float wisp = Smooth(0.54f, 0.72f, n) * band;
							Color lit = Color.Lerp(p.WispColor, p.HorizonGlow * 1.6f, Smooth(8f, 2f, e) * 0.6f);
							col = Color.Lerp(col, lit, wisp * p.Wisps);
						}
					}
					if (p.Aurora && e > 0f)
					{
						float center = 18f + 6f * Mathf.Sin(u * Mathf.PI * 4f + 1.3f) + 3f * Mathf.Sin(u * Mathf.PI * 10f + 0.4f);
						float width = 7f + 3f * Mathf.Sin(u * Mathf.PI * 6f);
						float d = (e - center) / width;
						float band = Mathf.Exp(-d * d);
						float streak = 0.55f + 0.45f * (0.5f + 0.5f * Mathf.Sin(u * Mathf.PI * 180f + 2f * Mathf.Sin(u * Mathf.PI * 22f)));
						Color auroraColor = Color.Lerp(new Color(0.15f, 0.95f, 0.55f), new Color(0.55f, 0.25f, 0.95f), Smooth(-1f, 1f, d));
						col += auroraColor * (band * streak * 0.85f);
					}
					if (p.Sun)
					{
						float cosA = dirX * sun.x + dirY * sun.y + dirZ * sun.z;
						float angle = Mathf.Acos(Mathf.Clamp(cosA, -1f, 1f)) * Mathf.Rad2Deg;
						float disc = Smooth(p.SunSize, p.SunSize * 0.8f, angle);
						float glow = Mathf.Exp(-angle / (p.SunSize * 3f)) * 0.9f + Mathf.Exp(-angle / (p.SunSize * 14f)) * 0.35f;
						col += p.SunColor * (disc * 1.2f + glow);
					}
					/* [user request 2026-09-26: night drive] */ // +-0.5/255 ordered-hash dither before the 8-bit store: the dark night gradient no longer bands
					uint hash = (uint)(x * 374761393) + (uint)(y * 668265263);
					hash = (hash ^ (hash >> 13)) * 1274126177u;
					float dither = (((hash >> 8) & 1023u) / 1023f - 0.5f) / 255f;
					col.r = Mathf.Clamp01(col.r + dither); col.g = Mathf.Clamp01(col.g + dither); col.b = Mathf.Clamp01(col.b + dither); col.a = 1f;
					pixels[x + y * w] = col;
				}
			}
			if (p.Stars > 0f) SplatStars(pixels, w, h, p, rng);
			Texture2D texture = new Texture2D(w, h, TextureFormat.RGBA32, true, false);
			texture.name = "Runtime Map Sky " + p.Name;
			texture.wrapModeU = TextureWrapMode.Repeat;
			texture.wrapModeV = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Trilinear;
			texture.anisoLevel = 4 * detail;   /* [user request 2026-09-26: night drive] */ // original: 4
			texture.SetPixels(pixels);
			texture.Apply(true, true);
			Debug.Log("[InfernoMaps] sky '" + p.Name + "' " + w + "x" + h + " generated in " + clock.ElapsedMilliseconds + " ms");
			return texture;
		}

		/* [user request 2026-09-26: night drive] */
		// Stars for the panorama. Density scales with the panorama area (SkyDetail squared) while the discs stay a
		// pixel or two wide, so a detail-2 sky has four times the stars at half the angular size: sharp points
		// instead of blobs. Brightness follows a power curve (many faint, few bright), 5 % are big soft stars, the
		// brightest 1.5 % get a four-point glint, and a Galaxy profile adds a dense field of small faint stars
		// along the same band GenerateSky hazes. original: 1400 x Stars stars, two sizes (sigma 0.6 / 1.1).
		private static void SplatStars(Color[] pixels, int w, int h, Profile p, System.Random rng)
		{
			int detail = Mathf.Max(1, w / SkyWidth);
			int count = Mathf.RoundToInt(1400f * p.Stars * detail * detail * 0.85f);
			for (int i = 0; i < count; i++)
			{
				float u = (float)rng.NextDouble();
				float v = 0.505f + (float)rng.NextDouble() * 0.49f;
				float brightness = 0.22f + 0.78f * Mathf.Pow((float)rng.NextDouble(), 2.2f);
				double kind = rng.NextDouble();
				bool big = kind < 0.05, glint = kind > 0.985;
				float sigma = big ? 0.9f + 0.3f * detail : 0.42f + 0.22f * (float)rng.NextDouble();
				int radius = big ? 2 + detail : 1;
				Color starColor = p.StarColor * brightness * (big ? 1.15f : 1f);
				float tintShift = (float)rng.NextDouble();
				if (tintShift < 0.2f) starColor *= new Color(1f, 0.9f, 0.76f);
				else if (tintShift > 0.8f) starColor *= new Color(0.78f, 0.88f, 1f);
				int cx = (int)(u * w), cy = (int)(v * h);
				Splat(pixels, w, h, cx, cy, radius, sigma, starColor);
				if (big) Splat(pixels, w, h, cx, cy, 3 * detail, 2.4f * detail, starColor * 0.12f);   // soft halo
				if (glint)
				{
					for (int k = 1; k <= 3 * detail; k++)
					{
						float fade = 0.35f * Mathf.Exp(-k / (1.4f * detail));
						Add(pixels, w, h, cx + k, cy, starColor * fade); Add(pixels, w, h, cx - k, cy, starColor * fade);
						Add(pixels, w, h, cx, cy + k, starColor * fade); Add(pixels, w, h, cx, cy - k, starColor * fade);
					}
				}
			}
			if (p.Galaxy > 0f)
			{
				int dense = Mathf.RoundToInt(2600f * p.Galaxy * detail * detail);
				Color dust = Color.Lerp(p.StarColor, p.GalaxyColor, 0.5f);
				for (int i = 0; i < dense; i++)
				{
					float u = (float)rng.NextDouble();
					float centre = 26f + 16f * Mathf.Sin(u * Mathf.PI * 2f + 0.8f);
					float width = 9f + 3.5f * Mathf.Sin(u * Mathf.PI * 4f + 2.1f);
					// gaussian-ish scatter around the band's centre line
					float g = ((float)rng.NextDouble() + (float)rng.NextDouble() + (float)rng.NextDouble() - 1.5f) * width * 0.9f;
					float e = centre + g;
					if (e < 0.5f || e > 89f) continue;
					float v = 0.5f + e / 180f;
					float brightness = 0.1f + 0.35f * Mathf.Pow((float)rng.NextDouble(), 1.8f);
					Splat(pixels, w, h, (int)(u * w), (int)(v * h), 1, 0.38f, dust * brightness);
				}
			}
		}

		private static void Add(Color[] pixels, int w, int h, int x, int y, Color c)
		{
			if (y < 0 || y >= h) return;
			x = ((x % w) + w) % w;
			int index = x + y * w;
			Color o = pixels[index] + c;
			o.r = Mathf.Clamp01(o.r); o.g = Mathf.Clamp01(o.g); o.b = Mathf.Clamp01(o.b); o.a = 1f;
			pixels[index] = o;
		}

		private static void Splat(Color[] pixels, int w, int h, int cx, int cy, int radius, float sigma, Color c)
		{
			float k = -1f / (2f * sigma * sigma);
			for (int dy = -radius; dy <= radius; dy++)
			{
				int y = cy + dy;
				if (y < 0 || y >= h) continue;
				for (int dx = -radius; dx <= radius; dx++)
					Add(pixels, w, h, cx + dx, y, c * Mathf.Exp((dx * dx + dy * dy) * k));
			}
		}

		// ---- lighting ------------------------------------------------------------------------
		private static void ApplyLighting(ShadowRunPrototype game, Profile p)
		{
			RenderSettings.ambientMode = AmbientMode.Flat;
			RenderSettings.ambientLight = p.Ambient;
			if (game.sceneKeyLight != null)
			{
				game.sceneKeyLight.color = p.KeyColor;
				game.sceneKeyLight.intensity = p.KeyIntensity;
				game.sceneKeyLight.transform.rotation = Quaternion.Euler(p.KeyEuler);
			}
		}

		// ---- road, lines, verge ----------------------------------------------------------------
		private static void ApplyRoad(ShadowRunPrototype game, Profile p)
		{
			Material m = game.roadMat;
			if (m == null) return;
			Texture2D baseMap = null, bump = null, occlusion = null, emissionMap = null;
			Vector2 tiling = InfernoLanes.TextureTiling; /* [user request 2026-09-22: five lanes] */ // original: RoadTextureTiling (1.35 x 170; x widened to 2.25 so the surface keeps its metres per tile)
			float bumpScale = 1.08f;
			switch (p.RoadKind)
			{
				case Road.Asphalt:
					baseMap = game.roadTexture; bump = game.roadNormalTexture; occlusion = game.roadAoTexture; emissionMap = baseMap;
					break;
				case Road.Wet:
					baseMap = game.roadTexture; bump = game.roadNormalTexture; occlusion = game.roadAoTexture; emissionMap = baseMap; bumpScale = 0.45f;
					break;
				case Road.Sand:
					baseMap = SandTexture(); bump = game.roadNormalTexture; emissionMap = baseMap; bumpScale = 0.7f;
					break;
				case Road.Snow:
					baseMap = SnowTexture(); emissionMap = baseMap; tiling = new Vector2(1f, RoadTextureTiling.y);   // x 1: the tyre tracks span the road once (SnowTexture is drawn for the five-lane road)
					break;
				case Road.Grid:
					emissionMap = GridTexture();
					break;
			}
			game.SetMaterialColor(m, p.RoadTint);
			if (m.HasProperty(SmoothnessId)) m.SetFloat(SmoothnessId, p.RoadSmoothness);
			if (m.HasProperty(MetallicId)) m.SetFloat(MetallicId, p.RoadMetallic);
			bool reflect = p.RoadKind == Road.Wet || p.RoadKind == Road.Grid;
			if (m.HasProperty("_EnvironmentReflections")) m.SetFloat("_EnvironmentReflections", reflect ? 1f : 0f);
			if (reflect) m.DisableKeyword("_ENVIRONMENTREFLECTIONS_OFF"); else m.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
			if (game.roadMatHasBaseMap) { m.SetTexture(BaseMapProperty, baseMap); m.SetTextureScale(BaseMapProperty, tiling); }
			if (game.roadMatHasMainTex) { m.SetTexture(MainTexProperty, baseMap); m.SetTextureScale(MainTexProperty, tiling); }
			if (game.roadMatHasBumpMap)
			{
				m.SetTexture(BumpMapProperty, bump);
				m.SetTextureScale(BumpMapProperty, tiling);
				if (m.HasProperty(BumpScaleProperty)) m.SetFloat(BumpScaleProperty, bumpScale);
				if (bump != null) m.EnableKeyword("_NORMALMAP"); else m.DisableKeyword("_NORMALMAP");
			}
			if (game.roadMatHasOcclusionMap)
			{
				m.SetTexture(OcclusionMapProperty, occlusion);
				m.SetTextureScale(OcclusionMapProperty, tiling);
				if (occlusion != null) m.EnableKeyword("_OCCLUSIONMAP"); else m.DisableKeyword("_OCCLUSIONMAP");
			}
			if (m.HasProperty(EmissionMapProperty)) m.SetTexture(EmissionMapProperty, emissionMap);
			if (m.HasProperty(EmissionColorProperty))
			{
				m.SetColor(EmissionColorProperty, p.RoadEmission);
				m.EnableKeyword("_EMISSION");
			}
			if (s_roadSurface != null) s_roadSurface.reflectionProbeUsage = reflect ? ReflectionProbeUsage.BlendProbes : ReflectionProbeUsage.Off;
			if (reflect) EnsureRoadProbe(game);
			SetLit(game, game.lineMat, p.LaneColor, p.LaneEmission);
			SetLit(game, s_edgeMat, p.EdgeColor, p.EdgeEmission);
			for (int i = 0; i < s_laneRenderers.Count; i++) if (s_laneRenderers[i] != null) s_laneRenderers[i].enabled = p.Lanes;
			for (int i = 0; i < s_edgeRenderers.Count; i++) if (s_edgeRenderers[i] != null) s_edgeRenderers[i].enabled = p.Lanes;
			SetLit(game, game.cityGroundMat, p.Verge, null);
			if (game.cityGroundMat != null && game.cityGroundMat.HasProperty(SmoothnessId)) game.cityGroundMat.SetFloat(SmoothnessId, p.VergeSmoothness);
		}

		private static void SetLit(ShadowRunPrototype game, Material mat, Color color, Color? emission)
		{
			if (mat == null) return;
			game.SetMaterialColor(mat, color);
			if (emission.HasValue && mat.HasProperty(EmissionColorProperty))
			{
				mat.SetColor(EmissionColorProperty, emission.Value);
				mat.EnableKeyword("_EMISSION");
			}
		}

		// A realtime probe rendered once per Apply (ViaScripting) so the wet / glass road reflects the city
		// and the current sky; box-projected over the whole road like the shipped CreateRoadReflectionProbe
		// (which is never called and would also dim every reflection to 0.35).
		private static void EnsureRoadProbe(ShadowRunPrototype game)
		{
			if (s_roadProbe == null)
			{
				GameObject probeObject = new GameObject("Map Road Reflection Probe");
				probeObject.transform.position = new Vector3(0f, 5.5f, -120f);
				s_roadProbe = probeObject.AddComponent<ReflectionProbe>();
				s_roadProbe.mode = ReflectionProbeMode.Realtime;
				s_roadProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
				s_roadProbe.timeSlicingMode = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
				s_roadProbe.importance = 1;
				s_roadProbe.intensity = 0.9f;
				s_roadProbe.boxProjection = true;
				s_roadProbe.size = new Vector3(InfernoLanes.Widen(34f), 18f, 1900f); /* [user request 2026-09-22: five lanes] */ // original: 34f wide, as the shipped probe
				s_roadProbe.center = new Vector3(0f, -3.5f, -780f);
				s_roadProbe.resolution = 64;
				s_roadProbe.nearClipPlane = 0.3f;
				s_roadProbe.farClipPlane = 950f;
				s_roadProbe.cullingMask = -1;
			}
			QualitySettings.realtimeReflectionProbes = true;
			s_roadProbe.RenderProbe();
		}

		// ---- the shipped city pieces --------------------------------------------------------------
		private static void CollectCity(ShadowRunPrototype game)
		{
			if (s_cityCollected) return;
			s_cityCollected = true;
			for (int i = 0; i < game.cityObjects.Count; i++)
			{
				Transform t = game.cityObjects[i];
				if (t == null) continue;
				string n = t.name;
				if (n.StartsWith("City Building", StringComparison.Ordinal)) s_buildings.Add(t);
				else if (n.IndexOf("Neon Fence", StringComparison.Ordinal) >= 0) s_fences.Add(t);
				else if (n.IndexOf("Street Lamp", StringComparison.Ordinal) >= 0) s_lamps.Add(t);
			}
			foreach (KeyValuePair<Material, Material> pair in game.cityUnlitMaterials)
			{
				Material mat = pair.Value;
				if (mat == null || s_buildingBaseColors.ContainsKey(mat)) continue;
				Color c = mat.HasProperty(BaseColorProperty) ? mat.GetColor(BaseColorProperty) : (mat.HasProperty(ColorProperty) ? mat.GetColor(ColorProperty) : Color.white);
				s_buildingBaseColors[mat] = c;
			}
			GameObject roadObject = GameObject.Find("Continuous Road");
			if (roadObject != null)
			{
				s_roadRoot = roadObject.transform;
				for (int i = 0; i < s_roadRoot.childCount; i++)
				{
					Transform child = s_roadRoot.GetChild(i);
					Renderer r = child.GetComponent<Renderer>();
					if (r == null) continue;
					if (child.name == "Road Surface") s_roadSurface = r;
					else if (child.name == "Left Edge" || child.name == "Right Edge") s_edgeRenderers.Add(r);
					else if (child.name == "Lane Mark") s_laneRenderers.Add(r);
				}
				if (game.lineMat != null && s_edgeMat == null)
				{
					s_edgeMat = new Material(game.lineMat);
					s_edgeMat.name = "Runtime Map Road Edge";
					for (int i = 0; i < s_edgeRenderers.Count; i++) s_edgeRenderers[i].sharedMaterial = s_edgeMat;
				}
				// Far ground beyond the verges (x > 22.5; 19.1 with the shipped road) so maps without buildings do
				// not show the skybox below the horizon; the verge material keeps them the same colour.
				if (s_roadRoot.Find("Map Ground L") == null && game.cityGroundMat != null)
				{
					game.AddPlane(s_roadRoot, "Map Ground L", new Vector3(-(169.1f + InfernoLanes.Shift), 0.008f, 0f), new Vector2(300f, RoadTextureLength), game.cityGroundMat); /* [user request 2026-09-22: five lanes] */ // original: -169.1f
					game.AddPlane(s_roadRoot, "Map Ground R", new Vector3(169.1f + InfernoLanes.Shift, 0.008f, 0f), new Vector2(300f, RoadTextureLength), game.cityGroundMat); /* [user request 2026-09-22: five lanes] */ // original: 169.1f
				}
			}
		}

		private static void ApplyCity(ShadowRunPrototype game, Profile p)
		{
			for (int i = 0; i < s_buildings.Count; i++) if (s_buildings[i] != null && s_buildings[i].gameObject.activeSelf != p.Buildings) s_buildings[i].gameObject.SetActive(p.Buildings);
			for (int i = 0; i < s_fences.Count; i++) if (s_fences[i] != null && s_fences[i].gameObject.activeSelf != p.Fences) s_fences[i].gameObject.SetActive(p.Fences);
			for (int i = 0; i < s_lamps.Count; i++) if (s_lamps[i] != null && s_lamps[i].gameObject.activeSelf != p.Lamps) s_lamps[i].gameObject.SetActive(p.Lamps);
			foreach (KeyValuePair<Material, Color> pair in s_buildingBaseColors)
			{
				Material mat = pair.Key;
				if (mat == null) continue;
				Color c = pair.Value;
				Color tinted = new Color(c.r * p.BuildingTint.r, c.g * p.BuildingTint.g, c.b * p.BuildingTint.b, c.a);
				if (mat.HasProperty(BaseColorProperty)) mat.SetColor(BaseColorProperty, tinted);
				if (mat.HasProperty(ColorProperty)) mat.SetColor(ColorProperty, tinted);
			}
			SetLit(game, game.neonFenceMat, p.FenceColor, p.FenceColor * p.FenceGlow);
			Material lamp = game.streetLampRedEmissiveMat;
			if (lamp != null)
			{
				if (lamp.HasProperty("_EmissionColor")) lamp.SetColor("_EmissionColor", p.LampEmission);
				if (lamp.HasProperty("_EmissiveColor")) lamp.SetColor("_EmissiveColor", p.LampEmission);
				if (lamp.HasProperty("_EmissiveFactor")) lamp.SetColor("_EmissiveFactor", p.LampEmission);
			}
		}

		// ---- extras: pulsar stars, falling stars, rain audio -------------------------------------------
		private static void ApplyExtras(ShadowRunPrototype game, Profile p)
		{
			if (p.Pulsar)
			{
				game.EnsurePulsarNightObject();
				if (game.pulsarNightRoot != null)
				{
					game.pulsarNightVisibility = 1f;
					if (!game.pulsarNightRoot.gameObject.activeSelf)
					{
						game.pulsarNightRoot.gameObject.SetActive(true);
						game.RestartBluePulsarRays();
					}
					if (game.cam != null && game.cam.enabled) game.SetPulsarNightCameraActive(true);
					game.AnimatePulsarNightSystem(1f);
				}
			}
			else if (game.pulsarNightRoot != null && game.pulsarNightRoot.gameObject.activeSelf)
			{
				game.pulsarNightVisibility = 0f;
				game.pulsarNightRoot.gameObject.SetActive(false);
				game.SetPulsarNightCameraActive(false);
			}
			if (p.FallingStars)
			{
				game.EnsureFallenSkyFallingStars();
				if (game.fallenSkyFallingStars != null && !game.fallenSkyFallingStarsActive)
				{
					game.fallenSkyFallingStarsActive = true;
					game.fallenSkyFallingStars.SetActive(true);
					ParticleSystem[] systems = game.fallenSkyFallingStars.GetComponentsInChildren<ParticleSystem>(true);
					for (int i = 0; i < systems.Length; i++) systems[i].Play(true);
				}
			}
			else if (game.fallenSkyFallingStars != null && game.fallenSkyFallingStarsActive)
			{
				game.fallenSkyFallingStarsActive = false;
				ParticleSystem[] systems = game.fallenSkyFallingStars.GetComponentsInChildren<ParticleSystem>(true);
				for (int i = 0; i < systems.Length; i++) systems[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				game.fallenSkyFallingStars.SetActive(false);
			}
			if (p.RainAudio)
			{
				if (s_rainSource == null)
				{
					AudioClip clip = Resources.Load<AudioClip>(RainAudioResource);
					if (clip != null)
					{
						GameObject audioObject = new GameObject("Map Rain Audio");
						audioObject.transform.SetParent(game.transform, false);
						s_rainSource = audioObject.AddComponent<AudioSource>();
						s_rainSource.clip = clip;
						s_rainSource.loop = true;
						s_rainSource.playOnAwake = false;
						s_rainSource.spatialBlend = 0f;
						s_rainSource.dopplerLevel = 0f;
						s_rainSource.priority = 64;
					}
				}
				if (s_rainSource != null)
				{
					s_rainVolume = p.RainVolume;
					s_rainMuted = !game.running || game.paused;
					s_rainMuteKnown = true;
					s_rainSource.volume = RainVolume(game);
					if (!s_rainSource.isPlaying) s_rainSource.Play();
				}
			}
			else if (s_rainSource != null && s_rainSource.isPlaying)
			{
				s_rainSource.Stop();
			}
		}

		private static float RainVolume(ShadowRunPrototype game)
		{
			if (s_rainMuted || !game.gameSoundEnabled) return 0f;
			return s_rainVolume * Mathf.Clamp01(game.sfxVolume / 100f);
		}

		// Called every frame from UpdateWeatherAudioMute (the weather stub's AudioModule.SetMuted is inert).
		public static void SetRainAudioMuted(ShadowRunPrototype game, bool muted)
		{
			if (s_game != game || s_rainSource == null) return;
			if (s_rainMuteKnown && s_rainMuted == muted) return;
			s_rainMuted = muted;
			s_rainMuteKnown = true;
			s_rainSource.volume = RainVolume(game);
		}

		// ---- lightning (Storm Chase) ----------------------------------------------------------------
		private static float NextLightningDelay()
		{
			return 6f + (float)s_rng.NextDouble() * 8f;
		}

		private static void TickLightning(ShadowRunPrototype game, Profile p, float dt)
		{
			s_lightningTimer -= dt;
			if (s_lightningTimer <= 0f)
			{
				s_lightningTimer = NextLightningDelay();
				s_flashTime = FlashDuration;
				s_flashing = true;
				EnsureBolt(game);
				if (s_boltQuad != null && s_rng.NextDouble() < 0.7)
				{
					float x = -18f + (float)s_rng.NextDouble() * 36f;
					s_boltQuad.localPosition = new Vector3(x, 13f, 70f);
					s_boltQuad.localScale = new Vector3(s_rng.NextDouble() < 0.5 ? 13f : -13f, 26f, 1f);
					s_boltQuad.gameObject.SetActive(true);
				}
			}
			if (!s_flashing) return;
			s_flashTime -= dt;
			float k = Mathf.Clamp01(s_flashTime / FlashDuration);
			if (game.sceneKeyLight != null) game.sceneKeyLight.intensity = s_keyBaseIntensity * (1f + 2f * k);
			RenderSettings.ambientLight = p.Ambient * (1f + 1.2f * k);
			if (s_skyActive != null && s_skyActive.HasProperty(ExposureId)) s_skyActive.SetFloat(ExposureId, s_skyBaseExposure * (1f + 0.9f * k));
			if (s_boltQuad != null && s_boltQuad.gameObject.activeSelf)
			{
				float alpha = Mathf.Clamp01((s_flashTime + 0.14f) / (FlashDuration + 0.14f));
				game.SetMaterialColor(s_boltMat, new Color(0.85f, 0.9f, 1f, alpha * alpha));
				if (s_flashTime <= -0.14f) s_boltQuad.gameObject.SetActive(false);
			}
			if (s_flashTime <= -0.14f)
			{
				s_flashing = false;
				if (game.sceneKeyLight != null) game.sceneKeyLight.intensity = s_keyBaseIntensity;
				RenderSettings.ambientLight = p.Ambient;
				if (s_skyActive != null && s_skyActive.HasProperty(ExposureId)) s_skyActive.SetFloat(ExposureId, s_skyBaseExposure);
			}
		}

		private static void EnsureBolt(ShadowRunPrototype game)
		{
			if (s_boltQuad != null || game.cam == null) return;
			if (s_boltTexture == null) s_boltTexture = MakeBoltTexture(96, 192, 0x7E11);
			s_boltMat = game.MatTransparentUnlit(new Color(0.85f, 0.9f, 1f, 0f), s_boltTexture, true);
			s_boltMat.name = "Runtime Map Lightning Bolt";
			s_boltMat.renderQueue = 3040;
			GameObject quad = game.Quad("Map Lightning Bolt", Vector3.zero, new Vector3(13f, 26f, 1f), s_boltMat);
			quad.transform.SetParent(game.cam.transform, false);
			quad.transform.localPosition = new Vector3(0f, 13f, 70f);
			quad.transform.localRotation = Quaternion.identity;
			s_boltQuad = quad.transform;
			s_boltQuad.gameObject.SetActive(false);
		}

		// ---- procedural textures --------------------------------------------------------------------
		private static float Unit(uint hash) { return HashAsphaltToUnit(HashAsphalt(hash)); }

		private static Texture2D MakeTexture(Color[] pixels, int w, int h, string name, bool wrap, bool linear)
		{
			Texture2D texture = new Texture2D(w, h, TextureFormat.RGBA32, true, linear);
			texture.name = name;
			texture.wrapMode = wrap ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Trilinear;
			texture.anisoLevel = 4;
			texture.SetPixels(pixels);
			texture.Apply(true, true);
			return texture;
		}

		// Cracked sand-blown asphalt: tan value noise + grain + a few random-walk cracks (wrapped, so it tiles).
		private static Texture2D SandTexture()
		{
			if (s_sandTexture != null) return s_sandTexture;
			const int size = 256;
			System.Random rng = new System.Random(0x5A4D);
			Lattice coarse = new Lattice(rng, 6, 6), fine = new Lattice(rng, 24, 24);
			float[] crack = new float[size * size];
			for (int walk = 0; walk < 11; walk++)
			{
				float x = (float)rng.NextDouble() * size, y = (float)rng.NextDouble() * size;
				float angle = (float)rng.NextDouble() * Mathf.PI * 2f;
				int steps = 60 + rng.Next(110);
				for (int i = 0; i < steps; i++)
				{
					x += Mathf.Cos(angle); y += Mathf.Sin(angle);
					angle += ((float)rng.NextDouble() - 0.5f) * 0.9f;
					int px = ((Mathf.RoundToInt(x) % size) + size) % size, py = ((Mathf.RoundToInt(y) % size) + size) % size;
					crack[px + py * size] = 1f;
					int nx = (px + 1) % size, ny = (py + 1) % size;
					crack[nx + py * size] = Mathf.Max(crack[nx + py * size], 0.45f);
					crack[px + ny * size] = Mathf.Max(crack[px + ny * size], 0.45f);
				}
			}
			Color[] pixels = new Color[size * size];
			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					float u = (x + 0.5f) / size, v = (y + 0.5f) / size;
					float value = 0.56f + (coarse.Sample(u, v) - 0.5f) * 0.14f + (fine.Sample(u, v) - 0.5f) * 0.08f;
					uint hash = (uint)(x + y * size) ^ 0x3C6EF372u;
					value += (Unit(hash) - 0.5f) * 0.1f;
					if (Unit(hash ^ 0x9E3779B9u) < 0.03f) value += 0.1f;
					value *= 1f - crack[x + y * size] * 0.55f;
					value = Mathf.Clamp01(value);
					pixels[x + y * size] = new Color(value * 1f, value * 0.86f, value * 0.66f, 1f);
				}
			}
			s_sandTexture = MakeTexture(pixels, size, size, "Runtime Map Sand Asphalt", true, false);
			return s_sandTexture;
		}

		// Packed snow with two darker tyre-track stripes per lane (x tiling 1 over the road, so each lane centre
		// maps to u = (x + HalfRoad) / RoadWidth, tyres 0.75 m either side of it). The texture is twice as wide
		// as it is tall and the noise is sampled in road metres, so the grain and the blotches keep their shipped
		// size on the 17 m road instead of stretching with it.
		/* [user request 2026-09-22: five lanes] */ // original: a 256 x 256 tile with lanes { 0.196f, 0.5f, 0.804f } (-3.1 / 0 / 3.1 over the 10.2 m road), tyre offset 0.0735 and track edges 0.021 / 0.011 in u
		private static Texture2D SnowTexture()
		{
			if (s_snowTexture != null) return s_snowTexture;
			const int w = 512, h = 256;
			float shippedWidth = InfernoLanes.ShippedHalfRoad * 2f;          // 10.2 m
			float stretch = InfernoLanes.RoadWidth / shippedWidth;           // 1.667: u -> shipped noise metres
			System.Random rng = new System.Random(0x5A0E);
			Lattice coarse = new Lattice(rng, 8, 8), fine = new Lattice(rng, 32, 32);
			float[] lanes = new float[InfernoLanes.X.Length];
			for (int i = 0; i < lanes.Length; i++) lanes[i] = (InfernoLanes.X[i] + InfernoLanes.HalfRoad) / InfernoLanes.RoadWidth;   // 0.135 / 0.318 / 0.5 / 0.682 / 0.865
			float tyre = 0.0735f * shippedWidth / InfernoLanes.RoadWidth;     // 0.75 m either side of the lane centre, in u
			float trackOuter = 0.021f / stretch, trackInner = 0.011f / stretch;   // the shipped 0.21 / 0.11 m track edges, in u
			Color[] pixels = new Color[w * h];
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					float u = (x + 0.5f) / w, v = (y + 0.5f) / h;
					float un = u * stretch;
					float value = 0.93f + (coarse.Sample(un, v) - 0.5f) * 0.07f + (fine.Sample(un, v) - 0.5f) * 0.05f;
					uint hash = (uint)(x + y * w) ^ 0x1B873593u;
					value += (Unit(hash) - 0.5f) * 0.05f;
					if (Unit(hash ^ 0x85EBCA6Bu) < 0.02f) value += 0.07f;
					float track = 0f;
					for (int lane = 0; lane < lanes.Length; lane++)
					{
						for (int side = -1; side <= 1; side += 2)
						{
							float d = Mathf.Abs(u - (lanes[lane] + side * tyre));
							track = Mathf.Max(track, Smooth(trackOuter, trackInner, d));
						}
					}
					track *= 0.8f + 0.2f * fine.Sample(un * 3f, v);
					Color snow = new Color(value * 0.94f, value * 0.96f, value, 1f);
					Color packed = new Color(value * 0.52f, value * 0.56f, value * 0.66f, 1f);
					pixels[x + y * w] = Color.Lerp(snow, packed, track);
				}
			}
			s_snowTexture = MakeTexture(pixels, w, h, "Runtime Map Snow", true, false);
			return s_snowTexture;
		}

		// Emission map for the glass road: a soft-glow grid of ~1.9 m squares (4 x 6 cells per tile at the
		// shipped 1.35 x 170 tiling over the 10.2 x 1900 m road). The cell count across the tile is recomputed
		// from the road width and its x tiling so the squares keep their size in metres on the wider road.
		/* [user request 2026-09-22: five lanes] */ // original: a fixed 4 cells across the tile (size / 4f, half cell size / 8f)
		private static Texture2D GridTexture()
		{
			if (s_gridTexture != null) return s_gridTexture;
			const int size = 256;
			float shippedCell = InfernoLanes.ShippedHalfRoad * 2f / RoadTextureTiling.x / 4f;                     // 1.89 m: the shipped square
			float cellsX = Mathf.Max(1f, Mathf.Round(InfernoLanes.RoadWidth / InfernoLanes.TextureTilingX / shippedCell));   // 4: the x tile is still 7.56 m
			float cellX = size / cellsX;
			Color[] pixels = new Color[size * size];
			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					float dx = Mathf.Abs(Mathf.Repeat(x + 0.5f, cellX) - cellX * 0.5f);
					float dy = Mathf.Abs(Mathf.Repeat(y + 0.5f, size / 6f) - size / 12f);
					float lineX = cellX * 0.5f - dx, lineY = size / 12f - dy;
					float d = Mathf.Min(lineX, lineY);
					float line = Mathf.Exp(-(d * d) / (1.6f * 1.6f)) + 0.35f * Mathf.Exp(-(d * d) / (6f * 6f));
					line = Mathf.Clamp01(line);
					pixels[x + y * size] = new Color(line, line, line, 1f);
				}
			}
			s_gridTexture = MakeTexture(pixels, size, size, "Runtime Map Glass Grid", true, false);
			return s_gridTexture;
		}

		// Soft radial dot: rgb == alpha so it works both alpha-blended and additive.
		private static Texture2D DotTexture()
		{
			if (s_dotTexture != null) return s_dotTexture;
			const int size = 64;
			Color[] pixels = new Color[size * size];
			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					float u = (x + 0.5f) / size * 2f - 1f, v = (y + 0.5f) / size * 2f - 1f;
					float r = Mathf.Sqrt(u * u + v * v);
					float a = Mathf.Pow(Smooth(1f, 0.15f, r), 1.6f);
					pixels[x + y * size] = new Color(a, a, a, a);
				}
			}
			s_dotTexture = MakeTexture(pixels, size, size, "Runtime Map Soft Dot", false, false);
			return s_dotTexture;
		}

		// A lightning bolt: a jittered random walk from the top with two branches, rasterised with a core
		// and a wide glow.
		private static Texture2D MakeBoltTexture(int w, int h, int seed)
		{
			System.Random rng = new System.Random(seed);
			float[] glow = new float[w * h];
			List<Vector4> segments = new List<Vector4>(64);
			Walk(segments, rng, w * 0.5f, h - 4f, -Mathf.PI * 0.5f, h * 0.92f, 0.35f);
			Walk(segments, rng, w * 0.5f + 4f, h * 0.68f, -Mathf.PI * 0.5f + 0.7f, h * 0.3f, 0.5f);
			Walk(segments, rng, w * 0.5f - 6f, h * 0.45f, -Mathf.PI * 0.5f - 0.6f, h * 0.25f, 0.5f);
			for (int i = 0; i < segments.Count; i++)
			{
				Vector4 seg = segments[i];
				Vector2 a = new Vector2(seg.x, seg.y), b = new Vector2(seg.z, seg.w);
				int minX = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(a.x, b.x)) - 7), maxX = Mathf.Min(w - 1, Mathf.CeilToInt(Mathf.Max(a.x, b.x)) + 7);
				int minY = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(a.y, b.y)) - 7), maxY = Mathf.Min(h - 1, Mathf.CeilToInt(Mathf.Max(a.y, b.y)) + 7);
				Vector2 ab = b - a;
				float len2 = Mathf.Max(0.0001f, ab.sqrMagnitude);
				for (int y = minY; y <= maxY; y++)
				{
					for (int x = minX; x <= maxX; x++)
					{
						Vector2 pt = new Vector2(x + 0.5f, y + 0.5f);
						float t = Mathf.Clamp01(Vector2.Dot(pt - a, ab) / len2);
						float d = Vector2.Distance(pt, a + ab * t);
						glow[x + y * w] += Mathf.Exp(-(d * d) / (1.1f * 1.1f)) + 0.22f * Mathf.Exp(-(d * d) / (5f * 5f));
					}
				}
			}
			Color[] pixels = new Color[w * h];
			for (int i = 0; i < pixels.Length; i++)
			{
				float a = Mathf.Clamp01(glow[i]);
				pixels[i] = new Color(a, a * 0.96f, a, a);
			}
			return MakeTexture(pixels, w, h, "Runtime Map Lightning Bolt", false, false);
		}

		private static void Walk(List<Vector4> segments, System.Random rng, float x, float y, float angle, float length, float jitter)
		{
			float travelled = 0f;
			while (travelled < length && y > 2f)
			{
				float step = 6f + (float)rng.NextDouble() * 10f;
				float nx = x + Mathf.Cos(angle) * step, ny = y + Mathf.Sin(angle) * step;
				segments.Add(new Vector4(x, y, nx, ny));
				x = nx; y = ny;
				angle += ((float)rng.NextDouble() - 0.5f) * 2f * jitter;
				angle = Mathf.Clamp(angle, -Mathf.PI * 0.5f - 1.1f, -Mathf.PI * 0.5f + 1.1f);
				travelled += step;
			}
		}

		// ---- mesh kit ---------------------------------------------------------------------------------
		// Flat-shaded low-poly builder: every face gets its own vertices, so RecalculateNormals gives crisp
		// facets. Boxes follow the winding of the game's AddBoxGeometry; cones the game's Cone.
		private sealed class MeshKit
		{
			private readonly List<Vector3> verts = new List<Vector3>(512);
			private readonly List<int>[] tris;

			public MeshKit(int subMeshes)
			{
				tris = new List<int>[Mathf.Max(1, subMeshes)];
				for (int i = 0; i < tris.Length; i++) tris[i] = new List<int>(512);
			}

			private void Tri(Vector3 a, Vector3 b, Vector3 c, int sub)
			{
				int s = verts.Count;
				verts.Add(a); verts.Add(b); verts.Add(c);
				List<int> t = tris[Mathf.Clamp(sub, 0, tris.Length - 1)];
				t.Add(s); t.Add(s + 1); t.Add(s + 2);
			}

			private void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int sub)
			{
				Tri(a, b, c, sub);
				Tri(a, c, d, sub);
			}

			public void Box(Vector3 center, Vector3 size, Quaternion rotation, int sub)
			{
				Vector3 h = size * 0.5f;
				Vector3 c0 = center + rotation * new Vector3(-h.x, -h.y, -h.z);
				Vector3 c1 = center + rotation * new Vector3(h.x, -h.y, -h.z);
				Vector3 c2 = center + rotation * new Vector3(h.x, -h.y, h.z);
				Vector3 c3 = center + rotation * new Vector3(-h.x, -h.y, h.z);
				Vector3 c4 = center + rotation * new Vector3(-h.x, h.y, -h.z);
				Vector3 c5 = center + rotation * new Vector3(h.x, h.y, -h.z);
				Vector3 c6 = center + rotation * new Vector3(h.x, h.y, h.z);
				Vector3 c7 = center + rotation * new Vector3(-h.x, h.y, h.z);
				Quad(c0, c1, c2, c3, sub);
				Quad(c4, c7, c6, c5, sub);
				Quad(c0, c4, c5, c1, sub);
				Quad(c1, c5, c6, c2, sub);
				Quad(c2, c6, c7, c3, sub);
				Quad(c3, c7, c4, c0, sub);
			}

			public void Box(Vector3 center, Vector3 size, int sub)
			{
				Box(center, size, Quaternion.identity, sub);
			}

			// Upright cone, apex at baseCenter + height.
			public void Cone(Vector3 baseCenter, float radius, float height, int sides, int sub, bool cap)
			{
				Vector3 apex = baseCenter + Vector3.up * height;
				for (int i = 0; i < sides; i++)
				{
					float a0 = i * Mathf.PI * 2f / sides, a1 = (i + 1) * Mathf.PI * 2f / sides;
					Vector3 p0 = baseCenter + new Vector3(Mathf.Cos(a0) * radius, 0f, Mathf.Sin(a0) * radius);
					Vector3 p1 = baseCenter + new Vector3(Mathf.Cos(a1) * radius, 0f, Mathf.Sin(a1) * radius);
					Tri(apex, p1, p0, sub);
					if (cap) Tri(baseCenter, p0, p1, sub);
				}
			}

			// Tube (frustum) from a to b; the winding is checked against the outward direction.
			public void Tube(Vector3 a, Vector3 b, float radiusA, float radiusB, int sides, int sub, bool caps)
			{
				Vector3 axis = (b - a);
				float length = axis.magnitude;
				if (length < 0.0001f) return;
				axis /= length;
				Vector3 helper = Mathf.Abs(axis.y) < 0.99f ? Vector3.up : Vector3.right;
				Vector3 t1 = Vector3.Cross(helper, axis).normalized;
				Vector3 t2 = Vector3.Cross(t1, axis);
				Vector3[] ringA = new Vector3[sides], ringB = new Vector3[sides];
				for (int i = 0; i < sides; i++)
				{
					float angle = i * Mathf.PI * 2f / sides;
					Vector3 dir = t1 * Mathf.Cos(angle) + t2 * Mathf.Sin(angle);
					ringA[i] = a + dir * radiusA;
					ringB[i] = b + dir * radiusB;
				}
				// orientation test on the first side quad
				Vector3 n = Vector3.Cross(ringB[0] - ringA[0], ringB[1 % sides] - ringA[0]);
				Vector3 outward = (ringA[0] + ringA[1 % sides]) * 0.5f - a;
				bool flip = Vector3.Dot(n, outward) < 0f;
				for (int i = 0; i < sides; i++)
				{
					int j = (i + 1) % sides;
					if (flip) Quad(ringA[i], ringA[j], ringB[j], ringB[i], sub);
					else Quad(ringA[i], ringB[i], ringB[j], ringA[j], sub);
					if (caps)
					{
						if (flip) { Tri(b, ringB[i], ringB[j], sub); Tri(a, ringA[j], ringA[i], sub); }
						else { Tri(b, ringB[j], ringB[i], sub); Tri(a, ringA[i], ringA[j], sub); }
					}
				}
			}

			public Mesh Build(string name)
			{
				Mesh mesh = new Mesh();
				mesh.name = name;
				mesh.SetVertices(verts);
				mesh.subMeshCount = tris.Length;
				for (int i = 0; i < tris.Length; i++) mesh.SetTriangles(tris[i], i);
				mesh.RecalculateNormals();
				mesh.RecalculateBounds();
				return mesh;
			}
		}

		private static float R(System.Random rng, float min, float max)
		{
			return min + (float)rng.NextDouble() * (max - min);
		}

		private static Material M(ShadowRunPrototype game, string key, Color color, Color? emission, float smoothness, float metallic)
		{
			Material mat;
			if (s_materials.TryGetValue(key, out mat) && mat != null) return mat;
			mat = game.Mat(color, emission);
			mat.name = "Runtime Map " + key;
			if (mat.HasProperty(SmoothnessId)) mat.SetFloat(SmoothnessId, smoothness);
			if (mat.HasProperty(MetallicId)) mat.SetFloat(MetallicId, metallic);
			s_materials[key] = mat;
			return mat;
		}

		private static Mesh CachedMesh(string key, Func<Mesh> build)
		{
			Mesh mesh;
			if (s_meshes.TryGetValue(key, out mesh) && mesh != null) return mesh;
			mesh = build();
			s_meshes[key] = mesh;
			return mesh;
		}

		private static void Prep(Renderer r)
		{
			r.shadowCastingMode = ShadowCastingMode.Off;
			r.receiveShadows = false;
			r.lightProbeUsage = LightProbeUsage.Off;
			r.reflectionProbeUsage = ReflectionProbeUsage.Off;
			r.allowOcclusionWhenDynamic = false;
		}

		// A scenery object under the Map Scenery root, registered in cityObjects so UpdateWorld loops it.
		private static Transform Spawn(ShadowRunPrototype game, string name, Mesh mesh, Material[] materials, float x, float y, float worldZ, float yaw, float scale)
		{
			GameObject go = new GameObject(name);
			Transform t = go.transform;
			t.SetParent(s_sceneryRoot, false);
			t.localPosition = new Vector3(x, y, worldZ - s_cityZ);
			t.localRotation = Quaternion.Euler(0f, yaw, 0f);
			t.localScale = Vector3.one * scale;
			go.AddComponent<MeshFilter>().sharedMesh = mesh;
			MeshRenderer r = go.AddComponent<MeshRenderer>();
			r.sharedMaterials = materials;
			Prep(r);
			game.cityObjects.Add(t);
			return t;
		}

		private static Transform SpawnSphere(ShadowRunPrototype game, string name, Material mat, float x, float y, float worldZ, Vector3 scale)
		{
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			go.name = name;
			game.PreparePrimitive(go, mat);
			Transform t = go.transform;
			t.SetParent(s_sceneryRoot, false);
			t.localPosition = new Vector3(x, y, worldZ - s_cityZ);
			t.localRotation = Quaternion.identity;
			t.localScale = scale;
			game.cityObjects.Add(t);
			return t;
		}

		private static void ClearScenery(ShadowRunPrototype game)
		{
			if (s_sceneryRoot != null)
			{
				Transform old = s_sceneryRoot;
				List<Transform> objects = game.cityObjects;
				for (int i = objects.Count - 1; i >= 0; i--)
				{
					Transform t = objects[i];
					if (t == null || t.parent == old) objects.RemoveAt(i);
				}
				old.gameObject.SetActive(false);   // Destroy is deferred; hide it now (thumbnail captures apply several maps in one frame)
				UnityEngine.Object.Destroy(old.gameObject);
			}
			s_sceneryRoot = null;
			s_floaters.Clear();
		}

		private static void BuildScenery(ShadowRunPrototype game, Profile p)
		{
			ClearScenery(game);
			if (game.cityRoot == null) return;
			s_sceneryRoot = new GameObject("Map Scenery").transform;
			s_sceneryRoot.SetParent(game.cityRoot, false);
			s_sceneryRoot.localPosition = Vector3.zero;
			s_sceneryRoot.localRotation = Quaternion.identity;
			s_cityZ = game.cityRoot.position.z;
			System.Random rng = new System.Random(p.Seed);
			switch (p.SceneryKind)
			{
				case Scenery.Trees: BuildTrees(game, rng); break;
				case Scenery.Totems: BuildTotems(game, rng); break;
				case Scenery.GuardRails: BuildGuardRails(game, rng); break;
				case Scenery.Desert: BuildDesert(game, rng); break;
				case Scenery.Pines: BuildPines(game, rng); break;
				case Scenery.Crystals: BuildCrystals(game, rng); break;
			}
		}

		// Loop span: world z from -504 to +24 (CityLoop = 528 wide, objects wrap when z > 38).
		private const float LoopStart = -504f, LoopEnd = 24f;

		private static Mesh TreeMesh(int variant)
		{
			return CachedMesh("Tree" + variant, () =>
			{
				System.Random rng = new System.Random(77 + variant);
				MeshKit kit = new MeshKit(2);
				float trunk = R(rng, 1.8f, 2.6f);
				kit.Tube(Vector3.zero, new Vector3(0f, trunk, 0f), 0.24f, 0.17f, 7, 0, false);
				float r1 = R(rng, 1.7f, 2.2f), h1 = R(rng, 2.8f, 3.6f);
				kit.Cone(new Vector3(0f, trunk * 0.7f, 0f), r1, h1, 8, 1, true);
				kit.Cone(new Vector3(0f, trunk * 0.7f + h1 * 0.5f, 0f), r1 * 0.72f, h1 * 0.85f, 8, 1, false);
				return kit.Build("Runtime Map Tree " + variant);
			});
		}

		private static Mesh BarrierMesh()
		{
			return CachedMesh("Barrier", () =>
			{
				MeshKit kit = new MeshKit(1);
				const float length = 23.4f;
				kit.Box(new Vector3(0f, 0.125f, 0f), new Vector3(0.64f, 0.25f, length), 0);
				kit.Box(new Vector3(0f, 0.47f, 0f), new Vector3(0.44f, 0.46f, length), 0);
				kit.Box(new Vector3(0f, 0.86f, 0f), new Vector3(0.28f, 0.34f, length), 0);
				return kit.Build("Runtime Map Concrete Barrier");
			});
		}

		private static void BuildTrees(ShadowRunPrototype game, System.Random rng)
		{
			Material trunk = M(game, "Bark", new Color(0.36f, 0.24f, 0.14f), null, 0.05f, 0f);
			Material[] greens =
			{
				M(game, "Leaf A", new Color(0.16f, 0.45f, 0.14f), null, 0.08f, 0f),
				M(game, "Leaf B", new Color(0.2f, 0.52f, 0.18f), null, 0.08f, 0f),
				M(game, "Leaf C", new Color(0.26f, 0.5f, 0.12f), null, 0.08f, 0f),
			};
			Material concrete = M(game, "Concrete", new Color(0.68f, 0.68f, 0.65f), null, 0.12f, 0f);
			for (int side = -1; side <= 1; side += 2)
			{
				for (float z = LoopStart + R(rng, 0f, 14f); z < LoopEnd; z += 14f)
				{
					int variant = rng.Next(4);
					Material[] mats = { trunk, greens[rng.Next(greens.Length)] };
					Spawn(game, "Map Tree", TreeMesh(variant), mats, side * (R(rng, 8.4f, 10.8f) + InfernoLanes.Shift), 0f, z + R(rng, -2f, 2f), R(rng, 0f, 360f), R(rng, 0.85f, 1.3f)); /* [user request 2026-09-22: five lanes] */ // original: side * R(rng, 8.4f, 10.8f)
				}
				for (float z = LoopStart + 12f; z < LoopEnd; z += 24f)
				{
					Spawn(game, "Map Barrier", BarrierMesh(), new[] { concrete }, side * (6.15f + InfernoLanes.Shift), 0f, z, 0f, 1f); /* [user request 2026-09-22: five lanes] */ // original: side * 6.15f
				}
			}
		}

		private static Mesh TotemMesh()
		{
			return CachedMesh("Totem", () =>
			{
				MeshKit kit = new MeshKit(2);
				kit.Box(new Vector3(0f, 0.25f, 0f), new Vector3(0.9f, 0.5f, 1.7f), 0);
				kit.Box(new Vector3(0f, 4.75f, 0f), new Vector3(0.5f, 9f, 1.2f), 0);
				kit.Box(new Vector3(0f, 9.4f, 0f), new Vector3(0.6f, 0.3f, 1.4f), 0);
				for (int i = 0; i < 3; i++)
				{
					float y = 2.3f + i * 2.7f;
					kit.Box(new Vector3(0.28f, y, 0f), new Vector3(0.06f, 2.2f, 1f), 1);
				}
				kit.Box(new Vector3(0.31f, 9.62f, 0f), new Vector3(0.06f, 0.08f, 1.3f), 1);
				return kit.Build("Runtime Map Neon Totem");
			});
		}

		private static void BuildTotems(ShadowRunPrototype game, System.Random rng)
		{
			Material body = M(game, "Totem Body", new Color(0.03f, 0.03f, 0.05f), null, 0.55f, 0.3f);
			Material[] panels =
			{
				M(game, "Neon Cyan", new Color(0.1f, 0.9f, 1f), new Color(0.1f, 0.9f, 1f) * 4f, 0.5f, 0f),
				M(game, "Neon Magenta", new Color(1f, 0.2f, 0.8f), new Color(1f, 0.2f, 0.8f) * 4f, 0.5f, 0f),
				M(game, "Neon Violet", new Color(0.6f, 0.3f, 1f), new Color(0.6f, 0.3f, 1f) * 4f, 0.5f, 0f),
				M(game, "Neon Orange", new Color(1f, 0.5f, 0.1f), new Color(1f, 0.5f, 0.1f) * 4f, 0.5f, 0f),
			};
			for (int side = -1; side <= 1; side += 2)
			{
				float z = LoopStart + (side < 0 ? 8f : 38f);
				for (; z < LoopEnd; z += 60f)
				{
					Material[] mats = { body, panels[rng.Next(panels.Length)] };
					Spawn(game, "Map Totem", TotemMesh(), mats, side * (9.6f + InfernoLanes.Shift), 0f, z, side < 0 ? 0f : 180f, R(rng, 0.9f, 1.15f)); /* [user request 2026-09-22: five lanes] */ // original: side * 9.6f
				}
			}
		}

		private static Mesh GuardRailMesh()
		{
			return CachedMesh("GuardRail", () =>
			{
				MeshKit kit = new MeshKit(3);
				const float length = 23.6f;
				for (float z = -length * 0.5f + 2f; z < length * 0.5f; z += 4f)
				{
					kit.Box(new Vector3(0f, 0.38f, z), new Vector3(0.12f, 0.76f, 0.14f), 0);
					if (((int)((z + length) / 4f)) % 2 == 0) kit.Box(new Vector3(0.09f, 0.8f, z), new Vector3(0.05f, 0.07f, 0.07f), 2);
				}
				kit.Box(new Vector3(0.08f, 0.7f, 0f), new Vector3(0.1f, 0.14f, length), 1);
				kit.Box(new Vector3(0.08f, 0.5f, 0f), new Vector3(0.1f, 0.14f, length), 1);
				kit.Box(new Vector3(0.04f, 0.6f, 0f), new Vector3(0.04f, 0.34f, length), 1);
				return kit.Build("Runtime Map Guard Rail");
			});
		}

		private static void BuildGuardRails(ShadowRunPrototype game, System.Random rng)
		{
			Material post = M(game, "Rail Post", new Color(0.25f, 0.27f, 0.3f), null, 0.45f, 0.6f);
			Material rail = M(game, "Rail Beam", new Color(0.62f, 0.66f, 0.7f), null, 0.72f, 0.65f);
			Material reflector = M(game, "Rail Reflector", new Color(1f, 0.9f, 0.6f), new Color(1f, 0.9f, 0.6f) * 2f, 0.6f, 0f);
			Material[] mats = { post, rail, reflector };
			for (int side = -1; side <= 1; side += 2)
			{
				for (float z = LoopStart + 12f; z < LoopEnd; z += 24f)
				{
					Spawn(game, "Map Guard Rail", GuardRailMesh(), mats, side * (6.1f + InfernoLanes.Shift), 0f, z, side < 0 ? 0f : 180f, 1f); /* [user request 2026-09-22: five lanes] */ // original: side * 6.1f
				}
			}
		}

		private static Mesh RockMesh(int variant, string prefix)
		{
			return CachedMesh(prefix + "Rock" + variant, () =>
			{
				System.Random rng = new System.Random(300 + variant);
				MeshKit kit = new MeshKit(2);
				int count = 3 + rng.Next(3);
				for (int i = 0; i < count; i++)
				{
					Vector3 center = new Vector3(R(rng, -0.7f, 0.7f), R(rng, 0.2f, 0.9f), R(rng, -0.7f, 0.7f));
					Vector3 size = new Vector3(R(rng, 0.9f, 2.2f), R(rng, 0.8f, 1.8f), R(rng, 0.9f, 2.2f));
					Quaternion rot = Quaternion.Euler(R(rng, -30f, 30f), R(rng, 0f, 90f), R(rng, -30f, 30f));
					kit.Box(center, size, rot, 0);
				}
				if (prefix == "Float")
				{
					for (int i = 0; i < 2; i++)
					{
						Vector3 center = new Vector3(R(rng, -0.6f, 0.6f), R(rng, -0.5f, 0.3f), R(rng, -0.6f, 0.6f));
						kit.Box(center, new Vector3(0.28f, R(rng, 1.2f, 2f), 0.28f), Quaternion.Euler(R(rng, 150f, 210f), R(rng, 0f, 360f), R(rng, -20f, 20f)), 1);
					}
				}
				return kit.Build("Runtime Map " + prefix + " Rock " + variant);
			});
		}

		private static Mesh CactusMesh(int variant)
		{
			return CachedMesh("Cactus" + variant, () =>
			{
				System.Random rng = new System.Random(500 + variant);
				MeshKit kit = new MeshKit(1);
				float height = R(rng, 2.6f, 3.8f);
				kit.Tube(Vector3.zero, new Vector3(0f, height, 0f), 0.3f, 0.24f, 7, 0, true);
				int arms = 1 + rng.Next(2);
				for (int i = 0; i < arms; i++)
				{
					float side = i == 0 ? 1f : -1f;
					float y = R(rng, 1.1f, 1.9f);
					Vector3 a = new Vector3(side * 0.2f, y, 0f), b = new Vector3(side * R(rng, 0.7f, 1f), y, 0f);
					kit.Tube(a, b, 0.16f, 0.16f, 6, 0, false);
					kit.Tube(b, b + new Vector3(0f, R(rng, 0.9f, 1.5f), 0f), 0.16f, 0.13f, 6, 0, true);
				}
				return kit.Build("Runtime Map Cactus " + variant);
			});
		}

		private static void BuildDesert(ShadowRunPrototype game, System.Random rng)
		{
			Material sand = M(game, "Dune Sand", new Color(0.62f, 0.5f, 0.32f), null, 0.05f, 0f);
			Material rock = M(game, "Desert Rock", new Color(0.42f, 0.33f, 0.26f), null, 0.1f, 0f);
			Material cactus = M(game, "Cactus", new Color(0.25f, 0.42f, 0.2f), null, 0.15f, 0f);
			Material[] rockMats = { rock, rock };
			for (int side = -1; side <= 1; side += 2)
			{
				for (float z = LoopStart + R(rng, 0f, 30f); z < LoopEnd; z += R(rng, 30f, 42f))
				{
					Vector3 scale = new Vector3(R(rng, 16f, 24f), R(rng, 5f, 9f), R(rng, 24f, 40f));
					SpawnSphere(game, "Map Dune", sand, side * (R(rng, 21f, 38f) + InfernoLanes.Shift), -scale.y * 0.32f, z, scale); /* [user request 2026-09-22: five lanes] */ // original: side * R(rng, 21f, 38f)
				}
				for (float z = LoopStart + R(rng, 0f, 30f); z < LoopEnd; z += R(rng, 32f, 48f))
				{
					float scale = R(rng, 0.8f, 1.6f);
					Spawn(game, "Map Rock", RockMesh(rng.Next(3), "Desert"), rockMats, side * (R(rng, 8f, 15f) + InfernoLanes.Shift), -0.15f * scale, z, R(rng, 0f, 360f), scale); /* [user request 2026-09-22: five lanes] */ // original: side * R(rng, 8f, 15f)
				}
				for (float z = LoopStart + R(rng, 0f, 40f); z < LoopEnd; z += R(rng, 40f, 56f))
				{
					Spawn(game, "Map Cactus", CactusMesh(rng.Next(3)), new[] { cactus }, side * (R(rng, 7.6f, 12.5f) + InfernoLanes.Shift), 0f, z, R(rng, 0f, 360f), R(rng, 0.8f, 1.3f)); /* [user request 2026-09-22: five lanes] */ // original: side * R(rng, 7.6f, 12.5f)
				}
			}
		}

		private static Mesh PineMesh(int variant)
		{
			return CachedMesh("Pine" + variant, () =>
			{
				System.Random rng = new System.Random(900 + variant);
				MeshKit kit = new MeshKit(3);
				float trunk = R(rng, 1.2f, 1.7f);
				kit.Tube(Vector3.zero, new Vector3(0f, trunk, 0f), 0.22f, 0.15f, 7, 0, false);
				float y = trunk * 0.7f;
				float radius = R(rng, 1.6f, 2f), height = R(rng, 2.2f, 2.8f);
				for (int tier = 0; tier < 3; tier++)
				{
					kit.Cone(new Vector3(0f, y, 0f), radius, height, 8, 1, tier == 0);
					kit.Cone(new Vector3(0f, y + height * 0.42f + 0.02f, 0f), radius * 0.6f, height * 0.58f + 0.06f, 8, 2, false);
					y += height * 0.62f;
					radius *= 0.74f;
					height *= 0.88f;
				}
				return kit.Build("Runtime Map Pine " + variant);
			});
		}

		private static void BuildPines(ShadowRunPrototype game, System.Random rng)
		{
			Material bark = M(game, "Pine Bark", new Color(0.24f, 0.16f, 0.1f), null, 0.05f, 0f);
			Material needles = M(game, "Pine Needles", new Color(0.08f, 0.28f, 0.16f), null, 0.12f, 0f);
			Material snowCap = M(game, "Snow Cap", new Color(0.92f, 0.95f, 1f), null, 0.3f, 0f);
			Material bank = M(game, "Snow Bank", new Color(0.9f, 0.93f, 1f), null, 0.25f, 0f);
			Material[] mats = { bark, needles, snowCap };
			for (int side = -1; side <= 1; side += 2)
			{
				for (float z = LoopStart + R(rng, 0f, 12f); z < LoopEnd; z += 12f)
				{
					Spawn(game, "Map Pine", PineMesh(rng.Next(3)), mats, side * (R(rng, 8.8f, 11.6f) + InfernoLanes.Shift), 0f, z + R(rng, -1.5f, 1.5f), R(rng, 0f, 360f), R(rng, 0.85f, 1.25f)); /* [user request 2026-09-22: five lanes] */ // original: side * R(rng, 8.8f, 11.6f)
				}
				for (float z = LoopStart + R(rng, 0f, 30f); z < LoopEnd; z += R(rng, 26f, 36f))
				{
					Vector3 scale = new Vector3(R(rng, 2.6f, 4.2f), R(rng, 0.7f, 1.2f), R(rng, 5f, 9f));
					SpawnSphere(game, "Map Snow Bank", bank, side * (R(rng, 6.3f, 7.2f) + InfernoLanes.Shift), -scale.y * 0.15f, z, scale); /* [user request 2026-09-22: five lanes] */ // original: side * R(rng, 6.3f, 7.2f)
				}
			}
		}

		private static Mesh CrystalMesh(int variant)
		{
			return CachedMesh("Crystal" + variant, () =>
			{
				System.Random rng = new System.Random(1200 + variant);
				MeshKit kit = new MeshKit(1);
				int count = 3 + rng.Next(3);
				for (int i = 0; i < count; i++)
				{
					float height = R(rng, 4f, 11f);
					float width = R(rng, 0.5f, 1.1f);
					Vector3 center = new Vector3(R(rng, -2.2f, 2.2f), height * 0.38f, R(rng, -2.2f, 2.2f));
					Quaternion rot = Quaternion.Euler(R(rng, -9f, 9f), 45f + R(rng, -20f, 20f), R(rng, -9f, 9f));
					kit.Box(center, new Vector3(width, height, width), rot, 0);
				}
				return kit.Build("Runtime Map Crystal Spires " + variant);
			});
		}

		private static void BuildCrystals(ShadowRunPrototype game, System.Random rng)
		{
			Material violet = M(game, "Crystal Violet", new Color(0.3f, 0.1f, 0.5f), new Color(0.55f, 0.25f, 1f) * 2.2f, 0.9f, 0.1f);
			Material cyan = M(game, "Crystal Cyan", new Color(0.1f, 0.3f, 0.45f), new Color(0.2f, 0.85f, 1f) * 2f, 0.9f, 0.1f);
			Material rock = M(game, "Float Rock", new Color(0.14f, 0.07f, 0.22f), null, 0.2f, 0f);
			Material shard = M(game, "Float Shard", new Color(0.4f, 0.15f, 0.7f), new Color(0.7f, 0.3f, 1f) * 2.5f, 0.9f, 0f);
			Material[] rockMats = { rock, shard };
			for (int side = -1; side <= 1; side += 2)
			{
				for (float z = LoopStart + R(rng, 0f, 30f); z < LoopEnd; z += R(rng, 26f, 34f))
				{
					Material[] mats = { rng.NextDouble() < 0.65 ? violet : cyan };
					Spawn(game, "Map Crystal Spires", CrystalMesh(rng.Next(3)), mats, side * (R(rng, 8.5f, 17f) + InfernoLanes.Shift), 0f, z, R(rng, 0f, 360f), R(rng, 0.8f, 1.4f)); /* [user request 2026-09-22: five lanes] */ // original: side * R(rng, 8.5f, 17f)
				}
				for (float z = LoopStart + R(rng, 0f, 45f); z < LoopEnd; z += R(rng, 40f, 52f))
				{
					float baseY = R(rng, 6f, 14f);
					Transform t = Spawn(game, "Map Floating Rock", RockMesh(rng.Next(3), "Float"), rockMats, side * (R(rng, 11f, 22f) + InfernoLanes.Shift), baseY, z, R(rng, 0f, 360f), R(rng, 0.9f, 1.8f)); /* [user request 2026-09-22: five lanes] */ // original: side * R(rng, 11f, 22f)
					s_floaters.Add(new Floater { T = t, BaseY = baseY, Phase = R(rng, 0f, 6.28f), Speed = R(rng, 0.9f, 1.6f), Amp = R(rng, 0.4f, 0.8f), Spin = R(rng, -8f, 8f) });
				}
			}
		}

		// ---- particles ---------------------------------------------------------------------------------------
		private static Material ParticleMaterial(ShadowRunPrototype game, string key, Texture texture, bool additive)
		{
			Material mat;
			if (s_materials.TryGetValue(key, out mat) && mat != null) return mat;
			Material source = Resources.Load<Material>("Rain Particles/Materials/Rain Sprite");
			if (source != null && source.shader != null)
			{
				mat = new Material(source);
				if (texture != null) mat.mainTexture = texture;
				if (additive)
				{
					if (mat.HasProperty("_SrcBlend")) mat.SetFloat("_SrcBlend", 1f);
					if (mat.HasProperty("_DstBlend")) mat.SetFloat("_DstBlend", 1f);
					if (mat.HasProperty("_ZWrite")) mat.SetFloat("_ZWrite", 0f);
				}
			}
			else
			{
				Shader shader = Shader.Find("Sprites/Default");
				mat = shader != null ? new Material(shader) : game.MatTransparentUnlit(Color.white, texture, additive);
				if (texture != null) mat.mainTexture = texture;
			}
			mat.name = "Runtime Map " + key;
			s_materials[key] = mat;
			return mat;
		}

		private static void ClearParticles()
		{
			if (s_particleRoot != null)
			{
				s_particleRoot.gameObject.SetActive(false);
				UnityEngine.Object.Destroy(s_particleRoot.gameObject);
			}
			s_particleRoot = null;
			s_scrollers.Clear();
			s_systems.Clear();
		}

		private static Gradient FadeGradient(float fadeIn, float fadeOut)
		{
			Gradient gradient = new Gradient();
			gradient.SetKeys(
				new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
				new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, fadeIn), new GradientAlphaKey(1f, fadeOut), new GradientAlphaKey(0f, 1f) });
			return gradient;
		}

		private static ParticleSystem MakeSystem(string name, Material mat, int maxParticles, float rate, float lifeMin, float lifeMax, float sizeMin, float sizeMax, Color color, Vector3 boxSize, Vector3 boxCenter)
		{
			// Every emitter box was sized around the shipped 10.2 m road; both sides move out with the road edge
			// (rain 26 -> 32.8, splashes 11 -> 17.8, snow 30 -> 36.8, dust 36 -> 42.8 and 40 -> 46.8, motes 30 -> 36.8)
			// and the rate and cap grow with the box so the particle density per metre stays as shipped.
			/* [user request 2026-09-22: five lanes] */ // original: boxSize, rate and maxParticles used as passed
			if (boxSize.x > 0f)
			{
				float widen = InfernoLanes.Widen(boxSize.x) / boxSize.x;
				boxSize.x = InfernoLanes.Widen(boxSize.x);
				rate *= widen;
				maxParticles = Mathf.RoundToInt(maxParticles * widen);
			}
			GameObject go = new GameObject(name);
			go.transform.SetParent(s_particleRoot, false);
			ParticleSystem ps = go.AddComponent<ParticleSystem>();
			ParticleSystem.MainModule main = ps.main;
			main.loop = true;
			main.prewarm = true;
			main.playOnAwake = true;
			main.simulationSpace = ParticleSystemSimulationSpace.World;
			main.scalingMode = ParticleSystemScalingMode.Local;
			main.startLifetime = new ParticleSystem.MinMaxCurve(lifeMin, lifeMax);
			main.startSpeed = new ParticleSystem.MinMaxCurve(0f);
			main.startSize = new ParticleSystem.MinMaxCurve(sizeMin, sizeMax);
			main.startColor = new ParticleSystem.MinMaxGradient(color);
			main.gravityModifier = new ParticleSystem.MinMaxCurve(0f);
			main.maxParticles = maxParticles;
			ParticleSystem.EmissionModule emission = ps.emission;
			emission.enabled = true;
			emission.rateOverTime = new ParticleSystem.MinMaxCurve(rate);
			ParticleSystem.ShapeModule shape = ps.shape;
			shape.enabled = true;
			shape.shapeType = ParticleSystemShapeType.Box;
			shape.scale = boxSize;
			shape.position = boxCenter;
			ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
			renderer.sharedMaterial = mat;
			renderer.renderMode = ParticleSystemRenderMode.Billboard;
			renderer.sortMode = ParticleSystemSortMode.None;
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			renderer.lightProbeUsage = LightProbeUsage.Off;
			renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			s_systems.Add(ps);
			return ps;
		}

		private static void SetVelocity(ParticleSystem ps, float xMin, float xMax, float yMin, float yMax, float z)
		{
			ParticleSystem.VelocityOverLifetimeModule velocity = ps.velocityOverLifetime;
			velocity.enabled = true;
			velocity.space = ParticleSystemSimulationSpace.World;
			// Unity requires the three axes to share one curve mode, and Tick rewrites z every frame
			// as a scroll speed: keep every axis TwoConstants (z with equal ends).
			velocity.x = new ParticleSystem.MinMaxCurve(xMin, xMax);
			velocity.y = new ParticleSystem.MinMaxCurve(yMin, yMax);
			velocity.z = new ParticleSystem.MinMaxCurve(z, z);
		}

		private static void SetFade(ParticleSystem ps, float fadeIn, float fadeOut)
		{
			ParticleSystem.ColorOverLifetimeModule color = ps.colorOverLifetime;
			color.enabled = true;
			color.color = new ParticleSystem.MinMaxGradient(FadeGradient(fadeIn, fadeOut));
		}

		private static void BuildParticles(ShadowRunPrototype game, Profile p)
		{
			ClearParticles();
			if (p.ParticleKind == Particles.None) return;
			s_particleRoot = new GameObject("Map Particles").transform;
			s_particleRoot.position = Vector3.zero;
			Texture2D dot = DotTexture();
			switch (p.ParticleKind)
			{
				case Particles.Rain:
				{
					Material rainMat = ParticleMaterial(game, "Rain Drop", dot, false);
					ParticleSystem rain = MakeSystem("Map Rain", rainMat, 800, 320f, 1f, 1.4f, 0.045f, 0.08f, new Color(0.7f, 0.8f, 0.92f, 0.45f), new Vector3(26f, 2f, 60f), new Vector3(0f, 13f, -30f));
					SetVelocity(rain, 0.4f, 0.8f, -19f, -17f, 0f);
					ParticleSystemRenderer rainRenderer = rain.GetComponent<ParticleSystemRenderer>();
					rainRenderer.renderMode = ParticleSystemRenderMode.Stretch;
					rainRenderer.velocityScale = 0.05f;
					rainRenderer.lengthScale = 0f;
					s_scrollers.Add(new Scroller { System = rain, Factor = 1f });
					Material splashMat = Resources.Load<Material>("Rain Particles/Materials/Rain Ground Hit");
					if (splashMat == null) splashMat = ParticleMaterial(game, "Splash", dot, false);
					ParticleSystem splash = MakeSystem("Map Rain Splashes", splashMat, 120, 70f, 0.25f, 0.4f, 0.12f, 0.28f, new Color(0.75f, 0.85f, 1f, 0.55f), new Vector3(11f, 0f, 40f), new Vector3(0f, 0.03f, -16f));
					splash.GetComponent<ParticleSystemRenderer>().renderMode = ParticleSystemRenderMode.HorizontalBillboard;
					SetVelocity(splash, 0f, 0f, 0f, 0f, 0f);   // every scroller needs the TwoConstants axes Tick expects
					ParticleSystem.SizeOverLifetimeModule size = splash.sizeOverLifetime;
					size.enabled = true;
					size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.3f, 1f, 1f));
					SetFade(splash, 0.05f, 0.4f);
					s_scrollers.Add(new Scroller { System = splash, Factor = 1f });
					break;
				}
				case Particles.Snow:
				{
					Material snowMat = Resources.Load<Material>("Rain Particles/Materials/Snow Sprite");
					if (snowMat == null) snowMat = ParticleMaterial(game, "Snow Flake", dot, false);
					ParticleSystem snow = MakeSystem("Map Snowfall", snowMat, 900, 220f, 3f, 4f, 0.07f, 0.18f, new Color(0.95f, 0.97f, 1f, 0.9f), new Vector3(30f, 8f, 90f), new Vector3(0f, 8f, -50f));
					SetVelocity(snow, -0.5f, 0.5f, -1.8f, -1.2f, 0f);
					ParticleSystem.NoiseModule noise = snow.noise;
					noise.enabled = true;
					noise.strength = new ParticleSystem.MinMaxCurve(0.5f);
					noise.frequency = 0.35f;
					noise.scrollSpeed = new ParticleSystem.MinMaxCurve(0.2f);
					SetFade(snow, 0.1f, 0.8f);
					s_scrollers.Add(new Scroller { System = snow, Factor = 0.75f });
					break;
				}
				case Particles.Dust:
				{
					Material dustMat = ParticleMaterial(game, "Dust", dot, false);
					ParticleSystem dust = MakeSystem("Map Dust", dustMat, 300, 80f, 2f, 2.8f, 0.25f, 0.6f, new Color(0.86f, 0.7f, 0.46f, 0.3f), new Vector3(36f, 6f, 90f), new Vector3(0f, 3f, -45f));
					SetVelocity(dust, 3.5f, 7f, 0.1f, 0.6f, 0f);
					ParticleSystemRenderer dustRenderer = dust.GetComponent<ParticleSystemRenderer>();
					dustRenderer.renderMode = ParticleSystemRenderMode.Stretch;
					dustRenderer.velocityScale = 0.02f;
					dustRenderer.lengthScale = 1.5f;
					SetFade(dust, 0.15f, 0.7f);
					s_scrollers.Add(new Scroller { System = dust, Factor = 0.9f });
					ParticleSystem motes = MakeSystem("Map Dust Motes", dustMat, 160, 30f, 3f, 4f, 0.5f, 1.2f, new Color(0.9f, 0.75f, 0.5f, 0.16f), new Vector3(40f, 10f, 110f), new Vector3(0f, 5f, -55f));
					SetVelocity(motes, 1f, 3f, -0.2f, 0.4f, 0f);
					SetFade(motes, 0.25f, 0.7f);
					s_scrollers.Add(new Scroller { System = motes, Factor = 0.6f });
					break;
				}
				case Particles.Motes:
				{
					Material glowMat = ParticleMaterial(game, "Glow Mote", dot, true);
					ParticleSystem motes = MakeSystem("Map Motes", glowMat, 240, 40f, 3.5f, 5f, 0.1f, 0.3f, new Color(0.7f, 0.35f, 1f, 0.8f), new Vector3(30f, 10f, 110f), new Vector3(0f, 4f, -55f));
					SetVelocity(motes, -0.4f, 0.4f, 0.1f, 0.5f, 0f);
					SetFade(motes, 0.2f, 0.7f);
					s_scrollers.Add(new Scroller { System = motes, Factor = 0.5f });
					break;
				}
			}
			for (int i = 0; i < s_systems.Count; i++) if (s_systems[i] != null) s_systems[i].Play(true);
		}

		// ---- sky props (camera-parented, inside the fog start distance) -----------------------------------------
		private static void BuildSkyProps(ShadowRunPrototype game, Profile p)
		{
			if (s_propsRoot != null)
			{
				s_propsRoot.gameObject.SetActive(false);
				UnityEngine.Object.Destroy(s_propsRoot.gameObject);
				s_propsRoot = null;
			}
			if (game.cam == null || (!p.Moon && !p.Planet)) return;
			s_propsRoot = new GameObject("Map Sky Props").transform;
			s_propsRoot.SetParent(game.cam.transform, false);
			s_propsRoot.localPosition = Vector3.zero;
			s_propsRoot.localRotation = Quaternion.identity;
			Texture2D dot = DotTexture();
			if (p.Moon)
			{
				Material moonMat = M(game, "Moon", new Color(0.9f, 0.93f, 1f), new Color(0.9f, 0.93f, 1f) * 1.4f, 0f, 0f);
				GameObject moon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				moon.name = "Map Moon";
				game.PreparePrimitive(moon, moonMat);
				moon.transform.SetParent(s_propsRoot, false);
				moon.transform.localPosition = new Vector3(14f, 22f, 85f);
				moon.transform.localScale = Vector3.one * 5.2f;
				Material haloMat;
				if (!s_materials.TryGetValue("Moon Halo", out haloMat) || haloMat == null)
				{
					haloMat = game.MatTransparentUnlit(new Color(0.3f, 0.4f, 0.7f, 0.5f), dot, true);
					haloMat.name = "Runtime Map Moon Halo";
					s_materials["Moon Halo"] = haloMat;
				}
				GameObject halo = game.Quad("Map Moon Halo", Vector3.zero, new Vector3(16f, 16f, 1f), haloMat);
				halo.transform.SetParent(s_propsRoot, false);
				halo.transform.localPosition = new Vector3(14f, 22f, 86f);
				halo.transform.localRotation = Quaternion.identity;
			}
			if (p.Planet)
			{
				Material planetMat = M(game, "Planet", Color.white, null, 0.25f, 0f);
				planetMat.mainTexture = PlanetTexture();
				GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				planet.name = "Map Planet";
				game.PreparePrimitive(planet, planetMat);
				planet.transform.SetParent(s_propsRoot, false);
				planet.transform.localPosition = new Vector3(-16f, 28f, 95f);
				planet.transform.localRotation = Quaternion.Euler(18f, 0f, 12f);
				planet.transform.localScale = Vector3.one * 32f;
				Material ringMat = game.MatTransparentUnlit(new Color(0.85f, 0.75f, 1f, 0.75f), RingTexture(), false);
				ringMat.name = "Runtime Map Planet Ring";
				if (ringMat.HasProperty("_Cull")) ringMat.SetFloat("_Cull", 0f);
				GameObject ring = game.Quad("Map Planet Ring", Vector3.zero, new Vector3(78f, 78f, 1f), ringMat);
				ring.transform.SetParent(s_propsRoot, false);
				ring.transform.localPosition = new Vector3(-16f, 28f, 95f);
				ring.transform.localRotation = Quaternion.Euler(72f, 0f, 18f);
			}
		}

		private static Texture2D PlanetTexture()
		{
			return TextureCache("Planet", () =>
			{
				const int w = 256, h = 128;
				System.Random rng = new System.Random(0x9A11);
				Lattice swirl = new Lattice(rng, 12, 6), fine = new Lattice(rng, 40, 20);
				Color[] pixels = new Color[w * h];
				Color violet = new Color(0.5f, 0.22f, 0.66f), blue = new Color(0.22f, 0.32f, 0.62f), tan = new Color(0.72f, 0.54f, 0.4f);
				for (int y = 0; y < h; y++)
				{
					for (int x = 0; x < w; x++)
					{
						float u = (x + 0.5f) / w, v = (y + 0.5f) / h;
						float band = 0.5f + 0.5f * Mathf.Sin(v * Mathf.PI * 9f + swirl.Sample(u, v) * 3.5f);
						float detail = (fine.Sample(u, v) - 0.5f) * 0.18f;
						Color c = band < 0.5f ? Color.Lerp(violet, blue, band * 2f) : Color.Lerp(blue, tan, (band - 0.5f) * 2f);
						c += new Color(detail, detail, detail);
						c.a = 1f;
						pixels[x + y * w] = c;
					}
				}
				return MakeTexture(pixels, w, h, "Runtime Map Planet", true, false);
			});
		}

		private static Texture2D RingTexture()
		{
			return TextureCache("Ring", () =>
			{
				const int size = 128;
				Color[] pixels = new Color[size * size];
				for (int y = 0; y < size; y++)
				{
					for (int x = 0; x < size; x++)
					{
						float u = (x + 0.5f) / size * 2f - 1f, v = (y + 0.5f) / size * 2f - 1f;
						float r = Mathf.Sqrt(u * u + v * v);
						float a = Smooth(0.5f, 0.56f, r) * (1f - Smooth(0.94f, 1f, r)) * (0.55f + 0.45f * Mathf.Sin(r * 70f));
						pixels[x + y * size] = new Color(1f, 1f, 1f, a);
					}
				}
				return MakeTexture(pixels, size, size, "Runtime Map Planet Ring", false, false);
			});
		}

		private static readonly Dictionary<string, Texture2D> s_textures = new Dictionary<string, Texture2D>();

		private static Texture2D TextureCache(string key, Func<Texture2D> build)
		{
			Texture2D texture;
			if (s_textures.TryGetValue(key, out texture) && texture != null) return texture;
			texture = build();
			s_textures[key] = texture;
			return texture;
		}
	}

#if UNITY_EDITOR
	/* [user request 2026-09-24: neon night cycle] */ // Editor-only: SendMessage("DebugAdvanceNeonCycle", seconds) from a RunCommand skips ahead in the cycle.
	private void DebugAdvanceNeonCycle(float seconds) { InfernoMaps.DebugAdvanceCycle(seconds); }
#endif
}
