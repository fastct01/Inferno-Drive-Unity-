using UnityEngine;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_UpdateTrafficAi, _QueueTrafficLaneChange,
// _SetTrafficTurnSignalToward, _RespawnTrafficBehind, _ChooseStoppedTrafficPassLane, _TryTriggerTrafficChainReaction,
// _BestTrafficLaneForSituation, _FindSafeHighBeamYieldLane, _IsHighBeamYieldLaneSafe, _ResolveTrafficVehicleSeparation,
// _ScoreTrafficLane, _TryGetLeadTraffic, _TryGetRearTraffic, _ClosestLane, _IsTrafficOccupyingLane,
// _IsTrafficLaneSafe (x3), _IsTrafficOnlyLaneSafe, _IsPlayerLaneReserved.
public sealed partial class ShadowRunPrototype
{
	private void UpdateTrafficAi(ref Pickup item, float dt)
	{
		float speedBefore = item.Speed;
		item.ChangeTimer -= dt;
		item.LaneChangeCooldown -= dt;
		item.AiThinkTimer -= dt;
		item.CruiseVariationTimer -= dt;
		item.NaturalLaneTimer -= dt;
		item.CutInCooldown -= dt;
		if (item.CourtesyYieldTimer > 0f)
		{
			item.CourtesyYieldTimer -= dt;
		}
		if (item.HighBeamCooldown > 0f)
		{
			item.HighBeamCooldown -= dt;
		}
		if (!item.HighBeamEvading && Time.time - item.HighBeamLastFlashTime > 1.2f)
		{
			item.HighBeamPressure = 0f;
		}
		if (item.HighBeamManeuverTimer > 0f)
		{
			item.HighBeamManeuverTimer -= dt;
		}
		else if (item.HighBeamEvading)
		{
			bool stillYielding = item.PendingLane < 0 && Mathf.Abs(item.TargetX - item.HighBeamTargetX) >= 0.08f;
			if (stillYielding)
			{
				item.HighBeamManeuverTimer = 1f;
			}
			else
			{
				item.HighBeamEvading = false;
				item.HighBeamRollingYield = false;
			}
		}
		Vector3 pos = item.Transform.position;
		int lane = ClosestLane(pos.x);
		if (item.RampEscapeLane >= 0 && item.PendingLane < 0 && Mathf.Abs(pos.x - item.TargetX) < 0.12f && !TrafficNeedsRampEscape(lane, pos.z))
		{
			item.RampEscapeLane = -1;
			item.WantsLaneChange = false;
			item.TurnSignalDirection = 0;
		}
		if (item.PendingLane >= 0)
		{
			item.TurnSignalLeadTimer -= dt;
			if (item.TurnSignalLeadTimer <= 0f)
			{
				item.Lane = item.PendingLane;
				item.TargetX = lanes[item.PendingLane];
				item.PendingLane = -1;
				item.WantsLaneChange = false;
			}
		}
		if (item.HighBeamEvading)
		{
			SetTrafficTurnSignalToward(ref item, item.HighBeamTargetX, pos.x);
			if (item.PendingLane < 0)
			{
				int yieldLane = ClosestLane(item.HighBeamTargetX);
				if (!(Mathf.Abs(item.TargetX - item.HighBeamTargetX) < 0.08f) && IsHighBeamYieldLaneSafe(yieldLane, pos.z, item.Transform, item))
				{
					QueueTrafficLaneChange(ref item, yieldLane, pos.x);
					item.TurnSignalLeadTimer = 0f;
				}
			}
			item.ChangeTimer = Mathf.Max(item.ChangeTimer, 0.8f);
			item.LaneChangeCooldown = Mathf.Max(item.LaneChangeCooldown, 0.8f);
		}
		float playerOffsetZ = player.position.z - pos.z;
		bool nearPlayer = Mathf.Abs(playerOffsetZ) < 62f;
		bool urgent = Mathf.Abs(playerOffsetZ) < 20f || item.ImpactTimer > 0f || item.PanicTimer > 0.05f || item.StoppingForSignal;
		bool think = item.AiThinkTimer <= 0f;
		if (think)
		{
			item.AiThinkTimer = urgent ? Random.Range(0.12f, 0.2f) : (nearPlayer ? Random.Range(0.22f, 0.36f) : Random.Range(0.42f, 0.72f));
			if (item.CruiseVariationTimer <= 0f)
			{
				item.CruiseVariation = Random.Range(-Mathf.Lerp(0.7f, 1.8f, item.Aggression), Mathf.Lerp(0.7f, 2.2f, item.Aggression));
				item.CruiseVariationTimer = Random.Range(4f, 9f);
			}
		}
		float laneDistanceToPlayer = Mathf.Abs(lanes[lane] - playerX);
		bool playerAheadClose = playerOffsetZ > 0f && playerOffsetZ < Mathf.Lerp(38f, 22f, item.RiskTolerance);
		bool playerBehindClose = playerOffsetZ < 0f && playerOffsetZ > -Mathf.Lerp(78f, 48f, item.RiskTolerance);
		bool playerTailing = pos.z > Mathf.Lerp(-70f, -52f, item.Bravery) && pos.z < -4f && laneDistanceToPlayer < Mathf.Lerp(1.95f, 1.38f, item.Bravery);
		bool playerBrakingBehind = playerTailing && brakePressure > 0.35f && laneDistanceToPlayer < Mathf.Lerp(2.15f, 1.45f, item.Awareness);
		bool playerSwervingBehind = playerTailing && Mathf.Abs(velocityX) > Mathf.Lerp(9f, 4.8f, item.Awareness) && Mathf.Abs(pos.z - player.position.z) < Mathf.Lerp(28f, 42f, item.Awareness) && Mathf.Abs(pos.x - playerX) < 3.35f;
		bool playerThreatFromAhead = playerBehindClose && -playerOffsetZ < Mathf.Lerp(18f, 26f, item.Awareness) && laneDistanceToPlayer < Mathf.Lerp(1.22f, 1.58f, item.Awareness) && (Mathf.Abs(velocityX) > Mathf.Lerp(8.5f, 5.2f, item.Awareness) || brakePressure > 0.48f);
		if (item.ImpactTimer > 0f)
		{
			item.ImpactTimer -= dt;
		}
		item.PanicTimer = Mathf.MoveTowards(item.PanicTimer, 0f, dt);
		item.TailgateTimer = Mathf.MoveTowards(item.TailgateTimer, 0f, dt);
		if (playerThreatFromAhead && item.ContactCooldown <= 0f)
		{
			item.PanicTimer = Mathf.Max(item.PanicTimer, Random.Range(0.9f, 1.55f));
			item.ContactCooldown = Random.Range(0.55f, 0.95f);
			item.LaneChangeCooldown = Mathf.Min(item.LaneChangeCooldown, Mathf.Lerp(0.55f, 0.12f, item.Awareness));
			TryTriggerTrafficChainReaction(ref item, lane, pos, dt);
		}
		else if (think && (playerBrakingBehind || playerSwervingBehind) && Random.value < item.Awareness * Mathf.Lerp(0.4f, 1.1f, 1f - item.RiskTolerance) * dt * 7f)
		{
			item.PanicTimer = Mathf.Max(item.PanicTimer, Random.Range(0.45f, 1.05f));
			item.LaneChangeCooldown = Mathf.Min(item.LaneChangeCooldown, Mathf.Lerp(0.85f, 0.18f, item.Awareness));
		}
		if (playerBehindClose && laneDistanceToPlayer < 1.55f && item.Bravery > 0.62f && item.Courtesy < 0.45f)
		{
			item.TailgateTimer = Mathf.Max(item.TailgateTimer, Random.Range(0.5f, 1.25f));
		}
		bool hasLead = TryGetLeadTraffic(lane, pos.z, item.Transform, out float leadGap, out float leadSpeed);
		bool hasSignal = TryGetStopSignal(pos.z, lane, out float stopZ);
		float signalGap = hasSignal ? pos.z - stopZ : 999f;
		item.StoppingForSignal = hasSignal && signalGap > 0.15f && signalGap < 90f;
		bool leadClose = hasLead && leadGap < item.SafeGap + Mathf.Lerp(3f, 10f, 1f - item.Patience);
		bool leadAnnoying = leadClose && leadGap < item.SafeGap + 12f && item.Aggression > 0.58f;
		int playerLane = ClosestLane(playerX);
		bool cutIn = think && item.CutInCooldown <= 0f && !item.HighBeamEvading && item.CourtesyYieldTimer <= 0f && item.PendingLane < 0 && item.TurnSignalDirection == 0
			&& lane != playerLane && Mathf.Abs(lane - playerLane) == 1 && playerOffsetZ > 9f && playerOffsetZ < 31f && item.RiskTolerance > 0.54f
			&& IsTrafficOnlyLaneSafe(playerLane, pos.z, item.Transform, item.RiskTolerance);
		if (cutIn)
		{
			float cutInChance = item.DriverStyle == TrafficDriverStyle.Aggressive ? 0.14f : (item.DriverStyle == TrafficDriverStyle.Fast ? 0.045f : 0f);
			if (Random.value < cutInChance)
			{
				QueueTrafficLaneChange(ref item, playerLane, pos.x);
				item.TurnSignalLeadTimer = Mathf.Lerp(1.05f, 0.78f, item.Aggression);
				item.LaneChangeCooldown = Random.Range(3.2f, 5.8f);
				item.CutInCooldown = Random.Range(9f, 16f);
			}
		}
		bool changingLane = item.PendingLane >= 0 || item.TurnSignalDirection > 0;
		bool mayChangeLane = !item.HighBeamEvading && item.CourtesyYieldTimer <= 0f && !changingLane;
		bool considerLaneChange = mayChangeLane && think && (leadClose || leadAnnoying || item.PanicTimer > 0.05f) && !item.StoppingForSignal && item.LaneChangeCooldown <= 0f;
		if (considerLaneChange)
		{
			item.IntentTimer += Mathf.Max(dt, item.AiThinkTimer);
			float intentThreshold = Mathf.Lerp(0.7f, 0.12f, item.Aggression) / Mathf.Max(0.65f, item.LaneChangeBias);
			if (item.IntentTimer >= intentThreshold)
			{
				int bestLane = BestTrafficLaneForSituation(item, lane, pos.z, item.Transform, item.PanicTimer > 0.05f, leadGap);
				if (bestLane != lane)
				{
					QueueTrafficLaneChange(ref item, bestLane, pos.x);
					item.LaneChangeCooldown = Mathf.Lerp(5.2f, 1.45f, item.Aggression) / Mathf.Max(0.68f, item.LaneChangeBias);
					item.ChangeTimer = Random.Range(1.6f, 3.6f);
					item.IntentTimer = 0f;
					item.WantsLaneChange = false;
				}
			}
		}
		else
		{
			item.IntentTimer = Mathf.MoveTowards(item.IntentTimer, 0f, dt * 1.5f);
			if (item.ChangeTimer <= 0f)
			{
				item.ChangeTimer = Random.Range(3.8f, 7.2f) / Mathf.Max(0.72f, item.LaneChangeBias);
			}
		}
		item.CruiseSpeed = Mathf.MoveTowards(item.CruiseSpeed, item.TargetSpeed + item.CruiseVariation, Mathf.Lerp(0.45f, 1.15f, item.Aggression) * dt);
		float desiredSpeed = item.CruiseSpeed;
		if (item.PanicTimer > 0f)
		{
			desiredSpeed = Mathf.Min(desiredSpeed, Mathf.Max(0f, speed - Mathf.Lerp(14f, 30f, item.Awareness)));
		}
		if (item.TailgateTimer > 0f && !item.StoppingForSignal)
		{
			desiredSpeed = Mathf.Max(desiredSpeed, speed + Mathf.Lerp(6f, 15f, item.Bravery));
		}
		if (item.HighBeamEvading && item.PendingLane < 0 && Mathf.Abs(item.TargetX - item.HighBeamTargetX) >= 0.08f)
		{
			desiredSpeed = Mathf.Max(desiredSpeed, item.Speed - 0.2f);
		}
		if (item.CourtesyYieldTimer > 0f)
		{
			desiredSpeed = Mathf.Min(desiredSpeed, item.CourtesyYieldSpeed);
		}
		if (playerTailing && !item.StoppingForSignal)
		{
			desiredSpeed = Mathf.Max(desiredSpeed, item.Speed - 0.35f);
		}
		if (hasLead)
		{
			float followGap = item.SafeGap + Mathf.Clamp(item.Speed * Mathf.Lerp(0.24f, 0.12f, item.Bravery), 1.8f, 12f);
			if (leadGap < followGap)
			{
				float followSpeed = Mathf.Max(0f, leadSpeed - Mathf.Lerp(7f, 0.8f, item.Aggression));
				if (leadGap < 7.5f)
				{
					followSpeed = 0f;
				}
				desiredSpeed = Mathf.Min(desiredSpeed, followSpeed);
			}
			else if (leadGap < followGap + 12f)
			{
				desiredSpeed = Mathf.Min(desiredSpeed, leadSpeed + Mathf.Lerp(1.2f, 7.5f, item.Aggression));
			}
		}
		if (item.StoppingForSignal && signalGap > 0f)
		{
			float approachSpeed = Mathf.Lerp(0f, Mathf.Lerp(16f, 25f, item.Aggression), Mathf.InverseLerp(4.5f, 42f, signalGap));
			desiredSpeed = Mathf.Min(desiredSpeed, approachSpeed);
			if (signalGap < 4.8f)
			{
				desiredSpeed = 0f;
			}
		}
		float minSpeed = desiredSpeed < 10f ? 0f : 10f;
		float maxSpeed = Mathf.Lerp(32f, 44f, Mathf.InverseLerp(0.72f, 1.34f, item.SpeedBias));
		float targetSpeed = Mathf.Clamp(desiredSpeed, minSpeed, maxSpeed);
		float speedRate = targetSpeed < item.Speed
			? Mathf.Lerp(8f, 15f, item.Awareness) / Mathf.Max(0.72f, item.Reaction)
			: Mathf.Lerp(3.8f, 7.5f, item.Aggression) / Mathf.Max(0.72f, item.Reaction);
		if (item.StoppingForSignal || item.PanicTimer > 0.05f)
		{
			speedRate = Mathf.Max(speedRate, Mathf.Lerp(13f, 21f, item.Awareness));
		}
		item.Speed = Mathf.MoveTowards(item.Speed, targetSpeed, speedRate * dt);
		pos.z += (speed - item.Speed) * dt;
		float minimumLeadGap = Mathf.Lerp(7.4f, 4.9f, item.RiskTolerance);
		if (TryGetLeadTraffic(lane, pos.z, item.Transform, out float currentLeadGap, out float currentLeadSpeed) && currentLeadGap < minimumLeadGap)
		{
			float leadZ = pos.z - currentLeadGap;
			pos.z = Mathf.Max(pos.z, leadZ + minimumLeadGap);
			item.Speed = Mathf.Min(item.Speed, currentLeadSpeed);
		}
		if (item.StoppingForSignal && pos.z < stopZ + 0.15f)
		{
			pos.z = stopZ + 0.15f;
			item.Speed = 0f;
		}
		float highBeamUrgency = Mathf.Clamp01((item.HighBeamPressure - 1f) * 0.5f);
		bool rampEscaping = item.RampEscapeLane >= 0 && Mathf.Abs(pos.x - lanes[Mathf.Clamp(item.RampEscapeLane, 0, lanes.Length - 1)]) > 0.12f;
		bool maneuvering = item.PendingLane >= 0 || item.RampEscapeLane >= 0 || item.HighBeamEvading || item.TurnSignalDirection != 0 || item.PanicTimer > 0.05f;
		if (maneuvering)
		{
			item.NaturalLaneTarget = 0f;
		}
		else if (item.NaturalLaneTimer <= 0f)
		{
			float wander = Mathf.Lerp(0.28f, 0.18f, item.Awareness);
			item.NaturalLaneTarget = Random.Range(-wander, wander);
			item.NaturalLaneTimer = Random.Range(3.4f, 7.2f);
		}
		float wanderRate = maneuvering ? 0.65f : Mathf.Lerp(0.11f, 0.15f, item.Awareness);
		item.NaturalLaneOffset = Mathf.MoveTowards(item.NaturalLaneOffset, item.NaturalLaneTarget, wanderRate * dt);
		float targetX = item.TargetX + item.NaturalLaneOffset;
		float laneSmoothTime = rampEscaping
			? 0.18f
			: (item.HighBeamManeuverTimer > 0f ? Mathf.Lerp(0.62f, 0.4f, highBeamUrgency) : Mathf.Lerp(0.78f, 0.48f, item.Aggression) / Mathf.Max(0.9f, Mathf.Min(1.2f, item.LaneChangeBias)));
		float laneMaxSpeed = rampEscaping ? 11f : Mathf.Lerp(3.4f, 5.4f, item.Aggression);
		pos.x = Mathf.SmoothDamp(pos.x, targetX, ref item.LaneVelocity, laneSmoothTime, laneMaxSpeed, dt);
		if (item.PanicTimer > 0f && laneDistanceToPlayer < 2.4f)
		{
			float away = Mathf.Sign(pos.x - playerX);
			if (Mathf.Abs(away) < 0.01f)
			{
				away = Random.value < 0.5f ? -1f : 1f;
			}
			pos.x += away * Mathf.Lerp(0.18f, 0.48f, item.Awareness) * dt;
			item.Yaw += away * Mathf.Lerp(4f, 10f, item.Awareness) * dt;
		}
		pos.x = Mathf.Clamp(pos.x + item.LateralVelocity * dt, PlayerMinX, PlayerMaxX);
		item.LateralVelocity = Mathf.MoveTowards(item.LateralVelocity, 0f, 8.5f * dt);
		ResolveTrafficVehicleSeparation(ref item, ref pos);
		item.Transform.position = pos;
		if (item.PendingLane < 0 && item.RampEscapeLane < 0 && !item.HighBeamEvading && Mathf.Abs(pos.x - item.TargetX) < 0.075f && Mathf.Abs(item.LaneVelocity) < 0.12f)
		{
			item.TurnSignalDirection = 0;
		}
		bool braking = (item.StoppingForSignal && (signalGap < 42f || item.Speed < 2f))
			|| desiredSpeed < speedBefore - 0.45f
			|| item.Speed < speedBefore - 0.035f
			|| item.PanicTimer > 0.05f
			|| item.CourtesyYieldTimer > 0f;
		item.BrakeLightHold = braking ? 0.65f : Mathf.Max(0f, item.BrakeLightHold - dt);
		bool signalOn = item.TurnSignalDirection != 0 && Mathf.Repeat(Time.time + Mathf.Abs(InstanceIdOf(item.Transform)) * 0.017f, 0.72f) < 0.42f;
		if (item.Lights != null)
		{
			item.Lights.SetState(item.BrakeLightHold > 0f, item.TurnSignalDirection, signalOn);
		}
		float steerYaw = Mathf.Clamp(-item.LaneVelocity * 4.2f, -14f, 14f);
		item.Spin = Mathf.MoveTowards(item.Spin, 0f, 65f * dt);
		item.Yaw = Mathf.MoveTowards(item.Yaw, 0f, 48f * dt);
		Quaternion targetRotation = Quaternion.Euler(0f, steerYaw + item.Yaw + item.Spin, 0f);
		item.Transform.rotation = Quaternion.Slerp(item.Transform.rotation, targetRotation, 1f - Mathf.Exp(-12f * dt));
		if (item.Wheels != null)
		{
			item.Wheels.UpdateSpin(item.Speed, dt);
		}
	}

