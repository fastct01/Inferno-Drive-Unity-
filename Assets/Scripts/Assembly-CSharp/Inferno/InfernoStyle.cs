using System.Collections.Generic;
using ShadowRun.Services;
using UnityEngine;
using UnityEngine.UI;

// Port of Il2CppOutputProject/Source/il2cppOutput/InfernoStyle.inc — car customisation,
// hand-maintained: TIRES (rim and tyre colours), FLAGS (a pennant on a pole at the rear),
// TRAILS (neon light streaks behind the car during a run) and, since the 2026-09-21
// request, PAINT (a body colour chosen per car). Bought with crystals and picked in a
// tabbed STYLE sheet on the garage page; tires / flags / trails apply to every car, the
// paint to the active car. Reuses the lobby, garage and driving helpers; wired in at the
// hook points of tools/apply_lobby_patches.py (patch_style). See
// docs/lobby-implementation.md, "Customisation".
/* [user request 2026-09-21: style] */
// What changed against the port (original values are kept as "// original:" comments):
// - trails: one textured ribbon per side whose width and alpha taper along its length,
//   a white-hot core and a spark emitter (original: two flat untextured lines), plus a
//   static ribbon behind the garage preview car so a bought trail can be seen there;
// - tires: CHROME / GOLD / BLACKOUT are metallic material instances of the rim materials,
//   the NEON items HDR-emissive instances (original: a flat colour tint); the wheel-part
//   matching covers every car model's naming (sportcar disc1 rims, P900 dtP900:*wheel*);
// - flags: 256x160 supersampled cloth on a 12x6 mesh that ripples every frame, lit and
//   double-sided, on a chrome pole (original: a rigid 96x64 unlit quad on a box pole);
// - PAINT catalogue: twelve colours, owned once (InfernoDrive.Style.Paint.<i>.Owned) and
//   selected per car (InfernoDrive.Paint.<carId>.Selected); InfernoCars.Decorate reads
//   SelectedPaint(carId) when it builds a model;
// - the sheet: four rhomboid tabs over one 2x4 grid of chips with a pager and the active
//   car's name in the header, built with the InfernoUi kit (original: three rows of four).
// /* [user request 2026-09-24: style sheet] */ the sheet is a bottom sheet now: it stands just above the
//   dock and lifts the garage's 3D preview into the free upper half of the page, so tyres / flags / trails /
//   paint show on the car while they are picked; the tabs carry glyphs, chips a halo in their colour, the
//   lock badge sits in the chip's corner and a hint line under the pager says what a tap does
//   (original: a 920 x 920 sheet centred over the car, which it covered).
public sealed partial class ShadowRunPrototype
{
	internal static partial class InfernoStyle
	{
		// ---- catalogue ------------------------------------------------------------------------
		public const int Tires = 0, Flags = 1, Trails = 2, Paint = 3;   // original: private const int Tires = 0, Flags = 1, Trails = 2, CategoryCount = 3;
		private const int CategoryCount = 4;
		private static readonly string[] CategoryNames = { "TIRES", "FLAGS", "TRAILS", "PAINT" };   // original: three names
		private static readonly string[] CategoryKeys = { "Tires", "Flags", "Trails", "Paint" };     // original: three keys

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

		// tires: a = rim colour, b = tyre colour. 1..3 are metallic finishes, 4..6 glow.
		private const int MetallicFirst = 1, MetallicLast = 3, NeonFirst = 4, NeonLast = 6, Blackout = 3;
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
		// paint: a = body colour, b = { metallic, smoothness }. Item 0 keeps the model's own
		// paint (InfernoCars.DefaultPaint); GOLD and CHROME are the metallic finishes.
		private static readonly Item[] PaintItems =
		{
			new Item("STOCK",         0,   new[] {0f, 0f, 0f},          new[] {0f, 0f, 0f},       0),
			new Item("MIDNIGHT",      60,  new[] {0.06f, 0.07f, 0.14f}, new[] {0.35f, 0.85f, 0f}, 0),
			new Item("ARCTIC",        60,  new[] {0.92f, 0.95f, 1.00f}, new[] {0.25f, 0.85f, 0f}, 0),
			new Item("GUNMETAL",      70,  new[] {0.28f, 0.30f, 0.34f}, new[] {0.80f, 0.75f, 0f}, 0),
			new Item("CRIMSON",       80,  new[] {0.78f, 0.05f, 0.10f}, new[] {0.30f, 0.88f, 0f}, 0),
			new Item("INFERNO",       80,  new[] {1.00f, 0.36f, 0.06f}, new[] {0.30f, 0.88f, 0f}, 0),
			new Item("ELECTRIC BLUE", 90,  new[] {0.08f, 0.40f, 1.00f}, new[] {0.35f, 0.90f, 0f}, 0),
			new Item("VIOLET",        90,  new[] {0.55f, 0.16f, 0.95f}, new[] {0.35f, 0.90f, 0f}, 0),
			new Item("ACID",          90,  new[] {0.62f, 1.00f, 0.12f}, new[] {0.25f, 0.90f, 0f}, 0),
			new Item("MAGENTA",       100, new[] {0.95f, 0.15f, 0.70f}, new[] {0.35f, 0.90f, 0f}, 0),
			new Item("GOLD",          120, new[] {0.95f, 0.72f, 0.22f}, new[] {0.90f, 0.85f, 0f}, 0),
			new Item("PEARL",         120, new[] {0.96f, 0.94f, 0.98f}, new[] {0.55f, 0.95f, 0f}, 0),
			new Item("CHROME",        150, new[] {0.85f, 0.87f, 0.92f}, new[] {1.00f, 0.95f, 0f}, 0),
		};

		private static Item[] Items(int category)
		{
			switch (category)
			{
				case Tires: return TireItems;
				case Flags: return FlagItems;
				case Trails: return TrailItems;
				default: return PaintItems;
			}
		}

		private static int Count(int category) { return Items(category).Length; }

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

		// Paint is chosen per car: the sheet edits the active car's choice (s_paintCar,
		// refreshed by RefreshForVehicle / SyncCar), InfernoCars.Decorate asks SelectedPaint
		// for whichever car it is building. Ids compare case-insensitively in the game, so
		// the key is spelt in lower case.
		private static string s_paintCar;

		private static string LowerId(string id)
		{
			char[] chars = id.ToCharArray();
			for (int i = 0; i < chars.Length; ++i) chars[i] = InfernoGarage.Lower(chars[i]);
			return new string(chars);
		}

		private static string PaintKey(string carId) { return "InfernoDrive.Paint." + LowerId(carId) + ".Selected"; }

		private static int SelectedPaintIndex(string carId)
		{
			if (string.IsNullOrEmpty(carId)) return 0;
			int index = PlayerPrefs.GetInt(PaintKey(carId), 0);   // missing -> 0 = STOCK, the model's own paint
			if (index <= 0 || index >= PaintItems.Length || !Owned(Paint, index)) return 0;
			return index;
		}

		private static int Selected(int category)
		{
			if (category == Paint) return SelectedPaintIndex(s_paintCar);
			int selected = PlayerPrefs.GetInt(Key(category, "Selected"), 0);
			if (selected < 0 || selected >= Count(category) || !Owned(category, selected)) return 0;
			return selected;
		}

		private static void Select(int category, int index)
		{
			if (category == Paint)
			{
				if (string.IsNullOrEmpty(s_paintCar)) return;
				PlayerPrefs.SetInt(PaintKey(s_paintCar), index);
			}
			else PlayerPrefs.SetInt(Key(category, "Selected"), index);
			PlayerPrefs.Save();
		}

		private static void Own(int category, int index)
		{
			PlayerPrefs.SetInt(Key(category, "Owned", index), 1);
			PlayerPrefs.Save();
		}

		public struct PaintChoice { public bool set; public Color color; public float metallic; public float smoothness; }

		// The paint InfernoCars.Decorate applies to a model: set = false keeps the model's default.
		public static PaintChoice SelectedPaint(string carId)
		{
			PaintChoice choice = default;
			int index = SelectedPaintIndex(carId);
			if (index <= 0) return choice;
			Item item = PaintItems[index];
			choice.set = true;
			choice.color = new Color(item.a[0], item.a[1], item.a[2], 1f);
			choice.metallic = item.b[0];
			choice.smoothness = item.b[1];
			return choice;
		}

		private static void SyncCar(ShadowRunPrototype game)
		{
			if (game == null) return;
			VehicleDefinition active = game.ActiveVehicle;
			if (active != null && !string.IsNullOrEmpty(active.Id)) s_paintCar = active.Id;
		}

		// Called when the active car changes (SetActiveVehicleDefinition): the PAINT tab and
		// the sheet's subtitle follow that car.
		public static void RefreshForVehicle(ShadowRunPrototype game)
		{
			SyncCar(game);
			if (s_subtitle != null) InfernoLobby.Caption(s_subtitle, CarLabel(game));
			if (s_sheet != null) Refresh(game);
		}

		private static string CarLabel(ShadowRunPrototype game)
		{
			string id = s_paintCar;
			if (string.IsNullOrEmpty(id)) return "";
			string name = InfernoCars.Contains(id) ? InfernoCars.Name(id) : null;
			if (string.IsNullOrEmpty(name)) name = id;
			return Spaced(name);
		}

		// "PHANTOM GT" -> "P H A N T O M   G T": the letter-spaced caps of the mockups' subtitles.
		private static string Spaced(string s)
		{
			System.Text.StringBuilder text = new System.Text.StringBuilder(s.Length * 2);
			for (int i = 0; i < s.Length; ++i)
			{
				char c = s[i];
				if (c == ' ') { text.Append("   "); continue; }
				if (i > 0 && s[i - 1] != ' ') text.Append(' ');
				text.Append(char.ToUpperInvariant(c));
			}
			return text.ToString();
		}

		// ---- procedural textures --------------------------------------------------------------------
		private static Sprite SpriteOf(Texture2D texture)
		{
			return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect, new Vector4(0f, 0f, 0f, 0f));
		}

		private static float Smooth(float edge0, float edge1, float x)
		{
			float t = Mathf.Max(0f, Mathf.Min(1f, (x - edge0) / (edge1 - edge0)));
			return t * t * (3f - 2f * t);
		}

		// Light from the upper left for every swatch (u right, v up, both -1..1).
		private static float Lit(float u, float v) { return Mathf.Clamp01(0.5f + 0.5f * (-u * 0.55f + v * 0.83f)); }

		// The trail ribbon's texture, stretched from the head (u = 0) to the tail (u = 1): a
		// bright core line softening to the edges, fading out along the length. The alpha
		// carries the fade because the URP Unlit material ignores vertex colours, so the
		// LineRenderer gradient alone would not taper it.
		private const int StreakTexW = 256, StreakTexH = 32;
		private static Texture2D s_streakTexture, s_softTexture;

		private static void StreakPixel(ref Color32 px, float u, float v)
		{
			float along = (u + 1f) * 0.5f;                       // 0 head .. 1 tail
			float core = Mathf.Exp(-(v * 2.4f) * (v * 2.4f));   // the bright centre line
			float edge = 1f - v * v;                             // soft edges
			float head = Mathf.Min(1f, along * 25f);             // no hard cut at the head
			float fade = Mathf.Pow(1f - along, 1.15f);
			int grey = (int)(255f * (0.30f + 0.70f * core) * edge);
			InfernoHazards.Set(ref px, grey, grey, grey, (int)(255f * 0.95f * fade * edge * head));
		}

		private static Texture2D StreakTexture()
		{
			if (s_streakTexture == null)
			{
				s_streakTexture = InfernoHazards.MakeTexture(StreakTexW, StreakTexH, StreakPixel);
				s_streakTexture.filterMode = FilterMode.Bilinear;
			}
			return s_streakTexture;
		}

		// A soft disc: the spark particle and the neon halo behind a rim swatch.
		private static void SoftPixel(ref Color32 px, float u, float v)
		{
			float r = Mathf.Sqrt(u * u + v * v);
			InfernoHazards.Set(ref px, 255, 255, 255, (int)(255f * (1f - Smooth(0.15f, 1f, r))));
		}

		private static Texture2D SoftTexture()
		{
			if (s_softTexture == null) s_softTexture = InfernoHazards.MakeTexture(64, 64, SoftPixel);
			return s_softTexture;
		}

