using UnityEngine;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_PrewarmTrafficPool, _LoadTrafficVehiclePrefab,
// _GetTrafficCarFromPool, _ReleaseTrafficCar, _PrepareTrafficVehicleModel, _PrewarmTrafficWreckPool,
// _AddTrafficWreckBodyPiece, _AddTrafficWreckWheel, _ReleaseTrafficWreck, _SetLayerRecursively, _UpdateSpawns,
// _SpawnTrafficCar, _SpawnTrafficPattern, _PickLateRunTrafficPattern, _SpawnTrafficPatternCar, _SeedInitialTraffic,
// _SpawnTrafficCarAt, _ApplyTrafficDriverAppearance, _ChooseTrafficDriverStyle, _GetTrafficDriverProfile,
// _ChooseTrafficLane, _PickTrafficSpawnZ.
public sealed partial class ShadowRunPrototype
{
	private void PrewarmTrafficPool()
	{
		bool poolIntact = trafficPool.Count == TrafficPoolSize;
		if (poolIntact)
		{
			for (int i = 0; i < trafficPool.Count; i++)
			{
				if (trafficPool[i] == null)
				{
					poolIntact = false;
					break;
				}
			}
		}
		if (poolIntact)
		{
			for (int i = 0; i < trafficPool.Count; i++)
			{
				ReleaseTrafficCar(trafficPool[i]);
			}
			PrewarmTrafficWreckPool();
			return;
		}
		for (int i = 0; i < trafficPool.Count; i++)
		{
			if (trafficPool[i] != null)
			{
				Destroy(trafficPool[i]);
			}
		}
		trafficPool.Clear();
		GameObject prefab = LoadTrafficVehiclePrefab();
		if (prefab == null)
		{
			Debug.LogError("Traffic vehicle model not found in Resources: uploads_files_4881612_uploads_files_3750868_MB.W222.AMG");
			return;
		}
		for (int i = 0; i < TrafficPoolSize; i++)
		{
			GameObject car = new GameObject("Traffic Car Pooled");
			car.layer = HighBeamTrafficLayer;
			BoxCollider collider = car.AddComponent<BoxCollider>();
			collider.center = new Vector3(0f, 0.62f, 0f);
			collider.size = new Vector3(1.6544f, 1.18f, 3.3488f);
			GameObject model = Instantiate(prefab);
			model.name = "Traffic Car Model";
			model.transform.SetParent(car.transform, false);
			model.transform.localRotation = VehicleModelBaseRotation();
			model.transform.localScale = Vector3.one;
			DisableImportedModelCamerasAndLights(model);
			PrepareTrafficVehicleModel(model);
			FitImportedVehicleModel(model.transform, car.transform, 1.88f, 3.64f, 1.08f);
			car.AddComponent<TrafficWheelAnimator>().Bind(model.transform);
			car.AddComponent<TrafficVehicleAppearance>().Bind(model.transform);
			car.AddComponent<TrafficVehicleLights>().Bind(model.transform);
			SetLayerRecursively(car, HighBeamTrafficLayer);
			car.SetActive(false);
			trafficPool.Add(car);
		}
		PrewarmTrafficWreckPool();
	}

	private GameObject LoadTrafficVehiclePrefab()
	{
		return Resources.Load<GameObject>(TrafficVehicleResource);
	}

	private GameObject GetTrafficCarFromPool()
	{
		foreach (GameObject car in trafficPool)
		{
			if (car != null && !car.activeSelf)
			{
				car.SetActive(true);
				return car;
			}
		}
		return null;
	}

	private void ReleaseTrafficCar(GameObject car)
	{
		if (car == null)
		{
			return;
		}
		TrafficVehicleLights lights = car.GetComponent<TrafficVehicleLights>();
		if (lights != null)
		{
			lights.ResetLights();
		}
		TrafficWheelAnimator wheels = car.GetComponent<TrafficWheelAnimator>();
		if (wheels != null)
		{
			wheels.ResetSpin();
		}
		TrafficVehicleAppearance appearance = car.GetComponent<TrafficVehicleAppearance>();
		if (appearance != null)
		{
			appearance.ResetPaint();
		}
		car.SetActive(false);
		car.transform.position = new Vector3(0f, -100f, 0f);
	}

