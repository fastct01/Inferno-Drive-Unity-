using System;
using System.Reflection;
using NUnit.Framework;

// Geometry rules of the kit's widgets (Inferno/InfernoUi.cs). Today: the five tick dots of
// MakeSlider3D's track, which must lie completely on the bar.
/* [user request 2026-09-27: dots inside the bar] */
public class InfernoUiTests
{
	private static readonly BindingFlags Any = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

	// InfernoUi is an internal nested class of the game: reached through reflection like the other suites.
	private static readonly Type UiType = typeof(ShadowRunPrototype).GetNestedType("InfernoUi", Any);
	private static float Const(string name) { return (float)UiType.GetField(name, Any).GetValue(null); }
	private static float PipX(int index, float trackWidth)
	{
		return (float)UiType.GetMethod("PipX", Any).Invoke(null, new object[] { index, trackWidth });
	}

	// The settings page builds its three volume sliders at this width (InfernoSettings.TrackWidth).
	private const float SettingsTrackWidth = 518f;
	private static readonly float[] Widths = { SettingsTrackWidth, 120f, 240f, 360f, 800f, 1024f };

	[Test]
	public void EveryDotLiesCompletelyOnTheBar()
	{
		float size = Const("PipSize"), half = size * 0.5f;
		foreach (float w in Widths)
		{
			for (int i = 0; i < 5; i++)
			{
				float x = PipX(i, w);
				Assert.GreaterOrEqual(x - half, 0f, "dot " + i + " overhangs the left end of a " + w + " px track");
				Assert.LessOrEqual(x + half, w, "dot " + i + " overhangs the right end of a " + w + " px track");
			}
		}
	}

	// The dots must clear the track's rounded caps too, not merely its rectangle: at the nominal inset a
	// dot sits where the 16 px capsule is at full height. This is what the old placement got wrong - the
	// end dots were centred on the caps, where there is no bar behind them at all.
	[Test]
	public void EndDotsClearTheRoundedCaps()
	{
		float size = Const("PipSize"), inset = Const("PipInset");
		Assert.GreaterOrEqual(inset, size * 0.5f, "the inset must at least cover the dot's own radius");
		foreach (float w in Widths)
		{
			Assert.GreaterOrEqual(PipX(0, w), size * 0.5f, "first dot sits on the left cap of a " + w + " px track");
			Assert.LessOrEqual(PipX(4, w), w - size * 0.5f, "last dot sits on the right cap of a " + w + " px track");
		}
	}

	[Test]
	public void DotsAreEvenlySpacedAndCentred()
	{
		foreach (float w in Widths)
		{
			Assert.AreEqual(w * 0.5f, PipX(2, w), 0.01f, "the middle dot should sit on the track's centre");
			float step = PipX(1, w) - PipX(0, w);
			for (int i = 1; i < 5; i++)
				Assert.AreEqual(step, PipX(i, w) - PipX(i - 1, w), 0.01f, "uneven gap before dot " + i);
			Assert.Greater(step, 0f, "dots must run left to right");
		}
	}

	[Test]
	public void IndexIsClampedAndNarrowTracksDoNotOverhang()
	{
		float half = Const("PipSize") * 0.5f;
		Assert.AreEqual(PipX(0, SettingsTrackWidth), PipX(-3, SettingsTrackWidth), 0.001f);
		Assert.AreEqual(PipX(4, SettingsTrackWidth), PipX(9, SettingsTrackWidth), 0.001f);
		foreach (float w in new[] { 12f, 20f, 30f })
			for (int i = 0; i < 5; i++)
			{
				Assert.GreaterOrEqual(PipX(i, w), half - 0.001f, "narrow track: dot " + i + " overhangs the left end");
				Assert.LessOrEqual(PipX(i, w), w - half + 0.001f, "narrow track: dot " + i + " overhangs the right end");
			}
	}
}
