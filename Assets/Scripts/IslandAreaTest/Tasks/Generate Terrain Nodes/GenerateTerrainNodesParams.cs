using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GenerateTerrainNodesParams", fileName = "New Generate Terrain Nodes Params")]
public class GenerateTerrainNodesParams : ScriptableObject
{
	public List<TerrainType> TerrainTypes;
	public int MinNodes;
	public int MaxNodes;
}