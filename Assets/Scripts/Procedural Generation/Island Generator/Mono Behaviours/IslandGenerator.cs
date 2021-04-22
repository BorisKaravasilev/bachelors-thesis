using System.Collections.Generic;
using Instantiators.ObjectGrid;
using ObjectPlacement.JitteredGrid;
using Unity.VisualScripting;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Procedurally generates islands from specified parameters.
	/// </summary>
	public class IslandGenerator : MonoBehaviour
	{
		[SerializeField] private BoundingBox3DVariable generatedArea;
		[SerializeField] private JitteredGridParams gridParams;
		[SerializeField] private IslandGenerationParams islandGenerationParams;

		private ObjectGrid<IslandArea> islandGrid;
		private bool InslandsOnStartGenerated = false;

		void Start()
		{
			islandGrid = new ObjectGrid<IslandArea>(gridParams.parameters, gridParams.offsetParams, gameObject.transform);
		}

		void Update()
		{
			islandGrid.InstantiateInBoundingBox(generatedArea.Value);
			List<IslandArea> islandAreas = islandGrid.GetObjects();

			Generate(islandAreas);
		}

		void OnValidate()
		{
			UpdateGridParameters();
		}

		/// <summary>
		/// Generates all islands on startup at once, or generates islands sequentially.
		/// </summary>
		public void Generate(List<IslandArea> islandAreas)
		{
			bool generateAll = islandGenerationParams.GenerateAllOnStart && !InslandsOnStartGenerated;

			if (generateAll)
			{
				GenerateIslands(islandAreas, false);
				InslandsOnStartGenerated = true;
				return;
			}

			GenerateClosestIsland(islandAreas, generatedArea.Value.Center);
		}

		/// <summary>
		/// Updates the parameters of the island grid and destroys already generated islands.
		/// </summary>
		public void UpdateGridParameters()
		{
			islandGrid?.UpdateParameters(gridParams.parameters, gridParams.offsetParams);
		}

		/// <summary>
		/// Generates an island area closest to the given point.
		/// </summary>
		private void GenerateClosestIsland(List<IslandArea> islandAreas, Vector3 point)
		{
			IslandArea closestIslandArea = GetClosestNotFinishedIslandArea(islandAreas, point);
			if (closestIslandArea != null) GenerateIslandArea(closestIslandArea, true);
		}

		/// <summary>
		/// Generates all islands at once (May drop frame rate).
		/// </summary>
		private void GenerateIslands(List<IslandArea> islandsToGenerate, bool generateInSteps)
		{
			foreach (IslandArea islandArea in islandsToGenerate)
			{
				GenerateIslandArea(islandArea, generateInSteps);
			}
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
		/// Returns true if at least one generation step got executed.
		/// </summary>
		private bool GenerateIslandArea(IslandArea islandArea, bool generateInSteps)
		{
			InitIslandArea(islandArea);

			if (!islandArea.Finished)
			{
				GenerateIslandStepOrAllSteps(islandArea, generateInSteps);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Executes a single generation step or all of them.
		/// </summary>
		private void GenerateIslandStepOrAllSteps(IslandArea islandArea, bool generateInSteps)
		{
			if (generateInSteps)
			{
				islandArea.GenerateStep();
			}
			else
			{
				islandArea.Generate();
			}
		}

		/// <summary>
		/// Returns true if area got initialized.
		/// </summary>
		private bool InitIslandArea(IslandArea islandArea)
		{
			if (!islandArea.Initialized)
			{
				islandArea.Init(islandGenerationParams);
				return true;
			}

			return false;
		}
	}
}
