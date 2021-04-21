using UnityEngine;

namespace IslandAreaTest
{
	public class PlacedObjectParams
	{
		[Range(0f, 1f)] private float minHeight = 0f;
		public float MinHeight => minHeight;

		[Range(0f, 1f)] private float maxHeight = 1f;
		public float MaxHeight => maxHeight;

		/// <summary>
		/// The minimum terrain type fraction required for the object to be placed.
		/// </summary>
		private TerrainTypeFraction minimumTerrainFraction;

		public TerrainTypeFraction MinimumTerrainFraction => minimumTerrainFraction;

		private GameObject objectToPlace;
		public GameObject ObjectToPlace => objectToPlace;

		public PlacedObjectParams(float minHeight, float maxHeight, TerrainTypeFraction minimumTerrainFraction,
			GameObject objectToPlace)
		{
			this.minHeight = minHeight;
			this.maxHeight = maxHeight;
			this.minimumTerrainFraction = minimumTerrainFraction;
			this.objectToPlace = objectToPlace;
		}
	}
}