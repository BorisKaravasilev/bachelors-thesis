using System.Collections.Generic;
using Instantiators.ObjectGrid;
using ObjectPlacement.JitteredGrid;
using UnityEngine;

namespace ProceduralGeneration.IslandArea
{
	public class IslandGenerator : MonoBehaviour
	{
		[SerializeField] private BoundingBox3DVariable generatedArea;
		[SerializeField] private JitteredGridParams gridParams;
		[SerializeField] private List<IslandType> islandTypes;

		private ObjectGrid<IslandAreaOld> islandGrid;

		void Start()
		{
			islandGrid = new ObjectGrid<IslandAreaOld>(gridParams.parameters, gridParams.offsetParams, gameObject.transform);
		}

		void Update()
		{
			islandGrid.InstantiateInBoundingBox(generatedArea.Value);
			List<IslandAreaOld> islandAreas = islandGrid.GetObjects();

			//GenerateIslandAreas(islandAreas);
		}

		void OnValidate()
		{
			UpdateParams();
		}

		public void UpdateParams()
		{
			islandGrid?.UpdateParameters(gridParams.parameters, gridParams.offsetParams);
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
