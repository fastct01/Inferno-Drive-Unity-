using UnityEngine;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_ResolveTrafficContact, _TrafficMovedIntoPlayerSide,
// _FindSweptVehicleHit, _VehicleHitboxesOverlapPrepared, _VehicleHitboxesOverlapOnAxis, _SpawnTrafficCollisionExplosion,
// _TryShowTrafficWow, _SpawnTrafficLandingPow, _GetTrafficCollisionExplosionPrefab, _CalculateRearTrafficImpactPosition,
// _CalculateSideTrafficImpactPosition, _AutoDestroyParticleEffect, _IsTrafficLandingCrush, _TriggerTrafficLandingWreck,
// _UpdateTrafficWrecks.
public sealed partial class ShadowRunPrototype
{
	private bool ResolveTrafficContact(ref Pickup item, float dx, float dz, float previousDx, float previousDz, float dt)
	{
		if (playerAirborne)
		{
			return false;
		}
		float playerHalfWidth = 0.8924f;
		float trafficHalfWidth = 0.9118f;
		float playerHalfLength = 1.7654f;
		float trafficHalfLength = 1.7654f;
		float trafficYaw = item.Transform != null ? Mathf.DeltaAngle(0f, item.Transform.eulerAngles.y) : 0f;
		if (!FindSweptVehicleHit(new Vector2(previousDx, previousDz), new Vector2(dx, dz), yawVisual, trafficYaw, playerHalfWidth, playerHalfLength, trafficHalfWidth, trafficHalfLength, out Vector2 contactOffset))
		{
			return false;
		}
		dx = contactOffset.x;
		dz = contactOffset.y;
		float combinedHalfWidth = playerHalfWidth + trafficHalfWidth;
		float combinedHalfLength = playerHalfLength + trafficHalfLength;
		float absDx = Mathf.Abs(dx);
		float absDz = Mathf.Abs(dz);
		absDx = Mathf.Min(absDx, combinedHalfWidth - 0.001f);
		absDz = Mathf.Min(absDz, combinedHalfLength - 0.001f);
		float overlapX = combinedHalfWidth - absDx;
		float overlapZ = combinedHalfLength - absDz;
		float pushDirection = dx >= 0f ? -1f : 1f;
		Vector3 trafficPosition = item.Transform.position;
		Vector3 impactPosition = trafficPosition;
		bool sideImpact = overlapX < overlapZ || absDx > 0.75f;
		float severity = Mathf.Clamp01((speed - item.Speed + 18f) / 54f);
		float penetration = Mathf.Clamp01(overlapX / combinedHalfWidth);
		float yawDirection = -pushDirection;
		bool trafficMovedIntoPlayer = sideImpact && TrafficMovedIntoPlayerSide(item, dx);
		if (sideImpact)
		{
			float pushShare = Mathf.Lerp(0.42f, 0.72f, severity);
			playerX = Mathf.Clamp(playerX + pushDirection * (overlapX + 0.04f) * pushShare, PlayerMinX, PlayerMaxX);
			velocityX = pushDirection * Mathf.Max(2.8f + severity * 3.2f, Mathf.Abs(velocityX) * Mathf.Lerp(0.28f, 0.48f, severity));
			float speedBefore = speed;
			speed = InfernoDrivingRules.Impact(speedBefore, Mathf.Max(25f, speedBefore * Mathf.Lerp(0.97f, 0.89f, severity))); /* [InfernoLobby patch] */
			drift = Mathf.Max(drift, Mathf.Lerp(0.12f, 0.34f, severity));
			tractionLoss = Mathf.Max(tractionLoss, Mathf.Lerp(0.14f, 0.38f, penetration));
			impactYaw = Mathf.Clamp(impactYaw + yawDirection * Mathf.Lerp(4f, 9f, severity), -12f, 12f);
			impactRoll = Mathf.Clamp(impactRoll + pushDirection * Mathf.Lerp(0.9f, 2.6f, penetration), -3.2f, 3.2f);
			impactShake = Mathf.Max(impactShake, Mathf.Lerp(0.22f, 0.52f, severity));
			trafficPosition.x = Mathf.Clamp(trafficPosition.x - pushDirection * overlapX * 0.42f, PlayerMinX, PlayerMaxX);
			item.LateralVelocity = Mathf.Clamp(item.LateralVelocity - pushDirection * Mathf.Lerp(1.8f, 4.6f, severity), -7f, 7f);
			item.Yaw += pushDirection * Mathf.Lerp(5f, 14f, severity);
			item.Spin += pushDirection * Mathf.Lerp(7f, 18f, penetration);
			item.ImpactTimer = 0.55f;
			item.TargetX = Mathf.Clamp(trafficPosition.x, PlayerMinX, PlayerMaxX);
			item.Lane = ClosestLane(item.TargetX);
			item.Transform.position = trafficPosition;
			if (item.CollisionCooldown <= 0f)
			{
				SpawnTrafficCollisionExplosion(true, impactPosition, dx);
				TriggerVehicleEngineImpact(true);
				PlayCollisionHaptic(true);
				item.CollisionCooldown = 0.18f;
			}
			if (invuln <= 0f && !trafficMovedIntoPlayer)
			{
				lifeHalfUnits = Mathf.Max(0, lifeHalfUnits - 1);
				invuln = 1.5f;
				combo = 0;
			}
		}
		else
		{
			trafficPosition.z = player.position.z - combinedHalfLength - 0.08f;
			item.Transform.position = trafficPosition;
			item.Speed = Mathf.Max(item.Speed, speed * Mathf.Lerp(0.96f, 1.12f, severity));
			item.TargetSpeed = Mathf.Max(item.TargetSpeed, speed * Mathf.Lerp(0.92f, 1.02f, severity));
			float shoveDirection = dx == 0f ? Random.Range(-1f, 1f) : Mathf.Sign(dx);
			item.LateralVelocity = Mathf.Clamp(item.LateralVelocity + shoveDirection * Mathf.Lerp(1.8f, 5.2f, severity), -7f, 7f);
			item.Yaw += Mathf.Sign(dx == 0f ? Random.Range(-1f, 1f) : dx) * Mathf.Lerp(7f, 18f, severity);
			item.Spin += Mathf.Sign(dx == 0f ? Random.Range(-1f, 1f) : dx) * Mathf.Lerp(10f, 27f, severity);
			item.ImpactTimer = 0.7f;
			float speedBefore = speed;
			speed = InfernoDrivingRules.Impact(speedBefore, Mathf.Max(18f, Mathf.Min(speedBefore * Mathf.Lerp(0.74f, 0.56f, severity), item.Speed + 4f))); /* [InfernoLobby patch] */
			velocityX += pushDirection * Mathf.Lerp(1.8f, 4.6f, severity);
			drift = Mathf.Max(drift, Mathf.Lerp(0.16f, 0.38f, severity));
			tractionLoss = Mathf.Max(tractionLoss, Mathf.Lerp(0.22f, 0.48f, severity));
			float yawKick = dx == 0f ? Random.Range(-1f, 1f) : -Mathf.Sign(dx);
			impactYaw = Mathf.Clamp(impactYaw + yawKick * Mathf.Lerp(5f, 12f, severity), -14f, 14f);
			impactRoll = Mathf.Clamp(impactRoll + pushDirection * Mathf.Lerp(1.4f, 3.4f, severity), -4f, 4f);
			impactShake = Mathf.Max(impactShake, Mathf.Lerp(0.48f, 0.82f, severity));
			if (item.CollisionCooldown <= 0f)
			{
				SpawnTrafficCollisionExplosion(false, impactPosition, dx);
				TriggerVehicleEngineImpact(false);
				PlayCollisionHaptic(false);
				item.CollisionCooldown = 0.35f;
				if (invuln <= 0f)
				{
					lifeHalfUnits = Mathf.Max(0, lifeHalfUnits - 2);
					invuln = 0.85f;
				}
				combo = 0;
			}
		}
		player.position = new Vector3(playerX, 0f, 5f);
		item.NearMiss = true;
		return true;
	}

