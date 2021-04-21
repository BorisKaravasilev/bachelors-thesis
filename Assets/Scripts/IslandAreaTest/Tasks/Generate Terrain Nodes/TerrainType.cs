using UnityEngine;

namespace IslandAreaTest
{
	[System.Serializable]
	public class TerrainType
	{
		public string Name = "New Terrain Type";
		public Color Color;

		[Range(0f, 1f)]
		public float
			StartingHeight =
				0; // This terrain type's color will be used from its starting height to the starting height of other type

		[Range(0f, 1f)] public float DominationProbability = 1f;
		public Noise2DParams NoiseParams;

		public TerrainType(string name, Color color, float startingHeight, float dominationProbability,
			Noise2DParams noiseParams)
		{
			Name = name;
			Color = color;
			StartingHeight = startingHeight;
			DominationProbability = dominationProbability;
			NoiseParams = noiseParams;
		}

		/// <summary>
		/// Returns a deep copy of the object.
		/// </summary>
		public TerrainType DeepCopy()
		{
			Noise2DParams noiseParams = new Noise2DParams(NoiseParams.Type, NoiseParams.Scale, NoiseParams.OffsetX,
				NoiseParams.OffsetY);
			TerrainType copy = new TerrainType(Name, Color, StartingHeight, DominationProbability, noiseParams);
			return copy;
		}
	}
}