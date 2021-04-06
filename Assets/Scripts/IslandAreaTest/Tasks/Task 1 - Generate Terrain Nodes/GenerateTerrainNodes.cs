using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrainNodes : SingleTask
{
	// Inputs
	private GenerateTerrainNodesParams nodesParams;
	private GridObjectParams objectParams;
	private Action<List<TerrainNode>> setTerrainNodes;

	// Outputs
	private List<TerrainNode> terrainNodes;

	/// <summary>
	/// Initializes the task's parameters and assigns the action to be executed after its execution is finished.
	/// </summary>
	public GenerateTerrainNodes(GenerateTerrainNodesParams nodesParams, GridObjectParams objectParams, Action<List<TerrainNode>> setTerrainNodes)
	{
		Name = "Generate Terrain Nodes";
		this.nodesParams = nodesParams;
		this.objectParams = objectParams;
		this.setTerrainNodes = setTerrainNodes;

		terrainNodes = new List<TerrainNode>();
		TotalSteps = RandomFromSeed.Range(objectParams.Position, nodesParams.MinNodes, nodesParams.MaxNodes + 1);
		RemainingSteps = TotalSteps;
	}

	protected override void ExecuteStep()
	{
		Vector3 randPosition = RandomFromSeed.RandomPointInRadius(objectParams.Position, Vector3.zero, objectParams.Radius);
		int randTypeIndex = RandomFromSeed.Range(objectParams.Position, 0, nodesParams.TerrainTypes.Count);

		TerrainNode randomNode = new TerrainNode(randPosition, nodesParams.TerrainTypes[randTypeIndex]);
		terrainNodes.Add(randomNode);
	}

	protected override void PassTaskResults()
	{
		setTerrainNodes(terrainNodes);
	}

	/// <summary>
	/// Executes a part of the task and returns true if finished.
	/// </summary>
	//public override int ExecuteStepSize()
	//{
	//	if (nodesToGenerate > 0)
	//	{
	//		Vector3 randPosition = RandomFromSeed.RandomPointInRadius(objectParams.Position, Vector3.zero, objectParams.Radius);
	//		int randTypeIndex = RandomFromSeed.Range(objectParams.Position, 0, nodesParams.TerrainTypes.Count);

	//		TerrainNode randomNode = new TerrainNode(randPosition, nodesParams.TerrainTypes[randTypeIndex]);
	//		terrainNodes.Add(randomNode);

	//		nodesToGenerate--;
	//	}

	//	if (nodesToGenerate == 0)
	//	{
	//		setTerrainNodes(terrainNodes);
	//		return true;
	//	}
	//	else
	//	{
	//		return false;
	//	}
	//}
}