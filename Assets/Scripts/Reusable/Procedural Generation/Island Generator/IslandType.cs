using MyRandom;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	[System.Serializable]
	public class IslandType : ScriptableObject, IHasProbability
	{
		public string Name;
		public float Probability { get; set; }
		public TerrainNodesParams TerrainNodesParams;
	}
}