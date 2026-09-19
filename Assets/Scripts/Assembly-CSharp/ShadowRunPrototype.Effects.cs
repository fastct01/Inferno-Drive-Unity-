using System;
using UnityEngine;
using UnityEngine.Rendering;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_EnsureVehicleSmoke, _EmitVehicleSmoke, _ResetVehicleSmoke,
// _SetupPlayerExhaustFire, _CreatePlayerExhaustFire, _SetPlayerExhaustFireActive, _ClearPlayerExhaustFire,
// _CalculateLocalRendererBounds, _MakeLocalBounds.
public sealed partial class ShadowRunPrototype
{
	private void EnsureVehicleSmoke()
	{
		if (vehicleSmoke != null)
		{
			return;
		}
		vehicleSmokeRoot = new GameObject("Vehicle Smoke").transform;
		vehicleSmoke = vehicleSmokeRoot.gameObject.AddComponent<ParticleSystem>();
		vehicleSmokeRenderer = vehicleSmokeRoot.GetComponent<ParticleSystemRenderer>();
		ParticleSystem.MainModule main = vehicleSmoke.main;
		main.loop = true;
		main.playOnAwake = false;
		main.maxParticles = 48;
		main.simulationSpace = ParticleSystemSimulationSpace.World;
		main.scalingMode = ParticleSystemScalingMode.Hierarchy;
		main.startLifetime = new ParticleSystem.MinMaxCurve(0.75f, 1.35f);
		main.startSpeed = new ParticleSystem.MinMaxCurve(0.25f, 0.85f);
		main.startSize3D = false;
		main.startSize = new ParticleSystem.MinMaxCurve(0.4f, 0.9f);
		main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
		main.startColor = new ParticleSystem.MinMaxGradient(Color.white);
		main.gravityModifier = 0.015f;
		ParticleSystem.EmissionModule emission = vehicleSmoke.emission;
		emission.enabled = true;
		emission.rateOverTime = 0f;
		emission.rateOverDistance = 0f;
		ParticleSystem.ShapeModule shape = vehicleSmoke.shape;
		shape.enabled = true;
		shape.shapeType = ParticleSystemShapeType.Cone;
		shape.angle = 21f;
		shape.radius = 0.1f;
		shape.length = 0.18f;
		ParticleSystem.VelocityOverLifetimeModule velocity = vehicleSmoke.velocityOverLifetime;
		velocity.enabled = true;
		velocity.space = ParticleSystemSimulationSpace.Local;
		velocity.x = new ParticleSystem.MinMaxCurve(-0.16f, 0.16f);
		velocity.y = new ParticleSystem.MinMaxCurve(0.02f, 0.14f);
		velocity.z = new ParticleSystem.MinMaxCurve(0.3f, 0.85f);
		ParticleSystem.InheritVelocityModule inherit = vehicleSmoke.inheritVelocity;
		inherit.enabled = true;
		inherit.mode = ParticleSystemInheritVelocityMode.Current;
		inherit.curve = new ParticleSystem.MinMaxCurve(0.5f);
		ParticleSystem.ForceOverLifetimeModule force = vehicleSmoke.forceOverLifetime;
		force.enabled = true;
		force.space = ParticleSystemSimulationSpace.World;
		force.x = new ParticleSystem.MinMaxCurve(0f);
		force.y = new ParticleSystem.MinMaxCurve(-0.07f);
		force.z = new ParticleSystem.MinMaxCurve(0f);
		ParticleSystem.SizeOverLifetimeModule size = vehicleSmoke.sizeOverLifetime;
		size.enabled = true;
		size.separateAxes = false;
		size.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe[3] { new Keyframe(0f, 0.45f), new Keyframe(0.38f, 1.08f), new Keyframe(1f, 1.55f) }));
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = vehicleSmoke.colorOverLifetime;
		colorOverLifetime.enabled = true;
		Gradient gradient = new Gradient();
		gradient.SetKeys(
			new GradientColorKey[3] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(0.82f, 0.86f, 0.9f), 0.55f), new GradientColorKey(new Color(0.28f, 0.29f, 0.31f), 1f) },
			new GradientAlphaKey[4] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.58f, 0.15f), new GradientAlphaKey(0.28f, 0.62f), new GradientAlphaKey(0f, 1f) });
		colorOverLifetime.color = gradient;
		ParticleSystem.NoiseModule noise = vehicleSmoke.noise;
		noise.enabled = true;
		noise.strength = 0.18f;
		noise.frequency = 0.65f;
		noise.scrollSpeed = 0.28f;
		if (vehicleSmokeRenderer != null)
		{
			vehicleSmokeRenderer.renderMode = ParticleSystemRenderMode.Billboard;
			vehicleSmokeRenderer.sharedMaterial = smokeParticleMat;
			vehicleSmokeRenderer.sortingFudge = 8f;
			vehicleSmokeRenderer.maxParticleSize = 0.34f;
			vehicleSmokeRenderer.shadowCastingMode = ShadowCastingMode.Off;
			vehicleSmokeRenderer.receiveShadows = false;
			vehicleSmokeRenderer.lightProbeUsage = LightProbeUsage.Off;
			vehicleSmokeRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		}
		ResetVehicleSmoke();
	}

	private void EmitVehicleSmoke(float dt, bool boost, bool braking, bool handbrake)
	{
		if (player == null || carVisual == null)
		{
			return;
		}
		if (!ActiveVehicle.UsesWheelSmoke)
		{
			ResetVehicleSmoke();
			return;
		}
		EnsureVehicleSmoke();
		if (vehicleSmoke == null || vehicleSmokeRoot == null)
		{
			return;
		}
		Quaternion yaw = Quaternion.Euler(0f, yawVisual, 0f);
		Vector3 origin = player.position + carVisual.localPosition;
		vehicleSmokeRoot.position = origin + yaw * ActiveVehicle.SmokeOffset;
		vehicleSmokeRoot.rotation = yaw;
		float deceleration = dt > 0f ? Mathf.Max(0f, smokeLastSpeed - speed) / dt : 0f;
		smokeLastSpeed = speed;
		float targetRate = 0f;
		if (braking && speed > 8f)
		{
			float speedT = Mathf.InverseLerp(12f, 58f, speed);
			targetRate = brakePressure * Mathf.InverseLerp(8f, 34f, deceleration) * speedT * 58f;
		}
		targetRate = Mathf.Clamp(targetRate, 0f, 58f);
		smokeEmissionRate = targetRate > 0f ? Mathf.Lerp(smokeEmissionRate, targetRate, 1f - Mathf.Exp(-dt * 5f)) : 0f;
		ParticleSystem.EmissionModule emission = vehicleSmoke.emission;
		float rate = smokeEmissionRate;
		emission.rateOverTime = rate;
		emission.rateOverDistance = Mathf.Clamp(rate * 0.08f, 0f, 4.5f);
		if (smokeEmissionRate > 0.05f)
		{
			if (!vehicleSmoke.isPlaying)
			{
				vehicleSmoke.Play();
			}
			return;
		}
		if (vehicleSmoke.isPlaying)
		{
			vehicleSmoke.Stop(false, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	private void ResetVehicleSmoke()
	{
		smokeEmissionRate = 0f;
		if (vehicleSmoke == null)
		{
			return;
		}
		ParticleSystem.EmissionModule emission = vehicleSmoke.emission;
		emission.rateOverTime = 0f;
		emission.rateOverDistance = 0f;
		vehicleSmoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	private void SetupPlayerExhaustFire(Transform attachRoot, Transform vehicleRoot)
	{
		ClearPlayerExhaustFire();
		if (ActiveVehicle != null && string.Equals(ActiveVehicle.Id, "flying_car", StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		if (attachRoot == null || vehicleRoot == null)
		{
			return;
		}
		playerExhaustFirePrefab = Resources.Load<GameObject>(ExhaustFireResource);
		if (playerExhaustFirePrefab == null)
		{
			if (!playerExhaustFireMissingLogged)
			{
				playerExhaustFireMissingLogged = true;
				Debug.LogWarning("Player exhaust fire prefab not found in Resources: JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Fire/CFXR Fire Breath");
			}
			return;
		}
		Transform mainBody = null;
		Transform[] nodes = vehicleRoot.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < nodes.Length; i++)
		{
			if (nodes[i] != null && string.Equals(nodes[i].name, "Main", StringComparison.OrdinalIgnoreCase) && nodes[i].GetComponent<Renderer>() != null)
			{
				mainBody = nodes[i];
				break;
			}
		}
		Bounds bounds = CalculateLocalRendererBounds(vehicleRoot, mainBody != null ? mainBody.gameObject : vehicleRoot.gameObject);
		float exhaustX = Mathf.Max(0.3f, bounds.extents.x * ActiveVehicle.ExhaustXExtentFactor);
		float exhaustY = bounds.min.y + bounds.size.y * ActiveVehicle.ExhaustYHeightFactor;
		bool rearIsMinZ = true;
		if (!string.IsNullOrEmpty(ActiveVehicle.ExhaustReferenceName))
		{
			for (int i = 0; i < nodes.Length; i++)
			{
				Transform node = nodes[i];
				if (node == null || !string.Equals(node.name, ActiveVehicle.ExhaustReferenceName, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				Renderer reference = node.GetComponent<Renderer>();
				if (reference == null)
				{
					reference = node.GetComponentInChildren<Renderer>(true);
				}
				if (reference != null)
				{
					Bounds referenceBounds = MakeLocalBounds(vehicleRoot, reference.bounds);
					rearIsMinZ = referenceBounds.center.z <= bounds.center.z;
				}
				break;
			}
		}
		float exhaustZ = rearIsMinZ ? bounds.min.z - ActiveVehicle.ExhaustRearOffset : bounds.max.z + ActiveVehicle.ExhaustRearOffset;
		playerExhaustFireRoot = new GameObject("Player Exhaust Fire").transform;
		playerExhaustFireRoot.SetParent(vehicleRoot, false);
		playerExhaustFireRoot.localPosition = Vector3.zero;
		playerExhaustFireRoot.localRotation = Quaternion.identity;
		playerExhaustFireRoot.localScale = Vector3.one;
		playerExhaustFireRoot.gameObject.SetActive(false);
		Vector3 leftSocket;
		Vector3 rightSocket;
		Quaternion exhaustTurn;
		if (ActiveVehicle.UsePlayerSpaceExhaustSockets)
		{
			Vector3 leftPlayerSpace = new Vector3(-ActiveVehicle.ExhaustPlayerX, ActiveVehicle.ExhaustPlayerY, ActiveVehicle.ExhaustPlayerZ);
			Vector3 rightPlayerSpace = new Vector3(ActiveVehicle.ExhaustPlayerX, ActiveVehicle.ExhaustPlayerY, ActiveVehicle.ExhaustPlayerZ);
			leftSocket = vehicleRoot.InverseTransformPoint(attachRoot.TransformPoint(leftPlayerSpace));
			rightSocket = vehicleRoot.InverseTransformPoint(attachRoot.TransformPoint(rightPlayerSpace));
			Vector3 forward = vehicleRoot.InverseTransformDirection(attachRoot.TransformDirection(Vector3.forward)).normalized;
			Vector3 up = vehicleRoot.InverseTransformDirection(attachRoot.TransformDirection(Vector3.up)).normalized;
			exhaustTurn = Quaternion.LookRotation(forward, up);
		}
		else
		{
			leftSocket = new Vector3(bounds.center.x - exhaustX, exhaustY, exhaustZ);
			rightSocket = new Vector3(bounds.center.x + exhaustX, exhaustY, exhaustZ);
			exhaustTurn = rearIsMinZ ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.identity;
		}
		CreatePlayerExhaustFire(leftSocket, exhaustTurn);
		CreatePlayerExhaustFire(rightSocket, exhaustTurn);
		playerExhaustFireActive = false;
		playerExhaustFireRoot.gameObject.SetActive(false);
	}

	private void CreatePlayerExhaustFire(Vector3 localPosition, Quaternion exhaustTurn)
	{
		GameObject flame = Instantiate(playerExhaustFirePrefab, playerExhaustFireRoot);
		flame.name = "Exhaust Flame";
		flame.transform.localPosition = localPosition;
		flame.transform.localRotation = playerExhaustFirePrefab.transform.localRotation * exhaustTurn;
		Vector3 scale = playerExhaustFirePrefab.transform.localScale * (0.624f * ActiveVehicle.ExhaustScaleMultiplier);
		if (ActiveVehicle.NormalizeExhaustWorldScale && playerExhaustFireRoot != null)
		{
			Vector3 lossy = playerExhaustFireRoot.lossyScale;
			scale = new Vector3(scale.x / Mathf.Max(Mathf.Abs(lossy.x), 0.0001f), scale.y / Mathf.Max(Mathf.Abs(lossy.y), 0.0001f), scale.z / Mathf.Max(Mathf.Abs(lossy.z), 0.0001f));
		}
		flame.transform.localScale = scale;
		Transform[] nodes = flame.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < nodes.Length; i++)
		{
			Transform node = nodes[i];
			if (node == null || node == flame.transform)
			{
				continue;
			}
			if (string.Equals(node.name, "Point Light", StringComparison.OrdinalIgnoreCase) || string.Equals(node.name, "Smoke", StringComparison.OrdinalIgnoreCase))
			{
				node.gameObject.SetActive(false);
			}
		}
		ParticleSystem[] systems = flame.GetComponentsInChildren<ParticleSystem>(true);
		for (int i = 0; i < systems.Length; i++)
		{
			ParticleSystem system = systems[i];
			if (system == null)
			{
				continue;
			}
			ParticleSystem.MainModule main = system.main;
			main.simulationSpace = ParticleSystemSimulationSpace.Local;
			main.gravityModifier = 0f;
			if (ActiveVehicle.CompactExhaustEffect)
			{
				main.startSpeedMultiplier = main.startSpeedMultiplier * 0.42f;
			}
			ParticleSystem.ShapeModule shape = system.shape;
			if (shape.enabled)
			{
				shape.randomDirectionAmount = 0f;
				shape.sphericalDirectionAmount = 0f;
				shape.angle = Mathf.Min(shape.angle, 10f);
			}
			if (system.transform != flame.transform)
			{
				system.transform.localRotation = Quaternion.identity;
				if (ActiveVehicle.CompactExhaustEffect)
				{
					system.transform.localPosition = Vector3.zero;
				}
			}
		}
	}

	private void SetPlayerExhaustFireActive(bool active)
	{
		if (playerExhaustFireRoot == null || playerExhaustFireActive == active)
		{
			return;
		}
		playerExhaustFireActive = active;
		playerExhaustFireRoot.gameObject.SetActive(active);
	}

	private void ClearPlayerExhaustFire()
	{
		playerExhaustFireActive = false;
		if (playerExhaustFireRoot == null)
		{
			return;
		}
		Destroy(playerExhaustFireRoot.gameObject);
		playerExhaustFireRoot = null;
	}

	private Bounds CalculateLocalRendererBounds(Transform relativeTo, GameObject root)
	{
		Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
		if (renderers.Length == 0)
		{
			return new Bounds(Vector3.zero, Vector3.one);
		}
		Bounds bounds = MakeLocalBounds(relativeTo, renderers[0].bounds);
		for (int i = 1; i < renderers.Length; i++)
		{
			bounds.Encapsulate(MakeLocalBounds(relativeTo, renderers[i].bounds));
		}
		return bounds;
	}

	private Bounds MakeLocalBounds(Transform relativeTo, Bounds worldBounds)
	{
		Vector3 min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		Vector3 max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
		for (int x = 0; x <= 1; x++)
		{
			for (int y = 0; y <= 1; y++)
			{
				for (int z = 0; z <= 1; z++)
				{
					Vector3 corner = new Vector3(x == 0 ? worldBounds.min.x : worldBounds.max.x, y == 0 ? worldBounds.min.y : worldBounds.max.y, z == 0 ? worldBounds.min.z : worldBounds.max.z);
					Vector3 local = relativeTo.InverseTransformPoint(corner);
					min = Vector3.Min(min, local);
					max = Vector3.Max(max, local);
				}
			}
		}
		return new Bounds((min + max) * 0.5f, max - min);
	}
}
