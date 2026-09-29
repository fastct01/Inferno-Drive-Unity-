using System.Collections.Generic;
using UnityEngine;

// [user request 2026-09-26: night drive] Complications for the NEON NIGHT map (Profile.NightHazards), on top
// of the shipped traffic, ramps and the ported puddles / wrecking balls:
//   BLACKOUT   a stretch of road where the city's power fails - street lamps, neon fences, lane lines and the
//              building windows flicker out, the fog closes in and the road is lit by the cars alone; the
//              power flickers back at the far end (InfernoMaps.SetBlackout drives the darkening).
//   ROADBLOCK  a row of road-works barriers (striped planks on legs, cones, amber blinkers) or a police
//              checkpoint (patrol cars with red / blue light bars) closing two or three adjacent lanes; the
//              open lanes are always at least one. Hitting a barrier or a car is a heavy frontal impact
//              (a whole heart, like the wrecking ball); cones just get knocked away.
//   DEBRIS     three or four tyre stacks / crates scattered over the lanes ahead; each costs half a heart and
//              a lot of speed, a jump clears them.
// Traffic steers out of the closed lanes ahead of a row (QueueTrafficLaneChange) and never spawns into one
// (TrafficSpawnBlockedByRamp asks LaneBlocked). The rules are pure statics (EditMode-tested); the scene
// objects follow InfernoHazards' lifecycle (Reset per run, Hide on menu / game over, Dispose on destroy).
public sealed partial class ShadowRunPrototype
{
	internal static partial class InfernoNightHazards
	{
		// ---- rules --------------------------------------------------------------------------------------
		public const float SpawnAhead = 175f;                      // where a row appears (metres ahead of the car), like the gantry
		public const float BlockFirstMin = 450f, BlockFirstMax = 700f, BlockGapMin = 550f, BlockGapMax = 900f;
		public const float BlackoutFirstMin = 700f, BlackoutFirstMax = 1000f, BlackoutGapMin = 900f, BlackoutGapMax = 1400f;
		public const float BlackoutLengthMin = 160f, BlackoutLengthMax = 240f, BlackoutRamp = 28f;
		public const float BlockKeepClearOfBlackout = 260f;        // no roadblock this close to a blackout's start
		public const float CarHalfWidth = 1.05f, CarHalfLength = 2.4f;
		public const float PlayerZ = InfernoHazards.PlayerZ;

		public enum Block { Works, Police, Debris }

		// A WORKS / POLICE row closes `closed` adjacent lanes; `roll` (0..1) picks where the span starts. At least one
		// lane stays open whatever the roll.
		public static int ClosedStart(int laneCount, int closed, float roll)
		{
			closed = Mathf.Clamp(closed, 1, Mathf.Max(1, laneCount - 1));
			int options = laneCount - closed + 1;
			return Mathf.Clamp((int)(Mathf.Clamp01(roll) * options), 0, options - 1);
		}

		public static bool LaneClosed(int lane, int start, int closed) { return lane >= start && lane < start + closed; }

		// The open lane nearest to `lane` (ties go toward the road's middle); `lane` itself when it is open.
		public static int NearestOpenLane(int lane, int start, int closed, int laneCount)
		{
			if (!LaneClosed(lane, start, closed)) return lane;
			int best = -1; float bestScore = float.MaxValue;
			for (int candidate = 0; candidate < laneCount; candidate++)
			{
				if (LaneClosed(candidate, start, closed)) continue;
				float score = Mathf.Abs(candidate - lane) + 0.1f * Mathf.Abs(candidate - (laneCount - 1) * 0.5f);
				if (score < bestScore) { bestScore = score; best = candidate; }
			}
			return best < 0 ? lane : best;
		}

		// How dark a blackout is `into` metres into a zone `length` long: ramps up over BlackoutRamp, holds, ramps down.
		public static float Darkness(float into, float length)
		{
			if (into <= 0f || into >= length || length <= 0f) return 0f;
			float up = Mathf.Clamp01(into / BlackoutRamp);
			float down = Mathf.Clamp01((length - into) / BlackoutRamp);
			return Mathf.Min(up, down);
		}

