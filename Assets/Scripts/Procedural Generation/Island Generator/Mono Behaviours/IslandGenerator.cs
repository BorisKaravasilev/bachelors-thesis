using System.Collections.Generic;
using Instantiators.ObjectGrid;
using ObjectPlacement.JitteredGrid;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	public class IslandGenerator : MonoBehaviour
	{
		[SerializeField] private BoundingBox3DVariable generatedArea;
		[SerializeField] private JitteredGridParams gridParams;
		[SerializeField] private IslandGenerationParams islandGenerationParams;

		private ObjectGrid<IslandArea> islandGrid;

		void Start()
		{
			islandGrid = new ObjectGrid<IslandArea>(gridParams.parameters, gridParams.offsetParams, gameObject.transform);
		}

		void Update()
		{
			islandGrid.InstantiateInBoundingBox(generatedArea.Value);
			List<IslandArea> islandAreas = islandGrid.GetObjects();
			GenerateClosestIsland(islandAreas, generatedArea.Value.Center);
		}

		void OnValidate()
		{
			UpdateParams();
		}

		/// <summary>
		/// Updates the parameters of the island grid and destroys already generated islands.
		/// </summary>
		public void UpdateParams()
		{
			islandGrid?.UpdateParameters(gridParams.parameters, gridParams.offsetParams);
		}

		/// <summary>
		/// Generates an island area closest to the given point.
		/// </summary>
		private void GenerateClosestIsland(List<IslandArea> islandAreas, Vector3 point)
		{
			IslandArea closestIslandArea = GetClosestNotFinishedIslandArea(islandAreas, point);
			if (closestIslandArea != null) InitOrGenerateArea(closestIslandArea);
		}

		/// <summary>
		/// Returns the closest not finished island area to the given point or null.
		/// </summary>
		private IslandArea GetClosestNotFinishedIslandArea(List<IslandArea> islandAreas, Vector3 point)
		{
			float distanceToClosest = float.MaxValue;
			IslandArea closestIslandArea = null;

			foreach (IslandArea islandArea in islandAreas)
			{
				float distance = GetIslandDistanceToPoint(islandArea, point);

				if (distance > 0f && distance < distanceToClosest)
				{
					distanceToClosest = distance;
					closestIslandArea = islandArea;
				}
			}

			return closestIslandArea;
		}

		/// <summary>
		/// Returns the distance to a point or -1f if island area not initialized.
		/// </summary>
		private float GetIslandDistanceToPoint(IslandArea islandArea, Vector3 point)
		{
			if (!islandArea.Initialized || !islandArea.Finished)
			{
				return Vector3.Distance(islandArea.Position, point);
			}

			return -1f;
		}

		/// <summary>
		/// Returns  true if area got initialized or generated.
		/// </summary>
		private bool InitOrGenerateArea(IslandArea islandArea)
		{
			if (!islandArea.Initialized)
			{
				islandArea.Init(islandGenerationParams);
				return true;
			}

			return GenerateIslandArea(islandArea);
		}

		/// <summary>
		/// Returns true if generation step got executed.
		/// </summary>
		private bool GenerateIslandArea(IslandArea islandArea)
		{
			if (!islandArea.Finished)
			{
				islandArea.GenerateStep();
				return true;
			}

			return false;
		}
	}
}