	private void PrepareTrafficVehicleModel(GameObject model)
	{
		Transform[] nodes = model.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < nodes.Length; i++)
		{
			Transform node = nodes[i];
			if (node != null && node.name == "Mball")
			{
				node.gameObject.SetActive(false);
				Destroy(node.gameObject);
			}
		}
		PrepareImportedVehicleModel(model);
	}

	private void PrewarmTrafficWreckPool()
	{
		bool poolIntact = trafficWreckPool.Count == 2;
		for (int i = 0; poolIntact && i < trafficWreckPool.Count; i++)
		{
			poolIntact = trafficWreckPool[i] != null && trafficWreckPool[i].Root != null;
		}
		if (poolIntact)
		{
			for (int i = 0; i < trafficWreckPool.Count; i++)
			{
				ReleaseTrafficWreck(trafficWreckPool[i]);
			}
			return;
		}
		for (int i = 0; i < trafficWreckPool.Count; i++)
		{
			if (trafficWreckPool[i] != null && trafficWreckPool[i].Root != null)
			{
				Destroy(trafficWreckPool[i].Root);
			}
		}
		trafficWreckPool.Clear();
		for (int i = 0; i < 2; i++)
		{
			TrafficWreck wreck = new TrafficWreck();
			wreck.Root = new GameObject("Traffic Landing Wreck Pooled");
			wreck.PaintBlock = new MaterialPropertyBlock();
			AddTrafficWreckBodyPiece(wreck, "Front Body", new Vector3(0f, 0.48f, -1.16f), new Vector3(1.72f, 0.42f, 1.05f));
			AddTrafficWreckBodyPiece(wreck, "Cabin", new Vector3(0f, 0.78f, 0f), new Vector3(1.48f, 0.62f, 1.25f));
			AddTrafficWreckBodyPiece(wreck, "Rear Body", new Vector3(0f, 0.48f, 1.16f), new Vector3(1.72f, 0.42f, 1.05f));
			AddTrafficWreckWheel(wreck, "Front Left Wheel", new Vector3(-0.91f, 0.34f, -1.08f));
			AddTrafficWreckWheel(wreck, "Front Right Wheel", new Vector3(0.91f, 0.34f, -1.08f));
			AddTrafficWreckWheel(wreck, "Rear Left Wheel", new Vector3(-0.91f, 0.34f, 1.08f));
			AddTrafficWreckWheel(wreck, "Rear Right Wheel", new Vector3(0.91f, 0.34f, 1.08f));
			ReleaseTrafficWreck(wreck);
			trafficWreckPool.Add(wreck);
		}
	}

	private void AddTrafficWreckBodyPiece(TrafficWreck wreck, string name, Vector3 localPosition, Vector3 scale)
	{
		GameObject piece = AddCube(wreck.Root.transform, name, localPosition, scale, carMat);
		TrafficWreckPiece data = new TrafficWreckPiece();
		data.Transform = piece.transform;
		data.Renderer = piece.GetComponent<Renderer>();
		data.HomePosition = localPosition;
		data.HomeRotation = Quaternion.identity;
		data.Paintable = true;
		wreck.Pieces.Add(data);
	}

	private void AddTrafficWreckWheel(TrafficWreck wreck, string name, Vector3 localPosition)
	{
		GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		wheel.name = name;
		wheel.transform.SetParent(wreck.Root.transform, false);
		wheel.transform.localPosition = localPosition;
		wheel.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
		wheel.transform.localScale = new Vector3(0.38f, 0.16f, 0.38f);
		PreparePrimitive(wheel, tireMat);
		TrafficWreckPiece data = new TrafficWreckPiece();
		data.Transform = wheel.transform;
		data.Renderer = wheel.GetComponent<Renderer>();
		data.HomePosition = localPosition;
		data.HomeRotation = wheel.transform.localRotation;
		data.Paintable = false;
		wreck.Pieces.Add(data);
	}

	private void ReleaseTrafficWreck(TrafficWreck wreck)
	{
		if (wreck == null || wreck.Root == null)
		{
			return;
		}
		wreck.Active = false;
		wreck.Age = 0f;
		for (int i = 0; i < wreck.Pieces.Count; i++)
		{
			TrafficWreckPiece piece = wreck.Pieces[i];
			if (piece.Transform == null)
			{
				continue;
			}
			piece.Transform.localPosition = piece.HomePosition;
			piece.Transform.localRotation = piece.HomeRotation;
			piece.Velocity = Vector3.zero;
			piece.AngularVelocity = Vector3.zero;
			if (piece.Paintable && piece.Renderer != null)
			{
				piece.Renderer.SetPropertyBlock(null);
			}
		}
		wreck.Root.SetActive(false);
		wreck.Root.transform.position = new Vector3(0f, -100f, 0f);
		wreck.Root.transform.rotation = Quaternion.identity;
	}

	private void SetLayerRecursively(GameObject go, int layer)
	{
		if (go == null)
		{
			return;
		}
		go.layer = layer;
		foreach (Transform child in go.transform)
		{
			SetLayerRecursively(child.gameObject, layer);
		}
	}

	private void UpdateSpawns(float dt)
	{
		UpdateTrafficWrecks(dt);
		bool wreckSpawnedThisFrame = false;
		float previousPlayerX = playerX - velocityX * dt;
		if (trafficIncidentCooldown > 0f)
		{
			trafficIncidentCooldown -= dt;
		}
		playerStoppedTrafficTimer = speed < 0.8f ? Mathf.Min(4f, playerStoppedTrafficTimer + dt) : 0f;
		bool playerStopped = playerStoppedTrafficTimer >= 1.4f;
		spawnTimer -= dt;
		if (spawnTimer <= 0f && pickups.Count < TrafficPoolSize)
		{
			if (distance < 150f)
			{
				SpawnTrafficCar();
				spawnTimer = Random.Range(0.85f, 1.35f);
			}
			else
			{
				int spawned = SpawnTrafficPattern();
				spawnTimer = Random.Range(1.05f, 1.55f) + Mathf.Max(0, spawned - 1) * 0.18f;
			}
		}
		for (int i = pickups.Count - 1; i >= 0; i--)
		{
			Pickup item = pickups[i];
			Vector3 previousPosition = item.Transform.position;
			UpdateTrafficAi(ref item, dt);
			if (item.ContactCooldown > 0f)
			{
				item.ContactCooldown -= dt;
			}
			if (item.CollisionCooldown > 0f)
			{
				item.CollisionCooldown -= dt;
			}
			float dx = item.Transform.position.x - playerX;
			float dz = item.Transform.position.z - player.position.z;
			float previousDx = previousPosition.x - previousPlayerX;
			float previousDz = previousPosition.z - player.position.z;
			if (!wreckSpawnedThisFrame && IsTrafficLandingCrush(dx, dz))
			{
				TriggerTrafficLandingWreck(item, dx);
				wreckSpawnedThisFrame = true;
				ReleaseTrafficCar(item.Transform.gameObject);
				pickups.RemoveAt(i);
				continue;
			}
			bool grounded = !playerAirborne;
			bool contact = grounded && ResolveTrafficContact(ref item, dx, dz, previousDx, previousDz, dt);
			bool passedThisFrame = previousDz < 0f && dz >= 0f;
			if (grounded && !contact && passedThisFrame && Mathf.Abs(dx) < 2.85f && Mathf.Abs(dx) > 1.25f && !item.NearMiss)
			{
				item.NearMiss = true;
				combo++;
				comboTimer = 1.4f;
				Reward(6f + combo * 1.5f);
				score += 55f * combo;
				TryShowTrafficWow(item, dx);
			}
			if (grounded && !contact && Mathf.Abs(dz) < 2.05f && Mathf.Abs(dx) < item.Radius + 0.72f)
			{
				bool sideImpact = Mathf.Abs(dx) > 0.75f;
				bool trafficMovedIntoPlayer = sideImpact && TrafficMovedIntoPlayerSide(item, dx);
				if (item.CollisionCooldown <= 0f)
				{
					SpawnTrafficCollisionExplosion(sideImpact, item.Transform.position, dx);
					TriggerVehicleEngineImpact(sideImpact);
					PlayCollisionHaptic(sideImpact);
					item.CollisionCooldown = sideImpact ? 0.18f : 0.35f;
				}
				if (invuln <= 0f && !trafficMovedIntoPlayer)
				{
					lifeHalfUnits = Mathf.Max(0, lifeHalfUnits - (sideImpact ? 1 : 2));
					invuln = sideImpact ? 1.5f : 1.15f;
					float speedBefore = speed;
					speed = InfernoDrivingRules.Impact(speedBefore, Mathf.Max(22f, speedBefore * 0.74f)); /* [InfernoLobby patch] */
					velocityX = -Mathf.Sign(dx == 0f ? steerInput : dx) * 3.4f;
					combo = 0;
				}
				item.NearMiss = true;
				pickups[i] = item;
				continue;
			}
			float z = item.Transform.position.z;
			if (z < (playerStopped ? -225f : TrafficHorizonRespawnZ))
			{
				RespawnTrafficBehind(ref item, Mathf.Clamp01(distance / 2400f), playerStopped);
				pickups[i] = item;
				continue;
			}
			if (!playerStopped && z > player.position.z + 11f)
			{
				RegisterDailyCarPassed();
				ReleaseTrafficCar(item.Transform.gameObject);
				pickups.RemoveAt(i);
				continue;
			}
			pickups[i] = item;
		}
	}

	private void SpawnTrafficCar()
	{
		float z = PickTrafficSpawnZ(-150f, -82f);
		int lane = FindRampSafeTrafficLane(ChooseTrafficLane(), z);
		SpawnTrafficCarAt(lane, z, Mathf.Clamp01(distance / 2400f), null, -1f);
	}

	private int SpawnTrafficPattern()
	{
		int available = TrafficPoolSize - pickups.Count;
		if (available <= 0)
		{
			return 0;
		}
		if (lanes.Length < 3)
		{
			SpawnTrafficCar();
			return 1;
		}
		float roll = Random.value;
		int pattern;
		if (available >= 5 && distance >= 1100f && roll < 0.018f)
		{
			pattern = 15;
		}
		else if (available >= 4 && distance >= 900f && roll < 0.048f)
		{
			pattern = 14;
		}
		else if (available >= 4 && distance >= 450f && roll < 0.13f)
		{
			pattern = Random.Range(12, 14);
		}
		else
		{
			pattern = distance < 4000f ? Random.Range(0, 12) : PickLateRunTrafficPattern();
		}
		if (pattern == lastTrafficPattern)
		{
			if (pattern >= 12)
			{
				pattern = pattern == 12 ? 13 : 12;
			}
			else if (distance >= 4000f)
			{
				do
				{
					pattern = PickLateRunTrafficPattern();
				}
				while (pattern == lastTrafficPattern);
			}
			else
			{
				pattern = (pattern + Random.Range(1, 12)) % 12;
			}
		}
		lastTrafficPattern = pattern;
		float pressure = Mathf.Clamp01(distance / 2400f);
		float z = PickTrafficSpawnZ(-174f, -116f);
		float stagger = Random.Range(-2.2f, 2.2f);
		float convoySpeed = Mathf.Clamp(Mathf.Lerp(19.5f, 27f, pressure) + Random.Range(-1f, 1.5f), 18f, 29f);
		float gap = Mathf.Clamp(Mathf.Max(8f, speed - convoySpeed) * 1.65f, 28f, 40f);
		float tightGap = gap * 0.82f;
		float wideGap = gap * 1.18f;
		int spawned = 0;
		switch (pattern)
		{
			case 0:
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z + stagger, pressure, -1f);
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z - stagger, pressure, -1f);
				break;
			case 1:
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z + stagger, pressure, -1f);
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z - stagger, pressure, -1f);
				break;
			case 2:
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z + stagger, pressure, -1f);
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z - stagger, pressure, -1f);
				break;
			case 3:
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z - gap, pressure, convoySpeed);
				break;
			case 4:
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z - gap, pressure, convoySpeed);
				break;
			case 5:
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z - gap, pressure, convoySpeed);
				break;
			case 6:
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z - gap, pressure, convoySpeed);
				break;
			case 7:
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z - gap, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z - gap * 2f, pressure, convoySpeed);
				break;
			case 8:
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z - gap, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z - gap * 2f, pressure, convoySpeed);
				break;
			case 9:
			{
				int lane = Random.Range(0, 3);
				SpawnTrafficPatternCar(ref available, ref spawned, lane, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, lane, z - gap, pressure, convoySpeed);
				break;
			}
			case 10:
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z - tightGap, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z - tightGap * 2f, pressure, convoySpeed);
				break;
			case 11:
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z - tightGap, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z - tightGap * 2f, pressure, convoySpeed);
				break;
			case 12:
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z - tightGap, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z - tightGap * 2f, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z - tightGap * 3f, pressure, convoySpeed);
				break;
			case 13:
			{
				int side = Random.value < 0.5f ? 0 : 2;
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, side, z - tightGap, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z - tightGap * 2f, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 2 - side, z - tightGap * 3f, pressure, convoySpeed);
				break;
			}
			case 14:
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z + stagger * 0.35f, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z - wideGap, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z - wideGap - stagger * 0.35f, pressure, convoySpeed);
				break;
			case 15:
			{
				int side = Random.value < 0.5f ? 0 : 2;
				SpawnTrafficPatternCar(ref available, ref spawned, 0, z, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 2, z + stagger * 0.25f, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 1, z - wideGap, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, side, z - wideGap * 2f, pressure, convoySpeed);
				SpawnTrafficPatternCar(ref available, ref spawned, 2 - side, z - wideGap * 3f, pressure, convoySpeed);
				break;
			}
		}
		return spawned;
	}

	private int PickLateRunTrafficPattern()
	{
		int roll = Random.Range(0, 100);
		if (roll < 20)
		{
			return Random.Range(0, 3);
		}
		if (roll < 50)
		{
			return Random.Range(3, 7);
		}
		if (roll < 62)
		{
			return 9;
		}
		int pick = Random.Range(0, 4);
		return pick < 2 ? 7 + pick : 8 + pick;
	}

	private void SpawnTrafficPatternCar(ref int available, ref int spawned, int lane, float z, float pressure, float forcedSpeed = -1f)
	{
		if (available <= 0)
		{
			return;
		}
		if (TrafficSpawnBlockedByRamp(lane, z))
		{
			return;
		}
		SpawnTrafficCarAt(lane, z, pressure, null, forcedSpeed);
		available--;
		spawned++;
	}

	private void SeedInitialTraffic()
	{
		int count = Random.Range(4, 7);
		int previousLane = -1;
		float z = -Random.Range(32f, 48f);
		for (int i = 0; i < count && pickups.Count < TrafficPoolSize; i++)
		{
			int lane = Random.Range(0, lanes.Length);
			if (lane == previousLane && Random.value < 0.72f)
			{
				lane = (lane + Random.Range(1, lanes.Length)) % lanes.Length;
			}
			SpawnTrafficCarAt(lane, z, 0f, null, -1f);
			previousLane = lane;
			z -= Random.Range(27f, 53f);
		}
		spawnTimer = Random.Range(0.85f, 1.35f);
	}

	private void SpawnTrafficCarAt(int lane, float z, float pressure, TrafficDriverStyle? forcedStyle = null, float forcedSpeed = -1f)
	{
		lane = Mathf.Clamp(lane, 0, lanes.Length - 1);
		GameObject car = GetTrafficCarFromPool();
		if (car == null)
		{
			return;
		}
		car.transform.position = new Vector3(lanes[lane], 0f, z);
		car.transform.rotation = Quaternion.identity;
		TrafficDriverStyle style = forcedStyle.HasValue ? forcedStyle.GetValueOrDefault() : ChooseTrafficDriverStyle(pressure);
		GetTrafficDriverProfile(style, out float aggression, out float patience, out float reaction, out float speedBias, out float laneChangeBias, out float bravery, out float awareness, out float courtesy, out float riskTolerance, out float safeGap);
		float cruiseSpeed = forcedSpeed > 0f
			? forcedSpeed + Random.Range(-0.7f, 1.1f)
			: Random.Range(Mathf.Lerp(15f, 18f, pressure), Mathf.Lerp(25f, 33f, pressure)) * speedBias;
		cruiseSpeed = Mathf.Clamp(cruiseSpeed, 11f, Mathf.Lerp(34f, 42f, pressure));
		ApplyTrafficDriverAppearance(car, style);
		Pickup item = default(Pickup);
		item.Transform = car.transform;
		item.Radius = 1.05f;
		item.Type = PickupType.Traffic;
		item.Lane = lane;
		item.TargetX = lanes[lane];
		item.Speed = forcedSpeed > 0f
			? Mathf.Clamp(forcedSpeed, 10f, 42f)
			: Random.Range(Mathf.Lerp(13f, 16f, pressure), Mathf.Lerp(23f, 30f, pressure)) * speedBias;
		item.TargetSpeed = cruiseSpeed;
		item.CruiseSpeed = cruiseSpeed;
		item.SpeedBias = speedBias;
		item.LaneVelocity = 0f;
		item.Aggression = aggression;
		item.Patience = patience;
		item.Reaction = reaction;
		item.SafeGap = safeGap;
		item.LaneChangeBias = laneChangeBias;
		item.Bravery = bravery;
		item.Awareness = awareness;
		item.Courtesy = courtesy;
		item.RiskTolerance = riskTolerance;
		item.ChangeTimer = Random.Range(1.2f, 3.4f) / Mathf.Max(0.55f, laneChangeBias);
		item.LaneChangeCooldown = Random.Range(1.4f, 4.2f) / Mathf.Max(0.6f, laneChangeBias);
		item.IntentTimer = 0f;
		item.AiThinkTimer = Random.Range(0f, 0.18f);
		item.CruiseVariation = Random.Range(-1.4f, 1.4f);
		item.CruiseVariationTimer = Random.Range(3.5f, 8f);
		item.NaturalLaneOffset = 0f;
		item.NaturalLaneTarget = Random.Range(-0.18f, 0.18f);
		item.NaturalLaneTimer = Random.Range(2.8f, 6.5f);
		item.CutInCooldown = Random.Range(3f, 8f);
		item.PanicTimer = 0f;
		item.TailgateTimer = 0f;
		item.ContactCooldown = 0f;
		item.CollisionCooldown = 0f;
		item.LateralVelocity = 0f;
		item.Yaw = 0f;
		item.Spin = 0f;
		item.ImpactTimer = 0f;
		item.HighBeamEvading = false;
		item.HighBeamManeuverTimer = 0f;
		item.HighBeamCooldown = 0f;
		item.HighBeamTargetX = lanes[lane];
		item.HighBeamPressure = 0f;
		item.HighBeamLastFlashTime = -10f;
		item.HighBeamRollingYield = false;
		item.CourtesyYieldTimer = 0f;
		item.CourtesyYieldSpeed = 0f;
		item.Lights = car.GetComponent<TrafficVehicleLights>();
		item.Wheels = car.GetComponent<TrafficWheelAnimator>();
		item.PendingLane = -1;
		item.TurnSignalDirection = 0;
		item.TurnSignalLeadTimer = 0f;
		item.BrakeLightHold = 0f;
		item.WantsLaneChange = false;
		item.RampEscapeLane = -1;
		item.StoppingForSignal = false;
		item.NearMiss = false;
		item.DriverStyle = style;
		pickups.Add(item);
	}

	private void ApplyTrafficDriverAppearance(GameObject trafficCar, TrafficDriverStyle style)
	{
		/* [InfernoLobby patch] */ style = (TrafficDriverStyle)InfernoDriving.NextPaintStyle();
		if (trafficCar == null)
		{
			return;
		}
		TrafficVehicleAppearance appearance = trafficCar.GetComponent<TrafficVehicleAppearance>();
		if (appearance == null)
		{
			return;
		}
		Color paint;
		if (style == TrafficDriverStyle.Slow)
		{
			paint = Color.Lerp(new Color(0.30f, 0.15f, 0.05f, 1f), new Color(0.44f, 0.24f, 0.08f, 1f), Random.value);
		}
		else if (style == TrafficDriverStyle.Cautious)
		{
			paint = Color.Lerp(new Color(0.05f, 0.10f, 0.30f, 1f), new Color(0.10f, 0.18f, 0.42f, 1f), Random.value);
		}
		else if (style == TrafficDriverStyle.Aggressive)
		{
			paint = Color.Lerp(new Color(0.34f, 0.03f, 0.05f, 1f), new Color(0.52f, 0.06f, 0.07f, 1f), Random.value);
		}
		else if (style == TrafficDriverStyle.Fast)
		{
			paint = Color.Lerp(new Color(0.04f, 0.20f, 0.09f, 1f), new Color(0.09f, 0.32f, 0.15f, 1f), Random.value);
		}
		else
		{
			paint = Color.Lerp(new Color(0.15f, 0.16f, 0.19f, 1f), new Color(0.30f, 0.32f, 0.36f, 1f), Random.value);
		}
		appearance.ApplyPaint(paint);
	}

	private TrafficDriverStyle ChooseTrafficDriverStyle(float pressure)
	{
		float roll = Random.value;
		float t = Mathf.Clamp01(pressure);
		if (roll < Mathf.Lerp(0.18f, 0.09f, t))
		{
			return TrafficDriverStyle.Slow;
		}
		if (roll < Mathf.Lerp(0.4f, 0.24f, t))
		{
			return TrafficDriverStyle.Cautious;
		}
		if (roll < Mathf.Lerp(0.74f, 0.58f, t))
		{
			return TrafficDriverStyle.Commuter;
		}
		if (roll < Mathf.Lerp(0.92f, 0.82f, t))
		{
			return TrafficDriverStyle.Fast;
		}
		return TrafficDriverStyle.Aggressive;
	}

	private void GetTrafficDriverProfile(TrafficDriverStyle style, out float aggression, out float patience, out float reaction, out float speedBias, out float laneChangeBias, out float bravery, out float awareness, out float courtesy, out float riskTolerance, out float safeGap)
	{
		if (style == TrafficDriverStyle.Slow)
		{
			aggression = Random.Range(0.12f, 0.28f);
			patience = Random.Range(0.78f, 1f);
			reaction = Random.Range(1.05f, 1.45f);
			speedBias = Random.Range(0.72f, 0.88f);
			laneChangeBias = Random.Range(0.42f, 0.72f);
			bravery = Random.Range(0.05f, 0.22f);
			awareness = Random.Range(0.58f, 0.86f);
			courtesy = Random.Range(0.68f, 1f);
			riskTolerance = Random.Range(0.05f, 0.2f);
			safeGap = Random.Range(22f, 28f);
			return;
		}
		if (style == TrafficDriverStyle.Cautious)
		{
			aggression = Random.Range(0.22f, 0.42f);
			patience = Random.Range(0.68f, 1f);
			reaction = Random.Range(0.85f, 1.25f);
			speedBias = Random.Range(0.88f, 1.02f);
			laneChangeBias = Random.Range(0.62f, 0.92f);
			bravery = Random.Range(0.18f, 0.38f);
			awareness = Random.Range(0.72f, 1f);
			courtesy = Random.Range(0.62f, 0.95f);
			riskTolerance = Random.Range(0.12f, 0.34f);
			safeGap = Random.Range(19f, 25f);
			return;
		}
		if (style == TrafficDriverStyle.Fast)
		{
			aggression = Random.Range(0.58f, 0.78f);
			patience = Random.Range(0.38f, 0.7f);
			reaction = Random.Range(0.48f, 0.85f);
			speedBias = Random.Range(1.12f, 1.28f);
			laneChangeBias = Random.Range(1.05f, 1.38f);
			bravery = Random.Range(0.55f, 0.78f);
			awareness = Random.Range(0.48f, 0.82f);
			courtesy = Random.Range(0.22f, 0.58f);
			riskTolerance = Random.Range(0.54f, 0.78f);
			safeGap = Random.Range(13f, 18f);
			return;
		}
		if (style == TrafficDriverStyle.Aggressive)
		{
			aggression = Random.Range(0.78f, 1f);
			patience = Random.Range(0.18f, 0.48f);
			reaction = Random.Range(0.36f, 0.72f);
			speedBias = Random.Range(1.08f, 1.34f);
			laneChangeBias = Random.Range(1.35f, 1.75f);
			bravery = Random.Range(0.78f, 1f);
			awareness = Random.Range(0.38f, 0.72f);
			courtesy = Random.Range(0.04f, 0.28f);
			riskTolerance = Random.Range(0.78f, 1f);
			safeGap = Random.Range(10.5f, 15.5f);
			return;
		}
		aggression = Random.Range(0.4f, 0.64f);
		patience = Random.Range(0.45f, 0.82f);
		reaction = Random.Range(0.65f, 1.05f);
		speedBias = Random.Range(0.96f, 1.12f);
		laneChangeBias = Random.Range(0.86f, 1.14f);
		bravery = Random.Range(0.36f, 0.62f);
		awareness = Random.Range(0.52f, 0.86f);
		courtesy = Random.Range(0.32f, 0.68f);
		riskTolerance = Random.Range(0.32f, 0.58f);
		safeGap = Random.Range(15f, 21f);
	}

	private int ChooseTrafficLane()
	{
		int bestLane = Random.Range(0, lanes.Length);
		float bestScore = -999f;
		for (int lane = 0; lane < lanes.Length; lane++)
		{
			float score = Mathf.Abs(lane - lanes.Length / 2) * 0.15f + Random.value;
			foreach (Pickup item in pickups)
			{
				if (item.Lane != lane)
				{
					continue;
				}
				float z = item.Transform.position.z;
				if (z < -145f)
				{
					score -= 1.35f;
				}
				if (z < -70f)
				{
					score -= 0.65f;
				}
			}
			if (score > bestScore)
			{
				bestScore = score;
				bestLane = lane;
			}
		}
		return bestLane;
	}

	private float PickTrafficSpawnZ(float minZ, float maxZ)
	{
		float bestZ = Random.Range(minZ, maxZ);
		float bestGap = -1f;
		for (int attempt = 0; attempt < 8; attempt++)
		{
			float z = Random.Range(minZ, maxZ);
			float gap = 999f;
			for (int i = 0; i < pickups.Count; i++)
			{
				Pickup item = pickups[i];
				if (item.Transform != null)
				{
					gap = Mathf.Min(gap, Mathf.Abs(item.Transform.position.z - z));
				}
			}
			if (gap > bestGap)
			{
				bestGap = gap;
				bestZ = z;
			}
			if (gap > 18f)
			{
				break;
			}
		}
		return bestZ;
	}
}
