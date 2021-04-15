using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.VersionControl;
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

	private Func<List<Color[]>> getMultipliedGradientsAndNoises;
	private List<Color[]> multipliedGradientsAndNoises;

	// Outputs
	private Color[] texture;

	// Internal
	private List<TerrainType> orderedTerrainTypes; // by starting height

	private List<BlendingRegion> blendingRegions;  // where two terrain types meet (blending based on height)
	private List<BlendingRegion> dominatedBlendingRegions;

	public GenerateIslandAreaTexture(TerrainNodesParams terrainNodesParams, Func<List<TerrainNode>> getTerrainNodes, Func<Color[]> getHeightmap, Func<List<Color[]>> getMultipliedGradientsAndNoises)
	{
		Name = "Generate Island Area Texture";

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
		heightmap = getHeightmap();
		multipliedGradientsAndNoises = getMultipliedGradientsAndNoises();

		orderedTerrainTypes = OrderTerrainTypes(terrainNodesParams.TerrainTypes);
		blendingRegions = CreateBlendingRegions(orderedTerrainTypes, terrainNodesParams.BlendingDistance);

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

	private Color GetPixelColor(float pixelHeight, int pixelIndex)
	{
		List<TerrainNode> dominantNodesInRange = DominantNodesInRange(pixelIndex);
		List<TerrainType> dominatedTerrainTypes = DominateTerrainTypes(dominantNodesInRange, orderedTerrainTypes);

		dominatedBlendingRegions = CreateBlendingRegions(dominatedTerrainTypes, terrainNodesParams.BlendingDistance);

		BlendingRegion blendingRegion = GetBlendingRegion(pixelHeight, blendingRegions);
		BlendingRegion dominantBlendingRegion = GetBlendingRegion(pixelHeight, dominatedBlendingRegions);

		Color normalColor = GetNormalColor(blendingRegion, pixelHeight);
		Color dominantColor = GetDominantColor(dominantBlendingRegion, pixelHeight, dominatedTerrainTypes);

		if (dominantNodesInRange.Count > 0)
		{
			return BlendDominantColor(pixelIndex, dominantNodesInRange, normalColor, dominantColor);
		}

		return normalColor;
	}

	private Color GetNormalColor(BlendingRegion blendingRegion, float pixelHeight)
	{
		if (blendingRegion != null)
		{
			return blendingRegion.GetColor(pixelHeight);
		}

		return GetHeightColor(pixelHeight, orderedTerrainTypes);
	}

	private Color GetDominantColor(BlendingRegion dominantBlendingRegion, float pixelHeight, List<TerrainType> dominatedTerrainTypes)
	{
		if (dominantBlendingRegion != null)
		{
			return dominantBlendingRegion.GetColor(pixelHeight);
		}

		return GetHeightColor(pixelHeight, dominatedTerrainTypes);
	}

	/// <summary>
	/// Creates a list of terrain types with shifted starting height. Dominant types expand to types below them.
	/// </summary>
	private List<TerrainType> DominateTerrainTypes(List<TerrainNode> dominantNodesInRange, List<TerrainType> ascendingHeightTypes)
	{
		List<TerrainType> dominatedTypes = new List<TerrainType>();
		TerrainType typeToAdd;

		for (int i = 0; i < ascendingHeightTypes.Count; i++)
		{
			TerrainType currentType = ascendingHeightTypes[i];
			int lastIndex = ascendingHeightTypes.Count - 1;
			typeToAdd = currentType;

			if (i != lastIndex)
			{
				TerrainType typeAbove = ascendingHeightTypes[i + 1];
				typeToAdd = DominateType(dominantNodesInRange, currentType, typeAbove);
			}

			dominatedTypes.Add(typeToAdd);
		}

		return dominatedTypes;
	}

	/// <summary>
	/// If the above type is dominant, its starting height is lowered to the type below. So it takes the below types place.
	/// </summary>
	private TerrainType DominateType(List<TerrainNode> dominantNodesInRange, TerrainType typeBelow, TerrainType typeAbove)
	{
		if (TypeInNodes(dominantNodesInRange, typeAbove))
		{
			// The dominant type's starting height is lowered to the dominated type.
			TerrainType dominantType = typeAbove.DeepCopy();
			dominantType.StartingHeight = typeBelow.StartingHeight;
			return dominantType;
		}
		else
		{
			return typeBelow;
		}
	}

	/// <summary>
	/// Returns true if the list of terrain nodes contains a node of the given terrain type.
	/// </summary>
	private bool TypeInNodes(List<TerrainNode> nodes, TerrainType type)
	{
		foreach (TerrainNode node in nodes)
		{
			if (node.Type == type) return true;
		}

		return false;
	}

	private List<TerrainNode> DominantNodesInRange(int pixelIndex)
	{
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

	/// <summary>
	/// Blends dominant and normal color depending on the height maps of the individual dominant nodes.
	/// </summary>
	private Color BlendDominantColor(int pixelIndex, List<TerrainNode> dominantNodes, Color normalColor, Color dominantColor)
	{
		float dominantHeightMapHeight = 0f;
		float blendingEdge = terrainNodesParams.BlendingDistance;

		for (int i = 0; i < dominantNodes.Count; i++)
		{
			int nodeIndex = terrainNodes.IndexOf(dominantNodes[i]);
			dominantHeightMapHeight += multipliedGradientsAndNoises[nodeIndex][pixelIndex].r;
		}

		if (dominantHeightMapHeight > blendingEdge) return dominantColor;

		float blendInterpolator = dominantHeightMapHeight / blendingEdge;
		return Color.Lerp(normalColor, dominantColor, blendInterpolator);
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

	private List<BlendingRegion> CreateBlendingRegions(List<TerrainType> ascendingHeightTypes, float blendingDistance)
	{
		List<BlendingRegion> foundRegions = new List<BlendingRegion>();

		if (ascendingHeightTypes.Count > 1)
		{
			for (int i = 1; i < ascendingHeightTypes.Count; i++)
			{
				BlendingRegion newRegion = CreateBlendingRegion(ascendingHeightTypes, blendingDistance, i);
				foundRegions.Add(newRegion);
			}
		}

		return foundRegions;
	}

	private BlendingRegion CreateBlendingRegion(List<TerrainType> ascendingHeightTypes, float blendingDistance, int i)
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

	/// <summary>
	/// Returns the blending region to which the pixel belongs to or null.
	/// </summary>
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