		// The lamps stutter while the power fails and comes back: a flicker only inside the ramps, never in the dark.
		public static float Flicker(float darkness, float time)
		{
			if (darkness <= 0f || darkness >= 1f) return Mathf.Clamp01(darkness);
			float f = 0.5f + 0.5f * Mathf.Sin(time * 37f) * Mathf.Sin(time * 23f + 1.3f);
			return Mathf.Clamp01(darkness * (0.55f + 0.45f * f));
		}

		// Axis-aligned box test between a piece (centre, half sizes) and the car (centre, the constants above).
		public static bool Hits(float pieceX, float halfWidth, float pieceZ, float halfLength, float carX, float carZ)
		{
			return Mathf.Abs(pieceX - carX) < halfWidth + CarHalfWidth && Mathf.Abs(pieceZ - carZ) < halfLength + CarHalfLength;
		}

		// ---- scene ---------------------------------------------------------------------------------------
		private const int RowCount = 2, MaxPieces = 10;

		private struct Piece
		{
			public Transform T;
			public float HalfW, HalfL, Height;
			public bool Solid;      // barrier plank / police car: a whole heart; else debris (half) or cone (nothing)
			public bool Cone;
			public bool Hit;
		}

		private struct Row
		{
			public Transform Root;
			public Piece[] Pieces;
			public int Count;
			public Block Kind;
			public bool Active;
			public int Start, Closed;
			public float Blink;
		}

		private static ShadowRunPrototype s_owner;
		private static readonly Row[] s_rows = new Row[RowCount];
		private static float s_nextBlock, s_nextBlackout;
		private static bool s_blackoutActive, s_blackoutAnnounced;
		private static float s_blackoutStart, s_blackoutLength;
		private static Material s_stripeRed, s_stripeWhite, s_leg, s_coneMat, s_coneBand, s_amber, s_policeBody, s_policeGlass, s_lightRed, s_lightBlue, s_tyre, s_crate, s_reflect;
		private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");

		private static bool Enabled(ShadowRunPrototype game)
		{
			return game != null && InfernoMaps.Current >= 0 && InfernoMaps.HasNightHazards(InfernoMaps.Current);
		}

		private static void EnsureMaterials(ShadowRunPrototype game)
		{
			if (s_stripeRed != null) return;
			Color? none = null;
			s_stripeRed = game.Mat(new Color(0.85f, 0.08f, 0.08f, 1f), new Color(0.6f, 0.03f, 0.03f));
			s_stripeWhite = game.Mat(new Color(0.92f, 0.92f, 0.9f, 1f), new Color(0.45f, 0.45f, 0.42f));
			s_leg = game.Mat(new Color(0.22f, 0.23f, 0.26f, 1f), none);
			s_coneMat = game.Mat(new Color(1f, 0.42f, 0.05f, 1f), new Color(0.5f, 0.16f, 0f));
			s_coneBand = game.Mat(new Color(0.95f, 0.95f, 0.95f, 1f), new Color(0.5f, 0.5f, 0.5f));
			s_amber = game.Mat(new Color(1f, 0.6f, 0.1f, 1f), new Color(1f, 0.55f, 0.08f) * 4f);
			s_policeBody = game.Mat(new Color(0.06f, 0.08f, 0.16f, 1f), none);
			s_policeGlass = game.Mat(new Color(0.3f, 0.36f, 0.5f, 1f), new Color(0.05f, 0.08f, 0.14f));
			s_lightRed = game.Mat(new Color(1f, 0.1f, 0.1f, 1f), new Color(1f, 0.05f, 0.05f) * 5f);
			s_lightBlue = game.Mat(new Color(0.2f, 0.4f, 1f, 1f), new Color(0.15f, 0.35f, 1f) * 5f);
			s_tyre = game.Mat(new Color(0.07f, 0.07f, 0.08f, 1f), none);
			s_crate = game.Mat(new Color(0.42f, 0.28f, 0.14f, 1f), none);
			s_reflect = game.Mat(new Color(0.95f, 0.95f, 0.9f, 1f), new Color(0.9f, 0.85f, 0.6f) * 1.6f);   // reflective tape: debris reads at night
		}

