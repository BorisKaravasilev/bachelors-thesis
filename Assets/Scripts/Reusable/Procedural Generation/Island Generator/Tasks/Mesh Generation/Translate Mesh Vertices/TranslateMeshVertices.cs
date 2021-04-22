using System;
using TaskManagement;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Translates mesh vertices to a target position.
	/// </summary>
	public class TranslateMeshVertices : DividableTask
	{
		// Inputs
		private bool visualize;

		// Inputs from previous tasks
		private Func<TerrainMesh> getTerrainMesh;
		private TerrainMesh terrainMesh;

		public TranslateMeshVertices(bool visualize, Func<TerrainMesh> getTerrainMesh)
		{
			Name = "Translate Mesh Vertices";

			this.visualize = visualize;
			this.getTerrainMesh = getTerrainMesh;
		}

		public TerrainMesh GetResult()
		{
			if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
			return terrainMesh;
		}

		protected override void ExecuteStep()
		{
			terrainMesh.UpdateVerticesPositions(0.01f, visualize);
		}

		protected override void GetInputFromPreviousStep()
		{
			terrainMesh = getTerrainMesh();
			terrainMesh.SetTargetVerticesPositions(visualize);

			if (!visualize) terrainMesh.UpdateVerticesPositions(1f, visualize);
		}

		protected override void SetSteps()
		{
			TotalSteps = visualize ? 100 : 0;
			RemainingSteps = TotalSteps;
		}
	}
}