using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMeshVertices : SingleTask
{
	// Input
	private bool visualize;
	private Transform parent;
	private Vector2Int resolution;
	private Vector3 dimensions;
	private Vector2Int verticesCount;

	// Inputs from previous task
	private Func<Color[]> getHeightmap;

	// Internal
	private Color[] heightmap;

	// Output
	private TerrainMesh terrainMesh;

	public GenerateMeshVertices(bool visualize, Transform parent, Vector2Int resolution, Vector3 dimensions, Vector2Int verticesCount, Func<Color[]> getHeightmap)
	{
		Name = "Generate Mesh Vertices";
		this.visualize = visualize;
		this.parent = parent;
		this.resolution = resolution;
		this.dimensions = dimensions;
		this.verticesCount = verticesCount;
		this.getHeightmap = getHeightmap;
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
	}

	protected override void SetSteps()
	{
		TotalSteps = terrainMesh.TotalVerticesToGenerate;
		RemainingSteps = TotalSteps;
	}
}
