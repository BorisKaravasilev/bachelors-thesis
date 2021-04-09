using System;
using System.Collections.Generic;
using MiscUtil.Xml.Linq.Extensions;
using UnityEngine;

/// <summary>
/// Generates a list of heightmaps represented by flattened pixel arrays, one pixel array for each terrain node.
/// </summary>
public class GenerateNodesGradients : SingleTask
{
	// Inputs
	private int resolution;
	private float areaRadius;

	// Inputs from previous task
	private Func<List<TerrainNode>> getTerrainNodes;
	private List<TerrainNode> terrainNodes;

	// Outputs
	private List<Color[]> heightmaps;

	// Internal
	int heightmapsToGenerate;
	int pixelsPerHeightmap;

	private float worldPixelLengthHalf;
	private float worldPixelLength;

	public GenerateNodesGradients(int resolution, float areaRadius, Func<List<TerrainNode>> getTerrainNodes)
	{
		Name = "Generate Nodes Gradients";

		this.resolution = resolution;
		this.areaRadius = areaRadius;
		this.getTerrainNodes = getTerrainNodes;

		heightmaps = new List<Color[]>();
		float areaDiameter  = areaRadius * 2;
		worldPixelLength = areaDiameter / resolution;
		worldPixelLengthHalf = worldPixelLength / 2;
	}

	public List<Color[]> GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return heightmaps;
	}

	protected override void ExecuteStep()
	{
		int nodeIndex = ExecutedSteps;
		TerrainNode terrainNode = terrainNodes[nodeIndex];
		PixelBoundingBox gradientArea = GetNodeBoundingBox(terrainNode);

		Debug.Log(gradientArea.BottomLeft);
		Debug.Log(gradientArea.TopRight);

		// Loop through pixels in the gradient's bounding box and calculation of the gradient pixels
		for (int x = gradientArea.BottomLeft.x; x <= gradientArea.TopRight.x; x++)
		{
			for (int y = gradientArea.BottomLeft.y; y <= gradientArea.TopRight.y; y++)
			{
				Vector2Int pixelCoords = new Vector2Int(x, y);
				int pixelIndex = TextureFunctions.CoordsToArrayIndex(resolution, resolution, pixelCoords);
				heightmaps[nodeIndex][pixelIndex] = GetGradientPixel(pixelCoords, terrainNode);
			}
		}
	}

	protected override void GetInputFromPreviousStep()
	{
		terrainNodes = getTerrainNodes();

		for (int i = 0; i < terrainNodes.Count; i++)
		{
			heightmaps.Add(TextureFunctions.GetBlackPixels(resolution * resolution));
		}
	}

	protected override void SetSteps()
	{
		TotalSteps = terrainNodes.Count;
		RemainingSteps = TotalSteps;
	}

	private Vector3 GetPixelLocalPosition(int x, int y)
	{
		float localX = PixelsWorldLength(x) + worldPixelLengthHalf - areaRadius;
		float localY = 0f;
		float localZ = PixelsWorldLength(y) + worldPixelLengthHalf - areaRadius;

		return new Vector3(localX, localY, localZ);
	}

	private Vector2Int LocalPositionToPixel(Vector3 position)
	{
		int x = (int) ((position.x + areaRadius) / worldPixelLength);
		int y = (int) ((position.y + areaRadius) / worldPixelLength);

		int maxPixelIndex = resolution - 1;
		if (x < 0) x = 0;
		if (x > maxPixelIndex) x = maxPixelIndex;

		if (y < 0) y = 0;
		if (y > maxPixelIndex) y = maxPixelIndex;

		return new Vector2Int(x, y);
	}

	private float PixelsWorldLength(int pixelsCount)
	{
		return pixelsCount * worldPixelLength;
	}

	private PixelBoundingBox GetNodeBoundingBox(TerrainNode node)
	{
		Vector3 areaLocalBottomLeft = node.Position;
		areaLocalBottomLeft.x -= node.Radius;
		areaLocalBottomLeft.y -= node.Radius;

		Vector3 areaLocalTopRight = node.Position;
		areaLocalTopRight.x += node.Radius;
		areaLocalTopRight.y += node.Radius;

		Vector2Int areaPixelBottomLeft = LocalPositionToPixel(areaLocalBottomLeft);
		Vector2Int areaPixelTopRight = LocalPositionToPixel(areaLocalTopRight);

		return new PixelBoundingBox(areaPixelBottomLeft, areaPixelTopRight);
	}

	private Color GetGradientPixel(Vector2Int pixelCoords, TerrainNode terrainNode)
	{
		Vector3 pixelWorldPos = GetPixelLocalPosition(pixelCoords.x, pixelCoords.y);
		float distanceFromCenter = Vector3.Distance(pixelWorldPos, terrainNode.Position);
		float normalizedDistance = distanceFromCenter / terrainNode.Radius;

		return Color.Lerp(Color.white, Color.black, normalizedDistance);
	}
}
