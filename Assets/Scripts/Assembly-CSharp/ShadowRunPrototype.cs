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

	private sealed class _003C_003Ec__DisplayClass250_0
	{
		public ShadowRunPrototype _003C_003E4__this;

		public int cardIndex;

		internal void _003CAddGarageVehicleCardDragEvents_003Eb__0(BaseEventData data)
		{
		}

		internal void _003CAddGarageVehicleCardDragEvents_003Eb__1(BaseEventData data)
		{
		}

		internal void _003CAddGarageVehicleCardDragEvents_003Eb__2(BaseEventData data)
		{
		}

		internal void _003CAddGarageVehicleCardDragEvents_003Eb__3(BaseEventData data)
		{
		}

		internal void _003CAddGarageVehicleCardDragEvents_003Eb__4(BaseEventData data)
		{
		}

		internal void _003CAddGarageVehicleCardDragEvents_003Eb__5(BaseEventData _)
		{
		}
	}

	private sealed class _003C_003Ec__DisplayClass340_0
	{
		public HashSet<int> activePointers;

		public Action changed;

		internal void _003CMakeGameplayTouchZone_003Eb__0(BaseEventData data)
		{
		}

		internal void _003CMakeGameplayTouchZone_003Eg__ReleasePointer_007C1(BaseEventData data)
		{
		}
	}

	private sealed class _003C_003Ec__DisplayClass367_0
	{
		public ShadowRunPrototype _003C_003E4__this;

		public int amount;

		internal void _003CMakeGemPackCard_003Eb__0()
		{
		}

		internal void _003CMakeGemPackCard_003Eb__1()
		{
		}
	}

	private sealed class _003C_003Ec__DisplayClass391_0
	{
		public ShadowRunPrototype _003C_003E4__this;

		public int requestSerial;

		public bool openingAgain;

		internal void _003CRequestCaseRewardedOpening_003Eb__0()
		{
		}

		internal void _003CRequestCaseRewardedOpening_003Eb__1(string message)
		{
		}
	}

	private sealed class _003C_003Ec__DisplayClass436_0
	{
		public ShadowRunPrototype _003C_003E4__this;

		public int index;

		public Button collectButton;

		internal void _003CMakeGoalRow_003Eb__0()
		{
		}
	}

	private sealed class _003C_003Ec__DisplayClass445_0
	{
		public ShadowRunPrototype _003C_003E4__this;

		public int tabIndex;

		internal void _003CMakeMenuTabButton_003Eb__0()
		{
		}
	}

	[StructLayout((LayoutKind)3)]
	private struct _003CAddGemsFromShop_003Ed__371 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public ShadowRunPrototype _003C_003E4__this;

		public int amount;

		private int _003CbalanceBeforePurchase_003E5__2;

		private TaskAwaiter<bool> _003C_003Eu__1;

		private RevenueCatPurchaseService.PurchaseOutcome _003Cresult_003E5__3;

		private int _003CexpectedBalance_003E5__4;

		private TaskAwaiter<RevenueCatPurchaseService.PurchaseOutcome> _003C_003Eu__2;

		private int _003Cattempt_003E5__5;

		private TaskAwaiter _003C_003Eu__3;

		private void MoveNext()
		{
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	[StructLayout((LayoutKind)3)]
	private struct _003CPurchaseSelectedGarageVehicleAsync_003Ed__238 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ShadowRunPrototype _003C_003E4__this;

		private string _003CvehicleId_003E5__2;

		private TaskAwaiter<bool> _003C_003Eu__1;

		private void MoveNext()
		{
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	[StructLayout((LayoutKind)3)]
	private struct _003CRunProfileAuthAsync_003Ed__488 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ShadowRunPrototype _003C_003E4__this;

		private TaskAwaiter<bool> _003C_003Eu__1;

		private void MoveNext()
		{
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	[StructLayout((LayoutKind)3)]
	private struct _003CRunProfileDeleteAsync_003Ed__495 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ShadowRunPrototype _003C_003E4__this;

		private TaskAwaiter<bool> _003C_003Eu__1;

		private void MoveNext()
		{
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	[StructLayout((LayoutKind)3)]
	private struct _003CRunProfileOAuthAsync_003Ed__489 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ShadowRunPrototype _003C_003E4__this;

		public bool apple;

		private TaskAwaiter<bool> _003C_003Eu__1;

		private void MoveNext()
		{
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	[StructLayout((LayoutKind)3)]
	private struct _003CRunProfileSignOutAsync_003Ed__494 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ShadowRunPrototype _003C_003E4__this;

		private TaskAwaiter<bool> _003C_003Eu__1;

		private void MoveNext()
		{
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
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


	private void MakeFirstRunTutorial(Transform parent)
	{
	}

	private void BeginFirstRunTutorialIfNeeded()
	{
	}

	private void RestoreFirstRunTutorialAfterPause()
	{
	}

	private void HideFirstRunTutorialForPause()
	{
	}

	private void CancelFirstRunTutorial()
	{
	}

	private void CompleteFirstRunTutorial()
	{
	}

	private void ReleaseFirstRunTutorialReferences()
	{
	}

	private void SetFirstRunTutorialStage(int stage)
	{
	}

	private void SetFirstRunTutorialCopy(string step, string title, string hint)
	{
	}

	private void CenterFirstRunTutorialText(Text text)
	{
	}

	private void LayoutFirstRunTutorialForScreen()
	{
	}

	private void SetFirstRunTutorialHand(bool visible, TextAnchor anchor, Vector2 position, float angle, float size)
	{
	}

	private void PointFirstRunTutorialHandAtButton(RectTransform target)
	{
	}

	private void UpdateFirstRunTutorialProgress(int stage)
	{
	}

	private void CopyFirstRunTutorialTarget(RectTransform target, Color color)
	{
	}

	private void SetFirstRunTutorialHighlightArea(Vector2 position, Vector2 size, TextAnchor anchor, Color color)
	{
	}

	private void UpdateFirstRunTutorial(float unscaledDt)
	{
	}

	private void UpdateFirstRunTutorialHandAnimation(float unscaledDt)
	{
	}

	private bool FirstRunTutorialDoubleTapRequested()
	{
		return false;
	}

	private bool RegisterFirstRunTutorialTap()
	{
		return false;
	}

	private void NotifyFirstRunTutorialBeamPressed()
	{
	}

	private float ReadFirstRunTutorialAccelerationDrag()
	{
		return 0f;
	}

	private bool IsHudActionPoint(Vector2 screenPoint)
	{
		return false;
	}

	private void MakeGarageScreen(Transform parent)
	{
	}

	private void MakeGaragePurchaseAction()
	{
	}

	private Task PurchaseSelectedGarageVehicleAsync()
	{
		return null;
	}

	private void MakeGarageVehicleSelector()
	{
	}

	private GarageVehicleCardData[] CreateGarageVehicleCards()
	{
		return null;
	}

	private Button MakeGarageVehicleCard(Transform parent, GarageVehicleCardData data, int index, Vector2 pos, Vector2 size)
	{
		return null;
	}

	private Sprite GetGarageLockSprite()
	{
		return null;
	}

	private void MakeGarageRarityBadge(Transform parent, GarageVehicleCardData data, Vector2 pos)
	{
	}

	private Sprite GetGarageVehicleCardFillSprite()
	{
		return null;
	}

	private void MakeGarageCardStat(Transform parent, string label, string value, float amount, Color color, Vector2 pos, float width, bool dimmed)
	{
	}

	private void SelectGarageVehicleCard(int index)
	{
	}

	private void QueueGarageVehicleSelection(string vehicleId)
	{
	}

	private void SelectGarageVehicleCardFromClick(int index)
	{
	}

	private void SelectGarageVehicleCardAtPointer(PointerEventData eventData)
	{
	}

	private void AddGarageVehicleCardDragEvents(GameObject target, int cardIndex = -1)
	{
	}

	private void BeginGarageVehicleCardPointer(PointerEventData eventData)
	{
	}

	private void InitializeGarageVehicleCardDrag(PointerEventData eventData)
	{
	}

	private void BeginGarageVehicleCardDrag(PointerEventData eventData)
	{
	}

	private void DragGarageVehicleCard(PointerEventData eventData)
	{
	}

	private void EndGarageVehicleCardDrag(PointerEventData eventData)
	{
	}

	private float ClampGarageVehicleCarouselX(float x)
	{
		return 0f;
	}

	private void SnapGarageVehicleScrollTo(int index, bool instant = false)
	{
	}

	private void UpdateGarageSelectedCardFromScroll()
	{
	}

	private int GetNearestGarageVehicleCardIndex()
	{
		return 0;
	}

	private int GetNearestGarageVehicleCardIndex(float contentX)
	{
		return 0;
	}

	private float GetGarageVehicleViewportWidth()
	{
		return 0f;
	}

	private void UpdateGarageVehicleScrollBounds()
	{
	}

	private void UpdateGarageVehicleCarousel(float dt)
	{
	}

	private int GetGarageVehicleCardIndexForVehicle(string vehicleId)
	{
		return 0;
	}

	private Button MakeGarageVehicleButton(Transform parent, string label, float x, out Text labelText)
	{
		labelText = null;
		return null;
	}

	private void MakeGarageBackground()
	{
	}

	private void MakeGarageAtmosphere()
	{
	}

	private void MakeGarageCar()
	{
	}

	private void AddGaragePreviewDragEvents(GameObject target)
	{
	}

	private void BeginGaragePreviewDrag(PointerEventData eventData)
	{
	}

	private void DragGaragePreview(PointerEventData eventData)
	{
	}

	private void EndGaragePreviewDrag()
	{
	}

	private void MakeGaragePreviewWorld()
	{
	}

	private void BuildImportedGarageScene()
	{
	}

	private void NormalizeGarageScene(GameObject scene)
	{
	}

	private void StyleImportedGarageScene(GameObject scene)
	{
	}

	private void FitGarageVehicle(GameObject previewCar, VehicleDefinition definition)
	{
	}

	private void StyleVehicleForGarage(GameObject previewCar)
	{
	}

	private void SetGaragePreviewVisible(bool visible)
	{
	}

	private void UpdateGaragePreview(float dt)
	{
	}

	private void ApplyGaragePreviewRotation()
	{
	}

	private void ShowGarageScreen()
	{
	}

	private void HideGarageScreen()
	{
	}

	private void UpdateGarageUi()
	{
	}

	private void UpdateGarageVehicleCardVisuals()
	{
	}

	private void ApplyGarageOwnershipFromProfile()
	{
	}

	private void SetGarageVehicleButtonVisual(Button button, Text label, bool selected)
	{
	}

	private void MakeUI()
	{
	}

	private void MakeGameplayTouchControls(Transform parent)
	{
	}

	private void MakeGameplayTouchZone(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, HashSet<int> activePointers, Action changed)
	{
	}

	private void MakeSpeedometer(Transform parent)
	{
	}

	private Sprite CreateSpeedometerNeedleSprite()
	{
		return null;
	}

	private Image MakeBar(Transform parent, Vector2 pos, Color color)
	{
		return null;
	}

	private void MakeHudPauseButton(Transform parent)
	{
	}

	private void MakeHudBrakeButton(Transform parent)
	{
	}

	private void ReleaseBrakePointer(BaseEventData data)
	{
	}

	private void MakeHudHighBeamButton(Transform parent)
	{
	}

	private Button MakeHudActionButton(Transform parent, string label, Vector2 pos, Vector2 size, Color accent)
	{
		return null;
	}

	private void MakeHudActionIcon(Transform parent, string label, Vector2 size, Color accent)
	{
	}

	private void MakeOverlay(Transform parent)
	{
	}

	private void MakeGameOverScreen(Transform parent)
	{
	}

	private void MakePauseActions(Transform parent)
	{
	}

	private Button MakePauseArtButton(Transform parent, string objectName, string resource, Vector2 position, Vector2 size)
	{
		return null;
	}

	private Button MakeSoundToggleButton(Transform parent, string objectName, TextAnchor anchor, Vector2 position, Vector2 size, out Image icon)
	{
		icon = null;
		return null;
	}

	private Text MakeGameOverValue(Transform parent, Vector2 position)
	{
		return null;
	}

	private Button MakeGameOverArtButton(Transform parent, string objectName, string resource, Vector2 position, Vector2 size)
	{
		return null;
	}

	private void MakeHomeMenu(Transform parent)
	{
	}

	private void ConfigureHomeCardLayout(RectTransform card, float preferredWidth)
	{
	}

	private void MakeGemCounterWidget(Transform parent)
	{
	}

	private void PositionGemCounterSafeArea()
	{
	}

	private void MakeGemShopOverlay(Transform parent)
	{
	}

	private void MakeGemPackCard(Transform parent, string iconResource, int amount, string price, Vector2 pos, bool sale)
	{
	}

	private Button MakeShopPurchaseButton(Transform parent, string price, Vector2 pos)
	{
		return null;
	}

	private void ShowGemShop()
	{
	}

	private void HideGemShop()
	{
	}

	private void AddGemsFromShop(int amount)
	{
	}

	private void MakeHomeHeroCard(Transform parent)
	{
	}

	private void MakeHomeDriveShine(Image driveArt)
	{
	}

	private Sprite CreateDriveGlossSprite()
	{
		return null;
	}

	private void MakeHome101Artwork(RectTransform layoutRow)
	{
	}

	private void MakeWeatherPill(Transform parent)
	{
	}

	private void MakeHomeCaseCard(Transform parent)
	{
	}

	private void MakeCaseOverlay(Transform parent)
	{
	}

	private void MakeCaseOpeningPreviewLayer(Transform parent)
	{
	}

	private void MakeCaseDisplay(Transform parent)
	{
	}

	private void MakeCaseBottomArea(Transform parent)
	{
	}

	private void MakeCaseRoulette(Transform parent)
	{
	}

	private void MakeCaseResultPanel(Transform parent)
	{
	}

	private void MakeCornerAccent(Transform parent, string name, Vector2 pos, bool left, bool top)
	{
	}

	private void ShowCaseResultPanel()
	{
	}

	private void UpdateCaseResultVisual()
	{
	}

	private void CollectCaseReward()
	{
	}

	private void ApplyCaseRewardOnce()
	{
	}

	private void SpinCaseAgain()
	{
	}

	private void RequestCaseRewardedOpening()
	{
	}

	private void RequestCaseRewardedOpening(bool openingAgain)
	{
	}

	private bool CaseOpeningRequestIsCurrent(int requestSerial)
	{
		return false;
	}

	private void SetCaseAdUnavailable(string message)
	{
	}

	private bool EnsureCasePreviewWorld()
	{
		return false;
	}

	private void NormalizeCasePreviewModel()
	{
	}

	private void AlignCaseHeadToClosedBodyPose()
	{
	}

	private void StyleCasePreviewModel()
	{
	}

	private void SetCasePreviewVisible(bool visible)
	{
	}

	private void AddCasePreviewDragEvents(GameObject target)
	{
	}

	private void DragCasePreview(PointerEventData eventData)
	{
	}

	private void ApplyCasePreviewPose()
	{
	}

	private void ResetCasePreviewPose()
	{
	}

	private void StartCaseOpening()
	{
	}

	private void BeginCaseRoulette()
	{
	}

	private CasePrize RollCasePrize()
	{
		return default;
	}

	private List<string> GetLockedCaseVehicles()
	{
		return null;
	}

	private CasePrize RandomRouletteDisplayPrize()
	{
		return default;
	}

	private string CaseVehicleDisplayName(string vehicleId)
	{
		return null;
	}

	private string CaseVehicleImageResource(string vehicleId)
	{
		return null;
	}

	private string CaseDiamondImageResource(CasePrize prize)
	{
		return null;
	}

	private Vector2 CaseRoulettePrizeArtSize(CasePrize prize)
	{
		return default;
	}

	private Vector2 CaseResultPrizeArtSize(CasePrize prize)
	{
		return default;
	}

	private Color CasePrizeColor(CasePrize prize)
	{
		return default;
	}

	private string CasePrizeRarity(CasePrize prize)
	{
		return null;
	}

	private void BuildCaseRouletteItems(int targetIndex, CasePrize targetPrize, float cardWidth, float cardHeight, float spacing)
	{
	}

	private void MakeCasePrizeCard(Transform parent, int index, CasePrize prize, Vector2 pos, Vector2 size, bool target)
	{
	}

	private void UpdateCaseRouletteFocus(float contentX, float cardWidth, float spacing, float viewportWidth, bool tick)
	{
	}

	private void UpdateCaseRouletteChrome(float t, float dt)
	{
	}

	private void UpdateCaseOpening(float dt)
	{
	}

	private static float HermitePosition(float start, float end, float startTangent, float endTangent, float t)
	{
		return 0f;
	}

	private void UpdateCaseOpenAnimation(float dt)
	{
	}

	private void FinishCaseOpenAnimationAndStartRoulette()
	{
	}

	private void CompleteCaseOpening()
	{
	}

	private void ResetCaseOpening(bool showReadyPreview = true)
	{
	}

	private void PlayCaseSequenceAudio(bool opening)
	{
	}

	private void StopCaseSequenceAudio()
	{
	}

	private void ShowCaseOverlay()
	{
	}

	private void HideCaseOverlay()
	{
	}

	private void MakeHomeGoalsCard(Transform parent)
	{
	}

	private HomeGoalData[] PickDailyGoals(int count)
	{
		return null;
	}

	private HomeGoalData[] CreateDailyGoalPool()
	{
		return null;
	}

	private HomeGoalData MakeDailyGoal(DailyGoalKind kind, string title, string iconName, Color accent)
	{
		return null;
	}

	private void MakeGoalRow(Transform parent, int index, HomeGoalData data)
	{
	}

	private void SetGoalRowTextRect(Text text, float centerOffset, float height)
	{
	}

	private void SetGoalTextAnchor(Text text, Vector2 anchor, Vector2 anchoredPosition, Vector2 size)
	{
	}

	private Image MakeReferenceArtImage(Transform parent, string objectName, string resource, Vector2 pos, Vector2 size, TextAnchor anchor)
	{
		return null;
	}

	private Sprite LoadTightMenuArtSprite(string resource)
	{
		return null;
	}

	private Button MakeReferenceArtButton(Transform parent, string objectName, string resource, Vector2 pos, Vector2 size, TextAnchor anchor)
	{
		return null;
	}

	private void UpdateDailyGoalsTimer(bool force = false)
	{
	}

	private void MakeMenuTabBar(Transform parent)
	{
	}

	private void MakeMenuTabButton(string label, string iconResource, float x, float iconSize, int tabIndex, out Text labelText, out Image icon, out CanvasGroup glow)
	{
		labelText = null;
		icon = null;
		glow = null;
	}

	private Image MakeTabIcon(Transform parent, string name, string resource, Vector2 size, Vector2 pos)
	{
		return null;
	}

	private Material GetMenuTabIconTintMaterial()
	{
		return null;
	}

	private RawImage MakeTabRawIcon(Transform parent, string name, Texture texture, Vector2 size, Vector2 pos)
	{
		return null;
	}

	private CanvasGroup MakeTabIconGlow(Transform parent, string name, Vector2 size, Vector2 pos)
	{
		return null;
	}

	private void UpdateMenuTabBar(float tabPosition)
	{
	}

	private void SetReferenceTabVisual(Text label, Image icon, CanvasGroup glow, float weight)
	{
	}

	private void SelectMenuTab(int tab, bool instant)
	{
	}

	private void BeginMenuTabTransition(bool instant)
	{
	}

	private void PositionMenuTabs(bool instant)
	{
	}

	private RectTransform MakeMenuPageHeader(Transform parent, string titleText, string subtitleText, out Text subtitle)
	{
		subtitle = null;
		return null;
	}

	private void LayoutMenuPageHeaders()
	{
	}

	private static void LayoutMenuPageHeader(RectTransform root, float left, float top, float width)
	{
	}

	private void SetMenuPagePosition(GameObject page, float index, float width)
	{
	}

	private float GetMenuViewportWidth()
	{
		return 0f;
	}

	private Image MakeGlassPanel(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor, Color fill, Color stroke)
	{
		return null;
	}

	private Image MakeThinGlassPanel(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor)
	{
		return null;
	}

	private Image MakeThinGlassPanel(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor, Color fill)
	{
		return null;
	}

	private Image MakeThinGlassPanel(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor, Color fill, Color borderColor)
	{
		return null;
	}

	private Image MakeThinGlassPanel(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor, Color fill, Color borderColor, float borderThickness)
	{
		return null;
	}

	private Button MakeIconButton(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor, string iconLabel, int iconSize)
	{
		return null;
	}

	private RawImage MakeResourceRawImage(Transform parent, string name, string resource, Vector2 pos, Vector2 size, TextAnchor anchor)
	{
		return null;
	}

	private Image MakeResourceImage(Transform parent, string name, string resource, Vector2 pos, Vector2 size, TextAnchor anchor, bool preserveAspect)
	{
		return null;
	}

	private Button MakeImageButton(Transform parent, string label, string resource, Vector2 pos, Vector2 size, TextAnchor anchor)
	{
		return null;
	}

	private void ApplyRawCover(RawImage image, float targetAspect)
	{
	}

	private void FitRawImageAspect(RawImage image, Vector2 maxSize)
	{
	}

	private void FitImageAspect(Image image, Vector2 maxSize)
	{
	}

	private void ApplyObjectFitCover(RectTransform img, RectTransform viewport, Sprite sprite)
	{
	}

	private void MakeProgressBar(Transform parent, Vector2 pos, Vector2 size, float progress, Color fillColor)
	{
	}

	private void MakeFigmaProgressBar(Transform parent, Vector2 pos, Vector2 size, float progress)
	{
	}

	private void LayoutHomeMenu()
	{
	}

	private void LayoutHomeHeroArt()
	{
	}

	private void MakeSettingsScreen(Transform parent)
	{
	}

	private Image MakeProfilePanel(Transform parent, string name, Vector2 pos, Vector2 size, Color fill, Color borderColor, bool useGeneratedOutline = true)
	{
		return null;
	}

	private Button MakeProfileActionButton(Transform parent, string label, Vector2 pos, Vector2 size, bool primary, bool danger, out Text labelText, bool neonContour = false, bool addNeonGlow = true)
	{
		labelText = null;
		return null;
	}

	private Button MakeProfileArtworkAuthButton(Transform parent, string name, string resource, Vector2 pos, Vector2 size)
	{
		return null;
	}

	private InputField MakeProfileInput(Transform parent, string name, string placeholderValue, Vector2 pos, bool password)
	{
		return null;
	}

	private void BuildProfileAuthForm()
	{
	}

	private void BuildSignedInProfileActions()
	{
	}

	private void LayoutProfileScreen()
	{
	}

	private void RefreshProfileUi()
	{
	}

	private void ShowProfileAuthForm(bool register)
	{
	}

	private void HideProfileAuthForm()
	{
	}

	private Task RunProfileAuthAsync()
	{
		return null;
	}

	private Task RunProfileOAuthAsync(bool apple)
	{
		return null;
	}

	private void RequestProfileSignOutConfirmation()
	{
	}

	private void RequestProfileDeleteConfirmation()
	{
	}

	public void OnNativeProfileSignOutConfirmed(string message)
	{
	}

	public void OnNativeProfileDeleteConfirmed(string message)
	{
	}

	private Task RunProfileSignOutAsync()
	{
		return null;
	}

	private Task RunProfileDeleteAsync()
	{
		return null;
	}

	private void SetProfileActionsInteractable(bool value)
	{
	}

	private void SetProfileAuthenticationStatus(string message)
	{
	}

	private Text MakeSettingsSlider(string label, float y, int value, UnityAction action)
	{
		return null;
	}

	private Text MakeSettingsStepper(string label, float y, UnityAction left, UnityAction right)
	{
		return null;
	}

	private Text MakeSettingsToggle(string label, float y, UnityAction action, out Image dot)
	{
		dot = null;
		return null;
	}

	private void MakeSettingsLabel(string label, float y)
	{
	}

	private Button MakeSettingsAction(string label, Vector2 pos, TextAnchor anchor)
	{
		return null;
	}

	private void MakeSettingsArrow(Vector2 pos, string label, UnityAction action)
	{
	}

	private void MakeSettingsClickArea(float y, UnityAction action)
	{
	}

	private void ShowSettingsScreen()
	{
	}

	private void HideSettingsScreen()
	{
	}

	private void UpdateSettingsUi()
	{
	}

	private void UpdateToggle(Text text, Image dot, bool value)
	{
	}

	private void MakeMenuBackdrop(Transform parent)
	{
	}

	private Sprite LoadResourceSprite(string path)
	{
		return null;
	}

	private Image MakeUiBlock(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor, Color color)
	{
		return null;
	}

	private Button MakeButton(Transform parent, string label, Vector2 pos, Vector2 size, TextAnchor anchor, Color bg, Color fg)
	{
		return null;
	}

	private Button MakeCaseActionButton(Transform parent, string label, Vector2 pos, Vector2 size, TextAnchor anchor, bool filled)
	{
		return null;
	}

	private Button MakeGlassButton(Transform parent, string label, Vector2 pos, Vector2 size, bool primary)
	{
		return null;
	}

	private Texture2D LoadMenuButtonTexture(string label)
	{
		return null;
	}

	private void AddButtonIcon(Transform parent, string label, Vector2 size, bool primary, Color red)
	{
	}

	private void MakeSpacedWord(Transform parent, string word, Vector2 pos, int size, float spacing, Color color, bool glow)
	{
	}

	private string AddButtonSpacing(string value)
	{
		return null;
	}

	private Sprite MakeFrostSprite(int width, int height, int seed)
	{
		return null;
	}

	private Sprite MakeCloudSprite(int width, int height, int seed)
	{
		return null;
	}

	private Sprite MakeAdvancedCloudSprite(int width, int height, int seed)
	{
		return null;
	}

	private Sprite MakeCircleSprite(int size)
	{
		return null;
	}

	private Sprite MakeRadialGlowSprite(int size)
	{
		return null;
	}

	private Sprite MakeRoundedRectSprite(int width, int height, int radius, float strength)
	{
		return null;
	}

	private Sprite MakeCaseActionButtonSprite(int width, int height, float radius, bool primary)
	{
		return null;
	}

	private Sprite MakeGemCounterSprite(int width, int height, float radius)
	{
		return null;
	}

	private Sprite MakeGradientBarSprite(int width, int height, Color left, Color right)
	{
		return null;
	}

	private Sprite MakeCaseRarityStripSprite(int width, int height)
	{
		return null;
	}

	private Sprite MakeCaseRouletteRailSprite(int width, int height)
	{
		return null;
	}

	private Sprite MakeHorizontalFadeSprite(int width, int height, bool leftToRight)
	{
		return null;
	}

	private Sprite MakeCaseScanlineSprite(int width, int height)
	{
		return null;
	}

	private Sprite MakeVerticalLineSprite(int width, int height, Color top, Color bottom)
	{
		return null;
	}

	private Sprite MakeTriangleSprite(int width, int height, bool down)
	{
		return null;
	}

	private Sprite MakeVerticalFadeSprite(int width, int height)
	{
		return null;
	}

	private Sprite MakeReferenceTabFrameSprite(int width, int height, float radius)
	{
		return null;
	}

	private Sprite MakeReferenceActiveTabSprite(int width, int height, float radius)
	{
		return null;
	}

	private float RoundedBoxDistance(Vector2 point, Vector2 halfSize, float radius)
	{
		return 0f;
	}

	private void ApplyAnchor(RectTransform rect, TextAnchor anchor)
	{
	}

	private Text MakeText(Transform parent, string content, int size, TextAnchor anchor, Vector2 pos)
	{
		return null;
	}

	private Font GetUiFont(string content)
	{
		return null;
	}

	private bool UsesMenuFontGlyphs(string content)
	{
		return false;
	}

	private void UpdateMenuAnimation(float dt)
	{
	}

	private void UpdateHomeDriveShine()
	{
	}

	private void ApplySafeArea()
	{
	}

	private void RebuildGaragePreviewVehicle()
	{
	}

	private void PreviewGarageVehicle(string vehicleId)
	{
	}

	private void _003CMakeGaragePurchaseAction_003Eb__237_0()
	{
	}

	private void _003CAddGaragePreviewDragEvents_003Eb__269_0(BaseEventData data)
	{
	}

	private void _003CAddGaragePreviewDragEvents_003Eb__269_1(BaseEventData data)
	{
	}

	private void _003CAddGaragePreviewDragEvents_003Eb__269_2(BaseEventData _)
	{
	}

	private void _003CAddGaragePreviewDragEvents_003Eb__269_3(BaseEventData _)
	{
	}

	private void _003CMakeGameplayTouchControls_003Eb__339_0()
	{
	}

	private void _003CMakeHudBrakeButton_003Eb__345_0(BaseEventData data)
	{
	}

	private void _003CMakeHudHighBeamButton_003Eb__347_0(BaseEventData _)
	{
	}

	private void _003CMakeHomeCaseCard_003Eb__377_0(BaseEventData _)
	{
	}

	private void _003CMakeHomeCaseCard_003Eb__377_1()
	{
	}

	private void _003CAddCasePreviewDragEvents_003Eb__401_0(BaseEventData data)
	{
	}

	private void _003CMakeSettingsScreen_003Eb__477_0()
	{
	}

	private void _003CMakeSettingsScreen_003Eb__477_1()
	{
	}

	private void _003CBuildProfileAuthForm_003Eb__482_0()
	{
	}

	private void _003CBuildProfileAuthForm_003Eb__482_1()
	{
	}

	private void _003CBuildProfileAuthForm_003Eb__482_2()
	{
	}
}