		private static void AddPiece(ref Row row, Transform t, float halfW, float halfL, float height, bool solid, bool cone)
		{
			if (row.Count >= row.Pieces.Length) return;
			row.Pieces[row.Count++] = new Piece { T = t, HalfW = halfW, HalfL = halfL, Height = height, Solid = solid, Cone = cone };
		}

		private static void ClearRow(ref Row row)
		{
			if (row.Root == null) return;
			for (int i = row.Root.childCount - 1; i >= 0; i--) Object.Destroy(row.Root.GetChild(i).gameObject);
			row.Count = 0;
		}

		// A road-works barrier in one lane: two legs, a red / white striped plank, an amber blinker on top and two
		// cones a few metres before it.
		private static void BuildBarrier(ShadowRunPrototype game, ref Row row, float x)
		{
			Transform barrier = new GameObject("Barrier").transform;
			barrier.SetParent(row.Root, false);
			barrier.localPosition = new Vector3(x, 0f, 0f);
			game.AddCube(barrier, "LegL", new Vector3(-1.05f, 0.5f, 0f), new Vector3(0.12f, 1.0f, 0.12f), s_leg);
			game.AddCube(barrier, "LegR", new Vector3(1.05f, 0.5f, 0f), new Vector3(0.12f, 1.0f, 0.12f), s_leg);
			game.AddCube(barrier, "FootL", new Vector3(-1.05f, 0.04f, 0f), new Vector3(0.14f, 0.08f, 0.7f), s_leg);
			game.AddCube(barrier, "FootR", new Vector3(1.05f, 0.04f, 0f), new Vector3(0.14f, 0.08f, 0.7f), s_leg);
			for (int i = 0; i < 5; i++)
				game.AddCube(barrier, "Stripe" + i, new Vector3(-1.04f + i * 0.52f, 0.92f, 0f), new Vector3(0.52f, 0.36f, 0.1f), (i & 1) == 0 ? s_stripeRed : s_stripeWhite);
			game.AddCube(barrier, "Blinker", new Vector3(0f, 1.26f, 0f), new Vector3(0.3f, 0.26f, 0.3f), s_amber);
			AddPiece(ref row, barrier, 1.35f, 0.35f, 1.4f, true, false);
			for (int side = -1; side <= 1; side += 2)
			{
				Transform cone = new GameObject("Cone").transform;
				cone.SetParent(row.Root, false);
				cone.localPosition = new Vector3(x + side * 0.85f, 0f, 3.2f + (side < 0 ? 0f : 0.8f));
				game.AddCube(cone, "Base", new Vector3(0f, 0.05f, 0f), new Vector3(0.5f, 0.1f, 0.5f), s_coneMat);
				game.AddCube(cone, "Body", new Vector3(0f, 0.36f, 0f), new Vector3(0.3f, 0.62f, 0.3f), s_coneMat);
				game.AddCube(cone, "Band", new Vector3(0f, 0.42f, 0f), new Vector3(0.32f, 0.12f, 0.32f), s_coneBand);
				game.AddCube(cone, "Tip", new Vector3(0f, 0.7f, 0f), new Vector3(0.18f, 0.12f, 0.18f), s_coneMat);
				AddPiece(ref row, cone, 0.28f, 0.28f, 0.75f, false, true);
			}
		}

