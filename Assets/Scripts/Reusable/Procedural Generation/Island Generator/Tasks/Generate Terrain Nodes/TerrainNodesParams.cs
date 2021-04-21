using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	[CreateAssetMenu]
	public class TerrainNodesParams : ScriptableObject
	{
		public List<TerrainType> TerrainTypes;
		public int MinNodes;
		public int MaxNodes;
		[Range(0f, 1f)] public float MaxDistanceMultiplier;
		[Range(0f, 1f)] public float BlendingHeight;

		public TerrainNodesParams()
		{
			MinNodes = 1;
			MaxNodes = 3;
			MaxDistanceMultiplier = 1;
			BlendingHeight = 0.05f;
		}

		public TerrainNodesParams(List<TerrainType> terrainTypes, int minNodes, int maxNodes,
			float maxDistanceMultiplier, float blendingHeight)
		{
			TerrainTypes = terrainTypes;
			MinNodes = minNodes;
			MaxNodes = maxNodes;
			MaxDistanceMultiplier = maxDistanceMultiplier;
			BlendingHeight = blendingHeight;
		}
	}
}