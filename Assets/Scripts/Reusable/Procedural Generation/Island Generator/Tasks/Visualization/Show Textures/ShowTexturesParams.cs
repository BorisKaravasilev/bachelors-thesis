using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	public class ShowTexturesParams
	{
		public Material TexturePreviewMaterial { get; set; }
		public float SideLength { get; set; }
		public int Resolution { get; set; }
		public Transform Parent { get; set; }
		public Func<List<Color[]>> GetTextures { get; set; }
	}
}