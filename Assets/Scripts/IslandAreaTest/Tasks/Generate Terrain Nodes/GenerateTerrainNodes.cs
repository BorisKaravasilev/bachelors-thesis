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

	// Internal
	private RandomFromPosition random;
	private int nodesToGenerate;

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

		random = new RandomFromPosition(seedPosition, Name);
		nodesToGenerate = random.Next(minNodes, maxNodes + 1);
	}

	public List<TerrainNode> GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return terrainNodes;
	}

	protected override void GetInputFromPreviousStep() { /* Not used */ }

	protected override void SetSteps()
	{
		TotalSteps = nodesToGenerate;
		RemainingSteps = TotalSteps;
	}

	protected override void ExecuteStep()
	{
		float maxNodeDistance = objectRadius * maxDistanceMultiplier; // From center
		Vector2 node2DPosition = random.Point2DInRadius(Vector2.zero, maxNodeDistance);
		Vector3 nodePosition = new Vector3(node2DPosition.x, 0f, node2DPosition.y);

		int randTypeIndex = random.Next(0, terrainTypes.Count);
		float nodeRadius = objectRadius - Vector3.Distance(nodePosition, Vector3.zero);
		float dominantProbability = terrainTypes[randTypeIndex].DominationProbability;
		bool isDominant = random.NextBool(dominantProbability);

		TerrainNode randomNode = new TerrainNode(nodePosition, terrainTypes[randTypeIndex], nodeRadius, isDominant);
		terrainNodes.Add(randomNode);
	}
}