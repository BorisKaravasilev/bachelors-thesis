using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	public class ShowTerrainNodesParams
	{
		public Material PreviewMaterial { get; set; }
		public float AreaRadius { get; set; }
		public Transform Parent { get; set; }
		public Func<List<TerrainNode>> GetTerrainNodes { get; set; }
	}
}
