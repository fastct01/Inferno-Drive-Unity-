using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_MakeRoad, _ConfigureRoadReflectionRenderer,
// _CreateRoadReflectionProbe, _RefreshRoadReflectionProbe, _MakeNeonFences, _AddNeonFenceSegment, _AddBoxMesh,
// _AddBoxGeometry, _AddQuad, _UpdateWorld, _MakeCity, _PickCityBuildingIndex, _MakeStreetLamps, _AddStreetLamp,
// _ApplyStreetLampRedEmission, _ApplyStreetLampSideAdjustments, _FitStreetLampModel, _LoadCityBuildingPrefabs,
// _AddBuilding, _GetIndividualBuildingScale, _CenterAndRegroundImportedModel, _RegroundImportedModel, _AddTop,
// _AddWindows, _Quad, _AddCube, _AddPlane, _PreparePrimitive, _Ellipse, _AddLine, _Cone, _PrepareImportedStaticModel,
// _FitImportedBuildingModel, _TryGetActiveRendererBounds, _RendererMatchesBoundsKeywords, _RendererTextContains,
// _MakeSkyClouds, _EnsureCitySilhouetteBackdrop, _MakeRedSkyBackdropTexture, _UpdateSkyClouds, _UpdateSkyBackdropScale,
// _DisableLegacySkyBackdrop, _DestroyRuntimeSkyObject, _MakeTrafficLights, _CreateTrafficLightSignal, _AddSignalLamp,
// _NormalizeTrafficLightModel, _ResetTrafficLights, _UpdateTrafficLights, _ApplyTrafficLightState, _TryGetStopSignal.
public sealed partial class ShadowRunPrototype
{
	private void MakeRoad()
	{
		Transform road = new GameObject("Continuous Road").transform;
		road.position = new Vector3(0f, 0f, -900f);
		GameObject roadSurface = AddPlane(road, "Road Surface", Vector3.zero, new Vector2(InfernoLanes.RoadWidth, RoadTextureLength), roadMat); /* [user request 2026-09-22: five lanes] */ // original: RoadWidth (10.2)
		ConfigureRoadReflectionRenderer(roadSurface);
		AddPlane(road, "Left Edge", new Vector3(-InfernoLanes.EdgeX, 0.025f, 0f), new Vector2(0.07f, 1890f), lineMat); /* [user request 2026-09-22: five lanes] */ // original: -5.1f
		AddPlane(road, "Right Edge", new Vector3(InfernoLanes.EdgeX, 0.025f, 0f), new Vector2(0.07f, 1890f), lineMat); /* [user request 2026-09-22: five lanes] */ // original: 5.1f
		float[] laneMarks = InfernoLanes.DividerX; /* [user request 2026-09-22: five lanes] */ // original: new float[2] { -1.55f, 1.55f } (two dividers between three lanes)
		for (int i = 0; i < laneMarks.Length; i++)
		{
			float x = laneMarks[i];
			AddPlane(road, "Lane Mark", new Vector3(x, 0.03f, 0f), new Vector2(0.035f, 1890f), lineMat);
		}
		AddPlane(road, "Left Concrete City Base", new Vector3(-InfernoLanes.VergeX, 0.012f, 0f), new Vector2(14.02f, 1890f), cityGroundMat); /* [user request 2026-09-22: five lanes] */ // original: -12.09f
		AddPlane(road, "Right Concrete City Base", new Vector3(InfernoLanes.VergeX, 0.012f, 0f), new Vector2(14.02f, 1890f), cityGroundMat); /* [user request 2026-09-22: five lanes] */ // original: 12.09f
	}

	private void ConfigureRoadReflectionRenderer(GameObject roadSurface)
	{
		if (roadSurface == null)
		{
			return;
		}
		roadSurface.isStatic = true;
		Renderer renderer = roadSurface.GetComponent<Renderer>();
		if (renderer == null)
		{
			return;
		}
		renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		renderer.lightProbeUsage = LightProbeUsage.Off;
	}

	private void CreateRoadReflectionProbe()
	{
		RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
		RenderSettings.defaultReflectionResolution = 64;
		RenderSettings.reflectionIntensity = 0.35f;
		DynamicGI.UpdateEnvironment();
		GameObject probeObject = new GameObject("Road Reflection Probe");
		probeObject.transform.position = new Vector3(0f, 5.5f, -120f);
		probeObject.isStatic = true;
		ReflectionProbe probe = probeObject.AddComponent<ReflectionProbe>();
		roadReflectionProbe = probe;
		probe.mode = ReflectionProbeMode.Realtime;
		probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
		probe.timeSlicingMode = ReflectionProbeTimeSlicingMode.IndividualFaces;
		probe.importance = 1;
		probe.intensity = 0.35f;
		probe.boxProjection = true;
		probe.size = new Vector3(InfernoLanes.Widen(34f), 18f, 1900f); /* [user request 2026-09-22: five lanes] */ // original: 34f wide (the box walls sat 11.9 m outside the shipped edge)
		probe.center = new Vector3(0f, -3.5f, -780f);
		probe.resolution = 64;
		probe.nearClipPlane = 0.3f;
		probe.farClipPlane = 950f;
		probe.cullingMask = -1;
	}

	private void RefreshRoadReflectionProbe()
	{
		if (roadReflectionProbe == null)
		{
			return;
		}
		QualitySettings.realtimeReflectionProbes = true;
		roadReflectionProbe.RenderProbe();
	}

	private void MakeNeonFences()
	{
		if (cityRoot == null)
		{
			return;
		}
		int count = Mathf.FloorToInt(CityLoop / NeonFenceSegmentLength);
		for (int i = 0; i < count; i++)
		{
			float z = -9f - i * NeonFenceSegmentLength;
			AddNeonFenceSegment(-1, z);
			AddNeonFenceSegment(1, z - 7.5f);
		}
	}

