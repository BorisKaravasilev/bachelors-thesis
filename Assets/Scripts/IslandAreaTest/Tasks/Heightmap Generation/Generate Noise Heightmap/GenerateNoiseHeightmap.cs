using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateNoiseHeightmap : SingleTask
{
	// Inputs
	private int resolution;

	// Inputs from previous task

	// Outputs
	private Color[] noiseTexturePixels;

	// Internal
	private int pixelCount;
	private PerlinNoise2D perlinNoise;

	public GenerateNoiseHeightmap(int resolution, Noise2DParams noiseParams, int stepSize = 1)
	{
		pixelCount = resolution * resolution;
		Name = "Generate Noise Heightmap";
		StepSize = stepSize;

		this.resolution = resolution;

		noiseTexturePixels = new Color[pixelCount];
		perlinNoise = new PerlinNoise2D(noiseParams);
	}

	public Color[] GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return noiseTexturePixels;
	}

	public List<Color[]> GetResultInList()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		List<Color[]> resultWrapperList = new List<Color[]>();
		resultWrapperList.Add(noiseTexturePixels);

		return resultWrapperList;
	}

	protected override void ExecuteStep()
	{
		int index = ExecutedSteps;
		Vector2Int pixel2DCoords = TextureFunctions.ArrayIndexToCoords(resolution, resolution, index);
		float intensity = perlinNoise.GetValue(pixel2DCoords);
		Color pixelColor = new Color(intensity, intensity, intensity);
		noiseTexturePixels[index] = pixelColor;
	}

	protected override void GetInputFromPreviousStep()
	{
		// TODO
	}

	protected override void SetSteps()
	{
		TotalSteps = pixelCount;
		RemainingSteps = TotalSteps;
	}
}
