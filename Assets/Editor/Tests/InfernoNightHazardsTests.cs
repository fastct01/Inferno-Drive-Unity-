using NUnit.Framework;
using Night = ShadowRunPrototype.InfernoNightHazards;

// [user request 2026-09-26: night drive] The pure rules of the NEON NIGHT complications: which lanes a roadblock
// closes (never all of them), where blocked traffic goes, the blackout's darkness curve and flicker, the box test
// against the car, and the spawn cadence constants.
public class InfernoNightHazardsTests
{
	[Test]
	public void ARoadblockAlwaysLeavesALaneOpen()
	{
		for (int closed = 1; closed <= 4; closed++)
			for (float roll = 0f; roll <= 1f; roll += 0.05f)
			{
				int start = Night.ClosedStart(5, closed, roll);
				Assert.GreaterOrEqual(start, 0);
				Assert.LessOrEqual(start + closed, 5, "span past the road");
				int open = 0;
				for (int lane = 0; lane < 5; lane++) if (!Night.LaneClosed(lane, start, closed)) open++;
				Assert.GreaterOrEqual(open, 1, "closed " + closed + " roll " + roll);
			}
		Assert.AreEqual(0, Night.ClosedStart(5, 3, 0f));
		Assert.AreEqual(2, Night.ClosedStart(5, 3, 1f));     // roll 1 is still a valid span (lanes 2..4)
		Assert.LessOrEqual(Night.ClosedStart(5, 5, 0.5f) + 4, 5, "closing every lane is clamped to four, which still fits");
		Assert.LessOrEqual(Night.ClosedStart(5, 9, 0.9f) + 4, 5);
	}

	[Test]
	public void BlockedTrafficMovesToTheNearestOpenLane()
	{
		// lanes 1..3 closed: 0 and 4 open
		Assert.AreEqual(0, Night.NearestOpenLane(1, 1, 3, 5));
		Assert.AreEqual(4, Night.NearestOpenLane(3, 1, 3, 5));
		int middle = Night.NearestOpenLane(2, 1, 3, 5);
		Assert.IsTrue(middle == 0 || middle == 4);
		Assert.AreEqual(0, Night.NearestOpenLane(0, 1, 3, 5), "an open lane is kept");
		// lanes 0..1 closed: 2 is nearest for both
		Assert.AreEqual(2, Night.NearestOpenLane(0, 0, 2, 5));
		Assert.AreEqual(2, Night.NearestOpenLane(1, 0, 2, 5));
		// lanes 3..4 closed
		Assert.AreEqual(2, Night.NearestOpenLane(4, 3, 2, 5));
	}

	[Test]
	public void BlackoutRampsInHoldsAndRampsOut()
	{
		float length = 200f;
		Assert.AreEqual(0f, Night.Darkness(-1f, length));
		Assert.AreEqual(0f, Night.Darkness(0f, length));
		Assert.AreEqual(0.5f, Night.Darkness(Night.BlackoutRamp * 0.5f, length), 0.0001f);
		Assert.AreEqual(1f, Night.Darkness(Night.BlackoutRamp, length), 0.0001f);
		Assert.AreEqual(1f, Night.Darkness(100f, length), 0.0001f);
		Assert.AreEqual(0.5f, Night.Darkness(length - Night.BlackoutRamp * 0.5f, length), 0.0001f);
		Assert.AreEqual(0f, Night.Darkness(length, length));
		Assert.AreEqual(0f, Night.Darkness(250f, length));
		// the flicker only stutters the ramps and never brightens past the darkness or below zero
		Assert.AreEqual(1f, Night.Flicker(1f, 3.3f));
		Assert.AreEqual(0f, Night.Flicker(0f, 3.3f));
		for (float t = 0f; t < 5f; t += 0.037f)
		{
			float f = Night.Flicker(0.5f, t);
			Assert.IsTrue(f >= 0f && f <= 0.5f, "flicker " + f + " at " + t);
		}
	}

	[Test]
	public void PiecesHitTheCarByBox()
	{
		// a barrier plank 2.7 m wide in lane x = 3.1 with the car at 5 (its z)
		Assert.IsTrue(Night.Hits(3.1f, 1.35f, 5f, 0.35f, 3.1f, 5f));
		Assert.IsTrue(Night.Hits(3.1f, 1.35f, 5f, 0.35f, 0.9f, 5f), "the car's edge clips the plank's end");
		Assert.IsFalse(Night.Hits(3.1f, 1.35f, 5f, 0.35f, 0.5f, 5f), "one lane over is clear");
		Assert.IsFalse(Night.Hits(3.1f, 1.35f, 5f + 3.0f, 0.35f, 3.1f, 5f), "3 m ahead is clear (car half length 2.4 + 0.35)");
		Assert.IsTrue(Night.Hits(3.1f, 1.35f, 5f + 2.6f, 0.35f, 3.1f, 5f));
	}

	[Test]
	public void CadenceConstants()
	{
		Assert.Less(Night.BlockFirstMin, Night.BlockFirstMax);
		Assert.Less(Night.BlockGapMin, Night.BlockGapMax);
		Assert.Less(Night.BlackoutFirstMin, Night.BlackoutFirstMax);
		Assert.Less(Night.BlackoutGapMin, Night.BlackoutGapMax);
		Assert.Less(Night.BlackoutLengthMin, Night.BlackoutLengthMax);
		Assert.Greater(Night.BlackoutLengthMin, 2f * Night.BlackoutRamp, "a zone is longer than its two ramps");
		Assert.Greater(Night.BlackoutFirstMin, Night.BlockFirstMin, "the first roadblock comes before the first blackout");
		Assert.AreEqual(ShadowRunPrototype.InfernoHazards.PlayerZ, Night.PlayerZ);
	}
}
