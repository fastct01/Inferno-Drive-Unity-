using UnityEngine;
using UnityEngine.Rendering;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_PrewarmRampPool, _MakeRampLightingIndependent,
// _RampHighEndFacesPlayer, _FitRampModel, _MakeRampDebris, _MakeRampNeonSign, _AddRampNeonSignBar, _AddRampDebrisPiece,
// _ReleaseAllRamps, _ReleaseRamp, _SpawnRamp, _UpdateRamps, _LaunchPlayerFromRamp, _BreakRampFromSide, _UpdateBrokenRamp,
// _RedirectTrafficFromRamp, _PreferredRampEscapeLane, _TrafficSpawnBlockedByRamp, _TrafficNeedsRampEscape,
// _FindRampSafeTrafficLane.
public sealed partial class ShadowRunPrototype
{
	private void PrewarmRampPool()
	{
		bool poolIntact = rampPool.Count == RampPoolSize;
		if (poolIntact)
		{
			for (int i = 0; i < rampPool.Count; i++)
			{
				if (rampPool[i] == null || rampPool[i].Root == null)
				{
					poolIntact = false;
					break;
				}
			}
		}
		if (poolIntact)
		{
			return;
		}
		for (int i = 0; i < rampPool.Count; i++)
		{
			GameObject root = rampPool[i] != null ? rampPool[i].Root : null;
			if (root != null)
			{
				Destroy(rampPool[i].Root);
			}
		}
		rampPool.Clear();
		GameObject prefab = Resources.Load<GameObject>(RampResource);
		if (prefab == null)
		{
			if (!rampMissingLogged)
			{
				rampMissingLogged = true;
				Debug.LogWarning("Ramp model not found in Resources: рампа");
			}
			return;
		}
		for (int i = 0; i < RampPoolSize; i++)
		{
			RampObstacle ramp = new RampObstacle();
			ramp.Root = new GameObject("Ramp Pooled " + (i + 1).ToString());
			ramp.Model = Instantiate(prefab, ramp.Root.transform, false);
			ramp.Model.name = "Ramp Model";
			DisableImportedModelCamerasAndLights(ramp.Model);
			MakeRampLightingIndependent(ramp.Model);
			Collider[] colliders = ramp.Model.GetComponentsInChildren<Collider>(true);
			for (int c = 0; c < colliders.Length; c++)
			{
				Destroy(colliders[c]);
			}
			Renderer[] renderers = ramp.Model.GetComponentsInChildren<Renderer>(true);
			for (int r = 0; r < renderers.Length; r++)
			{
				Renderer renderer = renderers[r];
				renderer.shadowCastingMode = ShadowCastingMode.Off;
				renderer.receiveShadows = false;
				renderer.lightProbeUsage = LightProbeUsage.Off;
				renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			}
			if (RampHighEndFacesPlayer(ramp.Model.transform))
			{
				ramp.Model.transform.localRotation = ramp.Model.transform.localRotation * Quaternion.Euler(0f, 180f, 0f);
			}
			FitRampModel(ramp.Model.transform, ramp.Root.transform);
			MakeRampNeonSign(ramp);
			MakeRampDebris(ramp);
			ramp.Root.SetActive(false);
			rampPool.Add(ramp);
		}
	}

