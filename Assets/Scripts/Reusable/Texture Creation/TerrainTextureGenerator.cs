using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainTextureGenerator
{
	private List<TerrainNode> terrainNodes;
	private List<Color[]> terrainNodesHeightMaps;

	private List<TerrainType> orderedTerrainTypes; // by starting height
	private float blendingHeight;
	private List<BlendingRegion> blendingRegions;

	public TerrainTextureGenerator(List<TerrainNode> terrainNodes, List<Color[]> terrainNodesHeightMaps, List<TerrainType> terrainTypes, float blendingHeight)
	{
		this.terrainNodes = terrainNodes;
		this.terrainNodesHeightMaps = terrainNodesHeightMaps;
		this.blendingHeight = blendingHeight;

		orderedTerrainTypes = OrderTerrainTypes(terrainTypes);
		blendingRegions = CreateBlendingRegions(orderedTerrainTypes, blendingHeight);
	}

	public Color GetPixelColor(float pixelHeight, int pixelIndex)
	{
		List<TerrainNode> dominantNodesInRange = DominantNodesInRange(pixelIndex);
		List<TerrainType> dominatedTerrainTypes = DominateTerrainTypes(dominantNodesInRange, orderedTerrainTypes);

		List<BlendingRegion> dominatedBlendingRegions = CreateBlendingRegions(dominatedTerrainTypes, blendingHeight);

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

	/// <summary>
	/// Determines the pixel color by height or blending region interpolation.
	/// </summary>
	private Color GetNormalColor(BlendingRegion blendingRegion, float pixelHeight)
	{
		if (blendingRegion != null)
		{
			return blendingRegion.GetColor(pixelHeight);
		}

		return GetColorByHeight(pixelHeight, orderedTerrainTypes);
	}

	/// <summary>
	/// Determines the pixel color by height or blending region interpolation.
	/// </summary>
	private Color GetDominantColor(BlendingRegion dominantBlendingRegion, float pixelHeight, List<TerrainType> dominatedTerrainTypes)
	{
		if (dominantBlendingRegion != null)
		{
			return dominantBlendingRegion.GetColor(pixelHeight);
		}

		return GetColorByHeight(pixelHeight, dominatedTerrainTypes);
	}

	/// <summary>
	/// Returns the color of the closest terrain type with a starting height below the given height.
	/// </summary>
	private Color GetColorByHeight(float pixelHeight, List<TerrainType> ascendingHeightTypes)
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

	/// <summary>
	/// Blends dominant and normal color depending on the height maps of the individual dominant nodes.
	/// </summary>
	private Color BlendDominantColor(int pixelIndex, List<TerrainNode> dominantNodes, Color normalColor, Color dominantColor)
	{
		float dominantHeightMapHeight = 0f;
		float blendingEdge = blendingHeight;

		for (int i = 0; i < dominantNodes.Count; i++)
		{
			int nodeIndex = terrainNodes.IndexOf(dominantNodes[i]);
			dominantHeightMapHeight += terrainNodesHeightMaps[nodeIndex][pixelIndex].r;
		}

		if (dominantHeightMapHeight > blendingEdge) return dominantColor;

		float blendInterpolator = dominantHeightMapHeight / blendingEdge;
		return Color.Lerp(normalColor, dominantColor, blendInterpolator);
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
	/// Creates blending regions between ordered terrain types.
	/// </summary>
	private List<BlendingRegion> CreateBlendingRegions(List<TerrainType> ascendingHeightTypes, float blendingDistance)
	{
		List<BlendingRegion> foundRegions = new List<BlendingRegion>();
		int typesCount = ascendingHeightTypes.Count;

		if (typesCount > 1)
		{
			for (int i = 1; i < typesCount; i++)
			{
				TerrainType previous = ascendingHeightTypes[i - 1];
				TerrainType current = ascendingHeightTypes[i];
				TerrainType next = i == typesCount - 1 ? null : ascendingHeightTypes[i + 1];

				BlendingRegion newRegion = BlendingRegionBetweenTypes(previous, current, next);
				foundRegions.Add(newRegion);
			}
		}

		return foundRegions;
	}

	/// <summary>
	/// Creates a blending region between
	/// </summary>
	private BlendingRegion BlendingRegionBetweenTypes(TerrainType previous, TerrainType current, TerrainType next = null)
	{
		float minHeight = previous.StartingHeight;
		float maxHeight = next?.StartingHeight ?? 1f; // TODO: Do I need the "next"?

		float bottomBorder = Mathf.Clamp(current.StartingHeight - blendingHeight, minHeight, maxHeight);
		float topBorder = Mathf.Clamp(current.StartingHeight + blendingHeight, minHeight, maxHeight);

		Color bottomColor = previous.Color;
		Color topColor = current.Color;

		return new BlendingRegion(bottomBorder, topBorder, bottomColor, topColor);
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

	/// <summary>
	/// Returns a list of dominant terrain nodes that the given pixel is in range of.
	/// </summary>
	private List<TerrainNode> DominantNodesInRange(int pixelIndex)
	{
		List<TerrainNode> dominantNodes = new List<TerrainNode>();

		for (int nodeIndex = 0; nodeIndex < terrainNodes.Count; nodeIndex++)
		{
			TerrainNode currentNode = terrainNodes[nodeIndex];

			bool inRange = NodeInRange(nodeIndex, pixelIndex);
			bool isDominant = currentNode.IsDominant;

			if (inRange && isDominant) dominantNodes.Add(currentNode);
		}

		return dominantNodes;
	}

	/// <summary>
	/// Returns true if pixel's intensity is higher than zero in the height map of the specified node.
	/// </summary>
	private bool NodeInRange(int nodeIndex, int pixelIndex)
	{
		TerrainNode checkedNode = terrainNodes[nodeIndex];
		float currentPixelIntensity = terrainNodesHeightMaps[nodeIndex][pixelIndex].r;

		if (currentPixelIntensity > 0f) return true;
		return false;
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