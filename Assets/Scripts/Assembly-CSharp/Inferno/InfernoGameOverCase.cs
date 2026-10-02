using UnityEngine;
using UnityEngine.UI;

/* [user request 2026-09-29: loss case box] "add the case box for the user to open when the user losts":
   the loss screen's flat "OPEN CASE" plate becomes a real case box - the lobby "Cases" card
   (InfernoLobbyStyle.CaseCard, 356x483) rebuilt at 280x380 (scale 380/483 = 0.7867) with the same live
   990x630 crate render, the same daily-allowance line and the same outlined OPEN CASE key. The whole box
   is one Button -> InfernoLobby.Click index 5 -> ShowCaseOverlay, so CaseVisibility hides this screen
   while the case sheet is up and brings the saved result back when it closes.
   The box keeps the plate's GameObject name ("OPEN CASE Profile Button"): it is still this screen's
   open-case action, and that name is the by-name contract used by Assets/Tests/PlayMode/SmokeTests.cs and
   by .claude/skills/inferno-screens/SKILL.md. It uses its own statics throughout, because
   InfernoLobby.s_caseArt / s_caseArtBound and InfernoDailyCases.s_cardSubtitle are single slots owned by
   the lobby card. */
public sealed partial class ShadowRunPrototype
{
	internal static partial class InfernoLobby
	{
		/* Loss-screen canvas px: the "Game Over Screen" root is 2400x2400 with a centred anchor, so every
		   number here is absolute canvas px from the screen centre, y up. The box fills the band the flat
		   plate leaves free - the stats card's bottom edge at y 100 down to the TRY AGAIN glow at y -294 -
		   as x -140..140, y -287..93: 7 px of air at each end, 18 px to the TRY AGAIN plate itself, so
		   nothing else on the screen moves. */
		private const float CaseBoxW = 280f, CaseBoxH = 380f, CaseBoxY = -97f;
		// 214.2 = 0.85 * 252 and 181 / 252 = 0.718: the lobby card's two crop ratios, so the crate body is
		// framed exactly as it is on the card.
		private const float CaseBoxArtW = 252f, CaseBoxArtH = 181f, CaseBoxArtBody = 214.2f;

		private static RawImage s_caseBoxArt;
		private static bool s_caseBoxArtBound;
		private static Text s_caseBoxAllowance;

		// Built by BuildGameOver in place of the old flat "OPEN CASE" plate.
		public static void BuildCaseBox(ShadowRunPrototype game, Transform parent)
		{
			Image box = SteelBox(game, parent, "OPEN CASE Profile Button", 0f, CaseBoxY, CaseBoxW, CaseBoxH, 14, 0, 14, 0);
			// SteelBox -> Block -> Place anchors (0.5, 1) = the PARENT'S TOP-centre, and this root is
			// 2400x2400 centred, so re-place with the centre anchor, as Shade and TitleRule do above.
			Place(box.rectTransform, 0f, CaseBoxY, CaseBoxW, CaseBoxH, new Vector2(0.5f, 0.5f));
			RectTransform boxRoot = box.rectTransform;
			Connect(game, ButtonOn(box), 5);   // the whole box is the button -> ShowCaseOverlay, like the lobby card
			Label(game, boxRoot, "CaseBoxCorner", "///", -96f, -22f, 60f, 22f, 16, MockMark, 3);   // 16 px, not the scaled 12.6: Label's best-fit floor is Max(16, size - 6)
			RectTransform artRect = Root(boxRoot, "CaseBoxArt");
			Place(artRect, 0f, -147f, CaseBoxArtW, CaseBoxArtH);
			s_caseBoxArt = artRect.gameObject.AddComponent<RawImage>();
			s_caseBoxArtBound = false;
			s_caseBoxArt.raycastTarget = false;
			// A RawImage with no texture draws a white quad: stay transparent (and invisible to the layout
			// audit) until CaseBoxShown binds the live render.
			s_caseBoxArt.color = new Color(1f, 1f, 1f, 0f);
			CaseArtCrop(s_caseBoxArt, CaseBoxArtW, CaseBoxArtH, CaseBoxArtBody);
			Text caseName = Label(game, boxRoot, "CaseBoxName", "N SERIES CASE", 0f, -275f, CaseBoxArtW, 28f, 21, 0xFFFFFF);
			caseName.fontStyle = FontStyle.BoldAndItalic;
			s_caseBoxAllowance = Tracked(SpacedLabel(game, boxRoot, "CaseBoxAllowance", "", 0f, -298f, CaseBoxArtW, 17f, 11, MockGreyLilac), 2.75f);
			Image key = SteelBox(game, boxRoot, "CaseBoxKey", 0f, -341f, 244f, 47f, 0, 0, 0, 0, Look.Outline);   // decorative: the whole box is the button
			SpacedLabel(game, key.transform, "Label", "OPEN CASE", 0f, -23.5f, 228f, 31f, 17, 0xFFFFFF);
			RefreshCaseBox();
			/* The box's own show hook - the root's UiScreenLink belongs to the gem counter. A child's
			   OnEnable fires with the root, so this runs every time the loss screen appears, including the
			   return from the case sheet, where HideCaseOverlay has just switched the preview camera off.
			   onHidden stays null on purpose: CaseVisibility disables this root in order to SHOW the case
			   sheet, and the sheet renders the same camera. */
			UiScreenLink link = box.gameObject.AddComponent<UiScreenLink>();
			link.onShown = () => CaseBoxShown(game);
		}

		/* The card's wording while cases remain, the reset countdown once the allowance is used up. Called
		   once a second from InfernoDailyCases.RefreshCard (UpdateDailyGoalsTimer keeps ticking on the loss
		   screen, because GameOver leaves the menu overlay active), on every show, and when the case sheet
		   closes. Shorter than the card's "ALL OPENED  -  NEXT IN ..." on purpose: that is 31 tracked
		   characters, about 260 px of ink in a 252 px column. */
		public static void RefreshCaseBox()
		{
			if (s_caseBoxAllowance == null) return;
			int left = InfernoDailyCases.Remaining();
			s_caseBoxAllowance.text = left > 0
				? left + " OF " + InfernoDailyCases.PerDay + " LEFT TODAY"
				: "NEXT IN " + InfernoDailyCases.Countdown();
		}

		private static void CaseBoxShown(ShadowRunPrototype game)
		{
			// GameOver sets showingGameOver before it activates the root, so this skips the one OnEnable that
			// fires while MakeUI is still building the screen.
			if (game == null || !game.showingGameOver) return;
			RefreshCaseBox();
			// The RETURN VALUE, not the texture: EnsureCasePreviewWorld can create the render texture and
			// still fail on the missing crate models, and then there is nothing to show.
			if (!game.EnsureCasePreviewWorld()) return;
			if (!s_caseBoxArtBound && s_caseBoxArt != null && game.casePreviewTexture != null)
			{
				s_caseBoxArt.texture = game.casePreviewTexture;
				s_caseBoxArt.color = Color.white;
				s_caseBoxArtBound = true;
			}
			// A finished opening leaves the lid at 76 degrees and HideCaseOverlay's ResetCaseOpening(false)
			// skips the pose reset, so without this the box would show an open, empty crate. The yaw dragged
			// on the case screen goes back to 0 with it.
			game.ResetCasePreviewPose();
			// HideCaseOverlay switches the camera off two lines before it re-shows this root; from the next
			// frame InfernoLobby.Layout's HomePreview keeps it on.
			game.SetCasePreviewVisible(true);
		}
	}
}
