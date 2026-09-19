using ShadowRun.Services;
using UnityEngine;
using UnityEngine.UI;

// Port of Il2CppOutputProject/Source/il2cppOutput/InfernoStyle.inc — car customisation,
// hand-maintained: TIRES (rim and tyre colours), FLAGS (a pennant on a pole at the rear)
// and TRAILS (neon light streaks behind the car during a run). Bought with crystals and
// picked in a STYLE sheet on the garage page; the choice applies to every car. Reuses the
// lobby, garage and driving helpers; wired in at the hook points of
// tools/apply_lobby_patches.py (patch_style). See docs/lobby-implementation.md,
// "Customisation".
public sealed partial class ShadowRunPrototype
{
	private static partial class InfernoStyle
	{
		// ---- catalogue ------------------------------------------------------------------------
		private const int Tires = 0, Flags = 1, Trails = 2, CategoryCount = 3;
		private static readonly string[] CategoryNames = { "TIRES", "FLAGS", "TRAILS" };
		private static readonly string[] CategoryKeys = { "Tires", "Flags", "Trails" };

		private sealed class Item
		{
			public readonly string name;
			public readonly int price;
			public readonly float[] a;
			public readonly float[] b;
			public readonly int pattern;

			public Item(string name, int price, float[] a, float[] b, int pattern)
			{
				this.name = name; this.price = price; this.a = a; this.b = b; this.pattern = pattern;
			}
		}

		// tires: a = rim colour, b = tyre colour
		private static readonly Item[] TireItems =
		{
			new Item("STOCK",       0,  new[] {0f, 0f, 0f},          new[] {0f, 0f, 0f},          0),
			new Item("CHROME",      40, new[] {0.85f, 0.87f, 0.92f}, new[] {0.05f, 0.05f, 0.06f}, 0),
			new Item("GOLD",        60, new[] {0.92f, 0.72f, 0.22f}, new[] {0.05f, 0.05f, 0.06f}, 0),
			new Item("BLACKOUT",    60, new[] {0.06f, 0.06f, 0.07f}, new[] {0.03f, 0.03f, 0.03f}, 0),
			new Item("NEON VIOLET", 90, new[] {0.72f, 0.25f, 1.00f}, new[] {0.30f, 0.08f, 0.45f}, 0),
			new Item("NEON CYAN",   90, new[] {0.20f, 0.90f, 1.00f}, new[] {0.05f, 0.30f, 0.40f}, 0),
			new Item("CRIMSON",     90, new[] {0.90f, 0.08f, 0.12f}, new[] {0.30f, 0.03f, 0.04f}, 0),
			new Item("WHITE WALL",  70, new[] {0.22f, 0.22f, 0.25f}, new[] {0.86f, 0.86f, 0.80f}, 0),
		};
		// flags: pattern (see FlagPixel), a / b = the two colours
		private static readonly Item[] FlagItems =
		{
			new Item("NONE",      0,  new[] {0f, 0f, 0f},             new[] {0f, 0f, 0f},             0),
			new Item("CHECKERED", 50, new[] {0.05f, 0.05f, 0.06f},  new[] {0.96f, 0.96f, 0.96f}, 1),
			new Item("STRIPES",   50, new[] {0.85f, 0.08f, 0.10f},  new[] {0.96f, 0.96f, 0.96f}, 2),
			new Item("NEON",      80, new[] {0.40f, 0.10f, 0.85f},  new[] {0.98f, 0.50f, 1.00f}, 3),
			new Item("PIRATE",    90, new[] {0.05f, 0.05f, 0.06f},  new[] {0.96f, 0.96f, 0.96f}, 4),
			new Item("SUNBURST",  70, new[] {0.95f, 0.45f, 0.05f},  new[] {1.00f, 0.90f, 0.25f}, 5),
			new Item("BOLT",      70, new[] {0.08f, 0.25f, 0.85f},  new[] {1.00f, 0.90f, 0.20f}, 6),
		};
		// trails: a = glow colour
		private static readonly Item[] TrailItems =
		{
			new Item("NONE",        0,  new[] {0f, 0f, 0f},             new[] {0f, 0f, 0f}, 0),
			new Item("VIOLET NEON", 50, new[] {0.58f, 0.20f, 1.00f},  new[] {0f, 0f, 0f}, 0),
			new Item("CYAN NEON",   50, new[] {0.20f, 0.90f, 1.00f},  new[] {0f, 0f, 0f}, 0),
			new Item("FIRE",        70, new[] {1.00f, 0.45f, 0.10f},  new[] {0f, 0f, 0f}, 0),
			new Item("EMERALD",     70, new[] {0.20f, 1.00f, 0.50f},  new[] {0f, 0f, 0f}, 0),
			new Item("MAGENTA",     70, new[] {1.00f, 0.25f, 0.75f},  new[] {0f, 0f, 0f}, 0),
			new Item("GOLD",        90, new[] {1.00f, 0.85f, 0.30f},  new[] {0f, 0f, 0f}, 0),
		};

		private static Item[] Items(int category) { return category == Tires ? TireItems : category == Flags ? FlagItems : TrailItems; }

		private static int Count(int category)
		{
			return category == Tires ? TireItems.Length
				: category == Flags ? FlagItems.Length
				: TrailItems.Length;
		}

