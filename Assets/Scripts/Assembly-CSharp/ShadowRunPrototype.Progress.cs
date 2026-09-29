using System;
using System.Globalization;
using UnityEngine;
using ShadowRun.Services;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_LoadSettings, _SaveSettings, _ApplySettings,
// _InitializeCloudProfile, _ApplyCloudProfile, _EnsureDailyGoalProgressLoaded, _SaveDailyGoalProgress,
// _AddDailyDistance, _AddDailyDriftTime, _RegisterDailyBoostState, _ResetDailyGoalRunState, _RegisterDailyRunCompleted,
// _RegisterDailyCarPassed, _RegisterDailyCaseOpened, _GetDailyGoalValue, _FormatDailyGoalProgress, _DailyGoalFill,
// _IsDailyGoalComplete, _IsDailyGoalClaimed, _ClaimDailyGoal, _ClaimVisibleDailyGoal, _CurrentDailyGoalsDateKey.
public sealed partial class ShadowRunPrototype
{
	private void LoadSettings()
	{
		musicVolume = PlayerPrefs.GetInt("shadowRunMusic", 70);
		sfxVolume = PlayerPrefs.GetInt("shadowRunSfx", 60);
		gameSoundEnabled = PlayerPrefs.GetInt("shadowRunSoundEnabled", 1) == 1;
		graphicsIndex = PlayerPrefs.GetInt("shadowRunGraphics", 1);
		frameRateIndex = PlayerPrefs.GetInt("shadowRunFrameRate", 1);
		languageIndex = PlayerPrefs.GetInt("shadowRunLanguage", 0);
		vibrationOn = PlayerPrefs.GetInt("shadowRunVibration", 1) == 1;
		darkModeOn = PlayerPrefs.GetInt("shadowRunDarkMode", 1) == 1;
		tutorialHints = PlayerPrefs.GetInt("shadowRunTutorial", 0) == 1;
	}

	private void SaveSettings()
	{
		PlayerPrefs.SetInt("shadowRunMusic", musicVolume);
		PlayerPrefs.SetInt("shadowRunSfx", sfxVolume);
		PlayerPrefs.SetInt("shadowRunSoundEnabled", gameSoundEnabled ? 1 : 0);
		PlayerPrefs.SetInt("shadowRunGraphics", graphicsIndex);
		PlayerPrefs.SetInt("shadowRunFrameRate", frameRateIndex);
		PlayerPrefs.SetInt("shadowRunLanguage", languageIndex);
		PlayerPrefs.SetInt("shadowRunVibration", vibrationOn ? 1 : 0);
		PlayerPrefs.SetInt("shadowRunDarkMode", darkModeOn ? 1 : 0);
		PlayerPrefs.SetInt("shadowRunTutorial", tutorialHints ? 1 : 0);
		PlayerPrefs.Save();
	}

	private void ApplySettings()
	{
		QualitySettings.SetQualityLevel(Mathf.Clamp(graphicsIndex, 0, QualitySettings.names.Length - 1), true);
		ApplyVisualQuality();
		ReapplyDynamicWeatherImmediate();
	}

	private void InitializeCloudProfile()
	{
		SupabaseGameManager.ProfileChanged += ApplyCloudProfile;
		SupabaseGameManager.AuthenticationStatusChanged += SetProfileAuthenticationStatus;
		SupabaseGameManager.AuthenticationCompleted += RefreshProfileUi;
		ApplyCloudProfile();
	}

	private void ApplyCloudProfile()
	{
		menuGemCount = SupabaseGameManager.Crystals;
		if (gemAmountText != null)
		{
			gemAmountText.text = InfernoLobby.GemText(menuGemCount);   /* [user request 2026-09-26: crystals] */
		}
		ApplyGarageOwnershipFromProfile();
		RefreshProfileUi();
	}

