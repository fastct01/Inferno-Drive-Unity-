using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// [user request 2026-09-21: overlaps] Layout audit for the code-built uGUI: walks every active
// RectTransform under a root, compares the drawn shapes of siblings pairwise and reports the
// pairs that partially overlap (> 4 px²; one inside the other is a frame with its content and
// is fine), plus every Text whose glyphs leave the rect of its parent. Decorations (shadows,
// glows, rims, backers, shades, plates, surfaces, sheens, lines, dividers, rules, fills, tracks,
// pips, accents, masks, blurs) and full-stretch backgrounds are skipped. Texts are measured by
// their rendered glyphs (the cached TextGenerator), containers without a Graphic by the union of
// their visible descendants, rhomboid tabs by the parallelogram inside their rect.
// Driven by the "Inferno Drive > Audit Layout" menu (Assets/Editor/Build/InfernoLayoutAuditMenu.cs)
// and the LayoutAuditTests play-mode test.
public sealed partial class ShadowRunPrototype
{
	internal static partial class InfernoLayoutAudit
	{
		private static readonly string[] Decorations =
		{
			"Shadow", "Glow", "Rim", "Backer", "Shade", "Plate", "Surface", "Sheen", "Line", "Divider", "Rule",
			"Fill", "Track", "Pip", "Accent", "Mask", "Blur", "Rail", "Halo", "Contour", "Underline", "Corner",
			"Tile", "Band", "Core", "Seam", "Vignette", "Scanline", "Gradient", "Fade", "Dim", "Ambient",
			"Viewport", "Background", "Artwork", "Photo", "Segment", "Needle", "Dial", "Touch Zone",
			"Animation Layer",   // the case screen's 3D render layer sits behind CaseDisplay by design
		};

		private const float MinOverlap = 4f;      // px² in canvas (reference) units
		private const float Tolerance = 1f;       // px slack for "inside" and "leaves the parent"

		public static bool IsDecoration(string name)
		{
			if (string.IsNullOrEmpty(name)) return false;
			for (int i = 0; i < Decorations.Length; i++)
				if (name.IndexOf(Decorations[i], StringComparison.OrdinalIgnoreCase) >= 0) return true;
			return false;
		}

		public static List<string> Report(RectTransform root)
		{
			List<string> issues = new List<string>();
			if (root == null || !root.gameObject.activeInHierarchy) return issues;
			Canvas canvas = root.GetComponentInParent<Canvas>();
			RectTransform space = canvas != null ? canvas.rootCanvas.transform as RectTransform : root;
			Walk(root, space, issues);
			return issues;
		}

		// Every screen root the game built that is reachable from the canvases (inactive screens are
		// skipped by the walk, so the report covers what is on screen in the current state).
		public static string ReportAll(ShadowRunPrototype game)
		{
			List<string> issues = new List<string>();
			Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
			for (int i = 0; i < canvases.Length; i++)
			{
				if (canvases[i] == null || !canvases[i].isRootCanvas) continue;
				RectTransform rect = canvases[i].transform as RectTransform;
				if (rect != null) issues.AddRange(Report(rect));
			}
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < issues.Count; i++)
			{
				if (i > 0) sb.Append('\n');
				sb.Append(issues[i]);
			}
			return sb.ToString();
		}

		// ---- geometry ------------------------------------------------------------------------
		private struct Entry
		{
			public RectTransform rect;
			public Rect bounds;
			public bool decoration;
		}

		private static Rect Union(Rect a, Rect b)
		{
			float xMin = Math.Min(a.xMin, b.xMin), yMin = Math.Min(a.yMin, b.yMin);
			float xMax = Math.Max(a.xMax, b.xMax), yMax = Math.Max(a.yMax, b.yMax);
			return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
		}

		private static Rect ToSpace(RectTransform rect, RectTransform space, Vector3[] corners)
		{
			rect.GetWorldCorners(corners);
			float xMin = float.MaxValue, yMin = float.MaxValue, xMax = float.MinValue, yMax = float.MinValue;
			for (int i = 0; i < 4; i++)
			{
				Vector3 p = space != null ? space.InverseTransformPoint(corners[i]) : corners[i];
				xMin = Math.Min(xMin, p.x); yMin = Math.Min(yMin, p.y);
				xMax = Math.Max(xMax, p.x); yMax = Math.Max(yMax, p.y);
			}
			return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
		}

		// The rect of a RectTransform in canvas units; rhomboid tabs are measured by the
		// parallelogram inside their rect (the slanted sides overlap the neighbour's rect by design).
		private static Rect RectOf(RectTransform rect, RectTransform space, Vector3[] corners)
		{
			Rect r = ToSpace(rect, space, corners);
			Image image = rect.GetComponent<Image>();
			if (image != null)
			{
				float inset = InfernoUi.VisualInset(image.sprite);
				if (inset > 0f && r.width > 2f * inset) { r.xMin += inset; r.xMax -= inset; }
			}
			return r;
		}

