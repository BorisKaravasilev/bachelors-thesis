using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrainNodesPreviews : SingleTask
{
	private Action<List<TerrainNodePreview>> setTerrainNodePreviews;
	private List<TerrainNode> terrainNodes;
	private float previewNodeRadius;
	private Transform previewParent;

	// Output
	private List<TerrainNodePreview> previews;

	/// <summary>
	/// Initializes the task's parameters and assigns the action to be executed after its execution is finished.
	/// </summary>
	public GenerateTerrainNodesPreviews(float previewNodeRadius, Transform previewParent, Action<List<TerrainNodePreview>> setTerrainNodePreviews)
	{
		Name = "Generate Terrain Nodes Previews";
		this.previewNodeRadius = previewNodeRadius;
		this.previewParent = previewParent;
		this.setTerrainNodePreviews = setTerrainNodePreviews;

		previews = new List<TerrainNodePreview>();
	}

	public void SetTerrainNodes(List<TerrainNode> terrainNodes)
	{
		this.terrainNodes = terrainNodes;
		TotalSteps = terrainNodes.Count;
		RemainingSteps = TotalSteps;
	}

	protected override void ExecuteStep()
	{
		int currentNodeIndex = TotalSteps - RemainingSteps;
		TerrainNode currentNode = terrainNodes[currentNodeIndex];
		TerrainNodePreview newPreview = new TerrainNodePreview(currentNode, previewNodeRadius, previewParent);
		previews.Add(newPreview);
	}

	protected override void PassTaskResults()
	{
		setTerrainNodePreviews(previews);
	}
}