using ObjectPlacement.JitteredGrid;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	[CreateAssetMenu]
	public class PlacedObjectParams : ScriptableObject
	{
		[Range(0f, 1f)]
		[SerializeField] private float minHeight = 0f;
		public float MinHeight => minHeight;

		[Range(0f, 1f)]
		[SerializeField] private float maxHeight = 1f;
		public float MaxHeight => maxHeight;

		/// <summary>
		/// The minimum terrain type fraction required for the object to be placed.
		/// </summary>
		public string MinimumTerrainFractionName;
		public float MinimumTerrainFractionAmount = 0.5f; // 0 to 1

		public GameObject ObjectToPlace;
		public GridParams GridParams;
		public OffsetParams OffsetParams;

		//public PlacedObjectParams(float minHeight, float maxHeight, TerrainTypeFraction minimumTerrainFraction,
		//	GameObject objectToPlace)
		//{
		//	this.minHeight = minHeight;
		//	this.maxHeight = maxHeight;
		//	this.minimumTerrainFraction = minimumTerrainFraction;
		//	this.objectToPlace = objectToPlace;
		//}
	}
}