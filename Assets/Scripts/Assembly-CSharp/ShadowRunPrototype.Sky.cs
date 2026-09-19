using System;
using UnityEngine;
using UnityEngine.Rendering;
using DynamicWeatherSystem;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_SetupWeatherSkybox, _LoadSky360Texture,
// _LoadConfiguredSky360Texture, _ApplySky360Skybox, _EnsureWeatherCameraSkybox, _ApplyFallbackSkybox, _ApplyFastSkyRuntime,
// _ApplyPulsarNightSkybox, _EnsureWeatherSkyboxActive, _FindProp, _NormalizeFastSkyProperty, _SetFloatSmart, _SetColorSmart,
// _ForceFastSkyDayClouds, _ForceFastSkyNightStars, _IsFastSkyMaterial, _EnsureSky360Cylinder, _CurrentSky360Center,
// _CurrentSky360TextureTiling, _CreateInvertedSkyCylinderMesh, _UpdateSky360Cylinder, _UpdateSky360WeatherColor,
// _SetSky360MaterialVisibility, _AdvanceWeatherPreset, _ChooseNextWeatherPresetIndex, _GetWeatherTransitionDuration,
// _IsWeatherState, _IsDarkFastSkyState, _TryGetDayNightTransition, _TryGetPlanetsTransition, _SmoothWeatherPhase,
// _SetWeatherPreset, _SetWeatherState, _CaptureVisualTransition, _ReapplyDynamicWeatherImmediate.
public sealed partial class ShadowRunPrototype
{
	private void SetupWeatherSkybox()
	{
		EnsureWeatherCameraSkybox();
		if (UseFastSkyRuntimeSkybox && !fastSkyRuntimeUnavailable)
		{
			if (IsFastSkyMaterial(RenderSettings.skybox))
			{
				return;
			}
			if (ApplyFastSkyRuntime())
			{
				return;
			}
		}
		ApplyFallbackSkybox();
	}

	private Texture2D LoadSky360Texture()
	{
		if (sky360Texture != null)
		{
			return sky360Texture;
		}
		sky360Texture = LoadConfiguredSky360Texture();
		if (sky360Texture == null)
		{
			Debug.LogError("Sky texture not found in Resources: sky_360_hd", this);
			return null;
		}
		sky360Texture.wrapModeU = TextureWrapMode.Repeat;
		sky360Texture.wrapModeV = TextureWrapMode.Clamp;
		sky360Texture.filterMode = FilterMode.Trilinear;
		sky360Texture.anisoLevel = 16;
		sky360Texture.mipMapBias = -1.1f;
		return sky360Texture;
	}

	private Texture2D LoadConfiguredSky360Texture()
	{
		return Resources.Load<Texture2D>(Sky360TextureResource);
	}

	private bool ApplySky360Skybox()
	{
		EnsureWeatherCameraSkybox();
		if (runtimeSky360Skybox != null)
		{
			if (RenderSettings.skybox != runtimeSky360Skybox)
			{
				RenderSettings.skybox = runtimeSky360Skybox;
				DynamicGI.UpdateEnvironment();
			}
			return true;
		}
		Shader shader = Shader.Find("ShadowRun/SolidColorSkybox");
		if (shader == null)
		{
			shader = Shader.Find("Skybox/Procedural");
		}
		if (shader == null)
		{
			shader = Shader.Find("Skybox/Panoramic");
		}
		if (shader == null)
		{
			if (!sky360SkyboxUnavailable)
			{
				Debug.LogError("No supported skybox shader found for runtime red sky", this);
			}
			sky360SkyboxUnavailable = true;
			return false;
		}
		sky360SkyboxUnavailable = false;
		runtimeSky360Skybox = new Material(shader);
		runtimeSky360Skybox.name = "Runtime Red Day Skybox";
		if (runtimeSky360Skybox.HasProperty("_Color"))
		{
			runtimeSky360Skybox.SetColor("_Color", new Color(1f, 0.03f, 0.02f));
		}
		if (runtimeSky360Skybox.HasProperty("_SkyTint"))
		{
			runtimeSky360Skybox.SetColor("_SkyTint", new Color(1f, 0.03f, 0.02f));
		}
		if (runtimeSky360Skybox.HasProperty("_GroundColor"))
		{
			runtimeSky360Skybox.SetColor("_GroundColor", new Color(0.72f, 0f, 0f));
		}
		if (runtimeSky360Skybox.HasProperty("_Tint"))
		{
			runtimeSky360Skybox.SetColor("_Tint", new Color(1f, 0.03f, 0.02f));
		}
		if (runtimeSky360Skybox.HasProperty("_Exposure"))
		{
			runtimeSky360Skybox.SetFloat("_Exposure", 1.3f);
		}
		if (runtimeSky360Skybox.HasProperty("_AtmosphereThickness"))
		{
			runtimeSky360Skybox.SetFloat("_AtmosphereThickness", 0f);
		}
		RenderSettings.skybox = runtimeSky360Skybox;
		DynamicGI.UpdateEnvironment();
		return true;
	}

