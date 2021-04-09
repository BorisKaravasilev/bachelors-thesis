using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateNodesHeightmap : SingleTask
{
	// Inputs
	private int resolution;
	private float areaRadius;
	private Func<List<TerrainNode>> getTerrainNodes;

	// Inputs from previous task
	private List<TerrainNode> terrainNodes;

	// Outputs
	private Color[] texturePixels;

	// Internal
	private float areaDiameter;
	private float pixelLength;

	public GenerateNodesHeightmap(int resolution, float areaRadius, Func<List<TerrainNode>> getTerrainNodes)
	{
		Name = "Generate Nodes Heightmap";

		this.resolution = resolution;
		this.areaRadius = areaRadius;
		this.getTerrainNodes = getTerrainNodes;
		this.texturePixels = new Color[resolution*resolution];

		areaDiameter = 2 * areaRadius;
		pixelLength = areaDiameter / resolution;
	}

	public Color[] GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return texturePixels;
	}

	protected override void ExecuteStep()
	{
		int pixelIndex = ExecutedSteps;
		Color pixelColor;
		Vector2Int pixel2DCoords = TextureFunctions.ArrayIndexToCoords(resolution, resolution, pixelIndex);
		Vector3 areaCenter = Vector3.zero;

		if (GetPixelDistanceFromPoint(pixel2DCoords, areaCenter) > areaRadius)
		{
			pixelColor = new Color(0f, 0f, 0f, 0f);
		}
		else
		{
			float pixelIntensity = 0f;

			foreach (TerrainNode terrainNode in terrainNodes)
			{
				float distanceFromNode = GetPixelDistanceFromPoint(pixel2DCoords, terrainNode.Position);
				float normalizedDistance = Mathf.Clamp(distanceFromNode, 0f, terrainNode.Radius) / terrainNode.Radius;
				pixelIntensity += 1f - normalizedDistance;

				if (pixelIntensity > 1f) pixelIntensity = 1f;
			}

			pixelColor = new Color(pixelIntensity, pixelIntensity, pixelIntensity, 1f);
		}

		texturePixels[pixelIndex] = pixelColor;
	}

	protected override void GetInputFromPreviousStep()
	{
		this.terrainNodes = getTerrainNodes();
	}

	protected override void SetSteps()
	{
		TotalSteps = resolution * resolution;
		RemainingSteps = TotalSteps;
		StepSize = resolution / 2;
	}

	/// <summary>
	/// Calculates the distance between the center of the pixel and the center of the height map.
	/// </summary>
	private float GetPixelDistanceFromPoint(Vector2Int pixelCoords, Vector3 point)
	{
		Vector3 pixelPosition = GetPixelLocalPosition(pixelCoords.x, pixelCoords.y);
		return Vector3.Distance(pixelPosition, point);
	}

	private Vector3 GetPixelLocalPosition(int x, int y)
	{
		float localX = GetPixelsLength(x) + (pixelLength / 2) - areaRadius;
		float localY = 0f;
		float localZ = GetPixelsLength(y) + (pixelLength / 2) - areaRadius;

		return new Vector3(localX, localY, localZ);
	}

	private float GetPixelsLength(int pixelsCount)
	{
		return ((float) pixelsCount / resolution) * areaDiameter;
	}
}