		// A patrol car parked across a lane: body, cabin, wheels and a light bar whose halves blink red / blue.
		private static void BuildPatrolCar(ShadowRunPrototype game, ref Row row, float x, float yaw)
		{
			Transform car = new GameObject("PatrolCar").transform;
			car.SetParent(row.Root, false);
			car.localPosition = new Vector3(x, 0f, 0f);
			car.localRotation = Quaternion.Euler(0f, yaw, 0f);
			game.AddCube(car, "Body", new Vector3(0f, 0.58f, 0f), new Vector3(1.9f, 0.8f, 4.3f), s_policeBody);
			game.AddCube(car, "Cabin", new Vector3(0f, 1.2f, -0.15f), new Vector3(1.6f, 0.5f, 2.0f), s_policeGlass);
			game.AddCube(car, "Bar", new Vector3(0f, 1.52f, -0.15f), new Vector3(1.5f, 0.14f, 0.36f), s_leg);
			game.AddCube(car, "LightRed", new Vector3(-0.42f, 1.62f, -0.15f), new Vector3(0.6f, 0.16f, 0.34f), s_lightRed);
			game.AddCube(car, "LightBlue", new Vector3(0.42f, 1.62f, -0.15f), new Vector3(0.6f, 0.16f, 0.34f), s_lightBlue);
			for (int i = 0; i < 4; i++)
				game.AddCube(car, "Wheel" + i, new Vector3((i & 1) == 0 ? -0.95f : 0.95f, 0.36f, i < 2 ? 1.4f : -1.4f), new Vector3(0.28f, 0.72f, 0.72f), s_tyre);
			game.AddCube(car, "Stripe", new Vector3(0f, 0.6f, 0f), new Vector3(1.94f, 0.16f, 4.0f), s_stripeWhite);
			AddPiece(ref row, car, 1.05f, 2.2f, 1.7f, true, false);
		}

		private static void BuildDebris(ShadowRunPrototype game, ref Row row, float x, float dz, bool crate)
		{
			Transform piece = new GameObject(crate ? "Crate" : "Tyres").transform;
			piece.SetParent(row.Root, false);
			piece.localPosition = new Vector3(x, 0f, dz);
			piece.localRotation = Quaternion.Euler(0f, Random.Range(-25f, 25f), 0f);
			if (crate)
			{
				game.AddCube(piece, "Box", new Vector3(0f, 0.5f, 0f), new Vector3(1.1f, 1.0f, 1.1f), s_crate);
				game.AddCube(piece, "BandA", new Vector3(0f, 0.5f, 0f), new Vector3(1.14f, 0.12f, 1.14f), s_reflect);
				game.AddCube(piece, "BandB", new Vector3(0f, 0.5f, 0f), new Vector3(0.12f, 1.04f, 1.14f), s_leg);
			}
			else
			{
				for (int i = 0; i < 3; i++)
					game.AddCube(piece, "Tyre" + i, new Vector3(i == 1 ? 0.06f : -0.04f, 0.16f + i * 0.3f, i == 2 ? 0.05f : 0f), new Vector3(0.9f, 0.3f, 0.9f), s_tyre);
				game.AddCube(piece, "Hub", new Vector3(0f, 0.75f, 0f), new Vector3(0.36f, 0.12f, 0.36f), s_leg);
				game.AddCube(piece, "Tape", new Vector3(0.06f, 0.46f, 0f), new Vector3(0.94f, 0.08f, 0.94f), s_reflect);
			}
			AddPiece(ref row, piece, 0.55f, 0.55f, 0.95f, false, false);
		}

		private static bool LanesClear(ShadowRunPrototype game, int start, int closed, float z)
		{
			if (game.lanes == null) return false;
			for (int lane = start; lane < start + closed; lane++)
			{
				if (!InfernoDriving.ClearLane(game, lane, game.lanes[lane], z)) return false;
				for (float dz = -30f; dz <= 30f; dz += 15f) if (game.TrafficSpawnBlockedByRamp(lane, z + dz)) return false;
			}
			return !InfernoHazards.NearPuddle(z, 40f) && !InfernoHazards.NearBall(z, 60f);
		}