		// ---- flag textures -----------------------------------------------------------------------
		// 256x160 (original: 96x64), every texel the mean of four FlagPixel samples, with a
		// mip chain (InfernoHazards.MakeTexture) and bilinear filtering.
		private const int FlagTexW = 256, FlagTexH = 160;   // original: FlagTexW = 96, FlagTexH = 64
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
			u = 0.5f - u * 0.5f; v = v * 0.5f + 0.5f;   // the pole is at the texture's right edge (see the banner mesh's uvs)
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

		// 2x2 supersampling: FlagPixel at the texel's quarter points (u, v run -1..1 across the
		// texture, so half a texel is 0.5 / W).
		private static void FlagPixelAA(ref Color32 px, float u, float v)
		{
			const float du = 0.5f / FlagTexW, dv = 0.5f / FlagTexH;
			Color32 sample = default;
			int r = 0, g = 0, b = 0;
			FlagPixel(ref sample, u - du, v - dv); r += sample.r; g += sample.g; b += sample.b;
			FlagPixel(ref sample, u + du, v - dv); r += sample.r; g += sample.g; b += sample.b;
			FlagPixel(ref sample, u - du, v + dv); r += sample.r; g += sample.g; b += sample.b;
			FlagPixel(ref sample, u + du, v + dv); r += sample.r; g += sample.g; b += sample.b;
			InfernoHazards.Set(ref px, (r + 2) / 4, (g + 2) / 4, (b + 2) / 4, 255);
		}

		private static Texture2D FlagTexture(int index)
		{
			if (index <= 0 || index >= Count(Flags)) return null;
			if (s_flagTextures[index] == null)
			{
				s_flagBeingDrawn = index;
				s_flagTextures[index] = InfernoHazards.MakeTexture(FlagTexW, FlagTexH, FlagPixelAA);   // original: FlagPixel
				s_flagTextures[index].filterMode = FilterMode.Bilinear;
				s_flagTextures[index].anisoLevel = 4;
			}
			return s_flagTextures[index];
		}

		private static Sprite FlagSprite(int index)
		{
			Texture2D texture = FlagTexture(index);
			if (texture == null) return null;
			if (s_flagSprites[index] == null) s_flagSprites[index] = SpriteOf(texture);
			return s_flagSprites[index];
		}

		// ---- dressing a car model -------------------------------------------------------------------
		// Called right after the game has fitted a freshly built model (player car in
		// RebuildPlayerVehicleModel, preview car in RebuildGaragePreviewVehicle).
		// Material names of the five meshes (Assets/Resources/*.prefab): sportcar rims
		// "diski"/"diski2" on the disc1.* nodes (its BrakeDiscNames) with "rubber" tyres and
		// "break" brake parts; Cayman "BlackGloss"/"Brake Disc"/"Tire" on CaymanGT4.Wheel.*
		// and "BrakeCaliper" on CaymanGT4.WheelBrake.*; W222 "W222 - Rims"/"Rims#2"/"Chrome"/
		// "Pure Black"/"Wheel" on W222.Wheel.*; P900 "Color" + "dtP900_car_DeTomaso_P900_details1"
		// on dtP900:rims_wheel*, "dtP900_shared_tire_racing1" on dtP900:tires_wheel*,
		// "dtP900_shared_calipers_05" on dtP900:calipers_garage_wheel*; the hover car has no
		// wheels and takes the rim colour on its "gold piping" / "wing lights".
		private static readonly string[] RimMaterials = { "diski2", "diski", "BlackGloss", "W222 - Rims", "W222 - Rims#2", "W222 - Chrome",
			"dtP900_car_DeTomaso_P900_details1", "Color", "Brake Disc", "BrakeCaliper", "dtP900_shared_calipers_05", "break" };   // original: also "gold piping", "wing lights" (now HoverAccents)
		private static readonly string[] TyreMaterials = { "rubber", "Tire", "dtP900_shared_tire_racing1", "W222 - Wheel", "W222 - Pure Black" };
		// Materials that only ever sit on a wheel: tinted even where the node is not under a
		// listed wheel mesh (the sportcar's rims and brakes hang beside its wheels, not under them).
		private static readonly string[] RimsAnywhere = { "diski2", "diski", "W222 - Rims", "W222 - Rims#2", "break", "BrakeCaliper" };
		private static readonly string[] TyresAnywhere = { "rubber", "Tire", "dtP900_shared_tire_racing1", "W222 - Wheel" };
		private static readonly string[] HoverAccents = { "gold piping", "wing lights" };

		private static bool Listed(string name, string[] list)
		{
			for (int i = 0; i < list.Length; ++i) if (InfernoGarage.SameName(name, list[i])) return true;
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

		private static bool InList(string name, string[] list)
		{
			if (list == null) return false;
			for (int i = 0; i < list.Length; ++i) if (SameString(name, list[i])) return true;
			return false;
		}

		// Case-insensitive "contains" without allocating (node names are matched every dress).
		private static bool ContainsWord(string s, string ascii)
		{
			if (s == null) return false;
			int n = s.Length, m = ascii.Length;
			for (int start = 0; start + m <= n; ++start)
			{
				int k = 0;
				while (k < m && InfernoGarage.Lower(s[start + k]) == ascii[k]) ++k;
				if (k == m) return true;
			}
			return false;
		}

		// Is this renderer part of a wheel? Its own node or an ancestor (below the model
		// root) is one of the definition's wheel or brake-disc meshes, or is named like a
		// wheel (CaymanGT4.Wheel.Bk.L, W222.Wheel.Ft.R, dtP900:rims_wheelFL_...).
		private static bool UnderWheel(Transform node, Transform root, VehicleDefinition definition)
		{
			for (Transform t = node; t != null && t != root; t = t.parent)
			{
				string name = t.gameObject.name;
				if (definition != null && (InList(name, definition.WheelMeshNames) || InList(name, definition.BrakeDiscNames))) return true;
				if (ContainsWord(name, "wheel")) return true;
			}
			return false;
		}

		private static readonly MaterialPropertyBlock s_block = new MaterialPropertyBlock();

		private static void Tint(Renderer renderer, int slot, float[] rgb)
		{
			s_block.Clear();
			Color colour = new Color(rgb[0], rgb[1], rgb[2], 1f);
			s_block.SetColor("_BaseColor", colour);
			s_block.SetColor("_Color", colour);
			renderer.SetPropertyBlock(s_block, slot);
		}

		// Rim finishes are material instances (keywords cannot come from a property block):
		// CHROME / GOLD / BLACKOUT metallic, the NEON items emissive. One instance per
		// (source material, item), owned here and destroyed in Dispose; the model keeps the
		// prefab's shared materials untouched. The same values also go into a property block
		// on the slot, so StyleVehicleForGarage (which darkens every preview material and
		// blacks out its emission) cannot dull them.
		private static readonly Dictionary<Material, Material[]> s_rimInstances = new Dictionary<Material, Material[]>();
		private static readonly Dictionary<Material, Material> s_rimSource = new Dictionary<Material, Material>();

		private static bool RimFinish(int index) { return index >= MetallicFirst && index <= NeonLast; }
		private static bool NeonRim(int index) { return index >= NeonFirst && index <= NeonLast; }

		private static Material RimInstance(Material source, int index)
		{
			Material[] instances;
			if (!s_rimInstances.TryGetValue(source, out instances))
			{
				instances = new Material[TireItems.Length];
				s_rimInstances[source] = instances;
			}
			Material instance = instances[index];
			if (instance != null) return instance;
			Item item = TireItems[index];
			Color colour = new Color(item.a[0], item.a[1], item.a[2], 1f);
			instance = new Material(source);
			instance.name = source.name + " (Style)";
			if (instance.HasProperty("_BaseColor")) instance.SetColor("_BaseColor", colour);
			if (instance.HasProperty("_Color")) instance.SetColor("_Color", colour);
			if (NeonRim(index))
			{
				instance.EnableKeyword("_EMISSION");
				if (instance.HasProperty("_EmissionColor")) instance.SetColor("_EmissionColor", colour * 3f);   // HDR: blooms on the run camera
				if (instance.HasProperty("_Metallic")) instance.SetFloat("_Metallic", 0.3f);
				if (instance.HasProperty("_Smoothness")) instance.SetFloat("_Smoothness", 0.7f);
			}
			else
			{
				float metallic = index == Blackout ? 0.2f : 0.95f, smoothness = index == Blackout ? 0.5f : 0.85f;
				if (instance.HasProperty("_Metallic")) instance.SetFloat("_Metallic", metallic);
				if (instance.HasProperty("_Smoothness")) instance.SetFloat("_Smoothness", smoothness);
				if (instance.HasProperty("_MetallicGlossMap")) instance.SetTexture("_MetallicGlossMap", null);
				if (instance.HasProperty("_SpecularHighlights")) instance.SetFloat("_SpecularHighlights", 1f);
				if (instance.HasProperty("_EnvironmentReflections")) instance.SetFloat("_EnvironmentReflections", 1f);
				instance.DisableKeyword("_METALLICSPECGLOSSMAP");
				instance.DisableKeyword("_SPECULARHIGHLIGHTS_OFF");
				instance.DisableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
			}
			instances[index] = instance;
			s_rimSource[instance] = source;
			return instance;
		}

		private static void TintRim(Renderer renderer, int slot, int index)
		{
			Item item = TireItems[index];
			Color colour = new Color(item.a[0], item.a[1], item.a[2], 1f);
			s_block.Clear();
			s_block.SetColor("_BaseColor", colour);
			s_block.SetColor("_Color", colour);
			if (NeonRim(index)) s_block.SetColor("_EmissionColor", colour * 3f);
			else if (RimFinish(index))
			{
				s_block.SetFloat("_Metallic", index == Blackout ? 0.2f : 0.95f);
				s_block.SetFloat("_Smoothness", index == Blackout ? 0.5f : 0.85f);
			}
			renderer.SetPropertyBlock(s_block, slot);
		}

		private static void DisposeRimInstances()
		{
			foreach (KeyValuePair<Material, Material[]> entry in s_rimInstances)
			{
				Material[] instances = entry.Value;
				for (int i = 0; i < instances.Length; ++i) if (instances[i] != null) Object.Destroy(instances[i]);
			}
			s_rimInstances.Clear();
			s_rimSource.Clear();
		}

		private static void DressTires(ShadowRunPrototype game, Transform root, VehicleDefinition definition)
		{
			int index = Selected(Tires);
			Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
			if (renderers == null) return;
			Item item = TireItems[index];
			bool hover = definition != null && definition.HoverEnabled;
			for (int i = 0; i < renderers.Length; ++i)
			{
				Renderer renderer = renderers[i];
				if (renderer == null) continue;
				Material[] materials = renderer.sharedMaterials;
				if (materials == null) continue;
				bool wheel = false, wheelKnown = false, changed = false;
				for (int j = 0; j < materials.Length; ++j)
				{
					Material material = materials[j];
					if (material == null) continue;
					// a slot still holding one of our instances (re-dressed model): back to its source first
					Material source;
					if (s_rimSource.TryGetValue(material, out source)) { material = materials[j] = source; changed = true; }
					if (index == 0) continue;
					string name = material.name;
					bool rim, tyre;
					if (hover)
					{
						rim = Listed(name, HoverAccents);   // hover cars have no wheels: piping / pad lights take the rim colour
						tyre = false;
					}
					else
					{
						bool rimName = Listed(name, RimMaterials), tyreName = Listed(name, TyreMaterials);
						if ((rimName || tyreName) && !wheelKnown) { wheel = UnderWheel(renderer.transform, root, definition); wheelKnown = true; }
						rim = rimName && (wheel || Listed(name, RimsAnywhere));
						tyre = tyreName && (wheel || Listed(name, TyresAnywhere));
					}
					if (rim)
					{
						if (!hover && RimFinish(index)) { materials[j] = RimInstance(material, index); changed = true; }
						TintRim(renderer, j, index);
					}
					else if (tyre) Tint(renderer, j, item.b);
				}
				if (changed) renderer.sharedMaterials = materials;
			}
		}

		// ---- flags: cloth on a chrome pole, in the world (player car and garage preview) ---------------
		private const int BannerCols = 12, BannerRows = 6;

		private struct FlagInstance
		{
			/* [user request 2026-09-29: flags and trails must sit on the car] The flag hangs off the car's
			   PARENT (the player root / the turntable holder), but the car itself is moved inside that parent
			   every frame: ShadowRunPrototype.Player rewrites carVisual's localPosition and localRotation for
			   drift yaw (up to +-22 degrees), body roll, jump height and the hover bob, and the run rebuild
			   re-centres the model after Dress has already measured it. A flag pinned to the parent therefore
			   stands still while the car slides and turns out from under it - which is exactly the "flag next
			   to the car" the player sees. Keeping the anchor in the MODEL's space and re-deriving the pole's
			   local pose every frame makes it ride the car without reparenting it (reparenting would put it
			   under the fit scale, flip the -z convention and hand it to the garage's material pass). */
			public Transform model;          // the car body this flag belongs to
			public Vector3 anchor;           // pole foot in the model's own space
			public Quaternion restRotation;  // the model's rotation when the flag was fitted
			public Transform pivot;
			public Quaternion base_;
			public float phase;
			public Mesh mesh;
			public Vector3[] rest;     // the flat cloth
			public Vector3[] verts;    // the rippled cloth written every frame
			public Renderer renderer;
			public float amplitude;    // metres at the free edge
			public float length;       // cloth width in metres
		}

		private static readonly FlagInstance[] s_flags = new FlagInstance[4];
		private static Material s_poleMaterial;
		private static readonly Material[] s_flagMaterials = new Material[8];

		private static Material PoleMaterial(ShadowRunPrototype game)
		{
			if (s_poleMaterial == null)
			{
				s_poleMaterial = game.Mat(new Color(0.80f, 0.80f, 0.86f, 1f));   // original: MatUnlit(0xD8D0E8)
				if (s_poleMaterial.HasProperty("_Metallic")) s_poleMaterial.SetFloat("_Metallic", 0.9f);
				if (s_poleMaterial.HasProperty("_Smoothness")) s_poleMaterial.SetFloat("_Smoothness", 0.85f);
			}
			return s_poleMaterial;
		}

		// The cloth: URP Lit, both faces (the ripple shows either side), the pattern as the
		// base map and, faintly, as emission so it still reads under the night maps' dim keys.
		private static Material FlagMaterial(ShadowRunPrototype game, int index)
		{
			if (s_flagMaterials[index] == null)
			{
				Texture2D texture = FlagTexture(index);
				Material material = game.Mat(new Color(1f, 1f, 1f, 1f));   // original: MatTransparentUnlit(0xFFFFFF, texture, false)
				if (material.HasProperty("_BaseMap")) material.SetTexture("_BaseMap", texture);
				if (material.HasProperty("_MainTex")) material.SetTexture("_MainTex", texture);
				if (material.HasProperty("_Cull")) material.SetFloat("_Cull", 0f);
				if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.25f);
				material.EnableKeyword("_EMISSION");
				if (material.HasProperty("_EmissionMap")) material.SetTexture("_EmissionMap", texture);
				if (material.HasProperty("_EmissionColor")) material.SetColor("_EmissionColor", new Color(0.35f, 0.35f, 0.35f, 1f));
				s_flagMaterials[index] = material;
			}
			return s_flagMaterials[index];
		}

