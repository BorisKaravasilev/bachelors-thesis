using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IslandAreaTest;

/// <summary>
/// Generates a terrain texture from terrain nodes, their heightmaps and a list of terrain types.
/// </summary>
class TerrainTextureGenerator
{
	private List<TerrainNode> terrainNodes;
	private List<Color[]> terrainNodesHeightMaps;

	private List<TerrainType> orderedTerrainTypes; // by starting height
	private float blendingHeight;

	public TerrainTextureGenerator(List<TerrainNode> terrainNodes, List<Color[]> terrainNodesHeightMaps, List<TerrainType> terrainTypes, float blendingHeight)
	{
		this.terrainNodes = terrainNodes;
		this.terrainNodesHeightMaps = terrainNodesHeightMaps;
		this.blendingHeight = blendingHeight;

		orderedTerrainTypes = OrderTerrainTypes(terrainTypes);
	}

	/// <summary>
	/// Returns the blend of terrain types at the given pixel.
	/// </summary>
	public TerrainBlend GetPixelTerrainBlend(float pixelHeight, int pixelIndex)
	{
		TerrainBlendList terrainBlendList = GetDominantNodesBlend(pixelHeight, pixelIndex);

		TerrainBlend nonDominatedTerrainBlend = GetTerrainBlendByHeight(pixelHeight, orderedTerrainTypes);
		terrainBlendList.Add(nonDominatedTerrainBlend);

		return terrainBlendList.Blend();
	}

	/// <summary>
	/// Blends terrain type blends enforced by dominant nodes in range of pixel.
	/// </summary>
	private TerrainBlendList GetDominantNodesBlend(float pixelHeight, int pixelIndex)
	{
		TerrainBlendList blendList = new TerrainBlendList();
		List<TerrainNode> dominantNodesInRange = DominantNodesInRange(pixelIndex);

		foreach (TerrainNode currentNode in dominantNodesInRange)
		{
			List<TerrainType> typesDominatedByNode = DominateTerrainTypes(currentNode, orderedTerrainTypes);

			TerrainBlend terrainBlend = GetTerrainBlendByHeight(pixelHeight, typesDominatedByNode);
			terrainBlend.Amount = GetNodeStrength(currentNode, pixelIndex);

			blendList.Add(terrainBlend);
		}

		return blendList;
	}

	/// <summary>
	/// Returns terrain nodes strength based on its heightmap and blending height.
	/// </summary>
	/// <returns></returns>
	private float GetNodeStrength(TerrainNode node, int pixelIndex)
	{
		int nodeIndex = terrainNodes.IndexOf(node);
		float pixelHeight = terrainNodesHeightMaps[nodeIndex][pixelIndex].r;

		if (blendingHeight > 0f)
		{
			return Mathf.Clamp01(pixelHeight / blendingHeight);
		}

		return 1f;
	}

	/// <summary>
	/// Order terrain types by starting height.
	/// </summary>
	private List<TerrainType> OrderTerrainTypes(List<TerrainType> terrainTypes)
	{
		List<TerrainType> ascendingTerrainType = terrainTypes.OrderBy(terrainType => terrainType.StartingHeight).ToList();
		return ascendingTerrainType;
	}

	/// <summary>
	/// Gets a blend of terrain types by height from an ordered list of terrain types by starting height.
	/// </summary>
	private TerrainBlend GetTerrainBlendByHeight(float pixelHeight, List<TerrainType> ascendingTypes)
	{
		TerrainType currentType = FindTerrainTypeByHeight(pixelHeight, ascendingTypes);
		int currentTypeIndex = ascendingTypes.IndexOf(currentType);

		bool isLastType = currentTypeIndex == ascendingTypes.Count - 1;
		TerrainType below = currentTypeIndex == 0 ? null : ascendingTypes[currentTypeIndex - 1];
		TerrainType above = isLastType ? null : ascendingTypes[currentTypeIndex + 1];

		TerrainBlend terrainBlend = GetTerrainTypeBlend(below, currentType, above, pixelHeight);

		return terrainBlend;
	}

	/// <summary>
	/// Returns a terrain type at the specified height.
	/// </summary>
	private TerrainType FindTerrainTypeByHeight(float height, List<TerrainType> ascendingTypes)
	{
		for (int i = 0; i < ascendingTypes.Count; i++)
		{
			TerrainType currentType = ascendingTypes[i];

			if (i == ascendingTypes.Count - 1) // last type
			{
				return currentType;
			}

			TerrainType aboveType = ascendingTypes[i + 1];

			if (height >= currentType.StartingHeight && height < aboveType.StartingHeight)
			{
				return currentType;
			}
		}

		return null;
	}

