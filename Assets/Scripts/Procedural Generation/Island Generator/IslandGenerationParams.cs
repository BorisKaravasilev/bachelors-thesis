using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	[System.Serializable]
	public class IslandGenerationParams
	{
		public bool GenerateAllOnStart = false;
		public bool PreviewProgress;
		public bool ShowIslandNames;
		public int IslandNamesFontSize = 20;
		public float VisualStepTime;
		public Material PreviewsMaterial;
		public Material TexturePreviewMaterial;
		public List<IslandType> IslandTypes;
	}
}