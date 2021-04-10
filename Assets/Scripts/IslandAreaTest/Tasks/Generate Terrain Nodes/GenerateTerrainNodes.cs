using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrainNodes : SingleTask
{
	// Inputs
	private Vector3 seedPosition;
	private float objectRadius;

	private int minNodes;
	private int maxNodes;
	private float maxDistanceMultiplier;

	private List<TerrainType> terrainTypes;

	// Outputs
	private List<TerrainNode> terrainNodes;

	/// <summary>
	/// Initializes the task's parameters.
	/// </summary>
	public GenerateTerrainNodes(TerrainNodesParams nodesParams, GridObjectParams objectParams, int stepSize = 1)
	{
		Name = "Generate Terrain Nodes";
		StepSize = stepSize;

		seedPosition = objectParams.Position;
		objectRadius = objectParams.Radius;

		minNodes = nodesParams.MinNodes;
		maxNodes = nodesParams.MaxNodes;
		maxDistanceMultiplier = nodesParams.MaxDistanceMultiplier;

		terrainTypes = nodesParams.TerrainTypes;
		terrainNodes = new List<TerrainNode>();
	}

	public List<TerrainNode> GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return terrainNodes;
	}

	protected override void GetInputFromPreviousStep() { /* Not used */ }

	protected override void SetSteps()
	{
		TotalSteps = RandomFromSeed.Range(seedPosition, minNodes, maxNodes + 1);
		RemainingSteps = TotalSteps;
	}

	protected override void ExecuteStep()
	{
		float maxNodeDistance = objectRadius * maxDistanceMultiplier; // From center
		Vector3 randPosition = RandomFromSeed.RandomPointInRadius(seedPosition, Vector3.zero, maxNodeDistance);
		int randTypeIndex = RandomFromSeed.Range(seedPosition, 0, terrainTypes.Count);
		float maxRadius = objectRadius - Vector3.Distance(randPosition, Vector3.zero);

		TerrainNode randomNode = new TerrainNode(randPosition, terrainTypes[randTypeIndex], maxRadius);
		terrainNodes.Add(randomNode);
	}
}