	private void AddNeonFenceSegment(int side, float z)
	{
		Transform root = new GameObject(side < 0 ? "Left Neon Fence Segment" : "Right Neon Fence Segment").transform;
		root.SetParent(cityRoot, false);
		root.position = new Vector3(side * InfernoLanes.FenceX, 0f, z); /* [user request 2026-09-22: five lanes] */ // original: side * NeonFenceX (6.08)
		root.gameObject.isStatic = false;
		float length = 13.8f;
		float tubeX = -side * 0.28f;
		List<BoxSpec> body = new List<BoxSpec>(18);
		body.Add(new BoxSpec(new Vector3(0f, 0.16f, 0f), new Vector3(0.42f, 0.32f, length)));
		body.Add(new BoxSpec(new Vector3(0f, 0.48f, 0f), new Vector3(0.24f, 0.18f, length * 0.94f)));
		body.Add(new BoxSpec(new Vector3(0f, 1.4f, 0f), new Vector3(0.2f, 0.18f, length * 0.92f)));
		body.Add(new BoxSpec(new Vector3(0f, 2.18f, 0f), new Vector3(0.34f, 0.26f, length)));
		for (int i = 0; i < 3; i++)
		{
			float postZ = Mathf.Lerp(-length * 0.5f, length * 0.5f, i * 0.5f);
			body.Add(new BoxSpec(new Vector3(0f, 1.08f, postZ), new Vector3(0.62f, 2.15f, 0.5f)));
			body.Add(new BoxSpec(new Vector3(0f, 2.32f, postZ), new Vector3(0.72f, 0.22f, 0.62f)));
		}
		for (int i = 0; i < 8; i++)
		{
			float t = (i + 1f) / 9f;
			float slatZ = Mathf.Lerp(-length * 0.42f, length * 0.42f, t);
			body.Add(new BoxSpec(new Vector3(0f, 1.18f, slatZ), new Vector3(0.16f, 1.25f, 0.13f)));
		}
		AddBoxMesh(root, "Neon Fence Body", body, neonFenceBodyMat);
		List<BoxSpec> tubes = new List<BoxSpec>(10);
		tubes.Add(new BoxSpec(new Vector3(tubeX, 2.02f, 0f), new Vector3(0.055f, 0.075f, length * 0.86f)));
		tubes.Add(new BoxSpec(new Vector3(tubeX, 0.68f, 0f), new Vector3(0.055f, 0.075f, length * 0.8f)));
		tubes.Add(new BoxSpec(new Vector3(tubeX, 0.3f, 0f), new Vector3(0.06f, 0.08f, length * 0.9f)));
		for (int i = 0; i < 3; i++)
		{
			float postZ = Mathf.Lerp(-length * 0.5f, length * 0.5f, i * 0.5f);
			tubes.Add(new BoxSpec(new Vector3(tubeX, 1.35f, postZ), new Vector3(0.06f, 1.35f, 0.07f)));
			tubes.Add(new BoxSpec(new Vector3(tubeX, 0.42f, postZ), new Vector3(0.07f, 0.08f, 0.42f)));
		}
		AddBoxMesh(root, "Neon Fence Tubes", tubes, neonFenceMat);
		cityObjects.Add(root);
	}

	private void AddBoxMesh(Transform parent, string name, List<BoxSpec> boxes, Material mat)
	{
		GameObject go = new GameObject(name);
		go.transform.SetParent(parent, false);
		Mesh mesh = new Mesh();
		mesh.name = name + " Mesh";
		List<Vector3> vertices = new List<Vector3>(boxes.Count * 8);
		List<int> triangles = new List<int>(boxes.Count * 36);
		for (int i = 0; i < boxes.Count; i++)
		{
			AddBoxGeometry(vertices, triangles, boxes[i].Center, boxes[i].Size);
		}
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		go.AddComponent<MeshFilter>().sharedMesh = mesh;
		go.AddComponent<MeshRenderer>();
		PreparePrimitive(go, mat);
	}

	private void AddBoxGeometry(List<Vector3> vertices, List<int> triangles, Vector3 center, Vector3 size)
	{
		int start = vertices.Count;
		Vector3 h = size * 0.5f;
		vertices.Add(center + new Vector3(-h.x, -h.y, -h.z));
		vertices.Add(center + new Vector3(h.x, -h.y, -h.z));
		vertices.Add(center + new Vector3(h.x, -h.y, h.z));
		vertices.Add(center + new Vector3(-h.x, -h.y, h.z));
		vertices.Add(center + new Vector3(-h.x, h.y, -h.z));
		vertices.Add(center + new Vector3(h.x, h.y, -h.z));
		vertices.Add(center + new Vector3(h.x, h.y, h.z));
		vertices.Add(center + new Vector3(-h.x, h.y, h.z));
		AddQuad(triangles, start, 0, 1, 2, 3);
		AddQuad(triangles, start, 4, 7, 6, 5);
		AddQuad(triangles, start, 0, 4, 5, 1);
		AddQuad(triangles, start, 1, 5, 6, 2);
		AddQuad(triangles, start, 2, 6, 7, 3);
		AddQuad(triangles, start, 3, 7, 4, 0);
	}

	private void AddQuad(List<int> triangles, int start, int a, int b, int c, int d)
	{
		triangles.Add(start + a);
		triangles.Add(start + b);
		triangles.Add(start + c);
		triangles.Add(start + a);
		triangles.Add(start + c);
		triangles.Add(start + d);
	}

