using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using DynamicWeatherSystem;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_EnsurePulsarNightObject, _EnsurePulsarNightCamera,
// _SetPulsarNightCameraActive, _CreatePulsarNightStar, _PreparePulsarNightInstance, _UpdatePulsarNight,
// _RestartBluePulsarRays, _AnimatePulsarNightSystem, _RotateViewportEllipse, _PositionPulsarNightStar,
// _PositionPulsarNightSatelliteTilted, _SetPulsarNightStarTransition, _EnsureLightningFlash, _UpdateLightningFlash,
// _EnsureRainStormSun, _CreateRainStormSunDisc, _MakeRainStormSunTexture, _GetStableRainStormSunLocalPosition,
// _UpdateRainStormSun, _EnsureFallenSkyFallingStars, _UpdateFallenSkyFallingStars.
public sealed partial class ShadowRunPrototype
{
	private void EnsurePulsarNightObject()
	{
		if (pulsarNightRoot != null || cam == null)
		{
			return;
		}
		EnsurePulsarNightCamera();
		pulsarNightRoot = new GameObject("Pulsar Night Camera FX").transform;
		pulsarNightRoot.SetParent(cam.transform, false);
		pulsarNightRoot.localPosition = Vector3.zero;
		pulsarNightRoot.localRotation = Quaternion.identity;
		pulsarNightRoot.localScale = Vector3.one;
		pulsarNightBlueInstance = CreatePulsarNightStar(PulsarNightBluePrefabResource, "Runtime StarCore Blue", Vector3.zero, PulsarNightBlueScale, true);
		pulsarNightRedInstance = CreatePulsarNightStar(PulsarNightRedPrefabResource, "Runtime StarCore Red", Vector3.zero, PulsarNightRedScale, false);
		pulsarNightRoot.gameObject.SetActive(false);
	}

	private void EnsurePulsarNightCamera()
	{
		if (cam == null)
		{
			return;
		}
		int mask = LayerMask.GetMask(new string[1] { "PulsarNightFX" });
		if (mask == 0)
		{
			mask = 1 << PulsarNightFxLayer;
		}
		bool created = false;
		if (pulsarNightCamera == null)
		{
			GameObject cameraObject = new GameObject("Pulsar Night Overlay Camera");
			cameraObject.transform.SetParent(cam.transform, false);
			cameraObject.transform.localPosition = Vector3.zero;
			cameraObject.transform.localRotation = Quaternion.identity;
			cameraObject.transform.localScale = Vector3.one;
			pulsarNightCamera = cameraObject.AddComponent<Camera>();
			created = true;
		}
		pulsarNightCamera.clearFlags = CameraClearFlags.Depth;
		pulsarNightCamera.fieldOfView = cam.fieldOfView;
		pulsarNightCamera.aspect = cam.aspect;
		pulsarNightCamera.cullingMask = mask;
		pulsarNightCamera.allowHDR = true;
		pulsarNightCamera.allowMSAA = cam.allowMSAA;
		pulsarNightCamera.nearClipPlane = 0.05f;
		pulsarNightCamera.farClipPlane = cam.farClipPlane;
		pulsarNightCamera.depth = cam.depth + 2f;
		UniversalAdditionalCameraData overlayData = pulsarNightCamera.GetComponent<UniversalAdditionalCameraData>();
		if (overlayData == null)
		{
			overlayData = pulsarNightCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();
		}
		overlayData.renderType = CameraRenderType.Overlay;
		overlayData.renderPostProcessing = true;
		UniversalAdditionalCameraData baseData = cam.GetComponent<UniversalAdditionalCameraData>();
		if (baseData == null)
		{
			baseData = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
		}
		if (created)
		{
			baseData.cameraStack.Remove(pulsarNightCamera);
			pulsarNightCamera.enabled = false;
			pulsarNightCamera.gameObject.SetActive(false);
		}
		cam.cullingMask = cam.cullingMask & ~mask;
	}

	private void SetPulsarNightCameraActive(bool active)
	{
		EnsurePulsarNightCamera();
		if (cam == null || pulsarNightCamera == null)
		{
			return;
		}
		UniversalAdditionalCameraData baseData = cam.GetComponent<UniversalAdditionalCameraData>();
		if (baseData == null)
		{
			baseData = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
		}
		if (active)
		{
			pulsarNightCamera.gameObject.SetActive(true);
			pulsarNightCamera.enabled = true;
			if (!baseData.cameraStack.Contains(pulsarNightCamera))
			{
				baseData.cameraStack.Add(pulsarNightCamera);
			}
			return;
		}
		baseData.cameraStack.Remove(pulsarNightCamera);
		pulsarNightCamera.enabled = false;
		pulsarNightCamera.gameObject.SetActive(false);
	}

