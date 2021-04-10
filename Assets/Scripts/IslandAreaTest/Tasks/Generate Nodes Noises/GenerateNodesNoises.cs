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

			Noise2DParams nodeNoiseParams = terrainNodes[nodeIndex].Type.NoiseParams;
			Noise2D nodeNoise = Noise2DFactory.GetNoise(nodeNoiseParams);

			float intensity = nodeNoise.GetValue(pixel2DCoords);
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
