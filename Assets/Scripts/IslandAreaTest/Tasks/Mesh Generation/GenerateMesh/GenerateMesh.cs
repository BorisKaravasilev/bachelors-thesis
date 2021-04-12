using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMesh : SingleTask
{
	// Input
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;

	// Inputs from previous task
	private Func<TerrainMesh> getTerrainMesh;
	private TerrainMesh terrainMesh;

	private Func<Color[]> getTexturePixels;
	private Color[] texturePixels;

	// Internal

	// Output


	public GenerateMesh(MeshFilter meshFilter, MeshRenderer meshRenderer, Func<TerrainMesh> getTerrainMesh, Func<Color[]> getTexturePixels)
	{
		Name = "Generate Mesh";

		this.meshFilter = meshFilter;
		this.meshRenderer = meshRenderer;
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
	}

	protected override void GetInputFromPreviousStep()
	{
		terrainMesh = getTerrainMesh();
		texturePixels = getTexturePixels();

		Texture2D texture = new Texture2D(terrainMesh.Resolution.x, terrainMesh.Resolution.y);
		texture.SetPixels(texturePixels);
		texture.Apply();

		meshFilter.mesh = terrainMesh.Mesh;
		meshRenderer.material.shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default");
		meshRenderer.material.SetTexture("_MainTex", texture);
	}

	protected override void SetSteps()
	{
		TotalSteps = terrainMesh.TotalTrianglesToGenerate;
		RemainingSteps = TotalSteps;
	}
}
