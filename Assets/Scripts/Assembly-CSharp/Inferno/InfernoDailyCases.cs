using System;
using UnityEngine;
using UnityEngine.UI;

// [user request 2026-09-21] A fixed number of case openings per day. The day is the local
// date, the same key the daily goals use, so both reset together at midnight. The lobby
// card keeps showing the case and how many openings are left; the case screen disables
// the open / again buttons and shows the reset countdown once the allowance is used up.
// State lives in PlayerPrefs (like the map unlocks): InfernoDrive.DailyCases.Day (date key)
// and InfernoDrive.DailyCases.Opened (count for that day).
public sealed partial class ShadowRunPrototype
{
	internal static class InfernoDailyCases
	{
		public const int PerDay = 3;
		private const string DayPref = "InfernoDrive.DailyCases.Day";
		private const string OpenedPref = "InfernoDrive.DailyCases.Opened";

		private static Text s_cardSubtitle;   // lobby case card: "OPEN CASE" line
		private static Text s_overlayLine;    // case screen: "N OF 3 CASES LEFT TODAY" under the drop table

		public static int DateKey()
		{
			DateTime now = DateTime.Now;
			return now.Year * 10000 + now.Month * 100 + now.Day;
		}

		public static int Opened()
		{
			if (PlayerPrefs.GetInt(DayPref, 0) != DateKey()) return 0;   // a new day: nothing opened yet
			return Mathf.Clamp(PlayerPrefs.GetInt(OpenedPref, 0), 0, PerDay);
		}

		public static int Remaining() { return PerDay - Opened(); }
		public static bool CanOpen() { return Remaining() > 0; }

		// Called when a case actually starts opening (after the rewarded ad, also for
		// "again"). Returns false and changes nothing when today's allowance is used up.
		public static bool Consume()
		{
			int opened = Opened();
			if (opened >= PerDay) return false;
			PlayerPrefs.SetInt(DayPref, DateKey());
			PlayerPrefs.SetInt(OpenedPref, opened + 1);
			PlayerPrefs.Save();
			return true;
		}

		public static string Countdown()
		{
			DateTime now = DateTime.Now;
			TimeSpan left = now.Date.AddDays(1.0) - now;
			return string.Format("{0:00}:{1:00}:{2:00}", (int)left.TotalHours, left.Minutes, left.Seconds);
		}

		// ---- lobby card -------------------------------------------------------------
		public static void BindCard(Text subtitle)
		{
			s_cardSubtitle = subtitle;
			RefreshCard();
		}

		// Once a second from UpdateDailyGoalsTimer and whenever the case screen closes.
		public static void RefreshCard()
		{
			/* [user request 2026-09-29: loss case box] the same tick feeds the loss screen's case box, which
			   keeps its own Text because this class stores exactly ONE s_cardSubtitle (binding the box here
			   would freeze the lobby card's caption at ""). Before the guard below, so the box still ticks if
			   the lobby card was never built. */
			InfernoLobby.RefreshCaseBox();
			if (s_cardSubtitle == null) return;
			int left = Remaining();
			// one line under the card title; the OPEN CASE outline button sits below it
			s_cardSubtitle.text = left > 0
				? left + " OF " + PerDay + " LEFT TODAY"
				: "ALL OPENED  •  NEXT IN " + Countdown();
		}

		// ---- case screen ------------------------------------------------------------
		public static void BindOverlay(ShadowRunPrototype game)
		{
			if (game.caseClosedRoot == null) return;
			s_overlayLine = game.MakeText(game.caseClosedRoot, "", 18, TextAnchor.UpperCenter, new Vector2(0f, -286f));
			s_overlayLine.fontStyle = FontStyle.Bold;
			s_overlayLine.color = new Color(0.75f, 0.4f, 1f, 1f);
			s_overlayLine.raycastTarget = false;
			s_overlayLine.GetComponent<RectTransform>().sizeDelta = new Vector2(640f, 30f);
			RefreshOverlay(game);
		}

		// Called from ShowCaseOverlay (after ResetCaseOpening re-enabled everything), once a
		// second while the screen is up, and after each opening starts.
		public static void RefreshOverlay(ShadowRunPrototype game)
		{
			int left = Remaining();
			if (s_overlayLine != null)
				s_overlayLine.text = left > 0
					? left + " OF " + PerDay + " CASES LEFT TODAY"
					: "NO CASES LEFT TODAY  •  NEXT IN " + Countdown();
			if (left > 0)
			{
				SetDots(game, true);   // the OPENING... dots come back once cases are available again
				return;
			}
			// Allowance used up: the open button becomes a disabled notice, "again" too.
			if (game.caseOpenButton != null) game.caseOpenButton.interactable = false;
			if (game.caseOpenButtonLabel != null) game.caseOpenButtonLabel.text = "NO CASES LEFT TODAY";
			if (game.caseSpinAgainButton != null) game.caseSpinAgainButton.interactable = false;
			if (game.caseOpeningStatusRoot != null && !game.caseSpinActive && !game.caseOpenAnimationActive && !game.caseRewardReady)
			{
				// the pill replaces the button (ResetCaseOpening brings the button back next time)
				if (game.caseOpenButton != null) game.caseOpenButton.gameObject.SetActive(false);
				if (game.caseOpenButtonGlow != null) game.caseOpenButtonGlow.SetActive(false);
				game.caseOpeningStatusRoot.gameObject.SetActive(true);
				if (game.caseStatusLabel != null)
				{
					game.caseStatusLabel.text = "NEXT CASE IN " + Countdown();
					game.caseStatusLabel.color = new Color(0.86f, 0.62f, 1f, 0.94f);
				}
				SetDots(game, false);
			}
		}

		private static void SetDots(ShadowRunPrototype game, bool on)
		{
			if (game.caseOpeningDots == null) return;
			for (int i = 0; i < game.caseOpeningDots.Length; i++)
				if (game.caseOpeningDots[i] != null) game.caseOpeningDots[i].gameObject.SetActive(on);
		}
	}
}
