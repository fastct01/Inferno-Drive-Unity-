using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_UpdatePlayer, _UpdatePlayerAirborne, _ReadTouchSteering,
// _ReadAccelerationDragSteering, _ResetGameplayTouches, _IsGameplayAccelerationHeld, _UpdateBrakeHaptic,
// _PlayCollisionHaptic, _PlayImpactHaptic, _UpdateCamera, _ResetCameraImmediate, _UpdateHud, _UpdateSpeedometerHud,
// _UpdateEvents, _UpdateSceneLighting, _AnimateSanta, _TriggerSanta.
public sealed partial class ShadowRunPrototype
{
	private void UpdatePlayer(float dt)
	{
		float speedAtFrameStart = speed;
		float keyboardSteer = 0f;
		if (KeyHeld(276, "left") || KeyHeld(97, "a"))
		{
			keyboardSteer += 1f;
		}
		if (KeyHeld(275, "right") || KeyHeld(100, "d"))
		{
			keyboardSteer -= 1f;
		}
		float steer = keyboardSteer == 0f ? ReadTouchSteering() : keyboardSteer;
		bool touchSteerWhileBoosting = keyboardSteer == 0f && touchBoostHeld && Mathf.Abs(steer) > 0.0001f;
		// [InfernoLobby patch] the brake input was removed (down/S/HUD brake button all replaced by `false`).
		bool braking = false;
		UpdateBrakeHaptic(braking);
		SetPlayerBrakeLights(braking);
		bool boosting = (touchBoostHeld || KeyHeld(32, "space") || KeyHeld(273, "up")) && !braking;
		RegisterDailyBoostState(boosting);
		bool drifting = KeyHeld(304, "shift") || KeyHeld(303, "shift");
		/* [user request 2026-09-21: cars] garage upgrades: ENGINE / HANDLING multiply the arcade values, ENGINE and
		   NITRO raise the cruise / boost caps (InfernoUpgrades), read once per frame from the active car */
		string upgradeId = ActiveVehicle.Id;
		float arcadeAcceleration = Mathf.Max(0.5f, ActiveVehicle.ArcadeAcceleration * InfernoUpgrades.AccelMultiplier(upgradeId));
		float arcadeSteering = Mathf.Max(0.5f, ActiveVehicle.ArcadeSteering * InfernoUpgrades.SteerMultiplier(upgradeId));
		float arcadeGrip = Mathf.Max(0.5f, ActiveVehicle.ArcadeGrip * InfernoUpgrades.GripMultiplier(upgradeId));
		float arcadeDrift = Mathf.Max(0.5f, ActiveVehicle.ArcadeDrift);
		float topSpeedBonus = InfernoUpgrades.TopSpeedBonus(upgradeId);
		float boostCapBonus = InfernoUpgrades.BoostCapBonus(upgradeId);
		float boostRateMultiplier = InfernoUpgrades.BoostRateMultiplier(upgradeId);
		// original: float arcadeAcceleration = Mathf.Max(0.5f, ActiveVehicle.ArcadeAcceleration); (steering / grip alike)
		// original: cruise = Mathf.Lerp(29f, 40f, Mathf.Clamp01(distance / 2600f))
		float cruise = InfernoDrivingRules.Cruise(distance, topSpeedBonus); /* [InfernoLobby patch] */   // original: InfernoDrivingRules.Cruise(distance)
		brakePressure = Mathf.MoveTowards(brakePressure, braking ? 1f : 0f, (braking ? 2.85f : 4.2f) * dt);
		// original: targetSpeed = braking ? 0f : (boosting ? 49f : (drifting ? Mathf.Max(20f, cruise - 8f) : cruise))
		float targetSpeed = Mathf.Max(speed, boosting ? InfernoDrivingRules.BoostTop(distance, boostCapBonus) : cruise); /* [InfernoLobby patch] */   // original: BoostTop(distance)
		if (braking && !playerAirborne)
		{
			brakeRecoveryActive = true;
		}
		else if (brakeRecoveryActive && speed >= cruise - 0.15f)
		{
			brakeRecoveryActive = false;
		}
		float recoveryScale = (brakeRecoveryActive && !braking && speed < targetSpeed) ? 0.7f : 1f;
		// original: accelerationRate = (boosting ? 18f : (speed < targetSpeed ? 8.6f : (drifting ? 11f : 6.8f))) * arcadeAcceleration * recoveryScale;
		//           speed = Mathf.MoveTowards(speed, targetSpeed, accelerationRate * dt)
		// [InfernoLobby patch] replaced by the exponential rule:
		speed = InfernoDrivingRules.AdvanceSpeed(speed, cruise, boosting, arcadeAcceleration, dt, playerAirborne, InfernoDrivingRules.BoostTop(distance, boostCapBonus), InfernoDrivingRules.CruiseTop(distance, topSpeedBonus), boostRateMultiplier);   // original: (…, BoostTop(distance), CruiseTop(distance))
		if (braking)
		{
			float brakeRate = Mathf.Lerp(5.5f, 18f, brakePressure) + Mathf.Clamp(speed * 0.14f, 0f, 9f);
			speed = Mathf.MoveTowards(speed, 0f, brakeRate * dt);
		}
		speed = Mathf.Max(speedAtFrameStart, Mathf.Clamp(speed, 0f, boosting ? InfernoDrivingRules.BoostTop(distance, boostCapBonus) : InfernoDrivingRules.CruiseTop(distance, topSpeedBonus))); /* [InfernoLobby patch] */   // original: BoostTop(distance) / CruiseTop(distance)
		if (playerAirborne)
		{
			// original: speed = Mathf.Max(0f, speedAtFrameStart - 0.22f * dt)
			speed = speedAtFrameStart; /* [InfernoLobby patch] airborne drag disabled */
		}
		float steerRate = Mathf.Lerp(7.4f, 5.4f, Mathf.Clamp01(speed / 49f)) * arcadeSteering;
		if (touchSteerWhileBoosting)
		{
			steerRate *= 1.15f;
		}
		float steerReturnRate = Mathf.Lerp(9.5f, 12.5f, Mathf.Clamp01(speed / 49f)) * Mathf.Lerp(0.96f, 1.08f, arcadeGrip);
		steerInput = Mathf.MoveTowards(steerInput, steer, (steer == 0f ? steerReturnRate : steerRate) * dt);
		float speedT = Mathf.Clamp01(speed / 49f);
		float steerMagnitude = Mathf.Abs(steerInput);
		float driftTarget = 0f;
		if (drifting && steerMagnitude > 0.12f && speed > 22f)
		{
			driftTarget = 0.5f + steerMagnitude * 0.34f;
		}
		driftTarget = Mathf.Clamp01(driftTarget * arcadeDrift);
		drift = Mathf.MoveTowards(drift, driftTarget, (driftTarget > drift ? 4.8f : 3.8f) * dt);
		float lowSpeedAuthority = Mathf.Lerp(0.72f, 1f, Mathf.InverseLerp(8f, 30f, speed));
		float lateralSpeed = Mathf.Lerp(7f, 9.2f, speedT) * arcadeSteering * Mathf.Lerp(1f, 1.14f, drift);
		if (playerAirborne)
		{
			lateralSpeed *= 0.58f;
		}
		float targetVelocityX = steerInput * lateralSpeed * lowSpeedAuthority;
		if (drifting && steerMagnitude > 0.08f)
		{
			targetVelocityX += steerInput * Mathf.Lerp(0.8f, 1.9f, speedT);
		}
		float lateralResponse = Mathf.Lerp(14.5f, 11.5f, speedT);
		lateralResponse *= arcadeGrip;
		lateralResponse *= Mathf.Lerp(1.04f, 0.54f, drift);
		lateralResponse *= Mathf.Lerp(1f, 0.8f, tractionLoss);
		if (playerAirborne)
		{
			lateralResponse *= 0.18f;
		}
		if (steer == 0f)
		{
			lateralResponse *= Mathf.Lerp(2.1f, 2.55f, speedT);
		}
		if (steer != 0f && Mathf.Sign(steer) != Mathf.Sign(velocityX) && !drifting)
		{
			lateralResponse *= 1.45f;
		}
		float blendedVelocityX = Mathf.Lerp(velocityX, targetVelocityX, 1f - Mathf.Exp(-lateralResponse * dt));
		float lateralAcceleration = Mathf.Lerp(25f, 36f, speedT) * arcadeSteering * Mathf.Lerp(1f, 0.68f, drift);
		if (touchSteerWhileBoosting)
		{
			lateralAcceleration *= 1.15f;
		}
		if (playerAirborne)
		{
			lateralAcceleration *= 0.22f;
		}
		velocityX = Mathf.MoveTowards(velocityX, blendedVelocityX, lateralAcceleration * dt);
		velocityX += impactYaw * 0.009f * dt;
		if (!playerAirborne && !drifting && speed > 33f)
		{
			float highSpeedTarget = steerInput * lateralSpeed * 0.94f;
			float highSpeedError = velocityX - highSpeedTarget;
			float highSpeedDamping = Mathf.Lerp(5.8f, 8.6f, speedT) * arcadeGrip;
			velocityX -= highSpeedError * Mathf.Clamp01(highSpeedDamping * dt);
		}
		/* [user request 2026-09-22: five lanes] the road edge moved out with the outer lanes: the push-back starts 0.4 m
		   inside the limit and the stop sits 0.01 m inside it, as they did against ±3.85 */
		float edgeOverlap = Mathf.Max(0f, Mathf.Abs(playerX) - (InfernoLanes.PlayerMaxX - 0.4f));   // original: Mathf.Abs(playerX) - 3.45f
		if (edgeOverlap > 0f)
		{
			velocityX -= Mathf.Sign(playerX) * edgeOverlap * 34f * dt;
		}
		velocityX = Mathf.Clamp(velocityX, -16f, 16f);
		if (steer == 0f && Mathf.Abs(velocityX) < 0.08f)
		{
			velocityX = 0f;
		}
		playerX = InfernoLanes.Clamp(playerX + velocityX * dt);   // original: Mathf.Clamp(playerX + velocityX * dt, PlayerMinX, PlayerMaxX)
		if (playerX <= InfernoLanes.PlayerMinX + 0.01f && velocityX < 0f)   // original: playerX <= -3.84f
		{
			velocityX = 0f;
		}
		if (playerX >= InfernoLanes.PlayerMaxX - 0.01f && velocityX > 0f)   // original: playerX >= 3.84f
		{
			velocityX = 0f;
		}
		player.position = new Vector3(playerX, 0f, 5f);
		impactYaw = Mathf.MoveTowards(impactYaw, 0f, 96f * dt);
		impactRoll = Mathf.MoveTowards(impactRoll, 0f, 82f * dt);
		impactShake = Mathf.MoveTowards(impactShake, 0f, 3.2f * dt);
		tractionLoss = Mathf.MoveTowards(tractionLoss, 0f, 2.35f * dt);
		float intendedVelocityX = steerInput * lateralSpeed * Mathf.Lerp(0.86f, 0.98f, lowSpeedAuthority);
		float slip = velocityX - intendedVelocityX;
		float slipAngle = Mathf.Atan2(slip, Mathf.Max(12f, speed));
		float travelYaw = -Mathf.Atan2(velocityX, Mathf.Max(10f, speed)) * Mathf.Rad2Deg;
		float steerYaw = -steerInput * Mathf.Lerp(2.8f, 4.8f, speedT) * lowSpeedAuthority;
		float slipYaw = Mathf.Clamp(-(slipAngle * Mathf.Rad2Deg) * Mathf.Lerp(0.06f, 0.22f, drift), -2.8f, 2.8f);
		float driftYaw = -drift * steerInput * Mathf.Lerp(4.5f, 8.5f, speedT);
		float targetYaw = Mathf.Clamp((travelYaw + steerYaw + slipYaw + driftYaw) * 0.9f + Mathf.Clamp(impactYaw, -9f, 9f), -22f, 22f);
		float targetRoll = Mathf.Clamp(-velocityX * 0.035f - steerInput * 0.38f - drift * steerInput * 0.24f + Mathf.Clamp(impactRoll, -2.4f, 2.4f), -2.8f, 2.8f);
		float visualBlend = 1f - Mathf.Exp(-Mathf.Lerp(11.5f, 9.5f, drift) * dt);
		yawVisual = Mathf.Lerp(yawVisual, targetYaw, visualBlend);
		rollVisual = Mathf.Lerp(rollVisual, targetRoll, visualBlend);
		UpdatePlayerAirborne(dt);
		carVisual.localRotation = playerAirborne
			? Quaternion.Euler(playerJumpPitch, playerVehicleYawOffset + yawVisual + playerJumpYaw, rollVisual + playerJumpRoll)
			: PlayerVehicleVisualRotation(yawVisual, rollVisual);
		float visualOffsetX = Mathf.Clamp(-slip * 0.0008f - drift * steerInput * 0.006f, -0.012f, 0.012f);
		carVisual.localPosition = new Vector3(visualOffsetX, CurrentPlayerVehicleRoadYOffset() + playerJumpHeight, 0f);
		SetPlayerHoverSuppressedForJump(playerAirborne);
		playerWheelSpin = Mathf.Repeat(playerWheelSpin + speed * dt * playerWheelSpinDegreesPerMeter, 360f);
		SetPlayerWheelSpin(playerWheelSpin);
		EmitVehicleSmoke(dt, boosting, braking, drifting);
		if (drift > 0.58f && speed > 20f)
		{
			AddDailyDriftTime(dt);
			driftChainTimer += dt;
			if (driftChainTimer >= 0.75f)
			{
				driftChainTimer -= 0.75f;
				combo++;
				comboTimer = 1.5f;
				score += 18f * Mathf.Max(1, combo);
				Reward(1.5f);
			}
		}
		else
		{
			driftChainTimer = Mathf.Max(0f, driftChainTimer - dt * 1.8f);
		}
		SetPlayerExhaustFireActive(boosting);
		float travelled = speed * dt;
		distance += travelled;
		AddDailyDistance(travelled);
		score = Mathf.Max(score, Mathf.Floor(distance));
		if (invuln > 0f)
		{
			invuln -= dt;
		}
		if (comboTimer > 0f)
		{
			comboTimer -= dt;
		}
		else
		{
			combo = 0;
		}
		carVisual.gameObject.SetActive(invuln <= 0f || Mathf.FloorToInt(Time.time * 14f) % 2 == 0);
	}