	private bool TrafficMovedIntoPlayerSide(Pickup item, float dx)
	{
		float side = Mathf.Sign(dx);
		if (Mathf.Abs(side) < 0.01f)
		{
			return false;
		}
		float playerTowardTraffic = Mathf.Max(0f, velocityX * side);
		float trafficLateral = item.LaneVelocity + item.LateralVelocity;
		float trafficTowardPlayer = Mathf.Max(0f, -trafficLateral * side);
		if (trafficTowardPlayer > 0.75f)
		{
			return trafficTowardPlayer > playerTowardTraffic + 0.35f;
		}
		return false;
	}

	private bool FindSweptVehicleHit(Vector2 previousOffset, Vector2 currentOffset, float playerYaw, float trafficYaw, float playerHalfWidth, float playerHalfLength, float trafficHalfWidth, float trafficHalfLength, out Vector2 contactOffset)
	{
		contactOffset = currentOffset;
		float broadPhase = (playerHalfLength + trafficHalfLength) * 1.415f + 0.12f;
		if (Mathf.Min(previousOffset.x, currentOffset.x) > broadPhase || Mathf.Max(previousOffset.x, currentOffset.x) < -broadPhase
			|| Mathf.Min(previousOffset.y, currentOffset.y) > broadPhase || Mathf.Max(previousOffset.y, currentOffset.y) < -broadPhase)
		{
			return false;
		}
		float playerRadians = playerYaw * Mathf.Deg2Rad;
		float trafficRadians = trafficYaw * Mathf.Deg2Rad;
		Vector2 playerRight = new Vector2(Mathf.Cos(playerRadians), -Mathf.Sin(playerRadians));
		Vector2 playerForward = new Vector2(Mathf.Sin(playerRadians), Mathf.Cos(playerRadians));
		Vector2 trafficRight = new Vector2(Mathf.Cos(trafficRadians), -Mathf.Sin(trafficRadians));
		Vector2 trafficForward = new Vector2(Mathf.Sin(trafficRadians), Mathf.Cos(trafficRadians));
		if (VehicleHitboxesOverlapPrepared(currentOffset, playerRight, playerForward, trafficRight, trafficForward, playerHalfWidth, playerHalfLength, trafficHalfWidth, trafficHalfLength))
		{
			return true;
		}
		int steps = Mathf.Clamp(Mathf.CeilToInt(Vector2.Distance(previousOffset, currentOffset) / 0.45f), 1, 6);
		for (int i = 1; i < steps; i++)
		{
			Vector2 sample = Vector2.Lerp(previousOffset, currentOffset, (float)i / steps);
			if (VehicleHitboxesOverlapPrepared(sample, playerRight, playerForward, trafficRight, trafficForward, playerHalfWidth, playerHalfLength, trafficHalfWidth, trafficHalfLength))
			{
				contactOffset = sample;
				return true;
			}
		}
		return false;
	}