		// ---- persistence (PlayerPrefs; item 0 of every category is free and owned) ----------------
		private static string Key(int category, string what, int index = -1)
		{
			if (index >= 0) return "InfernoDrive.Style." + CategoryKeys[category] + "." + index + "." + what;
			return "InfernoDrive.Style." + CategoryKeys[category] + "." + what;
		}

		private static bool Owned(int category, int index)
		{
			return index == 0 || PlayerPrefs.GetInt(Key(category, "Owned", index), 0) != 0;
		}

		private static int Selected(int category)
		{
			int index = PlayerPrefs.GetInt(Key(category, "Selected"), 0);
			if (index < 0 || index >= Count(category) || !Owned(category, index)) return 0;
			return index;
		}

		private static void Select(int category, int index)
		{
			PlayerPrefs.SetInt(Key(category, "Selected"), index);
			PlayerPrefs.Save();
		}

		private static void Own(int category, int index)
		{
			PlayerPrefs.SetInt(Key(category, "Owned", index), 1);
			PlayerPrefs.Save();
		}

		// ---- flag textures -----------------------------------------------------------------------
		private const int FlagTexW = 96, FlagTexH = 64;
		private static readonly Texture2D[] s_flagTextures = new Texture2D[8];
		private static readonly Sprite[] s_flagSprites = new Sprite[8];
		private static int s_flagBeingDrawn;

		private static void Mix(ref Color32 px, float[] c, float t, float[] d)
		{
			InfernoHazards.Set(ref px, (int)(255f * (c[0] + (d[0] - c[0]) * t)), (int)(255f * (c[1] + (d[1] - c[1]) * t)), (int)(255f * (c[2] + (d[2] - c[2]) * t)), 255);
		}

		// u: 0 at the pole .. 1 at the free edge; v: 0 bottom .. 1 top
		private static void FlagPixel(ref Color32 px, float u, float v)
		{
			Item item = FlagItems[s_flagBeingDrawn];
			u = 0.5f - u * 0.5f; v = v * 0.5f + 0.5f;   // the pole is at the texture's right edge (see DressFlag)
			float[] a = item.a; float[] b = item.b;
			float t = 0f;
			switch (item.pattern)
			{
				case 1: t = (((int)(u * 6f) + (int)(v * 4f)) & 1) != 0 ? 1.0f : 0.0f; break;                           // checkered
				case 2: t = ((int)(v * 5f) & 1) != 0 ? 1.0f : 0.0f; break;                                            // stripes
				case 3:                                                                                               // neon: gradient + diamond
				{
					float d = Mathf.Abs(u - 0.5f) * 1.6f + Mathf.Abs(v - 0.5f) * 1.1f;
					t = d < 0.34f ? 1.0f : Mathf.Max(0.0f, 0.35f * (1f - u) + 0.15f * v);
					break;
				}
				case 4:                                                                                               // pirate: skull-ish disc + crossed bones
				{
					float dx = (u - 0.5f) * 1.5f, dy = (v - 0.58f);
					float disc = Mathf.Sqrt(dx * dx + dy * dy);
					bool eye = (Mathf.Abs(Mathf.Abs(dx) - 0.10f) < 0.05f && Mathf.Abs(dy - 0.02f) < 0.06f);
					bool bones = v < 0.30f && (Mathf.Abs((u - 0.5f) * 1.5f - (v - 0.15f) * 2.2f) < 0.045f || Mathf.Abs((u - 0.5f) * 1.5f + (v - 0.15f) * 2.2f) < 0.045f);
					t = ((disc < 0.22f && !eye) || bones) ? 1.0f : 0.0f;
					break;
				}
				case 5:                                                                                               // sunburst rays
				{
					float ang = Mathf.Atan2(v - 0.5f, u);
					t = ((int)((ang + 3.1416f) / 0.3927f) & 1) != 0 ? 1.0f : 0.0f;
					break;
				}
				case 6:                                                                                               // bolt: zigzag
				{
					float zig = 0.5f + 0.18f * ((((int)(u * 4f) & 1) != 0 ? 1 : -1) * ((u * 4f) - (int)(u * 4f) - 0.5f) * 2f);
					t = Mathf.Abs(v - zig) < 0.09f ? 1.0f : 0.0f;
					break;
				}
				default: t = 0f; break;
			}
			Mix(ref px, a, t, b);
			// a darker fold near the pole and a lighter free edge
			float shade = 0.82f + 0.18f * u;
			InfernoHazards.Set(ref px, (int)(px.r * shade), (int)(px.g * shade), (int)(px.b * shade), 255);
		}

		private static Texture2D FlagTexture(int index)
		{
			if (index <= 0 || index >= Count(Flags)) return null;
			if (s_flagTextures[index] == null)
			{
				s_flagBeingDrawn = index;
				s_flagTextures[index] = InfernoHazards.MakeTexture(FlagTexW, FlagTexH, FlagPixel);
			}
			return s_flagTextures[index];
		}

		private static Sprite FlagSprite(int index)
		{
			Texture2D texture = FlagTexture(index);
			if (texture == null) return null;
			if (s_flagSprites[index] == null)
			{
				Rect full = new Rect(0f, 0f, FlagTexW, FlagTexH);
				s_flagSprites[index] = Sprite.Create(texture, full, new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect, new Vector4(0f, 0f, 0f, 0f));
			}
			return s_flagSprites[index];
		}