	private void UpdatePlayerAirborne(float dt)
	{
		playerLandedThisFrame = false;
		playerLandingDownSpeed = 0f;
		if (!playerAirborne)
		{
			return;
		}
		if (playerRampRiding)
		{
			playerRampRideElapsed += dt;
			float t = Mathf.Clamp01(playerRampRideElapsed / Mathf.Max(0.01f, playerRampRideDuration));
			float eased = t * t * (3f - 2f * t);
			playerJumpHeight = playerRampRiseHeight * eased;
			float rampAngle = Mathf.Atan2(playerRampRiseHeight, RampLength) * Mathf.Rad2Deg;
			playerJumpPitch = rampAngle * Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t * 1.45f));
			if (t < 1f)
			{
				return;
			}
			playerRampRiding = false;
			playerJumpVelocity = playerRampTakeoffVelocity;
			impactShake = Mathf.Max(impactShake, 0.34f);
			PlayImpactHaptic(0);
		}
		playerJumpVelocity -= 16.2f * dt;
		playerJumpHeight += playerJumpVelocity * dt;
		playerJumpPitchVelocity -= 7.2f * dt;
		playerJumpPitch = Mathf.Clamp(playerJumpPitch + playerJumpPitchVelocity * dt, -16f, 16f);
		playerJumpRoll += playerJumpRollVelocity * dt;
		playerJumpRollVelocity = Mathf.MoveTowards(playerJumpRollVelocity, 0f, 15f * dt);
		playerJumpRoll = Mathf.Clamp(playerJumpRoll, -24f, 24f);
		playerJumpYaw = Mathf.MoveTowards(playerJumpYaw, 0f, 3.6f * dt);
		if (playerJumpHeight > 0f || playerJumpVelocity >= 0f)
		{
			return;
		}
		playerLandedThisFrame = true;
		playerLandingDownSpeed = Mathf.Abs(playerJumpVelocity);
		playerAirborne = false;
		playerRampRiding = false;
		playerJumpHeight = 0f;
		playerJumpVelocity = 0f;
		playerJumpPitch = 0f;
		playerJumpPitchVelocity = 0f;
		playerJumpRoll = 0f;
		playerJumpRollVelocity = 0f;
		playerJumpYaw = 0f;
		impactShake = Mathf.Max(impactShake, 0.48f);
		PlayImpactHaptic(1);
	}

	private float ReadTouchSteering()
	{
		float drag = ReadAccelerationDragSteering();
		if (drag != 0f)
		{
			return drag;
		}
		if (touchSteerLeftPointers.Count == touchSteerRightPointers.Count)
		{
			return 0f;
		}
		return touchSteerLeftPointers.Count > touchSteerRightPointers.Count ? -1f : 1f;
	}

	private float ReadAccelerationDragSteering()
	{
		Touchscreen touchscreen = Touchscreen.current;
		if (touchscreen == null || Screen.width <= 0)
		{
			return 0f;
		}
		float deadZone = Mathf.Max(18f, Screen.width * 0.035f);
		float fullRange = Mathf.Max(deadZone + 1f, Screen.width * 0.2f);
		float strongest = 0f;
		foreach (TouchControl touch in touchscreen.touches)
		{
			if (!touch.press.isPressed)
			{
				continue;
			}
			Vector2 start = touch.startPosition.ReadValue();
			Vector2 position = touch.position.ReadValue();
			if (IsHudActionPoint(start) || IsHudActionPoint(position))
			{
				continue;
			}
			float delta = position.x - start.x;
			if (Mathf.Abs(delta) > Mathf.Abs(strongest))
			{
				strongest = delta;
			}
		}
		float magnitude = Mathf.Abs(strongest);
		if (magnitude <= deadZone)
		{
			return 0f;
		}
		float amount = Mathf.InverseLerp(deadZone, fullRange, magnitude);
		amount = Mathf.SmoothStep(0f, 1f, amount);
		return -Mathf.Sign(strongest) * amount;
	}

	private void ResetGameplayTouches()
	{
		touchSteerLeftPointers.Clear();
		touchSteerRightPointers.Clear();
		touchBoostPointers.Clear();
		brakeButtonPointers.Clear();
		touchBoostHeld = false;
		brakeButtonHeld = false;
		brakeHapticHeld = false;
		SetPlayerBrakeLights(false);
		SetPlayerExhaustFireActive(false);
	}

	private bool IsGameplayAccelerationHeld()
	{
		if (running && !paused)
		{
			return touchBoostHeld;
		}
		return false;
	}

	private void UpdateBrakeHaptic(bool braking)
	{
		if (braking && !brakeHapticHeld && running && !paused)
		{
			PlayImpactHaptic(0);
		}
		brakeHapticHeld = braking;
	}

	private void PlayCollisionHaptic(bool sideImpact)
	{
		if (!vibrationOn || !running || paused || Time.unscaledTime < nextCollisionHapticTime)
		{
			return;
		}
		nextCollisionHapticTime = Time.unscaledTime + 0.28f;
		PlayImpactHaptic(sideImpact ? 1 : 2);
	}

	private void PlayImpactHaptic(int strength)
	{
		if (!vibrationOn)
		{
			return;
		}
		ShadowRun_PlayImpactHaptic(Mathf.Clamp(strength, 0, 2));
	}

	/* [user request 2026-09-22: five lanes] chase-camera framing for the 17 m road (UpdateCamera / ResetCameraImmediate): the
	   shipped camera (height 1.95 → 2.45, distance 10.9 → 12.4, FOV 54.2 + speed) framed the 10.2 m road; it now sits 0.6 m
	   higher and 1.8 m further back with a 6° wider field of view, so the outer lanes (±6.2 m) read beside the car and the
	   whole road is in view a little way ahead. The speed / jump / drift modulation, the smoothing and the look target are
	   the shipped ones. */
	private const float CameraFovBase = 54.2f + 6f;      // original: 54.2f
	private const float CameraHeightRaise = 0.6f;        // added to the shipped 1.95 → 2.45 height
	private const float CameraDistanceBack = 1.8f;       // added to the shipped 10.9 → 12.4 distance behind the car

	private void UpdateCamera(float dt)
	{
		bool brakeHeld = brakeButtonHeld || KeyHeld(274, "down") || KeyHeld(115, "s");
		float speedT = Mathf.Clamp01(speed / 82f);
		float jumpT = Mathf.Clamp01(playerJumpHeight / 5.5f);
		float targetFov = CameraFovBase + speedT * 2.4f + drift * 0.18f - brakePressure * 0.35f + jumpT * 2.6f;   /* [user request 2026-09-22: five lanes] */   // original: 54.2f + speedT * 2.4f + …
		float fovSmoothTime = playerAirborne ? 0.15f : 0.2f;
		cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetFov, ref cameraFovVelocity, fovSmoothTime, 38f, dt);
		float shakeAmount = 0.0025f + drift * 0.0022f + impactShake * 0.032f;
		if (Time.time >= nextShakeSampleTime)
		{
			cachedShakeX = Mathf.PerlinNoise(Time.time * 22f, 0.31f);
			cachedShakeY = Mathf.PerlinNoise(0.73f, Time.time * 22f);
			nextShakeSampleTime = Time.time + 1f / 30f;
		}
		Vector3 shake = new Vector3(0f, (cachedShakeY - 0.5f) * shakeAmount * speedT, 0f);
		Vector3 targetPosition = new Vector3(playerX, Mathf.Lerp(1.95f, 2.45f, speedT) + CameraHeightRaise + playerJumpHeight * 0.78f + jumpT * 0.25f, Mathf.Lerp(10.9f, 12.4f, speedT) + CameraDistanceBack + jumpT * 1.4f) + shake;   /* [user request 2026-09-22: five lanes] */   // original: Mathf.Lerp(1.95f, 2.45f, speedT) + playerJumpHeight * 0.78f + jumpT * 0.25f, Mathf.Lerp(10.9f, 12.4f, speedT) + jumpT * 1.4f
		float positionSmoothTime = playerAirborne ? 0.11f : 0.12f;
		Vector3 position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref cameraVelocity, positionSmoothTime, 92f, dt);
		position.x = playerX;
		cameraVelocity.x = 0f;
		cam.transform.position = position;
		Vector3 targetLook = new Vector3(playerX, 0.74f + playerJumpHeight * 0.68f, Mathf.Lerp(-10.4f, -16.4f, speedT) - jumpT * 0.8f);
		float lookSmoothTime = playerAirborne ? 0.12f : 0.14f;
		Vector3 look = Vector3.SmoothDamp(cameraLookTarget, targetLook, ref cameraLookVelocity, lookSmoothTime, 86f, dt);
		look.x = playerX;
		cameraLookVelocity.x = 0f;
		cameraLookTarget = look;
		Quaternion targetRotation = Quaternion.LookRotation(cameraLookTarget - cam.transform.position, Vector3.up);
		cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, 1f - Mathf.Exp(-8.5f * dt));
	}

	private void ResetCameraImmediate()
	{
		if (cam == null)
		{
			return;
		}
		cameraVelocity = Vector3.zero;
		cameraLookVelocity = Vector3.zero;
		cameraFovVelocity = 0f;
		cameraLookTarget = new Vector3(0f, 0.74f, -10.4f);
		cam.fieldOfView = 54.5f + (CameraFovBase - 54.2f);   /* [user request 2026-09-22: five lanes] */   // original: 54.5f
		cam.transform.position = new Vector3(0f, 2.05f + CameraHeightRaise, 11.1f + CameraDistanceBack);   /* [user request 2026-09-22: five lanes] */   // original: new Vector3(0f, 2.05f, 11.1f)
		cam.transform.rotation = Quaternion.LookRotation(cameraLookTarget - cam.transform.position, Vector3.up);
	}

	private void UpdateHud()
	{
		int scoreMeters = Mathf.FloorToInt(score);
		int distanceMeters = Mathf.FloorToInt(distance);
		bool refreshText = Time.unscaledTime >= nextHudTextRefresh;
		if (refreshText)
		{
			nextHudTextRefresh = Time.unscaledTime + 0.1f;
		}
		if (showingGameOver && overlay != null && overlay.activeSelf && (scoreMeters != lastOverlayScore || best != lastOverlayBest))
		{
			if (gameOverDistanceText != null)
			{
				gameOverDistanceText.text = FormatGameOverDistance(distance);
			}
			if (gameOverBestText != null)
			{
				gameOverBestText.text = FormatGameOverDistance(best);
			}
			lastOverlayScore = scoreMeters;
			lastOverlayBest = best;
		}
		if (hudRoot == null || !hudRoot.activeInHierarchy)
		{
			return;
		}
		if (refreshText && scoreText != null && distanceMeters != lastHudScore)
		{
			scoreText.text = distanceMeters.ToString() + "m";
			scoreText.fontSize = distanceMeters >= 100000 ? 55 : (distanceMeters >= 10000 ? 65 : 77);
			lastHudScore = distanceMeters;
		}
		if (bestText != null && best != lastHudBest)
		{
			bestText.text = "BEST " + best.ToString() + "m";
			lastHudBest = best;
		}
		float speedFillAmount = Mathf.Clamp01((speed - 24f) / 58f);
		if (speedFill != null && speedFillAmount != lastSpeedFillAmount)
		{
			speedFill.fillAmount = speedFillAmount;
			lastSpeedFillAmount = speedFillAmount;
		}
		UpdateSpeedometerHud(refreshText);
		if (lifeHalfUnits != lastHudLifeHalfUnits)
		{
			for (int i = 0; i < lifeDots.Count; i++)
			{
				int halves = Mathf.Clamp(lifeHalfUnits - i * 2, 0, 2);
				lifeDots[i].fillAmount = halves * 0.5f;
			}
			lastHudLifeHalfUnits = lifeHalfUnits;
		}
	}

	private void UpdateSpeedometerHud(bool refreshText)
	{
		int kmh = Mathf.Clamp(Mathf.RoundToInt(speed * 3.6f), 0, 240);
		float needleT = Mathf.Clamp01(kmh / 240f);
		float needleAngle = Mathf.Lerp(135f, -135f, needleT);
		if (needleAngle != lastSpeedometerNeedleAngle)
		{
			Quaternion rotation = Quaternion.Euler(0f, 0f, needleAngle);
			if (speedometerNeedle != null)
			{
				speedometerNeedle.localRotation = rotation;
			}
			if (speedometerNeedleGlow != null)
			{
				speedometerNeedleGlow.localRotation = rotation;
			}
			lastSpeedometerNeedleAngle = needleAngle;
		}
		if (refreshText && speedometerValueText != null && kmh != lastSpeedometerKmh)
		{
			lastSpeedometerKmh = kmh;
			speedometerValueText.text = kmh.ToString("000");
			/* [user request 2026-09-22: numeric speed] the written speed changes colour as the speed rises */
			speedometerValueText.color = SpeedReadoutColor(kmh);
		}
	}

	/* [user request 2026-09-22: numeric speed] */
	// Cool → hot ramp over the dial's old 0..240 km/h range: pearl white at rest, electric violet by
	// 80, magenta by 140, orange by 190, red at the cap — the game's own palette, so the number reads
	// as "how fast" at a glance.
	private static readonly Color[] SpeedReadoutStops =
	{
		new Color(0.88f, 0.84f, 0.94f, 0.98f),   // 0   pearl
		new Color(0.58f, 0.20f, 1.00f, 0.98f),   // 80  electric violet
		new Color(1.00f, 0.25f, 0.75f, 0.98f),   // 140 magenta
		new Color(1.00f, 0.55f, 0.12f, 0.98f),   // 190 orange
		new Color(1.00f, 0.12f, 0.10f, 0.98f),   // 240 red
	};
	private static readonly float[] SpeedReadoutKmh = { 0f, 80f, 140f, 190f, 240f };

	private static Color SpeedReadoutColor(int kmh)
	{
		float v = Mathf.Clamp(kmh, 0f, 240f);
		for (int i = 1; i < SpeedReadoutKmh.Length; i++)
		{
			if (v > SpeedReadoutKmh[i]) continue;
			float t = Mathf.InverseLerp(SpeedReadoutKmh[i - 1], SpeedReadoutKmh[i], v);
			return Color.Lerp(SpeedReadoutStops[i - 1], SpeedReadoutStops[i], t);
		}
		return SpeedReadoutStops[SpeedReadoutStops.Length - 1];
	}

	private void UpdateEvents(float dt)
	{
		UpdateSceneLighting();
		eventTimer -= dt;
		if (eventTimer <= 0f)
		{
			if (demoSanta)
			{
				TriggerSanta();
				demoSanta = false;
			}
			else if (Random.value < 0.01f && !santaActive)
			{
				TriggerSanta();
			}
			eventTimer = Random.Range(1.1f, 2.2f);
		}
		if (santaActive)
		{
			santa.position = santa.position + Vector3.right * 14f * dt;
			santa.position = santa.position + Vector3.up * Mathf.Sin(Time.time * 2f) * dt * 1.4f;
			AnimateSanta();
			if (santa.position.x > 72f)
			{
				santa.gameObject.SetActive(false);
				santaActive = false;
			}
		}
	}

	private void UpdateSceneLighting()
	{
		/* [user request 2026-09-21: maps] every map but Neon Night owns its key light and ambient */
		if (InfernoMaps.OwnsLighting)
		{
			return;
		}
		if (dynamicWeatherReady)
		{
			return;
		}
		Color ambient, keyColor; float keyIntensity;
		ShippedLightPulse(out ambient, out keyColor, out keyIntensity);   /* [user request 2026-09-24: neon night cycle] */ // the same values, computed in one place so InfernoMaps can blend them
		RenderSettings.ambientLight = ambient;
		if (sceneKeyLight != null)
		{
			sceneKeyLight.color = keyColor;
			sceneKeyLight.intensity = keyIntensity;
		}
	}

	/* [user request 2026-09-24: neon night cycle] */
	// The per-frame red pulse of UpdateSceneLighting as values, so InfernoMaps.ApplyCycle can fade Neon Night's
	// night lighting toward its daytime look. The body is UpdateSceneLighting's, unchanged.
	internal void ShippedLightPulse(out Color ambient, out Color keyColor, out float keyIntensity)
	{
		int quality = Mathf.Clamp(graphicsIndex, 0, 2);
		float pulse = Mathf.Clamp01((Mathf.Sin(Time.time * 0.72f) + Mathf.Sin(Time.time * 0.29f + 1.7f) * 0.45f) * 0.5f + 0.5f);
		/* [user request 2026-09-20: brighter cars] */ // This runs every frame while the weather system is not ready (it never is
		// without the Dynamic Weather System package), so the MakeLights / ApplyVisualQuality values are overridden here:
		// the brightening has to happen in this pulse as well. original values in the comments.
		Color ambientLow = quality == 0 ? new Color(0.07f, 0.024f, 0.036f) : new Color(0.1f, 0.036f, 0.052f); // original: (0.04, 0.012, 0.02) / (0.058, 0.018, 0.03)
		Color ambientHigh = quality == 0 ? new Color(0.12f, 0.05f, 0.06f) : new Color(0.18f, 0.08f, 0.095f); // original: (0.07, 0.026, 0.034) / (0.105, 0.045, 0.055)
		ambient = Color.Lerp(ambientLow, ambientHigh, pulse);
		keyColor = Color.Lerp(new Color(0.9f, 0.3f, 0.26f), new Color(1f, 0.46f, 0.38f), pulse); // original: (0.82, 0.18, 0.16) -> (1, 0.36, 0.28)
		float baseIntensity = quality == 0 ? 1.45f : (quality == 1 ? 1.7f : 1.95f); // original: 0.95 / 1.12 / 1.28
		keyIntensity = baseIntensity + pulse * 0.3f; // original: pulse * 0.22
	}

	private void AnimateSanta()
	{
		float wave = Mathf.Sin(Time.time * 12f);
		for (int i = 0; i < santaAnimParts.Count; i++)
		{
			float sign = i % 2 != 0 ? -wave : wave;
			santaAnimParts[i].localRotation = Quaternion.Euler(0f, 0f, sign * 22f);
		}
		if (santaWhip != null)
		{
			float whip = Mathf.Sin(Time.time * 9f);
			santaWhip.SetPosition(0, new Vector3(-1.95f, 1.34f, 0f));
			santaWhip.SetPosition(1, new Vector3(-1.15f, 1.92f + whip * 0.28f, 0f));
			santaWhip.SetPosition(2, new Vector3(-0.25f, 1.62f - whip * 0.32f, 0f));
		}
	}

	private void TriggerSanta()
	{
		santaActive = true;
		santa.gameObject.SetActive(true);
		santa.position = new Vector3(-60f, Random.Range(35f, 46f), Random.Range(-72f, -102f));
		santa.localScale = Vector3.one * Random.Range(1.6f, 2.3f);
	}
}
