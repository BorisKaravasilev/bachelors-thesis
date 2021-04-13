using System;
using System.Collections.Generic;
using UnityEngine;

public class ShowTerrainNodes : SingleTask
{
	// Inputs
	private Material material;
	private float nodePreviewRadius;
	private Transform previewParent;
	private Func<List<TerrainNode>> getTerrainNodes;

	// Inputs from previous task
	private List<TerrainNode> terrainNodes;

	// Output
	private List<PreviewObject> previews;

	/// <summary>
	/// Initializes the task's parameters.
	/// </summary>
	/// <param name="getTerrainNodes">Delegate function that collects the result from the previous task.</param>
	public ShowTerrainNodes(Material material, float nodePreviewRadius, Transform previewParent, Func<List<TerrainNode>> getTerrainNodes)
	{
		Name = "Show Terrain Nodes Previews";

		this.material = material;
		this.nodePreviewRadius = nodePreviewRadius;
		this.previewParent = previewParent;
		this.getTerrainNodes = getTerrainNodes;

		previews = new List<PreviewObject>();
	}

	protected override void ExecuteStep()
	{
		int currentNodeIndex = TotalSteps - RemainingSteps;
		TerrainNode currentNode = terrainNodes[currentNodeIndex];
		PreviewObject newPreview = CreateNodePreview(currentNode);

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

	private PreviewObject CreateNodePreview(TerrainNode node)
	{
		PreviewObject newPreview = new PreviewObject(PrimitiveType.Sphere, material, previewParent);

		newPreview.SetName("Terrain Node Preview");
		newPreview.SetColor(node.Type.Color);
		newPreview.SetLocalPosition(node.Position);

		float diameter = nodePreviewRadius * 2f;
		Vector3 dimensions = new Vector3(diameter, diameter, diameter);
		newPreview.SetDimensions(dimensions);

		return newPreview;
	}
}