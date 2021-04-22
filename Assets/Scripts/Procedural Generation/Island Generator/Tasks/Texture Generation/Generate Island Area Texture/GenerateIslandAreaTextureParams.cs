using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	public class GenerateIslandAreaTextureParams
	{
		public int Resolution { get; set; }
		public TerrainNodesParams TerrainNodesParams { get; set; }
		public Func<List<TerrainNode>> GetTerrainNodes { get; set; }
		public Func<Color[]> GetHeightmap { get; set; }
		public Func<List<Color[]>> GetTerrainNodesHeightmaps { get; set; }
	}
}