using System.Collections.Generic;
using UnityEngine;

public sealed class TrafficVehicleAppearance : MonoBehaviour
{
	private sealed class PaintBinding
	{
		public Renderer Renderer;

		public int MaterialSlot;
	}

	private const int CombinedLampMaterialSlot = 5;

	private readonly List<PaintBinding> paintBindings;

	private MaterialPropertyBlock paintBlock;

	private Color CurrentPaint__BackingField;

	public Color CurrentPaint
	{
		get
		{
			return CurrentPaint__BackingField;
		}
		private set
		{
			CurrentPaint__BackingField = value;
		}
	}

	public void Bind(Transform modelRoot)
	{
	}

	public void ApplyPaint(Color color)
	{
	}

	public void ResetPaint()
	{
	}

	private static int FindPaintSlot(Renderer renderer, Mesh mesh)
	{
		return 0;
	}

	private static bool IsNonPaintMaterial(Material material)
	{
		return false;
	}

	private static bool Contains(string value, string token)
	{
		return false;
	}
}
