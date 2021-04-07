using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrainNodes : SingleTask
{
	// Inputs
	private GenerateTerrainNodesParams nodesParams;
	private GridObjectParams objectParams;

	// Outputs
	private List<TerrainNode> terrainNodes;

	/// <summary>
	/// Initializes the task's parameters.
	/// </summary>
	public GenerateTerrainNodes(GenerateTerrainNodesParams nodesParams, GridObjectParams objectParams)
	{
		Name = "Generate Terrain Nodes";
		this.nodesParams = nodesParams;
		this.objectParams = objectParams;

		terrainNodes = new List<TerrainNode>();
		TotalSteps = RandomFromSeed.Range(objectParams.Position, nodesParams.MinNodes, nodesParams.MaxNodes + 1);
		RemainingSteps = TotalSteps;
	}

	public List<TerrainNode> GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return terrainNodes;
	}

	protected override void GetInputFromPreviousStep() { /* Not used */ }

	protected override void ExecuteStep()
	{
		Vector3 randPosition = RandomFromSeed.RandomPointInRadius(objectParams.Position, Vector3.zero, objectParams.Radius);
		int randTypeIndex = RandomFromSeed.Range(objectParams.Position, 0, nodesParams.TerrainTypes.Count);

		TerrainNode randomNode = new TerrainNode(randPosition, nodesParams.TerrainTypes[randTypeIndex]);
		terrainNodes.Add(randomNode);
	}
}