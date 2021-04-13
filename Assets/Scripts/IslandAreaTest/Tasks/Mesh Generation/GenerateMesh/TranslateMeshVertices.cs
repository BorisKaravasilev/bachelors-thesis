using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateMeshVertices : SingleTask
{
	// Input
	private bool visualize;

	// Inputs from previous task
	private Func<TerrainMesh> getTerrainMesh;
	private TerrainMesh terrainMesh;

	// Internal

	// Output


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
