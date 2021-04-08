using System;
using UnityEngine;

/// <summary>
/// Generates a preview object for a texture.
/// </summary>
public class ShowTexture : SingleTask
{
	// Inputs
	private float sideLength;
	private int resolution;
	private Transform parent;
	private Func<Color[]> getTexturePixels;

	// Inputs from previous steps
	private Color[] texturePixels;

	// Output
	private TexturePreview previewObject;

	public ShowTexture(float sideLength, int resolution, Transform parent , Func<Color[]> getTexturePixels)
	{
		Name = "Generate Terrain Nodes";

		this.sideLength = sideLength;
		this.resolution = resolution;
		this.parent = parent;
		this.getTexturePixels = getTexturePixels;
	}

	protected override void ExecuteStep()
	{
		previewObject = new TexturePreview(parent);

		Texture2D texture = new Texture2D(resolution, resolution);
		texture.SetPixels(texturePixels);
		texture.Apply();

		previewObject.SetTexture(texture);
		previewObject.SetDimensions(new Vector3(sideLength, 0f, sideLength));
	}

	protected override void GetInputFromPreviousStep()
	{
		texturePixels = getTexturePixels();
	}
}
