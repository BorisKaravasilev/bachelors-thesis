using System;
using TaskManagement;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	public class GenerateMesh : DividableTask
	{
		// Input
		private Material material;

		// Inputs from previous task
		private Func<TerrainMesh> getTerrainMesh;
		private TerrainMesh terrainMesh;

		private Func<Color[]> getTexturePixels;
		private Color[] texturePixels;

		// Internal

		// Output


		public GenerateMesh(Material material, Func<TerrainMesh> getTerrainMesh, Func<Color[]> getTexturePixels)
		{
			Name = "Generate Mesh";

			this.material = material;
			this.getTerrainMesh = getTerrainMesh;
			this.getTexturePixels = getTexturePixels;
		}

		public TerrainMesh GetResult()
		{
			if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
			return terrainMesh;
		}

		protected override void ExecuteStep()
		{
			terrainMesh.GenerateMeshStep(1);

			if (RemainingSteps == 1)
			{
				terrainMesh.HideVertexVisualizations();
			}
		}

		protected override void GetInputFromPreviousTask()
		{
			terrainMesh = getTerrainMesh();
			texturePixels = getTexturePixels();

			Vector3 offset = new Vector3(-terrainMesh.Dimensions.x / 2, 0, -terrainMesh.Dimensions.z / 2);
			terrainMesh.CreateMeshGameObject(texturePixels, offset, material);
		}

		protected override void SetSteps()
		{
			TotalSteps = terrainMesh.TotalTrianglesToGenerate / 2; // Each step generates two triangles
			RemainingSteps = TotalSteps;
		}
	}
}