	private GameObject CreatePulsarNightStar(string resource, string objectName, Vector3 localPosition, float scale, bool blue)
	{
		GameObject prefab = Resources.Load<GameObject>(resource);
		if (prefab == null || pulsarNightRoot == null)
		{
			return null;
		}
		GameObject instance = Instantiate(prefab, pulsarNightRoot);
		instance.name = objectName;
		instance.transform.localPosition = localPosition;
		instance.transform.localRotation = Quaternion.identity;
		instance.transform.localScale = Vector3.one * scale;
		SetLayerRecursively(instance, PulsarNightFxLayer);
		PreparePulsarNightInstance(instance, blue);
		return instance;
	}

	private void PreparePulsarNightInstance(GameObject instance, bool blue)
	{
		if (instance == null)
		{
			return;
		}
		Collider[] colliders = instance.GetComponentsInChildren<Collider>(true);
		for (int i = 0; i < colliders.Length; i++)
		{
			Destroy(colliders[i]);
		}
		Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
		Material[][] materials = new Material[renderers.Length][];
		for (int r = 0; r < renderers.Length; r++)
		{
			Renderer renderer = renderers[r];
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			renderer.lightProbeUsage = LightProbeUsage.Off;
			renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			Material[] instanceMaterials = renderer.materials;
			materials[r] = instanceMaterials;
			for (int m = 0; m < instanceMaterials.Length; m++)
			{
				Material material = instanceMaterials[m];
				if (material == null)
				{
					continue;
				}
				if (material.HasProperty("_NoiseScale"))
				{
					material.SetFloat("_NoiseScale", 9.86f);
				}
				if (material.HasProperty("_NoisePow"))
				{
					material.SetFloat("_NoisePow", 5.18f);
				}
				if (material.HasProperty("_Animation_Speed"))
				{
					material.SetFloat("_Animation_Speed", blue ? 1f : 0.78f);
				}
				if (material.HasProperty("_RimBrightness"))
				{
					material.SetFloat("_RimBrightness", blue ? 20f : 24f);
				}
				if (material.HasProperty("_FinalBrightness"))
				{
					material.SetFloat("_FinalBrightness", 0.5f);
				}
				if (material.HasProperty("_BlinkPeriod"))
				{
					material.SetFloat("_BlinkPeriod", blue ? 2f : 2.9f);
				}
			}
		}
		if (blue)
		{
			pulsarNightBlueRenderers = renderers;
			pulsarNightBlueMaterials = materials;
		}
		else
		{
			pulsarNightRedRenderers = renderers;
			pulsarNightRedMaterials = materials;
		}
		if (blue)
		{
			VisualEffect[] effects = instance.GetComponentsInChildren<VisualEffect>(true);
			for (int i = 0; i < effects.Length; i++)
			{
				effects[i].enabled = true;
				effects[i].Reinit();
				effects[i].Play();
			}
		}
	}

	private void UpdatePulsarNight(float dt)
	{
		EnsurePulsarNightObject();
		WeatherStateData current = weatherManager != null ? weatherManager.CurrentState : null;
		float target = IsWeatherState(current, "Planets") ? 1f : 0f;
		if (TryGetPlanetsTransition(out bool enteringPlanets, out float progress))
		{
			target = enteringPlanets ? SmoothWeatherPhase(progress, 0.34f, 0.94f) : 1f - SmoothWeatherPhase(progress, 0.02f, 0.52f);
			pulsarNightVisibility = target;
		}
		else
		{
			float rate = target > pulsarNightVisibility ? 0.55f : 1.25f;
			pulsarNightVisibility = dt > 0f ? Mathf.MoveTowards(pulsarNightVisibility, target, rate * dt) : target;
		}
		if (pulsarNightRoot == null)
		{
			return;
		}
		bool visible = pulsarNightVisibility > 0.001f;
		if (pulsarNightRoot.gameObject.activeSelf != visible)
		{
			pulsarNightRoot.gameObject.SetActive(visible);
			SetPulsarNightCameraActive(visible);
			if (visible)
			{
				RestartBluePulsarRays();
			}
		}
		if (!visible)
		{
			return;
		}
		float eased = Mathf.SmoothStep(0f, 1f, pulsarNightVisibility);
		pulsarNightRoot.localScale = Vector3.one;
		AnimatePulsarNightSystem(eased);
	}

