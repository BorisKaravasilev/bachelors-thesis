using System;
using System.Collections.Generic;
using TaskManagement;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Generates a preview object for each texture pixels array.
	/// </summary>
	public class ShowTextures : DividableTask
	{
		// Inputs
		private Material material;
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

		public ShowTextures(ShowTexturesParams parameters)
		{
			Name = "Show Textures";

			material = parameters.TexturePreviewMaterial;
			sideLength = parameters.SideLength;
			resolution = parameters.Resolution;
			parent = parameters.Parent;
			getTextures = parameters.GetTextures;

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
			TexturePreview previewObject = new TexturePreview(material, parent);
			previewObject.SetName(Name);

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
}