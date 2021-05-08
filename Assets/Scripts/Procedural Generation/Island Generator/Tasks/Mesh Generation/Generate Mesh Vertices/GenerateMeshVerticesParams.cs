using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	public class GenerateMeshVerticesParams
	{
		public bool Visualize { get; set; }
		public Transform Parent { get; set; }
		public Vector2Int Resolution { get; set; }
		public Vector3 Dimensions { get; set; }
		public Vector2Int VerticesCount { get; set; }
		public Func<Color[]> GetHeightmap { get; set; }
		public Material VerticesPreviewMaterial { get; set; }
	}
}