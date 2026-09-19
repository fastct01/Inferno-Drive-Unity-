using UnityEngine;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_UpdateHighBeams, _TriggerHighBeamFlash, _ResetHighBeamAbility,
// _UpdateHighBeamAbility, _UpdateHighBeamCooldownVisual, _PlayHighBeamSound, _StartHighBeamPulse, _CompleteHighBeamPulse,
// _UpdateHighBeamTransform, _SendHighBeamReaction, _DisableHighBeam, _SetHighBeamVisual, _ReactToFlashing,
// _TryRequestHighBeamCourtesyGap.
public sealed partial class ShadowRunPrototype
{
	private void UpdateHighBeams(float dt)
	{
		UpdateHighBeamAbility(dt);
		if (highBeamOrigin == null)
		{
			return;
		}
		if (KeyDown(102, "f"))
		{
			TriggerHighBeamFlash();
		}
		if (highBeamRetriggerRemaining > 0f)
		{
			highBeamRetriggerRemaining = Mathf.Max(0f, highBeamRetriggerRemaining - dt);
			SetHighBeamVisual(false);
			if (highBeamRetriggerRemaining > 0f)
			{
				return;
			}
			if (highBeamQueuedPulses > 0)
			{
				highBeamQueuedPulses--;
			}
			StartHighBeamPulse();
		}
		if (highBeamFlashRemaining <= 0f)
		{
			DisableHighBeam();
			return;
		}
		highBeamFlashRemaining = Mathf.Max(0f, highBeamFlashRemaining - dt);
		SetHighBeamVisual(true);
		UpdateHighBeamTransform();
		if (!highBeamFlashReactionSent)
		{
			SendHighBeamReaction();
		}
		if (highBeamFlashRemaining <= 0f)
		{
			CompleteHighBeamPulse();
		}
	}

	private void TriggerHighBeamFlash()
	{
		if (!running || paused)
		{
			return;
		}
		if (highBeamCooldownRemaining > 0f)
		{
			return;
		}
		if (highBeamActiveWindowRemaining <= 0f)
		{
			highBeamActiveWindowRemaining = HighBeamActiveWindowDuration;
		}
		PlayHighBeamSound();
		if (highBeamFlashRemaining > 0f || highBeamRetriggerRemaining > 0f)
		{
			highBeamQueuedPulses = Mathf.Min(8, highBeamQueuedPulses + 1);
			if (highBeamFlashRemaining > 0f)
			{
				highBeamFlashRemaining = 0f;
				highBeamRetriggerRemaining = HighBeamRetriggerDarkGap;
				highBeamFlashReactionSent = false;
				SetHighBeamVisual(false);
			}
			return;
		}
		StartHighBeamPulse();
	}

	private void ResetHighBeamAbility()
	{
		highBeamActiveWindowRemaining = 0f;
		highBeamCooldownRemaining = 0f;
		UpdateHighBeamCooldownVisual();
	}

	private void UpdateHighBeamAbility(float dt)
	{
		if (highBeamActiveWindowRemaining > 0f)
		{
			highBeamActiveWindowRemaining = Mathf.Max(0f, highBeamActiveWindowRemaining - dt);
			if (highBeamActiveWindowRemaining <= 0f)
			{
				highBeamCooldownRemaining = HighBeamCooldownDuration;
				DisableHighBeam();
			}
		}
		else if (highBeamCooldownRemaining > 0f)
		{
			highBeamCooldownRemaining = Mathf.Max(0f, highBeamCooldownRemaining - dt);
		}
		UpdateHighBeamCooldownVisual();
	}

	private void UpdateHighBeamCooldownVisual()
	{
		bool coolingDown = highBeamCooldownRemaining > 0f;
		if (highBeamHudButton != null)
		{
			highBeamHudButton.interactable = !coolingDown;
		}
		if (highBeamCooldownFill == null)
		{
			return;
		}
		highBeamCooldownFill.gameObject.SetActive(coolingDown);
		highBeamCooldownFill.fillAmount = coolingDown ? 1f - Mathf.Clamp01(highBeamCooldownRemaining / HighBeamCooldownDuration) : 0f;
	}

	private void PlayHighBeamSound()
	{
		if (highBeamAudioSource == null)
		{
			GameObject audioObject = new GameObject("High Beam Audio");
			audioObject.transform.SetParent(transform, false);
			highBeamAudioSource = audioObject.AddComponent<AudioSource>();
			highBeamAudioSource.playOnAwake = false;
			highBeamAudioSource.loop = false;
			highBeamAudioSource.spatialBlend = 0f;
			highBeamAudioSource.ignoreListenerPause = true;
		}
		if (highBeamAudioClip == null)
		{
			highBeamAudioClip = Resources.Load<AudioClip>("звук бима");
		}
		if (highBeamAudioClip == null)
		{
			return;
		}
		highBeamAudioSource.Stop();
		highBeamAudioSource.clip = highBeamAudioClip;
		highBeamAudioSource.volume = Mathf.Clamp01(sfxVolume / 100f) * 0.6f;
		highBeamAudioSource.Play();
	}

