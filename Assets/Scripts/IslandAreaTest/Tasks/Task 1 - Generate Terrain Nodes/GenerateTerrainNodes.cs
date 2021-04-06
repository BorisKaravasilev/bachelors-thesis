using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrainNodes : SingleTask
{
	private Action<List<TerrainNode>> setTerrainNodes;
	private GenerateTerrainNodesParams nodesParams;
	private GridObjectParams objectParams;

	/// <summary>
	/// Initializes the task's parameters and assigns the action to be executed after its execution is finished.
	/// </summary>
	public GenerateTerrainNodes(GenerateTerrainNodesParams nodesParams, GridObjectParams objectParams, Action<List<TerrainNode>> setTerrainNodes)
	{
		Name = "Generate Terrain Nodes";
		this.nodesParams = nodesParams;
		this.objectParams = objectParams;
		this.setTerrainNodes = setTerrainNodes;
	}

	/// <summary>
	/// Executes the whole task at once.
	/// </summary>
	public override void Execute()
	{
		throw new System.NotImplementedException();
	}

	/// <summary>
	/// Executes a part of the task and returns true if finished.
	/// </summary>
	public override bool ExecuteStep()
	{
		List<TerrainNode> terrainNodes = new List<TerrainNode>();
		int generatedNodesCount = RandomFromSeed.Range(objectParams.Position, nodesParams.MinNodes, nodesParams.MaxNodes + 1);

		for (int i = 0; i < generatedNodesCount; i++)
		{
			Vector3 randPosition = RandomFromSeed.RandomPointInRadius(objectParams.Position, Vector3.zero, objectParams.Radius);
			int randTypeIndex = RandomFromSeed.Range(objectParams.Position, 0, nodesParams.TerrainTypes.Count);

			TerrainNode randomNode = new TerrainNode(randPosition, nodesParams.TerrainTypes[randTypeIndex]);
			terrainNodes.Add(randomNode);
		}

		setTerrainNodes(terrainNodes);

		Finished = true;
		Progress = 1f;
		return true;
	}
}