	// Unity 6000.6 rejects Object.GetInstanceID() and the EntityId->int conversion (obsolete-as-error). The shipped
	// code only used the instance id to de-phase the turn-signal blink per car, so the raw entity id (whose low
	// 32 bits carry the instance id) stands in for it. See docs/remake-assumptions.md.
	private static int InstanceIdOf(Object target)
	{
		return unchecked((int)target.GetEntityId().GetRawData());
	}

	private void QueueTrafficLaneChange(ref Pickup item, int targetLane, float currentX)
	{
		targetLane = Mathf.Clamp(targetLane, 0, lanes.Length - 1);
		int currentLane = ClosestLane(currentX);
		targetLane = Mathf.Clamp(targetLane, currentLane - 1, currentLane + 1);
		if (Mathf.Abs(lanes[targetLane] - currentX) < 0.08f)
		{
			return;
		}
		item.PendingLane = targetLane;
		item.TurnSignalLeadTimer = Mathf.Lerp(1.35f, 0.82f, item.Aggression);
		item.WantsLaneChange = true;
		SetTrafficTurnSignalToward(ref item, lanes[targetLane], currentX);
	}

	private void SetTrafficTurnSignalToward(ref Pickup item, float targetX, float currentX)
	{
		if (Mathf.Abs(targetX - currentX) < 0.08f)
		{
			return;
		}
		item.TurnSignalDirection = targetX > currentX ? -1 : 1;
	}

