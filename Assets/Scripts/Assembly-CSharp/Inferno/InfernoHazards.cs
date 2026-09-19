using UnityEngine;
using UnityEngine.UI;

// Port of Il2CppOutputProject/Source/il2cppOutput/InfernoHazards.inc — road hazards,
// hand-maintained: puddles that can throw the car into a spin, and wrecking balls
// swinging across the road from a steel gantry. Reuses InfernoDriving's lane checks and
// crystal pool and the lobby helpers; wired into the generated code at the hook points
// of tools/apply_lobby_patches.py (patch_hazards). See docs/lobby-implementation.md,
// "Road hazards".
public sealed partial class ShadowRunPrototype
{
	private static partial class InfernoHazards
	{
		public const float PlayerZ = 5.0f;              // the car never moves along z; the road scrolls (+z toward the camera)
		private const float CarHalfWidth = 1.05f, CarHalfLength = 2.4f, CarHeight = 1.6f;
		private const float Pi = 3.14159265f;

		private static ShadowRunPrototype s_owner;
		private static Text s_note;               // HUD: SPIN OUT! / SLIPPERY! / WRECKING BALL!
		private static float s_noteTime;
		private static uint s_noteColor = InfernoLobby.Blossom;

		// ---- procedural textures ----------------------------------------------------------
		public delegate void PixelShader(ref Color32 px, float u, float v);

		public static Texture2D MakeTexture(int w, int h, PixelShader pixel)
		{
			Texture2D texture = new Texture2D(w, h, TextureFormat.RGBA32, true);   // mip chain (U_NewTexture2DMips)
			Color32[] colors = new Color32[w * h];
			for (int y = 0; y < h; ++y)
				for (int x = 0; x < w; ++x)
					pixel(ref colors[y * w + x], (x + 0.5f) / w * 2f - 1f, (y + 0.5f) / h * 2f - 1f);
			texture.SetPixels32(colors);
			texture.Apply();
			texture.wrapMode = TextureWrapMode.Clamp;
			return texture;
		}

		private static float Smooth(float edge0, float edge1, float x)
		{
			float t = Mathf.Max(0f, Mathf.Min(1f, (x - edge0) / (edge1 - edge0)));
			return t * t * (3f - 2f * t);
		}

		public static void Set(ref Color32 px, int r, int g, int b, int a)
		{
			px.r = (byte)Mathf.Max(0, Mathf.Min(255, r)); px.g = (byte)Mathf.Max(0, Mathf.Min(255, g));
			px.b = (byte)Mathf.Max(0, Mathf.Min(255, b)); px.a = (byte)Mathf.Max(0, Mathf.Min(255, a));
		}

		// A puddle: dark water with a soft edge, a pale sky reflection across the middle
		// and faint ripple rings. u, v in -1..1 across the card.
		private static void PuddlePixel(ref Color32 px, float u, float v)
		{
			float r = Mathf.Sqrt(u * u + v * v);
			float edge = Smooth(1.0f, 0.78f, r);                       // 1 inside, fades out at the rim
			float sheen = Mathf.Exp(-Mathf.Pow((v - 0.2f) * 2.6f, 2.0f)) * Mathf.Exp(-Mathf.Pow(u * 1.3f, 2.0f));
			float ripple = 0.5f + 0.5f * Mathf.Sin(r * 21.0f - 1.2f);
			float baseR = 0.10f, baseG = 0.16f, baseB = 0.30f, skyR = 0.70f, skyG = 0.80f, skyB = 1.00f;
			float mix = Mathf.Min(1.0f, sheen * 0.75f + ripple * 0.10f * (1f - r));
			Set(ref px, (int)(255f * (baseR + (skyR - baseR) * mix)),
				(int)(255f * (baseG + (skyG - baseG) * mix)),
				(int)(255f * (baseB + (skyB - baseB) * mix)),
				(int)(255f * edge * (0.80f + 0.15f * ripple)));
		}

		// Soft round shadow under the wrecking ball.
		private static void ShadowPixel(ref Color32 px, float u, float v)
		{
			float r = Mathf.Sqrt(u * u + v * v);
			Set(ref px, 0, 0, 0, (int)(255f * 0.62f * Smooth(1.0f, 0.35f, r)));
		}