	private void EnsureDailyGoalProgressLoaded()
	{
		int today = CurrentDailyGoalsDateKey();
		if (dailyGoalProgressLoaded && dailyGoalProgressDateKey == today)
		{
			return;
		}
		int storedDate = PlayerPrefs.GetInt(DailyGoalDateKeyPref, -1);
		dailyGoalProgressDateKey = today;
		dailyGoalProgressLoaded = true;
		if (storedDate != today)
		{
			dailyGoalBoostActive = false;
			dailyGoalDistanceMeters = 0f;
			dailyGoalRuns = 0;
			dailyGoalBoosts = 0;
			dailyGoalDriftSeconds = 0f;
			dailyGoalCarsPassed = 0;
			dailyGoalCasesOpened = 0;
			dailyGoalClaimedMask = 0;
			dailyGoalProgressDirty = true;
			SaveDailyGoalProgress();
			return;
		}
		dailyGoalDistanceMeters = Mathf.Max(0f, PlayerPrefs.GetFloat(DailyGoalDistancePref, 0f));
		dailyGoalRuns = Mathf.Max(0, PlayerPrefs.GetInt(DailyGoalRunsPref, 0));
		dailyGoalBoosts = Mathf.Max(0, PlayerPrefs.GetInt(DailyGoalBoostsPref, 0));
		dailyGoalDriftSeconds = Mathf.Max(0f, PlayerPrefs.GetFloat(DailyGoalDriftPref, 0f));
		dailyGoalCarsPassed = Mathf.Max(0, PlayerPrefs.GetInt(DailyGoalCarsPref, 0));
		dailyGoalCasesOpened = Mathf.Max(0, PlayerPrefs.GetInt(DailyGoalCasesPref, 0));
		dailyGoalClaimedMask = Mathf.Max(0, PlayerPrefs.GetInt(DailyGoalClaimedMaskPref, 0));
	}

	private void SaveDailyGoalProgress()
	{
		if (dailyGoalProgressLoaded && dailyGoalProgressDirty)
		{
			PlayerPrefs.SetInt(DailyGoalDateKeyPref, dailyGoalProgressDateKey);
			PlayerPrefs.SetFloat(DailyGoalDistancePref, dailyGoalDistanceMeters);
			PlayerPrefs.SetInt(DailyGoalRunsPref, dailyGoalRuns);
			PlayerPrefs.SetInt(DailyGoalBoostsPref, dailyGoalBoosts);
			PlayerPrefs.SetFloat(DailyGoalDriftPref, dailyGoalDriftSeconds);
			PlayerPrefs.SetInt(DailyGoalCarsPref, dailyGoalCarsPassed);
			PlayerPrefs.SetInt(DailyGoalCasesPref, dailyGoalCasesOpened);
			PlayerPrefs.SetInt(DailyGoalClaimedMaskPref, dailyGoalClaimedMask);
		}
		PlayerPrefs.Save();
		dailyGoalProgressDirty = false;
	}

	private void AddDailyDistance(float meters)
	{
		if (meters <= 0f)
		{
			return;
		}
		EnsureDailyGoalProgressLoaded();
		float total = Mathf.Min(10000f, dailyGoalDistanceMeters + meters);
		if (Mathf.Approximately(total, dailyGoalDistanceMeters))
		{
			return;
		}
		dailyGoalDistanceMeters = total;
		dailyGoalProgressDirty = true;
	}

	private void AddDailyDriftTime(float seconds)
	{
		if (seconds <= 0f)
		{
			return;
		}
		EnsureDailyGoalProgressLoaded();
		float total = Mathf.Min(30f, dailyGoalDriftSeconds + seconds);
		if (Mathf.Approximately(total, dailyGoalDriftSeconds))
		{
			return;
		}
		dailyGoalDriftSeconds = total;
		dailyGoalProgressDirty = true;
	}

	private void RegisterDailyBoostState(bool active)
	{
		EnsureDailyGoalProgressLoaded();
		if (active && !dailyGoalBoostActive && dailyGoalBoosts < 10)
		{
			dailyGoalBoosts++;
			dailyGoalProgressDirty = true;
		}
		dailyGoalBoostActive = active;
	}

	private void ResetDailyGoalRunState()
	{
		dailyGoalBoostActive = false;
	}

	private void RegisterDailyRunCompleted()
	{
		EnsureDailyGoalProgressLoaded();
		if (dailyGoalRuns < 3)
		{
			dailyGoalRuns++;
			dailyGoalProgressDirty = true;
		}
		SaveDailyGoalProgress();
	}

	private void RegisterDailyCarPassed()
	{
		EnsureDailyGoalProgressLoaded();
		if (dailyGoalCarsPassed >= 25)
		{
			return;
		}
		dailyGoalCarsPassed++;
		dailyGoalProgressDirty = true;
	}

	private void RegisterDailyCaseOpened()
	{
		EnsureDailyGoalProgressLoaded();
		if (dailyGoalCasesOpened < 1)
		{
			dailyGoalCasesOpened++;
			dailyGoalProgressDirty = true;
		}
		SaveDailyGoalProgress();
	}

