using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	[System.Serializable]
	public class IslandGenerationParams
	{
		public bool GenerateAllOnStart = false;
		public bool PreviewProgress;
		public float VisualStepTime;
		public Material PreviewsMaterial;
		public Material TexturePreviewMaterial;
		[Range(1, 20)]
		public int PixelsPerUnit = 10;
		[Range(0.5f, 6f)]
		public float VerticesPerUnit = 0.5f;
		public List<IslandType> IslandTypes;
	}
}