	private void RespawnTrafficBehind(ref Pickup item, float pressure, bool passStoppedPlayer = false)
	{
		int lane = passStoppedPlayer ? ChooseStoppedTrafficPassLane() : ChooseTrafficLane();
		float z = passStoppedPlayer ? PickTrafficSpawnZ(player.position.z + 72f, player.position.z + 138f) : PickTrafficSpawnZ(-210f, -120f);
		lane = FindRampSafeTrafficLane(lane, z);
		item.Lane = lane;
		item.TargetX = lanes[lane];
		item.Transform.position = new Vector3(lanes[lane], 0f, z);
		item.Transform.rotation = Quaternion.identity;
		item.Speed = Random.Range(Mathf.Lerp(13f, 16f, pressure), Mathf.Lerp(23f, 30f, pressure)) * item.SpeedBias;
		item.TargetSpeed = Mathf.Clamp(item.TargetSpeed, passStoppedPlayer ? 19f : 11f, Mathf.Lerp(34f, 42f, pressure));
		item.CruiseSpeed = item.TargetSpeed;
		item.LaneVelocity = 0f;
		item.ChangeTimer = Random.Range(1.2f, 3.4f) / Mathf.Max(0.55f, item.LaneChangeBias);
		item.LaneChangeCooldown = Random.Range(1.4f, 4.2f) / Mathf.Max(0.6f, item.LaneChangeBias);
		item.IntentTimer = 0f;
		item.AiThinkTimer = Random.Range(0f, 0.18f);
		item.CruiseVariation = Random.Range(-1.4f, 1.4f);
		item.CruiseVariationTimer = Random.Range(3.5f, 8f);
		item.CutInCooldown = Random.Range(4f, 10f);
		item.PanicTimer = 0f;
		item.TailgateTimer = 0f;
		item.ContactCooldown = 0.45f;
		item.CollisionCooldown = 0.12f;
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
		item.PendingLane = -1;
		item.TurnSignalDirection = 0;
		item.TurnSignalLeadTimer = 0f;
		item.BrakeLightHold = 0f;
		item.WantsLaneChange = false;
		item.RampEscapeLane = -1;
		item.StoppingForSignal = false;
		item.NearMiss = false;
		if (item.Lights != null)
		{
			item.Lights.ResetLights();
		}
		if (item.Wheels != null)
		{
			item.Wheels.ResetSpin();
		}
	}