	private void EnsureWeatherCameraSkybox()
	{
		if (cam != null && cam.clearFlags != CameraClearFlags.Skybox)
		{
			cam.clearFlags = CameraClearFlags.Skybox;
		}
	}

	private void ApplyFallbackSkybox()
	{
		if (RenderSettings.skybox != null && RenderSettings.skybox.name.StartsWith("Runtime Weather Skybox"))
		{
			return;
		}
		if (runtimeFallbackSkybox == null)
		{
			Shader shader = Shader.Find("ShadowRun/SolidColorSkybox");
			if (shader == null)
			{
				shader = Shader.Find("Skybox/Procedural");
			}
			if (shader == null)
			{
				shader = Shader.Find("Skybox/Panoramic");
			}
			if (shader == null)
			{
				return;
			}
			runtimeFallbackSkybox = new Material(shader);
			runtimeFallbackSkybox.name = "Runtime Weather Skybox";
			if (runtimeFallbackSkybox.HasProperty("_Color"))
			{
				runtimeFallbackSkybox.SetColor("_Color", new Color(0.54f, 0.61f, 0.72f));
			}
			if (runtimeFallbackSkybox.HasProperty("_SkyTint"))
			{
				runtimeFallbackSkybox.SetColor("_SkyTint", new Color(0.54f, 0.61f, 0.72f));
			}
			if (runtimeFallbackSkybox.HasProperty("_Tint"))
			{
				runtimeFallbackSkybox.SetColor("_Tint", new Color(0.54f, 0.61f, 0.72f));
			}
			if (runtimeFallbackSkybox.HasProperty("_Exposure"))
			{
				runtimeFallbackSkybox.SetFloat("_Exposure", 0.9f);
			}
		}
		RenderSettings.skybox = runtimeFallbackSkybox;
	}

	private bool ApplyFastSkyRuntime()
	{
		EnsureWeatherCameraSkybox();
		if (fastSkyRuntimeUnavailable)
		{
			return false;
		}
		if (runtimeFastSkySkybox != null)
		{
			RenderSettings.skybox = runtimeFastSkySkybox;
			return true;
		}
		Material source = Resources.Load<Material>(FastSkyMaterialPath);
		if (source == null)
		{
			fastSkyRuntimeUnavailable = true;
			Debug.LogError("FastSky material not found in Resources/FastSky/Materials/RealisticSky", this);
			return false;
		}
		runtimeFastSkySkybox = new Material(source);
		runtimeFastSkySkybox.name = "Runtime FastSky Skybox";
		RenderSettings.skybox = runtimeFastSkySkybox;
		ForceFastSkyDayClouds(runtimeFastSkySkybox);
		DynamicGI.UpdateEnvironment();
		return true;
	}

	private bool ApplyPulsarNightSkybox()
	{
		EnsureWeatherCameraSkybox();
		if (runtimePulsarNightSkybox == null)
		{
			Material source = Resources.Load<Material>(PulsarNightSkyboxResource);
			if (source == null)
			{
				return false;
			}
			runtimePulsarNightSkybox = new Material(source);
			runtimePulsarNightSkybox.name = "Runtime Pulsar Night Skybox";
			if (runtimePulsarNightSkybox.HasProperty("Vector1_5FAFE31D"))
			{
				runtimePulsarNightSkybox.SetFloat("Vector1_5FAFE31D", 1.26f);
			}
			if (runtimePulsarNightSkybox.HasProperty("Vector1_A1B13CA9"))
			{
				runtimePulsarNightSkybox.SetFloat("Vector1_A1B13CA9", 0.48f);
			}
			if (runtimePulsarNightSkybox.HasProperty("Vector1_B851E38B"))
			{
				runtimePulsarNightSkybox.SetFloat("Vector1_B851E38B", 0.82f);
			}
			if (runtimePulsarNightSkybox.HasProperty("_Exposure"))
			{
				runtimePulsarNightSkybox.SetFloat("_Exposure", 0.76f);
			}
			if (runtimePulsarNightSkybox.HasProperty("_Tint"))
			{
				runtimePulsarNightSkybox.SetColor("_Tint", new Color(0.44f, 0.56f, 0.92f, 0.62f));
			}
			if (runtimePulsarNightSkybox.HasProperty("_Rotation"))
			{
				runtimePulsarNightSkybox.SetFloat("_Rotation", 66f);
			}
		}
		if (RenderSettings.skybox != runtimePulsarNightSkybox)
		{
			RenderSettings.skybox = runtimePulsarNightSkybox;
			DynamicGI.UpdateEnvironment();
		}
		return true;
	}

	private void EnsureWeatherSkyboxActive()
	{
		EnsureWeatherCameraSkybox();
		if (IsFastSkyMaterial(RenderSettings.skybox))
		{
			return;
		}
		SetupWeatherSkybox();
		if (weatherManager != null && weatherManager.CurrentState != null && IsFastSkyMaterial(RenderSettings.skybox))
		{
			weatherManager.SetWeather(weatherManager.CurrentState, 0f);
		}
	}

