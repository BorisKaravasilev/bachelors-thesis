using System;
using UnityEngine;

/// <summary>
/// Generates a preview object for a texture pixels array.
/// </summary>
public class ShowTexture : SingleTask
{
	// Inputs
	private float sideLength;
	private int resolution;
	private Transform parent;

	// Inputs from previous steps
	private Func<Color[]> getTexturePixels;
	private Color[] texturePixels;

	// Output
	private TexturePreview previewObject;

	// Internal
	private const float initialElevation = 0.1f;

	public ShowTexture(float sideLength, int resolution, Transform parent , Func<Color[]> getTexturePixels)
	{
		Name = "Show Texture";

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

		float elevation = initialElevation;

		previewObject.SetTexture(texture);
		previewObject.SetDimensions(new Vector3(sideLength, 0f, sideLength));
		previewObject.SetLocalPosition(new Vector3(0f, elevation, 0f));
	}

	protected override void GetInputFromPreviousStep()
	{
		texturePixels = getTexturePixels();
	}

	protected override void SetSteps()
	{
		TotalSteps = 1;
		RemainingSteps = TotalSteps;
	}
}
