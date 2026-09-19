using System;
using UnityEngine;
using UnityEngine.Rendering;
using ShadowRun.Services;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_get_ActiveVehicle, _MakeCar, _CreateVehicleDefinition,
// _CreateDefaultSportCarDefinition, _CreateFlyingCarDefinition, _CreateInfalModelDefinition, _CreateCar3Definition,
// _LoadActiveVehiclePrefab, _CreateConfiguredVehicleModel (x2), _VehicleNodeNameMatches, _SetActiveVehicleDefinition,
// _SelectVehicle, _RebuildPlayerVehicleModel, _SetPlayerHoverSuppressedForJump, _ConfigureVehicleHover (x2),
// _RefreshVehicleEffectAnchors, _PrepareImportedVehicleModel, _HideImportedVehicleRenderArtifacts,
// _IsImportedVehicleGroundShadow, _ShouldHideImportedVehicleRenderer, _IsNamedImportedVehiclePlaneArtifact,
// _IsPlaneArtifactName, _FitImportedVehicleModel, _VehicleModelBaseRotation, _PlayerVehicleModelBaseRotation,
// _PlayerVehicleVisualRotation, _CurrentPlayerVehicleRoadYOffset, _CachePlayerWheelMeshes, _ClearPlayerWheelMeshCache,
// _CachePlayerWheelBaseRotations, _CachePlayerWheelSpinRate, _AttachDiscToNearestWheel, _SetPlayerWheelSpin,
// _CachePlayerBrakeLights, _GetOrCreateP900BrakeLightMaterial, _ClearP900BrakeLightMaterial, _IsPlayerBrakeLightName,
// _SetPlayerBrakeLights, _MakeHighBeamLight, _CreateHeadlightEmitter, _CurrentHighBeam* (x6), _MakeHighBeamVisual,
// _CreateHighBeamPrefabInstance, _SetHighBeamShaftLocalZ, _ConfigureHighBeamPrefab.
public sealed partial class ShadowRunPrototype
{
	private VehicleDefinition ActiveVehicle
	{
		get
		{
			if (activeVehicleDefinition == null)
			{
				string vehicleId = PlayerPrefs.GetString(SelectedVehiclePreference, SportCarReferenceResource);
				if (!SupabaseGameManager.IsVehicleUnlocked(vehicleId))
				{
					vehicleId = SportCarReferenceResource;
				}
				activeVehicleDefinition = CreateVehicleDefinition(vehicleId);
			}
			return activeVehicleDefinition;
		}
	}

	private void MakeCar()
	{
		player = new GameObject("Player Car").transform;
		playerVehicleYawOffset = ActiveVehicle.ModelYawOffset;
		if (!RebuildPlayerVehicleModel())
		{
			return;
		}
		MakeHighBeamLight();
	}

	private VehicleDefinition CreateVehicleDefinition(string vehicleId)
	{
		/* [InfernoLobby patch] */ { VehicleDefinition edition = InfernoGarage.MakeDefinition(this, vehicleId); if (edition != null) return edition; }
		if (string.Equals(vehicleId, "flying_car", StringComparison.OrdinalIgnoreCase))
		{
			return CreateFlyingCarDefinition();
		}
		if (string.Equals(vehicleId, "infal_model", StringComparison.OrdinalIgnoreCase))
		{
			return CreateInfalModelDefinition();
		}
		if (string.Equals(vehicleId, "car3", StringComparison.OrdinalIgnoreCase))
		{
			return CreateCar3Definition();
		}
		return CreateDefaultSportCarDefinition();
	}

	private VehicleDefinition CreateDefaultSportCarDefinition()
	{
		VehicleDefinition definition = new VehicleDefinition();
		definition.Id = "sportcar";
		definition.ModelResource = "sportcar";
		definition.PlayerModelName = "Player Sportcar Model";
		definition.GarageModelName = "Garage Preview Sportcar";
		definition.ModelYawOffset = 180f;
		definition.RoadYOffset = 0.14f;
		definition.TargetModelSize = new Vector3(1.84f, 1.05f, 3.64f);
		definition.HeadlightOriginHeight = 0.3f;
		definition.HeadlightFrontInset = 0.08f;
		definition.HeadlightEmitterWidth = 0.5f;
		definition.HeadlightEmitterHeightOffset = -0.04f;
		definition.HeadlightLeftShaftLocalZ = -0.494f;
		definition.HeadlightRightShaftLocalZ = -0.447f;
		definition.SmokeOffset = new Vector3(0f, 0.46f, 1.92f);
		definition.WheelMeshNames = new string[2] { "gumamain.001", "gumamain.002" };
		definition.BrakeDiscNames = new string[2] { "disc1.001", "disc1.003" };
		definition.BrakeLightNames = new string[4] { "fara1", "fara1.001", "fara1.002", "fara2" };
		definition.UsePlayerSpaceExhaustSockets = true;
		definition.ExhaustPlayerX = 0.4437f;
		definition.ExhaustPlayerY = 0.184977f;
		definition.ExhaustPlayerZ = 1.72f;
		definition.BoundsRendererNameKeywords = Array.Empty<string>();
		definition.HideOversizedPlaneRenderers = false;
		definition.GarageTargetFootprint = 2.45f;
		definition.GarageMaximumScale = 1.35f;
		definition.GarageRoadYOffset = 0.02f;
		definition.GarageHorizontalOffset = 0.07f;
		definition.HoverEnabled = false;
		definition.UsesWheelSmoke = true;
		definition.ArcadeAcceleration = 1.04f;
		definition.ArcadeSteering = 1f;
		definition.ArcadeGrip = 1.02f;
		definition.ArcadeDrift = 1f;
		return definition;
	}

