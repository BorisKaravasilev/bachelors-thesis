using System;
using System.Collections.Generic;
using TaskManagement;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Generates a colorful texture for the terrain from the previous tasks inputs.
	/// </summary>
	public class GenerateIslandAreaTexture : DividableTask
	{
		// Inputs
		private int resolution;
		private List<TerrainType> terrainTypes;
		private float blendingHeight;

		// Inputs from previous tasks
		private Func<List<TerrainNode>> getTerrainNodes;
		private List<TerrainNode> terrainNodes;

		private Func<Color[]> getHeightmap;
		private Color[] heightmap;

		private Func<List<Color[]>> getTerrainNodesHeightmaps;
		private List<Color[]> terrainNodesHeightmaps;

		// Outputs
		private Color[] texturePixels;
		private TerrainBlend[] terrainTypesAtPixels;

		// Internal
		private TerrainTextureGenerator textureGenerator;

		public GenerateIslandAreaTexture(GenerateIslandAreaTextureParams parameters)
		{
			Name = "Generate Island Area Texture";

			resolution = parameters.Resolution;

			terrainTypes = parameters.TerrainNodesParams.TerrainTypes;
			blendingHeight = parameters.TerrainNodesParams.BlendingHeight;

			getTerrainNodes = parameters.GetTerrainNodes;
			getHeightmap = parameters.GetHeightmap;
			getTerrainNodesHeightmaps = parameters.GetTerrainNodesHeightmaps;
		}

		public Color[] GetResult()
		{
			if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
			return texturePixels;
		}

		public List<Color[]> GetResultInList()
		{
			if (!Finished) Debug.LogWarning($"\"GetResultInList()\" called on {Name} task before finished.");
			List<Color[]> resultWrapperList = new List<Color[]> {texturePixels};
			return resultWrapperList;
		}

		public TerrainBlend[] GetTerrainTypesAtPixels()
		{
			if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
			return terrainTypesAtPixels;
		}

		protected override void ExecuteStep()
		{
			int firstIndex = ExecutedSteps * resolution;
			int lastIndex = firstIndex + resolution;

			for (int pixelIndex = firstIndex; pixelIndex < lastIndex; pixelIndex++)
			{
				// All channels (r,g,b) of the heightmap should have the same value
				float pixelHeight = heightmap[pixelIndex].r;

				terrainTypesAtPixels[pixelIndex] = textureGenerator.GetPixelTerrainBlend(pixelHeight, pixelIndex);
				texturePixels[pixelIndex] = terrainTypesAtPixels[pixelIndex].GetColor();
			}
		}

		protected override void GetInputFromPreviousTask()
		{
			terrainNodes = getTerrainNodes();
			terrainNodesHeightmaps = getTerrainNodesHeightmaps();
			heightmap = getHeightmap();

			texturePixels = new Color[heightmap.Length];
			terrainTypesAtPixels = new TerrainBlend[heightmap.Length];
			textureGenerator =
				new TerrainTextureGenerator(terrainNodes, terrainNodesHeightmaps, terrainTypes, blendingHeight);
		}

		protected override void SetSteps()
		{
			TotalSteps = heightmap.Length / resolution;
			RemainingSteps = TotalSteps;
		}
	}
}