	private void StartHighBeamPulse()
	{
		highBeamRetriggerRemaining = 0f;
		highBeamFlashRemaining = HighBeamFlashDuration;
		highBeamFlashReactionSent = false;
		SetHighBeamVisual(true);
		UpdateHighBeamTransform();
		SendHighBeamReaction();
	}

	private void CompleteHighBeamPulse()
	{
		if (highBeamQueuedPulses <= 0)
		{
			DisableHighBeam();
			return;
		}
		highBeamFlashRemaining = 0f;
		highBeamRetriggerRemaining = HighBeamRetriggerDarkGap;
		highBeamFlashReactionSent = false;
		SetHighBeamVisual(false);
	}

	private void UpdateHighBeamTransform()
	{
		Vector3 origin = Quaternion.Euler(0f, yawVisual, 0f) * new Vector3(0f, CurrentHighBeamOriginHeight(), -VehicleHalfLength + CurrentHighBeamFrontInset());
		if (carVisual != null)
		{
			origin += carVisual.localPosition;
		}
		highBeamOrigin.localPosition = origin;
		highBeamOrigin.localRotation = Quaternion.Euler(0f, 180f + yawVisual, 0f);
	}

	private void SendHighBeamReaction()
	{
		if (highBeamFlashReactionSent || highBeamOrigin == null)
		{
			return;
		}
		highBeamFlashReactionSent = true;
		int playerLane = ClosestLane(playerX);
		Transform nearest = null;
		float nearestDistance = float.PositiveInfinity;
		for (int i = 0; i < pickups.Count; i++)
		{
			Pickup item = pickups[i];
			if (item.Type != PickupType.Traffic || item.Transform == null || !item.Transform.gameObject.activeInHierarchy)
			{
				continue;
			}
			Vector3 position = item.Transform.position;
			float distanceAhead = player.position.z - position.z;
			if (distanceAhead <= 0f)
			{
				continue;
			}
			if (distanceAhead > InfernoDrivingRules.HighBeamRange) /* [InfernoLobby patch] */
			{
				continue;
			}
			if (ClosestLane(position.x) != playerLane || Mathf.Abs(position.x - lanes[playerLane]) > 0.82f)
			{
				continue;
			}
			if (distanceAhead >= nearestDistance)
			{
				continue;
			}
			nearestDistance = distanceAhead;
			nearest = item.Transform;
		}
		if (nearest != null)
		{
			ReactToFlashing(nearest);
		}
	}

	private void DisableHighBeam()
	{
		highBeamFlashRemaining = 0f;
		highBeamRetriggerRemaining = 0f;
		highBeamQueuedPulses = 0;
		highBeamFlashReactionSent = false;
		SetHighBeamVisual(false);
	}

	private void SetHighBeamVisual(bool visible)
	{
		if (highBeamLeftPrefabInstance != null)
		{
			highBeamLeftPrefabInstance.SetActive(visible);
		}
		if (highBeamRightPrefabInstance != null)
		{
			highBeamRightPrefabInstance.SetActive(visible);
		}
	}