	private VehicleDefinition CreateFlyingCarDefinition()
	{
		VehicleDefinition definition = new VehicleDefinition();
		definition.Id = "flying_car";
		definition.ModelResource = "flying_car";
		definition.PlayerModelName = "Player Flying Car Model";
		definition.GarageModelName = "Garage Preview Flying Car";
		definition.ModelYawOffset = 180f;
		definition.RoadYOffset = 0.78f;
		definition.TargetModelSize = new Vector3(1.84f, 1.18f, 3.64f);
		definition.HeadlightOriginHeight = 0.52f;
		definition.HeadlightFrontInset = 0.04f;
		definition.HeadlightEmitterWidth = 0.44f;
		definition.HeadlightEmitterHeightOffset = 0.02f;
		definition.HeadlightLeftShaftLocalZ = -0.08f;
		definition.HeadlightRightShaftLocalZ = -0.08f;
		definition.SmokeOffset = Vector3.zero;
		definition.WheelMeshNames = Array.Empty<string>();
		definition.BrakeDiscNames = Array.Empty<string>();
		definition.BrakeLightNames = Array.Empty<string>();
		definition.BoundsRendererNameKeywords = Array.Empty<string>();
		definition.HideOversizedPlaneRenderers = false;
		definition.GarageTargetFootprint = 2.55f;
		definition.GarageMaximumScale = 1.35f;
		definition.GarageRoadYOffset = 0.35f;
		definition.GarageHorizontalOffset = 0.07f;
		definition.HoverEnabled = true;
		definition.HoverSpeed = 2f;
		definition.HoverHeight = 0.15f;
		definition.UsesWheelSmoke = false;
		definition.ArcadeAcceleration = 1.08f;
		definition.ArcadeSteering = 1.06f;
		definition.ArcadeGrip = 1.1f;
		definition.ArcadeDrift = 0.78f;
		return definition;
	}

	private VehicleDefinition CreateInfalModelDefinition()
	{
		VehicleDefinition definition = new VehicleDefinition();
		definition.Id = "infal_model";
		definition.ModelResource = "IFNAL_MODEL_P900";
		definition.PlayerModelName = "Player IFNAL P900 Model";
		definition.GarageModelName = "Garage Preview IFNAL P900";
		definition.ModelYawOffset = 180f;
		definition.RoadYOffset = 0f;
		definition.TargetModelSize = new Vector3(1.84f, 1.08f, 3.64f);
		definition.HeadlightOriginHeight = 0.34f;
		definition.HeadlightFrontInset = 0.08f;
		definition.HeadlightEmitterWidth = 0.52f;
		definition.HeadlightEmitterHeightOffset = -0.03f;
		definition.HeadlightLeftShaftLocalZ = -0.494f;
		definition.HeadlightRightShaftLocalZ = -0.447f;
		definition.SmokeOffset = new Vector3(0f, 0.44f, 1.92f);
		definition.WheelMeshNames = Array.Empty<string>();
		definition.BrakeDiscNames = Array.Empty<string>();
		definition.BrakeLightNames = new string[1] { "dtP900:glass_glass_UV1_car_DeTomaso_P900_glass" };
		definition.BrakeLightMaterialNames = new string[1] { "red_glass" };
		definition.ExhaustXExtentFactor = 0.76f;
		definition.ExhaustYHeightFactor = 0.18f;
		definition.ExhaustRearOffset = 0.08f;
		definition.ExhaustReferenceName = "dtP900:emissive_ID_rear_chassis_UV2_car_DeTomaso_P900_details_normal";
		definition.NormalizeExhaustWorldScale = true;
		definition.UsePlayerSpaceExhaustSockets = true;
		definition.ExhaustPlayerX = 0.56f;
		definition.ExhaustPlayerY = 0.475f;
		definition.ExhaustPlayerZ = 1.72f;
		definition.ExhaustScaleMultiplier = 0.333333f;
		definition.CompactExhaustEffect = true;
		definition.BoundsRendererNameKeywords = new string[9] { "carpaint", "chassis", "details", "diffuser", "lip", "rims", "wheel", "p900", "detomaso" };
		definition.HideOversizedPlaneRenderers = false;
		definition.GarageTargetFootprint = 2.5f;
		definition.GarageMaximumScale = 80f;
		definition.GarageRoadYOffset = 0.02f;
		definition.GarageHorizontalOffset = 0.07f;
		definition.HoverEnabled = false;
		definition.UsesWheelSmoke = true;
		definition.ArcadeAcceleration = 1.12f;
		definition.ArcadeSteering = 0.94f;
		definition.ArcadeGrip = 0.96f;
		definition.ArcadeDrift = 1.14f;
		return definition;
	}

