using ShadowRun.Services;
using UnityEngine;
using UnityEngine.UI;

// Port of Il2CppOutputProject/Source/il2cppOutput/InfernoDriving.inc — hand-maintained
// driving additions: road crystals, the per-run crystal counter, the SPEED UP stage
// flash, the neon-framed pause button and the traffic paint cycle. The namespace's
// static state (s_*) is kept as static fields of this nested class.
public sealed partial class ShadowRunPrototype
{
	private static partial class InfernoDriving
	{
		public struct Crystal
		{
			public Transform transform;
			public bool active;
			public float phase;
		}

		private static ShadowRunPrototype s_owner;
		public static readonly Crystal[] s_crystals = new Crystal[3];   // shared with InfernoHazards (NearCrystal)
		private static Material s_material;
		private static Text s_feedback;
		private static Text s_runCounter;      // HUD: crystals collected in this run
		private static Image s_runGem;
		private static int s_runCollected;
		private static float s_feedbackTime, s_nextDistance, s_previousX;
		private static Text s_stageNote;       // HUD: "SPEED UP" flash at every 1000 m speed step
		private static int s_stage;
		private static float s_stageTime;
		private static int s_lastPaint = -1;
		private const float CrystalSize = 1.8f;   // the card's edge; the PNG's diamond spans it tip to tip
		private const float CrystalHeight = 1.5f;

		// Three dark paints only: navy (style 0), dark red (style 4), dark green (style 3);
		// the colour pairs live in TRAFFIC_PAINTS in tools/apply_lobby_patches.py.
		// ApplyTrafficDriverAppearance tests style 2, 0, 4, 3 in that order and paints
		// anything else graphite, so style 1 is grey, not navy.
		private static readonly int[] PaintStyles = { 0, 4, 3 };

		public static int NextPaintStyle()
		{
			// Separate colour selection from AI behaviour, and avoid consecutive repeats.
			int count = PaintStyles.Length;
			int next = Random.Range(0, s_lastPaint < 0 ? count : count - 1);
			if (s_lastPaint >= 0 && next >= s_lastPaint) ++next;
			s_lastPaint = next;
			return PaintStyles[next];
		}

		// A road crystal is a camera-facing card showing the lobby's crystal picture
		// (GemSprite: Data/Raw/InfernoLobby/crystal.png, mip-mapped), on the game's own
		// transparent unlit material so the cut-out edges stay clean and it ignores the
		// scene lighting like the HUD art does. Both faces are drawn, so the card never
		// disappears while it turns toward the camera.
		private static void EnsureAssets(ShadowRunPrototype game)
		{
			if (s_material != null) return;
			Sprite sprite = InfernoLobby.GemSprite(game);
			Texture texture = sprite != null ? sprite.texture : null;
			s_material = game.MatTransparentUnlit(InfernoLobby.Hex(0xFFFFFF), texture, false);
			string cull = "_Cull";
			if (s_material.HasProperty(cull)) s_material.SetFloat(cull, 0f);
		}

		// Yaw that turns the quad's front (Unity's Quad faces -Z) toward the camera.
		private static Quaternion FaceCamera(ShadowRunPrototype game, Vector3 position)
		{
			float dx = 0f, dz = 1f;   // no camera: face +Z, where the chase camera sits
			if (game.cam != null)
			{
				Vector3 eye = game.cam.transform.position;
				dx = eye.x - position.x;
				dz = eye.z - position.z;
			}
			float yaw = Mathf.Atan2(-dx, -dz);
			return new Quaternion(0f, Mathf.Sin(yaw / 2f), 0f, Mathf.Cos(yaw / 2f));
		}

		private static void SetVisible(ref Crystal crystal, bool visible)
		{
			crystal.active = visible;
			if (crystal.transform != null) crystal.transform.gameObject.SetActive(visible);
		}