	private int ChooseStoppedTrafficPassLane()
	{
		int bestLane = 0;
		float bestScore = float.NegativeInfinity;
		for (int lane = 0; lane < lanes.Length; lane++)
		{
			float laneDistance = Mathf.Abs(lanes[lane] - playerX);
			float nearestBehind = 160f;
			for (int i = 0; i < pickups.Count; i++)
			{
				Pickup other = pickups[i];
				if (other.Transform == null || ClosestLane(other.Transform.position.x) != lane)
				{
					continue;
				}
				if (other.Transform.position.z < player.position.z + 10f)
				{
					continue;
				}
				nearestBehind = Mathf.Min(nearestBehind, other.Transform.position.z - player.position.z);
			}
			float score = laneDistance * 8f + Mathf.Min(nearestBehind, 80f) * 0.12f + Random.Range(0f, 1.5f);
			if (laneDistance < 2.15f)
			{
				score -= 30f;
			}
			if (score > bestScore)
			{
				bestScore = score;
				bestLane = lane;
			}
		}
		return bestLane;
	}

	private void TryTriggerTrafficChainReaction(ref Pickup front, int lane, Vector3 frontPos, float dt)
	{
		if (trafficIncidentCooldown > 0f)
		{
			return;
		}
		if (front.Transform == null)
		{
			return;
		}
		int rearIndex = -1;
		float rearGap = 999f;
		for (int i = 0; i < pickups.Count; i++)
		{
			Pickup other = pickups[i];
			if (other.Transform == null || other.Transform == front.Transform || !IsTrafficOccupyingLane(other, lane))
			{
				continue;
			}
			float gap = other.Transform.position.z - frontPos.z;
			if (gap <= 0f || gap >= rearGap)
			{
				continue;
			}
			rearGap = gap;
			rearIndex = i;
		}
		if (rearIndex < 0)
		{
			return;
		}
		Pickup rear = pickups[rearIndex];
		float closingSpeed = rear.Speed - front.Speed;
		float reactionGap = Mathf.Lerp(11.5f, 7.2f, rear.RiskTolerance);
		bool closingFast = closingSpeed > Mathf.Lerp(7f, 3.5f, rear.Awareness);
		float crashChance = Mathf.Lerp(0.06f, 0.22f, 1f - rear.Awareness) + Mathf.Lerp(0.02f, 0.12f, rear.RiskTolerance);
		if (!(rearGap < reactionGap) || !closingFast)
		{
			return;
		}
		if (Random.value > crashChance)
		{
			return;
		}
		trafficIncidentCooldown = Random.Range(7.5f, 13.5f);
		Vector3 rearPos = rear.Transform.position;
		rearPos.z = frontPos.z + Mathf.Lerp(4.2f, 5.2f, rear.RiskTolerance);
		rear.Transform.position = rearPos;
		rear.Speed = Mathf.Max(0f, front.Speed * Mathf.Lerp(0.55f, 0.82f, rear.RiskTolerance));
		rear.TargetSpeed = Mathf.Min(rear.TargetSpeed, rear.Speed + 8f);
		rear.PanicTimer = Mathf.Max(rear.PanicTimer, Random.Range(0.75f, 1.35f));
		rear.ContactCooldown = Random.Range(0.65f, 1.2f);
		rear.ImpactTimer = 0.85f;
		float side = rearPos.x >= frontPos.x ? 1f : -1f;
		if (Mathf.Abs(rearPos.x - frontPos.x) < 0.12f)
		{
			side = Random.value < 0.5f ? -1f : 1f;
		}
		rear.LateralVelocity = Mathf.Clamp(rear.LateralVelocity + side * Mathf.Lerp(3.5f, 8.5f, rear.RiskTolerance), -11f, 11f);
		rear.Yaw += side * Mathf.Lerp(14f, 34f, closingSpeed / 28f);
		rear.Spin += side * Mathf.Lerp(20f, 58f, closingSpeed / 28f);
		front.Speed = Mathf.Max(0f, front.Speed - Mathf.Lerp(4f, 12f, closingSpeed / 24f));
		front.PanicTimer = Mathf.Max(front.PanicTimer, 0.85f);
		front.LateralVelocity = Mathf.Clamp(front.LateralVelocity - side * Mathf.Lerp(1.2f, 3.8f, closingSpeed / 24f), -8f, 8f);
		front.Yaw -= side * Mathf.Lerp(8f, 18f, closingSpeed / 28f);
		pickups[rearIndex] = rear;
	}

