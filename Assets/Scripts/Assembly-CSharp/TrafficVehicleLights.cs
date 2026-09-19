using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   TrafficVehicleLights__ctor_mB8CBE93D8AC75D89260EA88EA5A9DEF4B994ACE5
//   TrafficVehicleLights__cctor_mED2CB06C5E403408CF87D7CD0440FCAB6CE49629
//   TrafficVehicleLights_Bind_mDBDB42B3D22BDFD01F45F2AC2AEE34AA23A4C1E2
//   TrafficVehicleLights_SetState_m9026939C1AACEF9CF2E8D9846D2ADCA5A8064279
//   TrafficVehicleLights_ResetLights_mD84B39DDFA6354C4A2CCD82EEB8921A07C53A682
//   TrafficVehicleLights_EnsureSharedMaterials_m547FC9E866DFDF2368E0DA9910A1B79FCBC50149
//   TrafficVehicleLights_CreateSharedMaterial_mB4C5458FEFF09560021E9E36A0358D46296D0CF7
//   TrafficVehicleLights_CreateVisibleRearBrakeRig_mD6CB3956E782BC9651D3220E9CC0D4DB1431D9BB
//   TrafficVehicleLights_CreateBodyTurnSignals_m2AD295A8CF0458F19F555E6B39C63052FAB49479
//   TrafficVehicleLights_BuildTurnSignalMeshes_mBB7B0F98D16EB8F0A9AAA63A8109195C4DE2B52D
//   TrafficVehicleLights_BuildSignalMesh_mB50AD32CB24BE1EE44F80896BB0A198AD0455B6A
//   TrafficVehicleLights_AddBodySignalRenderer_mE7429FBD16C227E107B5A90796CA4BAD3A3847A3
//   TrafficVehicleLights_AddProxyLamp_m1DF38C82D76DD9CFA4E17F0C9657180CE255BC5D
//   TrafficVehicleLights_ContainsRenderer_m8CDB484B0FC476FD082F16036B5397CDEF9CE532
//   TrafficVehicleLights_Apply_m1FF841C7917D2741B43231D5E3DBCD8ABE91AC11
public sealed class TrafficVehicleLights : MonoBehaviour
{
	private sealed class LampBinding
	{
		public Renderer Renderer;

		public Material[] OriginalMaterials;

		public Material[] LitMaterials;

		public bool ToggleVisibility;
	}

	private sealed class TurnSignalMeshes
	{
		public Mesh Left;

		public Mesh Right;
	}

	private const int W222CombinedLampMaterialSlot = 5;

	private static readonly Dictionary<Mesh, TurnSignalMeshes> sharedTurnSignalMeshes = new Dictionary<Mesh, TurnSignalMeshes>();

	private static readonly List<Material> sharedRuntimeMaterials = new List<Material>(2);

	private static Material sharedBrakeMaterial;

	private static Material sharedIndicatorMaterial;

	private static Material sharedBrakeOffMaterial;

	private static Material sharedIndicatorOffMaterial;

	private static bool missingShaderReported;

	private static bool missingNodesReported;

	private readonly List<LampBinding> brakeLamps = new List<LampBinding>(4);

	private readonly List<LampBinding> leftTurnLamps = new List<LampBinding>(2);

	private readonly List<LampBinding> rightTurnLamps = new List<LampBinding>(2);

	private bool brakeOn;

	private int turnDirection;

	private bool turnBlinkOn;

	public void Bind(Transform modelRoot)
	{
		brakeLamps.Clear();
		leftTurnLamps.Clear();
		rightTurnLamps.Clear();
		if (modelRoot == null || !EnsureSharedMaterials())
		{
			return;
		}
		CreateBodyTurnSignals(modelRoot);
		CreateVisibleRearBrakeRig(modelRoot);
		if (!missingNodesReported && (brakeLamps.Count == 0 || leftTurnLamps.Count == 0 || rightTurnLamps.Count == 0))
		{
			missingNodesReported = true;
			Debug.LogError(string.Format("Traffic lamps were not fully bound: brake={0}, left={1}, right={2}.", brakeLamps.Count, leftTurnLamps.Count, rightTurnLamps.Count));
		}
		ResetLights();
	}

