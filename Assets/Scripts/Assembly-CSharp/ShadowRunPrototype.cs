using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DynamicWeatherSystem;
using ShadowRun.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed partial class ShadowRunPrototype : MonoBehaviour
{
	private enum DailyGoalKind
	{
		Distance = 0,
		Runs = 1,
		Boosts = 2,
		Drift = 3,
		CarsPassed = 4,
		CasesOpened = 5
	}

	private sealed class GarageVehicleCardData
	{
		public string VehicleId;

		public string Name;

		public string Subtitle;

		public string Rarity;

		public string IconResource;

		public Color Color;

		public Color Accent;

		public float Power;

		public float Speed;

		public float Handling;

		public string PowerValue;

		public string SpeedValue;

		public string HandlingValue;

		public bool VisualLocked;
	}

	private struct Pickup
	{
		public Transform Transform;

		public float Radius;

		public PickupType Type;

		public int Lane;

		public float TargetX;

		public float Speed;

		public float TargetSpeed;

		public float CruiseSpeed;

		public float SpeedBias;

		public float LaneVelocity;

		public float Aggression;

		public float Patience;

		public float Reaction;

		public float SafeGap;

		public float LaneChangeBias;

		public float Bravery;

		public float Awareness;

		public float Courtesy;

		public float RiskTolerance;

		public float ChangeTimer;

		public float LaneChangeCooldown;

		public float IntentTimer;

		public float AiThinkTimer;

		public float CruiseVariation;

		public float CruiseVariationTimer;

		public float NaturalLaneOffset;

		public float NaturalLaneTarget;

		public float NaturalLaneTimer;

		public float CutInCooldown;

		public float PanicTimer;

		public float TailgateTimer;

		public float ContactCooldown;

		public float CollisionCooldown;

		public float LateralVelocity;

		public float Yaw;

		public float Spin;

		public float ImpactTimer;

		public bool HighBeamEvading;

		public float HighBeamManeuverTimer;

		public float HighBeamCooldown;

		public float HighBeamTargetX;

		public float HighBeamPressure;

		public float HighBeamLastFlashTime;

		public bool HighBeamRollingYield;

		public float CourtesyYieldTimer;

		public float CourtesyYieldSpeed;

		public TrafficVehicleLights Lights;

		public TrafficWheelAnimator Wheels;

		public int PendingLane;

		public int TurnSignalDirection;

		public float TurnSignalLeadTimer;

		public float BrakeLightHold;

		public bool WantsLaneChange;

		public int RampEscapeLane;

		public bool StoppingForSignal;

		public bool NearMiss;

		public TrafficDriverStyle DriverStyle;
	}

	private sealed class RampDebrisPiece
	{
		public Transform Transform;

		public Vector3 HomePosition;

		public Quaternion HomeRotation;

		public Vector3 Velocity;

		public Vector3 AngularVelocity;
	}

	private sealed class TrafficWreckPiece
	{
		public Transform Transform;

		public Renderer Renderer;

		public Vector3 HomePosition;

		public Quaternion HomeRotation;

		public Vector3 Velocity;

		public Vector3 AngularVelocity;

		public bool Paintable;
	}

	private sealed class TrafficWreck
	{
		public GameObject Root;

		public readonly List<TrafficWreckPiece> Pieces = new List<TrafficWreckPiece>(7);

		public MaterialPropertyBlock PaintBlock;

		public bool Active;

		public float Age;
	}

	private sealed class RampObstacle
	{
		public GameObject Root;

		public GameObject Model;

		public GameObject NeonSign;

		public Transform DebrisRoot;

		public readonly List<RampDebrisPiece> Debris = new List<RampDebrisPiece>(8);

		public int Lane;

		public float PreviousZ;

		public bool Active;

		public bool Triggered;

		public bool Broken;

		public float BrokenAge;
	}

	private enum PickupType
	{
		Traffic = 0
	}

	private enum TrafficDriverStyle
	{
		Cautious = 0,
		Commuter = 1,
		Slow = 2,
		Fast = 3,
		Aggressive = 4
	}

	private enum TrafficLightState
	{
		Unknown = -1,
		Green = 0,
		Yellow = 1,
		Red = 2
	}

	private struct TrafficLightSignal
	{
		public Transform Root;

		public Transform ModelRoot;

		public Renderer RedLamp;

		public Renderer YellowLamp;

		public Renderer GreenLamp;

		public float Z;

		public int Side;

		public float CycleOffset;

		public TrafficLightState State;

		public TrafficLightState AppliedState;
	}

	private struct MenuCloudLayer
	{
		public RectTransform Root;

		public float Speed;

		public float Width;
	}

	private enum CasePrizeKind
	{
		Diamonds = 0,
		Vehicle = 1
	}

	private struct CasePrize
	{
		public CasePrizeKind Kind;

		public int Diamonds;

		public string VehicleId;

		public static CasePrize Diamond(int amount)
		{
			CasePrize prize = default(CasePrize);
			prize.Kind = CasePrizeKind.Diamonds;
			prize.Diamonds = amount;
			return prize;
		}

		public static CasePrize Vehicle(string vehicleId)
		{
			CasePrize prize = default(CasePrize);
			prize.Kind = CasePrizeKind.Vehicle;
			prize.VehicleId = vehicleId;
			return prize;
		}
	}

	private sealed class HomeGoalData
	{
		public DailyGoalKind Kind;

		public string Title;

		public string Progress;

		public float Fill;

		public string IconResource;

		public Color Accent;
	}

	private struct BoxSpec
	{
		public readonly Vector3 Center;

		public readonly Vector3 Size;

		public BoxSpec(Vector3 center, Vector3 size)
		{
			Center = center;
			Size = size;
		}
	}

	private const string DailyGoalDateKeyPref = "shadowRun.daily.date";

	private const string DailyGoalDistancePref = "shadowRun.daily.distanceMeters";

	private const string DailyGoalRunsPref = "shadowRun.daily.runs";

	private const string DailyGoalBoostsPref = "shadowRun.daily.boosts";

	private const string DailyGoalDriftPref = "shadowRun.daily.driftSeconds";

	private const string DailyGoalCarsPref = "shadowRun.daily.carsPassed";

	private const string DailyGoalCasesPref = "shadowRun.daily.casesOpened";

	private const string DailyGoalClaimedMaskPref = "shadowRun.daily.claimedMask";

	private bool dailyGoalProgressLoaded;

	private int dailyGoalProgressDateKey = -1;

	private float dailyGoalDistanceMeters;

	private int dailyGoalRuns;

	private int dailyGoalBoosts;

	private float dailyGoalDriftSeconds;

	private int dailyGoalCarsPassed;

	private int dailyGoalCasesOpened;

	private int dailyGoalClaimedMask;

	private bool dailyGoalBoostActive;

	private bool dailyGoalProgressDirty;

	private const string FirstRunTutorialCompletedKey = "infernoDriveFirstRunTutorialV7";

	private const float FirstRunTutorialStageTimeout = 25f;

	private readonly HashSet<int> brakeButtonPointers = new HashSet<int>();

	private Button brakeHudButton;

	private RectTransform brakeHudButtonRect;

	private RectTransform highBeamHudButtonRect;

	private RectTransform pauseHudButtonRect;

	private GameObject firstRunTutorialRoot;

	private RectTransform firstRunTutorialRootRect;

	private RectTransform firstRunTutorialCardRect;

	private RectTransform firstRunTutorialCardGlowRect;

	private RectTransform firstRunTutorialSkipRect;

	private CanvasGroup firstRunTutorialGroup;

	private Text firstRunTutorialStepText;

	private Text firstRunTutorialTitleText;

	private Text firstRunTutorialHintText;

	private CanvasGroup firstRunTutorialCopyGroup;

	private RectTransform firstRunTutorialHighlight;

	private Image firstRunTutorialHighlightImage;

	private RectTransform firstRunTutorialTargetCore;

	private Image firstRunTutorialHandImage;

	private RectTransform firstRunTutorialHandRect;

	private Sprite firstRunTutorialHandSprite;

	private Vector2 firstRunTutorialHandBasePosition;

	private Vector2 firstRunTutorialHandVelocity;

	private float firstRunTutorialHandBaseAngle;

	private float firstRunTutorialHandAngleVelocity;

	private float firstRunTutorialHandSize;

	private float firstRunTutorialHandSizeVelocity;

	private float firstRunTutorialHandTargetSize;

	private float firstRunTutorialHandAlpha;

	private readonly Image[] firstRunTutorialProgressDots = new Image[5];

	private bool firstRunTutorialActive;

	private int firstRunTutorialStage;

	private float firstRunTutorialStageTime;

	private float firstRunTutorialVisibleTime;

	private float firstRunTutorialInputTime;

	private float firstRunTutorialSecondaryInputTime;

	private bool firstRunTutorialAccelerationHoldLearned;

	private bool firstRunTutorialMouseDragTracking;

	private float firstRunTutorialMouseDragStartX;

	private float firstRunTutorialInputLockUntil;

	private float firstRunTutorialTapIgnoreUntil;

	private float firstRunTutorialLastTapTime = -10f;

	private int firstRunTutorialLastTapFrame = -1;

	private bool firstRunTutorialRussian;

	private const float GaragePreviewHorizontalOffset = 0.25f;

	private const float GaragePreviewAutoRotateDelay = 0.5f;

	private const int GarageVehicleCrystalPrice = 100;

	private Sprite garageLockSprite;

	private RectTransform garagePurchaseActionRoot;

	private bool brakeHapticHeld;

	private float nextCollisionHapticTime;

	private const float CaseRouletteCardWidth = 250f;

	private const float CaseRouletteCardHeight = 310f;

	private const float CaseRouletteSpacing = 6f;

	private const float CaseRouletteSlotWidth = 256f;

	private const float CaseRouletteViewportWidth = 920f;

	private const float CaseRouletteViewportHeight = 350f;

	private const float CaseRouletteImageHeight = 241.79999f;

	private const float CaseRouletteRarityStripHeight = 80.6f;

	private const float HomeCardsHeight = 620f;

	private const float HomeCardsMinHeight = 620f;

	private const float HomeContentVerticalOffset = -280f;

	private const float HomeHeroOverlapOffset = 39f;

	private const float CasePreviewYawLimit = 60f;

	private const float CasePreviewModelDownOffset = 0.258f;

	private const float CaseVehiclePrizeChance = 0.06f;

	private const string CaseDiamondResource = "алмаз 1";

	private const string SportCarEngineAudioResource = "Car Engine Sound - i6 German Free/Assets/Audio/i6_german_free/";

	private const float VehicleEngineVolumeScale = 0.85f;

	private const float VehicleEngineAccelerationVolumeScale = 0.85f;

	private VehicleDefinition activeVehicleDefinition;

	private VehicleDefinition garagePreviewVehicleDefinition;

	private HoverAnimation playerHoverAnimation;

	private const float BackgroundMusicGapSeconds = 2f;

	private const float WeatherTransitionDuration = 4.25f;

	private const float NightToDayTransitionDuration = 4f;

	private const float FallenSkyTransitionDuration = 7.5f;

	private const float PlanetsTransitionDuration = 6.5f;

	private const float WeatherPhaseDuration = 35f;

	private const float LightningInterval = 3f;

	private const float LightningFlashDuration = 0.24f;

	private const string RainAudioResource = "WeatherAudio/low_rain_loop";

	private const string FallenSkyFallingStarsResource = "JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Magic Misc/CFXR4 Falling Stars";

	private const string FastSkyMaterialPath = "FastSky/Materials/RealisticSky";

	private const string Sky360TextureResource = "sky_360_hd";

	private const string Cloud360TextureResource = "clouds_panorama_8k";

	private static readonly bool UseSeparateCloud360Layer = false;

	private const string PulsarNightSkyboxResource = "Pulsar Neutron Star/Materials/SkyBox (NebulaPurple)";

	private const string PulsarNightBluePrefabResource = "Pulsar Neutron Star/Prefabs/StarCore Blue";

	private const string PulsarNightRedPrefabResource = "Pulsar Neutron Star/Prefabs/StarCore Red";

	private const int PulsarNightFxLayer = 27;

	private const float PulsarNightBlueDepth = 26f;

	private const float PulsarNightRedDepth = 27f;

	private const float PulsarNightBlueScale = 1.6848f;

	private const float PulsarNightRedScale = 0.656f;

	private const float PulsarNightPrefabSpinMultiplier = 0.25f;

	private const float Sky360RotationSpeed = 0.18f;

	private const float Sky360EmissionBoost = 1.85f;

	private const float Sky360RainDesaturation = 0.72f;

	private static readonly bool UseFastSkyRuntimeSkybox = true;

	private static readonly Vector2 Sky360TextureUvOffset = Vector2.zero;

	private const float Sky360ReferenceVerticalFov = 54.5f;

	private static readonly Vector2 Cloud360TextureTiling = Vector2.one;

	private static readonly Vector3 Sky360CylinderWorldPosition = new Vector3(-0.1f, 9.8f, -47.7f);

	private static readonly Vector3 Sky360CylinderScale = new Vector3(96f, 150.796448f, 96f);

	private static readonly Vector3 Cloud360CylinderScale = new Vector3(94f, 12.744f, 94f);

	private const float Sky360CylinderBaseYaw = 177.74f;

	private const float Sky360CylinderRoll = 0f;

	private static readonly Vector3 RainStormSunLocalPosition = new Vector3(8.79f, 61.75f, 153.8f);

	private const float RainStormSunReferenceFov = 65f;

	private static readonly Vector3 RainStormSunHaloScale = new Vector3(17.4f, 17.4f, 1f);

	private static readonly Vector3 RainStormSunGlowScale = new Vector3(10.8f, 10.8f, 1f);

	private static readonly Vector3 RainStormSunCoreScale = new Vector3(7.91f, 7.91f, 1f);

	private WeatherManager weatherManager;

	private AudioModule weatherAudioModule;

	private bool weatherAudioMuteInitialized;

	private bool weatherAudioMuted;

	private WeatherStateData weatherDayPreset;

	private WeatherStateData weatherNightPreset;

	private WeatherStateData weatherRainPreset;

	private WeatherStateData weatherStormPreset;

	private WeatherStateData weatherFallenSkyPreset;

	private WeatherStateData weatherPlanetsPreset;

	private WeatherStateData[] weatherPresetCycle;

	private int weatherPresetIndex;

	private int previousWeatherPresetIndex = -1;

	private float weatherAutoTimer;

	private bool dynamicWeatherReady;

	private Material runtimeFastSkySkybox;

	private Material runtimeFallbackSkybox;

	private Material runtimeSky360Skybox;

	private Material runtimePulsarNightSkybox;

	private bool fastSkyRuntimeUnavailable;

	private bool sky360SkyboxUnavailable;

	private WeatherStateData visualTransitionFromState;

	private WeatherStateData visualTransitionToState;

	private Transform pulsarNightRoot;

	private Camera pulsarNightCamera;

	private GameObject pulsarNightBlueInstance;

	private GameObject pulsarNightRedInstance;

	private Renderer[] pulsarNightBlueRenderers;

	private Renderer[] pulsarNightRedRenderers;

	private Material[][] pulsarNightBlueMaterials;

	private Material[][] pulsarNightRedMaterials;

	private float pulsarNightVisibility;

	private Transform rainStormSunRoot;

	private Renderer rainStormSunHaloRenderer;

	private Renderer rainStormSunGlowRenderer;

	private Renderer rainStormSunCoreRenderer;

	private Material rainStormSunHaloMat;

	private Material rainStormSunGlowMat;

	private Material rainStormSunCoreMat;

	private float rainStormSunVisibility;

	private float sky360Desaturation;

	private float sky360TransitionFromDesaturation;

	private float sky360TransitionToDesaturation;

	private float sky360AppliedDesaturation = -1f;

	private GameObject fallenSkyFallingStars;

	private bool fallenSkyFallingStarsActive;

	private readonly Dictionary<string, string> fastSkyPropertyCache = new Dictionary<string, string>(32);

	private readonly float[] weatherSelectionWeights = new float[6];

	private ReflectionProbe roadReflectionProbe;

	private const float RoadWidth = 10.2f;

	private const float SegmentLength = 18f;

	private const int SegmentCount = 42;

	private const float CityLoop = 528f;

	private const int CityBuildingRowCount = 17;

	private const float CityBuildingSpacing = 31.058823f;

	private const float CityBuildingYOffset = 0f;

	private const float CityBuildingScale = 1f;

	private const float StreetLampSpacing = 30f;

	private const float StreetLampX = 5.72f;

	private const float StreetLampHeight = 6.2f;

	private const float RightStreetLampModelX = 1.69f;

	private const float LeftStreetLampModelX = 1.69f;

	private const float LeftStreetLampModelY = 3.135f;

	private const float Buidling1ScaleMultiplier = 2f;

	private const float Buidling1RoadInsetFix = 1.2f;

	private const float Tower4ScaleMultiplier = 1.6f;

	private const float Tower4RoadInsetFix = 2.2f;

	private const float NeonFenceSegmentLength = 15f;

	private const float NeonFenceX = 6.08f;

	private const int TrafficLightCount = 0;

	private const float TrafficLightMinSpacing = 1850f;

	private const float TrafficLightMaxSpacing = 2450f;

	private const float TrafficStopLineOffset = 7.5f;

	private const float TrafficHorizonRespawnZ = -320f;

	private const float TrafficBehindRespawnMinZ = 7.5f;

	private const float TrafficBehindRespawnMaxZ = 10.5f;

	private const float TrafficPlayerLaneReserveMinGap = -8f;

	private const float TrafficPlayerLaneReserveMaxGap = 96f;

	private const float SpeedometerScale = 0.64f;

	private const int GaragePreviewLayer = 29;

	private const int TrafficPoolSize = 12;

	private const int TargetFrameRate = 60;

	private const int MenuTargetFrameRate = 30;

	private const float MaxGameplayDeltaTime = 1f / 30f;

	private const float PlayerMinX = -3.85f;

	private const float PlayerMaxX = 3.85f;

	private const float PlayerHalfWidth = 0.92f;

	private const float TrafficHalfWidth = 0.94f;

	private const float VehicleHalfLength = 1.82f;

	private const float ContactHalfLength = 1.22f;

	private const float VehicleModelYawOffset = 180f;

	private const int HighBeamTrafficLayer = 28;

	private const float HighBeamMaxRange = 20f;

	private const float HighBeamFlashDuration = 0.12f;

	private const float HighBeamRetriggerDarkGap = 0.04f;

	private const float HighBeamActiveWindowDuration = 5f;

	private const float HighBeamCooldownDuration = 10f;

	private const float HighBeamOriginHeight = 0.3f;

	private const float HighBeamFrontInset = 0.08f;

	private const float HighBeamEmitterWidth = 0.5f;

	private const float HighBeamEmitterHeightOffset = -0.04f;

	private const float HighBeamVisualHeightOffset = 0.08f;

	private const float HighBeamLeftShaftLocalZ = -0.494f;

	private const float HighBeamRightShaftLocalZ = -0.447f;

	private const string HighBeamPrefabResource = "FakeVolumetricLight-Unity-main/VolumatricSpotlight/Prefabs/VolumetricSpotLight_Cone";

	private const string SportCarReferenceResource = "sportcar";

	private const string FlyingCarReferenceResource = "flying_car";

	private const string InfalModelReferenceResource = "IFNAL_MODEL_P900";

	private const string Car3ReferenceResource = "car3";

	private const string SelectedVehiclePreference = "shadowRunSelectedVehicle";

	private const string StreetLampResource = "Meshy_AI_Lone_Street_Lamp_0617180213_texture";

	private const string TrafficVehicleResource = "uploads_files_4881612_uploads_files_3750868_MB.W222.AMG";

	private const string RearCollisionExplosionResource = "JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Explosions/CFXR Explosion 1";

	private const string SideCollisionExplosionResource = "JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Explosions/CFXR3 Fire Explosion B";

	private const string TrafficWowResource = "JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Texts/CFXR3 _WOW_";

	private const string TrafficPowResource = "JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Texts/CFXR _POW_";

	private const string RampResource = "рампа";

	private const string RampUnlitShaderResource = "Shaders/RampUnlit";

	private const int RampPoolSize = 2;

	private const float RampWidth = 2.68f;

	private const float RampLength = 4.75f;

	private const float RampHeight = 1.12f;

	private const string ExhaustFireResource = "JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Fire/CFXR Fire Breath";

	private static readonly int BaseMapProperty = Shader.PropertyToID("_BaseMap");

	private static readonly int MainTexProperty = Shader.PropertyToID("_MainTex");

	private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");

	private static readonly int ColorProperty = Shader.PropertyToID("_Color");

	private static readonly int EmissionMapProperty = Shader.PropertyToID("_EmissionMap");

	private static readonly int EmissionColorProperty = Shader.PropertyToID("_EmissionColor");

	private static readonly int BumpMapProperty = Shader.PropertyToID("_BumpMap");

	private static readonly int BumpScaleProperty = Shader.PropertyToID("_BumpScale");

	private static readonly int OcclusionMapProperty = Shader.PropertyToID("_OcclusionMap");

	private static readonly int OcclusionStrengthProperty = Shader.PropertyToID("_OcclusionStrength");

	private static readonly Vector2 RoadTextureTiling = new Vector2(1.35f, 170f);

	private const float RoadTextureLength = 1900f;

	private static readonly string[] CityBuildingResources = new string[16] { "здание 1", "здание 2", "здание 3", "здание 4", "здание 5", "здание 6", "здание 7", "здание 8", "здание 9", "здание 10", "здание 11", "здание 12", "здание 13", "здание 14", "здание 15", "здание 16" };

	private static readonly float[] CityBuildingHeightPattern = new float[8] { 64.4f, 75.6f, 58.8f, 81.2f, 68.6f, 61.6f, 78.4f, 71.4f };

	private const float LowQualityRenderScale = 0.72f;

	private const float HighQualityRenderScale = 0.86f;

	private const float UltraQualityRenderScale = 1f;

	private const float LowQualityCameraFar = 900f;

	private const float HighQualityCameraFar = 900f;

	private const float UltraQualityCameraFar = 900f;

	private const float MenuTabTweenDuration = 0.13f;

	private readonly List<Transform> cityObjects = new List<Transform>();

	private readonly Dictionary<Material, Material> cityUnlitMaterials = new Dictionary<Material, Material>();

	private readonly List<TrafficLightSignal> trafficLights = new List<TrafficLightSignal>();

	private readonly List<Pickup> pickups = new List<Pickup>();

	private readonly List<GameObject> trafficPool = new List<GameObject>();

	private readonly List<TrafficWreck> trafficWreckPool = new List<TrafficWreck>(2);

	private Material roadMat;

	private Material cityGroundMat;

	private Material lineMat;

	private Material blackMat;

	private Material carMat;

	private Material glassMat;

	private Material tireMat;

	private Material rimMat;

	private Material yellowWindowMat;

	private Material darkWindowMat;

	private Material redBuildingMat;

	private Material purpleBuildingMat;

	private Material facadeMat;

	private Material cloudMat;

	private Material obstacleMat;

	private Material coinMat;

	private Material pickupMat;

	private Material particleBlueMat;

	private Material particleRedMat;

	private Material particleVioletMat;

	private Material smokeParticleMat;

	private Material speedLineMat;

	private Material greenLightMat;

	private Material redLightOffMat;

	private Material yellowLightOffMat;

	private Material greenLightOffMat;

	private Material neonFenceBodyMat;

	private Material neonFenceMat;

	private Material streetLampRedEmissiveMat;

	private bool roadMatHasBaseMap;

	private bool roadMatHasMainTex;

	private bool roadMatHasBumpMap;

	private bool roadMatHasOcclusionMap;

	private Texture2D roadTexture;

	private Texture2D roadNormalTexture;

	private Texture2D roadAoTexture;

	private Transform player;

	private Transform carVisual;

	private readonly Transform[] playerWheelMeshes = new Transform[2];

	private readonly Quaternion[] playerWheelMeshBaseRotations = new Quaternion[2];

	private float playerWheelSpin;

	private float playerWheelSpinDegreesPerMeter = 120f;

	private readonly List<Renderer> playerBrakeLightRenderers = new List<Renderer>(4);

	private readonly List<int> playerBrakeLightMaterialIndices = new List<int>(4);

	private MaterialPropertyBlock playerBrakeLightBlock;

	private Material p900BrakeLightMaterial;

	private bool playerBrakeLightsOn;

	private Transform highBeamOrigin;

	private Transform highBeamLeftEmitter;

	private Transform highBeamRightEmitter;

	private Transform vehicleSmokeRoot;

	private ParticleSystem vehicleSmoke;

	private ParticleSystemRenderer vehicleSmokeRenderer;

	private Transform playerExhaustFireRoot;

	private Transform cityRoot;

	private Camera cam;

	private Light sceneKeyLight;

	private GameObject highBeamPrefab;

	private GameObject highBeamLeftPrefabInstance;

	private GameObject highBeamRightPrefabInstance;

	private Text scoreText;

	private Text bestText;

	private Text hintText;

	private Text speedometerValueText;

	private Text overlayTitle;

	private Text overlayTag;

	private Text overlayStats;

	private Text primaryButtonLabel;

	private Text dailyGoalsTimerText;

	private readonly Text[] dailyGoalTitleTexts = new Text[3];

	private readonly Text[] dailyGoalProgressTexts = new Text[3];

	private readonly Image[] dailyGoalCollectImages = new Image[3];

	private readonly Image[] dailyGoalClaimedImages = new Image[3];

	private int dailyGoalsDateKey = -1;

	private float nextDailyGoalsTimerUpdate;

	private GameObject overlay;

	private GameObject hudRoot;

	private GameObject menuDetails;

	private GameObject settingsRoot;

	private GameObject garageRoot;

	private RectTransform overlayRect;

	private Button primaryOverlayButton;

	private Button secondaryOverlayButton;

	private RectTransform gameOverRoot;

	private Text gameOverDistanceText;

	private Text gameOverBestText;

	private Button gameOverRetryButton;

	private Button gameOverMenuButton;

	private GameObject pauseActionsRoot;

	private Button pauseContinueButton;

	private Button pauseMenuButton;

	private Button pauseSoundButton;

	private Image pauseSoundIcon;

	private Image speedFill;

	private Image rushFill;

	private RectTransform speedometerNeedle;

	private RectTransform speedometerNeedleGlow;

	private Sprite speedometerNeedleSprite;

	private Sprite softCircleSprite;

	private Sprite caseRadialGlowSprite;

	private Sprite roundedGlassSprite;

	private Sprite roundedGlowSprite;

	private Sprite casePrimaryButtonSprite;

	private Sprite caseSecondaryButtonSprite;

	private Sprite gemCounterSprite;

	private RectTransform gemCounterRect;

	private Sprite purpleProgressSprite;

	private Sprite heroBottomFadeSprite;

	private Sprite menuTabFrameSprite;

	private Sprite menuTabActiveSprite;

	private Sprite garageVehicleCardFillSprite;

	private Sprite caseRarityStripSprite;

	private Sprite caseRouletteRailSprite;

	private Sprite caseRouletteLeftVignetteSprite;

	private Sprite caseRouletteRightVignetteSprite;

	private Sprite caseRouletteScanlineSprite;

	private Sprite caseTriangleDownSprite;

	private Sprite caseTriangleUpSprite;

	private Font menuUiFont;

	private Material menuTabIconTintMaterial;

	private RectTransform menuSceneRoot;

	private RectTransform menuSafeRoot;

	private RectTransform menuTabsRoot;

	private RectTransform menuMainTabRoot;

	private RectTransform menuTabBarRoot;

	private RectTransform menuTabActiveRoot;

	private RectTransform menuBackgroundRect;

	private RectTransform menuTitleRoot;

	private RectTransform menuButtonRoot;

	private Text garageTabLabel;

	private Text homeTabLabel;

	private Text profileTabLabel;

	private Text garageUnlockedCountText;

	private Image garageTabIcon;

	private Image homeTabIcon;

	private Image profileTabIcon;

	private CanvasGroup garageTabGlow;

	private CanvasGroup homeTabGlow;

	private CanvasGroup profileTabGlow;

	private RectTransform homeContentRoot;

	private RectTransform homePageHeaderRoot;

	private RectTransform garagePageHeaderRoot;

	private RectTransform profilePageHeaderRoot;

	private RectTransform profileContentRoot;

	private GameObject profileGuestRoot;

	private GameObject profileAccountRoot;

	private GameObject profileAuthFormRoot;

	private Text profileIdentityText;

	private Text profileAccountTypeText;

	private Text profileSyncText;

	private Text profileEmailText;

	private Text profileStatusText;

	private Text profileAuthTitleText;

	private Text profileAuthSubmitText;

	private Text profileDeleteButtonText;

	private InputField profileEmailInput;

	private InputField profilePasswordInput;

	private Button profileSignUpButton;

	private Button profileLoginButton;

	private Button profileAuthSubmitButton;

	private Button profileSignOutButton;

	private Button profileDeleteButton;

	private Button profileSoundButton;

	private Image profileSoundIcon;

	private bool profileAuthRegisterMode;

	private bool profileAuthBusy;

	private RectTransform homeHeroRoot;

	private RectTransform homeHeroImageRect;

	private RectTransform homeHeroArtRect;

	private RectTransform homeDriveArtRect;

	private RectTransform homeDriveHitRect;

	private RectTransform homeDriveShineRect;

	private Image homeDriveShineImage;

	private RectTransform homeCaseArtRect;

	private RectTransform homeGoalsArtRect;

	private GameObject gemShopRoot;

	private readonly Dictionary<int, Text> gemShopPriceTexts = new Dictionary<int, Text>();

	private GameObject caseOverlayRoot;

	private RectTransform caseOpeningWindowRoot;

	private RectTransform caseOpeningPreviewRoot;

	private RectTransform caseClosedRoot;

	private GameObject caseClosedStaticVisualRoot;

	private RectTransform caseRouletteRoot;

	private RectTransform caseRouletteContent;

	private RectTransform caseResultRoot;

	private RectTransform caseBottomAreaRoot;

	private RectTransform caseCostRowRoot;

	private RectTransform caseOpeningStatusRoot;

	private CanvasGroup caseClosedGroup;

	private CanvasGroup caseRouletteGroup;

	private CanvasGroup caseResultGroup;

	private Image[] caseOpeningDots;

	private Image caseRouletteMarkerGlow;

	private Image caseRouletteMarkerCore;

	private Image caseRoulettePulse;

	private Image caseRouletteSweep;

	private RawImage casePreviewImage;

	private RenderTexture casePreviewTexture;

	private Camera casePreviewCamera;

	private Transform casePreviewRoot;

	private Transform caseTurntableRoot;

	private Transform caseModelRoot;

	private Transform caseLidPivot;

	private Transform caseHeadRoot;

	private Vector3 caseModelBaseLocalPosition;

	private Button caseOpenButton;

	private GameObject caseOpenButtonGlow;

	private Button caseCollectButton;

	private Button caseSpinAgainButton;

	private Text caseOpenButtonLabel;

	private Text caseStatusLabel;

	private Image caseResultPrizeImage;

	private Text caseResultRarityText;

	private Text caseResultTitleText;

	private Text caseResultSubtitleText;

	private Text caseResultBonusText;

	private bool casePreviewReady;

	private bool caseOpenAnimationActive;

	private bool caseSpinActive;

	private CasePrize caseSelectedPrize;

	private bool caseRewardApplied;

	private bool caseRewardReady;

	private int caseOpeningRequestSerial;

	private bool caseHomeCardRequiresFreshPointerDown;

	private bool caseHasLidAnimation;

	private float casePreviewYaw;

	private float caseOpenAnimationElapsed;

	private float caseOpenAnimationDuration = 4f;

	private float caseSpinElapsed;

	private float caseSpinDuration;

	private float caseSpinStartX;

	private float caseSpinTargetX;

	private float caseSpinTickPulse;

	private int caseSpinLastTickIndex = -1;

	private AudioSource caseSequenceAudioSource;

	private AudioClip caseOpeningAudioClip;

	private AudioClip caseRouletteAudioClip;

	private bool caseOpeningAudioStarted;

	private Text gemAmountText;

	private int menuGemCount = 25;

	private float menuBackgroundAspect = 0.5625f;

	private float menuTabStartX;

	private float menuTabEndX;

	private float menuTabTweenElapsed = 0.13f;

	private int menuTargetTab = 1;

	private int menuActiveTab = 1;

	private Vector2 lastSafePos = new Vector2(-1f, -1f);

	private Vector2 lastSafeSize = new Vector2(-1f, -1f);

	private readonly List<MenuCloudLayer> menuCloudLayers = new List<MenuCloudLayer>();

	private readonly List<RectTransform> menuParallax = new List<RectTransform>();

	private readonly List<Image> lifeDots = new List<Image>();

	private Material lostLifeIconMaterial;

	private readonly List<Transform> skyClouds = new List<Transform>();

	private Transform skyBackdrop;

	private Texture skyBackdropTexture;

	private Transform sky360Cylinder;

	private Material sky360Material;

	private Renderer sky360Renderer;

	private Texture2D sky360Texture;

	private Transform cloud360Cylinder;

	private Material cloud360Material;

	private Renderer cloud360Renderer;

	private Texture2D cloud360Texture;

	private bool separateCloudCleanupComplete;

	private float sky360Visibility;

	private float sky360TextureOffset;

	private Transform citySilhouetteBackdrop;

	private Material citySilhouetteMaterial;

	private Texture2D citySilhouetteTexture;

	private Transform lightningFlash;

	private Material lightningFlashMaterial;

	private float lightningTimer;

	private float lightningFlashTime;

	private float lastSkyBackdropAspect = -1f;

	private float lastSkyBackdropFov = -1f;

	private float lastSkyBackdropDistance = -1f;

	private Text musicValueText;

	private Text sfxValueText;

	private Text graphicsValueText;

	private Text frameRateValueText;

	private Text languageValueText;

	private Text vibrationValueText;

	private Text darkModeValueText;

	private Text tutorialValueText;

	private Image musicSliderFill;

	private Image sfxSliderFill;

	private Image vibrationDot;

	private Image darkModeDot;

	private Image tutorialDot;

	private RawImage garageCarPreview;

	private Camera garagePreviewCamera;

	private RenderTexture garagePreviewTexture;

	private Transform garagePreviewRoot;

	private Transform garageSceneRoot;

	private Transform garageVirtualFloor;

	private Transform garagePreviewPivot;

	private Transform garagePreviewCarHolder;

	private GameObject garagePreviewVehicleRoot;

	private float garagePreviewYaw = 130f;

	private bool garagePreviewDragging;

	private float garagePreviewResumeDelay;

	private GarageVehicleCardData[] garageVehicleCards;

	private Button[] garageVehicleCardButtons;

	private Image[] garageVehicleCardBorders;

	private Image[] garageVehicleCardFills;

	private Image[] garageVehicleCardDots;

	private Image[] garageVehicleLockOverlays;

	private Text[] garageVehicleActionLabels;

	private Button garagePurchaseButton;

	private Text garagePurchaseButtonText;

	private Text garagePurchaseStatusText;

	private bool garagePurchaseBusy;

	private bool gemPurchaseBusy;

	private ScrollRect garageVehicleScroll;

	private RectTransform garageVehicleCardsContent;

	private int garageSelectedVehicleCardIndex;

	private bool garageVehicleCardPointerMoved;

	private bool garageVehicleCarouselDragging;

	private float garageVehicleCarouselDragStartX;

	private float garageVehicleCarouselPointerStartX;

	private float garageVehicleCarouselTargetX;

	private float garageVehicleCarouselVelocityX;

	private float garageVehicleCarouselDragVelocityX;

	private string garagePendingVehicleId;

	private float garagePendingVehicleApplyDelay;

	private float garageVehicleCardWidth = 1f;

	private float garageVehicleCardStep = 1f;

	private float garageVehicleSidePadding;

	private float garageVehicleMaxScroll;

	private Button garageSportCarButton;

	private Button garageFlyingCarButton;

	private Text garageSportCarLabel;

	private Text garageFlyingCarLabel;

	private float playerX;

	private float velocityX;

	private bool playerAirborne;

	private bool playerLandedThisFrame;

	private float playerLandingDownSpeed;

	private float playerJumpHeight;

	private float playerJumpVelocity;

	private float playerJumpPitch;

	private float playerJumpRoll;

	private float playerJumpRollVelocity;

	private float playerJumpYaw;

	private bool playerRampRiding;

	private float playerRampRideElapsed;

	private float playerRampRideDuration;

	private float playerRampRiseHeight;

	private float playerRampTakeoffVelocity;

	private float playerJumpPitchVelocity;

	private float speed = 18f;

	private int lastSpeedometerKmh = -1;

	private float lastSpeedFillAmount = -1f;

	private float lastRushFillAmount = -1f;

	private float lastSpeedometerNeedleAngle = 999f;

	private float brakePressure;

	private bool brakeRecoveryActive;

	private float trafficIncidentCooldown;

	private float roadTextureOffset;

	private float steerInput;

	private float drift;

	private float yawVisual;

	private float rollVisual;

	private float impactYaw;

	private float impactRoll;

	private float impactShake;

	private float tractionLoss;

	private int lifeHalfUnits = 6;

	private float invuln;

	private float rush = 42f;

	private float distance;

	private float score;

	private int combo;

	private float comboTimer;

	private int best;

	private int musicVolume = 70;

	private int sfxVolume = 60;

	private bool gameSoundEnabled = true;

	private bool externalAudioPlaying;

	private float nextExternalAudioCheck;

	private float nextHudTextRefresh;

	private int graphicsIndex = 1;

	private int frameRateIndex = 1;

	private int languageIndex;

	private bool vibrationOn = true;

	private bool darkModeOn = true;

	private bool tutorialHints;

	private bool brakeButtonHeld;

	private bool touchBoostHeld;

	private readonly HashSet<int> touchSteerLeftPointers = new HashSet<int>();

	private readonly HashSet<int> touchSteerRightPointers = new HashSet<int>();

	private readonly HashSet<int> touchBoostPointers = new HashSet<int>();

	private LayerMask highBeamTrafficMask;

	private float highBeamFlashRemaining;

	private float highBeamRetriggerRemaining;

	private float highBeamActiveWindowRemaining;

	private float highBeamCooldownRemaining;

	private int highBeamQueuedPulses;

	private bool highBeamFlashReactionSent;

	private bool highBeamCourtesyNext = true;

	private Button highBeamHudButton;

	private Image highBeamCooldownFill;

	private AudioSource highBeamAudioSource;

	private AudioClip highBeamAudioClip;

	private float driftChainTimer;

	private float smokeEmissionRate;

	private float smokeLastSpeed;

	private float spawnTimer = 1.2f;

	private int lastTrafficPattern = -1;

	private float playerStoppedTrafficTimer;

	private readonly List<RampObstacle> rampPool = new List<RampObstacle>(2);

	private Material rampUnlitMat;

	private Material rampNeonYellowMat;

	private float nextRampDistance;

	private int rampSpawnCount;

	private bool rampMissingLogged;

	private float eventTimer = 1.2f;

	private bool running;

	private bool paused;

	private int appliedTargetFrameRate = -1;

	private bool showingGameOver;

	private bool santaActive;

	private bool demoSanta = true;

	private Transform santa;

	private LineRenderer santaWhip;

	private Keyboard cachedKeyboard;

	private float lastCameraUpdateTime;

	private Vector3 lastPlayerPosition;

	private Quaternion lastPlayerRotation;

	private Shader cachedShader;

	private readonly List<Transform> santaAnimParts = new List<Transform>();

	private readonly float[] lanes = new float[3] { -3.1f, 0f, 3.1f };

	private readonly List<GameObject> trafficPrefabs = new List<GameObject>();

	private readonly List<GameObject> buildingPrefabs = new List<GameObject>();

	private GameObject trafficLightPrefab;

	private GameObject rearCollisionExplosionPrefab;

	private GameObject sideCollisionExplosionPrefab;

	private GameObject trafficWowPrefab;

	private float nextTrafficWowTime;

	private GameObject trafficPowPrefab;

	private GameObject playerExhaustFirePrefab;

	private AudioSource vehicleEngineStartupSource;

	private AudioSource vehicleEngineIdleSource;

	private AudioSource vehicleEngineOnSource;

	private AudioSource vehicleEngineOffSource;

	private bool vehicleEngineLoopsStarted;

	private float vehicleEngineLastSpeed;

	private float vehicleEngineImpactOffRemaining;

	private AudioSource backgroundMusicSource;

	private AudioClip[] backgroundMusicClips;

	private int backgroundMusicTrackIndex;

	private float backgroundMusicGapRemaining;

	private bool backgroundMusicTrackActive;

	private bool backgroundMusicPaused;

	private bool caseMusicSuppressed;

	private bool rearCollisionExplosionMissingLogged;

	private bool sideCollisionExplosionMissingLogged;

	private bool playerExhaustFireMissingLogged;

	private bool playerExhaustFireActive;

	private Vector3 cameraVelocity;

	private Vector3 cameraLookTarget = new Vector3(0f, 0.58f, -30f);

	private Vector3 cameraLookVelocity;

	private float cameraFovVelocity;

	private int lastHudScore = -1;

	private int lastHudBest = -1;

	private int lastHudLifeHalfUnits = -1;

	private int lastOverlayScore = -1;

	private int lastOverlayBest = -1;

	private int lastOverlayCombo = -1;

	private float playerVehicleYawOffset = 180f;

	private float cachedShakeX;

	private float cachedShakeY;

	private float nextShakeSampleTime;

	private float FirstRunTutorialGameplayScale => 0f;


}
