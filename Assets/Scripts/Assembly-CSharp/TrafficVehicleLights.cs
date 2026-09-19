using System.Collections.Generic;
using UnityEngine;

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

	private static readonly Dictionary<Mesh, TurnSignalMeshes> sharedTurnSignalMeshes;

	private static readonly List<Material> sharedRuntimeMaterials;

	private static Material sharedBrakeMaterial;

	private static Material sharedIndicatorMaterial;

	private static Material sharedBrakeOffMaterial;

	private static Material sharedIndicatorOffMaterial;

	private static bool missingShaderReported;

	private static bool missingNodesReported;

	private readonly List<LampBinding> brakeLamps;

	private readonly List<LampBinding> leftTurnLamps;

	private readonly List<LampBinding> rightTurnLamps;

	private bool brakeOn;

	private int turnDirection;

	private bool turnBlinkOn;

	public void Bind(Transform modelRoot)
	{
	}

	public void SetState(bool braking, int indicatorDirection, bool blinkOn)
	{
	}

	public void ResetLights()
	{
	}

	private static bool EnsureSharedMaterials()
	{
		return false;
	}

	private static Material CreateSharedMaterial(Shader shader, string materialName, Color hdrColor)
	{
		return null;
	}

	private void CreateVisibleRearBrakeRig(Transform modelRoot)
	{
	}

	private void CreateBodyTurnSignals(Transform modelRoot)
	{
	}

	private static TurnSignalMeshes BuildTurnSignalMeshes(Mesh source, Renderer bodyRenderer, Transform modelRoot)
	{
		return null;
	}

	private static Mesh BuildSignalMesh(Mesh source, Vector3[] vertices, Vector3[] normals, Vector2[] uv, List<int> triangles, string meshName)
	{
		return null;
	}

	private static void AddBodySignalRenderer(Transform body, string objectName, Mesh mesh, List<LampBinding> destination)
	{
	}

	private static void AddProxyLamp(Transform parent, string lampName, Vector3 localPosition, Vector3 localScale, List<LampBinding> destination, Material offMaterial, Material onMaterial)
	{
	}

	private static bool ContainsRenderer(List<LampBinding> bindings, Renderer renderer)
	{
		return false;
	}

	private static void Apply(List<LampBinding> bindings, bool enabled)
	{
	}
}