		private static GameObject AddPrimitive(ShadowRunPrototype game, Transform parent, PrimitiveType type, string name, Vector3 localPos, Vector3 scale, Material mat)
		{
			GameObject go = GameObject.CreatePrimitive(type);
			go.name = name;
			go.transform.SetParent(parent, false);
			go.transform.localPosition = localPos;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = scale;
			game.PreparePrimitive(go, mat);   // shared material, no shadows / probes, collider removed
			return go;
		}

		// A 12x6 grid of quads, x 0 (pole) .. width (free edge), y centred, facing +z (the
		// camera behind the car). The pole edge maps to the texture's right edge (FlagPixel).
		private static Mesh MakeBannerMesh(float width, float height, out Vector3[] rest)
		{
			int nx = BannerCols + 1, ny = BannerRows + 1;
			rest = new Vector3[nx * ny];
			Vector2[] uv = new Vector2[nx * ny];
			int[] triangles = new int[BannerCols * BannerRows * 6];
			for (int j = 0; j < ny; ++j)
				for (int i = 0; i < nx; ++i)
				{
					float fx = i / (float)BannerCols, fy = j / (float)BannerRows;
					rest[j * nx + i] = new Vector3(fx * width, (fy - 0.5f) * height, 0f);
					uv[j * nx + i] = new Vector2(1f - fx, fy);
				}
			int t = 0;
			for (int j = 0; j < BannerRows; ++j)
				for (int i = 0; i < BannerCols; ++i)
				{
					int a = j * nx + i, b = a + 1, c = a + nx, d = c + 1;
					triangles[t++] = a; triangles[t++] = b; triangles[t++] = c;   // clockwise seen from +z
					triangles[t++] = b; triangles[t++] = d; triangles[t++] = c;
				}
			Mesh mesh = new Mesh();
			mesh.name = "StyleBanner";
			mesh.MarkDynamic();
			mesh.vertices = rest;
			mesh.uv = uv;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			Bounds bounds = mesh.bounds;
			bounds.Expand(new Vector3(0f, 0f, 0.4f));   // the ripple moves vertices in z; the bounds stay put
			mesh.bounds = bounds;
			return mesh;
		}

		private static void ReleaseFlag(int slot)
		{
			if (s_flags[slot].mesh != null) Object.Destroy(s_flags[slot].mesh);
			s_flags[slot] = default;
		}

		private static void ReleaseFlagOf(Transform flagRoot)
		{
			for (int i = 0; i < s_flags.Length; i++)
			{
				if (ReferenceEquals(s_flags[i].pivot, null)) continue;
				if (s_flags[i].pivot == null || s_flags[i].pivot.parent == flagRoot) ReleaseFlag(i);
			}
		}

		private static void DressFlag(ShadowRunPrototype game, Transform model, VehicleDefinition definition, bool preview)
		{
			Transform parent = model.parent;
			if (parent == null) return;
			// one flag per car: drop the previous one (the game rebuilds models freely)
			Transform old = parent.Find("StyleFlag");
			if (old != null) { ReleaseFlagOf(old); Object.Destroy(old.gameObject); }
			int index = Selected(Flags);
			if (index == 0) return;
			/* [user request 2026-09-29: flags and trails must sit on the car] measure the body, not the effects
			   original: if (!game.TryGetActiveRendererBounds(model, out world, ...)) return; local = game.MakeLocalBounds(parent, world); */
			Bounds local;
			if (!TryGetBodyBounds(game, model, parent, definition, out local))
			{
				Bounds world;   // a model with no mesh filters at all: fall back rather than drop the flag
				if (!game.TryGetActiveRendererBounds(model, out world, definition != null ? definition.BoundsRendererNameKeywords : null)) return;
				local = game.MakeLocalBounds(parent, world);
			}
			Vector3 c = local.center, e = local.extents;
			// the model faces -z in its parent's space (that is what ModelYawOffset is for): rear = +z, left = -x
			float scale = Mathf.Max(0.4f, Mathf.Min(1.6f, e.z / 2.3f));
			// the garage preview camera sits close and the card carousel covers the top of the screen: a
			// shorter pole keeps the flag inside the turntable view instead of poking up behind the cards
			if (preview) scale *= 0.6f;
			float pole = 0.95f * scale, cloth = 0.56f * scale, drop = 0.36f * scale;
			Vector3 foot = new Vector3(c.x - e.x * 0.62f, c.y + e.y - 0.02f * scale, c.z + e.z * 0.80f);
			GameObject root = new GameObject("StyleFlag");
			Transform rootTransform = root.transform;
			rootTransform.SetParent(parent, false);
			rootTransform.localPosition = foot;
			rootTransform.localRotation = new Quaternion(0f, 0f, 0f, 1f);
			rootTransform.localScale = new Vector3(1f, 1f, 1f);
			// chrome pole: a cylinder (2 units tall) and a sphere cap   // original: two AddCubes with MatUnlit
			AddPrimitive(game, rootTransform, PrimitiveType.Cylinder, "Pole", new Vector3(0f, pole / 2f, 0f), new Vector3(0.035f * scale, pole / 2f, 0.035f * scale), PoleMaterial(game));
			AddPrimitive(game, rootTransform, PrimitiveType.Sphere, "Knob", new Vector3(0f, pole, 0f), new Vector3(0.07f * scale, 0.07f * scale, 0.07f * scale), PoleMaterial(game));
			// the cloth hangs from a pivot at the pole top and swings about the pole; the mesh
			// already faces +z with its pole edge at x = 0, so the pivot needs no turn
			GameObject pivotObject = new GameObject("Cloth");
			Transform pivot = pivotObject.transform;
			pivot.SetParent(rootTransform, false);
			pivot.localPosition = new Vector3(0f, pole - drop / 2f, 0f);
			Quaternion base_ = new Quaternion(0f, 0f, 0f, 1f);   // original: (0, 1, 0, 0) — the mirrored quad had to be turned
			pivot.localRotation = base_;
			pivot.localScale = new Vector3(1f, 1f, 1f);
			Vector3[] rest;
			Mesh mesh = MakeBannerMesh(cloth, drop, out rest);
			GameObject banner = new GameObject("Banner");
			banner.transform.SetParent(pivot, false);
			banner.transform.localPosition = new Vector3(0f, 0f, 0f);
			banner.transform.localRotation = new Quaternion(0f, 0f, 0f, 1f);
			banner.transform.localScale = new Vector3(1f, 1f, 1f);
			banner.AddComponent<MeshFilter>().sharedMesh = mesh;
			MeshRenderer renderer = banner.AddComponent<MeshRenderer>();
			renderer.sharedMaterial = FlagMaterial(game, index);
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			// register for waving (slot: reuse a dead entry)
			for (int i = 0; i < s_flags.Length; i++)
			{
				if (!ReferenceEquals(s_flags[i].pivot, null) && s_flags[i].pivot != null) continue;   // Object.op_Inequality: alive Unity object
				if (!ReferenceEquals(s_flags[i].pivot, null)) ReleaseFlag(i);
				s_flags[i].model = model; s_flags[i].anchor = model.InverseTransformPoint(parent.TransformPoint(foot)); s_flags[i].restRotation = model.localRotation;   /* [user request 2026-09-29: flags and trails must sit on the car] */
				s_flags[i].pivot = pivot; s_flags[i].base_ = base_; s_flags[i].phase = preview ? 1.7f : 0.0f;
				s_flags[i].mesh = mesh; s_flags[i].rest = rest; s_flags[i].verts = (Vector3[])rest.Clone();
				s_flags[i].renderer = renderer; s_flags[i].amplitude = 0.07f * scale; s_flags[i].length = cloth;
				break;
			}
		}

