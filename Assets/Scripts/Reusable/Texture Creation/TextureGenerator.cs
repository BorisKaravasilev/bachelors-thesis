using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Blend of multiple terrain types.
/// </summary>
public class TerrainBlend
{
	private float availableCapacity; // 0 to 1
	private List<TerrainTypeFraction> terrainFractions;

	public TerrainBlend()
	{
		availableCapacity = 1f;
		terrainFractions = new List<TerrainTypeFraction>();
	}

	public void AddTerrainType(TerrainType type, float amount)
	{
		float addedAmount = amount;

		if (addedAmount > availableCapacity) addedAmount = availableCapacity;
		TerrainTypeFraction typeFraction = new TerrainTypeFraction(type, addedAmount);
		terrainFractions.Add(typeFraction);
		availableCapacity -= addedAmount;
	}

	public Color GetColor()
	{
		List<Color> blendedColors = new List<Color>();

		for (int i = 0; i < terrainFractions.Count; i++)
		{
			Color colorToBlend = terrainFractions[i].Type.Color * terrainFractions[i].Amount;
			blendedColors.Add(colorToBlend);
		}

		Color blendedColor = Color.black;

		blendedColors.ForEach(colorToBlend => blendedColor += colorToBlend);
		return blendedColor;
	}
}

/// <summary>
/// Terrain type with a specified amount used for blending.
/// </summary>
public class TerrainTypeFraction
{
	public TerrainType Type { get; private set; }
	public float Amount { get; private set; } // 0 to 1

	public TerrainTypeFraction(TerrainType type, float amount)
	{
		this.Type = type;
		this.Amount = amount;
	}
}

class TextureGenerator
{
	private List<TerrainNode> terrainNodes;
	private List<Color[]> terrainNodesHeightMaps;

	private List<TerrainType> orderedTerrainTypes; // by starting height
	private float blendingHeight;

	public TextureGenerator(List<TerrainNode> terrainNodes, List<Color[]> terrainNodesHeightMaps, List<TerrainType> terrainTypes, float blendingHeight)
	{
		this.terrainNodes = terrainNodes;
		this.terrainNodesHeightMaps = terrainNodesHeightMaps;
		this.blendingHeight = blendingHeight;

		orderedTerrainTypes = OrderTerrainTypes(terrainTypes);
	}

	public Color GetPixelColor(float pixelHeight, int pixelIndex)
	{
		TerrainBlend terrainBlendByHeight = GetTerrainBlendByHeight(pixelHeight, orderedTerrainTypes);
		return terrainBlendByHeight.GetColor();
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

	//private Color GetPixelColorBlend(float pixelHeight, int pixelIndex)
	//{
	//	List<TerrainNode> dominantNodesInRange = DominantNodesInRange(pixelIndex);
	//	return new Color();
	//}


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

		if (IsHeightInTopBlendRegion(above, height))
		{
			terrainBlend = GetTopTerrainBlend(current, above, height); // Top blending
		}
		else if (IsHeightInBottomBlendRegion(below, current, height))
		{
			terrainBlend = GetBottomTerrainBlend(current, below, height); // Bottom blending
		}
		else
		{
			terrainBlend.AddTerrainType(current, 1f); // No blending
		}

		return terrainBlend;
	}

	/// <summary>
	/// Returns true if the height is in the top part of a terrain type where it has to be blended with the neighbor above.
	/// </summary>
	private bool IsHeightInTopBlendRegion(TerrainType above, float height)
	{
		return above != null && height > (above.StartingHeight - blendingHeight / 2);
	}

	/// <summary>
	/// Returns true if the height is in the bottom part of a terrain type where it has to be blended with the neighbor below.
	/// </summary>
	private bool IsHeightInBottomBlendRegion(TerrainType below, TerrainType current, float height)
	{
		return below != null && height < (current.StartingHeight + blendingHeight / 2);
	}

	/// <summary>
	/// Blends terrain type with the neighbor type above it.
	/// </summary>
	private TerrainBlend GetTopTerrainBlend(TerrainType current, TerrainType above, float height)
	{
		TerrainBlend terrainBlend = new TerrainBlend();

		float topBlendStartingHeight = above.StartingHeight - blendingHeight / 2;

		float aboveTypeAmount = (height - topBlendStartingHeight) / (blendingHeight); // 0.0 to 0.5
		float currentTypeAmount = 1 - aboveTypeAmount;								  // 1.0 to 0.5

		terrainBlend.AddTerrainType(above, aboveTypeAmount);
		terrainBlend.AddTerrainType(current, currentTypeAmount);

		return terrainBlend;
	}

	/// <summary>
	/// Blends terrain type with the neighbor type below it.
	/// </summary>
	private TerrainBlend GetBottomTerrainBlend(TerrainType current, TerrainType below, float height)
	{
		TerrainBlend terrainBlend = new TerrainBlend();

		float blendInterpolation = (height - current.StartingHeight) / (blendingHeight / 2);
		float currentTypeAmount = 0.5f + blendInterpolation / 2; // 0.5 to 1.0
		float belowTypeAmount = 0.5f - blendInterpolation / 2;   // 0.5 to 0.0

		terrainBlend.AddTerrainType(current, currentTypeAmount);
		terrainBlend.AddTerrainType(below, belowTypeAmount);

		return terrainBlend;
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