	private int BestTrafficLaneForSituation(Pickup item, int current, float z, Transform self, bool avoidPlayer, float currentLeadGap)
	{
		int bestLane = current;
		float bestScore = ScoreTrafficLane(item, current, current, z, self, avoidPlayer, currentLeadGap);
		int first = Mathf.Max(0, current - 1);
		int last = Mathf.Min(lanes.Length - 1, current + 1);
		for (int lane = first; lane <= last; lane++)
		{
			if (lane == current || !IsTrafficLaneSafe(lane, z, self, item))
			{
				continue;
			}
			float score = ScoreTrafficLane(item, lane, current, z, self, avoidPlayer, currentLeadGap);
			if (score > bestScore)
			{
				bestScore = score;
				bestLane = lane;
			}
		}
		return bestLane;
	}

	private int FindSafeHighBeamYieldLane(int current)
	{
		if (current > 0)
		{
			return current - 1;
		}
		if (current + 1 < lanes.Length)
		{
			return current + 1;
		}
		return current;
	}

	private bool IsHighBeamYieldLaneSafe(int lane, float z, Transform self, Pickup driver)
	{
		if (lane < 0 || lane >= lanes.Length)
		{
			return false;
		}
		if (IsPlayerLaneReserved(lane, z, 0.08f))
		{
			return false;
		}
		for (int i = 0; i < pickups.Count; i++)
		{
			Pickup other = pickups[i];
			if (other.Transform == null || other.Transform == self || !IsTrafficOccupyingLane(other, lane))
			{
				continue;
			}
			float gap = other.Transform.position.z - z;
			bool yielding = other.CourtesyYieldTimer > 0f;
			float behind = yielding ? 5.2f : Mathf.Lerp(7.5f, 6f, driver.Awareness);
			float ahead = yielding ? 7.2f : Mathf.Lerp(11.5f, 9f, driver.Awareness);
			if (gap > -behind && gap < ahead)
			{
				return false;
			}
		}
		return true;
	}