	private bool VehicleHitboxesOverlapPrepared(Vector2 trafficOffset, Vector2 playerRight, Vector2 playerForward, Vector2 trafficRight, Vector2 trafficForward, float playerHalfWidth, float playerHalfLength, float trafficHalfWidth, float trafficHalfLength)
	{
		if (VehicleHitboxesOverlapOnAxis(trafficOffset, playerRight, playerRight, playerForward, trafficRight, trafficForward, playerHalfWidth, playerHalfLength, trafficHalfWidth, trafficHalfLength)
			&& VehicleHitboxesOverlapOnAxis(trafficOffset, playerForward, playerRight, playerForward, trafficRight, trafficForward, playerHalfWidth, playerHalfLength, trafficHalfWidth, trafficHalfLength)
			&& VehicleHitboxesOverlapOnAxis(trafficOffset, trafficRight, playerRight, playerForward, trafficRight, trafficForward, playerHalfWidth, playerHalfLength, trafficHalfWidth, trafficHalfLength))
		{
			return VehicleHitboxesOverlapOnAxis(trafficOffset, trafficForward, playerRight, playerForward, trafficRight, trafficForward, playerHalfWidth, playerHalfLength, trafficHalfWidth, trafficHalfLength);
		}
		return false;
	}

	private bool VehicleHitboxesOverlapOnAxis(Vector2 trafficOffset, Vector2 axis, Vector2 playerRight, Vector2 playerForward, Vector2 trafficRight, Vector2 trafficForward, float playerHalfWidth, float playerHalfLength, float trafficHalfWidth, float trafficHalfLength)
	{
		float distance = Mathf.Abs(Vector2.Dot(trafficOffset, axis));
		float playerExtent = Mathf.Abs(Vector2.Dot(playerRight, axis)) * playerHalfWidth + Mathf.Abs(Vector2.Dot(playerForward, axis)) * playerHalfLength;
		float trafficExtent = Mathf.Abs(Vector2.Dot(trafficRight, axis)) * trafficHalfWidth + Mathf.Abs(Vector2.Dot(trafficForward, axis)) * trafficHalfLength;
		return distance < playerExtent + trafficExtent;
	}

