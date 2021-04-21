using System;
using System.Collections.Generic;
using TaskManagement;
using UnityEngine;

namespace IslandAreaTest
{
	/// <summary>
	/// Generates a list of heightmaps represented by flattened pixel arrays, one pixel array for each terrain node.
	/// </summary>
	public class GenerateNodesGradients : DividableTask
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

		private float worldPixelLength;

		public GenerateNodesGradients(int resolution, float areaRadius, Func<List<TerrainNode>> getTerrainNodes)
		{
			Name = "Generate Nodes Gradients";

			this.resolution = resolution;
			this.areaRadius = areaRadius;
			this.getTerrainNodes = getTerrainNodes;

			heightmaps = new List<Color[]>();
			float areaDiameter = areaRadius * 2;
			worldPixelLength = areaDiameter / resolution;
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
			PixelBoundingBox gradientArea = terrainNode.GetPixelBoundingBox(resolution, areaRadius);

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

		private Color GetGradientPixel(Vector2Int pixelCoords, TerrainNode terrainNode)
		{
			Vector3 pixelWorldPos = TextureFunctions.PixelToLocalPosition(pixelCoords, worldPixelLength, areaRadius);
			float distanceFromCenter = Vector3.Distance(pixelWorldPos, terrainNode.Position);
			float normalizedDistance = distanceFromCenter / terrainNode.Radius;

			return Color.Lerp(Color.white, Color.black, normalizedDistance);
		}
	}
}