	private void ResolveTrafficVehicleSeparation(ref Pickup item, ref Vector3 pos)
	{
		float minX = 1.9552f;
		float minZ = 4.459f;
		for (int i = 0; i < pickups.Count; i++)
		{
			Pickup other = pickups[i];
			if (other.Transform == null || other.Transform == item.Transform)
			{
				continue;
			}
			Vector3 otherPos = other.Transform.position;
			float dx = pos.x - otherPos.x;
			float dz = pos.z - otherPos.z;
			float absDx = Mathf.Abs(dx);
			float absDz = Mathf.Abs(dz);
			if (absDx >= minX || absDz >= minZ)
			{
				continue;
			}
			float overlapX = minX - absDx;
			float overlapZ = minZ - absDz;
			bool sameLane = IsTrafficOccupyingLane(other, ClosestLane(item.TargetX)) || Mathf.Abs(item.TargetX - other.TargetX) < 1.15f;
			if (sameLane || overlapZ <= overlapX * 0.8f)
			{
				if (dz >= 0f)
				{
					pos.z = otherPos.z + minZ;
					item.Speed = Mathf.Min(item.Speed, other.Speed);
					item.TargetSpeed = Mathf.Min(item.TargetSpeed, Mathf.Max(0f, other.Speed - 2.5f));
				}
				else
				{
					pos.z = otherPos.z - minZ;
					item.Speed = Mathf.Max(item.Speed, other.Speed + 1.5f);
				}
			}
			else
			{
				float side = dx >= 0f ? 1f : -1f;
				if (absDx < 0.05f)
				{
					side = pos.x >= lanes[ClosestLane(pos.x)] ? 1f : -1f;
				}
				pos.x = Mathf.Clamp(pos.x + side * (overlapX + 0.08f) * 0.55f, PlayerMinX, PlayerMaxX);
				item.LateralVelocity = Mathf.Clamp(item.LateralVelocity + side * 1.35f, -6.5f, 6.5f);
				item.Yaw += side * 4.5f;
			}
			item.ContactCooldown = Mathf.Max(item.ContactCooldown, 0.16f);
		}
	}