	private void UpdateWorld(float dt)
	{
		float move = speed * dt;
		if (cityRoot != null)
		{
			cityRoot.position = cityRoot.position + Vector3.forward * move;
		}
		for (int i = 0; i < cityObjects.Count; i++)
		{
			Transform item = cityObjects[i];
			if (item.position.z > 38f)
			{
				item.localPosition = item.localPosition - Vector3.forward * CityLoop;
			}
		}
		UpdateRoadTexture(move);
		UpdateTrafficLights(move);
		/* [InfernoLobby patch] */ InfernoDriving.Tick(this, dt);
		/* [InfernoLobby patch] */ InfernoHazards.Tick(this, dt);
		/* [user request 2026-09-26: night drive] */ InfernoNightHazards.Tick(this, dt);
		/* [InfernoLobby patch] */ InfernoStyle.TickTrails(this, dt);
		/* [user request 2026-09-21: maps] */ InfernoMaps.Tick(this, dt);
	}

	private void MakeCity()
	{
		cityRoot = new GameObject("City Scroll Root").transform;
		List<GameObject> prefabs = LoadCityBuildingPrefabs();
		if (prefabs.Count == 0)
		{
			Debug.LogError("No city building models found in Resources.");
			MakeNeonFences();
			MakeStreetLamps();
			MakeTrafficLights();
			return;
		}
		int rows = CityBuildingRowCount;
		for (int row = 0; row < rows; row++)
		{
			int leftIndex = PickCityBuildingIndex(row, -1, prefabs.Count);
			int rightIndex = PickCityBuildingIndex(row, 1, prefabs.Count);
			float z = -18f - row * CityBuildingSpacing;
			AddBuilding(prefabs[leftIndex], -1, z, row);
			AddBuilding(prefabs[rightIndex], 1, z, row + 80);
		}
		MakeNeonFences();
		MakeStreetLamps();
		MakeTrafficLights();
	}

	private int PickCityBuildingIndex(int row, int side, int count)
	{
		if (count <= 1)
		{
			return 0;
		}
		int sideOffset = side < 0 ? 0 : 1;
		return (row * 2 + sideOffset) % count;
	}

	private void MakeStreetLamps()
	{
		if (cityRoot == null)
		{
			return;
		}
		GameObject lampPrefab = Resources.Load<GameObject>(StreetLampResource);
		if (lampPrefab == null)
		{
			Debug.LogWarning("Street lamp model not found in Resources: Meshy_AI_Lone_Street_Lamp_0617180213_texture");
			return;
		}
		int count = Mathf.FloorToInt(CityLoop / StreetLampSpacing);
		for (int i = 0; i < count; i++)
		{
			float z = -8f - i * StreetLampSpacing;
			AddStreetLamp(lampPrefab, -1, z);
			AddStreetLamp(lampPrefab, 1, z);
		}
	}

	private void AddStreetLamp(GameObject lampPrefab, int side, float z)
	{
		Transform root = new GameObject(side < 0 ? "Left Street Lamp" : "Right Street Lamp").transform;
		root.SetParent(cityRoot, false);
		root.position = new Vector3(side * InfernoLanes.LampX, 0f, z); /* [user request 2026-09-22: five lanes] */ // original: side * StreetLampX (5.72)
		root.rotation = Quaternion.Euler(0f, side < 0 ? 0f : 180f, 0f);
		root.gameObject.isStatic = false;
		GameObject model = Instantiate(lampPrefab);
		model.name = "Meshy AI Street Lamp Model";
		model.transform.SetParent(root, false);
		model.transform.localPosition = Vector3.zero;
		model.transform.localRotation = Quaternion.identity;
		model.transform.localScale = Vector3.one;
		PrepareImportedStaticModel(model);
		ApplyStreetLampRedEmission(model);
		FitStreetLampModel(model.transform, root);
		ApplyStreetLampSideAdjustments(model.transform, side);
		cityObjects.Add(root);
	}