		private static bool SpawnRow(ShadowRunPrototype game, float playerZ)
		{
			int slot = -1;
			for (int i = 0; i < s_rows.Length; i++) if (!s_rows[i].Active) { slot = i; break; }
			if (slot < 0 || game.lanes == null || game.lanes.Length < 2) return false;
			float z = playerZ - SpawnAhead;
			int laneCount = game.lanes.Length;
			float roll = Random.value;
			Block kind = roll < 0.42f ? Block.Works : roll < 0.72f ? Block.Police : Block.Debris;
			if (s_debugKind >= 0) kind = (Block)s_debugKind;
			ref Row row = ref s_rows[slot];
			if (row.Root == null)
			{
				row.Root = new GameObject("NightRoadblock").transform;
				row.Root.SetParent(game.transform, false);
				row.Pieces = new Piece[MaxPieces];
			}
			EnsureMaterials(game);
			ClearRow(ref row);
			row.Kind = kind;
			row.Blink = 0f;
			if (kind == Block.Debris)
			{
				// three or four pieces over distinct lanes, staggered along the road, never every lane
				int count = Random.Range(3, 5);
				List<int> lanes = new List<int>();
				for (int lane = 0; lane < laneCount; lane++) lanes.Add(lane);
				int placed = 0;
				for (int attempt = 0; attempt < laneCount && placed < count && lanes.Count > 1; attempt++)
				{
					int pick = lanes[Random.Range(0, lanes.Count)];
					lanes.Remove(pick);
					if (pick == s_debugAvoid) continue;
					float dz = Random.Range(-7f, 7f);
					if (s_debugKind < 0 && (!InfernoDriving.ClearLane(game, pick, game.lanes[pick], z + dz) || game.TrafficSpawnBlockedByRamp(pick, z + dz))) continue;
					BuildDebris(game, ref row, game.lanes[pick] + Random.Range(-0.5f, 0.5f), dz, Random.value < 0.5f);
					placed++;
				}
				if (placed == 0) return false;
				row.Start = 0; row.Closed = 0;   // debris closes nothing for the traffic: cars weave through it
			}
			else
			{
				int closed = kind == Block.Police ? 3 : Random.Range(2, 4);
				closed = Mathf.Min(closed, laneCount - 1);
				if (s_debugAvoid >= 0) closed = Mathf.Min(closed, 2);   // editor hook: a two-lane span can always miss the player's lane
				int start = ClosedStart(laneCount, closed, Random.value);
				for (int retry = 0; s_debugAvoid >= 0 && retry < 12 && LaneClosed(s_debugAvoid, start, closed); retry++) start = ClosedStart(laneCount, closed, Random.value);
				if (s_debugKind < 0 && !LanesClear(game, start, closed, z))   // (the editor hook places the row regardless of traffic)
				{
					// try the mirrored span before giving up this attempt
					start = laneCount - closed - start;
					if (!LanesClear(game, start, closed, z)) return false;
				}
				row.Start = start; row.Closed = closed;
				for (int lane = start; lane < start + closed; lane++)
				{
					if (kind == Block.Works) BuildBarrier(game, ref row, game.lanes[lane]);
					else BuildPatrolCar(game, ref row, game.lanes[lane], (lane - start) % 2 == 0 ? 28f : -24f);
				}
			}
			row.Root.position = new Vector3(0f, 0f, z);
			row.Active = true;
			row.Root.gameObject.SetActive(true);
			return true;
		}

		// Is `lane` closed by an active row within reach of z? TrafficSpawnBlockedByRamp asks, so traffic never spawns
		// into a barrier and the ramp / gantry placement keeps clear of it.
		public static bool LaneBlocked(int lane, float z)
		{
			for (int i = 0; i < s_rows.Length; i++)
			{
				if (!s_rows[i].Active || s_rows[i].Root == null || s_rows[i].Closed == 0) continue;
				if (!LaneClosed(lane, s_rows[i].Start, s_rows[i].Closed)) continue;
				float gap = z - s_rows[i].Root.position.z;
				if (gap >= -30f && gap <= 90f) return true;
			}
			return false;
		}

