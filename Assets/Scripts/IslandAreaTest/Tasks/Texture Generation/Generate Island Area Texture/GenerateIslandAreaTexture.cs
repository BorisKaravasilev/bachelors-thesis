using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.VersionControl;
using UnityEngine;

public class GenerateIslandAreaTexture : SingleTask
{
	// Inputs
	private int resolution;
	private float areaRadius;
	private TerrainNodesParams terrainNodesParams;

	// Inputs from previous task
	private Func<List<TerrainNode>> getTerrainNodes;
	private List<TerrainNode> terrainNodes;

	private Func<Color[]> getHeightmap;
	private Color[] heightmap;

	private Func<List<Color[]>> getMultipliedGradientsAndNoises;
	private List<Color[]> multipliedGradientsAndNoises;

	// Outputs
	private Color[] texture;

	// Internal
	private List<TerrainType> terrainTypesAroundDominantNodes;

	private List<TerrainNode> dominantNodes;
	private List<PixelBoundingBox> dominantNodesBoundingBoxes;
	private List<TerrainType> orderedTerrainTypes; // by starting height
	private List<BlendingRegion> blendingRegions;  // where two terrain types meet (blending based on height)
	private List<BlendingRegion> blendingRegionsAroundDominantNodes;

	public GenerateIslandAreaTexture(int resolution, float areaRadius, TerrainNodesParams terrainNodesParams, Func<List<TerrainNode>> getTerrainNodes, Func<Color[]> getHeightmap, Func<List<Color[]>> getMultipliedGradientsAndNoises)
	{
		Name = "Generate Island Area Texture";

		this.resolution = resolution;
		this.areaRadius = areaRadius;
		this.terrainNodesParams = terrainNodesParams;
		this.getTerrainNodes = getTerrainNodes;
		this.getHeightmap = getHeightmap;

		this.getMultipliedGradientsAndNoises = getMultipliedGradientsAndNoises;
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
			float pixelHeight = heightmap[pixelIndex].r;    // All channels (r,g,b) should have the same value (gray scale image)
			texture[pixelIndex] = GetPixelColor(pixelHeight, pixelIndex);
		}
	}

	protected override void GetInputFromPreviousStep()
	{
		terrainNodes = getTerrainNodes();
		dominantNodes = GetDominantTerrainNodes();
		dominantNodesBoundingBoxes = GetNodesPixelBoundingBoxes(dominantNodes);

		orderedTerrainTypes = OrderTerrainTypes(terrainNodesParams.TerrainTypes);
		terrainTypesAroundDominantNodes = TerrainTypesAroundDominantNodes(orderedTerrainTypes);

		blendingRegions = FindBlendingRegions(orderedTerrainTypes, terrainNodesParams.BlendingDistance);
		blendingRegionsAroundDominantNodes =
			FindBlendingRegions(terrainTypesAroundDominantNodes, terrainNodesParams.BlendingDistance);

		heightmap = getHeightmap();
		texture = new Color[heightmap.Length];

		multipliedGradientsAndNoises = getMultipliedGradientsAndNoises();
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

	private Color GetPixelColor(float pixelHeight, int pixelIndex)
	{
		Color pixelColor;
		BlendingRegion enclosingBlendingRegion = GetBlendingRegion(pixelHeight, blendingRegions);
		BlendingRegion enclosingDominantBlendingRegion = GetBlendingRegion(pixelHeight, blendingRegionsAroundDominantNodes);
		List<TerrainNode> dominantNodesInRange = DominantNodesInRange(pixelIndex);

		Color normalColor = GetHeightColor(pixelHeight, orderedTerrainTypes);
		Color dominantColor = GetHeightColor(pixelHeight, terrainTypesAroundDominantNodes);

		if (enclosingBlendingRegion != null) normalColor = enclosingBlendingRegion.GetColor(pixelHeight);

		if (enclosingDominantBlendingRegion != null)
			dominantColor = enclosingDominantBlendingRegion.GetColor(pixelHeight);

		if (dominantNodesInRange.Count > 0)
		{
			pixelColor = GetHeightColorAroundDominantNode(pixelIndex, dominantNodesInRange, normalColor, dominantColor);
		}
		else
		{
			pixelColor = normalColor;
		}

		//pixelColor = GetHeightColor(pixelHeight, orderedTerrainTypes);

		//if (dominantNodeInRange != null)
		//{
		//	int dominantNodeIndex = terrainNodes.IndexOf(dominantNodeInRange);
		//	pixelColor = GetHeightColorAroundDominantNode(pixelIndex, dominantNodeIndex, pixelHeight);
		//	//pixelColor = GetHeightColor(pixelHeight, terrainTypesAroundDominantNodes);
		//}
		////else if (enclosingBlendingRegion != null)
		////{
		////	pixelColor = enclosingBlendingRegion.GetColor(pixelHeight);
		////}
		//else
		//{
		//	pixelColor = GetHeightColor(pixelHeight, orderedTerrainTypes);
		//}

		//if (dominantNodeInRange != null)
		//{
		//	pixelColor = GetColorInDominantNodeRange(dominantNodeInRange);
		//}
		//else if (enclosingBlendingRegion != null)
		//{
		//	pixelColor = enclosingBlendingRegion.GetColor(pixelHeight);
		//}
		//else
		//{
		//	pixelColor = GetHeightColor(pixelHeight, orderedTerrainTypes);
		//}

		return pixelColor;
	}

	/// <summary>
	/// Removes terrain types directly below a dominant type.
	/// For example if types are {dark sand, sand, grass} and grass is dominant, this function will return {dark sand, grass}.
	/// </summary>
	private List<TerrainType> TerrainTypesAroundDominantNodes(List<TerrainType> ascendingHeightTypes)
	{
		List<TerrainType> terrainTypes = new List<TerrainType>();

		for (int i = 0; i < ascendingHeightTypes.Count; i++)
		{
			TerrainType currentType = ascendingHeightTypes[i];
			int lastIndex = ascendingHeightTypes.Count - 1;

			if (i != lastIndex)
			{
				TerrainType typeAbove = ascendingHeightTypes[i + 1];

				if (typeAbove.DominationProbability > 0f)
				{
					// The dominant type's starting height is lowered to the dominated type.
					TerrainType loweredDominantType = typeAbove.DeepCopy();
					loweredDominantType.StartingHeight = currentType.StartingHeight;
					terrainTypes.Add(loweredDominantType);
					i++;
				}
				else
				{
					terrainTypes.Add(currentType);
				}
			}
			else
			{
				terrainTypes.Add(currentType);
			}
		}

		return terrainTypes;
	}

	private Color GetColorInDominantNodeRange(TerrainNode dominantNode)
	{
		return dominantNode.Type.Color;
	}

	private List<TerrainNode> DominantNodesInRange(int pixelIndex)
	{
		Vector2Int pixelCoords = TextureFunctions.ArrayIndexToCoords(resolution, resolution, pixelIndex);
		List<TerrainNode> dominantNodes = new List<TerrainNode>();

		for (int i = 0; i < terrainNodes.Count; i++)
		{

			if (terrainNodes[i].IsDominant && multipliedGradientsAndNoises[i][pixelIndex].r > 0f)
			{
				dominantNodes.Add(terrainNodes[i]);
			}
		}

		return dominantNodes;
	}

	private List<TerrainNode> GetDominantTerrainNodes()
	{
		List<TerrainNode> dominantNodes = new List<TerrainNode>();

		foreach (TerrainNode terrainNode in terrainNodes)
		{
			if (terrainNode.IsDominant) dominantNodes.Add(terrainNode);
		}

		return dominantNodes;
	}

	private List<PixelBoundingBox> GetNodesPixelBoundingBoxes(List<TerrainNode> nodes)
	{
		List<PixelBoundingBox> boundingBoxes = new List<PixelBoundingBox>();

		foreach (TerrainNode node in nodes)
		{
			boundingBoxes.Add(node.GetPixelBoundingBox(resolution, areaRadius));
		}

		return boundingBoxes;
	}

	private Color GetHeightColorAroundDominantNode(int pixelIndex, List<TerrainNode> dominantNodes, Color normalColor, Color dominantColor)
	{
		float dominantHeightMapHeight = 0f;
		float blendingEdge = terrainNodesParams.BlendingDistance;

		for (int i = 0; i < dominantNodes.Count; i++)
		{
			int nodeIndex = terrainNodes.IndexOf(dominantNodes[i]);
			dominantHeightMapHeight += multipliedGradientsAndNoises[nodeIndex][pixelIndex].r;
		}

		if (dominantHeightMapHeight > blendingEdge)
		{
			return dominantColor;
		}
		else
		{
			float blendInterpolator = dominantHeightMapHeight / blendingEdge;
			return Color.Lerp(normalColor, dominantColor, blendInterpolator);
		}
	}

	private Color GetHeightColor(float pixelHeight, List<TerrainType> ascendingHeightTypes)
	{
		Color pixelColor = new Color();

		for (int typeIndex = 0; typeIndex < ascendingHeightTypes.Count; typeIndex++)
		{
			TerrainType currentType = ascendingHeightTypes[typeIndex];
			pixelColor = currentType.Color;

			bool isLastType = typeIndex == ascendingHeightTypes.Count - 1;

			if (!isLastType)
			{
				TerrainType higherType = ascendingHeightTypes[typeIndex + 1];

				if (pixelHeight < higherType.StartingHeight) break;
			}
		}

		return pixelColor;
	}

	private List<BlendingRegion> FindBlendingRegions(List<TerrainType> ascendingHeightTypes, float blendingDistance)
	{
		List<BlendingRegion> foundRegions = new List<BlendingRegion>();

		if (ascendingHeightTypes.Count > 1)
		{
			for (int i = 1; i < ascendingHeightTypes.Count; i++)
			{
				BlendingRegion newRegion = CalculateBlendingRegion(ascendingHeightTypes, blendingDistance, i);
				foundRegions.Add(newRegion);
			}
		}

		return foundRegions;
	}

	private BlendingRegion CalculateBlendingRegion(List<TerrainType> ascendingHeightTypes, float blendingDistance, int i)
	{
		float previousStartingHeight = ascendingHeightTypes[i - 1].StartingHeight;
		float startingHeight = ascendingHeightTypes[i].StartingHeight;

		bool isLast = i == ascendingHeightTypes.Count - 1;

		float minHeight = previousStartingHeight;
		float maxHeight = isLast ? 1f : ascendingHeightTypes[i + 1].StartingHeight;

		float bottomBorder = Mathf.Clamp(startingHeight - blendingDistance, minHeight, maxHeight);
		float topBorder = Mathf.Clamp(startingHeight + blendingDistance, minHeight, maxHeight);

		Color bottomColor = ascendingHeightTypes[i - 1].Color;
		Color topColor = ascendingHeightTypes[i].Color;

		return new BlendingRegion(bottomBorder, topBorder, bottomColor, topColor);
	}

	private BlendingRegion GetBlendingRegion(float pixelHeight, List<BlendingRegion> regions)
	{
		BlendingRegion region = null;

		foreach (BlendingRegion blendingRegion in regions)
		{
			if (blendingRegion.IsWithin(pixelHeight))
			{
				region = blendingRegion;
				break;
			}
		}

		return region;
	}
}