	/// <summary>
	/// Returns a blend of terrain types. Given terrain types must be in the respective order by starting height.
	/// </summary>
	private TerrainBlend GetTerrainTypeBlend(TerrainType below, TerrainType current, TerrainType above, float height)
	{
		TerrainBlend terrainBlend = new TerrainBlend();
		float clampedBlendHeight = ClampTerrainBlendHeight(current, above, blendingHeight);

		if (IsHeightInTopBlendRegion(above, height, clampedBlendHeight))
		{
			terrainBlend = GetTopTerrainBlend(current, above, height, clampedBlendHeight); // Top blending
		}
		else if (IsHeightInBottomBlendRegion(below, current, height, clampedBlendHeight))
		{
			terrainBlend = GetBottomTerrainBlend(current, below, height, clampedBlendHeight); // Bottom blending
		}
		else
		{
			terrainBlend.AddTerrainType(current, 1f); // No blending
		}

		return terrainBlend;
	}

	/// <summary>
	/// Clamps the terrain blend height to the height of the current terrain type.
	/// </summary>
	private float ClampTerrainBlendHeight(TerrainType current, TerrainType above, float blendHeight)
	{
		float currentTypeCeiling = above?.StartingHeight ?? 1f;
		return Mathf.Clamp(blendHeight, 0f, currentTypeCeiling - current.StartingHeight);
	}

	/// <summary>
	/// Returns true if the height is in the top part of a terrain type where it has to be blended with the neighbor above.
	/// </summary>
	private bool IsHeightInTopBlendRegion(TerrainType above, float height, float blendHeight)
	{
		return above != null && height > (above.StartingHeight - blendHeight / 2);
	}

	/// <summary>
	/// Returns true if the height is in the bottom part of a terrain type where it has to be blended with the neighbor below.
	/// </summary>
	private bool IsHeightInBottomBlendRegion(TerrainType below, TerrainType current, float height, float blendHeight)
	{
		return below != null && height < (current.StartingHeight + blendHeight / 2);
	}

	/// <summary>
	/// Blends terrain type with the neighbor type above it.
	/// </summary>
	private TerrainBlend GetTopTerrainBlend(TerrainType current, TerrainType above, float height, float blendHeight)
	{
		TerrainBlend terrainBlend = new TerrainBlend();

		float topBlendStartingHeight = above.StartingHeight - blendHeight / 2;

		float aboveTypeAmount = (height - topBlendStartingHeight) / (blendHeight); // 0.0 to 0.5
		float currentTypeAmount = 1 - aboveTypeAmount;								  // 1.0 to 0.5

		terrainBlend.AddTerrainType(above, aboveTypeAmount);
		terrainBlend.AddTerrainType(current, currentTypeAmount);

		return terrainBlend;
	}

	/// <summary>
	/// Blends terrain type with the neighbor type below it.
	/// </summary>
	private TerrainBlend GetBottomTerrainBlend(TerrainType current, TerrainType below, float height, float blendHeight)
	{
		TerrainBlend terrainBlend = new TerrainBlend();

		float blendInterpolation = (height - current.StartingHeight) / (blendHeight / 2);
		float currentTypeAmount = 0.5f + blendInterpolation / 2; // 0.5 to 1.0
		float belowTypeAmount = 0.5f - blendInterpolation / 2;   // 0.5 to 0.0

		terrainBlend.AddTerrainType(current, currentTypeAmount);
		terrainBlend.AddTerrainType(below, belowTypeAmount);

		return terrainBlend;
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
		float currentPixelIntensity = terrainNodesHeightMaps[nodeIndex][pixelIndex].r;

		if (currentPixelIntensity > 0f) return true;
		return false;
	}

	/// <summary>
	/// Creates a list of terrain types with shifted starting height. Dominant types expand to types below them.
	/// </summary>
	private List<TerrainType> DominateTerrainTypes(TerrainNode dominantNode, List<TerrainType> ascendingHeightTypes)
	{
		List<TerrainType> dominatedTypes = new List<TerrainType>();

		for (int i = 0; i < ascendingHeightTypes.Count; i++)
		{
			TerrainType currentType = ascendingHeightTypes[i];
			int lastIndex = ascendingHeightTypes.Count - 1;
			TerrainType typeToAdd = currentType;

			if (i != lastIndex)
			{
				TerrainType typeAbove = ascendingHeightTypes[i + 1];
				typeToAdd = DominateType(dominantNode, currentType, typeAbove);
			}

			dominatedTypes.Add(typeToAdd);
		}

		return dominatedTypes;
	}

	/// <summary>
	/// If the above type is dominant, its starting height is lowered to the type below. So it takes the below types place.
	/// </summary>
	private TerrainType DominateType(TerrainNode dominantNode, TerrainType typeBelow, TerrainType typeAbove)
	{
		if (dominantNode.Type == typeAbove)
		{
			// The dominant type's starting height is lowered to the dominated type.
			TerrainType dominantType = typeAbove.DeepCopy();
			dominantType.StartingHeight = typeBelow.StartingHeight;
			return dominantType;
		}

		return typeBelow;
	}
}