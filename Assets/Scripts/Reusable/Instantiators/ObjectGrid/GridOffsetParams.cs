using UnityEngine;

namespace Instantiators.ObjectGrid
{
	[System.Serializable]
	public class GridOffsetParams
	{
		[Range(0f, 20f)] public float maxOffset;

		[Range(0f, 1f)] public float threshold;

		public Noise2DParams xOffsetParams;
		public Noise2DParams zOffsetParams;
		public Noise2DParams thresholdParams;
	}
}