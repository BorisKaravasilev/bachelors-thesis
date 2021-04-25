using MyRandom;
using System;
using System.Collections.Generic;
using TaskManagement;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Generates a noise texture for each terrain node.
	/// </summary>
	public class GenerateNodesNoises : DividableTask
	{
		// Inputs
		private Vector3 position;
		private float areaRadius;
		private int resolution;

		// Inputs from previous tasks
		private Func<List<TerrainNode>> getTerrainNodes;
		private List<TerrainNode> terrainNodes;

		// Outputs
		private List<Color[]> noises;

		// Internal
		private int texturePixelCount;
		private float worldPixelLength;
		private const float MAX_RANDOM_OFFSET = 10000f;

		public GenerateNodesNoises(Vector3 position, float areaRadius, int resolution, Func<List<TerrainNode>> getTerrainNodes)
		{
			Name = "Generate Nodes Noises";

			this.position = position;
			this.areaRadius = areaRadius;
			this.resolution = resolution;
			this.getTerrainNodes = getTerrainNodes;

			noises = new List<Color[]>();
			texturePixelCount = resolution * resolution;

			float diameter = areaRadius * 2;
			worldPixelLength = diameter / (resolution);
		}

		public List<Color[]> GetResult()
		{
			if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
			return noises;
		}

		protected override void ExecuteStep()
		{
			int nodeIndex = ExecutedSteps;
			Color[] noise = new Color[texturePixelCount];

			Noise2DParams nodeNoiseParams = terrainNodes[nodeIndex].Type.NoiseParams;
			Noise2D nodeNoise = Noise2DFactory.GetNoise(nodeNoiseParams);

			RandomFromSeed randomGenerator = new RandomFromSeed(position, Name);
			float randomXOffset = randomGenerator.NextFloat() * MAX_RANDOM_OFFSET;
			float randomYOffset = randomGenerator.NextFloat() * MAX_RANDOM_OFFSET;

			for (int pixelIndex = 0; pixelIndex < texturePixelCount; pixelIndex++)
			{
				Vector2Int pixel2DCoords = TextureFunctions.ArrayIndexToCoords(resolution, resolution, pixelIndex);

				Vector2 noiseCoords = new Vector2
				{
					x = (float) pixel2DCoords.x / resolution + randomXOffset,
					y = (float) pixel2DCoords.y / resolution + randomYOffset
				};

				float intensity = nodeNoise.GetValue(noiseCoords);
				Color pixelColor = new Color(intensity, intensity, intensity);

				noise[pixelIndex] = pixelColor;
			}

			noises.Add(noise);
		}

		protected override void GetInputFromPreviousStep()
		{
			terrainNodes = getTerrainNodes();
		}

		protected override void SetSteps()
		{
			TotalSteps = terrainNodes.Count;
			RemainingSteps = TotalSteps;
		}
	}
}