		// ---- puddles ----------------------------------------------------------------------
		// A puddle is a flat card on the road (the game's Quad helper, turned to face
		// up). Driving through one at speed either sends the car into a full spin
		// (about two thirds of the time) or just makes it skid. Airborne cars fly over.
		private const int PuddleCount = 3;
		private const float PuddleWidth = 3.0f, PuddleLength = 5.6f, PuddleY = 0.04f;
		private const float PuddleFirstMin = 500f, PuddleFirstMax = 900f, PuddleGapMin = 450f, PuddleGapMax = 800f;

		private struct Puddle
		{
			public Transform transform;
			public bool active;
			public bool used;
		}

		private static readonly Puddle[] s_puddles = new Puddle[PuddleCount];
		private static Material s_puddleMaterial;
		private static float s_nextPuddle;

		// The spin: the car visual turns a full circle about the world up axis on top
		// of whatever rotation the game gave it this frame, with the steering washed
		// out while it lasts. Ends exactly where it started (360 degrees), so there is
		// no snap when control returns.
		private struct Spin
		{
			public bool active;
			public float time, duration;
			public int direction;
		}

		private static Spin s_spin = new Spin { duration = 1.15f, direction = 1 };

		private static void SetActive(Transform transform, bool on)
		{
			if (transform != null) transform.gameObject.SetActive(on);
		}

		private static void ShowNote(ShadowRunPrototype game, string text, uint color)
		{
			if (game.hudRoot == null) return;
			if (s_note == null)
			{
				s_note = InfernoLobby.Label(game, game.hudRoot.transform, "HazardNote", text, 0f, 270f, 640f, 60f, 42, color);
				InfernoLobby.Place(s_note.rectTransform, 0f, 270f, 640f, 60f, new Vector2(0.5f, 0.5f));
			}
			else
			{
				InfernoLobby.Caption(s_note, text);
			}
			s_noteColor = color;
			s_noteTime = 1.4f;
			s_note.gameObject.SetActive(true);
		}

		private static bool NearCrystal(float x, float z)
		{
			var crystals = InfernoDriving.s_crystals;
			for (int i = 0; i < crystals.Length; i++)
			{
				if (!crystals[i].active || crystals[i].transform == null) continue;
				Vector3 pos = crystals[i].transform.position;
				if (Mathf.Abs(pos.x - x) < 2.4f && Mathf.Abs(pos.z - z) < 16f) return true;
			}
			return false;
		}

		private static bool NearPuddle(float z, float within)
		{
			for (int i = 0; i < s_puddles.Length; i++)
			{
				if (!s_puddles[i].active) continue;
				Vector3 pos = s_puddles[i].transform.position;
				if (Mathf.Abs(pos.z - z) < within) return true;
			}
			return false;
		}

		private static bool SpawnPuddle(ShadowRunPrototype game, float playerZ)
		{
			if (game.lanes == null || game.lanes.Length == 0) return false;
			int slot = -1;
			for (int i = 0; i < s_puddles.Length; i++) if (!s_puddles[i].active) { slot = i; break; }
			if (slot < 0) return false;
			int lanes = game.lanes.Length;
			int first = Random.Range(0, lanes);
			float z = playerZ - Random.Range(110f, 150f);
			for (int attempt = 0; attempt < lanes; ++attempt)
			{
				int lane = (first + attempt) % lanes;
				float x = game.lanes[lane];
				if (!InfernoDriving.ClearLane(game, lane, x, z) || NearCrystal(x, z)) continue;
				if (s_puddleMaterial == null)
				{
					s_puddleMaterial = game.MatTransparentUnlit(InfernoLobby.Hex(0xFFFFFF), MakeTexture(128, 128, PuddlePixel), false);
					string cull = "_Cull";
					if (s_puddleMaterial.HasProperty(cull)) s_puddleMaterial.SetFloat(cull, 0f);
				}
				Vector3 position = new Vector3(x, PuddleY, z);
				if (s_puddles[slot].transform == null)
				{
					GameObject obj = game.Quad("RoadPuddle", position, new Vector3(PuddleWidth, PuddleLength, 1f), s_puddleMaterial);
					s_puddles[slot].transform = obj.transform;
					s_puddles[slot].transform.SetParent(game.transform, false);
					// Unity's Quad faces -Z; a quarter turn about X lays it flat, face up.
					s_puddles[slot].transform.localRotation = new Quaternion(0.70710678f, 0f, 0f, 0.70710678f);
				}
				s_puddles[slot].transform.position = position;
				float stretch = Random.Range(0.85f, 1.15f);
				s_puddles[slot].transform.localScale = new Vector3(PuddleWidth * (2f - stretch) * 0.95f, PuddleLength * stretch, 1f);
				s_puddles[slot].active = true;
				s_puddles[slot].used = false;
				SetActive(s_puddles[slot].transform, true);
				return true;
			}
			return false;
		}

