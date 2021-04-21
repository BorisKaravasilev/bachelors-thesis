using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	[System.Serializable]
	public class IslandGenerationParams
	{
		public bool PreviewProgress;
		public float VisualStepTime;
		public Material PreviewsMaterial;
		public Material TexturePreviewMaterial;
		[Range(5, 30)]
		public int PixelsPerUnit = 10;
		public List<IslandType> IslandTypes;
	}
}