	private void RestartBluePulsarRays()
	{
		if (pulsarNightBlueInstance == null)
		{
			return;
		}
		VisualEffect[] effects = pulsarNightBlueInstance.GetComponentsInChildren<VisualEffect>(true);
		for (int i = 0; i < effects.Length; i++)
		{
			effects[i].enabled = true;
			effects[i].Reinit();
			effects[i].Play();
		}
	}

	private void AnimatePulsarNightSystem(float visibility)
	{
		if (cam == null)
		{
			return;
		}
		float time = Time.time;
		float orbit = time * 0.5775f;
		Vector2 offset = RotateViewportEllipse(orbit, 0.086f, 0.036f, 15f);
		Vector2 viewport = new Vector2(0.505f, 0.805f) + offset;
		float satelliteAngle = time * 2.4f + 1.1f;
		PositionPulsarNightStar(pulsarNightBlueInstance, viewport, PulsarNightBlueDepth);
		PositionPulsarNightSatelliteTilted(satelliteAngle);
		if (pulsarNightBlueInstance != null)
		{
			pulsarNightBlueInstance.transform.localRotation = Quaternion.Euler(time * 34f * PulsarNightPrefabSpinMultiplier, time * 58f * PulsarNightPrefabSpinMultiplier, time * 21f * PulsarNightPrefabSpinMultiplier);
		}
		if (pulsarNightRedInstance != null)
		{
			pulsarNightRedInstance.transform.localRotation = Quaternion.Euler(-time * 52f * PulsarNightPrefabSpinMultiplier, time * 96f * PulsarNightPrefabSpinMultiplier, -time * 37f * PulsarNightPrefabSpinMultiplier);
		}
		SetPulsarNightStarTransition(pulsarNightBlueInstance, pulsarNightBlueRenderers, pulsarNightBlueMaterials, visibility, PulsarNightBlueScale, 20f);
		SetPulsarNightStarTransition(pulsarNightRedInstance, pulsarNightRedRenderers, pulsarNightRedMaterials, visibility, PulsarNightRedScale, 24f);
	}

	private Vector2 RotateViewportEllipse(float angle, float radiusX, float radiusY, float rotationDegrees)
	{
		float x = Mathf.Cos(angle) * radiusX;
		float y = Mathf.Sin(angle) * radiusY;
		float cos = Mathf.Cos(rotationDegrees * Mathf.Deg2Rad);
		float sin = Mathf.Sin(rotationDegrees * Mathf.Deg2Rad);
		return new Vector2(x * cos - y * sin, x * sin + y * cos);
	}

	private void PositionPulsarNightStar(GameObject star, Vector2 viewportPosition, float depth)
	{
		if (star == null || cam == null)
		{
			return;
		}
		Vector3 world = cam.ViewportToWorldPoint(new Vector3(viewportPosition.x, viewportPosition.y, depth));
		star.transform.localPosition = cam.transform.InverseTransformPoint(world);
	}

	private void PositionPulsarNightSatelliteTilted(float angle)
	{
		if (pulsarNightBlueInstance == null || pulsarNightRedInstance == null)
		{
			return;
		}
		Vector3 center = pulsarNightBlueInstance.transform.localPosition;
		float cos = Mathf.Cos(angle);
		float sin = Mathf.Sin(angle);
		pulsarNightRedInstance.transform.localPosition = center + new Vector3(cos * 1.12f, cos * 0.13f + sin * 0.24f, sin * 1.2f);
	}