		private static void Splash(ShadowRunPrototype game, ref Puddle puddle)
		{
			puddle.used = true;
			float vx = game.velocityX;
			int direction = vx > 0.3f ? 1 : vx < -0.3f ? -1 : (Random.Range(0, 2) != 0 ? 1 : -1);
			game.tractionLoss = 1f;
			game.impactShake = Mathf.Max(game.impactShake, 0.35f);
			game.PlayImpactHaptic(1);
			if (Random.Range(0f, 1f) < 0.65f)
			{
				s_spin.active = true;
				s_spin.time = 0f;
				s_spin.duration = Random.Range(1.0f, 1.3f);
				s_spin.direction = direction;
				game.speed = InfernoDrivingRules.Impact(game.speed, game.speed * 0.78f);
				game.velocityX = direction * 6.0f;
				ShowNote(game, "SPIN OUT!", InfernoLobby.Magenta);
			}
			else
			{
				game.impactYaw = direction * 7.0f;
				game.speed = InfernoDrivingRules.Impact(game.speed, game.speed * 0.93f);
				ShowNote(game, "SLIPPERY!", InfernoLobby.Lavender);
			}
		}

		// Quaternion helpers (x, y, z, w).
		public static Quaternion Mul(Quaternion a, Quaternion b)
		{
			return new Quaternion(a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
				a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
				a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w,
				a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z);
		}

		private static Quaternion YawQuat(float degrees)
		{
			float half = degrees * Pi / 360.0f;
			return new Quaternion(0f, Mathf.Sin(half), 0f, Mathf.Cos(half));
		}

		private static void TickSpin(ShadowRunPrototype game, float dt)
		{
			if (!s_spin.active) return;
			s_spin.time += dt;
			float u = Mathf.Min(1.0f, s_spin.time / s_spin.duration);
			float yaw = 360.0f * u * u * (3f - 2f * u) * s_spin.direction;
			if (game.carVisual != null)
			{
				Quaternion current = game.carVisual.localRotation;
				game.carVisual.localRotation = Mul(YawQuat(yaw), current);
			}
			// No grip while spinning: the car slides on with whatever sideways motion it had.
			game.tractionLoss = 1f;
			game.velocityX += (s_spin.direction * 2.5f - game.velocityX) * (1f - Mathf.Exp(-2.5f * dt));   // -expm1(-2.5 dt)
			if (u >= 1f) s_spin.active = false;
		}

		private static void TickPuddles(ShadowRunPrototype game, float dt, float playerZ)
		{
			for (int i = 0; i < s_puddles.Length; i++)
			{
				if (!s_puddles[i].active) continue;
				Vector3 position = s_puddles[i].transform.position;
				position.z += game.speed * dt;
				s_puddles[i].transform.position = position;
				if (position.z > playerZ + 12f) { s_puddles[i].active = false; SetActive(s_puddles[i].transform, false); continue; }
				if (s_puddles[i].used || game.playerAirborne || game.playerRampRiding || game.lifeHalfUnits <= 0) continue;
				if (Mathf.Abs(position.x - game.playerX) < PuddleWidth * 0.5f + CarHalfWidth * 0.6f &&
					Mathf.Abs(position.z - playerZ) < PuddleLength * 0.5f + CarHalfLength * 0.5f && game.speed > 12f)
					Splash(game, ref s_puddles[i]);
			}
			if (game.lifeHalfUnits > 0 && game.distance >= s_nextPuddle)
			{
				bool spawned = SpawnPuddle(game, playerZ);
				s_nextPuddle = game.distance + (spawned ? Random.Range(PuddleGapMin, PuddleGapMax) : 70.0f);
			}
		}

