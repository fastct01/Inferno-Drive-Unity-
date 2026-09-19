using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_MakeMaterials, _ApplyRoadTexture,
// _ConfigureRoadTexture, _DisableRoadTexture, _UpdateRoadTexture, _Mat, _ApplyBuildingSurfaceFinish (x2),
// _ApplyBuildingEmission, _GetCityUnlitMaterial, _ConvertBuildingMaterialToUnlit,
// _FindImportedBuildingBaseTexture, _FindImportedBuildingBaseColor, _MatUnlit, _MatTransparent,
// _MatTransparentUnlit, _MakeSmokeParticleMaterial, _ApplyVisualQuality, _SetMaterialColor.
public sealed partial class ShadowRunPrototype
{
	private void MakeMaterials()
	{
		roadMat = Mat(new Color(0.45f, 0.45f, 0.45f));
		ApplyRoadTexture();
		cityGroundMat = Mat(new Color(0.035f, 0.026f, 0.026f));
		facadeMat = Mat(new Color(0.12f, 0.12f, 0.14f));
		ApplyBuildingSurfaceFinish(facadeMat);
		cloudMat = MatUnlit(Color.white);
		lineMat = Mat(new Color(0.18f, 0.9f, 1f), new Color(0.08f, 0.78f, 1.15f) * 4.4f);
		blackMat = Mat(Color.black);
		carMat = Mat(new Color(0.96f, 0.94f, 1f));
		glassMat = Mat(new Color(0.01f, 0.018f, 0.045f), new Color(0.02f, 0.28f, 0.5f) * 2.1f);
		tireMat = Mat(new Color(0.004f, 0.003f, 0.006f));
		rimMat = Mat(new Color(0.35f, 0.36f, 0.45f), new Color(0.05f, 0.35f, 0.65f) * 0.75f);
		yellowWindowMat = Mat(new Color(1f, 0.66f, 0.2f), new Color(1f, 0.46f, 0.08f) * 3.7f);
		darkWindowMat = Mat(new Color(0f, 0f, 0.004f));
		redBuildingMat = Mat(new Color(0.24f, 0.06f, 0.075f));
		purpleBuildingMat = Mat(new Color(0.045f, 0.025f, 0.07f));
		ApplyBuildingSurfaceFinish(redBuildingMat);
		ApplyBuildingSurfaceFinish(purpleBuildingMat);
		obstacleMat = Mat(new Color(0.84f, 0.03f, 0.12f), new Color(0.84f, 0.03f, 0.12f) * 1.7f);
		coinMat = Mat(new Color(0.08f, 0.55f, 1f), new Color(0.08f, 0.55f, 1f) * 2.8f);
		pickupMat = Mat(new Color(0.43f, 0.21f, 0.66f), new Color(0.43f, 0.21f, 0.66f) * 2.4f);
		particleBlueMat = Mat(new Color(0.08f, 0.55f, 1f), new Color(0.08f, 0.55f, 1f) * 2.5f);
		particleRedMat = Mat(new Color(0.84f, 0.03f, 0.12f), new Color(0.84f, 0.03f, 0.12f) * 2f);
		particleVioletMat = Mat(new Color(0.43f, 0.21f, 0.66f), new Color(0.43f, 0.21f, 0.66f) * 2f);
		smokeParticleMat = MakeSmokeParticleMaterial();
		speedLineMat = Mat(new Color(0.62f, 0.82f, 1f), new Color(0.18f, 0.65f, 1f) * 2.6f);
		greenLightMat = Mat(new Color(0.06f, 0.85f, 0.28f), new Color(0.06f, 0.85f, 0.28f) * 2.5f);
		redLightOffMat = MatUnlit(new Color(0.14f, 0.01f, 0.018f));
		yellowLightOffMat = MatUnlit(new Color(0.16f, 0.09f, 0.012f));
		greenLightOffMat = MatUnlit(new Color(0.01f, 0.12f, 0.04f));
		neonFenceBodyMat = Mat(new Color(0.018f, 0.023f, 0.026f), new Color(0.005f, 0.025f, 0.032f) * 0.5f);
		neonFenceMat = Mat(new Color(0.02f, 0.92f, 1f), new Color(0.02f, 0.92f, 1.1f) * 6.2f);
	}