	private float ScoreTrafficLane(Pickup item, int lane, int current, float z, Transform self, bool avoidPlayer, float currentLeadGap)
	{
		bool hasLead = TryGetLeadTraffic(lane, z, self, out float leadGap, out float leadSpeed);
		bool hasRear = TryGetRearTraffic(lane, z, self, out float rearGap, out float rearSpeed);
		float score = Random.value * Mathf.Lerp(0.05f, 0.18f, item.Aggression);
		score += lane == current
			? Mathf.Lerp(0.68f, 0.22f, item.LaneChangeBias - 0.45f)
			: -Mathf.Abs(lane - current) * Mathf.Lerp(0.34f, 0.12f, item.Bravery);
		score += Mathf.Clamp((hasLead ? leadGap : 90f) / 90f, 0f, 1f) * Mathf.Lerp(1.9f, 1.1f, item.Bravery);
		score += Mathf.Clamp((hasRear ? rearGap : 70f) / 70f, 0f, 1f) * Mathf.Lerp(1.1f, 0.48f, item.Bravery);
		if (hasLead)
		{
			score += Mathf.Clamp((leadSpeed - item.Speed + 8f) / 38f, -0.55f, 0.62f) * item.SpeedBias;
		}
		if (hasRear && rearGap < Mathf.Lerp(24f, 13f, item.Bravery) && rearSpeed > item.Speed + 4f)
		{
			score -= Mathf.Lerp(1.2f, 0.42f, item.Bravery);
		}
		if (avoidPlayer)
		{
			score += Mathf.Abs(lanes[lane] - playerX) * Mathf.Lerp(0.62f, 0.26f, item.Bravery);
		}
		float predictedPlayerX = Mathf.Clamp(playerX + velocityX * 0.28f, PlayerMinX, PlayerMaxX);
		float playerOffsetZ = player.position.z - z;
		float playerReach = Mathf.Lerp(96f, 52f, item.RiskTolerance) + Mathf.Clamp(speed * 0.32f, 0f, 24f);
		float playerPressure = Mathf.InverseLerp(2.55f, 0.55f, Mathf.Abs(lanes[lane] - predictedPlayerX)) * Mathf.InverseLerp(playerReach, 5f, Mathf.Abs(playerOffsetZ));
		if (playerOffsetZ > -8f && playerOffsetZ < playerReach)
		{
			playerPressure = Mathf.Max(playerPressure, Mathf.InverseLerp(2.25f, 0.35f, Mathf.Abs(lanes[lane] - predictedPlayerX)));
		}
		score -= playerPressure * Mathf.Lerp(1.5f, 0.38f, item.RiskTolerance);
		if (lane != current && IsPlayerLaneReserved(lane, z, item.RiskTolerance))
		{
			score -= Mathf.Lerp(3.4f, 1.35f, item.RiskTolerance);
		}
		if (lane != current && currentLeadGap > Mathf.Lerp(34f, 22f, item.Aggression))
		{
			score -= Mathf.Lerp(0.72f, 0.22f, item.LaneChangeBias - 0.5f);
		}
		if (item.DriverStyle == TrafficDriverStyle.Slow && lane != current)
		{
			score -= 0.38f;
		}
		if (item.DriverStyle == TrafficDriverStyle.Aggressive && lane != current)
		{
			score += 0.28f;
		}
		return score;
	}