		// ---- dressing a car model -------------------------------------------------------------------
		// Called right after the game has fitted a freshly built model (player car in
		// RebuildPlayerVehicleModel, preview car in RebuildGaragePreviewVehicle).
		private static readonly string[] RimMaterials = { "diski2", "diski", "BlackGloss", "W222 - Rims", "W222 - Rims#2", "W222 - Chrome",
			"dtP900_car_DeTomaso_P900_details1", "Brake Disc", "BrakeCaliper", "dtP900_shared_calipers_05", "break",
			"gold piping", "wing lights" };
		private static readonly string[] TyreMaterials = { "rubber", "Tire", "dtP900_shared_tire_racing1", "W222 - Wheel", "W222 - Pure Black" };

		private static bool Listed(string name, string[] list, int count)
		{
			for (int i = 0; i < count; ++i) if (InfernoGarage.SameName(name, list[i])) return true;
			return false;
		}

		private static bool SameString(string a, string b)
		{
			if (a == null || b == null) return false;
			int n = a.Length;
			if (n != b.Length) return false;
			for (int i = 0; i < n; ++i)
				if (InfernoGarage.Lower(a[i]) != InfernoGarage.Lower(b[i])) return false;
			return true;
		}

		// Is this renderer part of a wheel? Its own node or an ancestor (below the
		// model root) is one of the definition's wheel meshes.
		private static bool UnderWheel(Transform node, Transform root, string[] wheelNames)
		{
			if (wheelNames == null) return false;
			for (Transform t = node; t != null && t != root; t = t.parent)
			{
				string name = t.gameObject.name;
				for (int i = 0; i < wheelNames.Length; ++i)
					if (SameString(name, wheelNames[i])) return true;
			}
			return false;
		}

		private static void Tint(Renderer renderer, int slot, float[] rgb)
		{
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			Color colour = new Color(rgb[0], rgb[1], rgb[2], 1f);
			block.SetColor("_BaseColor", colour);
			block.SetColor("_Color", colour);
			renderer.SetPropertyBlock(block, slot);
		}

		private static void DressTires(ShadowRunPrototype game, Transform root, VehicleDefinition definition)
		{
			int index = Selected(Tires);
			if (index == 0 || definition == null) return;
			Item item = TireItems[index];
			Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
			if (renderers == null) return;
			string[] wheelNames = definition.WheelMeshNames;
			bool hasWheels = wheelNames != null && wheelNames.Length > 0;
			for (int i = 0; i < renderers.Length; ++i)
			{
				Renderer renderer = renderers[i];
				if (renderer == null) continue;
				bool wheel = hasWheels && UnderWheel(renderer.transform, root, wheelNames);
				Material[] materials = renderer.sharedMaterials;
				if (materials == null) continue;
				for (int j = 0; j < materials.Length; ++j)
				{
					Material material = materials[j];
					if (material == null) continue;
					string name = material.name;
					// hover cars have no wheels: their piping / pad lights take the rim colour instead
					bool rim = Listed(name, RimMaterials, RimMaterials.Length) && (wheel || !hasWheels);
					bool tyre = wheel && Listed(name, TyreMaterials, TyreMaterials.Length);
					if (rim) Tint(renderer, j, item.a);
					else if (tyre) Tint(renderer, j, item.b);
				}
			}
		}

		// Flags in the world (player car and garage preview) so they can wave.
		private struct FlagInstance
		{
			public Transform pivot;
			public Quaternion base_;
			public float phase;
		}

		private static readonly FlagInstance[] s_flags = new FlagInstance[4];
		private static Material s_poleMaterial;
		private static readonly Material[] s_flagMaterials = new Material[8];

		private static Material FlagMaterial(ShadowRunPrototype game, int index)
		{
			if (s_flagMaterials[index] == null)
			{
				s_flagMaterials[index] = game.MatTransparentUnlit(InfernoLobby.Hex(0xFFFFFF), FlagTexture(index), false);
				string cull = "_Cull";
				if (s_flagMaterials[index].HasProperty(cull)) s_flagMaterials[index].SetFloat(cull, 0f);
			}
			return s_flagMaterials[index];
		}

