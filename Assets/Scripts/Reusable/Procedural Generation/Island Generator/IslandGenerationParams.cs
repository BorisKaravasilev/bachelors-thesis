using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	[System.Serializable]
	public class IslandGenerationParams
	{
		public List<IslandType> IslandTypes;
		public bool PreviewProgress;
		public float VisualStepTime;
		public Material PreviewsMaterial;
	}
}
