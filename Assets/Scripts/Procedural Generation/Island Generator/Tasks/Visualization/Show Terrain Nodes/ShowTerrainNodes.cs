using System;
using System.Collections.Generic;
using TaskManagement;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Generates game objects to show basic properties of terrain nodes as position, terrain type color etc.
	/// </summary>
	public class ShowTerrainNodes : DividableTask
	{
		// Inputs
		private Material material;
		private float nodePreviewRadius;
		private Transform previewParent;

		// Inputs from previous tasks
		private Func<List<TerrainNode>> getTerrainNodes;
		private List<TerrainNode> terrainNodes;

		// Outputs
		private List<IHideable> previews;

		public ShowTerrainNodes(ShowTerrainNodesParams parameters)
		{
			Name = "Show Terrain Nodes Previews";

			material = parameters.PreviewMaterial;
			nodePreviewRadius = parameters.AreaRadius / 10f;
			previewParent = parameters.Parent;
			getTerrainNodes = parameters.GetTerrainNodes;

			previews = new List<IHideable>();
		}

		public List<IHideable> GetResult()
		{
			if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
			return previews;
		}

		protected override void ExecuteStep()
		{
			int currentNodeIndex = TotalSteps - RemainingSteps;
			TerrainNode currentNode = terrainNodes[currentNodeIndex];
			PreviewObject newPreview = CreateNodePreview(currentNode);

			previews.Add(newPreview);
		}

		protected override void GetInputFromPreviousTask()
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

			if (node.IsDominant) diameter = diameter * 1.5f;

			Vector3 dimensions = new Vector3(diameter, diameter, diameter);
			newPreview.SetDimensions(dimensions);

			return newPreview;
		}
	}
}