	private void SetPulsarNightStarTransition(GameObject star, Renderer[] renderers, Material[][] rendererMaterials, float visibility, float fullScale, float fullRimBrightness)
	{
		if (star == null)
		{
			return;
		}
		float eased = visibility * visibility * (3f - 2f * visibility);
		float scale = fullScale * Mathf.Lerp(0.015f, 1f, eased);
		star.transform.localScale = Vector3.one * scale;
		bool visible = visibility > 0.0005f;
		if (renderers == null || rendererMaterials == null)
		{
			return;
		}
		for (int r = 0; r < renderers.Length; r++)
		{
			Renderer renderer = renderers[r];
			if (renderer == null)
			{
				continue;
			}
			renderer.enabled = visible;
			Material[] materials = r < rendererMaterials.Length ? rendererMaterials[r] : null;
			if (materials == null)
			{
				continue;
			}
			for (int m = 0; m < materials.Length; m++)
			{
				Material material = materials[m];
				if (material == null)
				{
					continue;
				}
				if (material.HasProperty("_FinalBrightness"))
				{
					material.SetFloat("_FinalBrightness", 0.5f * eased);
				}
				if (material.HasProperty("_RimBrightness"))
				{
					material.SetFloat("_RimBrightness", fullRimBrightness * eased);
				}
			}
		}
	}

	private void EnsureLightningFlash()
	{
		if (lightningFlash != null || cam == null)
		{
			return;
		}
		Texture2D texture = Resources.Load<Texture2D>("молния1");
		if (texture == null)
		{
			return;
		}
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		lightningFlashMaterial = MatTransparentUnlit(Color.white, texture, true);
		lightningFlashMaterial.name = "Runtime Lightning Flash";
		lightningFlashMaterial.renderQueue = 3040;
		GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
		quad.name = "Lightning Flash";
		quad.transform.SetParent(cam.transform, false);
		quad.transform.localPosition = new Vector3(0f, 24f, 160f);
		quad.transform.localRotation = Quaternion.identity;
		quad.transform.localScale = new Vector3(142f, 94.67f, 1f);
		PreparePrimitive(quad, lightningFlashMaterial);
		lightningFlash = quad.transform;
		lightningFlash.gameObject.SetActive(false);
	}

	private void UpdateLightningFlash(float dt)
	{
		EnsureLightningFlash();
		if (lightningFlash == null)
		{
			return;
		}
		bool storming = weatherManager != null && IsWeatherState(weatherManager.CurrentState, "Storm") && weatherManager.CurrentState.rainIntensity > 0.85f;
		if (!storming)
		{
			lightningTimer = LightningInterval;
			lightningFlashTime = 0f;
			if (lightningFlash.gameObject.activeSelf)
			{
				lightningFlash.gameObject.SetActive(false);
			}
			return;
		}
		lightningTimer -= dt;
		if (lightningTimer <= 0f)
		{
			lightningTimer += LightningInterval;
			lightningFlashTime = LightningFlashDuration;
			lightningFlash.gameObject.SetActive(true);
		}
		if (lightningFlashTime <= 0f)
		{
			return;
		}
		lightningFlashTime = Mathf.Max(0f, lightningFlashTime - dt);
		float alpha = lightningFlashTime / LightningFlashDuration;
		alpha *= alpha;
		SetMaterialColor(lightningFlashMaterial, new Color(1f, 1f, 1f, alpha));
		if (lightningFlashTime <= 0f)
		{
			lightningFlash.gameObject.SetActive(false);
		}
	}

	private void EnsureRainStormSun()
	{
		if (rainStormSunRoot != null || cam == null)
		{
			return;
		}
		rainStormSunHaloMat = MatTransparentUnlit(new Color(1f, 1f, 1f, 0f), MakeRainStormSunTexture(192, 0.03f, 4.4f), true);
		rainStormSunHaloMat.name = "Runtime Rain Storm Sun Halo";
		rainStormSunHaloMat.renderQueue = 2504;
		rainStormSunGlowMat = MatTransparentUnlit(new Color(1f, 1f, 1f, 0f), MakeRainStormSunTexture(192, 0.24f, 3.6f), true);
		rainStormSunGlowMat.name = "Runtime Rain Storm Sun Glow";
		rainStormSunGlowMat.renderQueue = 2505;
		rainStormSunCoreMat = MatTransparentUnlit(new Color(1f, 1f, 1f, 0f), MakeRainStormSunTexture(256, 0.92f, 6.5f), false);
		rainStormSunCoreMat.name = "Runtime Rain Storm Sun Core";
		rainStormSunCoreMat.renderQueue = 2506;
		rainStormSunRoot = new GameObject("Rain Storm 2D Sun").transform;
		rainStormSunRoot.SetParent(cam.transform, false);
		rainStormSunRoot.localPosition = GetStableRainStormSunLocalPosition();
		rainStormSunRoot.localRotation = Quaternion.identity;
		rainStormSunRoot.localScale = Vector3.one;
		GameObject halo = CreateRainStormSunDisc("Rain Storm Sun Outer Halo", RainStormSunHaloScale, rainStormSunHaloMat, 40);
		halo.transform.SetParent(rainStormSunRoot, false);
		halo.transform.localPosition = new Vector3(0f, 0f, 0.04f);
		rainStormSunHaloRenderer = halo.GetComponent<Renderer>();
		GameObject glow = CreateRainStormSunDisc("Rain Storm Sun Soft Glow", RainStormSunGlowScale, rainStormSunGlowMat, 48);
		glow.transform.SetParent(rainStormSunRoot, false);
		glow.transform.localPosition = Vector3.zero;
		rainStormSunGlowRenderer = glow.GetComponent<Renderer>();
		GameObject core = CreateRainStormSunDisc("Rain Storm Sun White Core", RainStormSunCoreScale, rainStormSunCoreMat, 64);
		core.transform.SetParent(rainStormSunRoot, false);
		core.transform.localPosition = new Vector3(0f, 0f, -0.02f);
		rainStormSunCoreRenderer = core.GetComponent<Renderer>();
		rainStormSunRoot.gameObject.SetActive(false);
	}