		// ---- wrecking balls ------------------------------------------------------------------
		// A steel gantry spans the road; a ball on a rope swings across it as a
		// pendulum. It only reaches car height near the middle of its swing, so the
		// road is open while the ball is up at either side: time the pass. Contact is
		// a heavy side impact (a whole heart, the car is shoved across the road).
		private const int BallCount = 2;
		private const float GantryHeight = 10.6f, GantryHalfSpan = 9.6f, RopeLength = 9.2f, BallRadius = 1.1f;
		private const float SwingAmplitude = 62.0f * Pi / 180.0f, SwingPeriod = 2.7f;
		private const float BallFirstMin = 900f, BallFirstMax = 1300f, BallGapMin = 800f, BallGapMax = 1300f;

		private struct Ball
		{
			public Transform root;      // gantry (posts + beam); moves with the road
			public Transform ball;      // the sphere, positioned under the root every frame
			public Transform shadow;    // flat dark card on the road below the ball
			public LineRenderer rope;
			public bool active;
			public float phase, cooldown;
		}

		private static readonly Ball[] s_balls = new Ball[BallCount];
		private static Material s_steel;
		private static Material s_iron;
		private static Material s_neon;
		private static Material s_shadowMaterial;
		private static float s_nextBall;

		private static void EnsureBallMaterials(ShadowRunPrototype game)
		{
			if (s_steel != null) return;
			Color? none = null;
			s_steel = game.Mat(new Color(0.26f, 0.27f, 0.31f, 1f), none);
			s_iron = game.Mat(new Color(0.16f, 0.15f, 0.18f, 1f), none);
			s_neon = game.MatUnlit(InfernoLobby.Hex(InfernoLobby.Electric));
			s_shadowMaterial = game.MatTransparentUnlit(InfernoLobby.Hex(0xFFFFFF), MakeTexture(64, 64, ShadowPixel), false);
			string cull = "_Cull";
			if (s_shadowMaterial.HasProperty(cull)) s_shadowMaterial.SetFloat(cull, 0f);
		}

		private static void BuildBall(ShadowRunPrototype game, ref Ball slot)
		{
			EnsureBallMaterials(game);
			GameObject obj = new GameObject("WreckingBall");
			slot.root = obj.transform;
			slot.root.SetParent(game.transform, false);
			// gantry: two posts, a beam, and a neon strip under the beam
			game.AddCube(slot.root, "PostL", new Vector3(-GantryHalfSpan, GantryHeight / 2f, 0f), new Vector3(0.6f, GantryHeight, 0.6f), s_steel);
			game.AddCube(slot.root, "PostR", new Vector3(GantryHalfSpan, GantryHeight / 2f, 0f), new Vector3(0.6f, GantryHeight, 0.6f), s_steel);
			game.AddCube(slot.root, "Beam", new Vector3(0f, GantryHeight, 0f), new Vector3(GantryHalfSpan * 2f + 0.6f, 0.7f, 0.7f), s_steel);
			game.AddCube(slot.root, "BeamNeon", new Vector3(0f, GantryHeight - 0.42f, 0f), new Vector3(GantryHalfSpan * 2f - 1.0f, 0.08f, 0.5f), s_neon);
			game.AddCube(slot.root, "Hook", new Vector3(0f, GantryHeight - 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.5f), s_iron);
			// the ball
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.name = "Ball";
			game.PreparePrimitive(sphere, s_iron);
			slot.ball = sphere.transform;
			slot.ball.SetParent(slot.root, false);
			slot.ball.localScale = new Vector3(BallRadius * 2f, BallRadius * 2f, BallRadius * 2f);
			// studs: four small cubes on the ball so it reads as a wrecking ball, not a marble
			const float stud = 0.28f;
			game.AddCube(slot.ball, "Stud1", new Vector3(0.42f, 0.2f, 0f), new Vector3(stud, stud, stud), s_steel);
			game.AddCube(slot.ball, "Stud2", new Vector3(-0.42f, -0.2f, 0f), new Vector3(stud, stud, stud), s_steel);
			game.AddCube(slot.ball, "Stud3", new Vector3(0f, 0.3f, 0.4f), new Vector3(stud, stud, stud), s_steel);
			game.AddCube(slot.ball, "Stud4", new Vector3(0f, -0.3f, -0.4f), new Vector3(stud, stud, stud), s_steel);
			// the rope (local to the root: hook -> ball)
			Vector3[] points = new Vector3[2];
			points[0] = new Vector3(0f, GantryHeight - 0.5f, 0f);
			points[1] = new Vector3(0f, GantryHeight - RopeLength, 0f);
			slot.rope = game.AddLine(slot.root, "Rope", points, 0.14f, s_steel);
			// the shadow card on the road
			GameObject card = game.Quad("BallShadow", new Vector3(0f, 0.05f, 0f), new Vector3(BallRadius * 2.4f, BallRadius * 2.4f, 1f), s_shadowMaterial);
			slot.shadow = card.transform;
			slot.shadow.SetParent(slot.root, false);
			slot.shadow.localRotation = new Quaternion(0.70710678f, 0f, 0f, 0.70710678f);
		}