	private VehicleDefinition CreateCar3Definition()
	{
		VehicleDefinition definition = new VehicleDefinition();
		definition.Id = "car3";
		definition.ModelResource = "car3";
		definition.PlayerModelName = "Player Cayman GT4 Model";
		definition.GarageModelName = "Garage Preview Cayman GT4";
		definition.ModelYawOffset = 180f;
		definition.RoadYOffset = 0f;
		definition.TargetModelSize = new Vector3(1.84f, 1.06f, 3.64f);
		definition.HeadlightOriginHeight = 0.34f;
		definition.HeadlightFrontInset = 0.08f;
		definition.HeadlightEmitterWidth = 0.5f;
		definition.HeadlightEmitterHeightOffset = -0.035f;
		definition.HeadlightLeftShaftLocalZ = -0.494f;
		definition.HeadlightRightShaftLocalZ = -0.447f;
		definition.SmokeOffset = new Vector3(0f, 0.43f, 1.92f);
		definition.WheelMeshNames = new string[2] { "CaymanGT4.Wheel.Ft.L", "CaymanGT4.Wheel.Ft.R" };
		definition.BrakeDiscNames = new string[2] { "CaymanGT4.WheelBrake.Ft.L", "CaymanGT4.WheelBrake.Ft.R" };
		definition.BrakeLightNames = new string[2] { "CaymanGT4.LeftTaillight", "CaymanGT4.RightTaillight" };
		definition.BoundsRendererNameKeywords = new string[13] { "CaymanGT4", "Body", "Bumper", "Fender", "Door", "Hood", "Tailgate", "Spoiler", "Wheel", "Brake Disc", "Glass", "Headlight", "Taillight" };
		definition.HideOversizedPlaneRenderers = true;
		definition.GarageTargetFootprint = 2.48f;
		definition.GarageMaximumScale = 80f;
		definition.GarageRoadYOffset = 0.02f;
		definition.GarageHorizontalOffset = 0.07f;
		definition.HoverEnabled = false;
		definition.UsesWheelSmoke = true;
		definition.ArcadeAcceleration = 1f;
		definition.ArcadeSteering = 1.1f;
		definition.ArcadeGrip = 1.14f;
		definition.ArcadeDrift = 0.9f;
		return definition;
	}

	private GameObject LoadActiveVehiclePrefab()
	{
		return Resources.Load<GameObject>(ActiveVehicle.ModelResource);
	}

	private GameObject CreateConfiguredVehicleModel(Transform parent, string modelName)
	{
		return CreateConfiguredVehicleModel(parent, modelName, ActiveVehicle);
	}

	private GameObject CreateConfiguredVehicleModel(Transform parent, string modelName, VehicleDefinition definition)
	{
		if (definition == null)
		{
			return null;
		}
		GameObject prefab = Resources.Load<GameObject>(definition.ModelResource);
		if (prefab == null)
		{
			Debug.LogError("Vehicle model not found in Resources: " + definition.ModelResource);
			return null;
		}
		GameObject model = Instantiate(prefab);
		model.name = modelName;
		model.transform.SetParent(parent, false);
		model.transform.localPosition = Vector3.zero;
		model.transform.localRotation = Quaternion.Euler(0f, definition.ModelYawOffset, 0f);
		model.transform.localScale = Vector3.one;
		DisableImportedModelCamerasAndLights(model);
		PrepareImportedVehicleModel(model);
		/* [InfernoLobby patch] */ InfernoGarage.Decorate(this, definition, model);
		return model;
	}

