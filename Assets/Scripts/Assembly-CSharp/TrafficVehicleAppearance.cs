using System;
using System.Collections.Generic;
using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   TrafficVehicleAppearance__ctor_m25595D293A5F1D73B564664FB267226F637B2C3C
//   TrafficVehicleAppearance_Bind_m2FC4C629CE4EC9EF2A0A1F6F239A325661457222
//   TrafficVehicleAppearance_ApplyPaint_mEE20DCFC06DB60197108920A1B4A18519E9844A4
//   TrafficVehicleAppearance_ResetPaint_m4BCBC2ED39D2EC633E501AA2EFE821C6B7DF0672
//   TrafficVehicleAppearance_FindPaintSlot_mCC90DD666FB6B697A630B2E5E9D6335B73010B9C
//   TrafficVehicleAppearance_IsNonPaintMaterial_m6E2AC6DD8088698C251170F49F4F8E92998AF120
//   TrafficVehicleAppearance_Contains_m40B34C73B7186B2ED1E8B7F053C9AAC967979D0C
public sealed class TrafficVehicleAppearance : MonoBehaviour
{
	private sealed class PaintBinding
	{
		public Renderer Renderer;

		public int MaterialSlot;
	}

	private const int CombinedLampMaterialSlot = 5;

	private readonly List<PaintBinding> paintBindings = new List<PaintBinding>(2);

	private MaterialPropertyBlock paintBlock;

	public Color CurrentPaint { get; private set; } = Color.white;

	public void Bind(Transform modelRoot)
	{
		paintBindings.Clear();
		if (modelRoot == null)
		{
			return;
		}
		MeshRenderer[] renderers = modelRoot.GetComponentsInChildren<MeshRenderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			MeshRenderer renderer = renderers[i];
			if (renderer == null)
			{
				continue;
			}
			MeshFilter filter = renderer.GetComponent<MeshFilter>();
			if (filter == null || filter.sharedMesh == null)
			{
				continue;
			}
			Material[] materials = renderer.sharedMaterials;
			int slotCount = Mathf.Min(materials.Length, filter.sharedMesh.subMeshCount);
			for (int slot = 0; slot < slotCount; slot++)
			{
				Material material = materials[slot];
				if (material != null && Contains(material.name, "car color"))
				{
					paintBindings.Add(new PaintBinding
					{
						Renderer = renderer,
						MaterialSlot = slot
					});
				}
			}
		}
		if (paintBindings.Count != 0)
		{
			return;
		}
		for (int i = 0; i < renderers.Length; i++)
		{
			MeshRenderer renderer = renderers[i];
			if (renderer == null || !renderer.name.EndsWith("W222.Body", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			MeshFilter filter = renderer.GetComponent<MeshFilter>();
			if (filter == null || filter.sharedMesh == null)
			{
				continue;
			}
			int slot = FindPaintSlot(renderer, filter.sharedMesh);
			if (slot < 0)
			{
				return;
			}
			paintBindings.Add(new PaintBinding
			{
				Renderer = renderer,
				MaterialSlot = slot
			});
			return;
		}
	}

	public void ApplyPaint(Color color)
	{
		CurrentPaint = color;
		if (paintBindings.Count == 0)
		{
			return;
		}
		if (paintBlock == null)
		{
			paintBlock = new MaterialPropertyBlock();
		}
		paintBlock.Clear();
		paintBlock.SetColor("_BaseColor", color);
		paintBlock.SetColor("_Color", color);
		for (int i = 0; i < paintBindings.Count; i++)
		{
			PaintBinding binding = paintBindings[i];
			if (binding.Renderer != null)
			{
				binding.Renderer.SetPropertyBlock(paintBlock, binding.MaterialSlot);
			}
		}
	}

	public void ResetPaint()
	{
		CurrentPaint = Color.white;
		for (int i = 0; i < paintBindings.Count; i++)
		{
			PaintBinding binding = paintBindings[i];
			if (binding.Renderer != null)
			{
				binding.Renderer.SetPropertyBlock(null, binding.MaterialSlot);
			}
		}
	}

	private static int FindPaintSlot(Renderer renderer, Mesh mesh)
	{
		Material[] materials = renderer.sharedMaterials;
		int slotCount = Mathf.Min(materials.Length, mesh.subMeshCount);
		int bestSlot = -1;
		long bestScore = long.MinValue;
		for (int slot = 0; slot < slotCount; slot++)
		{
			if (slot == CombinedLampMaterialSlot)
			{
				continue;
			}
			Material material = materials[slot];
			if (material == null || IsNonPaintMaterial(material))
			{
				continue;
			}
			long score = mesh.GetIndexCount(slot);
			string name = material.name ?? string.Empty;
			if (Contains(name, "paint") || Contains(name, "body") || Contains(name, "exterior"))
			{
				score += 1000000000L;
			}
			if (material.renderQueue >= 3000)
			{
				score -= 500000000L;
			}
			if (score > bestScore)
			{
				bestScore = score;
				bestSlot = slot;
			}
		}
		return bestSlot;
	}

	private static bool IsNonPaintMaterial(Material material)
	{
		string name = material.name ?? string.Empty;
		return Contains(name, "glass") || Contains(name, "window") || Contains(name, "wheel") || Contains(name, "tire") || Contains(name, "tyre") || Contains(name, "rim") || Contains(name, "chrome") || Contains(name, "light") || Contains(name, "lamp") || Contains(name, "brake") || Contains(name, "indicator") || Contains(name, "interior") || Contains(name, "mirror") || Contains(name, "badge") || Contains(name, "logo");
	}

	private static bool Contains(string value, string token)
	{
		return value.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
	}
}