	private void SpawnTrafficCollisionExplosion(bool sideImpact, Vector3 trafficPosition, float dx)
	{
		GameObject prefab = GetTrafficCollisionExplosionPrefab(sideImpact);
		if (prefab == null)
		{
			return;
		}
		Vector3 position = sideImpact ? CalculateSideTrafficImpactPosition(trafficPosition, dx) : CalculateRearTrafficImpactPosition(trafficPosition);
		float yaw = sideImpact ? (dx >= 0f ? -90f : 90f) : 0f;
		GameObject instance = Instantiate(prefab, position, Quaternion.Euler(0f, yaw, 0f));
		instance.name = sideImpact ? "Traffic Side Fire Explosion FX" : "Traffic Rear Explosion FX";
		AutoDestroyParticleEffect(instance, 4.5f);
	}

	private void TryShowTrafficWow(Pickup passedCar, float passedDx)
	{
		if (passedCar.Transform == null)
		{
			return;
		}
		float now = Time.unscaledTime;
		float clearance = Mathf.Abs(passedDx) - 1.86f;
		bool tightPass = speed >= 30f && clearance <= 0.82f;
		if (!tightPass || now < nextTrafficWowTime)
		{
			return;
		}
		if (trafficWowPrefab == null)
		{
			trafficWowPrefab = Resources.Load<GameObject>(TrafficWowResource);
		}
		if (trafficWowPrefab == null)
		{
			return;
		}
		nextTrafficWowTime = now + Random.Range(30f, 45f);
		float side = passedDx < 0f ? -1f : 1f;
		Vector3 position = new Vector3(Mathf.Clamp(playerX + side * 1.15f, PlayerMinX, PlayerMaxX), 2.75f, player.position.z - 2.8f);
		GameObject instance = Instantiate(trafficWowPrefab, position, Quaternion.identity);
		instance.name = "Traffic Gap WOW FX";
		instance.transform.localScale = Vector3.one * 0.72f;
		AutoDestroyParticleEffect(instance, 3.2f);
	}

	private void SpawnTrafficLandingPow(Vector3 impactPosition)
	{
		if (trafficPowPrefab == null)
		{
			trafficPowPrefab = Resources.Load<GameObject>(TrafficPowResource);
		}
		if (trafficPowPrefab == null)
		{
			return;
		}
		Vector3 position = impactPosition + new Vector3(0f, 2.75f, 0f);
		GameObject instance = Instantiate(trafficPowPrefab, position, Quaternion.identity);
		instance.name = "Traffic Landing POW FX";
		instance.transform.localScale = Vector3.one * 0.8f;
		AutoDestroyParticleEffect(instance, 3.2f);
	}