	private float GetDailyGoalValue(DailyGoalKind kind)
	{
		EnsureDailyGoalProgressLoaded();
		switch (kind)
		{
			case DailyGoalKind.Distance: return dailyGoalDistanceMeters;
			case DailyGoalKind.Runs: return dailyGoalRuns;
			case DailyGoalKind.Boosts: return dailyGoalBoosts;
			case DailyGoalKind.Drift: return dailyGoalDriftSeconds;
			case DailyGoalKind.CarsPassed: return dailyGoalCarsPassed;
			case DailyGoalKind.CasesOpened: return dailyGoalCasesOpened;
		}
		return 0f;
	}

	private string FormatDailyGoalProgress(DailyGoalKind kind)
	{
		float value = GetDailyGoalValue(kind);
		switch (kind)
		{
			case DailyGoalKind.Distance: return (value / 1000f).ToString("0.#", CultureInfo.InvariantCulture) + "/10 km driven";
			case DailyGoalKind.Runs: return Mathf.FloorToInt(value).ToString() + "/3 runs completed";
			case DailyGoalKind.Boosts: return Mathf.FloorToInt(value).ToString() + "/10 boosts used";
			case DailyGoalKind.Drift: return Mathf.FloorToInt(value).ToString() + "/30 sec drifting";
			case DailyGoalKind.CarsPassed: return Mathf.FloorToInt(value).ToString() + "/25 cars passed";
			case DailyGoalKind.CasesOpened: return Mathf.FloorToInt(value).ToString() + "/1 case opened";
		}
		return "";
	}

	private float DailyGoalFill(DailyGoalKind kind)
	{
		float value = GetDailyGoalValue(kind);
		switch (kind)
		{
			case DailyGoalKind.Distance: return Mathf.Clamp01(value / 10000f);
			case DailyGoalKind.Runs: return Mathf.Clamp01(value / 3f);
			case DailyGoalKind.Boosts: return Mathf.Clamp01(value / 10f);
			case DailyGoalKind.Drift: return Mathf.Clamp01(value / 30f);
			case DailyGoalKind.CarsPassed: return Mathf.Clamp01(value / 25f);
			case DailyGoalKind.CasesOpened: return Mathf.Clamp01(value);
		}
		return 0f;
	}

	private bool IsDailyGoalComplete(DailyGoalKind kind)
	{
		return DailyGoalFill(kind) >= 1f;
	}

	private bool IsDailyGoalClaimed(DailyGoalKind kind)
	{
		EnsureDailyGoalProgressLoaded();
		return (dailyGoalClaimedMask & (1 << ((int)kind & 31))) > 0;
	}

	private void ClaimDailyGoal(DailyGoalKind kind, GameObject collectButton)
	{
		if (!IsDailyGoalComplete(kind) || IsDailyGoalClaimed(kind))
		{
			return;
		}
		dailyGoalClaimedMask |= 1 << ((int)kind & 31);
		PlayerPrefs.SetInt(DailyGoalDateKeyPref, dailyGoalProgressDateKey);
		PlayerPrefs.SetInt(DailyGoalClaimedMaskPref, dailyGoalClaimedMask);
		PlayerPrefs.Save();
		// original: menuGemCount = Mathf.Max(0, SupabaseGameManager.Crystals) + 1
		menuGemCount = Mathf.Max(0, SupabaseGameManager.Crystals) + InfernoLobby.GoalReward((int)kind); /* [InfernoLobby patch] */
		SupabaseGameManager.SetCrystals(menuGemCount);
		SupabaseGameManager.SyncProfileAfterEconomyChangeAsync();
		if (gemAmountText != null)
		{
			gemAmountText.text = InfernoLobby.GemText(menuGemCount);   /* [user request 2026-09-26: crystals] */
		}
		if (collectButton != null)
		{
			collectButton.SetActive(false);
		}
		UpdateDailyGoalsTimer(true);
	}

	private void ClaimVisibleDailyGoal(int index, GameObject collectButton)
	{
		HomeGoalData[] goals = PickDailyGoals(3);
		if (index < 0 || index >= goals.Length)
		{
			return;
		}
		ClaimDailyGoal(goals[index].Kind, collectButton);
	}

	private int CurrentDailyGoalsDateKey()
	{
		DateTime now = DateTime.Now;
		return now.Year * 10000 + now.Month * 100 + now.Day;
	}
}
