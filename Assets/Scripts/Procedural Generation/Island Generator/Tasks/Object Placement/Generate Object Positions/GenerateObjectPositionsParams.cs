using ObjectPlacement.JitteredGrid;
using System;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	public class GenerateObjectPositionsParams
	{
		public PlacedObjectParams PlacedObjectParams { get; set; }
		public int Resolution { get; set; }
		public float Radius { get; set; }
		public float MaxTerrainHeight { get; set; }
		public Func<Color[]> GetHeightmap { get; set; }
		public Func<TerrainBlend[]> GetTerrainTypesAtPixels { get; set; }
	}
}