	private GameObject GetTrafficCollisionExplosionPrefab(bool sideImpact)
	{
		if (sideImpact)
		{
			if (sideCollisionExplosionPrefab == null)
			{
				sideCollisionExplosionPrefab = Resources.Load<GameObject>(SideCollisionExplosionResource);
				if (sideCollisionExplosionPrefab == null && !sideCollisionExplosionMissingLogged)
				{
					sideCollisionExplosionMissingLogged = true;
					Debug.LogWarning("Traffic side collision explosion not found in Resources: JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Explosions/CFXR3 Fire Explosion B");
				}
			}
			return sideCollisionExplosionPrefab;
		}
		if (rearCollisionExplosionPrefab == null)
		{
			rearCollisionExplosionPrefab = Resources.Load<GameObject>(RearCollisionExplosionResource);
			if (rearCollisionExplosionPrefab == null && !rearCollisionExplosionMissingLogged)
			{
				rearCollisionExplosionMissingLogged = true;
				Debug.LogWarning("Traffic rear collision explosion not found in Resources: JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Explosions/CFXR Explosion 1");
			}
		}
		return rearCollisionExplosionPrefab;
	}

	private Vector3 CalculateRearTrafficImpactPosition(Vector3 trafficPosition)
	{
		float playerFrontZ = player != null ? player.position.z - ContactHalfLength : 3.78f;
		float trafficRearZ = trafficPosition.z + ContactHalfLength;
		return new Vector3(Mathf.Lerp(playerX, trafficPosition.x, 0.5f), 0.72f, Mathf.Lerp(playerFrontZ, trafficRearZ, 0.5f));
	}

	private Vector3 CalculateSideTrafficImpactPosition(Vector3 trafficPosition, float dx)
	{
		float side = dx >= 0f ? 1f : -1f;
		float playerEdgeX = playerX + side * 0.92f;
		float trafficEdgeX = trafficPosition.x - side * 0.94f;
		float playerZ = player != null ? player.position.z : 5f;
		return new Vector3(Mathf.Lerp(playerEdgeX, trafficEdgeX, 0.5f), 0.72f, Mathf.Clamp(trafficPosition.z, playerZ - 1.159f, playerZ + 1.159f));
	}

	private void AutoDestroyParticleEffect(GameObject instance, float fallbackLifetime)
	{
		if (instance == null)
		{
			return;
		}
		float lifetime = fallbackLifetime;
		ParticleSystem[] systems = instance.GetComponentsInChildren<ParticleSystem>(true);
		for (int i = 0; i < systems.Length; i++)
		{
			ParticleSystem system = systems[i];
			if (system == null)
			{
				continue;
			}
			ParticleSystem.MainModule main = system.main;
			lifetime = Mathf.Max(lifetime, main.duration + main.startLifetime.constantMax + 0.35f);
			if (!system.isPlaying)
			{
				system.Play(true);
			}
		}
		Destroy(instance, Mathf.Clamp(lifetime, 1.5f, 8f));
	}

	private bool IsTrafficLandingCrush(float dx, float dz)
	{
		if (!playerLandedThisFrame || playerLandingDownSpeed < 3f)
		{
			return false;
		}
		if (Mathf.Abs(dx) < 1.62f)
		{
			return Mathf.Abs(dz) < 2.44f;
		}
		return false;
	}

