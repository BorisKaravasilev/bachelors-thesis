using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateNoiseHeightmap : SingleTask
{
	// Inputs
	private int resolution;
	private Noise2DParams noiseParams;

	// Inputs from previous task

	// Outputs
	private Color[] noiseTexturePixels;

	// Internal
	private PerlinNoise2D perlinNoise;

	public GenerateNoiseHeightmap(int resolution, Noise2DParams noiseParams)
	{
		int pixelCount = resolution * resolution;

		Name = "Generate Noise Heightmap";
		StepSize = resolution;
		TotalSteps = pixelCount;
		RemainingSteps = TotalSteps;

		this.resolution = resolution;
		this.noiseParams = noiseParams;

		noiseTexturePixels = new Color[pixelCount];
		perlinNoise = new PerlinNoise2D(noiseParams);
	}

	public Color[] GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return noiseTexturePixels;
	}

	protected override void ExecuteStep()
	{
		int index = ExecutedSteps;
		Vector2Int pixel2DCoords = TextureFunctions.ArrayIndexToCoords(resolution, index);
		float intensity = perlinNoise.GetValue(pixel2DCoords);
		Color pixelColor = new Color(intensity, intensity, intensity);
		noiseTexturePixels[index] = pixelColor;
	}

	protected override void GetInputFromPreviousStep()
	{
		// TODO
	}
}
