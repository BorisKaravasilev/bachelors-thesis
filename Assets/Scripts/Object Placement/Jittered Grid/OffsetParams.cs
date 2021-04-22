using Noise;
using Unity.Mathematics;
using UnityEngine;

namespace ObjectPlacement.JitteredGrid
{
	[System.Serializable]
	public class OffsetParams
	{
		[Range(0f, 20f)]
		[SerializeField] private float maxOffset = 0f;
		public float MaxOffset => maxOffset;

		[Range(0f, 1f)]
		[SerializeField] private float threshold = 0f;
		public float Threshold => threshold;

		[SerializeField] private Noise2DParams xOffsetParams;
		public Noise2DParams XOffsetParams
		{
			get => CheckInitialized(xOffsetParams);
			private set => xOffsetParams = value;
		}

		[SerializeField] private Noise2DParams zOffsetParams;
		public Noise2DParams ZOffsetParams
		{
			get => CheckInitialized(zOffsetParams);
			private set => zOffsetParams = value;
		}

		[SerializeField] private Noise2DParams thresholdParams;
		public Noise2DParams ThresholdParams
		{
			get => CheckInitialized(thresholdParams);
			private set => thresholdParams = value;
		}

		public OffsetParams(float maxOffset, float threshold)
		{
			this.maxOffset = maxOffset;
			this.threshold = threshold;
		}

		public void SetNoiseParams(Noise2DParams xOffsetParams, Noise2DParams zOffsetParams, Noise2DParams thresholdParams)
		{
			this.xOffsetParams = xOffsetParams;
			this.zOffsetParams = zOffsetParams;
			this.thresholdParams = thresholdParams;
		}

		/// <summary>
		/// Prints error if noise parameters are not initialized.
		/// </summary>
		private Noise2DParams CheckInitialized(Noise2DParams noiseParams)
		{
			if (noiseParams == null)
			{
				Debug.LogError("Trying to get not initialized \"Noise2DParams\" from \"OffsetParams\".");
			}

			return noiseParams;
		}
	}
}