	private GameObject CreateRainStormSunDisc(string objectName, Vector3 localScale, Material material, int segments)
	{
		GameObject disc = new GameObject(objectName);
		disc.transform.localRotation = Quaternion.identity;
		disc.transform.localScale = localScale;
		int count = Mathf.Max(12, segments);
		Vector3[] vertices = new Vector3[count + 1];
		Vector2[] uvs = new Vector2[count + 1];
		int[] triangles = new int[count * 3];
		vertices[0] = Vector3.zero;
		uvs[0] = new Vector2(0.5f, 0.5f);
		for (int i = 0; i < count; i++)
		{
			float angle = (float)i / count * Mathf.PI * 2f;
			float x = Mathf.Cos(angle) * 0.5f;
			float y = Mathf.Sin(angle) * 0.5f;
			vertices[i + 1] = new Vector3(x, y, 0f);
			uvs[i + 1] = new Vector2(x + 0.5f, y + 0.5f);
			int t = i * 3;
			triangles[t] = 0;
			triangles[t + 1] = i + 1;
			triangles[t + 2] = i == count - 1 ? 1 : i + 2;
		}
		Mesh mesh = new Mesh();
		mesh.name = objectName + " Mesh";
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		disc.AddComponent<MeshFilter>().sharedMesh = mesh;
		disc.AddComponent<MeshRenderer>();
		PreparePrimitive(disc, material);
		return disc;
	}