		/* [user request 2026-09-29: flags and trails must sit on the car] Anchor bounds for the flag pole and
		   the trail ribbons, measured from the body meshes only.

		   The game's TryGetActiveRendererBounds unions every renderer's WORLD AABB and MakeLocalBounds then
		   re-boxes that into the parent's space, which is wrong for this job twice over. First, the trail
		   ribbons are LineRenderers parented to the car, so a car already wearing a trail measures far bigger
		   than its body: on the sportcar the old path returned extents (2.45, 0.73, 3.28) against the body's
		   real (0.76, 0.53, 1.34), which put the pole foot at x -1.24 when the body's left flank is at -0.76 -
		   half a metre out in the air beside the car, and roughly three metres behind it. Every re-dress fed
		   the previous trail back in, so it drifted further each time. Second, a world AABB of a car turning
		   on the garage turntable is wider than the car at every angle but a right one, so the same car
		   anchored differently depending on where the turntable happened to be.

		   Measuring each body mesh in its own local space and mapping the corners into the parent gives the
		   same answer at any yaw, and cannot be inflated by the styles we attach ourselves. */
		private static bool TryGetBodyBounds(ShadowRunPrototype game, Transform model, Transform parent, VehicleDefinition definition, out Bounds local)
		{
			local = default(Bounds);
			string[] keywords = definition != null ? definition.BoundsRendererNameKeywords : null;
			Vector3 min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
			Vector3 max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
			bool found = false;
			Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < renderers.Length; i++)
			{
				Renderer renderer = renderers[i];
				if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
				{
					continue;
				}
				if (renderer is LineRenderer || renderer is TrailRenderer || renderer is ParticleSystemRenderer)
				{
					continue;   // ribbons, exhaust smoke, skid marks: effects that follow the car, not parts of it
				}
				if (IsStyleAttachment(renderer.transform))
				{
					continue;   // our own pole and cloth
				}
				if (!game.RendererMatchesBoundsKeywords(renderer, keywords))
				{
					continue;
				}
				Bounds meshBounds;
				SkinnedMeshRenderer skinned = renderer as SkinnedMeshRenderer;
				if (skinned != null && skinned.sharedMesh != null)
				{
					meshBounds = skinned.sharedMesh.bounds;
				}
				else
				{
					MeshFilter filter = renderer.GetComponent<MeshFilter>();
					if (filter == null || filter.sharedMesh == null)
					{
						continue;
					}
					meshBounds = filter.sharedMesh.bounds;
				}
				Vector3 lo = meshBounds.min, hi = meshBounds.max;
				for (int corner = 0; corner < 8; corner++)
				{
					Vector3 point = new Vector3((corner & 1) == 0 ? lo.x : hi.x, (corner & 2) == 0 ? lo.y : hi.y, (corner & 4) == 0 ? lo.z : hi.z);
					point = parent.InverseTransformPoint(renderer.transform.TransformPoint(point));
					min = Vector3.Min(min, point);
					max = Vector3.Max(max, point);
				}
				found = true;
			}
			if (!found)
			{
				return false;
			}
			local = new Bounds((min + max) * 0.5f, max - min);
			/* [user request 2026-09-29: flags and trails must sit on the car] Clamp to the size the car was
			   fitted to. Some models carry geometry that is nowhere near the body and is not an effect the
			   filter above can spot: the sportcar's headlight glow discs are 5.4 x 4.4 units, the Cayman
			   prefab holds an 86-unit Icosphere that the runtime artifact filter lets through, and the flying
			   car has a child literally named Plane that only the garage disables. FitImportedVehicleModel
			   has already scaled the body to TargetModelSize, so anything beyond that box is not the car;
			   half the target plus a fifth of tolerance keeps real bodywork (mirrors, wings) and drops the
			   rest. Without this the pole is planted wherever the widest stray quad reaches. */
			Vector3 target = definition != null ? definition.TargetModelSize : Vector3.zero;
			if (target.x > 0.01f && target.y > 0.01f && target.z > 0.01f)
			{
				Vector3 cap = new Vector3(target.x, target.y, target.z) * 0.6f;
				Vector3 centre = local.center, extents = local.extents;
				centre = new Vector3(Mathf.Clamp(centre.x, -cap.x, cap.x), Mathf.Clamp(centre.y, -cap.y, cap.y), Mathf.Clamp(centre.z, -cap.z, cap.z));
				extents = new Vector3(Mathf.Min(extents.x, cap.x), Mathf.Min(extents.y, cap.y), Mathf.Min(extents.z, cap.z));
				local = new Bounds(centre, extents * 2f);
			}
			return true;
		}

		// "StyleFlag" / "StyleTrail" and everything under them belong to us and must never measure the car.
		private static bool IsStyleAttachment(Transform candidate)
		{
			for (Transform t = candidate; t != null; t = t.parent)
			{
				if (t.name == "StyleFlag" || t.name == "StyleTrail")
				{
					return true;
				}
			}
			return false;
		}

		private static Quaternion YawQuat(float degrees)
		{
			float half = degrees * 3.14159265f / 360.0f;
			return new Quaternion(0f, Mathf.Sin(half), 0f, Mathf.Cos(half));
		}

		// Every frame (menu and run): flags swing about their pole and their cloth ripples
		// (z += amplitude * sin(6x - 9t) * x, x = 0 at the pole .. 1 at the free edge; one
		// SetVertices + RecalculateNormals per visible banner, no allocations).
		private static void Wave(ShadowRunPrototype game)
		{
			float t = Time.unscaledTime;
			for (int i = 0; i < s_flags.Length; i++)
			{
				if (ReferenceEquals(s_flags[i].pivot, null)) continue;
				if (s_flags[i].pivot == null) { ReleaseFlag(i); continue; }   // destroyed with its car
				/* [user request 2026-09-29: flags and trails must sit on the car] ride the body */
				Transform flagModel = s_flags[i].model, flagRoot = s_flags[i].pivot.parent;
				if (flagModel != null && flagRoot != null)
				{
					// The anchor is a point in the model, so this is just its live position in the parent;
					// the rotation is the DELTA since fitting, which keeps the pole upright and the banner
					// facing the way it was built while still turning with drift, roll, jumps and the hover.
					flagRoot.localPosition = flagModel.localPosition + flagModel.localRotation * Vector3.Scale(flagModel.localScale, s_flags[i].anchor);
					flagRoot.localRotation = InfernoHazards.Mul(flagModel.localRotation, Quaternion.Inverse(s_flags[i].restRotation));
				}
				float phase = s_flags[i].phase;
				float swing = 9.0f * Mathf.Sin(t * 5.5f + phase) + 3.0f * Mathf.Sin(t * 11.0f + phase * 2f);   // original: 16 / 5 degrees at 7.5 / 13 (the cloth ripples itself now)
				s_flags[i].pivot.localRotation = InfernoHazards.Mul(s_flags[i].base_, YawQuat(swing));
				Renderer renderer = s_flags[i].renderer;
				if (renderer == null || !renderer.isVisible || s_flags[i].mesh == null) continue;
				Vector3[] verts = s_flags[i].verts, rest = s_flags[i].rest;
				float amplitude = s_flags[i].amplitude, inverseLength = 1f / Mathf.Max(0.001f, s_flags[i].length);
				for (int k = 0; k < verts.Length; ++k)
				{
					float x = rest[k].x * inverseLength;
					verts[k].x = rest[k].x;
					verts[k].y = rest[k].y;
					verts[k].z = amplitude * Mathf.Sin(6f * x - 9f * t + phase) * x;
				}
				s_flags[i].mesh.SetVertices(verts);
				s_flags[i].mesh.RecalculateNormals();
			}
		}

		// ---- trails -----------------------------------------------------------------------------------
		// Per side: a textured ribbon whose width tapers 0.42 -> 0.06 m and whose alpha fades
		// 0.95 -> 0 over 16 points x 0.5 m (original: 12 x 0.55 m, a flat 0.36 glow), a thin
		// white-hot core and a spark emitter at the head. The materials are additive URP
		// Unlit in HDR colours (item.a x 2.2) so the run camera's bloom picks them up.
		private const int TrailPoints = 16;                                                 // original: 12
		private const float TrailStep = 0.5f;                                               // original: 0.55f
		private const float TrailHeadWidth = 0.42f, TrailTailWidth = 0.06f, TrailCoreWidth = 0.08f;   // original: TrailGlowWidth = 0.36f, TrailCoreWidth = 0.10f
		private const float TrailHdr = 2.2f;
		private const float SparkRate = 40f, SparkLife = 0.35f, SparkMinSize = 0.03f, SparkMaxSize = 0.06f;

		private struct Trail
		{
			public LineRenderer glow;
			public LineRenderer core;
			public ParticleSystem sparks;
			public Vector3[] points;
			public bool visible;
		}

		private static readonly Trail[] s_trails = new Trail[2];
		/* [user request 2026-09-29: flags and trails must sit on the car] The run's ribbons used to leave the
		   car from a fixed (+-0.78, 0.22, +1.55) around the player. Fitted body half-widths are 0.92 for the
		   sportcar and the flying car but 0.79 for the Cayman, 0.74 for the P900 and 0.73 for the W222, so on
		   those three the ribbons came out of thin air beside the doors; the fixed 0.22 lift ignored
		   RoadYOffset, leaving the flying cars (0.78 + hover) trailing well below themselves; and the fixed
		   1.55 ignored lengths that run 3.11 - 3.64 m. These are re-measured from the body whenever the car
		   model changes, and the offsets turn with the car so a drifting car keeps its ribbons at its own
		   rear corners. */
		private static Transform s_trailCar;
		private static Bounds s_trailBody;
		private static bool s_trailBodyKnown;
		private static Material s_trailGlow;
		private static Material s_trailCore;
		private static Material s_trailSpark;
		private static int s_trailStyle = -1;
		private static ShadowRunPrototype s_owner;
		private static bool s_trailPrimed;

		private static Color Hdr(float[] rgb, float boost) { return new Color(rgb[0] * boost, rgb[1] * boost, rgb[2] * boost, 1f); }

		private static Color HotCore(float[] rgb, float boost)
		{
			return new Color(Mathf.Min(1f, rgb[0] * 0.35f + 0.65f) * boost, Mathf.Min(1f, rgb[1] * 0.35f + 0.65f) * boost, Mathf.Min(1f, rgb[2] * 0.35f + 0.65f) * boost, 1f);
		}

		// Ribbon look on a fresh AddLine: stretched streak texture, width taper, alpha fade.
		private static void Ribbon(LineRenderer line, Color colour, float headWidth, float tailWidth)
		{
			line.textureMode = LineTextureMode.Stretch;
			line.widthMultiplier = 1f;
			line.widthCurve = AnimationCurve.Linear(0f, headWidth, 1f, tailWidth);
			Gradient gradient = new Gradient();
			gradient.SetKeys(
				new[] { new GradientColorKey(colour, 0f), new GradientColorKey(colour, 1f) },
				new[] { new GradientAlphaKey(0.95f, 0f), new GradientAlphaKey(0f, 1f) });
			line.colorGradient = gradient;
			line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			line.receiveShadows = false;
			line.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			line.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
		}

		// Sparks: a cone at the ribbon head pointing up and back; they shrink over their life
		// (the additive Unlit material ignores particle colour, so size does the fading) and
		// TickTrails gives them the road's speed so they fall behind the car.
		private static ParticleSystem MakeSparks(Transform parent, string name, Material material)
		{
			GameObject go = new GameObject(name);
			go.transform.SetParent(parent, false);
			ParticleSystem sparks = go.AddComponent<ParticleSystem>();
			ParticleSystem.MainModule main = sparks.main;
			main.loop = true;
			main.playOnAwake = false;
			main.startLifetime = SparkLife;
			main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 4f);
			main.startSize = new ParticleSystem.MinMaxCurve(SparkMinSize, SparkMaxSize);
			main.startColor = material.color;
			main.gravityModifier = 0.8f;
			main.simulationSpace = ParticleSystemSimulationSpace.World;
			main.scalingMode = ParticleSystemScalingMode.Local;
			main.maxParticles = 64;
			ParticleSystem.EmissionModule emission = sparks.emission;
			emission.enabled = true;
			emission.rateOverTime = SparkRate;
			ParticleSystem.ShapeModule shape = sparks.shape;
			shape.enabled = true;
			shape.shapeType = ParticleSystemShapeType.Cone;
			shape.angle = 28f;
			shape.radius = 0.06f;
			shape.rotation = new Vector3(-25f, 0f, 0f);   // the cone's +z tilted up: back and up from the wheel
			ParticleSystem.VelocityOverLifetimeModule velocity = sparks.velocityOverLifetime;
			velocity.enabled = true;
			velocity.space = ParticleSystemSimulationSpace.World;
			velocity.z = 0f;
			ParticleSystem.SizeOverLifetimeModule size = sparks.sizeOverLifetime;
			size.enabled = true;
			size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));
			ParticleSystemRenderer renderer = go.GetComponent<ParticleSystemRenderer>();
			renderer.renderMode = ParticleSystemRenderMode.Billboard;
			renderer.sharedMaterial = material;
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			sparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			return sparks;
		}

		private static void TrailVisible(bool on)
		{
			for (int i = 0; i < s_trails.Length; i++)
			{
				if (s_trails[i].visible == on) continue;
				s_trails[i].visible = on;
				if (s_trails[i].glow != null) s_trails[i].glow.gameObject.SetActive(on);
				if (s_trails[i].core != null) s_trails[i].core.gameObject.SetActive(on);
				if (s_trails[i].sparks != null)
				{
					s_trails[i].sparks.gameObject.SetActive(on);
					if (on) s_trails[i].sparks.Play(true);
				}
			}
		}

		private static void PauseSparks()
		{
			for (int i = 0; i < s_trails.Length; i++)
				if (s_trails[i].sparks != null && s_trails[i].visible && s_trails[i].sparks.isPlaying) s_trails[i].sparks.Pause(true);
		}

		private static void DisposeTrails()
		{
			for (int i = 0; i < s_trails.Length; i++)
			{
				if (s_trails[i].glow != null) Object.Destroy(s_trails[i].glow.gameObject);
				if (s_trails[i].core != null) Object.Destroy(s_trails[i].core.gameObject);
				if (s_trails[i].sparks != null) Object.Destroy(s_trails[i].sparks.gameObject);
				s_trails[i] = default;
			}
			if (s_trailGlow != null) Object.Destroy(s_trailGlow);
			if (s_trailCore != null) Object.Destroy(s_trailCore);
			if (s_trailSpark != null) Object.Destroy(s_trailSpark);
			s_trailGlow = s_trailCore = s_trailSpark = null;
			s_trailStyle = -1;
		}

		private static void BuildTrails(ShadowRunPrototype game, int style)
		{
			DisposeTrails();
			Item item = TrailItems[style];
			Color glow = Hdr(item.a, TrailHdr), hot = HotCore(item.a, 1.6f);
			s_trailGlow = game.MatTransparentUnlit(glow, StreakTexture(), true);   // original: flat colour, alpha 0.22, no texture
			s_trailCore = game.MatTransparentUnlit(hot, StreakTexture(), true);
			s_trailSpark = game.MatTransparentUnlit(Hdr(item.a, 1.8f), SoftTexture(), true);
			Transform parent = game.transform;
			for (int side = 0; side < 2; ++side)
			{
				s_trails[side].points = new Vector3[TrailPoints];
				for (int i = 0; i < TrailPoints; ++i) s_trails[side].points[i] = new Vector3(0f, -5f, 0f);
				s_trails[side].glow = game.AddLine(parent, side != 0 ? "TrailGlowR" : "TrailGlowL", s_trails[side].points, TrailHeadWidth, s_trailGlow);
				Ribbon(s_trails[side].glow, glow, TrailHeadWidth, TrailTailWidth);
				s_trails[side].core = game.AddLine(parent, side != 0 ? "TrailCoreR" : "TrailCoreL", s_trails[side].points, TrailCoreWidth, s_trailCore);
				Ribbon(s_trails[side].core, hot, TrailCoreWidth, TrailCoreWidth * 0.25f);
				s_trails[side].sparks = MakeSparks(parent, side != 0 ? "TrailSparksR" : "TrailSparksL", s_trailSpark);
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
			/* [user request 2026-09-29: flags and trails must sit on the car] measure this car once per model */
			Transform car = game.carVisual;
			if (car != null && car != s_trailCar)
			{
				s_trailCar = car;
				Bounds measured;
				s_trailBodyKnown = TryGetBodyBounds(game, car, car.parent != null ? car.parent : car, game.ActiveVehicle, out measured);
				s_trailBody = measured;
			}
			// 0.62 of the half-width is the same inset the flag pole uses: just inside the flank, not on it
			float halfWidth = s_trailBodyKnown ? Mathf.Max(0.30f, s_trailBody.extents.x * 0.62f) : 0.78f;
			float rear = s_trailBodyKnown ? s_trailBody.center.z + s_trailBody.extents.z * 0.92f : 1.55f;
			float floor = s_trailBodyKnown ? Mathf.Clamp(s_trailBody.center.y - s_trailBody.extents.y + 0.10f, 0.06f, 2.2f) : 0.22f;
			// the car's own axes, so drift yaw, roll and the hover carry the ribbon roots with the body
			Vector3 sideways = car != null ? -(car.localRotation * Vector3.right) : Vector3.right;
			Vector3 backwards = car != null ? -(car.localRotation * Vector3.forward) : Vector3.forward;
			sideways.y = 0f; backwards.y = 0f;
			if (sideways.sqrMagnitude < 0.0001f) sideways = Vector3.right; else sideways.Normalize();
			if (backwards.sqrMagnitude < 0.0001f) backwards = Vector3.forward; else backwards.Normalize();
			/* original: float lift = (game.playerAirborne ? game.playerJumpHeight : 0.0f) + 0.22f; */
			float lift = (game.playerAirborne ? game.playerJumpHeight : 0.0f) + floor;
			float moved = game.speed * dt;
			bool show = game.speed > 6f && game.lifeHalfUnits > 0;
			for (int side = 0; side < 2; ++side)
			{
				Vector3[] points = s_trails[side].points;
				/* original: Vector3 head = new Vector3(game.playerX + (side != 0 ? 0.78f : -0.78f), lift, InfernoHazards.PlayerZ + 1.55f); */
				Vector3 offset = sideways * (side != 0 ? halfWidth : -halfWidth) + backwards * rear;
				Vector3 head = new Vector3(game.playerX + offset.x, lift, InfernoHazards.PlayerZ + offset.z);
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
				ParticleSystem sparks = s_trails[side].sparks;
				if (sparks != null)
				{
					sparks.transform.localPosition = new Vector3(head.x, head.y - 0.15f, head.z);
					ParticleSystem.VelocityOverLifetimeModule velocity = sparks.velocityOverLifetime;
					velocity.z = game.speed * 0.85f;   // carried back with the road (a little slower: they drag)
					if (show && sparks.isPaused) sparks.Play(true);
				}
			}
			s_trailPrimed = true;
			TrailVisible(show);
		}

		// ---- the garage's static ribbon ----------------------------------------------------------
		// A ~3 m glow behind the preview car (under the model's offset node, so it takes
		// layer 29 with the car and goes when the model is rebuilt) so a chosen trail shows
		// in the garage. Its materials are separate from the run's (those come and go with
		// ResetTrails) and follow the selected style.
		private static Material s_previewGlow, s_previewCore;
		private static int s_previewStyle = -1;

		private static void DisposePreviewMaterials()
		{
			if (s_previewGlow != null) Object.Destroy(s_previewGlow);
			if (s_previewCore != null) Object.Destroy(s_previewCore);
			s_previewGlow = s_previewCore = null;
			s_previewStyle = -1;
		}

		private static void DressPreviewTrail(ShadowRunPrototype game, Transform model, VehicleDefinition definition)
		{
			Transform parent = model.parent;
			if (parent == null) return;
			Transform old = parent.Find("StyleTrail");
			if (old != null) Object.Destroy(old.gameObject);
			int index = Selected(Trails);
			if (index == 0) return;
			/* [user request 2026-09-29: flags and trails must sit on the car] the ribbons used to be measured
			   with the previous ribbons included, so each re-dress pushed them further out to the sides
			   original: if (!game.TryGetActiveRendererBounds(model, out world, ...)) return; local = game.MakeLocalBounds(parent, world); */
			Bounds local;
			if (!TryGetBodyBounds(game, model, parent, definition, out local))
			{
				Bounds world;
				if (!game.TryGetActiveRendererBounds(model, out world, definition != null ? definition.BoundsRendererNameKeywords : null)) return;
				local = game.MakeLocalBounds(parent, world);
			}
			Vector3 c = local.center, e = local.extents;
			if (index != s_previewStyle)
			{
				DisposePreviewMaterials();
				Item item = TrailItems[index];
				// the preview camera has no HDR or bloom: a milder boost keeps the hue instead of clipping to white
				s_previewGlow = game.MatTransparentUnlit(Hdr(item.a, 1.25f), StreakTexture(), true);
				s_previewCore = game.MatTransparentUnlit(HotCore(item.a, 1.0f), StreakTexture(), true);
				s_previewStyle = index;
			}
			// the preview car is fitted to a ~2.5 m footprint (half-length ~1.25 m); the run
			// car is 3.64 m long, so widths and the ribbon length scale by e.z / 1.82
			float k = Mathf.Max(0.4f, Mathf.Min(1.6f, e.z / 1.82f));
			GameObject root = new GameObject("StyleTrail");
			Transform rootTransform = root.transform;
			rootTransform.SetParent(parent, false);
			rootTransform.localPosition = new Vector3(0f, 0f, 0f);
			rootTransform.localRotation = new Quaternion(0f, 0f, 0f, 1f);
			rootTransform.localScale = new Vector3(1f, 1f, 1f);
			const int ribbonPoints = 8;
			for (int side = 0; side < 2; ++side)
			{
				float x = c.x + (side != 0 ? 1f : -1f) * e.x * 0.62f;
				float y = c.y - e.y + 0.12f * k;
				float z0 = c.z + e.z * 0.92f;
				Vector3[] points = new Vector3[ribbonPoints];
				for (int i = 0; i < ribbonPoints; ++i) points[i] = new Vector3(x, y, z0 + i * (3.0f * k / (ribbonPoints - 1)));
				LineRenderer glow = game.AddLine(rootTransform, side != 0 ? "RibbonR" : "RibbonL", points, TrailHeadWidth * k, s_previewGlow);
				Ribbon(glow, Hdr(TrailItems[index].a, 1.25f), TrailHeadWidth * k, TrailTailWidth * k);
				LineRenderer core = game.AddLine(rootTransform, side != 0 ? "RibbonCoreR" : "RibbonCoreL", points, TrailCoreWidth * k, s_previewCore);
				Ribbon(core, HotCore(TrailItems[index].a, 1.0f), TrailCoreWidth * k, TrailCoreWidth * k * 0.25f);
			}
		}

		public static void Dress(ShadowRunPrototype game, GameObject instance, VehicleDefinition definition, bool preview)
		{
			if (instance == null) return;
			Transform model = instance.transform;
			DressTires(game, model, definition);
			DressFlag(game, model, definition, preview);
			if (preview) DressPreviewTrail(game, model, definition);
		}

		// ---- swatch art (procedural sprites, cached for the session) --------------------------------
		private static Sprite s_streakSprite, s_softSprite, s_tyreSprite, s_rimSprite, s_ballSprite, s_shineSprite, s_circle;

		private static Sprite StreakSprite() { if (s_streakSprite == null) s_streakSprite = SpriteOf(StreakTexture()); return s_streakSprite; }
		private static Sprite SoftSprite() { if (s_softSprite == null) s_softSprite = SpriteOf(SoftTexture()); return s_softSprite; }

		// A tyre ring (outer radius 0.98, inner 0.66 of the texture) with a faint tread and
		// the upper-left light; white, tinted by the item's tyre colour.
		private static void TyrePixel(ref Color32 px, float u, float v)
		{
			float r = Mathf.Sqrt(u * u + v * v);
			float alpha = Mathf.Clamp01((0.98f - r) * 24f) * Mathf.Clamp01((r - 0.66f) * 24f);
			float tread = 0.88f + 0.12f * (((int)((Mathf.Atan2(v, u) + 3.1416f) * 4.77f)) & 1);
			int grey = (int)(255f * (0.62f + 0.38f * Lit(u, v)) * tread);
			InfernoHazards.Set(ref px, grey, grey, grey, (int)(255f * alpha));
		}

		// A five-spoke rim (disc radius 0.64: it fills the tyre's hole) with a metallic
		// highlight band; white, tinted by the rim colour. Between the spokes the chip shows through.
		private static void RimPixel(ref Color32 px, float u, float v)
		{
			float r = Mathf.Sqrt(u * u + v * v);
			float outer = Mathf.Clamp01((0.64f - r) * 24f);
			float lip = Mathf.Clamp01((r - 0.50f) * 24f);
			const float sector = 2f * 3.14159265f / 5f;
			float spoke = Mathf.Abs(Mathf.Repeat(Mathf.Atan2(v, u) + sector * 0.5f, sector) - sector * 0.5f);   // angle to the nearest spoke
			float spokes = Mathf.Clamp01((0.20f + 0.16f * (1f - r / 0.64f) - spoke) * 12f);
			float hub = Mathf.Clamp01((0.22f - r) * 24f);
			float alpha = outer * Mathf.Max(lip, Mathf.Max(spokes, hub));
			float band = 0.35f * Mathf.Exp(-((u + v) * 2.2f) * ((u + v) * 2.2f));
			int grey = (int)(255f * Mathf.Min(1f, 0.45f + 0.55f * Lit(u, v) + band));
			InfernoHazards.Set(ref px, grey, grey, grey, (int)(255f * alpha));
		}

		// A lit sphere (radius 0.97) for the paint swatches; white, tinted by the colour.
		private static void BallPixel(ref Color32 px, float u, float v)
		{
			float rr = u * u + v * v, r = Mathf.Sqrt(rr);
			float nz = Mathf.Sqrt(Mathf.Max(0f, 1f - Mathf.Min(1f, rr)));
			float diffuse = Mathf.Max(0f, -u * 0.40f + v * 0.62f + nz * 0.68f);
			float shade = (0.16f + 0.84f * diffuse) * (1f - 0.35f * Mathf.Pow(Mathf.Clamp01(r), 6f));
			int grey = (int)(255f * Mathf.Min(1f, shade));
			InfernoHazards.Set(ref px, grey, grey, grey, (int)(255f * Mathf.Clamp01((0.97f - r) * 24f)));
		}

		// The sphere's specular highlight and a soft window reflection, drawn over the ball.
		private static void ShinePixel(ref Color32 px, float u, float v)
		{
			float rr = u * u + v * v, r = Mathf.Sqrt(rr);
			float nz = Mathf.Sqrt(Mathf.Max(0f, 1f - Mathf.Min(1f, rr)));
			float ndh = Mathf.Max(0f, -u * 0.22f + v * 0.34f + nz * 0.92f);
			float spec = Mathf.Pow(ndh, 48f);
			float window = 0.28f * Mathf.Exp(-((v - 0.55f) * 3.5f) * ((v - 0.55f) * 3.5f)) * Mathf.Exp(-((u + 0.3f) * 2.2f) * ((u + 0.3f) * 2.2f));
			float alpha = Mathf.Clamp01(spec * 0.95f + window) * Mathf.Clamp01((0.97f - r) * 24f);
			InfernoHazards.Set(ref px, 255, 255, 255, (int)(255f * alpha));
		}

		private static Sprite TyreSprite() { if (s_tyreSprite == null) s_tyreSprite = SpriteOf(InfernoHazards.MakeTexture(96, 96, TyrePixel)); return s_tyreSprite; }
		private static Sprite RimSprite() { if (s_rimSprite == null) s_rimSprite = SpriteOf(InfernoHazards.MakeTexture(96, 96, RimPixel)); return s_rimSprite; }
		private static Sprite BallSprite() { if (s_ballSprite == null) s_ballSprite = SpriteOf(InfernoHazards.MakeTexture(96, 96, BallPixel)); return s_ballSprite; }
		private static Sprite ShineSprite() { if (s_shineSprite == null) s_shineSprite = SpriteOf(InfernoHazards.MakeTexture(96, 96, ShinePixel)); return s_shineSprite; }

		// ---- the STYLE sheet on the garage page ------------------------------------------------------
		// /* [user request 2026-09-24: style sheet] */ A bottom sheet: 920 x 840 with its bottom edge SheetBottom
		// (196 px) above the page bottom, 12 px over the dock, so the upper 884 px of the page stay free; OpenSheet
		// lifts the garage's full-page 3D preview image by PreviewLift so the car and its platform sit in that band
		// and every pick shows on the car at once. Four tabs (glyph over label) over one grid of 2 x 4 chips, a pager
		// and a hint / notice line. Coordinates are relative to the sheet's centre (x right, y up); the frame's
		// contour runs ~15 px in, so content stays within +-445 x +-405.
		// original: 920 x 920 centred at y 20 over the car (its layout is in the git history of this block).
		//
		//   Title      "STYLE" 40 px, box 620x48 at (0, 366):     x -310..310,  y 342..390   (centred)
		//   Subtitle   car name 20 px, box 660x26 at (0, 328):     x -330..330,  y 315..341   (centred)
		//   Close      80x80 at (400, 355):                          x 360..440,   y 315..395
		//   Rule       860x2 at (0, 302)
		//   Tabs       200x84 at x -330 / -110 / 110 / 330, y 246:   y 204..288 (glyph 28 px at +14, label at -14)
		//   Row 1      170x170 chips at x -300 / -100 / 100 / 300, y 97:    y 12..182
		//   Caption 1  30 high at chip y - 116 = -19:                        y -34..-4
		//   Row 2      y -141:                                                y -226..-56
		//   Caption 2  y -257:                                                y -272..-242
		//   Pager      Prev 64x64 at (-120, -322), page label 120x30 at (0, -322), Next at (120, -322): y -354..-290
		//   Hint       800x26 at (0, -378); the Notice takes the same slot while it shows: y -391..-365
		private const int PerPage = 8, Columns = 4;   // original: Visible = 4 chips per row, one row per category
		private const float SheetWidth = 920f, SheetHeight = 840f, SheetBottom = 196f;   // original: SheetHeight = 920f (790f in the port), centred at y 20
		private const float PreviewLift = 540f;                                          // how far the garage preview image rises while the sheet is open
		/* [user request 2026-09-27: centre the heading] "STYLE" and the car name sit on the sheet's centre line
		   instead of starting at its left edge. Both boxes are widened so the centred text has room and are kept
		   clear of the 80 px close button at x 360..440: title -310..310, subtitle -330..330.
		   original: TitleX = -320f, TitleW = 240f; SubtitleX = -230f, SubtitleW = 420f (both MiddleLeft) */
		private const float TitleX = 0f, TitleY = 366f, TitleW = 620f, TitleH = 48f;                 // original: (-320, 420) 240x40, 32 px
		private const float SubtitleX = 0f, SubtitleY = 328f, SubtitleW = 660f, SubtitleH = 26f;     // original: (-230, 381)
		private const float CloseX = 400f, CloseY = 355f, CloseSize = 80f;                            // original: (396, 396) 88x88
		private const float RuleY = 302f;
		private const float TabY = 246f, TabW = 200f, TabH = 84f;                                     // original: y 296, 70 tall, no glyph
		private static readonly float[] TabX = { -330f, -110f, 110f, 330f };
		private const float TabIconY = 14f, TabIconSize = 28f, TabLabelY = -14f;
		private const float ChipSize = 170f;                                                          // original: 150f
		private static readonly float[] ChipX = { -300f, -100f, 100f, 300f };                        // original: +-0.5 / +-1.5 x 175
		private static readonly float[] RowY = { 97f, -141f };                                       // original: 156 / -76
		private const float CaptionDrop = 116f, CaptionH = 30f;                                       // original: 112 (ChipSize / 2 + 20 in the port)
		private const float PagerY = -322f, PagerX = 120f, PagerButton = 64f;                         // original: y -262, 72 px
		private const float NoticeY = -378f, NoticeW = 800f, NoticeH = 26f;                           // original: (0, -340) 800x40
		private const float LockBadgeX = 59f, LockBadgeY = 58f, LockBadgeSize = 24f;
		private const float StyleButtonX = 385f, StyleButtonWidth = 170f;   /* [user request 2026-09-27: action row] */ // the row's level comes from GarageActionRowBottom / GarageActionRowHeight                  // original: 44 px over the swatch at (0, SwatchY)
		private const float SwatchY = 14f;                                                            // swatch centre inside a chip
		private const float NameY = -52f, NameW = 130f, NameH = 22f;                                  // the chip's bottom band (y -63..-41)
		private static readonly float[] StockTyre = { 0.10f, 0.09f, 0.13f }, StockRim = { 0.35f, 0.33f, 0.44f };

		private sealed class Chip
		{
			public Image frame;     // the neon frame (always full tint, like every lobby button)
			public Image plate;     // electric plate inside it = selected (the PLAY button treatment)
			public Image shade;     // dark plate + lock badge = not owned yet (the garage's locked-card treatment)
			public Image lockBadge;
			public Image[] swatch = new Image[4]; public Text empty; public Text name; public Text price; public Image gem; public int index = -1;
			public float x, y;      // chip centre in the grid
		}

		private static RectTransform s_sheet;
		private static RectTransform s_openRoot;       // holds the StyleButton and its shadow (hidden while the sheet is open)
		private static Button s_openButton;
		private static readonly Chip[][] s_chips = NewChips();
		private static readonly int[] s_page = new int[CategoryCount];
		private static readonly RectTransform[] s_grids = new RectTransform[CategoryCount];
		private static readonly Button[] s_tabs = new Button[CategoryCount];
		private static int s_tab;
		private static Button s_prev, s_next;
		private static CanvasGroup s_prevGroup, s_nextGroup;
		private static Text s_pageLabel, s_subtitle;
		private static Text s_notice;
		private static Text s_hint;   /* [user request 2026-09-24: style sheet] */
		private static float s_noticeUntil;

		private static Chip[][] NewChips()
		{
			Chip[][] chips = new Chip[CategoryCount][];
			for (int c = 0; c < CategoryCount; c++)
			{
				chips[c] = new Chip[PerPage];
				for (int s = 0; s < PerPage; s++) chips[c][s] = new Chip();
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

		// The chamfered frame (InfernoLobby.NeonFrame draws the kit's cut-corner sprite).
		private static Image CFrame(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height)
		{
			Image image = InfernoLobby.NeonFrame(game, parent, name, x, y, width, height);
			Centre(image.rectTransform, x, y, width, height);
			return image;
		}

		// The frame's inner plate (cut like the frame), tinted: electric for a selected
		// chip, translucent black as the shade over a locked one. Chips are buttons, so their
		// frame and plates take the kit's Medium (28 / 10 px) cut instead of the Large one
		// ChamferFor picks for anything over 160 px (plan 0: "buttons 28/10").
		private static Image CPlate(ShadowRunPrototype game, Transform parent, string name, float size, Color tint)
		{
			Image image = InfernoLobby.NeonPlate(game, parent, name, 0f, 0f, size - 2f * InfernoLobby.PlateInset, size - 2f * InfernoLobby.PlateInset);
			Centre(image.rectTransform, 0f, 0f, size - 2f * InfernoLobby.PlateInset, size - 2f * InfernoLobby.PlateInset);
			Sprite plate = InfernoUi.ChamferPlateSprite(InfernoUi.Chamfer.Medium);
			if (plate != null) { image.sprite = plate; image.type = Image.Type.Sliced; }
			image.color = tint;
			return image;
		}

		// A sprite swatch: a block showing `sprite` (aspect kept), tinted later by RefreshChip.
		private static Image Swatch(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height, Sprite sprite)
		{
			Image image = CBlock(game, parent, name, x, y, width, height, 0xFFFFFF);
			image.sprite = sprite;
			image.type = Image.Type.Simple;
			image.preserveAspect = true;
			return image;
		}

		private static Image Circle(ShadowRunPrototype game, Transform parent, string name, float x, float y, float size, uint color)
		{
			if (s_circle == null) s_circle = game.MakeCircleSprite(128);
			Image image = CBlock(game, parent, name, x, y, size, size, color);
			if (s_circle != null) { image.sprite = s_circle; image.type = Image.Type.Simple; image.preserveAspect = true; }
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
			if (s_hint != null) Show(s_hint, false);   /* [user request 2026-09-24: style sheet] */ // the hint yields its line to the notice
			s_noticeUntil = Time.unscaledTime + 2.2f;
		}

		private static void RefreshGems(ShadowRunPrototype game, int balance)
		{
			game.menuGemCount = balance;
			if (game.gemAmountText != null)
			{
				InfernoLobby.Caption(game.gemAmountText, InfernoLobby.GemText(balance));   /* [user request 2026-09-26: crystals] */
			}
		}

		private static void Apply(ShadowRunPrototype game)
		{
			// the garage preview and the player car are rebuilt so the new dress shows at once
			// (InfernoCars.Decorate applies the paint while the models are built)
			game.RebuildGaragePreviewVehicle();
			game.RebuildPlayerVehicleModel();
		}

		private static Color Rgb(float[] c, float alpha = 1f) { return new Color(c[0], c[1], c[2], alpha); }
		private static void Tint(Image image, Color color) { image.color = color; }
		private static void Show(Component component, bool on) { component.gameObject.SetActive(on); }

		/* [user request 2026-09-24: style sheet] */ // the tab glyphs are the chips' own swatch art: a rim, the checkered flag, a streak, a paint sphere
		private static Sprite TabIcon(int category)
		{
			switch (category)
			{
				case Tires: return RimSprite();
				case Flags: return FlagSprite(1);
				case Trails: return StreakSprite();
				default: return BallSprite();
			}
		}

		// The STOCK paint swatch shows the active car's own colour.
		private static Color StockPaint()
		{
			string id = s_paintCar;
			if (!string.IsNullOrEmpty(id) && InfernoCars.Contains(id)) return InfernoCars.DefaultPaint(id);
			return new Color(0.42f, 0.40f, 0.48f, 1f);
		}

		private static void RefreshChip(ShadowRunPrototype game, int category, int slot)
		{
			Chip chip = s_chips[category][slot];
			if (chip.frame == null) return;
			int index = s_page[category] * PerPage + slot;
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
				Tint(chip.swatch[0], Rgb(none ? StockTyre : item.b));
				Tint(chip.swatch[1], Rgb(none ? StockRim : item.a));
				bool neon = NeonRim(index);
				/* [user request 2026-09-24: style sheet] */ // every finish gets a halo in its rim colour, the neon ones brighter; original: neon only
				Show(chip.swatch[3], !none);
				if (!none) Tint(chip.swatch[3], Rgb(item.a, neon ? 0.55f : 0.26f));
			}
			else if (category == Flags)
			{
				Sprite sprite = none ? null : FlagSprite(index);
				Show(chip.swatch[0], sprite != null);
				Show(chip.swatch[1], sprite != null);
				Show(chip.empty, sprite == null);
				if (sprite != null) { chip.swatch[0].sprite = sprite; InfernoLobby.Paint(chip.swatch[0], 0xFFFFFF); }
			}
			else if (category == Trails)
			{
				Show(chip.swatch[0], !none);
				Show(chip.swatch[1], !none);
				Show(chip.empty, none);
				Show(chip.swatch[2], !none);   /* [user request 2026-09-24: style sheet] */ // colour halo behind the streak
				if (!none)
				{
					Tint(chip.swatch[0], Rgb(item.a));
					Tint(chip.swatch[1], HotCore(item.a, 1f));
					Tint(chip.swatch[2], Rgb(item.a, 0.42f));
				}
			}
			else
			{
				Color body = none ? StockPaint() : Rgb(item.a);
				Tint(chip.swatch[0], body);
				Tint(chip.swatch[1], new Color(1f, 1f, 1f, 0.55f + 0.45f * (none ? 0f : item.b[0])));   // metallic paints shine harder
				Tint(chip.swatch[2], new Color(body.r, body.g, body.b, 0.4f));   /* [user request 2026-09-24: style sheet] */ // colour halo behind the sphere
			}
			// caption under the chip: state in the lobby's lilac, a price next to the crystal
			bool gem = false;
			if (selected) { InfernoLobby.Caption(chip.price, "SELECTED"); chip.price.color = InfernoLobby.Hex(InfernoLobby.Blossom); }
			else if (owned) { InfernoLobby.Caption(chip.price, "OWNED"); chip.price.color = InfernoLobby.Hex(InfernoLobby.Lilac); }
			else
			{
				InfernoLobby.Caption(chip.price, item.price.ToString());
				chip.price.color = InfernoLobby.Hex(InfernoLobby.Pearl);
				Show(chip.gem, true);
				gem = true;
			}
			// the price box is centred under the chip, or shifted right of the crystal
			Centre(chip.price.rectTransform, chip.x + (gem ? 12f : 0f), chip.y - CaptionDrop, gem ? 120f : ChipSize, CaptionH);
		}

		private static int Pages(int category) { return (Count(category) + PerPage - 1) / PerPage; }

		private static void RefreshPager(ShadowRunPrototype game)
		{
			int pages = Pages(s_tab);
			bool paged = pages > 1;
			if (s_pageLabel != null) InfernoLobby.Caption(s_pageLabel, (s_page[s_tab] + 1) + " / " + pages);
			// dimmed, still clickable (a no-op) when the tab fits on one page
			if (s_prevGroup != null) s_prevGroup.alpha = paged ? 1f : 0.38f;
			if (s_nextGroup != null) s_nextGroup.alpha = paged ? 1f : 0.38f;
		}

		private static void Refresh(ShadowRunPrototype game)
		{
			for (int c = 0; c < CategoryCount; ++c)
				for (int s = 0; s < PerPage; ++s) RefreshChip(game, c, s);
			RefreshPager(game);
		}

		private static void SelectTab(ShadowRunPrototype game, int tab)
		{
			s_tab = Mathf.Max(0, Mathf.Min(CategoryCount - 1, tab));
			for (int c = 0; c < CategoryCount; ++c)
			{
				if (s_grids[c] != null) Show(s_grids[c], c == s_tab);
				if (s_tabs[c] != null) InfernoUi.SetTabActive(s_tabs[c], c == s_tab);
			}
			Refresh(game);
		}

		private static void Pick(ShadowRunPrototype game, int category, int slot)
		{
			if (!IsOpen(game)) return;
			int index = s_page[category] * PerPage + slot;
			if (index >= Count(category)) return;
			if (category == Paint) SyncCar(game);
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
			int pages = Pages(category);
			if (pages <= 1) return;   // one page: Prev / Next are dimmed no-ops
			s_page[category] = (s_page[category] + direction + pages) % pages;
			Refresh(game);
		}

		/* [user request 2026-09-24: style sheet] */
		// The sheet stands SheetBottom above the page bottom (bottom-centre anchor and pivot) and is scaled down
		// about that edge when the page is too short for its 840 px plus a 380 px band for the lifted car.
		// original: centred at y 20, scaled to the page height less 460 px of header and dock reserves
		private static void FitSheet(ShadowRunPrototype game)
		{
			if (s_sheet == null || game.garageRoot == null) return;
			PlaceSheet(s_sheet, 1f);
			RectTransform pageRect = game.garageRoot.transform.GetComponent<RectTransform>();
			if (pageRect == null) return;
			Rect bounds = pageRect.rect;
			float roomY = bounds.height - SheetBottom - 380f;
			float scale = Mathf.Max(0.5f, Mathf.Min(1.0f, Mathf.Min((bounds.width - 40f) / SheetWidth, roomY / SheetHeight)));
			PlaceSheet(s_sheet, scale);
		}

		private static void PlaceSheet(RectTransform sheet, float scale)
		{
			sheet.anchorMin = sheet.anchorMax = new Vector2(0.5f, 0f);
			sheet.pivot = new Vector2(0.5f, 0f);
			sheet.sizeDelta = new Vector2(SheetWidth, SheetHeight);
			sheet.anchoredPosition = new Vector2(0f, SheetBottom);
			sheet.localScale = new Vector3(scale, scale, 1f);
			sheet.localRotation = Quaternion.identity;
		}

		// The garage's 3D preview is a full-page RawImage (MakeGarageCar) whose render texture holds the car alone;
		// the platform it stands on is the page's "Garage Background Image" (garage12). While the sheet is open
		// both rise by PreviewLift (scaled with the sheet): the car through the RawImage's uvRect, the background
		// through its rect offsets, so the car keeps standing on its platform in the band above the sheet, where
		// every pick is visible at once. The RawImage rect itself stays full-page (the layout audit keeps treating
		// it as the page's backdrop, and the spin-drag still covers the screen); the strip the scroll uncovers at
		// the bottom is the render texture's clamped floor row over the page's black root, behind the sheet.
		private static void LiftPreview(ShadowRunPrototype game, bool lifted)
		{
			if (game == null || game.garageCarPreview == null) return;
			RawImage image = game.garageCarPreview;
			float height = image.rectTransform.rect.height;
			if (height < 1f) height = 1920f;
			float lift = lifted ? PreviewLift * (s_sheet != null ? s_sheet.localScale.y : 1f) : 0f;
			image.uvRect = new Rect(0f, -lift / height, 1f, 1f);
			Transform background = game.garageRoot != null ? game.garageRoot.transform.Find("Garage Background Image") : null;
			RectTransform rect = background as RectTransform;
			if (rect != null)
			{
				rect.offsetMin = new Vector2(rect.offsetMin.x, lift);
				rect.offsetMax = new Vector2(rect.offsetMax.x, lift);
			}
		}

		private static void OpenSheet(ShadowRunPrototype game)
		{
			if (s_sheet == null) return;
			SyncCar(game);
			if (s_subtitle != null) InfernoLobby.Caption(s_subtitle, CarLabel(game));
			FitSheet(game);
			s_sheet.gameObject.SetActive(true);
			if (s_openRoot != null) Show(s_openRoot, false);   // nothing peeks out from under the sheet
			/* [user request 2026-09-21: no overlaps] the sheet is modal: the card carousel it used to cover
			   half-way (its top row showed above the sheet) and the page header are hidden while it is open */
			ShowGaragePage(game, false);
			LiftPreview(game, true);   /* [user request 2026-09-24: style sheet] */
			SelectTab(game, s_tab);
		}

		public static void CloseSheet(ShadowRunPrototype game)
		{
			if (s_sheet != null) s_sheet.gameObject.SetActive(false);
			if (s_openRoot != null) Show(s_openRoot, true);
			ShowGaragePage(game, true);
			LiftPreview(game, false);   /* [user request 2026-09-24: style sheet] */
		}

		private static void ShowGaragePage(ShadowRunPrototype game, bool shown)
		{
			if (game == null || game.garageRoot == null) return;
			Transform selector = game.garageRoot.transform.Find("Garage Vehicle Card Selector");
			if (selector != null) selector.gameObject.SetActive(shown);
			/* [user request 2026-09-24: style sheet] the MY GARAGE header stays over the lifted car (the bottom sheet is nowhere
			   near it); original: hidden with the carousel */
			if (game.garagePageHeaderRoot != null) game.garagePageHeaderRoot.gameObject.SetActive(true);
		}

		private static void OpenClick(ShadowRunPrototype game) { OpenSheet(game); }
		private static void CloseClick(ShadowRunPrototype game) { CloseSheet(game); }

		// Closed instance delegates (C++: MethodInfo trampolines, InfernoSettings::Hook); C#: a lambda over the game.
		private static void Hook(ShadowRunPrototype game, Button button, System.Action<ShadowRunPrototype> fn)
		{
			button.onClick.AddListener(() => fn(game));
		}

		// One chip of the grid: the kit's chamfered frame as the button, the swatch art in its
		// upper part, the item name in its bottom band and the caption (price / OWNED /
		// SELECTED) under it. The 170 px frame's contour runs ~12 px in with cut corners, so
		// the art stays inside |x|, |y| <= 62 and the name band sits at y -63..-41.
		private static void BuildChip(ShadowRunPrototype game, int C, int S, Transform grid, float x, float y)
		{
			Chip chip = s_chips[C][S];
			chip.x = x; chip.y = y;
			chip.frame = CFrame(game, grid, "Chip", x, y, ChipSize, ChipSize);
			Sprite medium = InfernoUi.ChamferFrameSprite(InfernoUi.Chamfer.Medium);   // a button's cut (see CPlate)
			if (medium != null) { chip.frame.sprite = medium; chip.frame.type = Image.Type.Sliced; }
			Button button = InfernoLobby.ButtonOn(chip.frame);
			Hook(game, button, g => Pick(g, C, S));
			Transform inside = chip.frame.transform;
			chip.plate = CPlate(game, inside, "Selected", ChipSize, InfernoLobby.Hex(0xFFFFFF));   // drawn first: under the swatch
			if (C == Tires)
			{
				// three concentric pieces: a neon halo (neon items only), the tyre ring, the
				// five-spoke rim (its disc is 0.64 of the ring's texture: it fills the hole) and the hub
				chip.swatch[3] = Swatch(game, inside, "Halo", 0f, SwatchY, 118f, 118f, SoftSprite());        // original: none
				chip.swatch[0] = Swatch(game, inside, "Tyre", 0f, SwatchY, 92f, 92f, TyreSprite());          // original: Circle 80 px
				chip.swatch[1] = Swatch(game, inside, "Rim", 0f, SwatchY, 92f, 92f, RimSprite());            // original: Circle 50 px
				chip.swatch[2] = Circle(game, inside, "Hub", 0f, SwatchY, 14f, InfernoLobby.Ink);           // original: 16 px
			}
			else if (C == Flags)
			{
				chip.swatch[1] = CBlock(game, inside, "Pole", -40f, 12f, 5f, 66f, 0xD8D0E8);     // original: -44f
				chip.swatch[0] = CBlock(game, inside, "Banner", 4f, 16f, 80f, 50f, 0xFFFFFF);   // original: 2f, 21f, 84f, 48f (the 256x160 art is 1.6:1)
				chip.empty = CLabel(game, inside, "Empty", "--", 0f, SwatchY, 100f, 60f, 34, InfernoLobby.Lavender);
			}
			else if (C == Trails)
			{
				chip.swatch[2] = Swatch(game, inside, "Halo", 0f, 16f, 132f, 66f, SoftSprite());       /* [user request 2026-09-24: style sheet] */ // colour halo, drawn first: under the streak
				chip.swatch[0] = Swatch(game, inside, "Glow", 0f, 16f, 112f, 28f, StreakSprite());   // original: flat block 100x24
				chip.swatch[1] = Swatch(game, inside, "Core", 0f, 16f, 112f, 10f, StreakSprite());   // original: flat block 100x8
				chip.empty = CLabel(game, inside, "Empty", "--", 0f, SwatchY, 100f, 60f, 34, InfernoLobby.Lavender);
			}
			else
			{
				// a glossy sphere in the colour with its highlight drawn over it, on a halo of the same colour
				chip.swatch[2] = Swatch(game, inside, "Halo", 0f, 16f, 122f, 122f, SoftSprite());     /* [user request 2026-09-24: style sheet] */
				chip.swatch[0] = Swatch(game, inside, "Ball", 0f, 16f, 84f, 84f, BallSprite());
				chip.swatch[1] = Swatch(game, inside, "Shine", 0f, 16f, 84f, 84f, ShineSprite());
			}
			// Name in the bottom band; long names (NEON VIOLET, ELECTRIC BLUE) shrink to 12 px.
			chip.name = CLabel(game, inside, "Name", "", 0f, NameY, NameW, NameH, 16, InfernoLobby.FrameText);   // original: (0, -40) 100x26, 18 px
			chip.name.resizeTextMinSize = 12;   // original: 13
			// locked: the garage's own dark shade + lock badge over the content
			chip.shade = CPlate(game, inside, "Locked", ChipSize, InfernoLobby.Hex(InfernoLobby.Black, 0.5f));   /* [user request 2026-09-24: style sheet] */ // original: 0.58
			if (game.garageLockSprite != null)
			{
				chip.lockBadge = CBlock(game, inside, "Lock", LockBadgeX, LockBadgeY, LockBadgeSize, LockBadgeSize, 0xFFFFFF);   /* [user request 2026-09-24: style sheet] */ // corner badge: the swatch stays visible
				chip.lockBadge.sprite = game.garageLockSprite;
				chip.lockBadge.preserveAspect = true;
			}
			chip.price = CLabel(game, grid, "Price", "", x + 12f, y - CaptionDrop, 120f, CaptionH, 22, InfernoLobby.Lilac);
			chip.gem = CBlock(game, grid, "PriceGem", x - 34f, y - CaptionDrop, 26f, 26f, 0xFFFFFF);
			chip.gem.sprite = InfernoLobby.GemSprite(game);
			chip.gem.preserveAspect = true;
		}

		// Built once at the end of MakeGarageScreen: the STYLE button in the garage's action
		// row (bottom-anchored like the purchase button), and the hidden sheet above the dock.
		public static void Build(ShadowRunPrototype game)
		{
			if (game.garageRoot == null || s_openRoot != null) return;
			Transform page = game.garageRoot.transform;
			SyncCar(game);
			// Action row at y 580 from the page bottom (530..630): the purchase button is 560
			// wide centred (-280..280); this 170 px button sits at x 385 (300..470), 20 px to
			// its right and 70 px from the page edge (540). Its root hides both the body and
			// the kit's shadow sibling while the sheet is open.
			/* [user request 2026-09-27: action row] */ // Both buttons of the row take their level from the same two constants
			// (MakeGaragePurchaseAction uses them for the PURCHASE plate), so the pair cannot drift apart:
			// centre y = GarageActionRowBottom + GarageActionRowHeight / 2 = 580, height = GarageActionRowHeight.
			// original: Place(s_openRoot, 385f, 580f, 170f, 100f, ...) with the level written out as literals
			s_openRoot = InfernoLobby.Root(page, "StyleButtonRoot");
			InfernoLobby.Place(s_openRoot, StyleButtonX, GarageActionRowBottom + GarageActionRowHeight * 0.5f, StyleButtonWidth, GarageActionRowHeight, new Vector2(0.5f, 0f));   // original: FrameButton "StyleButton" 150x121 at (375, 445)
			s_openButton = InfernoUi.MakeButton3D(game, s_openRoot, "StyleButton", "STYLE", 0f, 0f, StyleButtonWidth, GarageActionRowHeight, InfernoUi.Style.Frame, 28);
			/* [user request 2026-09-27: action row] */ // a capsule, so the small STYLE key is a different SHAPE from the cut-corner PURCHASE
			// plate next to it instead of a smaller copy of it (same rect, same level, same press feedback)
			InfernoUi.MakePill(game, s_openButton);
			Hook(game, s_openButton, OpenClick);
			// 920x920 in the middle of the page (FitSheet positions and scales it on open): a
			// root holding an opaque black plate (sized to the frame's inside like CPlate) so
			// the garage cards never bleed through, with the frame drawn over it.
			/* [user request 2026-09-20: pause/UI fixes] */
			s_sheet = InfernoLobby.Root(page, "StyleSheet");
			PlaceSheet(s_sheet, 1f);   /* [user request 2026-09-24: style sheet] */
			Image backer = InfernoLobby.NeonPlate(game, s_sheet, "Backer", 0f, 0f, SheetWidth - 2f * InfernoLobby.PlateInset, SheetHeight - 2f * InfernoLobby.PlateInset);
			Centre(backer.rectTransform, 0f, 0f, SheetWidth - 2f * InfernoLobby.PlateInset, SheetHeight - 2f * InfernoLobby.PlateInset);
			backer.color = InfernoLobby.Hex(InfernoLobby.Night);   /* [user request 2026-09-24: style sheet] */ // original: Black
			Image sheet = CFrame(game, s_sheet, "Frame", 0f, 0f, SheetWidth, SheetHeight);
			sheet.raycastTarget = true;   // swallows taps meant for the carousel underneath
			Transform inside = sheet.transform;
			// heading: the mockups' bold-italic title with a letter-spaced subtitle (the active car)
			Text title = CLabel(game, inside, "Title", "STYLE", TitleX, TitleY, TitleW, TitleH, 40, 0xFFFFFF);   /* [user request 2026-09-24: style sheet] */ // original: 32 px Pearl
			title.alignment = TextAnchor.MiddleCenter;   /* [user request 2026-09-27: centre the heading] */ // original: MiddleLeft
			title.fontStyle = FontStyle.BoldAndItalic;
			title.resizeTextMinSize = 28;
			s_subtitle = CLabel(game, inside, "Subtitle", CarLabel(game), SubtitleX, SubtitleY, SubtitleW, SubtitleH, 20, InfernoLobby.Lilac);
			s_subtitle.alignment = TextAnchor.MiddleCenter;   /* [user request 2026-09-27: centre the heading] */ // original: MiddleLeft
			s_subtitle.resizeTextMinSize = 14;
			Button close = InfernoUi.MakeButton3D(game, inside, "Close", "X", CloseX, CloseY, CloseSize, CloseSize, InfernoUi.Style.Frame, 40);   // original: FrameButton 88x88 at (396, 396)
			Hook(game, close, CloseClick);
			CBlock(game, inside, "HeaderRule", 0f, RuleY, SheetWidth - 60f, 2f, 0x2A2632);   /* [user request 2026-09-24: style sheet] */ // thin rule between the heading and the tabs
			// the rhomboid tab row
			for (int c = 0; c < CategoryCount; ++c)
			{
				int C = c;
				s_tabs[c] = InfernoUi.MakeTab3D(game, inside, "Tab " + CategoryKeys[c], CategoryNames[c], null, TabX[c], TabY, TabW, TabH);
				Hook(game, s_tabs[c], g => SelectTab(g, C));
				/* [user request 2026-09-24: style sheet] */ // a swatch glyph over the label; SetTabActive paints the "Icon" child steel / neon like
				// the dock's icons. original: no icon, the label centred at y 2 in a 70 px tab
				Text label = InfernoUi.ButtonLabel(s_tabs[c]);
				if (label != null) Centre(label.rectTransform, 0f, TabLabelY, TabW - 60f, 26f);
				Image glyph = Swatch(game, s_tabs[c].transform, "Icon", 0f, TabIconY, c == Trails ? 48f : TabIconSize, TabIconSize, TabIcon(c));   // the streak is wide
				glyph.color = InfernoLobby.Hex(0x9A94A8);
			}
			// one grid per tab (only the open tab's is active, so its chips are the ones named "Chip" in play)
			for (int c = 0; c < CategoryCount; ++c)
			{
				RectTransform grid = InfernoLobby.Root(inside, "Grid " + CategoryKeys[c]);
				Centre(grid, 0f, 0f, SheetWidth, SheetHeight);
				s_grids[c] = grid;
				for (int s = 0; s < PerPage; ++s) BuildChip(game, c, s, grid, ChipX[s % Columns], RowY[s / Columns]);
			}
			// the pager under the grid: always present, dimmed when the tab fits on one page
			s_prev = InfernoUi.MakeButton3D(game, inside, "Prev", "<", -PagerX, PagerY, PagerButton, PagerButton, InfernoUi.Style.Frame, 34);   /* [user request 2026-09-24: style sheet] */ // original: 72 px, 40 px glyph
			s_next = InfernoUi.MakeButton3D(game, inside, "Next", ">", PagerX, PagerY, PagerButton, PagerButton, InfernoUi.Style.Frame, 34);
			Hook(game, s_prev, g => Page(g, s_tab, -1));
			Hook(game, s_next, g => Page(g, s_tab, 1));
			s_prevGroup = s_prev.gameObject.AddComponent<CanvasGroup>();
			s_nextGroup = s_next.gameObject.AddComponent<CanvasGroup>();
			s_pageLabel = CLabel(game, inside, "PageLabel", "", 0f, PagerY, 120f, CaptionH, 22, InfernoLobby.Lilac);
			// hint / notice line under the pager: the hint says what a tap does, a notice replaces it for 2.2 s
			/* [user request 2026-09-24: style sheet] */ // original: the notice alone, 22 px
			s_hint = CLabel(game, inside, "Hint", "TAP AN ITEM TO EQUIP IT   -   LOCKED ITEMS COST CRYSTALS", 0f, NoticeY, NoticeW, NoticeH, 18, InfernoLobby.Lilac);
			s_hint.resizeTextMinSize = 12;
			s_notice = CLabel(game, inside, "Notice", "", 0f, NoticeY, NoticeW, NoticeH, 20, InfernoLobby.Blossom);
			Show(s_notice, false);
			SelectTab(game, 0);
			s_sheet.gameObject.SetActive(false);
		}

		// Every frame from ShadowRunPrototype.Update (unscaled): flags wave, notices expire,
		// sparks hold still while the run is paused (TickTrails resumes them).
		public static void Frame(ShadowRunPrototype game)
		{
			Wave(game);
			if (s_notice != null && s_noticeUntil > 0f && Time.unscaledTime > s_noticeUntil)
			{
				s_noticeUntil = 0f;
				Show(s_notice, false);
				if (s_hint != null) Show(s_hint, true);   /* [user request 2026-09-24: style sheet] */
			}
			if (s_owner == game && s_trailStyle > 0 && game.paused) PauseSparks();
		}

		public static void Dispose(ShadowRunPrototype game)
		{
			if (s_owner == game) { DisposeTrails(); s_owner = null; }
			for (int i = 0; i < s_flags.Length; i++) ReleaseFlag(i);
			DisposeRimInstances();
			DisposePreviewMaterials();
			s_sheet = null; s_openRoot = null; s_openButton = null; s_notice = null; s_hint = null; s_circle = null;
			s_prev = s_next = null; s_prevGroup = s_nextGroup = null; s_pageLabel = s_subtitle = null;
			for (int c = 0; c < CategoryCount; c++)
			{
				s_grids[c] = null; s_tabs[c] = null; s_page[c] = 0;
				for (int s = 0; s < PerPage; s++) s_chips[c][s] = new Chip();
			}
			s_tab = 0;
		}
	}
}