		private static void DressFlag(ShadowRunPrototype game, Transform model, VehicleDefinition definition, bool preview)
		{
			Transform parent = model.parent;
			if (parent == null) return;
			// one flag per car: drop the previous one (the game rebuilds models freely)
			Transform old = parent.Find("StyleFlag");
			if (old != null) Object.Destroy(old.gameObject);
			int index = Selected(Flags);
			if (index == 0) return;
			Bounds world;
			if (!game.TryGetActiveRendererBounds(model, out world, definition != null ? definition.BoundsRendererNameKeywords : null)) return;
			Bounds local = game.MakeLocalBounds(parent, world);
			Vector3 c = local.center, e = local.extents;
			// the model faces -z in its parent's space (that is what ModelYawOffset is for): rear = +z, left = -x
			float scale = Mathf.Max(0.4f, Mathf.Min(1.6f, e.z / 2.3f));
			float pole = 0.95f * scale, cloth = 0.56f * scale, drop = 0.36f * scale;
			Vector3 foot = new Vector3(c.x - e.x * 0.62f, c.y + e.y - 0.02f * scale, c.z + e.z * 0.80f);
			if (s_poleMaterial == null) s_poleMaterial = game.MatUnlit(InfernoLobby.Hex(0xD8D0E8));
			GameObject root = new GameObject("StyleFlag");
			Transform rootTransform = root.transform;
			rootTransform.SetParent(parent, false);
			rootTransform.localPosition = foot;
			rootTransform.localRotation = new Quaternion(0f, 0f, 0f, 1f);
			rootTransform.localScale = new Vector3(1f, 1f, 1f);
			game.AddCube(rootTransform, "Pole", new Vector3(0f, pole / 2f, 0f), new Vector3(0.035f * scale, pole, 0.035f * scale), s_poleMaterial);
			game.AddCube(rootTransform, "Knob", new Vector3(0f, pole, 0f), new Vector3(0.07f * scale, 0.07f * scale, 0.07f * scale), s_poleMaterial);
			// the cloth hangs from a pivot at the pole top and swings about the pole
			GameObject pivotObject = new GameObject("Cloth");
			Transform pivot = pivotObject.transform;
			pivot.SetParent(rootTransform, false);
			pivot.localPosition = new Vector3(0f, pole - drop / 2f, 0f);
			// the quad's front faces -z; turn it to face the camera behind the car (+z)
			Quaternion base_ = new Quaternion(0f, 1f, 0f, 0f);
			pivot.localRotation = base_;
			pivot.localScale = new Vector3(1f, 1f, 1f);
			GameObject quad = game.Quad("Banner", new Vector3(0f, 0f, 0f), new Vector3(cloth, drop, 1f), FlagMaterial(game, index));
			Transform quadTransform = quad.transform;
			quadTransform.SetParent(pivot, false);
			quadTransform.localPosition = new Vector3(-cloth / 2f, 0f, 0f);   // pole edge at the pivot (mirrored: facing +z)
			quadTransform.localRotation = new Quaternion(0f, 0f, 0f, 1f);
			quadTransform.localScale = new Vector3(cloth, drop, 1f);
			// register for waving (slot: reuse a dead entry)
			for (int i = 0; i < s_flags.Length; i++)
			{
				if (s_flags[i].pivot != null) continue;   // Object.op_Inequality: alive Unity object
				s_flags[i].pivot = pivot; s_flags[i].base_ = base_; s_flags[i].phase = preview ? 1.7f : 0.0f;
				break;
			}
		}

		public static void Dress(ShadowRunPrototype game, GameObject instance, VehicleDefinition definition, bool preview)
		{
			if (instance == null) return;
			Transform model = instance.transform;
			DressTires(game, model, definition);
			DressFlag(game, model, definition, preview);
		}

		private static Quaternion YawQuat(float degrees)
		{
			float half = degrees * 3.14159265f / 360.0f;
			return new Quaternion(0f, Mathf.Sin(half), 0f, Mathf.Cos(half));
		}

		// Every frame (menu and run): flags swing about their pole.
		private static void Wave(ShadowRunPrototype game)
		{
			float t = Time.unscaledTime;
			for (int i = 0; i < s_flags.Length; i++)
			{
				if (ReferenceEquals(s_flags[i].pivot, null)) continue;
				if (s_flags[i].pivot == null) { s_flags[i].pivot = null; continue; }   // destroyed with its car
				float swing = 16.0f * Mathf.Sin(t * 7.5f + s_flags[i].phase) + 5.0f * Mathf.Sin(t * 13.0f + s_flags[i].phase * 2f);
				s_flags[i].pivot.localRotation = InfernoHazards.Mul(s_flags[i].base_, YawQuat(swing));
			}
		}

		// ---- trails (run only) ---------------------------------------------------------------------
		// Slim and short (user request 2026-09-18): ~6 m of streak, a 0.10 core inside
		// a 0.36 glow (was 18 x 0.7 m, 0.30 / 0.95).
		private const int TrailPoints = 12;
		private const float TrailStep = 0.55f;
		private const float TrailGlowWidth = 0.36f, TrailCoreWidth = 0.10f;

		private struct Trail
		{
			public LineRenderer glow;
			public LineRenderer core;
			public Vector3[] points;
			public bool visible;
		}

		private static readonly Trail[] s_trails = new Trail[2];
		private static Material s_trailGlow;
		private static Material s_trailCore;
		private static int s_trailStyle = -1;
		private static ShadowRunPrototype s_owner;
		private static bool s_trailPrimed;

		private static void TrailVisible(bool on)
		{
			for (int i = 0; i < s_trails.Length; i++)
			{
				if (s_trails[i].visible == on) continue;
				s_trails[i].visible = on;
				if (s_trails[i].glow != null) s_trails[i].glow.gameObject.SetActive(on);
				if (s_trails[i].core != null) s_trails[i].core.gameObject.SetActive(on);
			}
		}

		private static void DisposeTrails()
		{
			for (int i = 0; i < s_trails.Length; i++)
			{
				if (s_trails[i].glow != null) Object.Destroy(s_trails[i].glow.gameObject);
				if (s_trails[i].core != null) Object.Destroy(s_trails[i].core.gameObject);
				s_trails[i] = default;
			}
			if (s_trailGlow != null) Object.Destroy(s_trailGlow);
			if (s_trailCore != null) Object.Destroy(s_trailCore);
			s_trailGlow = s_trailCore = null;
			s_trailStyle = -1;
		}