	private Texture2D MakeRainStormSunTexture(int size, float solidRadius, float falloffPower)
	{
		Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false, true);
		texture.name = "Runtime Rain Storm Soft White Sun";
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		Vector2 center = new Vector2((size - 1f) * 0.5f, (size - 1f) * 0.5f);
		float radius = Mathf.Max(1f, (size - 1f) * 0.5f);
		solidRadius = Mathf.Clamp01(solidRadius);
		for (int y = 0; y < size; y++)
		{
			for (int x = 0; x < size; x++)
			{
				float distance = Vector2.Distance(new Vector2(x, y), center) / radius;
				float alpha = distance <= solidRadius ? 1f : Mathf.Pow(Mathf.Clamp01(1f - Mathf.InverseLerp(solidRadius, 1f, distance)), falloffPower);
				texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
			}
		}
		texture.Apply(false, true);
		return texture;
	}

	private Vector3 GetStableRainStormSunLocalPosition()
	{
		if (cam == null)
		{
			return RainStormSunLocalPosition;
		}
		float currentTan = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f);
		float referenceTan = Mathf.Tan(0.567232013f);
		float scale = referenceTan > 0.001f ? currentTan / referenceTan : 1f;
		return new Vector3(RainStormSunLocalPosition.x * scale, RainStormSunLocalPosition.y * scale, RainStormSunLocalPosition.z);
	}

	private void UpdateRainStormSun(float dt)
	{
		EnsureRainStormSun();
		if (rainStormSunRoot == null)
		{
			return;
		}
		WeatherStateData current = weatherManager != null ? weatherManager.CurrentState : null;
		float target = IsWeatherState(current, "Storm") ? 1f : 0f;
		if (weatherManager != null && weatherManager.IsTransitioning)
		{
			bool fromStorm = IsWeatherState(visualTransitionFromState, "Storm");
			bool toStorm = IsWeatherState(visualTransitionToState, "Storm");
			if (toStorm && !fromStorm)
			{
				target = SmoothWeatherPhase(weatherManager.TransitionProgress, 0.12f, 0.78f);
			}
			else if (fromStorm && !toStorm)
			{
				target = 1f - SmoothWeatherPhase(weatherManager.TransitionProgress, 0.08f, 0.72f);
			}
		}
		float rate = target > rainStormSunVisibility ? 0.72f : 0.82f;
		rainStormSunVisibility = dt > 0f ? Mathf.MoveTowards(rainStormSunVisibility, target, rate * dt) : target;
		float eased = Mathf.SmoothStep(0f, 1f, rainStormSunVisibility);
		bool visible = eased > 0.01f;
		if (rainStormSunRoot.gameObject.activeSelf != visible)
		{
			rainStormSunRoot.gameObject.SetActive(visible);
		}
		if (!visible)
		{
			return;
		}
		rainStormSunRoot.localPosition = GetStableRainStormSunLocalPosition();
		rainStormSunRoot.localRotation = Quaternion.identity;
		rainStormSunRoot.localScale = Vector3.one * Mathf.Lerp(0.94f, 1f, eased);
		SetMaterialColor(rainStormSunCoreMat, new Color(1.62f * eased, 1.52f * eased, 1.3f * eased, 1f * eased));
		SetMaterialColor(rainStormSunGlowMat, new Color(1.34f * eased, 1.08f * eased, 0.8f * eased, 0.2f * eased));
		SetMaterialColor(rainStormSunHaloMat, new Color(1.12f * eased, 0.8f * eased, 0.58f * eased, 0.045f * eased));
		if (rainStormSunCoreRenderer != null)
		{
			rainStormSunCoreRenderer.enabled = eased > 0.0005f;
		}
		if (rainStormSunGlowRenderer != null)
		{
			rainStormSunGlowRenderer.enabled = eased > 0.0005f;
		}
		if (rainStormSunHaloRenderer != null)
		{
			rainStormSunHaloRenderer.enabled = eased > 0.0005f;
		}
	}

	private void EnsureFallenSkyFallingStars()
	{
		if (fallenSkyFallingStars != null || cam == null)
		{
			return;
		}
		GameObject prefab = Resources.Load<GameObject>(FallenSkyFallingStarsResource);
		if (prefab == null)
		{
			Debug.LogWarning("[WEATHER] CFXR4 Falling Stars prefab not found in Resources", this);
			return;
		}
		fallenSkyFallingStars = Instantiate(prefab, cam.transform, false);
		fallenSkyFallingStars.name = "Fallen Sky - CFXR4 Falling Stars";
		fallenSkyFallingStars.transform.localPosition = new Vector3(0f, 65f, 180f);
		fallenSkyFallingStars.transform.localRotation = Quaternion.identity;
		fallenSkyFallingStars.transform.localScale = Vector3.one * 10f;
		Renderer[] renderers = fallenSkyFallingStars.GetComponentsInChildren<Renderer>(true);
		for (int r = 0; r < renderers.Length; r++)
		{
			Material[] materials = renderers[r].materials;
			for (int m = 0; m < materials.Length; m++)
			{
				if (materials[m] != null && materials[m].HasProperty("_ZTest"))
				{
					materials[m].SetFloat("_ZTest", 4f);
				}
			}
		}
		fallenSkyFallingStars.SetActive(false);
	}

	private void UpdateFallenSkyFallingStars()
	{
		EnsureFallenSkyFallingStars();
		if (fallenSkyFallingStars == null)
		{
			return;
		}
		bool active = IsWeatherState(weatherManager.CurrentState, "Fallen Sky");
		if (active == fallenSkyFallingStarsActive)
		{
			return;
		}
		fallenSkyFallingStarsActive = active;
		fallenSkyFallingStars.SetActive(active);
		ParticleSystem[] systems = fallenSkyFallingStars.GetComponentsInChildren<ParticleSystem>(true);
		for (int i = 0; i < systems.Length; i++)
		{
			if (active)
			{
				systems[i].Play(true);
			}
			else
			{
				systems[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
		}
	}
}