		public static void Hide(ShadowRunPrototype game)
		{
			if (s_owner != game) return;
			for (int i = 0; i < s_crystals.Length; i++) SetVisible(ref s_crystals[i], false);
			s_feedbackTime = 0f;
			s_stageTime = 0f;
			if (s_feedback != null) s_feedback.gameObject.SetActive(false);
			if (s_stageNote != null) s_stageNote.gameObject.SetActive(false);
		}

		public static void Dispose(ShadowRunPrototype game)
		{
			if (s_owner != game) return;
			for (int i = 0; i < s_crystals.Length; i++)
			{
				if (s_crystals[i].transform != null) Object.Destroy(s_crystals[i].transform.gameObject);
				s_crystals[i] = default;
			}
			if (s_feedback != null) Object.Destroy(s_feedback.gameObject);
			if (s_stageNote != null) Object.Destroy(s_stageNote.gameObject);
			if (s_runCounter != null) Object.Destroy(s_runCounter.gameObject);
			if (s_runGem != null) Object.Destroy(s_runGem.gameObject);
			if (s_material != null) Object.Destroy(s_material);   // the texture belongs to GemSprite
			s_material = null; s_feedback = null; s_stageNote = null; s_runCounter = null; s_runGem = null; s_owner = null;
		}

		// Crystals collected during the current run, shown at the top centre of the HUD
		// (score is top-left, lives and pause top-right). Created on the first run.
		private static void ShowRunCounter(ShadowRunPrototype game)
		{
			if (game.hudRoot == null) return;
			Transform hud = game.hudRoot.transform;
			if (s_runGem == null)
			{
				s_runGem = InfernoLobby.Block(game, hud, "RunCrystalGem", -46f, -58f, 44f, 44f, 0xFFFFFF);
				InfernoLobby.Place(s_runGem.rectTransform, -46f, -58f, 44f, 44f, new Vector2(0.5f, 1f));
				s_runGem.sprite = InfernoLobby.GemSprite(game);
				s_runGem.preserveAspect = true;
			}
			if (s_runCounter == null)
			{
				s_runCounter = InfernoLobby.Label(game, hud, "RunCrystalCount", "0", 30f, -58f, 120f, 52f, 36, InfernoLobby.Pearl, 3);
				InfernoLobby.Place(s_runCounter.rectTransform, 30f, -58f, 120f, 52f, new Vector2(0.5f, 1f));
			}
			InfernoLobby.Caption(s_runCounter, s_runCollected.ToString());
			s_runGem.gameObject.SetActive(true);
			s_runCounter.gameObject.SetActive(true);
		}

		public static void Reset(ShadowRunPrototype game)
		{
			if (s_owner != null && s_owner != game) Dispose(s_owner);
			s_owner = game;
			Hide(game);
			s_previousX = game.playerX;
			s_nextDistance = game.distance + Random.Range(350f, 650f);
			s_lastPaint = -1;
			s_runCollected = 0;
			s_stage = 0;
			ShowRunCounter(game);
		}

		// Speed stages (InfernoDrivingRules.Stage): every 1000 m the cruise speed steps
		// up; the HUD flashes SPEED UP with the stage number for a moment.
		private static void AnnounceStage(ShadowRunPrototype game, int stage)
		{
			if (game.hudRoot == null) return;
			string note = "SPEED UP  -  STAGE " + stage;
			if (s_stageNote == null)
			{
				s_stageNote = InfernoLobby.Label(game, game.hudRoot.transform,
					"SpeedStage", note, 0f, 210f, 640f, 60f, 40, InfernoLobby.Electric);
				InfernoLobby.Place(s_stageNote.rectTransform, 0f, 210f, 640f, 60f, new Vector2(0.5f, 0.5f));
			}
			else
			{
				InfernoLobby.Caption(s_stageNote, note);
			}
			s_stageTime = 1.8f;
			s_stageNote.gameObject.SetActive(true);
		}

