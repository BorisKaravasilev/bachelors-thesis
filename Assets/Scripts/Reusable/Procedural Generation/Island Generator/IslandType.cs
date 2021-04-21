using MyRandom;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	[CreateAssetMenu]
	public class IslandType : ScriptableObject, IHasProbability
	{
		public string Name;

		[SerializeField] private float probability = 1f;
		public float Probability
		{
			get => probability;
			set => probability = value;
		}

		public TerrainNodesParams TerrainNodesParams;
	}
}