	private bool TryGetLeadTraffic(int lane, float z, Transform self, out float gap, out float leadSpeed)
	{
		gap = 999f;
		leadSpeed = 0f;
		bool found = false;
		for (int i = 0; i < pickups.Count; i++)
		{
			Pickup other = pickups[i];
			if (other.Transform == self || !IsTrafficOccupyingLane(other, lane))
			{
				continue;
			}
			float distanceAhead = z - other.Transform.position.z;
			if (distanceAhead <= 0f || distanceAhead >= gap)
			{
				continue;
			}
			gap = distanceAhead;
			leadSpeed = other.Speed;
			found = true;
		}
		return found;
	}

	private bool TryGetRearTraffic(int lane, float z, Transform self, out float gap, out float rearSpeed)
	{
		gap = 999f;
		rearSpeed = 0f;
		bool found = false;
		for (int i = 0; i < pickups.Count; i++)
		{
			Pickup other = pickups[i];
			if (other.Transform == self || !IsTrafficOccupyingLane(other, lane))
			{
				continue;
			}
			float distanceBehind = other.Transform.position.z - z;
			if (distanceBehind <= 0f || distanceBehind >= gap)
			{
				continue;
			}
			gap = distanceBehind;
			rearSpeed = other.Speed;
			found = true;
		}
		return found;
	}

	private int ClosestLane(float x)
	{
		int best = 0;
		float bestDistance = Mathf.Abs(x - lanes[0]);
		for (int i = 1; i < lanes.Length; i++)
		{
			float distance = Mathf.Abs(x - lanes[i]);
			if (distance < bestDistance)
			{
				bestDistance = distance;
				best = i;
			}
		}
		return best;
	}

	private bool IsTrafficOccupyingLane(Pickup other, int lane)
	{
		if (lane < 0 || lane >= lanes.Length || other.Transform == null)
		{
			return false;
		}
		float laneX = lanes[lane];
		if (other.Lane == lane || Mathf.Abs(other.TargetX - laneX) < 0.9f)
		{
			return true;
		}
		return Mathf.Abs(other.Transform.position.x - laneX) < 1.08f;
	}

	private bool IsTrafficLaneSafe(int lane, float z, Transform self)
	{
		return IsTrafficLaneSafe(lane, z, self, 0.35f);
	}

	private bool IsTrafficLaneSafe(int lane, float z, Transform self, Pickup driver)
	{
		return IsTrafficLaneSafe(lane, z, self, driver.RiskTolerance);
	}

	private bool IsTrafficLaneSafe(int lane, float z, Transform self, float risk)
	{
		if (lane < 0 || lane >= lanes.Length)
		{
			return false;
		}
		if (IsPlayerLaneReserved(lane, z, risk))
		{
			return false;
		}
		foreach (Pickup other in pickups)
		{
			if (other.Transform == self || !IsTrafficOccupyingLane(other, lane))
			{
				continue;
			}
			float gap = other.Transform.position.z - z;
			if (gap > -Mathf.Lerp(18f, 10f, risk) && gap < Mathf.Lerp(29f, 18f, risk))
			{
				return false;
			}
		}
		return true;
	}

	private bool IsTrafficOnlyLaneSafe(int lane, float z, Transform self, float risk)
	{
		if (lane < 0 || lane >= lanes.Length)
		{
			return false;
		}
		for (int i = 0; i < pickups.Count; i++)
		{
			Pickup other = pickups[i];
			if (other.Transform == null || other.Transform == self || !IsTrafficOccupyingLane(other, lane))
			{
				continue;
			}
			float gap = other.Transform.position.z - z;
			if (gap > -Mathf.Lerp(19f, 12f, risk) && gap < Mathf.Lerp(31f, 21f, risk))
			{
				return false;
			}
		}
		return true;
	}

	private bool IsPlayerLaneReserved(int lane, float z, float risk)
	{
		float lookAhead = Mathf.Lerp(0.48f, 0.2f, risk);
		float predictedPlayerX = Mathf.Clamp(playerX + velocityX * lookAhead, PlayerMinX, PlayerMaxX);
		float predictedDistance = Mathf.Abs(lanes[lane] - predictedPlayerX);
		float playerOffsetZ = player.position.z - z;
		float reserveWidth = Mathf.Lerp(2.35f, 1.35f, risk);
		float reserveLength = Mathf.Lerp(96f, 54f, risk) + Mathf.Clamp(speed * 0.34f, 0f, 26f);
		if (predictedDistance < reserveWidth && playerOffsetZ > -8f && playerOffsetZ < reserveLength)
		{
			return true;
		}
		if (Mathf.Abs(lanes[lane] - playerX) < Mathf.Lerp(1.85f, 1.12f, risk) && playerOffsetZ > -10f && playerOffsetZ < Mathf.Lerp(34f, 18f, risk))
		{
			return true;
		}
		return false;
	}
}