		private static bool RoadClearForGantry(ShadowRunPrototype game, float z)
		{
			if (game.lanes == null) return false;
			for (int lane = 0; lane < game.lanes.Length; ++lane)
				for (float dz = -24f; dz <= 24f; dz += 12f)
					if (game.TrafficSpawnBlockedByRamp(lane, z + dz)) return false;
			return !NearPuddle(z, 30f);
		}

		private static bool SpawnBall(ShadowRunPrototype game, float playerZ)
		{
			int slot = -1;
			for (int i = 0; i < s_balls.Length; i++) if (!s_balls[i].active) { slot = i; break; }
			if (slot < 0) return false;
			float z = playerZ - 175f;
			if (!RoadClearForGantry(game, z)) return false;
			if (s_balls[slot].root == null) BuildBall(game, ref s_balls[slot]);
			s_balls[slot].root.position = new Vector3(0f, 0f, z);
			s_balls[slot].phase = Random.Range(0f, 2f * Pi);
			s_balls[slot].cooldown = 0f;
			s_balls[slot].active = true;
			SetActive(s_balls[slot].root, true);
			return true;
		}

		// Position of the ball for a swing phase: hangs from the hook, angle theta.
		private static void PlaceBall(ref Ball ball)
		{
			float theta = SwingAmplitude * Mathf.Sin(ball.phase);
			Vector3 local = new Vector3(RopeLength * Mathf.Sin(theta), GantryHeight - 0.5f - RopeLength * Mathf.Cos(theta), 0f);
			ball.ball.localPosition = local;
			ball.rope.SetPosition(1, local);
			float height = Mathf.Max(0f, local.y - BallRadius);
			float size = BallRadius * 2.4f * Mathf.Max(0.35f, 1f - height / 12.0f);
			ball.shadow.localPosition = new Vector3(local.x, 0.05f, 0f);
			ball.shadow.localScale = new Vector3(size, size, 1f);
		}

		private static void Wreck(ShadowRunPrototype game, ref Ball ball, Vector3 ballWorld)
		{
			ball.cooldown = 1.2f;
			// shove the car the way the ball is travelling; at the turning point, away from the ball
			float swing = Mathf.Cos(ball.phase);
			int direction = swing > 0.05f ? 1 : swing < -0.05f ? -1 : (game.playerX >= ballWorld.x ? 1 : -1);
			game.velocityX = direction * 14.0f;
			game.impactYaw = direction * 9.0f;
			game.impactRoll = direction * 2.4f;
			game.impactShake = 1f;
			game.tractionLoss = 1f;
			game.speed = InfernoDrivingRules.Impact(game.speed, game.speed * 0.5f);
			game.SpawnTrafficCollisionExplosion(true, ballWorld, direction);
			game.TriggerVehicleEngineImpact(true);
			game.PlayCollisionHaptic(true);
			if (game.invuln <= 0f)
			{
				game.lifeHalfUnits = Mathf.Max(0, game.lifeHalfUnits - 2);
				game.invuln = 1.5f;
				game.combo = 0;
			}
			ShowNote(game, "WRECKING BALL!", InfernoLobby.Violet);
		}

