using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateMeshVertices : SingleTask
{
	// Input

	// Inputs from previous task
	private Func<TerrainMesh> getTerrainMesh;
	private TerrainMesh terrainMesh;

	// Internal

	// Output


	public TranslateMeshVertices(Func<TerrainMesh> getTerrainMesh)
	{
		Name = "Translate Mesh Vertices";
		this.getTerrainMesh = getTerrainMesh;
	}

	public TerrainMesh GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return terrainMesh;
	}

	protected override void ExecuteStep()
	{
		terrainMesh.UpdateVerticesPositions(0.01f);
	}

	protected override void GetInputFromPreviousStep()
	{
		terrainMesh = getTerrainMesh();
		terrainMesh.SetTargetVerticesPositions();
	}

	protected override void SetSteps()
	{
		TotalSteps = 100;
		RemainingSteps = TotalSteps;
	}
}