		// A Text's glyph bounds (the cached generator holds the rendered quads in pixel units around
		// the pivot); falls back to the rect when nothing has been rendered yet.
		private static bool InkRect(Text text, RectTransform space, Vector3[] corners, out Rect ink)
		{
			ink = default(Rect);
			if (text == null || string.IsNullOrEmpty(text.text) || string.IsNullOrEmpty(text.text.Trim())) return false;
			TextGenerator generator = text.cachedTextGenerator;
			IList<UIVertex> verts = generator != null ? generator.verts : null;
			if (verts == null || verts.Count < 4)
			{
				ink = ToSpace(text.rectTransform, space, corners);
				return true;
			}
			float scale = text.pixelsPerUnit;
			if (scale <= 0f) scale = 1f;
			float xMin = float.MaxValue, yMin = float.MaxValue, xMax = float.MinValue, yMax = float.MinValue;
			for (int i = 0; i < verts.Count; i++)
			{
				Vector3 p = verts[i].position / scale;
				xMin = Math.Min(xMin, p.x); yMin = Math.Min(yMin, p.y);
				xMax = Math.Max(xMax, p.x); yMax = Math.Max(yMax, p.y);
			}
			if (xMax <= xMin || yMax <= yMin) { ink = ToSpace(text.rectTransform, space, corners); return true; }
			Vector3 a = text.rectTransform.TransformPoint(new Vector3(xMin, yMin, 0f));
			Vector3 b = text.rectTransform.TransformPoint(new Vector3(xMax, yMax, 0f));
			if (space != null) { a = space.InverseTransformPoint(a); b = space.InverseTransformPoint(b); }
			ink = new Rect(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Abs(b.x - a.x), Math.Abs(b.y - a.y));
			return true;
		}

		// What a child draws: its own Graphic (a Text by its glyphs), or the union of the visible
		// graphics below it when it is a plain container. Null (false) when nothing is visible.
		private static bool Bounds(RectTransform rect, RectTransform space, Vector3[] corners, out Rect bounds)
		{
			bounds = default(Rect);
			if (rect == null || !rect.gameObject.activeInHierarchy) return false;
			Graphic graphic = rect.GetComponent<Graphic>();
			if (graphic != null)
			{
				Text text = graphic as Text;
				if (text != null) return InkRect(text, space, corners, out bounds);
				if (graphic.color.a > 0.02f && graphic.enabled)
				{
					RawImage raw = graphic as RawImage;
					if (raw == null || raw.texture != null)
					{
						bounds = RectOf(rect, space, corners);
						return bounds.width > 0.5f && bounds.height > 0.5f;
					}
				}
			}
			bool any = false;
			for (int i = 0; i < rect.childCount; i++)
			{
				RectTransform child = rect.GetChild(i) as RectTransform;
				Rect childBounds;
				if (child == null || !Bounds(child, space, corners, out childBounds)) continue;
				bounds = any ? Union(bounds, childBounds) : childBounds;
				any = true;
			}
			return any;
		}

		private static bool Contains(Rect outer, Rect inner)
		{
			return inner.xMin >= outer.xMin - Tolerance && inner.xMax <= outer.xMax + Tolerance
				&& inner.yMin >= outer.yMin - Tolerance && inner.yMax <= outer.yMax + Tolerance;
		}

		private static float OverlapArea(Rect a, Rect b)
		{
			float w = Math.Min(a.xMax, b.xMax) - Math.Max(a.xMin, b.xMin);
			float h = Math.Min(a.yMax, b.yMax) - Math.Max(a.yMin, b.yMin);
			return w > 0f && h > 0f ? w * h : 0f;
		}

		private static string Path(Transform t)
		{
			StringBuilder sb = new StringBuilder(t.name);
			int depth = 0;
			for (Transform p = t.parent; p != null && depth < 4; p = p.parent, depth++)
			{
				sb.Insert(0, '/');
				sb.Insert(0, p.name);
			}
			return sb.ToString();
		}

		private static string Fmt(Rect r)
		{
			return "[" + r.xMin.ToString("0") + ".." + r.xMax.ToString("0") + " x " + r.yMin.ToString("0") + ".." + r.yMax.ToString("0") + "]";
		}

		// ---- the walk --------------------------------------------------------------------------
		private static void Walk(RectTransform parent, RectTransform space, List<string> issues)
		{
			Vector3[] corners = new Vector3[4];
			Rect parentRect = ToSpace(parent, space, corners);
			List<Entry> entries = new List<Entry>();
			for (int i = 0; i < parent.childCount; i++)
			{
				RectTransform child = parent.GetChild(i) as RectTransform;
				if (child == null || !child.gameObject.activeInHierarchy) continue;
				Rect bounds;
				if (Bounds(child, space, corners, out bounds))
				{
					bool background = Contains(bounds, parentRect) && parentRect.width > 8f && parentRect.height > 8f;   // covers the parent: a backdrop, not content
					entries.Add(new Entry { rect = child, bounds = bounds, decoration = IsDecoration(child.name) || background });
					Text text = child.GetComponent<Text>();
					// "text running into a box" only means something for a drawn box: a bare RectTransform used
					// as a layout group (CaseDisplay, Info Panel…) may place a caption outside itself, so the test
					// uses the nearest ancestor that actually draws (card, frame, window).
					if (text != null)
					{
						RectTransform box = parent;
						while (box != null && !(box.GetComponent<Graphic>() != null && box.GetComponent<Graphic>().color.a > 0.05f)) box = box.parent as RectTransform;
						if (box != null)
						{
							Rect boxRect = ToSpace(box, space, corners);
							if (!Contains(boxRect, bounds) && boxRect.width > 8f && boxRect.height > 8f)
								issues.Add("text leaves " + box.name + ": " + Path(child) + " " + Fmt(bounds) + " outside " + Fmt(boxRect));
						}
					}
				}
				Walk(child, space, issues);
			}
			for (int a = 0; a < entries.Count; a++)
			{
				if (entries[a].decoration) continue;
				for (int b = a + 1; b < entries.Count; b++)
				{
					if (entries[b].decoration) continue;
					Rect ra = entries[a].bounds, rb = entries[b].bounds;
					float area = OverlapArea(ra, rb);
					if (area <= MinOverlap) continue;
					if (Contains(ra, rb) || Contains(rb, ra)) continue;   // a frame and its content
					issues.Add("overlap " + area.ToString("0") + " px²: " + Path(entries[a].rect) + " " + Fmt(ra) + " vs " + entries[b].rect.name + " " + Fmt(rb));
				}
			}
		}
	}
}
