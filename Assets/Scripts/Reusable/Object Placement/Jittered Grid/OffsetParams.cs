using Noise;
using UnityEngine;

namespace ObjectPlacement.JitteredGrid
{
	[System.Serializable]
	public class OffsetParams
	{
		[Range(0f, 20f)]
		private float maxOffset = 0f;
		public float MaxOffset => maxOffset;

		[Range(0f, 1f)]
		private float threshold = 0f;
		public float Threshold => threshold;

		private INoise2D xOffsetNoise;
		public INoise2D XOffsetNoise => CheckInitialized(xOffsetNoise);

		private INoise2D zOffsetNoise;
		public INoise2D ZOffsetNoise => CheckInitialized(zOffsetNoise);

		private INoise2D thresholdNoise;
		public INoise2D ThresholdNoise => CheckInitialized(thresholdNoise);

		public OffsetParams(float maxOffset, float threshold)
		{
			this.maxOffset = maxOffset;
			this.threshold = threshold;
		}

		public void SetNoises(INoise2D xOffsetNoise, INoise2D zOffsetNoise, INoise2D thresholdNoise)
		{
			this.xOffsetNoise = xOffsetNoise;
			this.zOffsetNoise = zOffsetNoise;
			this.thresholdNoise = thresholdNoise;
		}

		/// <summary>
		/// Prints error if noise is not initialized.
		/// </summary>
		private INoise2D CheckInitialized(INoise2D noise)
		{
			if (noise == null)
			{
				Debug.LogError("Trying to get not initialized \"INoise2D\" from \"OffsetNoise\"");
			}

			return noise;
		}
	}
}
