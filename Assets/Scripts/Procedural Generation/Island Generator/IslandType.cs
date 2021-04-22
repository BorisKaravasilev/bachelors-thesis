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

		public TerrainNodesParams TerrainNodesParams;
		public float MaxTerrainHeight = 1f;
		public Material TerrainMeshMaterial;
		public TextAsset IslandNames;
	}
}