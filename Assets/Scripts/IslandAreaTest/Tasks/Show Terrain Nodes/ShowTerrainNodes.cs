using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTerrainNodes : SingleTask
{
	// Inputs
	private float previewNodeRadius;
	private Transform previewParent;
	private Func<List<TerrainNode>> getTerrainNodes;

	// Inputs from previous task
	private List<TerrainNode> terrainNodes;

	// Output
	private List<TerrainNodePreview> previews;

	/// <summary>
	/// Initializes the task's parameters.
	/// </summary>
	/// <param name="getTerrainNodes">Delegate function that collects the result from the previous task.</param>
	public ShowTerrainNodes(float previewNodeRadius, Transform previewParent, Func<List<TerrainNode>> getTerrainNodes)
	{
		Name = "Generate Terrain Nodes Previews";
		this.previewNodeRadius = previewNodeRadius;
		this.previewParent = previewParent;
		this.getTerrainNodes = getTerrainNodes;

		previews = new List<TerrainNodePreview>();
	}

	protected override void ExecuteStep()
	{
		int currentNodeIndex = TotalSteps - RemainingSteps;
		TerrainNode currentNode = terrainNodes[currentNodeIndex];
		TerrainNodePreview newPreview = new TerrainNodePreview(currentNode, previewNodeRadius, previewParent);
		previews.Add(newPreview);
	}

	protected override void GetInputFromPreviousStep()
	{
		this.terrainNodes = getTerrainNodes();
		TotalSteps = terrainNodes.Count;
		RemainingSteps = TotalSteps;
	}
}