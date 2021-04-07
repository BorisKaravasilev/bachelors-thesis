using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNodesHeightmap : SingleTask
{
	// Inputs
	private float areaRadius;
	private int resolution;
	private Transform previewParent;
	private Func<Color[]> getHeightmapPixels;

	// Inputs from previous steps
	private Color[] heightmapPixels;

	// Output
	private TexturePreview heightmapPreview;

	public ShowNodesHeightmap(float areaRadius, int resolution, Transform previewParent , Func<Color[]> getHeightmapPixels)
	{
		Name = "Generate Terrain Nodes";

		this.areaRadius = areaRadius;
		this.resolution = resolution;
		this.previewParent = previewParent;
		this.getHeightmapPixels = getHeightmapPixels;
	}

	protected override void ExecuteStep()
	{
		heightmapPreview = new TexturePreview(previewParent);

		Texture2D heightmapTexture = new Texture2D(resolution, resolution);
		heightmapTexture.SetPixels(heightmapPixels);
		heightmapTexture.Apply();

		heightmapPreview.SetTexture(heightmapTexture);
		heightmapPreview.SetDimensions(new Vector3(areaRadius * 2, 0f, areaRadius * 2));
	}

	protected override void GetInputFromPreviousStep()
	{
		heightmapPixels = getHeightmapPixels();
	}
}