	private void ReactToFlashing(Transform trafficRoot)
	{
		if (trafficRoot == null)
		{
			return;
		}
		for (int i = 0; i < pickups.Count; i++)
		{
			Pickup item = pickups[i];
			if (item.Transform == null)
			{
				continue;
			}
			if (trafficRoot != item.Transform && !trafficRoot.IsChildOf(item.Transform))
			{
				continue;
			}
			if (Random.value >= 0.7f)
			{
				return;
			}
			float now = Time.time;
			bool repeatedFlash = now - item.HighBeamLastFlashTime <= 1.15f;
			item.HighBeamPressure = repeatedFlash ? Mathf.Min(5f, item.HighBeamPressure + 1f) : 1f;
			item.HighBeamLastFlashTime = now;
			if (item.HighBeamEvading)
			{
				float leadTimer = item.HighBeamPressure >= 3f ? 0.1f : 0.32f;
				if (item.PendingLane >= 0)
				{
					item.TurnSignalLeadTimer = Mathf.Min(item.TurnSignalLeadTimer, leadTimer);
				}
				item.HighBeamManeuverTimer = Mathf.Max(item.HighBeamManeuverTimer, 2.4f);
				pickups[i] = item;
				return;
			}
			int currentLane = ClosestLane(item.Transform.position.x);
			int intendedLane = item.PendingLane >= 0 ? item.PendingLane : ClosestLane(item.TargetX);
			int playerLane = ClosestLane(playerX);
			float distanceAhead = player.position.z - item.Transform.position.z;
			bool abortCutIn = intendedLane == playerLane && intendedLane != currentLane && distanceAhead > 2f && distanceAhead < 82f;
			if (abortCutIn)
			{
				item.PendingLane = -1;
				item.Lane = currentLane;
				item.TargetX = lanes[currentLane];
				item.WantsLaneChange = false;
				item.TurnSignalDirection = 0;
				item.LaneChangeCooldown = Mathf.Max(item.LaneChangeCooldown, 2.2f);
				item.HighBeamCooldown = 0.3f;
				item.BrakeLightHold = Mathf.Max(item.BrakeLightHold, 0.32f);
				item.HighBeamPressure = 0f;
				item.HighBeamRollingYield = false;
				combo++;
				comboTimer = 1.5f;
				score += 45f * Mathf.Max(1, combo);
				Reward(2.5f);
				pickups[i] = item;
				return;
			}
			int yieldLane = FindSafeHighBeamYieldLane(item, currentLane);
			if (yieldLane == currentLane)
			{
				item.HighBeamCooldown = 0.35f;
				item.BrakeLightHold = Mathf.Max(item.BrakeLightHold, 0.24f);
				pickups[i] = item;
				return;
			}
			float yieldX = lanes[yieldLane];
			item.HighBeamEvading = true;
			item.HighBeamManeuverTimer = 14f;
			item.HighBeamCooldown = 4.2f;
			item.HighBeamTargetX = yieldX;
			item.HighBeamRollingYield = highBeamCourtesyNext;
			highBeamCourtesyNext = !highBeamCourtesyNext;
			if (item.HighBeamRollingYield)
			{
				TryRequestHighBeamCourtesyGap(item, yieldLane);
			}
			if (IsHighBeamYieldLaneSafe(yieldLane, item.Transform.position.z, item.Transform, item))
			{
				QueueTrafficLaneChange(ref item, yieldLane, item.Transform.position.x);
				item.TurnSignalLeadTimer = 0.12f;
			}
			else
			{
				item.TargetX = lanes[currentLane];
				item.Lane = currentLane;
				item.PendingLane = -1;
			}
			item.LaneChangeCooldown = Mathf.Max(item.LaneChangeCooldown, item.HighBeamManeuverTimer + 1.2f);
			combo++;
			comboTimer = 1.7f;
			score += 80f * Mathf.Max(1, combo);
			Reward(4f);
			pickups[i] = item;
			return;
		}
	}

	private bool TryRequestHighBeamCourtesyGap(Pickup yieldingCar, int targetLane)
	{
		if (yieldingCar.Transform == null)
		{
			return false;
		}
		int bestIndex = -1;
		float bestDistance = float.PositiveInfinity;
		float yieldingZ = yieldingCar.Transform.position.z;
		for (int i = 0; i < pickups.Count; i++)
		{
			Pickup other = pickups[i];
			if (other.Transform == null || other.Transform == yieldingCar.Transform || other.PendingLane >= 0 || other.HighBeamEvading || other.CourtesyYieldTimer > 0f)
			{
				continue;
			}
			if (!IsTrafficOccupyingLane(other, targetLane) || Mathf.Abs(other.Transform.position.x - lanes[targetLane]) > 0.72f)
			{
				continue;
			}
			float offset = other.Transform.position.z - yieldingZ;
			float distance = Mathf.Abs(offset);
			if (offset < -18f || offset > 50f || distance >= bestDistance)
			{
				continue;
			}
			if (player.position.z - other.Transform.position.z < 9f)
			{
				continue;
			}
			bestDistance = distance;
			bestIndex = i;
		}
		if (bestIndex < 0)
		{
			return false;
		}
		Pickup courteous = pickups[bestIndex];
		courteous.CourtesyYieldTimer = Random.Range(3.1f, 4.2f);
		courteous.CourtesyYieldSpeed = Mathf.Max(10f, courteous.Speed * Random.Range(0.62f, 0.74f));
		courteous.BrakeLightHold = Mathf.Max(courteous.BrakeLightHold, courteous.CourtesyYieldTimer);
		courteous.LaneChangeCooldown = Mathf.Max(courteous.LaneChangeCooldown, courteous.CourtesyYieldTimer + 1.1f);
		pickups[bestIndex] = courteous;
		return true;
	}
}