		private static void BuildTrails(ShadowRunPrototype game, int style)
		{
			DisposeTrails();
			Item item = TrailItems[style];
			s_trailGlow = game.MatTransparentUnlit(new Color(item.a[0], item.a[1], item.a[2], 0.22f), null, true);
			s_trailCore = game.MatTransparentUnlit(new Color(Mathf.Min(1.0f, item.a[0] * 0.6f + 0.4f), Mathf.Min(1.0f, item.a[1] * 0.6f + 0.4f), Mathf.Min(1.0f, item.a[2] * 0.6f + 0.4f), 0.9f), null, true);
			Transform parent = game.transform;
			for (int side = 0; side < 2; ++side)
			{
				s_trails[side].points = new Vector3[TrailPoints];
				for (int i = 0; i < TrailPoints; ++i) s_trails[side].points[i] = new Vector3(0f, -5f, 0f);
				s_trails[side].glow = game.AddLine(parent, side != 0 ? "TrailGlowR" : "TrailGlowL", s_trails[side].points, TrailGlowWidth, s_trailGlow);
				s_trails[side].core = game.AddLine(parent, side != 0 ? "TrailCoreR" : "TrailCoreL", s_trails[side].points, TrailCoreWidth, s_trailCore);
				s_trails[side].visible = true;
			}
			s_trailStyle = style;
			s_trailPrimed = false;
		}

		public static void ResetTrails(ShadowRunPrototype game)
		{
			s_owner = game;
			int style = Selected(Trails);
			if (style == 0) { DisposeTrails(); return; }
			if (style != s_trailStyle || s_trails[0].glow == null) BuildTrails(game, style);
			s_trailPrimed = false;
			TrailVisible(false);
		}

		public static void HideTrails(ShadowRunPrototype game)
		{
			if (s_owner != game) return;
			TrailVisible(false);
		}

		// Called from the running simulation with the scaled gameplay dt: the points
		// scroll back with the road, the head follows the rear wheels.
		public static void TickTrails(ShadowRunPrototype game, float dt)
		{
			if (s_owner != game || s_trailStyle <= 0 || s_trails[0].points == null || !game.running || game.paused || dt <= 0f) return;
			float lift = (game.playerAirborne ? game.playerJumpHeight : 0.0f) + 0.22f;
			float moved = game.speed * dt;
			bool show = game.speed > 6f && game.lifeHalfUnits > 0;
			for (int side = 0; side < 2; ++side)
			{
				Vector3[] points = s_trails[side].points;
				Vector3 head = new Vector3(game.playerX + (side != 0 ? 0.78f : -0.78f), lift, InfernoHazards.PlayerZ + 1.55f);
				if (!s_trailPrimed)
				{
					for (int i = 0; i < TrailPoints; ++i) points[i] = head;
				}
				else
				{
					for (int i = 1; i < TrailPoints; ++i) { Vector3 p = points[i]; p.z += moved; points[i] = p; }
					if (points[1].z - head.z >= TrailStep)
					{
						for (int i = TrailPoints - 1; i > 1; --i) points[i] = points[i - 1];
						points[1] = head;
					}
					points[0] = head;
				}
				if (s_trails[side].glow != null) s_trails[side].glow.SetPositions(points);
				if (s_trails[side].core != null) s_trails[side].core.SetPositions(points);
			}
			s_trailPrimed = true;
			TrailVisible(show);
		}

		// ---- the STYLE sheet on the garage page ------------------------------------------------------
		// Coordinates inside the sheet are relative to its centre (x right, y up).
		private const int Visible = 4;                 // chips per row
		private const float ChipSize = 150f, ChipStep = 175f;

		private sealed class Chip
		{
			public Image frame;     // the neon frame (always full tint, like every lobby button)
			public Image plate;     // electric plate inside it = selected (the PLAY button treatment)
			public Image shade;     // dark plate + lock badge = not owned yet (the garage's locked-card treatment)
			public Image lockBadge;
			public Image[] swatch = new Image[3]; public Text empty; public Text name; public Text price; public Image gem; public int index = -1;
		}

		private static RectTransform s_sheet;
		private static Image s_openButton;
		private static readonly Chip[][] s_chips = NewChips();
		private static readonly int[] s_page = new int[CategoryCount];
		private static Text s_notice;
		private static float s_noticeUntil;
		private static Sprite s_circle;

		private static Chip[][] NewChips()
		{
			Chip[][] chips = new Chip[CategoryCount][];
			for (int c = 0; c < CategoryCount; c++)
			{
				chips[c] = new Chip[Visible];
				for (int s = 0; s < Visible; s++) chips[c][s] = new Chip();
			}
			return chips;
		}

		private static void Centre(RectTransform rect, float x, float y, float width, float height) { InfernoLobby.Place(rect, x, y, width, height, new Vector2(0.5f, 0.5f)); }

		private static Image CBlock(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height, uint color)
		{
			Image image = InfernoLobby.Block(game, parent, name, x, y, width, height, color);
			Centre(image.rectTransform, x, y, width, height);
			return image;
		}

		private static Text CLabel(ShadowRunPrototype game, Transform parent, string name, string value, float x, float y, float width, float height, int size, uint color = InfernoLobby.Pearl)
		{
			Text text = InfernoLobby.Label(game, parent, name, value, x, y, width, height, size, color);
			Centre(text.rectTransform, x, y, width, height);
			return text;
		}