	private bool VehicleNodeNameMatches(string nodeName, string[] configuredNames)
	{
		if (string.IsNullOrEmpty(nodeName) || configuredNames == null)
		{
			return false;
		}
		for (int i = 0; i < configuredNames.Length; i++)
		{
			if (string.Equals(nodeName, configuredNames[i], StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private bool SetActiveVehicleDefinition(VehicleDefinition definition)
	{
		if (definition == null || string.IsNullOrWhiteSpace(definition.ModelResource))
		{
			return false;
		}
		if (!SupabaseGameManager.IsVehicleUnlocked(definition.Id))
		{
			return false;
		}
		if (Resources.Load<GameObject>(definition.ModelResource) == null)
		{
			return false;
		}
		activeVehicleDefinition = definition;
		playerVehicleYawOffset = definition.ModelYawOffset;
		PlayerPrefs.SetString(SelectedVehiclePreference, definition.Id);
		PlayerPrefs.Save();
		if (player != null)
		{
			RebuildPlayerVehicleModel();
		}
		bool previewMatches = garagePreviewVehicleRoot != null && garagePreviewVehicleDefinition != null && string.Equals(garagePreviewVehicleDefinition.Id, definition.Id, StringComparison.OrdinalIgnoreCase);
		if (garagePreviewCarHolder != null && !previewMatches)
		{
			RebuildGaragePreviewVehicle();
		}
		UpdateGarageUi();
		return true;
	}

	private void SelectVehicle(string vehicleId)
	{
		if (!SupabaseGameManager.IsVehicleUnlocked(vehicleId))
		{
			return;
		}
		if (string.Equals(ActiveVehicle.Id, vehicleId, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		SetActiveVehicleDefinition(CreateVehicleDefinition(vehicleId));
	}

	private bool RebuildPlayerVehicleModel()
	{
		GameObject model = CreateConfiguredVehicleModel(player, ActiveVehicle.PlayerModelName);
		if (model == null)
		{
			return false;
		}
		DisableHighBeam();
		ClearPlayerExhaustFire();
		ClearP900BrakeLightMaterial();
		if (carVisual != null)
		{
			Destroy(carVisual.gameObject);
		}
		ClearPlayerWheelMeshCache();
		playerBrakeLightRenderers.Clear();
		playerBrakeLightsOn = false;
		Vector3 targetSize = ActiveVehicle.TargetModelSize;
		FitImportedVehicleModel(model.transform, player, targetSize.x, targetSize.z, targetSize.y, ActiveVehicle.BoundsRendererNameKeywords);
		/* [InfernoLobby patch] */ InfernoStyle.Dress(this, model, ActiveVehicle, false);
		HideImportedVehicleRenderArtifacts(model);
		CachePlayerWheelMeshes(model.transform);
		CachePlayerBrakeLights(model.transform);
		carVisual = model.transform;
		carVisual.localPosition = new Vector3(0f, ActiveVehicle.RoadYOffset, 0f);
		carVisual.localRotation = PlayerVehicleModelBaseRotation();
		ConfigureVehicleHover(model, false);
		playerHoverAnimation = model.GetComponent<HoverAnimation>();
		SetupPlayerExhaustFire(player, carVisual);
		RefreshVehicleEffectAnchors();
		return true;
	}

	private void SetPlayerHoverSuppressedForJump(bool suppressed)
	{
		if (playerHoverAnimation == null)
		{
			return;
		}
		bool enabled = !suppressed;
		if (playerHoverAnimation.enabled == enabled)
		{
			return;
		}
		if (enabled)
		{
			playerHoverAnimation.CaptureBaseTransform();
			playerHoverAnimation.enabled = true;
			return;
		}
		playerHoverAnimation.enabled = false;
	}

	private void ConfigureVehicleHover(GameObject model, bool garagePreview)
	{
		ConfigureVehicleHover(model, garagePreview, ActiveVehicle);
	}

	private void ConfigureVehicleHover(GameObject model, bool garagePreview, VehicleDefinition definition)
	{
		if (model == null || definition == null || !definition.HoverEnabled)
		{
			return;
		}
		HoverAnimation hover = model.GetComponent<HoverAnimation>();
		if (hover == null)
		{
			hover = model.AddComponent<HoverAnimation>();
		}
		hover.hoverSpeed = definition.HoverSpeed;
		hover.hoverHeight = definition.HoverHeight;
		hover.swaySpeed = 1.35f;
		hover.swayAngle = garagePreview ? 1.4f : 0.9f;
		hover.rotationSpeed = 0f;
		hover.useUnscaledTime = garagePreview;
		hover.CaptureBaseTransform();
	}

	private void RefreshVehicleEffectAnchors()
	{
		if (highBeamOrigin != null)
		{
			highBeamOrigin.localPosition = new Vector3(0f, CurrentHighBeamOriginHeight(), -VehicleHalfLength + CurrentHighBeamFrontInset());
		}
		if (highBeamLeftEmitter != null)
		{
			highBeamLeftEmitter.localPosition = new Vector3(-CurrentHighBeamEmitterWidth(), CurrentHighBeamEmitterHeightOffset(), 0f);
		}
		if (highBeamRightEmitter != null)
		{
			highBeamRightEmitter.localPosition = new Vector3(CurrentHighBeamEmitterWidth(), CurrentHighBeamEmitterHeightOffset(), 0f);
		}
		if (highBeamLeftPrefabInstance != null)
		{
			SetHighBeamShaftLocalZ(highBeamLeftPrefabInstance, CurrentHighBeamLeftShaftLocalZ());
		}
		if (highBeamRightPrefabInstance != null)
		{
			SetHighBeamShaftLocalZ(highBeamRightPrefabInstance, CurrentHighBeamRightShaftLocalZ());
		}
	}

	private void PrepareImportedVehicleModel(GameObject model)
	{
		Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			if (ShouldHideImportedVehicleRenderer(renderer))
			{
				renderer.enabled = false;
				continue;
			}
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			renderer.lightProbeUsage = LightProbeUsage.Off;
			renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		}
		Collider[] colliders = model.GetComponentsInChildren<Collider>(true);
		for (int i = 0; i < colliders.Length; i++)
		{
			Destroy(colliders[i]);
		}
	}

	private void HideImportedVehicleRenderArtifacts(GameObject model)
	{
		if (model == null)
		{
			return;
		}
		Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			if (ShouldHideImportedVehicleRenderer(renderer) || IsImportedVehicleGroundShadow(renderer, model.transform))
			{
				renderer.enabled = false;
			}
		}
	}

	private bool IsImportedVehicleGroundShadow(Renderer renderer, Transform modelRoot)
	{
		if (renderer == null || modelRoot == null)
		{
			return false;
		}
		bool namedShadow = RendererTextContains(renderer.name, "shadow") || RendererTextContains(renderer.gameObject.name, "shadow");
		bool planeArtifact = IsNamedImportedVehiclePlaneArtifact(renderer);
		if (!namedShadow && !planeArtifact)
		{
			return false;
		}
		Bounds bounds = renderer.bounds;
		Vector3 targetSize = ActiveVehicle.TargetModelSize;
		bool wideEnough = bounds.size.x >= targetSize.x * 0.68f && bounds.size.z >= targetSize.z * 0.68f;
		bool flatEnough = bounds.size.y <= targetSize.y * 0.12f;
		Transform space = modelRoot.parent != null ? modelRoot.parent : modelRoot;
		Vector3 localCenter = space.InverseTransformPoint(bounds.center);
		bool lowEnough = localCenter.y <= ActiveVehicle.RoadYOffset + targetSize.y * 0.18f;
		return wideEnough && flatEnough && lowEnough;
	}

	private bool ShouldHideImportedVehicleRenderer(Renderer renderer)
	{
		if (renderer == null || !ActiveVehicle.HideOversizedPlaneRenderers)
		{
			return false;
		}
		if (IsNamedImportedVehiclePlaneArtifact(renderer))
		{
			return true;
		}
		Bounds bounds = renderer.bounds;
		float largest = Mathf.Max(new float[3] { bounds.size.x, bounds.size.y, bounds.size.z });
		float smallest = Mathf.Min(new float[3] { bounds.size.x, bounds.size.y, bounds.size.z });
		float ratio = smallest > 0.001f ? largest / Mathf.Max(0.001f, smallest) : largest;
		if (largest > 3.2f)
		{
			return ratio > 24f;
		}
		return false;
	}

	private bool IsNamedImportedVehiclePlaneArtifact(Renderer renderer)
	{
		if (renderer == null)
		{
			return false;
		}
		if (IsPlaneArtifactName(renderer.name) || IsPlaneArtifactName(renderer.gameObject.name))
		{
			return true;
		}
		Transform parent = renderer.transform.parent;
		while (parent != null)
		{
			if (IsPlaneArtifactName(parent.name))
			{
				return true;
			}
			parent = parent.parent;
		}
		return false;
	}

	private bool IsPlaneArtifactName(string objectName)
	{
		if (string.IsNullOrEmpty(objectName))
		{
			return false;
		}
		if (string.Equals(objectName, "Plane", StringComparison.OrdinalIgnoreCase) || objectName.StartsWith("Plane.", StringComparison.OrdinalIgnoreCase) || objectName.StartsWith("Plane_", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return objectName.StartsWith("Plane ", StringComparison.OrdinalIgnoreCase);
	}

	private void FitImportedVehicleModel(Transform model, Transform localRoot, float targetWidth, float targetLength, float targetHeight, string[] rendererNameKeywords = null)
	{
		if (!TryGetActiveRendererBounds(model, out Bounds bounds, rendererNameKeywords))
		{
			return;
		}
		float scaleX = bounds.size.x > 0.001f ? targetWidth / bounds.size.x : 1f;
		float scaleZ = bounds.size.z > 0.001f ? targetLength / bounds.size.z : 1f;
		float scaleY = bounds.size.y > 0.001f ? targetHeight / bounds.size.y : 1f;
		float scale = Mathf.Min(new float[3] { scaleX, scaleZ, scaleY });
		model.localScale = model.localScale * scale;
		if (!TryGetActiveRendererBounds(model, out bounds, rendererNameKeywords))
		{
			return;
		}
		Transform space = localRoot != null ? localRoot : model;
		Vector3 center = space.InverseTransformPoint(bounds.center);
		Vector3 min = space.InverseTransformPoint(bounds.min);
		model.localPosition = model.localPosition - new Vector3(center.x, min.y, center.z);
	}

	private Quaternion VehicleModelBaseRotation()
	{
		return Quaternion.Euler(0f, VehicleModelYawOffset, 0f);
	}

	private Quaternion PlayerVehicleModelBaseRotation()
	{
		return Quaternion.Euler(0f, playerVehicleYawOffset, 0f);
	}

	private Quaternion PlayerVehicleVisualRotation(float yawOffset, float rollOffset)
	{
		return Quaternion.Euler(0f, playerVehicleYawOffset + yawOffset, rollOffset);
	}

	private float CurrentPlayerVehicleRoadYOffset()
	{
		return ActiveVehicle.RoadYOffset;
	}

	private void CachePlayerWheelMeshes(Transform model)
	{
		ClearPlayerWheelMeshCache();
		if (model == null)
		{
			return;
		}
		Transform leftWheel = null;
		Transform rightWheel = null;
		Transform leftDisc = null;
		Transform rightDisc = null;
		Transform[] nodes = model.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < nodes.Length; i++)
		{
			Transform node = nodes[i];
			if (node == null)
			{
				continue;
			}
			string name = node.name.ToLowerInvariant();
			string[] wheelNames = ActiveVehicle.WheelMeshNames;
			string[] discNames = ActiveVehicle.BrakeDiscNames;
			if (wheelNames != null && wheelNames.Length != 0 && string.Equals(name, wheelNames[0], StringComparison.OrdinalIgnoreCase))
			{
				leftWheel = node;
			}
			else if (wheelNames != null && wheelNames.Length > 1 && string.Equals(name, wheelNames[1], StringComparison.OrdinalIgnoreCase))
			{
				rightWheel = node;
			}
			else if (discNames != null && discNames.Length != 0 && string.Equals(name, discNames[0], StringComparison.OrdinalIgnoreCase))
			{
				leftDisc = node;
			}
			else if (discNames != null && discNames.Length > 1 && string.Equals(name, discNames[1], StringComparison.OrdinalIgnoreCase))
			{
				rightDisc = node;
			}
		}
		playerWheelMeshes[0] = leftWheel;
		playerWheelMeshes[1] = rightWheel;
		CachePlayerWheelBaseRotations();
		CachePlayerWheelSpinRate();
		AttachDiscToNearestWheel(leftDisc);
		AttachDiscToNearestWheel(rightDisc);
	}

	private void ClearPlayerWheelMeshCache()
	{
		for (int i = 0; i < playerWheelMeshes.Length; i++)
		{
			playerWheelMeshes[i] = null;
			playerWheelMeshBaseRotations[i] = Quaternion.identity;
		}
		playerWheelSpinDegreesPerMeter = 120f;
	}

	private void CachePlayerWheelBaseRotations()
	{
		for (int i = 0; i < playerWheelMeshes.Length; i++)
		{
			playerWheelMeshBaseRotations[i] = playerWheelMeshes[i] != null ? playerWheelMeshes[i].localRotation : Quaternion.identity;
		}
	}

	private void CachePlayerWheelSpinRate()
	{
		float radiusSum = 0f;
		int count = 0;
		for (int i = 0; i < playerWheelMeshes.Length; i++)
		{
			Transform wheel = playerWheelMeshes[i];
			if (wheel == null || !TryGetActiveRendererBounds(wheel, out Bounds bounds))
			{
				continue;
			}
			float radius = Mathf.Max(bounds.size.y, bounds.size.z) * 0.5f;
			if (radius <= 0.001f)
			{
				continue;
			}
			radiusSum += radius;
			count++;
		}
		if (count == 0)
		{
			return;
		}
		float averageRadius = radiusSum / count;
		playerWheelSpinDegreesPerMeter = 360f / (Mathf.PI * 2f * averageRadius);
	}

	private void AttachDiscToNearestWheel(Transform disc)
	{
		if (disc == null || (playerWheelMeshes[0] == null && playerWheelMeshes[1] == null))
		{
			return;
		}
		Transform nearest = null;
		float nearestDistance = float.PositiveInfinity;
		for (int i = 0; i < playerWheelMeshes.Length; i++)
		{
			Transform wheel = playerWheelMeshes[i];
			if (wheel == null)
			{
				continue;
			}
			float distance = (disc.position - wheel.position).sqrMagnitude;
			if (distance < nearestDistance)
			{
				nearestDistance = distance;
				nearest = wheel;
			}
		}
		if (nearest != null)
		{
			disc.SetParent(nearest, true);
		}
	}

	private void SetPlayerWheelSpin(float degrees)
	{
		Quaternion spin = Quaternion.Euler(degrees, 0f, 0f);
		for (int i = 0; i < playerWheelMeshes.Length; i++)
		{
			if (playerWheelMeshes[i] == null)
			{
				continue;
			}
			playerWheelMeshes[i].localRotation = playerWheelMeshBaseRotations[i] * spin;
		}
	}

	private void CachePlayerBrakeLights(Transform model)
	{
		ClearP900BrakeLightMaterial();
		playerBrakeLightRenderers.Clear();
		playerBrakeLightMaterialIndices.Clear();
		playerBrakeLightsOn = false;
		if (model == null)
		{
			return;
		}
		bool isP900 = string.Equals(ActiveVehicle.Id, "infal_model", StringComparison.OrdinalIgnoreCase);
		bool matchMaterials = ActiveVehicle.BrakeLightMaterialNames != null && ActiveVehicle.BrakeLightMaterialNames.Length > 0;
		Transform[] nodes = model.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < nodes.Length; i++)
		{
			Transform node = nodes[i];
			if (node == null || !IsPlayerBrakeLightName(node.name))
			{
				continue;
			}
			Renderer renderer = node.GetComponent<Renderer>();
			if (renderer == null)
			{
				renderer = node.GetComponentInChildren<Renderer>(true);
			}
			if (renderer == null)
			{
				continue;
			}
			Material[] materials = renderer.sharedMaterials;
			if (matchMaterials)
			{
				for (int m = 0; m < materials.Length; m++)
				{
					Material material = materials[m];
					if (material == null || !VehicleNodeNameMatches(material.name, ActiveVehicle.BrakeLightMaterialNames))
					{
						continue;
					}
					if (isP900)
					{
						Material lens = GetOrCreateP900BrakeLightMaterial();
						if (lens == null)
						{
							continue;
						}
						materials[m] = lens;
						renderer.sharedMaterials = materials;
					}
					else
					{
						material.EnableKeyword("_EMISSION");
					}
					playerBrakeLightRenderers.Add(renderer);
					playerBrakeLightMaterialIndices.Add(m);
				}
				continue;
			}
			for (int m = 0; m < materials.Length; m++)
			{
				if (materials[m] != null)
				{
					materials[m].EnableKeyword("_EMISSION");
				}
			}
			playerBrakeLightRenderers.Add(renderer);
			playerBrakeLightMaterialIndices.Add(-1);
		}
	}

	private Material GetOrCreateP900BrakeLightMaterial()
	{
		if (p900BrakeLightMaterial != null)
		{
			return p900BrakeLightMaterial;
		}
		Shader shader = Shader.Find("ShadowRun/TrafficLampUnlit");
		if (shader == null)
		{
			Debug.LogError("Required shader ShadowRun/TrafficLampUnlit was not included in the build.");
			return null;
		}
		p900BrakeLightMaterial = new Material(shader);
		p900BrakeLightMaterial.name = "P900 Brake Lens Runtime";
		p900BrakeLightMaterial.hideFlags = HideFlags.HideAndDontSave;
		p900BrakeLightMaterial.enableInstancing = true;
		p900BrakeLightMaterial.SetColor("_BaseColor", new Color(0.075f, 0.003f, 0.002f, 1f));
		return p900BrakeLightMaterial;
	}

	private void ClearP900BrakeLightMaterial()
	{
		if (p900BrakeLightMaterial == null)
		{
			return;
		}
		Destroy(p900BrakeLightMaterial);
		p900BrakeLightMaterial = null;
	}

	private bool IsPlayerBrakeLightName(string name)
	{
		return VehicleNodeNameMatches(name, ActiveVehicle.BrakeLightNames);
	}

	private void SetPlayerBrakeLights(bool on)
	{
		if (playerBrakeLightsOn == on)
		{
			return;
		}
		playerBrakeLightsOn = on;
		if (playerBrakeLightBlock == null)
		{
			playerBrakeLightBlock = new MaterialPropertyBlock();
		}
		bool isP900 = string.Equals(ActiveVehicle.Id, "infal_model", StringComparison.OrdinalIgnoreCase);
		bool brightLens = isP900 || string.Equals(ActiveVehicle.Id, "sportcar", StringComparison.OrdinalIgnoreCase);
		Color baseColor = isP900 ? new Color(5.8f, 0.035f, 0.012f, 1f) : (brightLens ? new Color(0.86f, 0.018f, 0.025f, 1f) : new Color(0.3f, 0.008f, 0.014f, 1f));
		Color emission = brightLens ? new Color(1.15f, 0.018f, 0.012f, 1f) * 4.4f : new Color(0.72f, 0.018f, 0.012f, 1f);
		for (int i = 0; i < playerBrakeLightRenderers.Count; i++)
		{
			Renderer renderer = playerBrakeLightRenderers[i];
			if (renderer == null)
			{
				continue;
			}
			int materialIndex = i < playerBrakeLightMaterialIndices.Count ? playerBrakeLightMaterialIndices[i] : -1;
			if (!on)
			{
				if (materialIndex >= 0)
				{
					renderer.SetPropertyBlock(null, materialIndex);
				}
				else
				{
					renderer.SetPropertyBlock(null);
				}
				continue;
			}
			playerBrakeLightBlock.Clear();
			playerBrakeLightBlock.SetColor("_BaseColor", baseColor);
			playerBrakeLightBlock.SetColor("_Color", baseColor);
			playerBrakeLightBlock.SetColor("_EmissionColor", emission);
			if (materialIndex >= 0)
			{
				renderer.SetPropertyBlock(playerBrakeLightBlock, materialIndex);
			}
			else
			{
				renderer.SetPropertyBlock(playerBrakeLightBlock);
			}
		}
	}

	private void MakeHighBeamLight()
	{
		if (player == null)
		{
			return;
		}
		highBeamTrafficMask = 1 << HighBeamTrafficLayer;
		highBeamOrigin = new GameObject("High Beam Origin").transform;
		highBeamOrigin.SetParent(player, false);
		highBeamOrigin.localPosition = new Vector3(0f, CurrentHighBeamOriginHeight(), -VehicleHalfLength + CurrentHighBeamFrontInset());
		highBeamOrigin.localRotation = Quaternion.Euler(0f, 180f, 0f);
		highBeamLeftEmitter = CreateHeadlightEmitter("Left High Beam Emitter", -CurrentHighBeamEmitterWidth());
		highBeamRightEmitter = CreateHeadlightEmitter("Right High Beam Emitter", CurrentHighBeamEmitterWidth());
		MakeHighBeamVisual();
	}

	private Transform CreateHeadlightEmitter(string name, float x)
	{
		Transform emitter = new GameObject(name).transform;
		emitter.SetParent(highBeamOrigin, false);
		emitter.localPosition = new Vector3(x, CurrentHighBeamEmitterHeightOffset(), 0f);
		emitter.localRotation = Quaternion.identity;
		return emitter;
	}

	private float CurrentHighBeamOriginHeight()
	{
		return ActiveVehicle.HeadlightOriginHeight;
	}

	private float CurrentHighBeamFrontInset()
	{
		return ActiveVehicle.HeadlightFrontInset;
	}

	private float CurrentHighBeamEmitterWidth()
	{
		return ActiveVehicle.HeadlightEmitterWidth;
	}

	private float CurrentHighBeamEmitterHeightOffset()
	{
		return ActiveVehicle.HeadlightEmitterHeightOffset;
	}

	private float CurrentHighBeamLeftShaftLocalZ()
	{
		return ActiveVehicle.HeadlightLeftShaftLocalZ;
	}

	private float CurrentHighBeamRightShaftLocalZ()
	{
		return ActiveVehicle.HeadlightRightShaftLocalZ;
	}

	private void MakeHighBeamVisual()
	{
		if (highBeamLeftEmitter == null || highBeamRightEmitter == null)
		{
			return;
		}
		highBeamPrefab = Resources.Load<GameObject>(HighBeamPrefabResource);
		if (highBeamPrefab == null)
		{
			Debug.LogError("High beam prefab not found in Resources: FakeVolumetricLight-Unity-main/VolumatricSpotlight/Prefabs/VolumetricSpotLight_Cone");
			return;
		}
		highBeamLeftPrefabInstance = CreateHighBeamPrefabInstance(highBeamLeftEmitter, "Left High Beam Prefab", CurrentHighBeamLeftShaftLocalZ());
		highBeamRightPrefabInstance = CreateHighBeamPrefabInstance(highBeamRightEmitter, "Right High Beam Prefab", CurrentHighBeamRightShaftLocalZ());
		SetHighBeamVisual(false);
	}

	private GameObject CreateHighBeamPrefabInstance(Transform parent, string name, float shaftLocalZ)
	{
		GameObject instance = Instantiate(highBeamPrefab);
		instance.name = name;
		instance.transform.SetParent(parent, false);
		instance.transform.localPosition = new Vector3(0f, HighBeamVisualHeightOffset, 0f);
		instance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
		instance.transform.localScale = new Vector3(1f, 2f, 1f);
		SetHighBeamShaftLocalZ(instance, shaftLocalZ);
		ConfigureHighBeamPrefab(instance);
		instance.SetActive(false);
		return instance;
	}

	private void SetHighBeamShaftLocalZ(GameObject instance, float localZ)
	{
		Transform[] nodes = instance.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < nodes.Length; i++)
		{
			Transform node = nodes[i];
			if (node == null || node.name != "LightShaftCone")
			{
				continue;
			}
			Vector3 localPosition = node.localPosition;
			localPosition.x = ActiveVehicle.Id == "flying_car" ? 1.08f : 0f;
			localPosition.z = localZ;
			node.localPosition = localPosition;
			return;
		}
	}

	private void ConfigureHighBeamPrefab(GameObject instance)
	{
		Light[] lights = instance.GetComponentsInChildren<Light>(true);
		for (int i = 0; i < lights.Length; i++)
		{
			Light light = lights[i];
			light.enabled = true;
			light.type = LightType.Spot;
			light.shadows = LightShadows.None;
			light.range = InfernoDrivingRules.HighBeamRange; /* [InfernoLobby patch] */
			light.spotAngle = 30f;
			light.innerSpotAngle = 16f;
			light.color = new Color(0.72f, 0.95f, 1f);
			light.renderMode = LightRenderMode.ForcePixel;
		}
		Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			renderer.lightProbeUsage = LightProbeUsage.Off;
			renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		}
	}
}