	private void ApplyRoadTexture()
	{
		Texture2D texture = Resources.Load<Texture2D>("asphalt_04_diff_1k");
		if (texture == null)
		{
			texture = Resources.Load<Texture2D>("road1");
		}
		if (texture == null)
		{
			Debug.LogWarning("Road texture not found in Resources: asphalt_04_diff_1k or road1");
			DisableRoadTexture();
			return;
		}
		roadTexture = texture;
		roadNormalTexture = Resources.Load<Texture2D>("asphalt_04_nor_gl_1k");
		roadAoTexture = Resources.Load<Texture2D>("asphalt_04_ao_1k");
		ConfigureRoadTexture(texture, true);
		ConfigureRoadTexture(roadNormalTexture, false);
		ConfigureRoadTexture(roadAoTexture, false);
		Color tint = new Color(0.0175f, 0.019f, 0.021f);
		roadMat.color = tint;
		roadMatHasBaseMap = roadMat.HasProperty(BaseMapProperty);
		roadMatHasMainTex = roadMat.HasProperty(MainTexProperty);
		roadMatHasBumpMap = roadMat.HasProperty(BumpMapProperty);
		roadMatHasOcclusionMap = roadMat.HasProperty(OcclusionMapProperty);
		if (roadMat.HasProperty("_BaseColor"))
		{
			roadMat.SetColor("_BaseColor", tint);
		}
		if (roadMat.HasProperty("_Color"))
		{
			roadMat.SetColor("_Color", tint);
		}
		if (roadMat.HasProperty("_Smoothness"))
		{
			roadMat.SetFloat("_Smoothness", 0.58f);
		}
		if (roadMat.HasProperty("_Metallic"))
		{
			roadMat.SetFloat("_Metallic", 0f);
		}
		if (roadMat.HasProperty("_SpecColor"))
		{
			roadMat.SetColor("_SpecColor", new Color(0.16f, 0.18f, 0.2f));
		}
		if (roadMat.HasProperty("_SpecularHighlights"))
		{
			roadMat.SetFloat("_SpecularHighlights", 1f);
		}
		if (roadMat.HasProperty("_EnvironmentReflections"))
		{
			roadMat.SetFloat("_EnvironmentReflections", 0f);
		}
		roadMat.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
		if (roadMatHasBaseMap)
		{
			roadMat.SetTexture(BaseMapProperty, texture);
		}
		if (roadMatHasMainTex)
		{
			roadMat.SetTexture(MainTexProperty, texture);
		}
		if (roadMatHasBaseMap)
		{
			roadMat.SetTextureScale(BaseMapProperty, RoadTextureTiling);
		}
		if (roadMatHasMainTex)
		{
			roadMat.SetTextureScale(MainTexProperty, RoadTextureTiling);
		}
		if (roadMatHasBumpMap && roadNormalTexture != null)
		{
			roadMat.SetTexture(BumpMapProperty, roadNormalTexture);
			roadMat.SetTextureScale(BumpMapProperty, RoadTextureTiling);
			if (roadMat.HasProperty(BumpScaleProperty))
			{
				roadMat.SetFloat(BumpScaleProperty, 1.08f);
			}
			roadMat.EnableKeyword("_NORMALMAP");
		}
		if (roadMatHasOcclusionMap && roadAoTexture != null)
		{
			roadMat.SetTexture(OcclusionMapProperty, roadAoTexture);
			roadMat.SetTextureScale(OcclusionMapProperty, RoadTextureTiling);
			if (roadMat.HasProperty(OcclusionStrengthProperty))
			{
				roadMat.SetFloat(OcclusionStrengthProperty, 0.6f);
			}
			roadMat.EnableKeyword("_OCCLUSIONMAP");
		}
	}

	private void ConfigureRoadTexture(Texture2D texture, bool colorTexture)
	{
		if (texture == null)
		{
			return;
		}
		texture.wrapMode = TextureWrapMode.Repeat;
		texture.filterMode = FilterMode.Trilinear;
		texture.anisoLevel = colorTexture ? 5 : 4;
	}