	public void SetState(bool braking, int indicatorDirection, bool blinkOn)
	{
		indicatorDirection = Mathf.Clamp(indicatorDirection, -1, 1);
		if (brakeOn == braking && turnDirection == indicatorDirection && turnBlinkOn == blinkOn)
		{
			return;
		}
		brakeOn = braking;
		turnDirection = indicatorDirection;
		turnBlinkOn = blinkOn;
		Apply(brakeLamps, brakeOn);
		Apply(leftTurnLamps, turnDirection < 0 && turnBlinkOn);
		Apply(rightTurnLamps, turnDirection > 0 && turnBlinkOn);
	}

	public void ResetLights()
	{
		brakeOn = false;
		turnDirection = 0;
		turnBlinkOn = false;
		Apply(brakeLamps, false);
		Apply(leftTurnLamps, false);
		Apply(rightTurnLamps, false);
	}

	private static bool EnsureSharedMaterials()
	{
		if (sharedBrakeMaterial != null && sharedIndicatorMaterial != null && sharedBrakeOffMaterial != null && sharedIndicatorOffMaterial != null)
		{
			return true;
		}
		Shader shader = Shader.Find("ShadowRun/TrafficLampUnlit");
		if (shader == null)
		{
			if (!missingShaderReported)
			{
				missingShaderReported = true;
				Debug.LogError("Required shader ShadowRun/TrafficLampUnlit was not included in the build.");
			}
			return false;
		}
		sharedBrakeMaterial = CreateSharedMaterial(shader, "Traffic Brake Lamp Shared", new Color(5.8f, 0.035f, 0.012f, 1f));
		sharedIndicatorMaterial = CreateSharedMaterial(shader, "Traffic Indicator Lamp Shared", new Color(7.2f, 1.38f, 0.022f, 1f));
		sharedBrakeOffMaterial = CreateSharedMaterial(shader, "Traffic Brake Lens Shared", new Color(0.075f, 0.003f, 0.002f, 1f));
		sharedIndicatorOffMaterial = CreateSharedMaterial(shader, "Traffic Indicator Lens Shared", new Color(0.065f, 0.016f, 0.001f, 1f));
		return true;
	}

	private static Material CreateSharedMaterial(Shader shader, string materialName, Color hdrColor)
	{
		Material material = new Material(shader)
		{
			name = materialName,
			hideFlags = HideFlags.HideAndDontSave,
			enableInstancing = true
		};
		material.SetColor("_BaseColor", hdrColor);
		sharedRuntimeMaterials.Add(material);
		return material;
	}