		private static Image CFrame(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height)
		{
			Image image = InfernoLobby.NeonFrame(game, parent, name, x, y, width, height);
			Centre(image.rectTransform, x, y, width, height);
			return image;
		}

		// The frame's inner plate (chamfered like the frame), tinted: electric for a
		// selected chip, translucent black as the shade over a locked one.
		private static Image CPlate(ShadowRunPrototype game, Transform parent, string name, float size, Color tint)
		{
			Image image = InfernoLobby.NeonPlate(game, parent, name, 0f, 0f, size - 2f * InfernoLobby.PlateInset, size - 2f * InfernoLobby.PlateInset);
			Centre(image.rectTransform, 0f, 0f, size - 2f * InfernoLobby.PlateInset, size - 2f * InfernoLobby.PlateInset);
			image.color = tint;
			return image;
		}

		public static bool IsOpen(ShadowRunPrototype game)
		{
			return s_sheet != null && s_sheet.gameObject.activeInHierarchy;
		}

		private static void Notice(ShadowRunPrototype game, string text)
		{
			if (s_notice == null) return;
			InfernoLobby.Caption(s_notice, text);
			s_notice.gameObject.SetActive(true);
			s_noticeUntil = Time.unscaledTime + 2.2f;
		}

		private static void RefreshGems(ShadowRunPrototype game, int balance)
		{
			game.menuGemCount = balance;
			if (game.gemAmountText != null)
			{
				InfernoLobby.Caption(game.gemAmountText, balance.ToString());
			}
		}

		private static void Apply(ShadowRunPrototype game)
		{
			// the garage preview and the player car are rebuilt so the new dress shows at once
			game.RebuildGaragePreviewVehicle();
			game.RebuildPlayerVehicleModel();
		}

		private static Color Rgb(float[] c, float alpha = 1f) { return new Color(c[0], c[1], c[2], alpha); }
		private static void Tint(Image image, Color color) { image.color = color; }
		private static void Show(Component component, bool on) { component.gameObject.SetActive(on); }

		private static void RefreshChip(ShadowRunPrototype game, int category, int slot)
		{
			Chip chip = s_chips[category][slot];
			if (chip.frame == null) return;
			int index = s_page[category] * Visible + slot;
			chip.index = index;
			bool present = index < Count(category);
			Show(chip.frame, present);
			Show(chip.price, present);
			Show(chip.gem, false);
			if (!present) return;
			Item item = Items(category)[index];
			bool owned = Owned(category, index), selected = Selected(category) == index;
			Show(chip.plate, selected);
			Show(chip.shade, !owned);
			if (chip.lockBadge != null) Show(chip.lockBadge, !owned);
			InfernoLobby.Caption(chip.name, item.name);
			chip.name.color = InfernoLobby.Hex(owned ? InfernoLobby.FrameText : InfernoLobby.Lilac);
			bool none = index == 0;
			if (category == Tires)
			{
				float[] stockTyre = { 0.10f, 0.09f, 0.13f }, stockRim = { 0.35f, 0.33f, 0.44f };
				Tint(chip.swatch[0], Rgb(none ? stockTyre : item.b));
				Tint(chip.swatch[1], Rgb(none ? stockRim : item.a));
			}
			else if (category == Flags)
			{
				Sprite sprite = none ? null : FlagSprite(index);
				Show(chip.swatch[0], sprite != null);
				Show(chip.swatch[1], sprite != null);
				Show(chip.empty, sprite == null);
				if (sprite != null) { chip.swatch[0].sprite = sprite; InfernoLobby.Paint(chip.swatch[0], 0xFFFFFF); }
			}
			else
			{
				Show(chip.swatch[0], !none);
				Show(chip.swatch[1], !none);
				Show(chip.empty, none);
				if (!none)
				{
					Tint(chip.swatch[0], Rgb(item.a, 0.35f));
					float[] core = { Mathf.Min(1.0f, item.a[0] * 0.5f + 0.5f), Mathf.Min(1.0f, item.a[1] * 0.5f + 0.5f), Mathf.Min(1.0f, item.a[2] * 0.5f + 0.5f) };
					Tint(chip.swatch[1], Rgb(core));
				}
			}
			// caption under the chip: state in the lobby's lilac, a price next to the crystal
			if (selected) { InfernoLobby.Caption(chip.price, "SELECTED"); chip.price.color = InfernoLobby.Hex(InfernoLobby.Blossom); }
			else if (owned) { InfernoLobby.Caption(chip.price, "OWNED"); chip.price.color = InfernoLobby.Hex(InfernoLobby.Lilac); }
			else
			{
				InfernoLobby.Caption(chip.price, item.price.ToString());
				chip.price.color = InfernoLobby.Hex(InfernoLobby.Pearl);
				Show(chip.gem, true);
			}
		}

		private static void Refresh(ShadowRunPrototype game)
		{
			for (int c = 0; c < CategoryCount; ++c)
				for (int s = 0; s < Visible; ++s) RefreshChip(game, c, s);
		}