	private void DisableRoadTexture()
	{
		roadTexture = null;
		roadNormalTexture = null;
		roadAoTexture = null;
		roadTextureOffset = 0f;
		roadMatHasBaseMap = roadMat != null && roadMat.HasProperty(BaseMapProperty);
		roadMatHasMainTex = roadMat != null && roadMat.HasProperty(MainTexProperty);
		roadMatHasBumpMap = roadMat != null && roadMat.HasProperty(BumpMapProperty);
		roadMatHasOcclusionMap = roadMat != null && roadMat.HasProperty(OcclusionMapProperty);
		if (roadMat == null)
		{
			return;
		}
		if (roadMat.HasProperty("_BaseColor"))
		{
			roadMat.SetColor("_BaseColor", new Color(0.012f, 0.012f, 0.014f));
		}
		if (roadMat.HasProperty("_Color"))
		{
			roadMat.SetColor("_Color", new Color(0.012f, 0.012f, 0.014f));
		}
		if (roadMatHasBaseMap)
		{
			roadMat.SetTexture(BaseMapProperty, null);
		}
		if (roadMatHasMainTex)
		{
			roadMat.SetTexture(MainTexProperty, null);
		}
		if (roadMatHasBumpMap)
		{
			roadMat.SetTexture(BumpMapProperty, null);
		}
		if (roadMatHasOcclusionMap)
		{
			roadMat.SetTexture(OcclusionMapProperty, null);
		}
		roadMat.DisableKeyword("_NORMALMAP");
		roadMat.DisableKeyword("_OCCLUSIONMAP");
	}

	private void UpdateRoadTexture(float move)
	{
		if (roadMat == null)
		{
			return;
		}
		roadTextureOffset = Mathf.Repeat(roadTextureOffset + move / (RoadTextureLength / RoadTextureTiling.y), 1f);
		Vector2 offset = new Vector2(0f, roadTextureOffset);
		if (roadMatHasBaseMap)
		{
			roadMat.SetTextureOffset(BaseMapProperty, offset);
		}
		if (roadMatHasMainTex)
		{
			roadMat.SetTextureOffset(MainTexProperty, offset);
		}
		if (roadMatHasBumpMap)
		{
			roadMat.SetTextureOffset(BumpMapProperty, offset);
		}
		if (roadMatHasOcclusionMap)
		{
			roadMat.SetTextureOffset(OcclusionMapProperty, offset);
		}
	}

	private Material Mat(Color color, Color? emission = null)
	{
		if (cachedShader == null)
		{
			cachedShader = Shader.Find("Universal Render Pipeline/Lit");
			if (cachedShader == null)
			{
				cachedShader = Shader.Find("Standard");
			}
			if (cachedShader == null)
			{
				cachedShader = Shader.Find("Sprites/Default");
			}
		}
		Material mat = new Material(cachedShader);
		mat.enableInstancing = true;
		mat.color = color;
		if (mat.HasProperty("_Smoothness"))
		{
			mat.SetFloat("_Smoothness", 0f);
		}
		if (mat.HasProperty("_Metallic"))
		{
			mat.SetFloat("_Metallic", 0f);
		}
		if (mat.HasProperty("_SpecColor"))
		{
			mat.SetColor("_SpecColor", Color.black);
		}
		if (emission.HasValue)
		{
			mat.EnableKeyword("_EMISSION");
			mat.SetColor("_EmissionColor", emission.Value);
		}
		return mat;
	}

	private void ApplyBuildingSurfaceFinish(Material mat)
	{
		if (mat == null)
		{
			return;
		}
		if (mat.HasProperty("_Smoothness"))
		{
			mat.SetFloat("_Smoothness", 0.76f);
		}
		if (mat.HasProperty("_Glossiness"))
		{
			mat.SetFloat("_Glossiness", 0.76f);
		}
		if (mat.HasProperty("_Roughness"))
		{
			mat.SetFloat("_Roughness", 0.24f);
		}
		if (mat.HasProperty("_Metallic"))
		{
			mat.SetFloat("_Metallic", 0.28f);
		}
		if (mat.HasProperty("_MetallicFactor"))
		{
			mat.SetFloat("_MetallicFactor", 0.28f);
		}
		if (mat.HasProperty("_SpecularHighlights"))
		{
			mat.SetFloat("_SpecularHighlights", 1f);
		}
		if (mat.HasProperty("_EnvironmentReflections"))
		{
			mat.SetFloat("_EnvironmentReflections", 1f);
		}
		if (mat.HasProperty("_SpecColor"))
		{
			mat.SetColor("_SpecColor", new Color(0.36f, 0.3f, 0.3f));
		}
	}

