using ObjectPlacement.JitteredGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManagement;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Generates positions on an island area by removing unsuitable positions on a jittered grid.
	/// </summary>
	public class GenerateObjectPositions : DividableTask
	{
		// Inputs
		private PlacedObjectParams placedObjectParams;
		private int resolution;
		private float radius;
		private float maxTerrainHeight;

		// Inputs from previous tasks
		private Func<Color[]> getHeightmap;
		private Color[] heightmap;

		private Func<TerrainMesh> getTerrainMesh;
		private TerrainMesh terrainMesh;

		private Func<TerrainBlend[]> getTerrainTypesAtPixels;
		private TerrainBlend[] terrainTypesAtPixels;

		// Internal
		private BoundingBox3D boundingBox;
		private JitteredGrid jitteredGrid;

		// Outputs
		private List<GridPoint> positions;

		public GenerateObjectPositions(GenerateObjectPositionsParams parameters)
		{
			Name = "Generate Object Positions";

			placedObjectParams = parameters.PlacedObjectParams;
			resolution = parameters.Resolution;
			radius = parameters.Radius;
			maxTerrainHeight = parameters.MaxTerrainHeight;
			getHeightmap = parameters.GetHeightmap;
			getTerrainMesh = parameters.GetTerrainMesh;
			getTerrainTypesAtPixels = parameters.GetTerrainTypesAtPixels;

			Vector3 areaBottomLeft = new Vector3(-radius, 0f, -radius);
			Vector3 areaTopRight = new Vector3(radius, 0f, radius);
			boundingBox = new BoundingBox3D(areaBottomLeft, areaTopRight);

			jitteredGrid = new JitteredGrid(placedObjectParams.GridParams, placedObjectParams.OffsetParams);
		}

		public List<GridPoint> GetResult()
		{
			if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
			return positions;
		}

		protected override void ExecuteStep()
		{
			positions = jitteredGrid.GetPointsInBoundingBox(boundingBox);
			positions.RemoveAll(IsPositionBad);
			SetPositionsHeights();
		}

		protected override void GetInputFromPreviousTask()
		{
			heightmap = getHeightmap();
			terrainMesh = getTerrainMesh();
			terrainTypesAtPixels = getTerrainTypesAtPixels();
		}

		protected override void SetSteps()
		{
			TotalSteps = 1;
			RemainingSteps = TotalSteps;
		}

		private void SetPositionsHeights()
		{
			foreach (GridPoint position in positions)
			{
				Vector2Int pixelCoords = TextureFunctions.LocalPositionToPixel(position.Position, resolution, radius);
				int pixelIndex = TextureFunctions.CoordsToArrayIndex(resolution, resolution, pixelCoords);

				Vector3 liftedPosition = position.Position;
				liftedPosition.y = heightmap[pixelIndex].r * maxTerrainHeight;

				float groundedObjectHeight = GetClosestFourVerticesAverageHeight(liftedPosition);
				liftedPosition.y = groundedObjectHeight;

				position.Position = liftedPosition;
			}
		}

		/// <summary>
		/// To prevent objects floating in the air its height is calculated as an average of the closest four vertices.
		/// </summary>
		private float GetClosestFourVerticesAverageHeight(Vector3 position)
		{
			List<Vector3> vertices = terrainMesh.Vertices.ToList();
			List<Vector3> orderedByDistance = vertices.OrderBy(vertex => GetDistanceToVertex(vertex, position)).ToList();

			int verticesToAverageCount = orderedByDistance.Count >= 4 ? 4 : orderedByDistance.Count;
			float heightSum = 0f;

			for (int i = 0; i < verticesToAverageCount; i++)
			{
				heightSum += orderedByDistance[i].y;
			}

			float average = orderedByDistance.Count > 0 ? heightSum / verticesToAverageCount : position.y;

			return average;
		}

		/// <summary>
		/// Compensates for the vertices offset from center by "radius" in X and Z. Calculates distance to position.
		/// </summary>
		private float GetDistanceToVertex(Vector3 vertex, Vector3 position)
		{
			Vector3 offsettedVertex = vertex;
			offsettedVertex.x -= radius;
			offsettedVertex.z -= radius;

			return Vector3.Distance(offsettedVertex, position);
		}

		private bool IsPositionBad(GridPoint position)
		{
			Vector2Int pixelCoords = TextureFunctions.LocalPositionToPixel(position.Position, resolution, radius);
			int pixelIndex = TextureFunctions.CoordsToArrayIndex(resolution, resolution, pixelCoords);

			return !HeightOk(pixelIndex) || !TerrainTypeOk(pixelIndex);
		}

		private bool HeightOk(int pixelIndex)
		{
			float pixelHeight = heightmap[pixelIndex].r;
			bool tooLow = pixelHeight < placedObjectParams.MinHeight;
			bool tooHigh = pixelHeight > placedObjectParams.MaxHeight;
			return !tooLow && !tooHigh;
		}

		private bool TerrainTypeOk(int pixelIndex)
		{
			List<TerrainTypeFraction> terrainTypes = terrainTypesAtPixels[pixelIndex].TerrainFractions;

			foreach (TerrainTypeFraction typeFraction in terrainTypes)
			{
				bool typeOk = typeFraction.Type.Name == placedObjectParams.MinimumTerrainFractionName;
				bool minAmountOk = typeFraction.Amount >= placedObjectParams.MinimumTerrainFractionAmount;

				if (typeOk && minAmountOk) return true;
			}

			return false;
		}
	}
}