		private static void Pick(ShadowRunPrototype game, int category, int slot)
		{
			if (!IsOpen(game)) return;
			int index = s_page[category] * Visible + slot;
			if (index >= Count(category)) return;
			Item item = Items(category)[index];
			if (Selected(category) == index) return;
			if (!Owned(category, index))
			{
				int balance = SupabaseGameManager.Crystals;
				if (balance == int.MaxValue) { Notice(game, "PROFILE NOT READY"); return; }
				if (balance < item.price) { Notice(game, "NOT ENOUGH CRYSTALS - TAP THE GEM COUNTER FOR MORE"); return; }
				balance -= item.price;
				SupabaseGameManager.SetCrystals(balance);
				RefreshGems(game, balance);
				Own(category, index);
				Notice(game, "UNLOCKED");
			}
			Select(category, index);
			Refresh(game);
			Apply(game);
		}

		private static void Page(ShadowRunPrototype game, int category, int direction)
		{
			if (!IsOpen(game)) return;
			int pages = (Count(category) + Visible - 1) / Visible;
			s_page[category] = (s_page[category] + direction + pages) % pages;
			Refresh(game);
		}

		private const float SheetWidth = 920f, SheetHeight = 790f;

		// The sheet pops up in the middle of the garage page (anchor and pivot at the
		// page centre) and is scaled down when the page is too short for its 790 px
		// plus the header and dock reserves, so the bottom row never leaves the screen.
		private static void FitSheet(ShadowRunPrototype game)
		{
			if (s_sheet == null || game.garageRoot == null) return;
			InfernoLobby.Place(s_sheet, 0f, 20f, SheetWidth, SheetHeight, new Vector2(0.5f, 0.5f));
			RectTransform pageRect = game.garageRoot.transform.GetComponent<RectTransform>();
			if (pageRect == null) return;
			Rect bounds = pageRect.rect;
			float room = Mathf.Min(bounds.width - 40f, bounds.height - 460f);   // header + dock reserves
			float scale = Mathf.Max(0.5f, Mathf.Min(1.0f, Mathf.Min(room / SheetWidth, (bounds.height - 460f) / SheetHeight)));
			s_sheet.localScale = new Vector3(scale, scale, 1f);
		}

		private static void OpenSheet(ShadowRunPrototype game)
		{
			if (s_sheet == null) return;
			FitSheet(game);
			s_sheet.gameObject.SetActive(true);
			if (s_openButton != null) Show(s_openButton, false);   // nothing peeks out from under the sheet
			Refresh(game);
		}

		public static void CloseSheet(ShadowRunPrototype game)
		{
			if (s_sheet != null) s_sheet.gameObject.SetActive(false);
			if (s_openButton != null) Show(s_openButton, true);
		}

		private static void OpenClick(ShadowRunPrototype game) { OpenSheet(game); }
		private static void CloseClick(ShadowRunPrototype game) { CloseSheet(game); }

		// Closed instance delegates (C++: MethodInfo trampolines, InfernoSettings::Hook); C#: a lambda over the game.
		private static void Hook(ShadowRunPrototype game, Button button, System.Action<ShadowRunPrototype> fn)
		{
			button.onClick.AddListener(() => fn(game));
		}

		private static Image FrameButton(ShadowRunPrototype game, Transform parent, string name, string title, float x, float y, float width, float height, int size, System.Action<ShadowRunPrototype> fn)
		{
			Image image = CFrame(game, parent, name, x, y, width, height);
			Button button = InfernoLobby.ButtonOn(image);
			if (title != null) CLabel(game, image.transform, "Label", title, 0f, 0f, width - 16f, height - 8f, size, InfernoLobby.FrameText);
			Hook(game, button, fn);
			return image;
		}

		private static Image Circle(ShadowRunPrototype game, Transform parent, string name, float x, float y, float size, uint color)
		{
			if (s_circle == null) s_circle = game.MakeCircleSprite(128);
			Image image = CBlock(game, parent, name, x, y, size, size, color);
			if (s_circle != null) { image.sprite = s_circle; image.type = Image.Type.Simple; image.preserveAspect = true; }
			return image;
		}

		private static void BuildChip(ShadowRunPrototype game, int C, int S, Transform row, float x, float y)
		{
			Chip chip = s_chips[C][S];
			chip.frame = CFrame(game, row, "Chip", x, y, ChipSize, ChipSize);
			Button button = InfernoLobby.ButtonOn(chip.frame);
			Hook(game, button, g => Pick(g, C, S));
			Transform inside = chip.frame.transform;
			chip.plate = CPlate(game, inside, "Selected", ChipSize, InfernoLobby.Hex(0xFFFFFF));   // drawn first: under the swatch
			if (C == Tires)
			{
				chip.swatch[0] = Circle(game, inside, "Tyre", 0f, 14f, 92f, 0x1A1622);
				chip.swatch[1] = Circle(game, inside, "Rim", 0f, 14f, 58f, 0x5A5470);
				chip.swatch[2] = Circle(game, inside, "Hub", 0f, 14f, 18f, InfernoLobby.Ink);
			}
			else if (C == Flags)
			{
				chip.swatch[1] = CBlock(game, inside, "Pole", -54f, 12f, 5f, 86f, 0xD8D0E8);
				chip.swatch[0] = CBlock(game, inside, "Banner", 4f, 26f, 108f, 60f, 0xFFFFFF);
				chip.empty = CLabel(game, inside, "Empty", "--", 0f, 14f, 100f, 60f, 34, InfernoLobby.Lavender);
			}
			else
			{
				chip.swatch[0] = CBlock(game, inside, "Glow", 0f, 14f, 118f, 26f, 0xFFFFFF);
				chip.swatch[1] = CBlock(game, inside, "Core", 0f, 14f, 118f, 8f, 0xFFFFFF);
				chip.empty = CLabel(game, inside, "Empty", "--", 0f, 14f, 100f, 60f, 34, InfernoLobby.Lavender);
			}
			chip.name = CLabel(game, inside, "Name", "", 0f, -50f, 142f, 30f, 19, InfernoLobby.FrameText);
			// locked: the garage's own dark shade + lock badge over the content
			chip.shade = CPlate(game, inside, "Locked", ChipSize, InfernoLobby.Hex(InfernoLobby.Black, 0.58f));
			if (game.garageLockSprite != null)
			{
				chip.lockBadge = CBlock(game, inside, "Lock", 0f, 14f, 44f, 44f, 0xFFFFFF);
				chip.lockBadge.sprite = game.garageLockSprite;
				chip.lockBadge.preserveAspect = true;
			}
			chip.price = CLabel(game, row, "Price", "", x + 12f, y - ChipSize / 2f - 22f, 120f, 30f, 22, InfernoLobby.Lilac);
			chip.gem = CBlock(game, row, "PriceGem", x - 34f, y - ChipSize / 2f - 22f, 26f, 26f, 0xFFFFFF);
			chip.gem.sprite = InfernoLobby.GemSprite(game);
			chip.gem.preserveAspect = true;
		}