	private void ApplyBuildingEmission(Material mat)
	{
		if (mat == null || !mat.HasProperty(EmissionColorProperty))
		{
			return;
		}
		Texture emissionMap = mat.HasProperty(EmissionMapProperty) ? mat.GetTexture(EmissionMapProperty) : null;
		Texture baseMap = null;
		if (mat.HasProperty(BaseMapProperty))
		{
			baseMap = mat.GetTexture(BaseMapProperty);
		}
		if (baseMap == null && mat.HasProperty(MainTexProperty))
		{
			baseMap = mat.GetTexture(MainTexProperty);
		}
		string name = mat.name == null ? "" : mat.name.ToLowerInvariant();
		Color baseColor = mat.HasProperty(BaseColorProperty) ? mat.GetColor(BaseColorProperty) : (mat.HasProperty(ColorProperty) ? mat.GetColor(ColorProperty) : Color.white);
		bool looksEmissive = name.Contains("emiss") || name.Contains("window") || name.Contains("light") || name.Contains("yellow") || (baseColor.r > 0.62f && baseColor.g > 0.42f && baseColor.b < 0.32f);
		bool hasEmissionMap = emissionMap != null;
		if (!looksEmissive && !hasEmissionMap)
		{
			return;
		}
		mat.EnableKeyword("_EMISSION");
		mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
		if (mat.HasProperty(EmissionMapProperty) && emissionMap == null && baseMap != null)
		{
			mat.SetTexture(EmissionMapProperty, baseMap);
		}
		mat.SetColor(EmissionColorProperty, new Color(1f, 0.36f, 0.06f, 1f) * 5.5f);
	}