	private void TriggerTrafficLandingWreck(Pickup item, float dx)
	{
		if (item.Transform == null)
		{
			return;
		}
		TrafficWreck wreck = null;
		for (int i = 0; i < trafficWreckPool.Count; i++)
		{
			if (trafficWreckPool[i] != null && !trafficWreckPool[i].Active)
			{
				wreck = trafficWreckPool[i];
				break;
			}
		}
		if (wreck == null && trafficWreckPool.Count > 0)
		{
			wreck = trafficWreckPool[0];
			for (int i = 1; i < trafficWreckPool.Count; i++)
			{
				if (trafficWreckPool[i].Age > wreck.Age)
				{
					wreck = trafficWreckPool[i];
				}
			}
			ReleaseTrafficWreck(wreck);
		}
		Vector3 position = item.Transform.position;
		if (wreck != null && wreck.Root != null)
		{
			wreck.Active = true;
			wreck.Age = 0f;
			wreck.Root.transform.position = position;
			wreck.Root.transform.rotation = item.Transform.rotation;
			wreck.Root.SetActive(true);
			Color paint = Color.white;
			TrafficVehicleAppearance appearance = item.Transform.GetComponent<TrafficVehicleAppearance>();
			if (appearance != null)
			{
				paint = appearance.CurrentPaint;
			}
			if (wreck.PaintBlock == null)
			{
				wreck.PaintBlock = new MaterialPropertyBlock();
			}
			wreck.PaintBlock.Clear();
			wreck.PaintBlock.SetColor("_BaseColor", paint);
			wreck.PaintBlock.SetColor("_Color", paint);
			float scatterSide = Mathf.Abs(dx) > 0.08f ? Mathf.Sign(dx) : (Random.value < 0.5f ? -1f : 1f);
			for (int i = 0; i < wreck.Pieces.Count; i++)
			{
				TrafficWreckPiece piece = wreck.Pieces[i];
				if (piece.Transform == null)
				{
					continue;
				}
				piece.Transform.localPosition = piece.HomePosition;
				piece.Transform.localRotation = piece.HomeRotation;
				float side = Mathf.Abs(piece.HomePosition.x) > 0.05f ? Mathf.Sign(piece.HomePosition.x) : scatterSide * (i % 2 != 0 ? -1f : 1f);
				float wheelBoost = piece.Paintable ? 1f : 1.35f;
				piece.Velocity = new Vector3(side * Random.Range(2.2f, 4.6f) * wheelBoost + velocityX * 0.12f, Random.Range(3.2f, 5.8f) + playerLandingDownSpeed * 0.16f, Random.Range(-2.8f, 3.8f));
				piece.AngularVelocity = new Vector3(Random.Range(-260f, 260f), Random.Range(-220f, 220f), Random.Range(-300f, 300f));
				if (piece.Paintable && piece.Renderer != null)
				{
					piece.Renderer.SetPropertyBlock(wreck.PaintBlock);
				}
			}
		}
		TriggerVehicleEngineImpact(false);
		PlayCollisionHaptic(false);
		impactShake = Mathf.Max(impactShake, 0.9f);
		SpawnTrafficLandingPow(position);
		float speedBefore = speed;
		speed = InfernoDrivingRules.Impact(speedBefore, Mathf.Max(24f, speedBefore * 0.94f)); /* [InfernoLobby patch] */
		combo++;
		comboTimer = 1.6f;
		score += 120f;
	}

	private void UpdateTrafficWrecks(float dt)
	{
		for (int i = 0; i < trafficWreckPool.Count; i++)
		{
			TrafficWreck wreck = trafficWreckPool[i];
			if (wreck == null || !wreck.Active || wreck.Root == null)
			{
				continue;
			}
			wreck.Age += dt;
			Transform root = wreck.Root.transform;
			root.position = root.position + Vector3.forward * speed * dt;
			for (int p = 0; p < wreck.Pieces.Count; p++)
			{
				TrafficWreckPiece piece = wreck.Pieces[p];
				if (piece.Transform == null)
				{
					continue;
				}
				Vector3 localPosition = piece.Transform.localPosition + piece.Velocity * dt;
				piece.Velocity.y -= 14.5f * dt;
				if (localPosition.y < 0.1f && piece.Velocity.y < 0f)
				{
					localPosition.y = 0.1f;
					piece.Velocity.y *= -0.28f;
					piece.Velocity.x *= 0.82f;
					piece.Velocity.z *= 0.82f;
					piece.AngularVelocity = piece.AngularVelocity * 0.78f;
				}
				piece.Transform.localPosition = localPosition;
				piece.Transform.localRotation = piece.Transform.localRotation * Quaternion.Euler(piece.AngularVelocity * dt);
			}
			if (wreck.Age >= 2.25f || root.position.z > player.position.z + 14f)
			{
				ReleaseTrafficWreck(wreck);
			}
		}
	}
}
