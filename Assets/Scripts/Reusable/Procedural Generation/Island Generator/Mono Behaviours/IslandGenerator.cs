using System.Collections.Generic;
using ObjectPlacement.JitteredGrid;
using UnityEngine;

namespace ProceduralGeneration.IslandArea
{
	public class IslandGenerator : MonoBehaviour
	{
		[SerializeField] private BoundingBox3DVariable generatedArea;
		[SerializeField] private GridParams gridParams;
		[SerializeField] private OffsetParams offsetParams;
		[SerializeField] private List<IslandType> islandTypes;

		private JitteredGrid islandGrid;

		void Start()
		{
			//islandGrid = new JitteredGrid(gridParams, offsetParams);
		}

		void Update()
		{
			//islandGrid.GetPointsInBoundingBox(generatedArea.Value);
			//List<GridPoint> islandPositions = islandGrid.GetPoints();
			//GenerateIslands(islandPositions);
		}

		void OnValidate()
		{
			UpdateParams();
		}

		public void UpdateParams()
		{
			islandGrid?.UpdateParameters(gridParams, offsetParams);
		}

		/// <summary>
		/// 
		/// </summary>
		//private void GenerateIsland(List<GridPoint> islandPositions)
		//{
		//	IslandAreaOld closestIslandArea = GetClosestNotFinishedIslandArea(islandAreas);
		//	if (closestIslandArea != null) InitOrGenerateArea(closestIslandArea);
		//}
	}
}