	private void ApplyBuildingSurfaceFinish(GameObject building)
	{
		if (building == null)
		{
			return;
		}
		Shader unlitShader = Shader.Find("Universal Render Pipeline/Unlit");
		if (unlitShader == null)
		{
			unlitShader = Shader.Find("Unlit/Texture");
		}
		if (unlitShader == null)
		{
			Debug.LogWarning("No unlit shader found for city buildings.");
			return;
		}
		Renderer[] renderers = building.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			if (renderer == null)
			{
				continue;
			}
			renderer.receiveShadows = false;
			renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			renderer.lightProbeUsage = LightProbeUsage.Off;
			Material[] materials = renderer.sharedMaterials;
			for (int m = 0; m < materials.Length; m++)
			{
				materials[m] = GetCityUnlitMaterial(materials[m], unlitShader);
			}
			renderer.sharedMaterials = materials;
		}
	}

	private Material GetCityUnlitMaterial(Material source, Shader unlitShader)
	{
		if (source == null || unlitShader == null)
		{
			return source;
		}
		if (cityUnlitMaterials.TryGetValue(source, out Material cached) && cached != null)
		{
			return cached;
		}
		Material unlit = new Material(source);
		ConvertBuildingMaterialToUnlit(unlit, unlitShader);
		cityUnlitMaterials[source] = unlit;
		return unlit;
	}

	private void ConvertBuildingMaterialToUnlit(Material material, Shader unlitShader)
	{
		if (material == null || unlitShader == null)
		{
			return;
		}
		Texture baseTexture = null;
		Vector2 scale = Vector2.one;
		Vector2 offset = Vector2.zero;
		if (material.HasProperty(BaseMapProperty))
		{
			baseTexture = material.GetTexture(BaseMapProperty);
			scale = material.GetTextureScale(BaseMapProperty);
			offset = material.GetTextureOffset(BaseMapProperty);
		}
		if (baseTexture == null && material.HasProperty(MainTexProperty))
		{
			baseTexture = material.GetTexture(MainTexProperty);
			scale = material.GetTextureScale(MainTexProperty);
			offset = material.GetTextureOffset(MainTexProperty);
		}
		if (baseTexture == null)
		{
			baseTexture = FindImportedBuildingBaseTexture(material, out scale, out offset);
		}
		Color baseColor = material.HasProperty(BaseColorProperty) ? material.GetColor(BaseColorProperty) : (material.HasProperty(ColorProperty) ? material.GetColor(ColorProperty) : Color.white);
		if (!material.HasProperty(BaseColorProperty) && !material.HasProperty(ColorProperty))
		{
			baseColor = FindImportedBuildingBaseColor(material);
		}
		bool transparent;
		if (baseColor.a < 0.999f || material.renderQueue >= 3000)
		{
			transparent = true;
		}
		else if (material.HasProperty("_Surface"))
		{
			transparent = material.GetFloat("_Surface") > 0.5f;
		}
		else
		{
			transparent = false;
		}
		bool alphaClip = material.IsKeywordEnabled("_ALPHATEST_ON") || (material.HasProperty("_AlphaClip") && material.GetFloat("_AlphaClip") > 0.5f);
		float cutoff = material.HasProperty("_Cutoff") ? material.GetFloat("_Cutoff") : 0.5f;
		material.shader = unlitShader;
		material.name = material.name.Replace(" (Instance)", "") + " City Unlit";
		material.enableInstancing = true;
		if (material.HasProperty(BaseMapProperty))
		{
			material.SetTexture(BaseMapProperty, baseTexture);
			material.SetTextureScale(BaseMapProperty, scale);
			material.SetTextureOffset(BaseMapProperty, offset);
		}
		if (material.HasProperty(MainTexProperty))
		{
			material.SetTexture(MainTexProperty, baseTexture);
			material.SetTextureScale(MainTexProperty, scale);
			material.SetTextureOffset(MainTexProperty, offset);
		}
		if (material.HasProperty(BaseColorProperty))
		{
			material.SetColor(BaseColorProperty, baseColor);
		}
		if (material.HasProperty(ColorProperty))
		{
			material.SetColor(ColorProperty, baseColor);
		}
		if (alphaClip)
		{
			if (material.HasProperty("_AlphaClip"))
			{
				material.SetFloat("_AlphaClip", 1f);
			}
			if (material.HasProperty("_Cutoff"))
			{
				material.SetFloat("_Cutoff", cutoff);
			}
			material.EnableKeyword("_ALPHATEST_ON");
		}
		if (transparent)
		{
			if (material.HasProperty("_Surface"))
			{
				material.SetFloat("_Surface", 1f);
			}
			if (material.HasProperty("_Blend"))
			{
				material.SetFloat("_Blend", 0f);
			}
			if (material.HasProperty("_SrcBlend"))
			{
				material.SetFloat("_SrcBlend", 5f);
			}
			if (material.HasProperty("_DstBlend"))
			{
				material.SetFloat("_DstBlend", 10f);
			}
			if (material.HasProperty("_ZWrite"))
			{
				material.SetFloat("_ZWrite", 0f);
			}
			material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
			material.renderQueue = 3000;
			return;
		}
		if (material.HasProperty("_Surface"))
		{
			material.SetFloat("_Surface", 0f);
		}
		if (material.HasProperty("_ZWrite"))
		{
			material.SetFloat("_ZWrite", 1f);
		}
		material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
		material.renderQueue = alphaClip ? 2450 : 2000;
	}

	private Texture FindImportedBuildingBaseTexture(Material material, out Vector2 scale, out Vector2 offset)
	{
		scale = Vector2.one;
		offset = Vector2.zero;
		if (material == null || material.shader == null)
		{
			return null;
		}
		Texture best = null;
		int bestScore = int.MinValue;
		Shader shader = material.shader;
		for (int i = 0; i < shader.GetPropertyCount(); i++)
		{
			if (shader.GetPropertyType(i) != ShaderPropertyType.Texture)
			{
				continue;
			}
			string propertyName = shader.GetPropertyName(i);
			Texture texture = material.GetTexture(propertyName);
			if (texture == null)
			{
				continue;
			}
			string description = shader.GetPropertyDescription(i);
			string key = (propertyName + " " + description).ToLowerInvariant();
			int score = 0;
			if (key.Contains("basecolor") || key.Contains("base color"))
			{
				score += 100;
			}
			if (key.Contains("albedo") || key.Contains("diffuse"))
			{
				score += 80;
			}
			if (key.Contains("color") || key.Contains("colour"))
			{
				score += 20;
			}
			if (key.Contains("normal") || key.Contains("bump") || key.Contains("metal") || key.Contains("rough") || key.Contains("occlusion") || key.Contains("emission") || key.Contains("mask"))
			{
				score -= 200;
			}
			if (score > bestScore)
			{
				bestScore = score;
				best = texture;
				scale = material.GetTextureScale(propertyName);
				offset = material.GetTextureOffset(propertyName);
			}
		}
		return best;
	}

	private Color FindImportedBuildingBaseColor(Material material)
	{
		if (material == null || material.shader == null)
		{
			return Color.white;
		}
		Shader shader = material.shader;
		for (int i = 0; i < shader.GetPropertyCount(); i++)
		{
			if (shader.GetPropertyType(i) != ShaderPropertyType.Color)
			{
				continue;
			}
			string propertyName = shader.GetPropertyName(i);
			string description = shader.GetPropertyDescription(i);
			string key = (propertyName + " " + description).ToLowerInvariant();
			if (key.Contains("basecolor") || key.Contains("base color") || key.Contains("albedo") || key.Contains("diffuse"))
			{
				return material.GetColor(propertyName);
			}
		}
		return Color.white;
	}

	private Material MatUnlit(Color color)
	{
		Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
		if (shader == null)
		{
			shader = Shader.Find("Unlit/Color");
		}
		if (shader == null)
		{
			shader = Shader.Find("Sprites/Default");
		}
		Material mat = new Material(shader);
		mat.enableInstancing = true;
		mat.color = color;
		if (mat.HasProperty("_BaseColor"))
		{
			mat.SetColor("_BaseColor", color);
		}
		if (mat.HasProperty("_Color"))
		{
			mat.SetColor("_Color", color);
		}
		return mat;
	}

	private Material MatTransparent(Color color, Color? emission = null)
	{
		Material mat = Mat(color, emission);
		mat.renderQueue = 3100;
		if (mat.HasProperty("_Surface"))
		{
			mat.SetFloat("_Surface", 1f);
		}
		if (mat.HasProperty("_Blend"))
		{
			mat.SetFloat("_Blend", 0f);
		}
		if (mat.HasProperty("_SrcBlend"))
		{
			mat.SetFloat("_SrcBlend", 5f);
		}
		if (mat.HasProperty("_DstBlend"))
		{
			mat.SetFloat("_DstBlend", 10f);
		}
		if (mat.HasProperty("_ZWrite"))
		{
			mat.SetFloat("_ZWrite", 0f);
		}
		mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
		mat.EnableKeyword("_ALPHABLEND_ON");
		return mat;
	}

	private Material MatTransparentUnlit(Color color, Texture texture, bool additive)
	{
		Material mat = MatUnlit(color);
		mat.renderQueue = additive ? 3150 : 3050;
		mat.color = color;
		if (mat.HasProperty("_BaseColor"))
		{
			mat.SetColor("_BaseColor", color);
		}
		if (mat.HasProperty("_Color"))
		{
			mat.SetColor("_Color", color);
		}
		if (texture != null)
		{
			if (mat.HasProperty(BaseMapProperty))
			{
				mat.SetTexture(BaseMapProperty, texture);
			}
			if (mat.HasProperty(MainTexProperty))
			{
				mat.SetTexture(MainTexProperty, texture);
			}
		}
		if (mat.HasProperty("_Surface"))
		{
			mat.SetFloat("_Surface", 1f);
		}
		if (mat.HasProperty("_Blend"))
		{
			mat.SetFloat("_Blend", additive ? 1f : 0f);
		}
		if (mat.HasProperty("_SrcBlend"))
		{
			mat.SetFloat("_SrcBlend", 5f);
		}
		if (mat.HasProperty("_DstBlend"))
		{
			mat.SetFloat("_DstBlend", additive ? 1f : 10f);
		}
		if (mat.HasProperty("_ZWrite"))
		{
			mat.SetFloat("_ZWrite", 0f);
		}
		if (mat.HasProperty("_Cull"))
		{
			mat.SetFloat("_Cull", 0f);
		}
		mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
		mat.EnableKeyword("_ALPHABLEND_ON");
		return mat;
	}

	private Material MakeSmokeParticleMaterial()
	{
		Texture2D texture = Resources.Load<Texture2D>("smoke-particle-texture-11549386025zm1qlmgdcd");
		if (texture == null)
		{
			texture = Resources.Load<Texture2D>("small_smoke");
		}
		if (texture != null)
		{
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Bilinear;
			texture.anisoLevel = 1;
		}
		Shader shader = Shader.Find("ShadowRun/VehicleSmokeMobile");
		if (shader == null)
		{
			shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
		}
		if (shader == null)
		{
			shader = Shader.Find("Particles/Standard Unlit");
		}
		if (shader == null)
		{
			shader = Shader.Find("Universal Render Pipeline/Unlit");
		}
		if (shader == null)
		{
			return MatTransparentUnlit(new Color(1f, 1f, 1f, 0.95f), texture, false);
		}
		Material mat = new Material(shader);
		mat.name = "Runtime Vehicle Smoke Particle";
		mat.color = new Color(1f, 1f, 1f, 0.95f);
		mat.renderQueue = 3150;
		if (texture != null)
		{
			if (mat.HasProperty(BaseMapProperty))
			{
				mat.SetTexture(BaseMapProperty, texture);
			}
			if (mat.HasProperty(MainTexProperty))
			{
				mat.SetTexture(MainTexProperty, texture);
			}
		}
		if (mat.HasProperty("_BaseColor"))
		{
			mat.SetColor("_BaseColor", mat.color);
		}
		if (mat.HasProperty("_Color"))
		{
			mat.SetColor("_Color", mat.color);
		}
		if (mat.HasProperty("_Surface"))
		{
			mat.SetFloat("_Surface", 1f);
		}
		if (mat.HasProperty("_Blend"))
		{
			mat.SetFloat("_Blend", 0f);
		}
		if (mat.HasProperty("_SrcBlend"))
		{
			mat.SetFloat("_SrcBlend", 5f);
		}
		if (mat.HasProperty("_DstBlend"))
		{
			mat.SetFloat("_DstBlend", 10f);
		}
		if (mat.HasProperty("_ZWrite"))
		{
			mat.SetFloat("_ZWrite", 0f);
		}
		if (mat.HasProperty("_Cull"))
		{
			mat.SetFloat("_Cull", 0f);
		}
		mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
		mat.EnableKeyword("_ALPHABLEND_ON");
		return mat;
	}

	private void ApplyVisualQuality()
	{
		int quality = Mathf.Clamp(graphicsIndex, 0, 2);
		ConfigurePerformance();
		float renderScale = quality == 0 ? LowQualityRenderScale : (quality == 1 ? HighQualityRenderScale : UltraQualityRenderScale);
		if (UniversalRenderPipeline.asset != null)
		{
			UniversalRenderPipeline.asset.renderScale = renderScale;
			UniversalRenderPipeline.asset.upscalingFilter = quality < 2 ? UpscalingFilterSelection.FSR : UpscalingFilterSelection.Auto;
			UniversalRenderPipeline.asset.fsrOverrideSharpness = true;
			UniversalRenderPipeline.asset.fsrSharpness = quality == 0 ? 0.68f : (quality == 1 ? 0.82f : 0.55f);
		}
		if (cam != null)
		{
			cam.nearClipPlane = 0.1f;
			cam.farClipPlane = quality == 0 ? LowQualityCameraFar : (quality == 1 ? HighQualityCameraFar : UltraQualityCameraFar);
			cam.allowHDR = true;
			UniversalAdditionalCameraData cameraData = cam.GetComponent<UniversalAdditionalCameraData>();
			if (cameraData != null)
			{
				cameraData.renderPostProcessing = true;
				cameraData.volumeLayerMask = -1;
			}
		}
		if (!dynamicWeatherReady)
		{
			RenderSettings.fog = false;
			RenderSettings.ambientLight = quality == 0 ? new Color(0.045f, 0.014f, 0.023f) : new Color(0.07f, 0.026f, 0.036f);
			if (sceneKeyLight != null)
			{
				sceneKeyLight.intensity = quality == 0 ? 1.25f : (quality == 1 ? 1.5f : 1.72f);
				sceneKeyLight.color = new Color(1f, 0.28f, 0.24f);
			}
		}
		if (roadTexture != null)
		{
			roadTexture.filterMode = FilterMode.Trilinear;
			roadTexture.anisoLevel = quality == 0 ? 1 : (quality == 1 ? 3 : 5);
		}
		if (roadNormalTexture != null)
		{
			roadNormalTexture.filterMode = FilterMode.Trilinear;
			roadNormalTexture.anisoLevel = quality == 0 ? 1 : (quality == 1 ? 3 : 4);
		}
		if (roadAoTexture != null)
		{
			roadAoTexture.filterMode = FilterMode.Trilinear;
			roadAoTexture.anisoLevel = quality == 0 ? 1 : (quality == 1 ? 3 : 4);
		}
		Texture2D skyTexture = skyBackdropTexture as Texture2D;
		if (skyTexture != null)
		{
			skyTexture.filterMode = FilterMode.Trilinear;
			skyTexture.anisoLevel = quality == 0 ? 1 : 2;
		}
	}

	private void SetMaterialColor(Material mat, Color color)
	{
		if (mat == null)
		{
			return;
		}
		mat.color = color;
		if (mat.HasProperty("_BaseColor"))
		{
			mat.SetColor("_BaseColor", color);
		}
		if (mat.HasProperty("_Color"))
		{
			mat.SetColor("_Color", color);
		}
	}
}