		// Traffic in a closed lane that is about to reach a row moves to the nearest open lane (one lane per
		// request, like the high-beam yield); the AI's own lane-change handles the steering.
		private static void SteerTraffic(ShadowRunPrototype game, ref Row row)
		{
			if (row.Closed == 0 || game.pickups == null) return;
			float rowZ = row.Root.position.z;
			for (int i = 0; i < game.pickups.Count; i++)
			{
				Pickup item = game.pickups[i];
				if (item.Transform == null || !item.Transform.gameObject.activeInHierarchy || item.PendingLane >= 0) continue;
				float ahead = item.Transform.position.z - rowZ;   // traffic approaches a row from larger z
				if (ahead < 4f || ahead > 75f) continue;
				int lane = game.ClosestLane(item.Transform.position.x);
				if (!LaneClosed(lane, row.Start, row.Closed)) continue;
				int target = NearestOpenLane(lane, row.Start, row.Closed, game.lanes.Length);
				int step = lane + (target > lane ? 1 : -1);
				game.QueueTrafficLaneChange(ref item, step, item.Transform.position.x);
				item.LaneChangeCooldown = 0f;
				game.pickups[i] = item;
			}
		}

		private static void Crash(ShadowRunPrototype game, ref Row row, ref Piece piece, Vector3 world)
		{
			piece.Hit = true;
			int direction = game.playerX >= world.x ? 1 : -1;
			game.velocityX = direction * 9.0f;
			game.impactYaw = direction * 8.0f;
			game.impactRoll = direction * 2.0f;
			game.impactShake = 1f;
			game.tractionLoss = 1f;
			game.speed = InfernoDrivingRules.Impact(game.speed, game.speed * 0.45f);
			game.SpawnTrafficCollisionExplosion(false, world, -direction);
			game.TriggerVehicleEngineImpact(false);
			game.PlayCollisionHaptic(false);
			if (game.invuln <= 0f)
			{
				game.lifeHalfUnits = Mathf.Max(0, game.lifeHalfUnits - 2);
				game.invuln = 1.5f;
				game.combo = 0;
			}
			if (row.Kind == Block.Works && piece.T != null)
			{
				// the barrier folds over and slides aside
				piece.T.localRotation = Quaternion.Euler(-72f, 0f, -direction * 18f);
				piece.T.localPosition += new Vector3(-direction * 0.9f, 0f, -0.6f);
			}
			InfernoHazards.ShowNote(game, row.Kind == Block.Police ? "CHECKPOINT!" : "ROADBLOCK!", InfernoLobby.Blossom);
		}

		private static void Bump(ShadowRunPrototype game, ref Row row, ref Piece piece, Vector3 world)
		{
			piece.Hit = true;
			int direction = game.playerX >= world.x ? 1 : -1;
			if (piece.Cone)
			{
				game.impactShake = Mathf.Max(game.impactShake, 0.22f);
				game.impactYaw += direction * 1.5f;
				game.speed = InfernoDrivingRules.Impact(game.speed, game.speed * 0.97f);
				game.PlayImpactHaptic(1);
				if (piece.T != null) piece.T.gameObject.SetActive(false);   // knocked away
				return;
			}
			game.velocityX += direction * 3.5f;
			game.impactYaw += direction * 5.0f;
			game.impactShake = Mathf.Max(game.impactShake, 0.6f);
			game.tractionLoss = Mathf.Max(game.tractionLoss, 0.5f);
			game.speed = InfernoDrivingRules.Impact(game.speed, game.speed * 0.72f);
			game.SpawnTrafficCollisionExplosion(true, world, -direction);
			game.TriggerVehicleEngineImpact(true);
			game.PlayCollisionHaptic(true);
			if (game.invuln <= 0f)
			{
				game.lifeHalfUnits = Mathf.Max(0, game.lifeHalfUnits - 1);
				game.invuln = 0.9f;
				game.combo = 0;
			}
			if (piece.T != null) piece.T.gameObject.SetActive(false);
			InfernoHazards.ShowNote(game, "DEBRIS!", InfernoLobby.Lavender);
		}

