using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a preview object for each texture pixels array.
/// </summary>
public class ShowTextures : SingleTask
{
	// Inputs
	private float sideLength;
	private float heightAboveParent;
	private int resolution;
	private Transform parent;
	private Func<List<Color[]>> getTextures;

	// Inputs from previous steps
	private List<Color[]> textures;

	// Output
	private List<TexturePreview> previewObjects;

	public ShowTextures(float sideLength, float heightAboveParent, int resolution, Transform parent, Func<List<Color[]>> getTextures)
	{
		Name = "Show Textures";

		this.sideLength = sideLength;
		this.heightAboveParent = heightAboveParent;
		this.resolution = resolution;
		this.parent = parent;
		this.getTextures = getTextures;

		previewObjects = new List<TexturePreview>();
	}

	protected override void ExecuteStep()
	{
		int textureIndex = ExecutedSteps;
		TexturePreview previewObject = new TexturePreview(parent);

		Texture2D texture = new Texture2D(resolution, resolution);
		texture.SetPixels(textures[textureIndex]);
		texture.Apply();

		previewObject.SetTexture(texture);
		previewObject.SetDimensions(new Vector3(sideLength, 0f, sideLength));
		previewObject.SetLocalPosition(new Vector3(0, heightAboveParent + textureIndex * 0.01f, 0));

		previewObjects.Add(previewObject);
	}

	protected override void GetInputFromPreviousStep()
	{
		textures = getTextures();
	}

	protected override void SetSteps()
	{
		TotalSteps = textures.Count;
		RemainingSteps = TotalSteps;
	}
}
