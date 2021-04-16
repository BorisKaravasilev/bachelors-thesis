using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateIslandAreaTexture : SingleTask
{
	// Inputs
	private int resolution;
	private List<TerrainType> terrainTypes;
	private float blendingHeight;

	// Inputs from previous task
	private Func<List<TerrainNode>> getTerrainNodes;
	private List<TerrainNode> terrainNodes;

	private Func<Color[]> getHeightmap;
	private Color[] heightmap;

	private Func<List<Color[]>> getTerrainNodesHeightmaps;
	private List<Color[]> terrainNodesHeightmaps;

	// Outputs
	private Color[] texture;

	// Internal
	private TerrainTextureGenerator textureGenerator;

	public GenerateIslandAreaTexture(int resolution, TerrainNodesParams terrainNodesParams, Func<List<TerrainNode>> getTerrainNodes, Func<Color[]> getHeightmap, Func<List<Color[]>> getTerrainNodesHeightmaps, int stepSize)
	{
		Name = "Generate Island Area Texture";
		StepSize = stepSize;

		this.resolution = resolution;

		terrainTypes = terrainNodesParams.TerrainTypes;
		blendingHeight = terrainNodesParams.BlendingHeight;

		this.getTerrainNodes = getTerrainNodes;
		this.getHeightmap = getHeightmap;
		this.getTerrainNodesHeightmaps = getTerrainNodesHeightmaps;
	}

	public Color[] GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return texture;
	}

	protected override void ExecuteStep()
	{
		int firstIndex = ExecutedSteps * resolution;
		int lastIndex = firstIndex + resolution - 1;

		for (int pixelIndex = firstIndex; pixelIndex < lastIndex; pixelIndex++)
		{
			// All channels (r,g,b) of the heightmap should have the same value
			float pixelHeight = heightmap[pixelIndex].r;
			texture[pixelIndex] = textureGenerator.GetPixelColor(pixelHeight, pixelIndex);
		}
	}

	protected override void GetInputFromPreviousStep()
	{
		terrainNodes = getTerrainNodes();
		terrainNodesHeightmaps = getTerrainNodesHeightmaps();
		heightmap = getHeightmap();

		texture = new Color[heightmap.Length];
		textureGenerator = new TerrainTextureGenerator(terrainNodes, terrainNodesHeightmaps, terrainTypes, blendingHeight);
	}

	protected override void SetSteps()
	{
		TotalSteps = heightmap.Length / resolution;
		RemainingSteps = TotalSteps;
	}
}
