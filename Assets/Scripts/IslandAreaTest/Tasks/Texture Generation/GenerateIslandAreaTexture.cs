using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateIslandAreaTexture : SingleTask
{
	// Inputs
	private TerrainNodesParams terrainNodesParams;

	// Inputs from previous task
	private Func<List<TerrainNode>> getTerrainNodes;
	private List<TerrainNode> terrainNodes;

	private Func<Color[]> getHeightmap;
	private Color[] heightmap;

	// Outputs
	private Color[] texture;

	// Internal
	private List<TerrainType> orderedTerrainTypes; // by starting height

	public GenerateIslandAreaTexture(TerrainNodesParams terrainNodesParams, Func<List<TerrainNode>> getTerrainNodes, Func<Color[]> getHeightmap)
	{
		Name = "Generate Island Area Texture";

		this.terrainNodesParams = terrainNodesParams;
		this.getTerrainNodes = getTerrainNodes;
		this.getHeightmap = getHeightmap;
	}

	public Color[] GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return texture;
	}

	protected override void ExecuteStep()
	{
		for (int pixelIndex = 0; pixelIndex < heightmap.Length; pixelIndex++)
		{
			float pixelHeight = heightmap[pixelIndex].r;    // All channels (r,g,b) should have the same value
			Color pixelColor = GetPixelColor(pixelHeight, orderedTerrainTypes);
			texture[pixelIndex] = pixelColor;
		}
	}

	protected override void GetInputFromPreviousStep()
	{
		terrainNodes = getTerrainNodes();
		orderedTerrainTypes = OrderTerrainTypes(terrainNodesParams.TerrainTypes);

		heightmap = getHeightmap();
		texture = new Color[heightmap.Length];
	}

	protected override void SetSteps()
	{
		TotalSteps = 1;
		RemainingSteps = TotalSteps;
	}

	/// <summary>
	/// Order terrain types by starting height.
	/// </summary>
	private List<TerrainType> OrderTerrainTypes(List<TerrainType> terrainTypes)
	{
		List<TerrainType> orderedTerrainTypes = terrainTypes.OrderBy(terrainType => terrainType.StartingHeight).ToList();
		return orderedTerrainTypes;
	}

	/// <summary>
	/// Get terrain types contained in the terrain nodes without repetition.
	/// </summary>
	private List<TerrainType> GetUniqueTerrainTypes(List<TerrainNode> terrainNodes)
	{
		List<TerrainType> terrainTypes = new List<TerrainType>();

		foreach (TerrainNode node in terrainNodes)
		{
			bool typeAdded = terrainTypes.Contains(node.Type);

			if (!typeAdded)
			{
				terrainTypes.Add(node.Type);
			}
		}

		return terrainTypes;
	}

	private Color GetPixelColor(float pixelHeight, List<TerrainType> ascendingHeightTypes)
	{
		Color pixelColor = new Color();

		for (int typeIndex = 0; typeIndex < orderedTerrainTypes.Count; typeIndex++)
		{
			TerrainType currentType = orderedTerrainTypes[typeIndex];
			pixelColor = currentType.Color;

			bool isLastType = typeIndex == orderedTerrainTypes.Count - 1;

			if (!isLastType)
			{
				TerrainType higherType = orderedTerrainTypes[typeIndex + 1];

				if (pixelHeight < higherType.StartingHeight) break;
			}
		}

		return pixelColor;
	}
}
