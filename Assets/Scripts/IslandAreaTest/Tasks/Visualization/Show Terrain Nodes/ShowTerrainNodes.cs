using System;
using System.Collections.Generic;
using UnityEngine;

public class ShowTerrainNodes : SingleTask
{
	// Inputs
	private float nodePreviewRadius;
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
	public ShowTerrainNodes(float nodePreviewRadius, Transform previewParent, Func<List<TerrainNode>> getTerrainNodes)
	{
		Name = "Show Terrain Nodes Previews";

		this.nodePreviewRadius = nodePreviewRadius;
		this.previewParent = previewParent;
		this.getTerrainNodes = getTerrainNodes;

		previews = new List<TerrainNodePreview>();
	}

	protected override void ExecuteStep()
	{
		int currentNodeIndex = TotalSteps - RemainingSteps;
		TerrainNode currentNode = terrainNodes[currentNodeIndex];
		TerrainNodePreview newPreview = new TerrainNodePreview(currentNode, nodePreviewRadius, previewParent);
		newPreview.SetName("Terrain Node Preview");
		previews.Add(newPreview);
	}

	protected override void GetInputFromPreviousStep()
	{
		this.terrainNodes = getTerrainNodes();
	}

	protected override void SetSteps()
	{
		TotalSteps = terrainNodes.Count;
		RemainingSteps = TotalSteps;
	}
}