	private void MakeRampLightingIndependent(GameObject model)
	{
		if (model == null)
		{
			return;
		}
		if (rampUnlitMat == null)
		{
			Renderer firstRenderer = model.GetComponentInChildren<Renderer>(true);
			Material source = firstRenderer != null ? firstRenderer.sharedMaterial : null;
			Shader shader = Resources.Load<Shader>(RampUnlitShaderResource);
			if (shader == null)
			{
				Debug.LogError("Ramp unlit shader is missing from Resources: Shaders/RampUnlit");
				return;
			}
			rampUnlitMat = new Material(shader);
			rampUnlitMat.name = "Shared Ramp Original Color Unlit";
			if (source != null)
			{
				Texture baseTexture = source.HasProperty("baseColorTexture")
					? source.GetTexture("baseColorTexture")
					: (source.HasProperty(BaseMapProperty) ? source.GetTexture(BaseMapProperty) : source.mainTexture);
				Color baseColor = source.HasProperty("baseColorFactor")
					? source.GetColor("baseColorFactor")
					: (source.HasProperty(BaseColorProperty) ? source.GetColor(BaseColorProperty) : (source.HasProperty(ColorProperty) ? source.GetColor(ColorProperty) : Color.white));
				rampUnlitMat.SetTexture("baseColorTexture", baseTexture);
				rampUnlitMat.SetColor("baseColorFactor", baseColor);
				if (source.HasProperty("baseColorTexture_ST"))
				{
					rampUnlitMat.SetVector("baseColorTexture_ST", source.GetVector("baseColorTexture_ST"));
				}
			}
		}
		if (rampUnlitMat == null)
		{
			return;
		}
		Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			Material[] materials = renderer.sharedMaterials;
			for (int m = 0; m < materials.Length; m++)
			{
				materials[m] = rampUnlitMat;
			}
			renderer.sharedMaterials = materials;
		}
	}

	private bool RampHighEndFacesPlayer(Transform modelRoot)
	{
		MeshFilter filter = modelRoot.GetComponentInChildren<MeshFilter>(true);
		if (filter == null || filter.sharedMesh == null || !filter.sharedMesh.isReadable)
		{
			return false;
		}
		Vector3[] vertices = filter.sharedMesh.vertices;
		if (vertices == null || vertices.Length == 0)
		{
			return false;
		}
		float minZ = float.PositiveInfinity;
		float maxZ = float.NegativeInfinity;
		for (int i = 0; i < vertices.Length; i++)
		{
			float z = modelRoot.InverseTransformPoint(filter.transform.TransformPoint(vertices[i])).z;
			minZ = Mathf.Min(minZ, z);
			maxZ = Mathf.Max(maxZ, z);
		}
		float band = Mathf.Max(0.001f, (maxZ - minZ) * 0.18f);
		float nearHeight = 0f;
		float farHeight = 0f;
		int nearCount = 0;
		int farCount = 0;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 local = modelRoot.InverseTransformPoint(filter.transform.TransformPoint(vertices[i]));
			if (local.z <= minZ + band)
			{
				nearHeight += local.y;
				nearCount++;
			}
			if (local.z >= maxZ - band)
			{
				farHeight += local.y;
				farCount++;
			}
		}
		if (nearCount == 0 || farCount == 0)
		{
			return false;
		}
		return farHeight / farCount > nearHeight / nearCount;
	}

	private void FitRampModel(Transform model, Transform root)
	{
		if (!TryGetActiveRendererBounds(model, out Bounds bounds))
		{
			return;
		}
		Vector3 scale = model.localScale;
		scale.x *= RampWidth / Mathf.Max(0.001f, bounds.size.x);
		scale.y *= RampHeight / Mathf.Max(0.001f, bounds.size.y);
		scale.z *= RampLength / Mathf.Max(0.001f, bounds.size.z);
		model.localScale = scale;
		if (!TryGetActiveRendererBounds(model, out bounds))
		{
			return;
		}
		Vector3 center = root.InverseTransformPoint(bounds.center);
		Vector3 min = root.InverseTransformPoint(bounds.min);
		model.localPosition = model.localPosition - new Vector3(center.x, min.y, center.z);
	}

	private void MakeRampDebris(RampObstacle ramp)
	{
		ramp.DebrisRoot = new GameObject("Pooled Wooden Debris").transform;
		ramp.DebrisRoot.SetParent(ramp.Root.transform, false);
		Material material = null;
		Renderer renderer = ramp.Model.GetComponentInChildren<Renderer>(true);
		if (renderer != null)
		{
			material = renderer.sharedMaterial;
		}
		for (int i = 0; i < 6; i++)
		{
			float z = Mathf.Lerp(-1.995f, 1.995f, i / 5f);
			float y = Mathf.Lerp(0.8736f, 0.1456f, i / 5f);
			AddRampDebrisPiece(ramp, "Ramp Plank " + (i + 1).ToString(), new Vector3(0f, y, z), new Vector3(2.5192f, 0.12f, 0.7125f), material);
		}
		AddRampDebrisPiece(ramp, "Ramp Rail L", new Vector3(-1.0184f, 0.4144f, 0f), new Vector3(0.15f, 0.17f, 4.275f), material);
		AddRampDebrisPiece(ramp, "Ramp Rail R", new Vector3(1.0184f, 0.4144f, 0f), new Vector3(0.15f, 0.17f, 4.275f), material);
		ramp.DebrisRoot.gameObject.SetActive(false);
	}

	private void MakeRampNeonSign(RampObstacle ramp)
	{
		if (rampNeonYellowMat == null)
		{
			Color yellow = new Color(1f, 0.72f, 0.015f, 1f);
			rampNeonYellowMat = Mat(yellow, yellow * 7.5f);
			rampNeonYellowMat.name = "Ramp Neon Yellow";
		}
		ramp.NeonSign = new GameObject("Ramp Neon Top Sign");
		ramp.NeonSign.transform.SetParent(ramp.Root.transform, false);
		float signZ = -1.4725f;
		float slopeT = Mathf.Clamp01((RampLength * 0.5f - signZ) / RampLength);
		float signY = RampHeight * slopeT + 0.055f;
		float slopeAngle = Mathf.Atan2(RampHeight, RampLength) * Mathf.Rad2Deg;
		AddRampNeonSignBar(ramp.NeonSign.transform, "Neon Chevron L", new Vector3(-0.19f, signY, signZ), Quaternion.Euler(slopeAngle, 55f, 0f));
		AddRampNeonSignBar(ramp.NeonSign.transform, "Neon Chevron R", new Vector3(0.19f, signY, signZ), Quaternion.Euler(slopeAngle, -55f, 0f));
	}

	private void AddRampNeonSignBar(Transform parent, string name, Vector3 position, Quaternion rotation)
	{
		GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
		bar.name = name;
		bar.transform.SetParent(parent, false);
		bar.transform.localPosition = position;
		bar.transform.localRotation = rotation;
		bar.transform.localScale = new Vector3(0.92f, 0.055f, 0.15f);
		Collider collider = bar.GetComponent<Collider>();
		if (collider != null)
		{
			Destroy(collider);
		}
		Renderer renderer = bar.GetComponent<Renderer>();
		if (renderer == null)
		{
			return;
		}
		renderer.sharedMaterial = rampNeonYellowMat;
		renderer.shadowCastingMode = ShadowCastingMode.Off;
		renderer.receiveShadows = false;
		renderer.lightProbeUsage = LightProbeUsage.Off;
		renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
	}

	private void AddRampDebrisPiece(RampObstacle ramp, string name, Vector3 position, Vector3 scale, Material material)
	{
		GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
		piece.name = name;
		piece.transform.SetParent(ramp.DebrisRoot, false);
		piece.transform.localPosition = position;
		piece.transform.localRotation = Quaternion.identity;
		piece.transform.localScale = scale;
		Collider collider = piece.GetComponent<Collider>();
		if (collider != null)
		{
			Destroy(collider);
		}
		Renderer renderer = piece.GetComponent<Renderer>();
		if (renderer != null)
		{
			if (material != null)
			{
				renderer.sharedMaterial = material;
			}
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
		}
		RampDebrisPiece data = new RampDebrisPiece();
		data.Transform = piece.transform;
		data.HomePosition = position;
		data.HomeRotation = Quaternion.identity;
		ramp.Debris.Add(data);
	}

	private void ReleaseAllRamps()
	{
		for (int i = 0; i < rampPool.Count; i++)
		{
			ReleaseRamp(rampPool[i]);
		}
	}

	private void ReleaseRamp(RampObstacle ramp)
	{
		if (ramp == null || ramp.Root == null)
		{
			return;
		}
		ramp.Active = false;
		ramp.Triggered = false;
		ramp.Broken = false;
		ramp.BrokenAge = 0f;
		if (ramp.Model != null)
		{
			ramp.Model.SetActive(true);
		}
		if (ramp.NeonSign != null)
		{
			ramp.NeonSign.SetActive(true);
		}
		if (ramp.DebrisRoot != null)
		{
			ramp.DebrisRoot.gameObject.SetActive(false);
		}
		for (int i = 0; i < ramp.Debris.Count; i++)
		{
			RampDebrisPiece piece = ramp.Debris[i];
			if (piece.Transform == null)
			{
				continue;
			}
			piece.Transform.localPosition = piece.HomePosition;
			piece.Transform.localRotation = piece.HomeRotation;
			piece.Velocity = Vector3.zero;
			piece.AngularVelocity = Vector3.zero;
		}
		ramp.Root.SetActive(false);
		ramp.Root.transform.position = new Vector3(0f, -100f, 0f);
	}

	private bool SpawnRamp()
	{
		RampObstacle ramp = null;
		for (int i = 0; i < rampPool.Count; i++)
		{
			if (rampPool[i] != null && !rampPool[i].Active)
			{
				ramp = rampPool[i];
				break;
			}
		}
		if (ramp == null)
		{
			return false;
		}
		float z = Random.Range(-172f, -150f);
		int startLane = Random.Range(0, lanes.Length);
		int bestLane = startLane;
		float bestScore = -1f;
		for (int offset = 0; offset < lanes.Length; offset++)
		{
			int lane = (startLane + offset) % lanes.Length;
			float nearest = 999f;
			for (int i = 0; i < pickups.Count; i++)
			{
				Pickup item = pickups[i];
				if (item.Transform == null || ClosestLane(item.Transform.position.x) != lane)
				{
					continue;
				}
				nearest = Mathf.Min(nearest, Mathf.Abs(item.Transform.position.z - z));
			}
			float score = nearest + Random.Range(0f, 8f);
			if (score > bestScore)
			{
				bestScore = score;
				bestLane = lane;
			}
		}
		ramp.Lane = bestLane;
		ramp.Active = true;
		ramp.Triggered = false;
		ramp.Broken = false;
		ramp.BrokenAge = 0f;
		ramp.Root.transform.position = new Vector3(lanes[bestLane], 0f, z);
		ramp.Root.transform.rotation = Quaternion.identity;
		ramp.PreviousZ = z;
		if (ramp.Model != null)
		{
			ramp.Model.SetActive(true);
		}
		if (ramp.NeonSign != null)
		{
			ramp.NeonSign.SetActive(true);
		}
		if (ramp.DebrisRoot != null)
		{
			ramp.DebrisRoot.gameObject.SetActive(false);
		}
		ramp.Root.SetActive(true);
		RedirectTrafficFromRamp(ramp);
		return true;
	}

	private void UpdateRamps(float dt)
	{
		if (distance >= nextRampDistance)
		{
			bool anyActive = false;
			for (int i = 0; i < rampPool.Count; i++)
			{
				anyActive |= rampPool[i] != null && rampPool[i].Active;
			}
			if (!anyActive && SpawnRamp())
			{
				rampSpawnCount++;
				nextRampDistance += Random.Range(1000f, 1200f);
			}
		}
		float previousPlayerX = playerX - velocityX * dt;
		for (int i = 0; i < rampPool.Count; i++)
		{
			RampObstacle ramp = rampPool[i];
			if (ramp == null || !ramp.Active || ramp.Root == null)
			{
				continue;
			}
			ramp.PreviousZ = ramp.Root.transform.position.z;
			ramp.Root.transform.position = ramp.Root.transform.position + Vector3.forward * speed * dt;
			float rampZ = ramp.Root.transform.position.z;
			if (!ramp.Triggered && !ramp.Broken)
			{
				RedirectTrafficFromRamp(ramp);
			}
			if (ramp.Broken)
			{
				UpdateBrokenRamp(ramp, dt);
				if (ramp.BrokenAge >= 2.6f)
				{
					ReleaseRamp(ramp);
				}
				continue;
			}
			if (!ramp.Triggered && !playerAirborne)
			{
				float contactZ = player.position.z - VehicleHalfLength - RampLength * 0.5f + 0.2f;
				bool crossedContact = ramp.PreviousZ < contactZ && rampZ >= contactZ;
				float dx = playerX - ramp.Root.transform.position.x;
				float previousDx = previousPlayerX - ramp.Root.transform.position.x;
				float overlap = Mathf.Min(playerX + PlayerHalfWidth, ramp.Root.transform.position.x + RampWidth * 0.5f) - Mathf.Max(playerX - PlayerHalfWidth, ramp.Root.transform.position.x - RampWidth * 0.5f);
				if (crossedContact && overlap > 0.1f)
				{
					float coverage = Mathf.Clamp01(overlap / (PlayerHalfWidth * 2f));
					LaunchPlayerFromRamp(ramp, coverage, dx);
				}
				else
				{
					float sideReach = 2.26f;
					float lengthReach = 3.595f;
					bool alongside = Mathf.Abs(rampZ - player.position.z) < lengthReach;
					bool sweptIn = Mathf.Abs(previousDx) >= sideReach && Mathf.Abs(dx) < sideReach && Mathf.Abs(velocityX) > 4.2f;
					if (alongside && sweptIn)
					{
						BreakRampFromSide(ramp, dx);
					}
				}
			}
			if (ramp.Active && rampZ > player.position.z + 13f)
			{
				ReleaseRamp(ramp);
			}
		}
	}

	private void LaunchPlayerFromRamp(RampObstacle ramp, float coverage, float dx)
	{
		ramp.Triggered = true;
		float speedT = Mathf.InverseLerp(16f, 49f, speed);
		float coverageScale = Mathf.Lerp(0.62f, 1f, coverage);
		float offset = Mathf.Clamp(dx / 2.26f, -1f, 1f);
		float edgeFactor = (1f - coverage) * Mathf.Abs(offset);
		playerAirborne = true;
		playerRampRiding = true;
		playerRampRideElapsed = 0f;
		playerRampRideDuration = Mathf.Clamp(RampLength / Mathf.Max(12f, speed), 0.12f, 0.3f);
		playerRampRiseHeight = RampHeight * Mathf.Lerp(0.78f, 1f, coverage);
		playerRampTakeoffVelocity = Mathf.Lerp(5.85f, 8.8f, speedT) * coverageScale;
		playerJumpHeight = 0f;
		playerJumpVelocity = 0f;
		playerJumpPitch = 0f;
		playerJumpPitchVelocity = -Mathf.Lerp(3.2f, 5.2f, speedT);
		playerJumpRoll = offset * edgeFactor * 5f;
		playerJumpRollVelocity = offset * edgeFactor * 42f;
		playerJumpYaw = -offset * edgeFactor * 8.5f;
		velocityX += offset * edgeFactor * 3.2f;
		impactShake = Mathf.Max(impactShake, Mathf.Lerp(0.1f, 0.22f, speedT));
	}

	private void BreakRampFromSide(RampObstacle ramp, float dx)
	{
		ramp.Triggered = true;
		ramp.Broken = true;
		ramp.BrokenAge = 0f;
		if (ramp.Model != null)
		{
			ramp.Model.SetActive(false);
		}
		if (ramp.NeonSign != null)
		{
			ramp.NeonSign.SetActive(false);
		}
		if (ramp.DebrisRoot != null)
		{
			ramp.DebrisRoot.gameObject.SetActive(true);
		}
		float scatter = dx != 0f ? -Mathf.Sign(dx) : (Random.value < 0.5f ? -1f : 1f);
		for (int i = 0; i < ramp.Debris.Count; i++)
		{
			RampDebrisPiece piece = ramp.Debris[i];
			if (piece.Transform == null)
			{
				continue;
			}
			piece.Velocity = new Vector3(scatter * Random.Range(2.8f, 6.2f) + piece.HomePosition.x * 0.7f, Random.Range(2.8f, 6.5f), Random.Range(-2.4f, 3.4f));
			piece.AngularVelocity = new Vector3(Random.Range(-260f, 260f), Random.Range(-220f, 220f), Random.Range(-300f, 300f));
		}
		float speedBefore = speed;
		speed = InfernoDrivingRules.Impact(speedBefore, Mathf.Max(16f, speedBefore * 0.82f)); /* [InfernoLobby patch] */
		velocityX += -scatter * 2.8f;
		impactYaw = Mathf.Clamp(impactYaw - scatter * 7f, -12f, 12f);
		impactRoll = Mathf.Clamp(impactRoll + scatter * 2.2f, -4f, 4f);
		impactShake = Mathf.Max(impactShake, 0.72f);
		PlayCollisionHaptic(true);
	}

	private void UpdateBrokenRamp(RampObstacle ramp, float dt)
	{
		ramp.BrokenAge += dt;
		for (int i = 0; i < ramp.Debris.Count; i++)
		{
			RampDebrisPiece piece = ramp.Debris[i];
			if (piece.Transform == null)
			{
				continue;
			}
			piece.Velocity = piece.Velocity + Vector3.down * 14f * dt;
			piece.Transform.localPosition = piece.Transform.localPosition + piece.Velocity * dt;
			piece.Transform.Rotate(piece.AngularVelocity * dt, Space.Self);
		}
	}

	private void RedirectTrafficFromRamp(RampObstacle ramp)
	{
		if (ramp == null || ramp.Root == null)
		{
			return;
		}
		float rampZ = ramp.Root.transform.position.z;
		for (int i = 0; i < pickups.Count; i++)
		{
			Pickup item = pickups[i];
			if (item.Transform == null)
			{
				continue;
			}
			float z = item.Transform.position.z;
			if (z < rampZ - 18f || z > rampZ + 58f)
			{
				continue;
			}
			int currentLane = ClosestLane(item.Transform.position.x);
			int targetLane = ClosestLane(item.TargetX);
			if (currentLane != ramp.Lane)
			{
				if (targetLane == ramp.Lane || item.PendingLane == ramp.Lane)
				{
					item.PendingLane = -1;
					item.Lane = currentLane;
					item.TargetX = lanes[currentLane];
					item.WantsLaneChange = false;
					item.RampEscapeLane = -1;
					item.TurnSignalDirection = 0;
					item.LaneChangeCooldown = Mathf.Max(item.LaneChangeCooldown, 2.5f);
					pickups[i] = item;
				}
				continue;
			}
			if (item.PendingLane >= 0 && item.PendingLane != ramp.Lane)
			{
				continue;
			}
			if (targetLane != ramp.Lane && Mathf.Abs(item.TargetX - item.Transform.position.x) > 0.12f)
			{
				continue;
			}
			int escapeLane = PreferredRampEscapeLane(item, currentLane, ramp.Lane);
			if (escapeLane < 0)
			{
				continue;
			}
			bool urgent = z - rampZ <= 32f;
			if (IsTrafficLaneSafe(escapeLane, z, item.Transform, item) || urgent)
			{
				QueueTrafficLaneChange(ref item, escapeLane, item.Transform.position.x);
				item.RampEscapeLane = escapeLane;
				item.TurnSignalLeadTimer = urgent ? 0.05f : 0.42f;
				item.LaneChangeCooldown = Mathf.Max(item.LaneChangeCooldown, 3.8f);
				item.ChangeTimer = Mathf.Max(item.ChangeTimer, 3.8f);
			}
			else
			{
				SetTrafficTurnSignalToward(ref item, lanes[escapeLane], item.Transform.position.x);
				item.WantsLaneChange = true;
				item.RampEscapeLane = escapeLane;
			}
			pickups[i] = item;
		}
	}

	private int PreferredRampEscapeLane(Pickup traffic, int currentLane, int rampLane)
	{
		int bestLane = -1;
		float bestScore = float.NegativeInfinity;
		for (int lane = Mathf.Max(0, currentLane - 1); lane <= Mathf.Min(lanes.Length - 1, currentLane + 1); lane++)
		{
			if (lane == currentLane || lane == rampLane)
			{
				continue;
			}
			float nearest = 120f;
			for (int i = 0; i < pickups.Count; i++)
			{
				Pickup other = pickups[i];
				if (other.Transform == null || other.Transform == traffic.Transform || !IsTrafficOccupyingLane(other, lane))
				{
					continue;
				}
				nearest = Mathf.Min(nearest, Mathf.Abs(other.Transform.position.z - traffic.Transform.position.z));
			}
			bool safe = IsTrafficLaneSafe(lane, traffic.Transform.position.z, traffic.Transform, traffic);
			float score = nearest + (safe ? 1000f : 0f);
			if (score > bestScore)
			{
				bestScore = score;
				bestLane = lane;
			}
		}
		return bestLane;
	}

	private bool TrafficSpawnBlockedByRamp(int lane, float z)
	{
		for (int i = 0; i < rampPool.Count; i++)
		{
			RampObstacle ramp = rampPool[i];
			if (ramp == null || !ramp.Active || ramp.Broken || ramp.Triggered || ramp.Root == null || ramp.Lane != lane)
			{
				continue;
			}
			float gap = z - ramp.Root.transform.position.z;
			if (gap >= -18f && gap <= 58f)
			{
				return true;
			}
		}
		return false;
	}

	private bool TrafficNeedsRampEscape(int lane, float z)
	{
		for (int i = 0; i < rampPool.Count; i++)
		{
			RampObstacle ramp = rampPool[i];
			if (ramp == null || !ramp.Active || ramp.Broken || ramp.Triggered || ramp.Root == null || ramp.Lane != lane)
			{
				continue;
			}
			float gap = z - ramp.Root.transform.position.z;
			if (gap >= -18f && gap <= 58f)
			{
				return true;
			}
		}
		return false;
	}

	private int FindRampSafeTrafficLane(int preferredLane, float z)
	{
		preferredLane = Mathf.Clamp(preferredLane, 0, lanes.Length - 1);
		if (!TrafficSpawnBlockedByRamp(preferredLane, z))
		{
			return preferredLane;
		}
		for (int offset = 1; offset < lanes.Length; offset++)
		{
			int lane = (preferredLane + offset) % lanes.Length;
			if (!TrafficSpawnBlockedByRamp(lane, z))
			{
				return lane;
			}
		}
		return preferredLane;
	}
}
