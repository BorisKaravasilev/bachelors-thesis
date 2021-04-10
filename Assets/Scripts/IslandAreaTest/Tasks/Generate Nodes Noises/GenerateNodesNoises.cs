using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateNodesNoises : SingleTask
{
	// Inputs
	private int resolution;

	// Inputs from previous task
	private Func<List<TerrainNode>> getTerrainNodes;
	private List<TerrainNode> terrainNodes;

	// Outputs
	private List<Color[]> noises;

	// Internal
	private int texturePixelCount;
	private PerlinNoise2D perlinNoise;

	public GenerateNodesNoises(int resolution, Noise2DParams noiseParams, Func<List<TerrainNode>> getTerrainNodes, int stepSize = 1)
	{
		Name = "Generate Nodes Noises";
		StepSize = stepSize;

		this.resolution = resolution;
		this.getTerrainNodes = getTerrainNodes;

		noises = new List<Color[]>();
		texturePixelCount = resolution * resolution;
	}

	protected override void ExecuteStep()
	{
		int nodeIndex = ExecutedSteps;
		Color[] noise = new Color[texturePixelCount];

		for (int pixelIndex = 0; pixelIndex < texturePixelCount; pixelIndex++)
		{
			Vector2Int pixel2DCoords = TextureFunctions.ArrayIndexToCoords(resolution, resolution, pixelIndex);
			float intensity = perlinNoise.GetValue(pixel2DCoords);
			Color pixelColor = new Color(intensity, intensity, intensity);
			noise[pixelIndex] = pixelColor;
		}
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
