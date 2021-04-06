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

	/// <summary>
	/// Initializes the task's parameters and assigns the action to be executed after its execution is finished.
	/// </summary>
	public GenerateTerrainNodesPreviews(float previewNodeRadius, Transform previewParent, Action<List<TerrainNodePreview>> setTerrainNodePreviews)
	{
		Name = "Generate Terrain Nodes Previews";
		this.previewNodeRadius = previewNodeRadius;
		this.previewParent = previewParent;
		this.setTerrainNodePreviews = setTerrainNodePreviews;
	}

	public void SetTerrainNodes(List<TerrainNode> terrainNodes)
	{
		this.terrainNodes = terrainNodes;
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
		List<TerrainNodePreview> previews = new List<TerrainNodePreview>();

		foreach (TerrainNode node in terrainNodes)
		{
			TerrainNodePreview newPreview = new TerrainNodePreview(node, previewNodeRadius, previewParent);
			previews.Add(newPreview);
		}

		setTerrainNodePreviews(previews);

		Finished = true;
		Progress = 1f;
		return true;
	}
}