	private string FindProp(Material mat, string wanted)
	{
		if (mat == null || mat.shader == null)
		{
			return null;
		}
		string key = NormalizeFastSkyProperty(wanted);
		if (fastSkyPropertyCache.TryGetValue(key, out string cached))
		{
			return cached;
		}
		Shader shader = mat.shader;
		for (int i = 0; i < shader.GetPropertyCount(); i++)
		{
			string propertyName = shader.GetPropertyName(i);
			string description = shader.GetPropertyDescription(i);
			string normalizedName = NormalizeFastSkyProperty(propertyName);
			string normalizedDescription = NormalizeFastSkyProperty(description);
			if (normalizedName == key || normalizedDescription == key || normalizedName.Contains(key) || normalizedDescription.Contains(key))
			{
				fastSkyPropertyCache[key] = propertyName;
				return propertyName;
			}
		}
		Debug.LogWarning("FastSky property not found: " + wanted, this);
		return null;
	}

	private static string NormalizeFastSkyProperty(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return value.Replace("_", string.Empty).Replace(" ", string.Empty).ToLowerInvariant();
		}
		return string.Empty;
	}

	private void SetFloatSmart(Material mat, string prop, float value)
	{
		string propertyName = FindProp(mat, prop);
		if (!string.IsNullOrEmpty(propertyName))
		{
			mat.SetFloat(propertyName, value);
		}
	}

	private void SetColorSmart(Material mat, string prop, Color value)
	{
		string propertyName = FindProp(mat, prop);
		if (!string.IsNullOrEmpty(propertyName))
		{
			mat.SetColor(propertyName, value);
		}
	}

	private void ForceFastSkyDayClouds(Material mat)
	{
		if (mat == null)
		{
			return;
		}
		SetColorSmart(mat, "DayColour", new Color(0.5f, 0.025f, 0.045f));
		SetColorSmart(mat, "EveningColour", new Color(1.1f, 0.1f, 0.045f));
		SetFloatSmart(mat, "SunIntensity", 12f);
		SetFloatSmart(mat, "SunSize", 0.055f);
		SetFloatSmart(mat, "Saturation", 1.65f);
		SetFloatSmart(mat, "DayBrightness", 0.38f);
		SetFloatSmart(mat, "EveningBrightness", 0.16f);
		SetFloatSmart(mat, "EveningThreshold", 1.34f);
		SetColorSmart(mat, "Wavelengths", new Color(760f, 470f, 390f, 0f));
		SetFloatSmart(mat, "DayScatterStrength", 1.12f);
		SetFloatSmart(mat, "EveningScatterStrength", 19f);
		SetFloatSmart(mat, "DensityPower", 7.2f);
		SetFloatSmart(mat, "StarBrightness", 0f);
		SetColorSmart(mat, "CloudColour", Color.white);
		SetFloatSmart(mat, "CloudBrightness", 10f);
		SetFloatSmart(mat, "CloudAnisotropy", 0f);
		SetFloatSmart(mat, "CloudDensity", 2f);
		SetFloatSmart(mat, "CloudHeight", 0.25f);
		SetFloatSmart(mat, "CloudThickness", 0.6f);
		SetFloatSmart(mat, "CloudSoftness", 0.05f);
		SetFloatSmart(mat, "CloudSpeed", 5f);
		SetFloatSmart(mat, "CloudScale", 0.5f);
		SetFloatSmart(mat, "CloudTwirl", 1f);
		SetFloatSmart(mat, "CloudSmoothness", 0f);
		SetFloatSmart(mat, "CloudDensitySmoothness", 1f);
		DynamicGI.UpdateEnvironment();
	}

	private void ForceFastSkyNightStars(Material mat)
	{
		if (mat == null)
		{
			return;
		}
		SetFloatSmart(mat, "SunIntensity", 0.08f);
		SetColorSmart(mat, "DayColour", new Color(0.025f, 0.06f, 0.18f));
		SetColorSmart(mat, "EveningColour", new Color(0.006f, 0.016f, 0.06f));
		SetFloatSmart(mat, "Saturation", 1.85f);
		SetFloatSmart(mat, "DayBrightness", 0.24f);
		SetFloatSmart(mat, "EveningBrightness", 0.105f);
		SetFloatSmart(mat, "EveningThreshold", 1.62f);
		SetColorSmart(mat, "Wavelengths", new Color(520f, 575f, 760f, 0f));
		SetFloatSmart(mat, "DayScatterStrength", 1.28f);
		SetFloatSmart(mat, "EveningScatterStrength", 22f);
		SetFloatSmart(mat, "DensityPower", 6.2f);
		SetColorSmart(mat, "StarColour", new Color(1.35f, 1.55f, 2.15f));
		SetFloatSmart(mat, "StarBrightness", 1180f);
		SetFloatSmart(mat, "StarSize", 0.9985f);
		SetFloatSmart(mat, "StarScale", 2.8f);
		SetFloatSmart(mat, "StarSpeed", 0f);
		SetFloatSmart(mat, "StarJitterSpeed", 0f);
		SetFloatSmart(mat, "StarRotateSpeed", 0f);
		SetFloatSmart(mat, "StarThreshold", 23f);
		SetFloatSmart(mat, "CloudDensity", 0f);
		SetFloatSmart(mat, "CloudBrightness", 0f);
		DynamicGI.UpdateEnvironment();
	}

	private bool IsFastSkyMaterial(Material material)
	{
		if (material == null)
		{
			return false;
		}
		if ((material.shader != null && material.shader.name == "GapperGames/FastSky")
			|| (material.shader != null && material.shader.name == "GapperGames/Fast_Skybox")
			|| (material.shader != null && material.shader.name == "GapperGames/Skybox/Fast_Skybox")
			|| material.HasProperty("_DayBrightness")
			|| material.name.Contains("RealisticSky"))
		{
			return true;
		}
		return material.name.Contains("Runtime FastSky");
	}

	private void EnsureSky360Cylinder()
	{
		if (!UseSeparateCloud360Layer && !separateCloudCleanupComplete)
		{
			GameObject legacyClouds = GameObject.Find("CloudLayerCylinder");
			if (legacyClouds != null)
			{
				Destroy(legacyClouds);
			}
			cloud360Cylinder = null;
			cloud360Renderer = null;
			cloud360Material = null;
			cloud360Texture = null;
			separateCloudCleanupComplete = true;
		}
		if (sky360Cylinder != null || cam == null)
		{
			return;
		}
		sky360Texture = LoadSky360Texture();
		if (sky360Texture == null)
		{
			return;
		}
		if (UseSeparateCloud360Layer)
		{
			cloud360Texture = Resources.Load<Texture2D>(Cloud360TextureResource);
			if (cloud360Texture == null)
			{
				Debug.LogError("Cloud panorama not found in Resources: clouds_panorama_8k", this);
				return;
			}
			cloud360Texture.wrapModeU = TextureWrapMode.Repeat;
			cloud360Texture.wrapModeV = TextureWrapMode.Clamp;
			cloud360Texture.filterMode = FilterMode.Trilinear;
			cloud360Texture.anisoLevel = 16;
			cloud360Texture.mipMapBias = -0.85f;
		}
		Shader skyShader = Shader.Find("ShadowRun/Sky360NoFog");
		if (skyShader == null)
		{
			skyShader = Shader.Find("Universal Render Pipeline/Unlit");
		}
		if (skyShader == null)
		{
			skyShader = Shader.Find("Unlit/Texture");
		}
		sky360Material = skyShader != null ? new Material(skyShader) : Mat(Color.white);
		sky360Material.name = "Runtime Sky 360 Cylinder";
		sky360Material.renderQueue = 2501;
		if (sky360Material.HasProperty(BaseMapProperty))
		{
			sky360Material.SetTexture(BaseMapProperty, sky360Texture);
		}
		if (sky360Material.HasProperty(MainTexProperty))
		{
			sky360Material.SetTexture(MainTexProperty, sky360Texture);
		}
		Vector2 tiling = CurrentSky360TextureTiling();
		if (sky360Material.HasProperty(BaseMapProperty))
		{
			sky360Material.SetTextureScale(BaseMapProperty, tiling);
		}
		if (sky360Material.HasProperty(MainTexProperty))
		{
			sky360Material.SetTextureScale(MainTexProperty, tiling);
		}
		if (sky360Material.HasProperty(BaseMapProperty))
		{
			sky360Material.SetTextureOffset(BaseMapProperty, Sky360TextureUvOffset);
		}
		if (sky360Material.HasProperty(MainTexProperty))
		{
			sky360Material.SetTextureOffset(MainTexProperty, Sky360TextureUvOffset);
		}
		if (sky360Material.HasProperty("_BaseColor"))
		{
			sky360Material.SetColor("_BaseColor", Color.white);
		}
		if (sky360Material.HasProperty("_Color"))
		{
			sky360Material.SetColor("_Color", Color.white);
		}
		if (sky360Material.HasProperty("_EmissionMap"))
		{
			sky360Material.SetTexture("_EmissionMap", sky360Texture);
		}
		if (sky360Material.HasProperty("_EmissionColor"))
		{
			sky360Material.SetColor("_EmissionColor", Color.white * Sky360EmissionBoost);
		}
		if (sky360Material.HasProperty("_Smoothness"))
		{
			sky360Material.SetFloat("_Smoothness", 0f);
		}
		if (sky360Material.HasProperty("_Metallic"))
		{
			sky360Material.SetFloat("_Metallic", 0f);
		}
		sky360Material.EnableKeyword("_EMISSION");
		if (sky360Material.HasProperty("_Surface"))
		{
			sky360Material.SetFloat("_Surface", 0f);
		}
		if (sky360Material.HasProperty("_Blend"))
		{
			sky360Material.SetFloat("_Blend", 0f);
		}
		if (sky360Material.HasProperty("_AlphaClip"))
		{
			sky360Material.SetFloat("_AlphaClip", 0f);
		}
		if (sky360Material.HasProperty("_SrcBlend"))
		{
			sky360Material.SetFloat("_SrcBlend", 1f);
		}
		if (sky360Material.HasProperty("_DstBlend"))
		{
			sky360Material.SetFloat("_DstBlend", 0f);
		}
		if (sky360Material.HasProperty("_ZWrite"))
		{
			sky360Material.SetFloat("_ZWrite", 0f);
		}
		if (sky360Material.HasProperty("_Cull"))
		{
			sky360Material.SetFloat("_Cull", 0f);
		}
		if (sky360Material.HasProperty("_ReceiveShadows"))
		{
			sky360Material.SetFloat("_ReceiveShadows", 0f);
		}
		if (sky360Material.HasProperty("_Desaturation"))
		{
			sky360Material.SetFloat("_Desaturation", 0f);
		}
		GameObject cylinder = new GameObject("SkyBoxCylinder");
		cylinder.transform.position = CurrentSky360Center();
		cylinder.transform.rotation = Quaternion.Euler(0f, Sky360CylinderBaseYaw, 0f);
		cylinder.transform.localScale = Sky360CylinderScale;
		cylinder.AddComponent<MeshFilter>().sharedMesh = CreateInvertedSkyCylinderMesh();
		sky360Renderer = cylinder.AddComponent<MeshRenderer>();
		sky360Renderer.sharedMaterial = sky360Material;
		sky360Renderer.shadowCastingMode = ShadowCastingMode.Off;
		sky360Renderer.receiveShadows = false;
		sky360Renderer.lightProbeUsage = LightProbeUsage.Off;
		sky360Renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		sky360Cylinder = cylinder.transform;
		sky360Cylinder.gameObject.SetActive(false);
		if (!UseSeparateCloud360Layer)
		{
			return;
		}
		Shader cloudShader = Shader.Find("ShadowRun/Sky360CloudLayer");
		if (cloudShader == null)
		{
			cloudShader = Shader.Find("Universal Render Pipeline/Unlit");
		}
		if (cloudShader == null)
		{
			cloudShader = Shader.Find("Unlit/Transparent");
		}
		cloud360Material = cloudShader != null ? new Material(cloudShader) : MatTransparentUnlit(Color.white, cloud360Texture, false);
		cloud360Material.name = "Runtime 8K Cloud 360 Cylinder";
		cloud360Material.renderQueue = 2510;
		if (cloud360Material.HasProperty(BaseMapProperty))
		{
			cloud360Material.SetTexture(BaseMapProperty, cloud360Texture);
		}
		if (cloud360Material.HasProperty(MainTexProperty))
		{
			cloud360Material.SetTexture(MainTexProperty, cloud360Texture);
		}
		if (cloud360Material.HasProperty(BaseMapProperty))
		{
			cloud360Material.SetTextureScale(BaseMapProperty, Cloud360TextureTiling);
		}
		if (cloud360Material.HasProperty(MainTexProperty))
		{
			cloud360Material.SetTextureScale(MainTexProperty, Cloud360TextureTiling);
		}
		if (cloud360Material.HasProperty("_Color"))
		{
			cloud360Material.SetColor("_Color", Color.white);
		}
		if (cloud360Material.HasProperty("_Visibility"))
		{
			cloud360Material.SetFloat("_Visibility", 0f);
		}
		if (cloud360Material.HasProperty("_Desaturation"))
		{
			cloud360Material.SetFloat("_Desaturation", 0f);
		}
		GameObject cloudCylinder = new GameObject("CloudLayerCylinder");
		cloudCylinder.transform.position = CurrentSky360Center();
		cloudCylinder.transform.rotation = Quaternion.Euler(0f, Sky360CylinderBaseYaw, 0f);
		cloudCylinder.transform.localScale = Cloud360CylinderScale;
		cloudCylinder.AddComponent<MeshFilter>().sharedMesh = CreateInvertedSkyCylinderMesh();
		cloud360Renderer = cloudCylinder.AddComponent<MeshRenderer>();
		cloud360Renderer.sharedMaterial = cloud360Material;
		cloud360Renderer.shadowCastingMode = ShadowCastingMode.Off;
		cloud360Renderer.receiveShadows = false;
		cloud360Renderer.lightProbeUsage = LightProbeUsage.Off;
		cloud360Renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		cloud360Cylinder = cloudCylinder.transform;
		cloud360Cylinder.gameObject.SetActive(false);
	}

	private Vector3 CurrentSky360Center()
	{
		return Sky360CylinderWorldPosition;
	}

	private Vector2 CurrentSky360TextureTiling()
	{
		float aspect = cam != null && cam.aspect > 0.01f ? cam.aspect : 0.5625f;
		float halfVerticalFov = 0.47560221f;
		float horizontalFov = 2f * Mathf.Atan(Mathf.Tan(halfVerticalFov) * aspect) * Mathf.Rad2Deg;
		return new Vector2(360f / Mathf.Max(1f, horizontalFov), 1f);
	}

	private Mesh CreateInvertedSkyCylinderMesh()
	{
		Vector3[] vertices = new Vector3[514];
		Vector2[] uvs = new Vector2[vertices.Length];
		int[] triangles = new int[1536];
		int vertexIndex = 0;
		for (int i = 0; i <= 256; i++)
		{
			float u = i / 256f;
			float angle = u * Mathf.PI * 2f;
			float x = Mathf.Sin(angle) * 1f;
			float z = Mathf.Cos(angle) * 1f;
			vertices[vertexIndex] = new Vector3(x, -1f, z);
			vertices[vertexIndex + 1] = new Vector3(x, 1f, z);
			uvs[vertexIndex] = new Vector2(u, 0f);
			uvs[vertexIndex + 1] = new Vector2(u, 1f);
			vertexIndex += 2;
		}
		int triangleIndex = 0;
		for (int i = 0; i < 256; i++)
		{
			int bottomLeft = i * 2;
			int topLeft = bottomLeft + 1;
			int bottomRight = (i + 1) * 2;
			int topRight = bottomRight + 1;
			triangles[triangleIndex++] = bottomLeft;
			triangles[triangleIndex++] = topRight;
			triangles[triangleIndex++] = topLeft;
			triangles[triangleIndex++] = bottomLeft;
			triangles[triangleIndex++] = bottomRight;
			triangles[triangleIndex++] = topRight;
		}
		Mesh mesh = new Mesh();
		mesh.name = "Runtime Inverted Sky Cylinder Mesh";
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		return mesh;
	}

	private void UpdateSky360Cylinder(float dt)
	{
		EnsureSky360Cylinder();
		Texture2D configured = sky360Texture != null ? sky360Texture : LoadConfiguredSky360Texture();
		if (configured != null && configured != sky360Texture)
		{
			sky360Texture = configured;
			sky360Texture.wrapModeU = TextureWrapMode.Repeat;
			sky360Texture.wrapModeV = TextureWrapMode.Clamp;
			sky360Texture.filterMode = FilterMode.Trilinear;
			sky360Texture.anisoLevel = 16;
			sky360Texture.mipMapBias = -1.1f;
			if (sky360Material != null)
			{
				if (sky360Material.HasProperty(BaseMapProperty))
				{
					sky360Material.SetTexture(BaseMapProperty, sky360Texture);
				}
				if (sky360Material.HasProperty(MainTexProperty))
				{
					sky360Material.SetTexture(MainTexProperty, sky360Texture);
				}
				if (sky360Material.HasProperty("_EmissionMap"))
				{
					sky360Material.SetTexture("_EmissionMap", sky360Texture);
				}
			}
		}
		WeatherStateData current = weatherManager != null ? weatherManager.CurrentState : null;
		float targetVisibility = IsDarkFastSkyState(current) ? 0f : 1f;
		bool dayNightTransition = TryGetDayNightTransition(out bool enteringNight, out float progress);
		if (dayNightTransition)
		{
			targetVisibility = enteringNight ? 1f - SmoothWeatherPhase(progress, 0.06f, 0.84f) : SmoothWeatherPhase(progress, 0f, 0.62f);
			sky360Visibility = targetVisibility;
		}
		else
		{
			float fadeRate = 1f / Mathf.Max(0.01f, WeatherTransitionDuration);
			sky360Visibility = dt > 0f ? Mathf.MoveTowards(sky360Visibility, targetVisibility, fadeRate * dt) : targetVisibility;
		}
		if (sky360Cylinder != null && !sky360Cylinder.gameObject.activeSelf)
		{
			sky360Cylinder.gameObject.SetActive(true);
		}
		if (cloud360Cylinder != null && !cloud360Cylinder.gameObject.activeSelf)
		{
			cloud360Cylinder.gameObject.SetActive(true);
		}
		if (sky360Cylinder == null)
		{
			return;
		}
		bool leavingNight = dayNightTransition && !enteringNight && progress < 0.96f;
		if (!IsDarkFastSkyState(current) && !leavingNight)
		{
			ApplySky360Skybox();
		}
		sky360TextureOffset = Mathf.Repeat(sky360TextureOffset + Sky360RotationSpeed * dt / 360f, 1f);
		float yaw = sky360TextureOffset * 360f;
		Vector3 center = CurrentSky360Center();
		sky360Cylinder.position = center;
		sky360Cylinder.localScale = Sky360CylinderScale;
		sky360Cylinder.rotation = Quaternion.Euler(0f, Sky360CylinderBaseYaw + yaw, 0f);
		if (cloud360Cylinder != null)
		{
			cloud360Cylinder.position = center;
			cloud360Cylinder.localScale = Cloud360CylinderScale;
			cloud360Cylinder.rotation = Quaternion.Euler(0f, Sky360CylinderBaseYaw + yaw * 1.06f, 0f);
		}
		if (sky360Material.HasProperty(BaseMapProperty))
		{
			sky360Material.SetTextureOffset(BaseMapProperty, Sky360TextureUvOffset);
		}
		if (sky360Material.HasProperty(MainTexProperty))
		{
			sky360Material.SetTextureOffset(MainTexProperty, Sky360TextureUvOffset);
		}
		Vector2 tiling = CurrentSky360TextureTiling();
		if (sky360Material.HasProperty(BaseMapProperty))
		{
			sky360Material.SetTextureScale(BaseMapProperty, tiling);
		}
		if (sky360Material.HasProperty(MainTexProperty))
		{
			sky360Material.SetTextureScale(MainTexProperty, tiling);
		}
		if (cloud360Material != null)
		{
			if (cloud360Material.HasProperty(BaseMapProperty))
			{
				cloud360Material.SetTextureScale(BaseMapProperty, Cloud360TextureTiling);
			}
			if (cloud360Material.HasProperty(MainTexProperty))
			{
				cloud360Material.SetTextureScale(MainTexProperty, Cloud360TextureTiling);
			}
		}
		UpdateSky360WeatherColor(current);
		SetSky360MaterialVisibility(sky360Visibility);
	}

	private void UpdateSky360WeatherColor(WeatherStateData current)
	{
		float targetDesaturation = IsWeatherState(current, "Rain") ? Sky360RainDesaturation : 0f;
		sky360Desaturation = weatherManager != null && weatherManager.IsTransitioning
			? Mathf.Lerp(sky360TransitionFromDesaturation, sky360TransitionToDesaturation, weatherManager.TransitionProgress)
			: targetDesaturation;
		if (Mathf.Abs(sky360AppliedDesaturation - sky360Desaturation) <= 0.0005f)
		{
			return;
		}
		if (sky360Material.HasProperty("_Desaturation"))
		{
			sky360Material.SetFloat("_Desaturation", sky360Desaturation);
		}
		if (cloud360Material != null && cloud360Material.HasProperty("_Desaturation"))
		{
			cloud360Material.SetFloat("_Desaturation", sky360Desaturation);
		}
		sky360AppliedDesaturation = sky360Desaturation;
	}

	private void SetSky360MaterialVisibility(float visibility)
	{
		if (sky360Material == null)
		{
			return;
		}
		float clamped = Mathf.Clamp01(visibility);
		bool hasSkyBlend = sky360Material.HasProperty("_SkyBlend");
		Color tint = new Color(1f, 1f, 1f, hasSkyBlend ? 1f : clamped);
		if (sky360Material.HasProperty("_BaseColor"))
		{
			sky360Material.SetColor("_BaseColor", tint);
		}
		if (sky360Material.HasProperty("_Color"))
		{
			sky360Material.SetColor("_Color", tint);
		}
		if (hasSkyBlend)
		{
			sky360Material.SetFloat("_SkyBlend", clamped);
		}
		if (sky360Material.HasProperty("_EmissionColor"))
		{
			sky360Material.SetColor("_EmissionColor", Color.white * Sky360EmissionBoost * Mathf.Clamp01(visibility));
		}
		if (sky360Renderer != null)
		{
			sky360Renderer.enabled = visibility > 0.001f;
		}
		if (cloud360Material != null)
		{
			if (cloud360Material.HasProperty("_Visibility"))
			{
				cloud360Material.SetFloat("_Visibility", clamped);
			}
			if (cloud360Material.HasProperty("_Color"))
			{
				cloud360Material.SetColor("_Color", new Color(1f, 1f, 1f, clamped));
			}
		}
		if (cloud360Renderer != null)
		{
			cloud360Renderer.enabled = visibility > 0.001f;
		}
	}

	private void AdvanceWeatherPreset()
	{
		/* [InfernoLobby patch] */ return;
#pragma warning disable CS0162
		if (weatherPresetCycle == null || weatherPresetCycle.Length < 2)
		{
			return;
		}
		int next = ChooseNextWeatherPresetIndex();
		SetWeatherPreset(next, GetWeatherTransitionDuration(weatherPresetCycle[next]));
#pragma warning restore CS0162
	}

	private int ChooseNextWeatherPresetIndex()
	{
		weatherSelectionWeights[0] = 0.26f;
		weatherSelectionWeights[1] = 0.21f;
		weatherSelectionWeights[2] = 0.22f;
		weatherSelectionWeights[3] = 0.11f;
		weatherSelectionWeights[4] = 0.12f;
		weatherSelectionWeights[5] = 0.08f;
		float total = 0f;
		for (int i = 0; i < weatherPresetCycle.Length; i++)
		{
			if (i == weatherPresetIndex)
			{
				weatherSelectionWeights[i] = 0f;
			}
			else if (i == previousWeatherPresetIndex)
			{
				weatherSelectionWeights[i] *= 0.22f;
			}
			total += weatherSelectionWeights[i];
		}
		float roll = UnityEngine.Random.value * total;
		for (int i = 0; i < weatherSelectionWeights.Length; i++)
		{
			roll -= weatherSelectionWeights[i];
			if (roll <= 0f && weatherSelectionWeights[i] > 0f)
			{
				return i;
			}
		}
		return (weatherPresetIndex + 1) % weatherPresetCycle.Length;
	}

	private float GetWeatherTransitionDuration(WeatherStateData target)
	{
		if (IsWeatherState(target, "Fallen Sky"))
		{
			return FallenSkyTransitionDuration;
		}
		if (IsWeatherState(target, "Planets") || (weatherManager != null && IsWeatherState(weatherManager.CurrentState, "Planets")))
		{
			return PlanetsTransitionDuration;
		}
		if (weatherManager != null && IsWeatherState(weatherManager.CurrentState, "Night") && IsWeatherState(target, "Day"))
		{
			return NightToDayTransitionDuration;
		}
		return WeatherTransitionDuration;
	}

	private bool IsWeatherState(WeatherStateData state, string stateName)
	{
		if (state != null)
		{
			return state.stateName == stateName;
		}
		return false;
	}

	private bool IsDarkFastSkyState(WeatherStateData state)
	{
		if (IsWeatherState(state, "Night"))
		{
			return true;
		}
		return IsWeatherState(state, "Planets");
	}

	private bool TryGetDayNightTransition(out bool enteringNight, out float progress)
	{
		enteringNight = false;
		progress = 0f;
		if (weatherManager == null || !weatherManager.IsTransitioning)
		{
			return false;
		}
		bool fromNight = IsWeatherState(visualTransitionFromState, "Night");
		bool toNight = IsWeatherState(visualTransitionToState, "Night");
		if (fromNight == toNight)
		{
			return false;
		}
		enteringNight = toNight;
		progress = weatherManager.TransitionProgress;
		return true;
	}

	private bool TryGetPlanetsTransition(out bool enteringPlanets, out float progress)
	{
		enteringPlanets = false;
		progress = 0f;
		if (weatherManager == null || !weatherManager.IsTransitioning)
		{
			return false;
		}
		bool fromPlanets = IsWeatherState(visualTransitionFromState, "Planets");
		bool toPlanets = IsWeatherState(visualTransitionToState, "Planets");
		if (fromPlanets == toPlanets)
		{
			return false;
		}
		enteringPlanets = toPlanets;
		progress = weatherManager.TransitionProgress;
		return true;
	}

	private float SmoothWeatherPhase(float progress, float start, float end)
	{
		float t = Mathf.InverseLerp(start, end, progress);
		return t * t * (3f - 2f * t);
	}

	private bool SetWeatherPreset(int index, float duration)
	{
		if (weatherPresetCycle == null || index < 0 || index >= weatherPresetCycle.Length)
		{
			return false;
		}
		previousWeatherPresetIndex = weatherPresetIndex;
		weatherPresetIndex = index;
		weatherAutoTimer = WeatherPhaseDuration + Mathf.Max(0f, duration);
		WeatherStateData state = weatherPresetCycle[weatherPresetIndex];
		CaptureVisualTransition(state, duration);
		weatherManager.SetWeather(state, duration);
		return true;
	}

	private bool SetWeatherState(WeatherStateData state, float duration)
	{
		if (state == null)
		{
			return false;
		}
		if (weatherPresetCycle != null && weatherPresetCycle.Length != 0)
		{
			int index = Array.IndexOf(weatherPresetCycle, state);
			if (index >= 0)
			{
				previousWeatherPresetIndex = weatherPresetIndex;
				weatherPresetIndex = index;
			}
		}
		weatherAutoTimer = WeatherPhaseDuration + Mathf.Max(0f, duration);
		CaptureVisualTransition(state, duration);
		weatherManager.SetWeather(state, duration);
		return true;
	}

	private void CaptureVisualTransition(WeatherStateData target, float duration)
	{
		sky360TransitionFromDesaturation = sky360Desaturation;
		sky360TransitionToDesaturation = IsWeatherState(target, "Rain") ? Sky360RainDesaturation : 0f;
		visualTransitionFromState = duration > 0f && weatherManager != null ? weatherManager.CurrentState : target;
		visualTransitionToState = target;
	}

	private void ReapplyDynamicWeatherImmediate()
	{
		if (!dynamicWeatherReady || weatherManager == null || weatherManager.CurrentState == null)
		{
			return;
		}
		SetupWeatherSkybox();
		DisableLegacySkyBackdrop();
		weatherManager.SetWeather(weatherManager.CurrentState, 0f);
	}
}
