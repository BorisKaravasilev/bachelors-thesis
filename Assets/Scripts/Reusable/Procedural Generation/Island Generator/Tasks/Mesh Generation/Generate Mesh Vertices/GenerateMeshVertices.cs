using System;
using TaskManagement;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Generates and optionally visualizes vertices of a mesh from a heightmap.
	/// </summary>
	public class GenerateMeshVertices : DividableTask
	{
		// Input
		private bool visualize;
		private Transform parent;
		private Vector2Int resolution;
		private Vector3 dimensions;
		private Vector2Int verticesCount;

		// Inputs from previous task
		private Func<Color[]> getHeightmap;
		private Color[] heightmap;

		// Output
		private TerrainMesh terrainMesh;

		public GenerateMeshVertices(GenerateMeshVerticesParams parameters)
		{
			Name = "Generate Mesh Vertices";

			visualize = parameters.Visualize;
			parent = parameters.Parent;
			resolution = parameters.Resolution;
			dimensions = parameters.Dimensions;
			verticesCount = parameters.VerticesCount;
			getHeightmap = parameters.GetHeightmap;
		}

		public TerrainMesh GetResult()
		{
			if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
			return terrainMesh;
		}

		protected override void ExecuteStep()
		{
			terrainMesh.GenerateVerticesStep(1, visualize);
		}

		protected override void GetInputFromPreviousStep()
		{
			heightmap = getHeightmap();
			terrainMesh = new TerrainMesh(parent, heightmap, resolution, dimensions, verticesCount);

			Vector3 offset = new Vector3(-dimensions.x / 2, 0, -dimensions.z / 2);
			terrainMesh.CreateVerticesParent(offset);
		}

		protected override void SetSteps()
		{
			TotalSteps = terrainMesh.TotalVerticesToGenerate;
			RemainingSteps = TotalSteps;
		}
	}
}