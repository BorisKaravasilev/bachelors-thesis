using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	public class IslandGenerationParams
	{
		public List<IslandType> IslandTypes { get; set; }
		public bool PreviewProgress { get; set; }
		public float VisualStepTime { get; set; }
		public Material PreviewsMaterial { get; set; }
	}
}