	private void ApplyStreetLampRedEmission(GameObject model)
	{
		if (model == null)
		{
			return;
		}
		Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			if (renderer == null)
			{
				continue;
			}
			Material[] materials = renderer.sharedMaterials;
			for (int m = 0; m < materials.Length; m++)
			{
				Material source = materials[m];
				if (source == null)
				{
					continue;
				}
				if (streetLampRedEmissiveMat == null)
				{
					streetLampRedEmissiveMat = new Material(source);
					streetLampRedEmissiveMat.name = "Shared Street Lamp Red Emissive";
					Color emission = new Color(1f, 0.015f, 0.008f, 1f) * 3.2f;
					if (streetLampRedEmissiveMat.HasProperty("_EmissionColor"))
					{
						streetLampRedEmissiveMat.SetColor("_EmissionColor", emission);
					}
					if (streetLampRedEmissiveMat.HasProperty("_EmissiveColor"))
					{
						streetLampRedEmissiveMat.SetColor("_EmissiveColor", emission);
					}
					if (streetLampRedEmissiveMat.HasProperty("_EmissiveFactor"))
					{
						streetLampRedEmissiveMat.SetColor("_EmissiveFactor", emission);
					}
					streetLampRedEmissiveMat.EnableKeyword("_EMISSION");
					streetLampRedEmissiveMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
				}
				materials[m] = streetLampRedEmissiveMat;
			}
			renderer.sharedMaterials = materials;
		}
	}

	private void ApplyStreetLampSideAdjustments(Transform model, int side)
	{
		if (model == null)
		{
			return;
		}
		Vector3 localPosition = model.localPosition;
		if (side > 0)
		{
			localPosition.x = RightStreetLampModelX;
		}
		else
		{
			localPosition.x = LeftStreetLampModelX;
			localPosition.y = LeftStreetLampModelY;
		}
		model.localPosition = localPosition;
	}

	private void FitStreetLampModel(Transform model, Transform localRoot)
	{
		if (!TryGetActiveRendererBounds(model, out Bounds bounds))
		{
			return;
		}
		float scale = bounds.size.y > 0.001f ? StreetLampHeight / bounds.size.y : 1f;
		model.localScale = model.localScale * Mathf.Clamp(scale, 0.02f, 18f);
		if (!TryGetActiveRendererBounds(model, out bounds))
		{
			return;
		}
		Vector3 center = localRoot.InverseTransformPoint(bounds.center);
		Vector3 min = localRoot.InverseTransformPoint(bounds.min);
		model.localPosition = model.localPosition - new Vector3(center.x, min.y - 0.035f, center.z);
	}

	private List<GameObject> LoadCityBuildingPrefabs()
	{
		List<GameObject> prefabs = new List<GameObject>(CityBuildingResources.Length);
		for (int i = 0; i < CityBuildingResources.Length; i++)
		{
			GameObject prefab = Resources.Load<GameObject>(CityBuildingResources[i]);
			if (prefab != null)
			{
				prefabs.Add(prefab);
			}
			else
			{
				Debug.LogWarning("City building model not found in Resources: " + CityBuildingResources[i]);
			}
		}
		return prefabs;
	}

	private void AddBuilding(GameObject buildingPrefab, int side, float z, int seed)
	{
		float height = CityBuildingHeightPattern[Mathf.Abs(seed) % CityBuildingHeightPattern.Length];
		float width = 11.76f;
		float depth = 22.4f;
		float individualScale = GetIndividualBuildingScale(buildingPrefab);
		float inset = (Mathf.Abs(seed) % 3) * 0.65f;
		height *= CityBuildingScale;
		width *= CityBuildingScale;
		depth *= CityBuildingScale;
		float x = side * (InfernoLanes.BuildingX + inset); /* [user request 2026-09-22: five lanes] */ // original: side * (12.5f + inset)
		Transform root = new GameObject("City Building " + buildingPrefab.name + " (" + (side < 0 ? "L" : "R") + ")").transform;
		if (cityRoot != null)
		{
			root.SetParent(cityRoot, false);
		}
		root.position = new Vector3(x, 0f, z);
		root.rotation = Quaternion.Euler(0f, side < 0 ? 90f : -90f, 0f);
		root.gameObject.isStatic = false;
		GameObject model = Instantiate(buildingPrefab);
		model.name = "City Building Model";
		model.transform.SetParent(root, false);
		model.transform.localPosition = Vector3.zero;
		model.transform.localRotation = Quaternion.identity;
		model.transform.localScale = Vector3.one;
		PrepareImportedStaticModel(model);
		ApplyBuildingSurfaceFinish(model);
		FitImportedBuildingModel(model.transform, root, width, depth, height);
		if (!Mathf.Approximately(individualScale, 1f))
		{
			float widthBefore = TryGetActiveRendererBounds(model.transform, out Bounds before) ? before.size.x : 0f;
			model.transform.localScale = model.transform.localScale * individualScale;
			CenterAndRegroundImportedModel(model.transform, root);
			if (widthBefore > 0f && TryGetActiveRendererBounds(model.transform, out Bounds after))
			{
				float shift = (after.size.x - widthBefore) * 0.5f;
				root.position = root.position + Vector3.right * (side * shift);
			}
		}
		cityObjects.Add(root);
	}

	private float GetIndividualBuildingScale(GameObject buildingPrefab)
	{
		if (buildingPrefab == null)
		{
			return 1f;
		}
		switch (buildingPrefab.name)
		{
			case "здание 1": return 1.3f;
			case "здание 2": return 1.3f;
			case "здание 4": return 1.2f;
			case "здание 5": return 1.3f;
			case "здание 7": return 0.7f;
			case "здание 9": return 1.6f;
		}
		return 1f;
	}

	private void CenterAndRegroundImportedModel(Transform model, Transform localRoot)
	{
		if (!TryGetActiveRendererBounds(model, out Bounds bounds))
		{
			return;
		}
		Transform space = localRoot != null ? localRoot : model;
		Vector3 center = space.InverseTransformPoint(bounds.center);
		Vector3 min = space.InverseTransformPoint(bounds.min);
		model.localPosition = model.localPosition - new Vector3(center.x, min.y, center.z);
	}

	private void RegroundImportedModel(Transform model, Transform localRoot)
	{
		if (!TryGetActiveRendererBounds(model, out Bounds bounds))
		{
			return;
		}
		Transform space = localRoot != null ? localRoot : model;
		Vector3 min = space.InverseTransformPoint(bounds.min);
		model.localPosition = model.localPosition - new Vector3(0f, min.y, 0f);
	}

	private void AddTop(Transform root, float type, float width, float height, float depth)
	{
		AddCube(root, "Roof", new Vector3(0f, height / 2f + 0.2f, 0f), new Vector3(width * 1.04f, 0.3f, depth * 1.02f), blackMat);
		if (type == 0f)
		{
			for (int i = 0; i < 3; i++)
			{
				AddCube(root, "Layered Roof", new Vector3(0f, height / 2f + 0.75f + i * 0.65f, 0f), new Vector3(width * (0.75f - i * 0.16f), 0.75f, depth * (0.75f - i * 0.12f)), blackMat);
			}
			AddCube(root, "Antenna", new Vector3(0f, height / 2f + 5f, 0f), new Vector3(0.12f, 6f, 0.12f), blackMat);
			return;
		}
		if (type == 1f)
		{
			GameObject cone = Cone("Pointed Roof", 12);
			cone.name = "Pointed Roof";
			cone.transform.SetParent(root);
			cone.transform.localPosition = new Vector3(0f, height / 2f + 2.4f, 0f);
			cone.transform.localScale = new Vector3(width * 0.55f, 4.8f, width * 0.55f);
			PreparePrimitive(cone, blackMat);
			return;
		}
		if (type == 2f)
		{
			AddCube(root, "Shoulder L", new Vector3(-width * 0.58f, -height * 0.1f, 0f), new Vector3(width * 0.25f, height * 0.55f, depth * 0.86f), purpleBuildingMat);
			AddCube(root, "Shoulder R", new Vector3(width * 0.58f, -height * 0.1f, 0f), new Vector3(width * 0.25f, height * 0.55f, depth * 0.86f), purpleBuildingMat);
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			AddCube(root, "Roof Box", new Vector3(Random.Range(-width * 0.25f, width * 0.25f), height / 2f + 0.65f, Random.Range(-depth * 0.25f, depth * 0.25f)), new Vector3(1.1f, 0.75f, 1.4f), blackMat);
		}
	}

	private void AddWindows(Transform root, int side, float width, float height, float depth, int seed)
	{
		int rows = Mathf.Max(3, Mathf.FloorToInt(height / 2.2f));
		int cols = Mathf.Max(3, Mathf.FloorToInt(depth / 1.1f));
		float x = -side * (width / 2f + 0.025f);
		Quaternion rotation = Quaternion.Euler(0f, side > 0 ? -90f : 90f, 0f);
		for (int r = 0; r < rows; r++)
		{
			for (int c = 0; c < cols; c++)
			{
				bool lit = (r * 13 + c * 7 + seed * 17) % 10 == 0;
				GameObject window = Quad("Window", Vector3.zero, new Vector3(lit ? 0.48f : 0.36f, lit ? 0.62f : 0.48f, 1f), lit ? yellowWindowMat : darkWindowMat);
				window.transform.SetParent(root);
				window.transform.localPosition = new Vector3(x, -height * 0.38f + r * (height * 0.68f / Mathf.Max(1, rows - 1)), -depth * 0.38f + c * (depth * 0.76f / Mathf.Max(1, cols - 1)));
				window.transform.localRotation = rotation;
			}
		}
	}

	private GameObject Quad(string name, Vector3 pos, Vector3 scale, Material mat)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
		go.name = name;
		go.transform.position = pos;
		go.transform.localScale = scale;
		PreparePrimitive(go, mat);
		return go;
	}

	private GameObject AddCube(Transform parent, string name, Vector3 localPos, Vector3 scale, Material mat)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.name = name;
		go.transform.SetParent(parent);
		go.transform.localPosition = localPos;
		go.transform.localScale = scale;
		PreparePrimitive(go, mat);
		return go;
	}

	private GameObject AddPlane(Transform parent, string name, Vector3 localPos, Vector2 size, Material mat)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
		go.name = name;
		go.transform.SetParent(parent);
		go.transform.localPosition = localPos;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = new Vector3(size.x / 10f, 1f, size.y / 10f);
		PreparePrimitive(go, mat);
		return go;
	}

	private void PreparePrimitive(GameObject go, Material mat)
	{
		Renderer renderer = go.GetComponent<Renderer>();
		if (renderer != null)
		{
			renderer.sharedMaterial = mat;
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			renderer.lightProbeUsage = LightProbeUsage.Off;
			renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			renderer.allowOcclusionWhenDynamic = false;
		}
		Collider collider = go.GetComponent<Collider>();
		if (collider != null)
		{
			Destroy(collider);
		}
	}

	private GameObject Ellipse(string name, Transform parent, Vector3 localPos, Vector2 size, Material mat)
	{
		GameObject go = new GameObject(name);
		go.transform.SetParent(parent);
		go.transform.localPosition = localPos;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		int segments = 24;
		Mesh mesh = new Mesh();
		List<Vector3> vertices = new List<Vector3> { Vector3.zero };
		List<int> triangles = new List<int>();
		for (int i = 0; i < segments; i++)
		{
			float angle = i * Mathf.PI * 2f / segments;
			vertices.Add(new Vector3(Mathf.Cos(angle) * size.x * 0.5f, Mathf.Sin(angle) * size.y * 0.5f, 0f));
		}
		for (int i = 0; i < segments; i++)
		{
			triangles.Add(0);
			triangles.Add(1 + i);
			triangles.Add(1 + (i + 1) % segments);
		}
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		go.AddComponent<MeshFilter>().sharedMesh = mesh;
		go.AddComponent<MeshRenderer>();
		PreparePrimitive(go, mat);
		return go;
	}

	private LineRenderer AddLine(Transform parent, string name, Vector3[] points, float width, Material mat)
	{
		LineRenderer line = new GameObject(name).AddComponent<LineRenderer>();
		line.transform.SetParent(parent);
		line.transform.localPosition = Vector3.zero;
		line.transform.localRotation = Quaternion.identity;
		line.useWorldSpace = false;
		line.positionCount = points.Length;
		line.SetPositions(points);
		line.widthMultiplier = width;
		line.numCapVertices = 3;
		line.numCornerVertices = 3;
		line.material = mat;
		return line;
	}

	private GameObject Cone(string name, int sides)
	{
		GameObject go = new GameObject(name);
		Mesh mesh = new Mesh();
		List<Vector3> vertices = new List<Vector3> { new Vector3(0f, 0.5f, 0f), new Vector3(0f, -0.5f, 0f) };
		List<int> triangles = new List<int>();
		for (int i = 0; i < sides; i++)
		{
			float angle = i * Mathf.PI * 2f / sides;
			vertices.Add(new Vector3(Mathf.Cos(angle) * 0.5f, -0.5f, Mathf.Sin(angle) * 0.5f));
		}
		for (int i = 0; i < sides; i++)
		{
			int a = 2 + i;
			int b = 2 + (i + 1) % sides;
			triangles.Add(0);
			triangles.Add(b);
			triangles.Add(a);
			triangles.Add(1);
			triangles.Add(a);
			triangles.Add(b);
		}
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		go.AddComponent<MeshFilter>().sharedMesh = mesh;
		go.AddComponent<MeshRenderer>();
		PreparePrimitive(go, null);
		return go;
	}

	private void PrepareImportedStaticModel(GameObject model)
	{
		DisableImportedModelCamerasAndLights(model);
		Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			renderer.lightProbeUsage = LightProbeUsage.Off;
			renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			renderer.allowOcclusionWhenDynamic = true;
		}
		Collider[] colliders = model.GetComponentsInChildren<Collider>(true);
		for (int i = 0; i < colliders.Length; i++)
		{
			Destroy(colliders[i]);
		}
		Animator[] animators = model.GetComponentsInChildren<Animator>(true);
		for (int i = 0; i < animators.Length; i++)
		{
			Destroy(animators[i]);
		}
		Animation[] animations = model.GetComponentsInChildren<Animation>(true);
		for (int i = 0; i < animations.Length; i++)
		{
			Destroy(animations[i]);
		}
	}

	private void FitImportedBuildingModel(Transform model, Transform localRoot, float targetWidth, float targetDepth, float targetHeight)
	{
		if (!TryGetActiveRendererBounds(model, out Bounds bounds))
		{
			return;
		}
		float scaleX = bounds.size.x > 0.001f ? targetWidth / bounds.size.x : 1f;
		float scaleZ = bounds.size.z > 0.001f ? targetDepth / bounds.size.z : 1f;
		float scaleY = bounds.size.y > 0.001f ? targetHeight / bounds.size.y : 1f;
		float scale = Mathf.Min(new float[3] { scaleX, scaleZ, scaleY });
		model.localScale = model.localScale * Mathf.Clamp(scale, 0.02f, 12f);
		if (!TryGetActiveRendererBounds(model, out bounds))
		{
			return;
		}
		Transform space = localRoot != null ? localRoot : model;
		Vector3 center = space.InverseTransformPoint(bounds.center);
		Vector3 min = space.InverseTransformPoint(bounds.min);
		model.localPosition = model.localPosition - new Vector3(center.x, min.y, center.z);
	}

	private bool TryGetActiveRendererBounds(Transform root, out Bounds bounds, string[] rendererNameKeywords = null)
	{
		bounds = default(Bounds);
		bool found = false;
		Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy || !RendererMatchesBoundsKeywords(renderer, rendererNameKeywords))
			{
				continue;
			}
			if (!found)
			{
				bounds = renderer.bounds;
				found = true;
			}
			else
			{
				bounds.Encapsulate(renderer.bounds);
			}
		}
		return found;
	}

	private bool RendererMatchesBoundsKeywords(Renderer renderer, string[] keywords)
	{
		if (keywords == null || keywords.Length == 0)
		{
			return true;
		}
		if (renderer == null)
		{
			return false;
		}
		for (int i = 0; i < keywords.Length; i++)
		{
			string keyword = keywords[i];
			if (string.IsNullOrWhiteSpace(keyword))
			{
				continue;
			}
			if (RendererTextContains(renderer.name, keyword))
			{
				return true;
			}
			if (RendererTextContains(renderer.gameObject.name, keyword))
			{
				return true;
			}
			Material[] materials = renderer.sharedMaterials;
			for (int m = 0; m < materials.Length; m++)
			{
				Material material = materials[m];
				if (material != null && RendererTextContains(material.name, keyword))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool RendererTextContains(string text, string keyword)
	{
		if (!string.IsNullOrEmpty(text))
		{
			return text.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0;
		}
		return false;
	}

	private void MakeSkyClouds()
	{
		DisableLegacySkyBackdrop();
		EnsureCitySilhouetteBackdrop();
	}

	private void EnsureCitySilhouetteBackdrop()
	{
		if (citySilhouetteBackdrop != null || cam == null)
		{
			return;
		}
		citySilhouetteTexture = Resources.Load<Texture2D>("bg_building_final");
		if (citySilhouetteTexture == null)
		{
			return;
		}
		citySilhouetteTexture.filterMode = FilterMode.Trilinear;
		citySilhouetteTexture.wrapMode = TextureWrapMode.Clamp;
		citySilhouetteTexture.anisoLevel = 4;
		citySilhouetteMaterial = MatTransparentUnlit(Color.white, citySilhouetteTexture, false);
		citySilhouetteMaterial.name = "City Horizon Silhouette Material";
		GameObject quad = Quad("City Horizon Silhouette", Vector3.zero, Vector3.one, citySilhouetteMaterial);
		citySilhouetteBackdrop = quad.transform;
		citySilhouetteBackdrop.SetParent(cam.transform, false);
		citySilhouetteBackdrop.localPosition = new Vector3(-3.9f, 13.8f, 145f);
		citySilhouetteBackdrop.localRotation = Quaternion.identity;
		citySilhouetteBackdrop.localScale = new Vector3(34.818f, 17.409f, 0.415f);
		citySilhouetteBackdrop.SetParent(null, true);
	}

	private Texture2D MakeRedSkyBackdropTexture(int width, int height)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
		texture.name = "Runtime Red Vertical Sky";
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Trilinear;
		for (int y = 0; y < height; y++)
		{
			float v = height > 1 ? y / (height - 1f) : 0f;
			Color bottom = new Color(1f, 0f, 0f);
			Color middle = new Color(0.42f, 0f, 0f);
			Color top = new Color(0.12f, 0.005f, 0.005f);
			Color row = v < 0.62f ? Color.Lerp(bottom, middle, Mathf.SmoothStep(0f, 1f, v / 0.62f)) : Color.Lerp(middle, top, Mathf.SmoothStep(0f, 1f, (v - 0.62f) / 0.38f));
			for (int x = 0; x < width; x++)
			{
				float u = width > 1 ? x / (width - 1f) : 0f;
				float streak = Mathf.Sin((u * 5.2f + Mathf.Sin(v * 4.5f) * 0.08f) * Mathf.PI * 2f) * 0.025f;
				float vignette = Mathf.Pow(Mathf.Abs(u - 0.5f) * 2f, 1.8f) * 0.06f;
				float topFade = Mathf.SmoothStep(0.72f, 1f, v) * 0.08f;
				Color pixel = row;
				pixel.r = Mathf.Clamp01(pixel.r + streak - vignette - topFade);
				pixel.g = Mathf.Clamp01(pixel.g + streak * 0.08f);
				pixel.b = Mathf.Clamp01(pixel.b + streak * 0.05f);
				texture.SetPixel(x, y, pixel);
			}
		}
		texture.Apply(false, true);
		return texture;
	}

	private void UpdateSkyClouds(float dt)
	{
		if (skyBackdrop == null || skyBackdropTexture == null)
		{
			return;
		}
		UpdateSkyBackdropScale(skyBackdrop, skyBackdropTexture, false);
	}

	private void UpdateSkyBackdropScale(Transform backdrop, Texture texture, bool force)
	{
		if (backdrop == null || cam == null || texture == null)
		{
			return;
		}
		float distance = Mathf.Max(1f, backdrop.localPosition.z);
		if (!force && cam.aspect == lastSkyBackdropAspect && cam.fieldOfView == lastSkyBackdropFov && distance == lastSkyBackdropDistance)
		{
			return;
		}
		lastSkyBackdropAspect = cam.aspect;
		lastSkyBackdropFov = cam.fieldOfView;
		lastSkyBackdropDistance = distance;
		float frustumHeight = 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float frustumWidth = frustumHeight * lastSkyBackdropAspect;
		float textureAspect = texture.height > 0 ? (float)texture.width / texture.height : 0.5625f;
		float scaledHeight = Mathf.Max(frustumHeight, frustumWidth / textureAspect) * 1.08f;
		backdrop.localScale = new Vector3(scaledHeight * textureAspect, scaledHeight, 1f);
		float overflow = Mathf.Max(0f, scaledHeight - frustumHeight);
		backdrop.localPosition = new Vector3(0f, 8.5f - overflow * 0.22f, distance);
	}

	private void DisableLegacySkyBackdrop()
	{
		if (skyBackdrop != null)
		{
			Destroy(skyBackdrop.gameObject);
			skyBackdrop = null;
		}
		skyBackdropTexture = null;
		skyClouds.Clear();
		lastSkyBackdropAspect = -1f;
		lastSkyBackdropFov = -1f;
		lastSkyBackdropDistance = -1f;
		DestroyRuntimeSkyObject(ref sky360Cylinder, "SkyBoxCylinder");
		DestroyRuntimeSkyObject(ref cloud360Cylinder, "CloudLayerCylinder");
		sky360Renderer = null;
		cloud360Renderer = null;
		sky360Material = null;
		cloud360Material = null;
		sky360Texture = null;
		cloud360Texture = null;
		if (rainStormSunRoot != null)
		{
			Destroy(rainStormSunRoot.gameObject);
			rainStormSunRoot = null;
		}
		rainStormSunHaloRenderer = null;
		rainStormSunGlowRenderer = null;
		rainStormSunCoreRenderer = null;
		rainStormSunHaloMat = null;
		rainStormSunGlowMat = null;
		rainStormSunCoreMat = null;
		rainStormSunVisibility = 0f;
		GameObject legacyCloudLayer = GameObject.Find("Fast Sky Volumetric Cloud Layer");
		if (legacyCloudLayer != null)
		{
			Destroy(legacyCloudLayer);
		}
	}

	private void DestroyRuntimeSkyObject(ref Transform runtimeObject, string objectName)
	{
		GameObject target = runtimeObject != null ? runtimeObject.gameObject : GameObject.Find(objectName);
		if (target != null)
		{
			Destroy(target);
		}
		runtimeObject = null;
	}

	private void MakeTrafficLights()
	{
		for (int i = 0; i < trafficLights.Count; i++)
		{
			if (trafficLights[i].Root != null)
			{
				Destroy(trafficLights[i].Root.gameObject);
			}
		}
		trafficLights.Clear();
		trafficLightPrefab = null;
	}

	private TrafficLightSignal CreateTrafficLightSignal(float z, int side, int seed)
	{
		Transform root = new GameObject("Traffic Light Signal").transform;
		root.position = new Vector3(side * InfernoLanes.TrafficLightX, 0f, z); /* [user request 2026-09-22: five lanes] */ // original: side * 6.7f
		root.rotation = Quaternion.Euler(0f, side > 0 ? -90f : 90f, 0f);
		Transform modelRoot = root;
		if (trafficLightPrefab != null)
		{
			GameObject model = Instantiate(trafficLightPrefab);
			model.name = "Traffic Light FBX";
			model.transform.SetParent(root, false);
			model.transform.localPosition = Vector3.zero;
			model.transform.localRotation = Quaternion.identity;
			model.transform.localScale = Vector3.one;
			DisableImportedModelCamerasAndLights(model);
			Collider[] colliders = model.GetComponentsInChildren<Collider>(true);
			for (int i = 0; i < colliders.Length; i++)
			{
				Destroy(colliders[i]);
			}
			NormalizeTrafficLightModel(model, 1.25f, 4.8f);
			modelRoot = model.transform;
		}
		else
		{
			AddCube(root, "Pole", new Vector3(0f, 2f, 0f), new Vector3(0.12f, 4f, 0.12f), blackMat);
			AddCube(root, "Light Box", new Vector3(0f, 4.2f, 0f), new Vector3(0.75f, 1.4f, 0.25f), blackMat);
		}
		Renderer red = AddSignalLamp(root, "Red Lamp", new Vector3(0f, 4.95f, -0.34f), particleRedMat);
		Renderer yellow = AddSignalLamp(root, "Yellow Lamp", new Vector3(0f, 4.34f, -0.34f), yellowWindowMat);
		Renderer green = AddSignalLamp(root, "Green Lamp", new Vector3(0f, 3.73f, -0.34f), greenLightMat);
		TrafficLightSignal signal = default(TrafficLightSignal);
		signal.Root = root;
		signal.ModelRoot = modelRoot;
		signal.RedLamp = red;
		signal.YellowLamp = yellow;
		signal.GreenLamp = green;
		signal.Z = z;
		signal.Side = side;
		signal.CycleOffset = seed * 2.7f + Random.Range(0f, 12f);
		signal.State = TrafficLightState.Unknown;
		signal.AppliedState = TrafficLightState.Unknown;
		return signal;
	}

	private Renderer AddSignalLamp(Transform root, string name, Vector3 localPos, Material mat)
	{
		GameObject lamp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		lamp.name = name;
		lamp.transform.SetParent(root, false);
		lamp.transform.localPosition = localPos;
		lamp.transform.localScale = Vector3.one * 0.42f;
		PreparePrimitive(lamp, mat);
		return lamp.GetComponent<Renderer>();
	}

	private void NormalizeTrafficLightModel(GameObject model, float targetWidth, float targetHeight)
	{
		Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
		Bounds bounds = default(Bounds);
		bool found = false;
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			if (renderer == null || !renderer.enabled)
			{
				continue;
			}
			if (!found)
			{
				bounds = renderer.bounds;
				found = true;
			}
			else
			{
				bounds.Encapsulate(renderer.bounds);
			}
		}
		if (!found)
		{
			return;
		}
		float width = Mathf.Max(0.001f, Mathf.Max(bounds.size.x, bounds.size.z));
		float height = Mathf.Max(0.001f, bounds.size.y);
		float scale = Mathf.Min(targetWidth / width, targetHeight / height);
		scale = Mathf.Clamp(scale, 0.02f, 3f);
		Vector3 center = model.transform.parent.InverseTransformPoint(bounds.center);
		Vector3 min = model.transform.parent.InverseTransformPoint(bounds.min);
		model.transform.localScale = Vector3.one * scale;
		model.transform.localPosition = new Vector3(-center.x * scale, -min.y * scale, -center.z * scale);
	}

	private void ResetTrafficLights()
	{
		float z = -900f;
		for (int i = 0; i < trafficLights.Count; i++)
		{
			TrafficLightSignal signal = trafficLights[i];
			z -= Random.Range(TrafficLightMinSpacing, TrafficLightMaxSpacing);
			signal.Z = z;
			signal.Side = Random.value < 0.5f ? -1 : 1;
			signal.Root.position = new Vector3(signal.Side * InfernoLanes.TrafficLightX, 0f, signal.Z); /* [user request 2026-09-22: five lanes] */ // original: signal.Side * 6.7f
			signal.Root.rotation = Quaternion.Euler(0f, signal.Side > 0 ? -90f : 90f, 0f);
			signal.CycleOffset = i * 3.9f + Random.Range(0f, 12f);
			signal.AppliedState = TrafficLightState.Unknown;
			ApplyTrafficLightState(ref signal);
			trafficLights[i] = signal;
		}
	}

	private void UpdateTrafficLights(float move)
	{
		float farthestZ = 0f;
		for (int i = 0; i < trafficLights.Count; i++)
		{
			if (i == 0 || trafficLights[i].Z < farthestZ)
			{
				farthestZ = trafficLights[i].Z;
			}
		}
		for (int i = 0; i < trafficLights.Count; i++)
		{
			TrafficLightSignal signal = trafficLights[i];
			signal.Z += move;
			if (signal.Z > 42f)
			{
				signal.Z = farthestZ - Random.Range(TrafficLightMinSpacing, TrafficLightMaxSpacing);
				farthestZ = signal.Z;
				signal.Side = Random.value < 0.5f ? -1 : 1;
				signal.CycleOffset = Random.Range(0f, 18f);
				signal.AppliedState = TrafficLightState.Unknown;
			}
			signal.Root.position = new Vector3(signal.Side * InfernoLanes.TrafficLightX, 0f, signal.Z); /* [user request 2026-09-22: five lanes] */ // original: signal.Side * 6.7f
			signal.Root.rotation = Quaternion.Euler(0f, signal.Side > 0 ? -90f : 90f, 0f);
			ApplyTrafficLightState(ref signal);
			trafficLights[i] = signal;
		}
	}

	private void ApplyTrafficLightState(ref TrafficLightSignal signal)
	{
		float phase = Mathf.Repeat(Time.time + signal.CycleOffset, 12f);
		TrafficLightState state = phase < 6.8f ? TrafficLightState.Green : (phase < 8.6f ? TrafficLightState.Yellow : TrafficLightState.Red);
		if (signal.State == state && signal.AppliedState == state)
		{
			return;
		}
		signal.State = state;
		signal.AppliedState = state;
		if (signal.RedLamp != null)
		{
			signal.RedLamp.sharedMaterial = signal.State == TrafficLightState.Red ? particleRedMat : redLightOffMat;
		}
		if (signal.YellowLamp != null)
		{
			signal.YellowLamp.sharedMaterial = signal.State == TrafficLightState.Yellow ? yellowWindowMat : yellowLightOffMat;
		}
		if (signal.GreenLamp != null)
		{
			signal.GreenLamp.sharedMaterial = signal.State == TrafficLightState.Green ? greenLightMat : greenLightOffMat;
		}
	}

	private bool TryGetStopSignal(float z, int lane, out float stopZ)
	{
		stopZ = 0f;
		bool found = false;
		float nearest = 999f;
		for (int i = 0; i < trafficLights.Count; i++)
		{
			TrafficLightSignal signal = trafficLights[i];
			if (signal.State == TrafficLightState.Green)
			{
				continue;
			}
			float gap = z - signal.Z;
			if (gap < 0f || gap > 95f || gap >= nearest)
			{
				continue;
			}
			nearest = gap;
			stopZ = signal.Z + TrafficStopLineOffset;
			found = true;
		}
		return found;
	}
}
