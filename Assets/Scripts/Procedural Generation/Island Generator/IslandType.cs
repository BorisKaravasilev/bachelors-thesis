using System.Collections.Generic;
using MyRandom;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	[CreateAssetMenu]
	public class IslandType : ScriptableObject, IHasProbability
	{
		public string Name = "New Island Type";

		[SerializeField] private float probability = 1f;
		public float Probability
		{
			get => probability;
			set => probability = value;
		}

		[Range(1, 20)]
		public int PixelsPerUnit = 10;
		[Range(0.5f, 6f)]
		public float VerticesPerUnit = 0.5f;

		public TerrainNodesParams TerrainNodesParams;
		public List<PlacedObjectParams> PlacedObjectParams;

		public float MaxTerrainHeight = 1f;
		public Material TerrainMeshMaterial;
		public TextAsset IslandNames;
	}
}