using System;
using UnityEngine;

[Serializable]
public sealed class VehicleDefinition
{
	public string Id;

	public string ModelResource;

	public string PlayerModelName;

	public string GarageModelName;

	public float ModelYawOffset;

	public float RoadYOffset;

	public Vector3 TargetModelSize;

	public float HeadlightOriginHeight;

	public float HeadlightFrontInset;

	public float HeadlightEmitterWidth;

	public float HeadlightEmitterHeightOffset;

	public float HeadlightLeftShaftLocalZ;

	public float HeadlightRightShaftLocalZ;

	public Vector3 SmokeOffset;

	public string[] WheelMeshNames;

	public string[] BrakeDiscNames;

	public string[] BrakeLightNames;

	public string[] BrakeLightMaterialNames;

	public string[] BoundsRendererNameKeywords;

	public bool HideOversizedPlaneRenderers;

	public float ExhaustXExtentFactor = 0.46f;

	public float ExhaustYHeightFactor = 0.055f;

	public float ExhaustRearOffset = 0.15f;

	public string ExhaustReferenceName;

	public bool NormalizeExhaustWorldScale;

	public bool UsePlayerSpaceExhaustSockets;

	public float ExhaustPlayerX;

	public float ExhaustPlayerY;

	public float ExhaustPlayerZ;

	public float ExhaustScaleMultiplier = 1f;

	public bool CompactExhaustEffect;

	public float GarageTargetFootprint;

	public float GarageMaximumScale;

	public float GarageRoadYOffset;

	public float GarageHorizontalOffset;

	public bool HoverEnabled;

	public float HoverSpeed;

	public float HoverHeight;

	public bool UsesWheelSmoke;

	public float ArcadeAcceleration = 1f;

	public float ArcadeSteering = 1f;

	public float ArcadeGrip = 1f;

	public float ArcadeDrift = 1f;
}