		private static void TickBalls(ShadowRunPrototype game, float dt, float playerZ)
		{
			for (int i = 0; i < s_balls.Length; i++)
			{
				if (!s_balls[i].active) continue;
				Vector3 position = s_balls[i].root.position;
				position.z += game.speed * dt;
				s_balls[i].root.position = position;
				if (position.z > playerZ + 26f) { s_balls[i].active = false; SetActive(s_balls[i].root, false); continue; }
				s_balls[i].phase = (s_balls[i].phase + dt * 2f * Pi / SwingPeriod) % (2f * Pi);
				PlaceBall(ref s_balls[i]);
				if (s_balls[i].cooldown > 0f) s_balls[i].cooldown = Mathf.Max(0f, s_balls[i].cooldown - dt);
				if (s_balls[i].cooldown > 0f || game.lifeHalfUnits <= 0) continue;
				// sphere against the car's box (x: lane position, y: on the road or in a jump, z: fixed)
				Vector3 centre = s_balls[i].ball.position;
				float bottom = game.playerAirborne ? game.playerJumpHeight : 0f;
				float cx = Mathf.Max(game.playerX - CarHalfWidth, Mathf.Min(game.playerX + CarHalfWidth, centre.x));
				float cy = Mathf.Max(bottom, Mathf.Min(bottom + CarHeight, centre.y));
				float cz = Mathf.Max(PlayerZ - CarHalfLength, Mathf.Min(PlayerZ + CarHalfLength, centre.z));
				float dx = centre.x - cx, dy = centre.y - cy, dz = centre.z - cz;
				if (dx * dx + dy * dy + dz * dz < BallRadius * BallRadius) Wreck(game, ref s_balls[i], centre);
			}
			if (game.lifeHalfUnits > 0 && game.distance >= s_nextBall)
			{
				bool spawned = SpawnBall(game, playerZ);
				s_nextBall = game.distance + (spawned ? Random.Range(BallGapMin, BallGapMax) : 60.0f);
			}
		}

		// ---- lifecycle (mirrors InfernoDriving: Reset per run, Hide on menu/game over, Dispose on destroy)
		public static void Hide(ShadowRunPrototype game)
		{
			if (s_owner != game) return;
			for (int i = 0; i < s_puddles.Length; i++) { s_puddles[i].active = false; SetActive(s_puddles[i].transform, false); }
			for (int i = 0; i < s_balls.Length; i++) { s_balls[i].active = false; SetActive(s_balls[i].root, false); }
			s_spin.active = false;
			s_noteTime = 0f;
			if (s_note != null) s_note.gameObject.SetActive(false);
		}

		public static void Dispose(ShadowRunPrototype game)
		{
			if (s_owner != game) return;
			for (int i = 0; i < s_puddles.Length; i++)
			{
				if (s_puddles[i].transform != null) Object.Destroy(s_puddles[i].transform.gameObject);
				s_puddles[i] = default;
			}
			for (int i = 0; i < s_balls.Length; i++)
			{
				if (s_balls[i].root != null) Object.Destroy(s_balls[i].root.gameObject);
				s_balls[i] = default;
			}
			if (s_note != null) Object.Destroy(s_note.gameObject);
			if (s_puddleMaterial != null) Object.Destroy(s_puddleMaterial);
			if (s_steel != null) Object.Destroy(s_steel);
			if (s_iron != null) Object.Destroy(s_iron);
			if (s_neon != null) Object.Destroy(s_neon);
			if (s_shadowMaterial != null) Object.Destroy(s_shadowMaterial);
			s_puddleMaterial = null; s_steel = null; s_iron = null; s_neon = null; s_shadowMaterial = null;
			s_note = null; s_owner = null; s_spin = new Spin { duration = 1.15f, direction = 1 };
		}

		public static void Reset(ShadowRunPrototype game)
		{
			if (s_owner != null && s_owner != game) Dispose(s_owner);
			s_owner = game;
			Hide(game);
			s_nextPuddle = game.distance + Random.Range(PuddleFirstMin, PuddleFirstMax);
			s_nextBall = game.distance + Random.Range(BallFirstMin, BallFirstMax);
		}

		// Called at the end of UpdateWorld with the scaled gameplay dt, after
		// UpdatePlayer has set this frame's car rotation (the spin builds on it).
		public static void Tick(ShadowRunPrototype game, float dt)
		{
			if (s_owner != game || !game.running || game.paused || dt <= 0f || game.player == null) return;
			float playerZ = game.player.position.z;
			TickPuddles(game, dt, playerZ);
			TickBalls(game, dt, playerZ);
			TickSpin(game, dt);
			if (s_noteTime > 0f)
			{
				s_noteTime = Mathf.Max(0f, s_noteTime - dt);
				if (s_note != null)
				{
					s_note.color = InfernoLobby.Hex(s_noteColor, Mathf.Min(1f, s_noteTime / 0.35f));
					if (s_noteTime == 0f) s_note.gameObject.SetActive(false);
				}
			}
		}
	}
}