		private static void TickRows(ShadowRunPrototype game, float dt, float playerZ)
		{
			for (int i = 0; i < s_rows.Length; i++)
			{
				ref Row row = ref s_rows[i];
				if (!row.Active) continue;
				Vector3 position = row.Root.position;
				position.z += game.speed * dt;
				row.Root.position = position;
				if (position.z > playerZ + 30f) { row.Active = false; row.Root.gameObject.SetActive(false); continue; }
				// blinkers: amber pulse on the works, alternating red / blue on the patrol cars
				row.Blink += dt;
				if (row.Kind != Block.Debris)
				{
					float phase = row.Blink * (row.Kind == Block.Police ? 6f : 3f);
					bool on = Mathf.Sin(phase * Mathf.PI) > 0f;
					if (row.Kind == Block.Works) s_amber.SetColor(EmissionId, new Color(1f, 0.55f, 0.08f) * (on ? 4.5f : 0.6f));
					else
					{
						s_lightRed.SetColor(EmissionId, new Color(1f, 0.05f, 0.05f) * (on ? 6f : 0.3f));
						s_lightBlue.SetColor(EmissionId, new Color(0.15f, 0.35f, 1f) * (on ? 0.3f : 6f));
					}
				}
				SteerTraffic(game, ref row);
				if (game.lifeHalfUnits <= 0) continue;
				for (int k = 0; k < row.Count; k++)
				{
					ref Piece piece = ref row.Pieces[k];
					if (piece.Hit || piece.T == null || !piece.T.gameObject.activeSelf) continue;
					Vector3 world = piece.T.position;
					if (!Hits(world.x, piece.HalfW, world.z, piece.HalfL, game.playerX, PlayerZ)) continue;
					if (game.playerAirborne && game.playerJumpHeight > piece.Height) continue;   // cleared it in a jump
					if (piece.Solid) Crash(game, ref row, ref piece, world);
					else Bump(game, ref row, ref piece, world);
				}
			}
			if (game.lifeHalfUnits > 0 && game.distance >= s_nextBlock)
			{
				bool nearBlackout = s_blackoutActive || (s_nextBlackout - game.distance) < BlockKeepClearOfBlackout;
				bool spawned = !nearBlackout && SpawnRow(game, playerZ);
				s_nextBlock = game.distance + (spawned ? Random.Range(BlockGapMin, BlockGapMax) : 60f);
			}
		}

		private static void TickBlackout(ShadowRunPrototype game, float dt)
		{
			if (!s_blackoutActive)
			{
				if (game.lifeHalfUnits > 0 && game.distance >= s_nextBlackout && InfernoMaps.DayBlend < 0.5f)
				{
					s_blackoutActive = true;
					s_blackoutAnnounced = false;
					s_blackoutStart = game.distance + 40f;   // the lights ahead start to stutter a moment before the car is in the dark
					s_blackoutLength = Random.Range(BlackoutLengthMin, BlackoutLengthMax);
				}
				else
				{
					if (InfernoMaps.Blackout > 0f) InfernoMaps.SetBlackout(game, 0f);
					return;
				}
			}
			float into = game.distance - s_blackoutStart;
			float darkness = Flicker(Darkness(into, s_blackoutLength), Time.time);
			InfernoMaps.SetBlackout(game, darkness);
			if (!s_blackoutAnnounced && darkness > 0.5f)
			{
				s_blackoutAnnounced = true;
				InfernoHazards.ShowNote(game, "POWER OUT!", 0xFFB347);
			}
			if (into >= s_blackoutLength)
			{
				s_blackoutActive = false;
				InfernoMaps.SetBlackout(game, 0f);
				s_nextBlackout = game.distance + Random.Range(BlackoutGapMin, BlackoutGapMax);
			}
		}

