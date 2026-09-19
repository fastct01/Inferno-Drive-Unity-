using System.Collections.Generic;

namespace Unity.Services.LevelPlay
{
	public sealed class LevelPlaySegment
	{
		public int Level;

		public int IsPaying;

		public long UserCreationDate;

		public double IapTotal;

		public string SegmentName;

		public readonly Dictionary<string, string> CustomData;

		public void SetCustom(string key, string value)
		{
		}

		public Dictionary<string, string> GetSegmentAsDictionary()
		{
			return null;
		}
	}
}