		// A row: a lilac heading at the card's left margin (the lobby's card-row style),
		// four chips, and the map selector's < > arrow buttons at the ends.
		private static void BuildRow(ShadowRunPrototype game, int C, Transform sheet, float top)
		{
			Text heading = CLabel(game, sheet, "RowTitle", CategoryNames[C], -412f + 165f, top, 330f, 40f, 26, InfernoLobby.Lilac);
			heading.alignment = TextAnchor.MiddleLeft;
			float y = top - 112f;
			FrameButton(game, sheet, "Prev", "<", -418f, y, 72f, 72f, 40, g => Page(g, C, -1));
			FrameButton(game, sheet, "Next", ">", 418f, y, 72f, 72f, 40, g => Page(g, C, 1));
			BuildChip(game, C, 0, sheet, -1.5f * ChipStep, y);
			BuildChip(game, C, 1, sheet, -0.5f * ChipStep, y);
			BuildChip(game, C, 2, sheet, 0.5f * ChipStep, y);
			BuildChip(game, C, 3, sheet, 1.5f * ChipStep, y);
		}

		// Built once at the end of MakeGarageScreen: the STYLE button beside the
		// purchase action (bottom-anchored like it), and the hidden sheet above the dock.
		public static void Build(ShadowRunPrototype game)
		{
			if (game.garageRoot == null || s_openButton != null) return;
			Transform page = game.garageRoot.transform;
			// same height and baseline as the game's purchase button (620x121 centred
			// at x -40, y 445 from the page bottom), 30 px to its right
			s_openButton = FrameButton(game, page, "StyleButton", "STYLE", 0f, 0f, 150f, 121f, 28, OpenClick);
			InfernoLobby.Place(s_openButton.rectTransform, 375f, 445f, 150f, 121f, new Vector2(0.5f, 0f));
			// 920x790 in the middle of the page (FitSheet positions and scales it on open)
			Image sheet = InfernoLobby.NeonFrame(game, page, "StyleSheet", 0f, 0f, SheetWidth, SheetHeight);
			sheet.raycastTarget = true;   // swallows taps meant for the carousel underneath
			s_sheet = sheet.rectTransform;
			InfernoLobby.Place(s_sheet, 0f, 20f, SheetWidth, SheetHeight, new Vector2(0.5f, 0.5f));
			Transform inside = sheet.transform;
			// card title like DAILY GOALS (pearl, 32, at the 48 px inner margin); close
			// is the 88 px corner button the gem shop uses
			Text title = CLabel(game, inside, "Title", "STYLE", -412f + 200f, 344f, 400f, 42f, 32, InfernoLobby.Pearl);
			title.alignment = TextAnchor.MiddleLeft;
			FrameButton(game, inside, "Close", "X", 396f, 331f, 88f, 88f, 40, CloseClick);
			BuildRow(game, Tires, inside, 286f);
			BuildRow(game, Flags, inside, 56f);
			BuildRow(game, Trails, inside, -174f);
			s_notice = CLabel(game, inside, "Notice", "", 0f, -370f, 860f, 34f, 22, InfernoLobby.Blossom);
			Show(s_notice, false);
			s_sheet.gameObject.SetActive(false);
		}

		// Every frame from ShadowRunPrototype.Update (unscaled): flags wave, notices expire.
		public static void Frame(ShadowRunPrototype game)
		{
			Wave(game);
			if (s_notice != null && s_noticeUntil > 0f && Time.unscaledTime > s_noticeUntil)
			{
				s_noticeUntil = 0f;
				Show(s_notice, false);
			}
		}

		public static void Dispose(ShadowRunPrototype game)
		{
			if (s_owner == game) { DisposeTrails(); s_owner = null; }
			for (int i = 0; i < s_flags.Length; i++) s_flags[i].pivot = null;
			s_sheet = null; s_openButton = null; s_notice = null; s_circle = null;
			for (int c = 0; c < CategoryCount; c++) for (int s = 0; s < Visible; s++) s_chips[c][s] = new Chip();
		}
	}
}