		// ---- lifecycle (mirrors InfernoHazards) ----------------------------------------------------------
		public static void Hide(ShadowRunPrototype game)
		{
			if (s_owner != game) return;
			for (int i = 0; i < s_rows.Length; i++) { s_rows[i].Active = false; if (s_rows[i].Root != null) s_rows[i].Root.gameObject.SetActive(false); }
			s_blackoutActive = false;
			InfernoMaps.SetBlackout(game, 0f);
		}

		public static void Dispose(ShadowRunPrototype game)
		{
			if (s_owner != game) return;
			for (int i = 0; i < s_rows.Length; i++)
			{
				if (s_rows[i].Root != null) Object.Destroy(s_rows[i].Root.gameObject);
				s_rows[i] = default;
			}
			Material[] materials = { s_stripeRed, s_stripeWhite, s_leg, s_coneMat, s_coneBand, s_amber, s_policeBody, s_policeGlass, s_lightRed, s_lightBlue, s_tyre, s_crate, s_reflect };
			for (int i = 0; i < materials.Length; i++) if (materials[i] != null) Object.Destroy(materials[i]);
			s_stripeRed = s_stripeWhite = s_leg = s_coneMat = s_coneBand = s_amber = s_policeBody = s_policeGlass = s_lightRed = s_lightBlue = s_tyre = s_crate = s_reflect = null;
			s_owner = null;
		}

		public static void Reset(ShadowRunPrototype game)
		{
			if (s_owner != null && s_owner != game) Dispose(s_owner);
			s_owner = game;
			Hide(game);
			s_nextBlock = game.distance + Random.Range(BlockFirstMin, BlockFirstMax);
			s_nextBlackout = game.distance + Random.Range(BlackoutFirstMin, BlackoutFirstMax);
		}

		// Editor / test hooks: force a row of a kind (0 works, 1 police, 2 debris) or the next blackout right away.
		public static bool DebugSpawnRow(ShadowRunPrototype game, int kind, bool avoidPlayerLane)
		{
			if (s_owner != game || game.player == null) return false;
			s_debugKind = Mathf.Clamp(kind, 0, 2);
			s_debugAvoid = avoidPlayerLane && game.lanes != null ? game.ClosestLane(game.playerX) : -1;
			bool ok = SpawnRow(game, game.player.position.z);
			s_debugKind = -1; s_debugAvoid = -1;
			return ok;
		}
		public static void DebugStartBlackout(ShadowRunPrototype game) { if (s_owner == game) s_nextBlackout = game.distance; }
		private static int s_debugKind = -1, s_debugAvoid = -1;

		// End of UpdateWorld, after InfernoHazards.Tick and before InfernoMaps.Tick (which applies the blackout).
		public static void Tick(ShadowRunPrototype game, float dt)
		{
			if (s_owner != game || !game.running || game.paused || dt <= 0f || game.player == null) return;
			if (!Enabled(game)) { if (InfernoMaps.Blackout > 0f) InfernoMaps.SetBlackout(game, 0f); return; }
			float playerZ = game.player.position.z;
			TickRows(game, dt, playerZ);
			TickBlackout(game, dt);
		}
	}

#if UNITY_EDITOR
	/* [user request 2026-09-26: night drive] */ // Editor-only: SendMessage("DebugNightRow", kind) / SendMessage("DebugNightBlackout") from a RunCommand.
	// code: 0 works, 1 police, 2 debris; +10 = keep the player's lane open. DebugInvulnerable(seconds) keeps the hearts for a scripted drive.
	private void DebugNightRow(int code) { InfernoNightHazards.DebugSpawnRow(this, code % 10, code >= 10); }
	private void DebugNightBlackout() { InfernoNightHazards.DebugStartBlackout(this); }
	private void DebugInvulnerable(float seconds) { invuln = Mathf.Max(invuln, seconds); }
#endif
}