		// The HUD pause button in the lobby's neon frame. The game builds it from
		// rounded glass: a tinted rim, a dark surface (the Button's target graphic)
		// and the bars. The rim becomes an invisible hit area and the surface becomes
		// the octagon frame stretched over the whole button, so the pressed tint the
		// game applies to it now shows on the frame; the bars keep their logic. (The
		// BEAM button had the same treatment briefly and went back to the game's own
		// look on request, 2026-09-18.)
		private static bool Named(Transform transform, string name)
		{
			return transform.name == name;
		}

		private static void Stretch(RectTransform rect)
		{
			rect.anchorMin = new Vector2(0f, 0f);
			rect.anchorMax = new Vector2(1f, 1f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = new Vector2(0f, 0f);
			rect.anchoredPosition = new Vector2(0f, 0f);
		}

		private static Image FrameHudButton(ShadowRunPrototype game, RectTransform rect, string surface)
		{
			Image rim = rect.GetComponent<Image>();
			if (rim != null) rim.color = InfernoLobby.Hex(InfernoLobby.Black, 0f);   // hit area only
			Transform face = InfernoLobby.Find(rect, surface);
			Image frame = face != null ? face.GetComponent<Image>() : null;
			if (frame == null) return null;
			Stretch(frame.rectTransform);
			frame.sprite = InfernoLobby.NeonFrameSprite(game);
			frame.type = Image.Type.Sliced;
			InfernoLobby.Paint(frame, 0xFFFFFF);
			return frame;
		}

		public static void StylePauseButton(ShadowRunPrototype game)
		{
			RectTransform rect = game.pauseHudButtonRect;
			if (rect == null || FrameHudButton(game, rect, "Pause Dark Surface") == null) return;
			int count = rect.childCount;
			for (int i = 0; i < count; ++i)
			{
				Transform child = rect.GetChild(i);
				if (Named(child, "Pause Bar")) InfernoLobby.Paint(child.GetComponent<Image>(), InfernoLobby.FrameText);
			}
		}

		public static bool ClearLane(ShadowRunPrototype game, int lane, float x, float z)
		{
			if (game.TrafficSpawnBlockedByRamp(lane, z)) return false;
			var traffic = game.pickups;
			if (traffic == null) return true;
			for (int i = 0; i < traffic.Count; ++i)
			{
				Pickup car = traffic[i];
				if (car.Transform == null) continue;
				Vector3 pos = car.Transform.position;
				if (Mathf.Abs(pos.x - x) < 2.4f && Mathf.Abs(pos.z - z) < 20f) return false;
			}
			return true;
		}

		private static bool Spawn(ShadowRunPrototype game, float playerZ)
		{
			if (game.lanes == null || game.lanes.Length == 0) return false;
			int slot = -1;
			for (int i = 0; i < s_crystals.Length; i++) if (!s_crystals[i].active) { slot = i; break; }
			if (slot < 0) return false;
			int lanes = game.lanes.Length;
			int first = Random.Range(0, lanes);
			float z = playerZ - Random.Range(100f, 140f);
			for (int attempt = 0; attempt < lanes; ++attempt)
			{
				int lane = (first + attempt) % lanes;
				float x = game.lanes[lane];
				if (!ClearLane(game, lane, x, z)) continue;
				EnsureAssets(game);
				Vector3 position = new Vector3(x, CrystalHeight, z);
				if (s_crystals[slot].transform == null)
				{
					// The game's Quad helper: primitive quad, material applied, collider removed, no shadows.
					GameObject obj = game.Quad("RoadCrystal", position, new Vector3(CrystalSize, CrystalSize, 1f), s_material);
					s_crystals[slot].transform = obj.transform;
					s_crystals[slot].transform.SetParent(game.transform, false);
				}
				s_crystals[slot].phase = Random.Range(0f, 6.2831853f);
				s_crystals[slot].transform.position = position;
				s_crystals[slot].transform.localRotation = FaceCamera(game, position);
				SetVisible(ref s_crystals[slot], true);
				return true;
			}
			return false;
		}

		private static void Collect(ShadowRunPrototype game)
		{
			int balance = SupabaseGameManager.Crystals;
			if (balance == int.MaxValue) return;
			game.menuGemCount = balance + 1;
			SupabaseGameManager.SetCrystals(game.menuGemCount);
			++s_runCollected;
			ShowRunCounter(game);
			if (s_feedback == null && game.hudRoot != null)
			{
				s_feedback = InfernoLobby.Label(game, game.hudRoot.transform,
					"CrystalCollected", "+1 CRYSTAL", 0f, 140f, 420f, 48f, 30, InfernoLobby.Blossom);
				InfernoLobby.Place(s_feedback.rectTransform, 0f, 140f, 420f, 48f, new Vector2(0.5f, 0.5f));
			}
			s_feedbackTime = 1.2f;
			if (s_feedback != null) s_feedback.gameObject.SetActive(true);
		}

		// Called only from the running simulation with its scaled gameplay dt. Crystals
		// are separate from the traffic list, so they never participate in car collisions.
		public static void Tick(ShadowRunPrototype game, float dt)
		{
			if (s_owner != game || !game.running || game.paused || dt <= 0f || game.player == null) return;
			Vector3 player = game.player.position;
			for (int i = 0; i < s_crystals.Length; i++)
			{
				if (!s_crystals[i].active) continue;
				Vector3 previous = s_crystals[i].transform.position;
				Vector3 position = previous;
				position.z += game.speed * dt;
				s_crystals[i].phase = (s_crystals[i].phase + dt * 1.8f) % 6.2831853f;
				position.y = CrystalHeight + 0.16f * Mathf.Sin(s_crystals[i].phase * 2f);
				bool hit = !game.playerAirborne && !game.playerRampRiding && game.lifeHalfUnits > 0 &&
					InfernoDrivingRules.CrossesCrystal(previous.x - s_previousX, previous.z - player.z,
						position.x - game.playerX, position.z - player.z);
				if (hit) { SetVisible(ref s_crystals[i], false); Collect(game); continue; }
				if (position.z > player.z + 15f) { SetVisible(ref s_crystals[i], false); continue; }
				s_crystals[i].transform.position = position;
				// Keep the card square to the camera as it approaches; a slow breathing
				// scale stands in for the glints the faceted mesh used to catch.
				s_crystals[i].transform.localRotation = FaceCamera(game, position);
				float size = CrystalSize * (1f + 0.05f * Mathf.Sin(s_crystals[i].phase));
				s_crystals[i].transform.localScale = new Vector3(size, size, 1f);
			}
			s_previousX = game.playerX;
			if (s_feedbackTime > 0f)
			{
				s_feedbackTime = Mathf.Max(0f, s_feedbackTime - dt);
				if (s_feedback != null)
				{
					s_feedback.color = InfernoLobby.Hex(InfernoLobby.Blossom, Mathf.Min(1f, s_feedbackTime / 0.3f));
					if (s_feedbackTime == 0f) s_feedback.gameObject.SetActive(false);
				}
			}
			if (s_stageTime > 0f)
			{
				s_stageTime = Mathf.Max(0f, s_stageTime - dt);
				if (s_stageNote != null)
				{
					float pulse = 1f + 0.08f * Mathf.Sin(s_stageTime * 18f);
					s_stageNote.rectTransform.localScale = new Vector3(pulse, pulse, 1f);
					s_stageNote.color = InfernoLobby.Hex(InfernoLobby.Electric, Mathf.Min(1f, s_stageTime / 0.4f));
					if (s_stageTime == 0f) s_stageNote.gameObject.SetActive(false);
				}
			}
			int stage = InfernoDrivingRules.Stage(game.distance);
			if (stage > s_stage && game.lifeHalfUnits > 0) { s_stage = stage; AnnounceStage(game, stage); }
			if (game.lifeHalfUnits > 0 && game.distance >= s_nextDistance)
			{
				bool spawned = Spawn(game, player.z);
				s_nextDistance = game.distance + (spawned ?
					InfernoDrivingRules.CrystalGap(Random.Range(0f, 1f)) : 80f);
			}
		}
	}
}