	private void CreateVisibleRearBrakeRig(Transform modelRoot)
	{
		Renderer[] renderers = modelRoot.GetComponentsInChildren<Renderer>(true);
		bool hasBounds = false;
		Bounds bounds = default(Bounds);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			if (renderer == null || !renderer.enabled)
			{
				continue;
			}
			if (!hasBounds)
			{
				bounds = renderer.bounds;
				hasBounds = true;
			}
			else
			{
				bounds.Encapsulate(renderer.bounds);
			}
		}
		if (!hasBounds)
		{
			return;
		}
		transform.InverseTransformPoint(bounds.center);
		Vector3 min = transform.InverseTransformPoint(bounds.min);
		Vector3 max = transform.InverseTransformPoint(bounds.max);
		float halfWidth = Mathf.Max(0.65f, Mathf.Abs(max.x - min.x) * 0.5f);
		float rearZ = Mathf.Max(min.z, max.z) + 0.025f;
		float lampY = min.y + Mathf.Abs(max.y - min.y) * 0.54f;
		GameObject rig = new GameObject("Traffic Rear Lamp Rig");
		rig.transform.SetParent(transform, false);
		float lampX = halfWidth * 0.56f;
		Vector3 lampScale = new Vector3(Mathf.Max(0.28f, halfWidth * 0.3f), 0.135f, 0.035f);
		AddProxyLamp(rig.transform, "Brake Light L", new Vector3(-lampX, lampY, rearZ), lampScale, brakeLamps, sharedBrakeOffMaterial, sharedBrakeMaterial);
		AddProxyLamp(rig.transform, "Brake Light R", new Vector3(lampX, lampY, rearZ), lampScale, brakeLamps, sharedBrakeOffMaterial, sharedBrakeMaterial);
	}

	private void CreateBodyTurnSignals(Transform modelRoot)
	{
		MeshRenderer body = null;
		MeshFilter bodyFilter = null;
		MeshRenderer[] renderers = modelRoot.GetComponentsInChildren<MeshRenderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			if (!renderers[i].name.EndsWith("W222.Body", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			MeshFilter filter = renderers[i].GetComponent<MeshFilter>();
			if (filter == null || filter.sharedMesh == null)
			{
				continue;
			}
			body = renderers[i];
			bodyFilter = filter;
			break;
		}
		if (body == null || bodyFilter == null)
		{
			return;
		}
		Mesh sourceMesh = bodyFilter.sharedMesh;
		if (!sharedTurnSignalMeshes.TryGetValue(sourceMesh, out TurnSignalMeshes meshes))
		{
			meshes = BuildTurnSignalMeshes(sourceMesh, body, modelRoot);
			sharedTurnSignalMeshes[sourceMesh] = meshes;
		}
		AddBodySignalRenderer(body.transform, "W222 Left Turn Signal", meshes.Left, leftTurnLamps);
		AddBodySignalRenderer(body.transform, "W222 Right Turn Signal", meshes.Right, rightTurnLamps);
	}

	private static TurnSignalMeshes BuildTurnSignalMeshes(Mesh source, Renderer bodyRenderer, Transform modelRoot)
	{
		TurnSignalMeshes result = new TurnSignalMeshes();
		if (!source.isReadable)
		{
			Debug.LogError("W222.Body must have Read/Write enabled to split its combined lamp material.");
			return result;
		}
		Vector3[] vertices = source.vertices;
		Vector3[] normals = source.normals;
		Vector2[] uv = source.uv;
		Vector3[] modelSpace = new Vector3[vertices.Length];
		Bounds bounds = default(Bounds);
		for (int i = 0; i < vertices.Length; i++)
		{
			modelSpace[i] = modelRoot.InverseTransformPoint(bodyRenderer.transform.TransformPoint(vertices[i]));
			if (i == 0)
			{
				bounds = new Bounds(modelSpace[i], Vector3.zero);
			}
			else
			{
				bounds.Encapsulate(modelSpace[i]);
			}
		}
		List<int> leftTriangles = new List<int>(256);
		List<int> rightTriangles = new List<int>(256);
		if (source.subMeshCount <= W222CombinedLampMaterialSlot)
		{
			Debug.LogError(string.Format("W222.Body has {0} submeshes; required lamp Element {1} is missing.", source.subMeshCount, W222CombinedLampMaterialSlot));
			return result;
		}
		int[] triangles = source.GetTriangles(W222CombinedLampMaterialSlot);
		for (int i = 0; i + 2 < triangles.Length; i += 3)
		{
			int a = triangles[i];
			int b = triangles[i + 1];
			int c = triangles[i + 2];
			Vector3 centroid = (modelSpace[a] + modelSpace[b] + modelSpace[c]) / 3f;
			if (Mathf.InverseLerp(bounds.min.y, bounds.max.y, centroid.y) < 0.43f)
			{
				continue;
			}
			List<int> destination = centroid.x < bounds.center.x ? leftTriangles : rightTriangles;
			destination.Add(a);
			destination.Add(b);
			destination.Add(c);
		}
		result.Left = BuildSignalMesh(source, vertices, normals, uv, leftTriangles, "W222 Left Turn Signal Mesh");
		result.Right = BuildSignalMesh(source, vertices, normals, uv, rightTriangles, "W222 Right Turn Signal Mesh");
		return result;
	}

	private static Mesh BuildSignalMesh(Mesh source, Vector3[] vertices, Vector3[] normals, Vector2[] uv, List<int> triangles, string meshName)
	{
		if (triangles.Count == 0)
		{
			return null;
		}
		Mesh mesh = new Mesh
		{
			name = meshName,
			hideFlags = HideFlags.HideAndDontSave,
			indexFormat = source.indexFormat
		};
		mesh.vertices = vertices;
		if (normals != null && normals.Length == vertices.Length)
		{
			mesh.normals = normals;
		}
		if (uv != null && uv.Length == vertices.Length)
		{
			mesh.uv = uv;
		}
		mesh.SetTriangles(triangles, 0, true);
		return mesh;
	}

	private static void AddBodySignalRenderer(Transform body, string objectName, Mesh mesh, List<LampBinding> destination)
	{
		if (mesh == null)
		{
			return;
		}
		GameObject signal = new GameObject(objectName);
		signal.transform.SetParent(body, false);
		signal.transform.localPosition = Vector3.zero;
		signal.transform.localRotation = Quaternion.identity;
		signal.transform.localScale = Vector3.one * 1.0015f;
		signal.AddComponent<MeshFilter>().sharedMesh = mesh;
		MeshRenderer renderer = signal.AddComponent<MeshRenderer>();
		renderer.sharedMaterial = sharedIndicatorMaterial;
		renderer.shadowCastingMode = ShadowCastingMode.Off;
		renderer.receiveShadows = false;
		renderer.lightProbeUsage = LightProbeUsage.Off;
		renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		renderer.enabled = false;
		LampBinding binding = new LampBinding();
		binding.Renderer = renderer;
		binding.OriginalMaterials = new Material[1] { sharedIndicatorMaterial };
		binding.LitMaterials = new Material[1] { sharedIndicatorMaterial };
		binding.ToggleVisibility = true;
		destination.Add(binding);
	}

	private static void AddProxyLamp(Transform parent, string lampName, Vector3 localPosition, Vector3 localScale, List<LampBinding> destination, Material offMaterial, Material onMaterial)
	{
		GameObject lamp = GameObject.CreatePrimitive(PrimitiveType.Cube);
		lamp.name = lampName;
		lamp.transform.SetParent(parent, false);
		lamp.transform.localPosition = localPosition;
		lamp.transform.localRotation = Quaternion.identity;
		lamp.transform.localScale = localScale;
		Collider collider = lamp.GetComponent<Collider>();
		if (collider != null)
		{
			Destroy(collider);
		}
		Renderer renderer = lamp.GetComponent<Renderer>();
		renderer.sharedMaterial = offMaterial;
		renderer.shadowCastingMode = ShadowCastingMode.Off;
		renderer.receiveShadows = false;
		renderer.lightProbeUsage = LightProbeUsage.Off;
		renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		LampBinding binding = new LampBinding();
		binding.Renderer = renderer;
		binding.OriginalMaterials = new Material[1] { offMaterial };
		binding.LitMaterials = new Material[1] { onMaterial };
		destination.Add(binding);
	}

	private static bool ContainsRenderer(List<LampBinding> bindings, Renderer renderer)
	{
		for (int i = 0; i < bindings.Count; i++)
		{
			if (bindings[i].Renderer == renderer)
			{
				return true;
			}
		}
		return false;
	}

	private static void Apply(List<LampBinding> bindings, bool enabled)
	{
		for (int i = 0; i < bindings.Count; i++)
		{
			LampBinding binding = bindings[i];
			if (binding.Renderer == null)
			{
				continue;
			}
			if (binding.ToggleVisibility)
			{
				binding.Renderer.enabled = enabled;
				continue;
			}
			binding.Renderer.sharedMaterials = enabled ? binding.LitMaterials : binding.OriginalMaterials;
		}
	}
}
