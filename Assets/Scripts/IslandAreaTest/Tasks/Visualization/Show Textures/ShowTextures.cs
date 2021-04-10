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
	private int resolution;
	private Transform parent;
	private Func<List<Color[]>> getTextures;

	// Inputs from previous steps
	private List<Color[]> textures;

	// Output
	private List<TexturePreview> previewObjects;

	// Internal
	private const float initialElevation = 0.1f;

	public ShowTextures(float sideLength, int resolution, Transform parent, Func<List<Color[]>> getTextures)
	{
		Name = "Show Textures";

		this.sideLength = sideLength;
		this.resolution = resolution;
		this.parent = parent;
		this.getTextures = getTextures;

		previewObjects = new List<TexturePreview>();
	}

	public List<TexturePreview> GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return previewObjects;
	}

	protected override void ExecuteStep()
	{
		int textureIndex = ExecutedSteps;
		TexturePreview previewObject = new TexturePreview(parent);

		Texture2D texture = new Texture2D(resolution, resolution);
		texture.SetPixels(textures[textureIndex]);
		texture.Apply();

		float elevation = textureIndex * 0.01f + initialElevation;

		previewObject.SetTexture(texture);
		previewObject.SetDimensions(new Vector3(sideLength, 0f, sideLength));
		previewObject.SetLocalPosition(new Vector3(0, elevation, 0));

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
