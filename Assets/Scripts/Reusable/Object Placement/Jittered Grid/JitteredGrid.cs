using System.Collections.Generic;
using Noise;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace ObjectPlacement.JitteredGrid
{
	/// <summary>
	/// Generates points on a square grid and offsets them by noise.
	/// </summary>
	public class JitteredGrid
	{
		public GridParams GridParams { get; private set; }
		public OffsetParams OffsetParams { get; private set; }

		Noise2D xOffsetNoise;
		Noise2D zOffsetNoise;
		Noise2D thresholdNoise;

		private List<GridPoint> points;

		/// <summary>
		/// Initialization from settings.
		/// </summary>
		public JitteredGrid(GridParams gridParams, OffsetParams offsetParams)
		{
			GridParams = gridParams;
			OffsetParams = offsetParams;

			points = new List<GridPoint>();

			xOffsetNoise = Noise2DFactory.GetNoise(offsetParams.XOffsetParams);
			zOffsetNoise = Noise2DFactory.GetNoise(offsetParams.ZOffsetParams);
			thresholdNoise = Noise2DFactory.GetNoise(offsetParams.ThresholdParams);
		}

		/// <summary>
		/// Returns newly emerged points within the bounding box.
		/// </summary>
		/// <returns>Newly generated points.</returns>
		public List<GridPoint> GetPointsInBoundingBox(BoundingBox3D boundingBox)
		{
			List<GridPoint> newlyGenerated = GeneratePoints(boundingBox);
			return newlyGenerated;
		}

		/// <summary>
		/// Updates the grid's parameters and removes all previously generated points.
		/// </summary>
		public void UpdateParameters(GridParams newGridParams, OffsetParams newOffsetParams)
		{
			GridParams = newGridParams;
			OffsetParams = newOffsetParams;

			points.Clear();
		}

		/// <summary>
		/// Returns a list of all points generated by the object grid in the last specified bounding box.
		/// </summary>
		public List<GridPoint> GetPoints()
		{
			return points;
		}

		/// <summary>
		/// Returns a list of the removed grid points outside of the given bounding box.
		/// </summary>
		public List<GridPoint> RemovePointsOutOfRange(BoundingBox3D boundingBox)
		{
			List<GridPoint> pointsToRemove = new List<GridPoint>();

			foreach (GridPoint point in points)
			{
				if (!boundingBox.EnclosesInXZ(point.Position))
				{
					pointsToRemove.Add(point);
				}
			}

			return RemovePointsFromList(pointsToRemove);
		}

		#region Private Methods

		private List<GridPoint> RemovePointsFromList(List<GridPoint> pointsToRemove)
		{
			foreach (GridPoint point in pointsToRemove)
			{
				points.Remove(point);
			}

			return pointsToRemove;
		}

		/// <summary>
		/// Generates points on a grid and returns the newly instantiated ones.
		/// </summary>
		private List<GridPoint> GeneratePoints(BoundingBox3D boundingBox)
		{
			List<GridPoint> newlyInstantiated = new List<GridPoint>();

			Vector3 bottomLeftGridPoint = FindBottomLeftPointPosition(boundingBox);

			int minX = (int)bottomLeftGridPoint.x;
			int minZ = (int)bottomLeftGridPoint.z;

			int maxX = Mathf.FloorToInt(boundingBox.TopRight.x);
			int maxZ = Mathf.FloorToInt(boundingBox.TopRight.z);

			for (int z = minZ; z <= maxZ; z += GridParams.Spacing)
			{
				for (int x = minX; x <= maxX; x += GridParams.Spacing)
				{
					if (PointCountLimitReached()) return newlyInstantiated;

					Vector3 onGridPosition = new Vector3(x, 0f, z);
					Vector3 offsetPosition = OffsetPositionByNoise(onGridPosition);
					GridPoint newGridPoint = CreateNewPoint(onGridPosition, offsetPosition);

					if (newGridPoint != null)
					{
						points.Add(newGridPoint);
						newlyInstantiated.Add(newGridPoint);
					}
				}
			}

			return newlyInstantiated;
		}

		/// <summary>
		/// Creates a new grid points and calculates it's maximum radius depending on its offset.
		/// </summary>
		private GridPoint CreateNewPoint(Vector3 onGridPosition, Vector3 offsetPosition)
		{
			if (!IsPointGenerated(offsetPosition) && IsPointAboveThreshold(offsetPosition))
			{
				float maxRadius = GetPointMaxRadius(onGridPosition, offsetPosition);
				return new GridPoint(offsetPosition, maxRadius);
			}

			return null;
		}

		/// <summary>
		/// Checks if maximum generated points limit has been reached.
		/// </summary>
		private bool PointCountLimitReached()
		{
			if (points.Count >= GridParams.PointCountLimit)
			{
				Debug.LogWarning("Reached limit of " + GridParams.PointCountLimit + " generated grid points.");
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Calculates the maximum radius of a point without a collision with other points.
		/// </summary>
		private float GetPointMaxRadius(Vector3 onGridPosition, Vector3 offsetPosition)
		{
			float offsetAmount = Vector3.Distance(onGridPosition, offsetPosition);
			float maxRadius = GridParams.Spacing / 2f - offsetAmount;

			return maxRadius;
		}

		/// <summary>
		/// Finds the left most bottom most point on the grid within the bounding box.
		/// </summary>
		private Vector3 FindBottomLeftPointPosition(BoundingBox3D boundingBox)
		{
			int pointMinX = Mathf.CeilToInt(boundingBox.BottomLeft.x);
			int pointMinZ = Mathf.CeilToInt(boundingBox.BottomLeft.z);

			int xDistanceToGridPoint = pointMinX % GridParams.Spacing;
			int mostLeftXOnGridInBounds = pointMinX - xDistanceToGridPoint;

			int zDistanceToGridPoint = pointMinZ % GridParams.Spacing;
			int lowestZOnGridInBounds = pointMinZ - zDistanceToGridPoint;

			return new Vector3(mostLeftXOnGridInBounds, 0f, lowestZOnGridInBounds);
		}

		/// <summary>
		/// Checks if point is already generated at the given position.
		/// </summary>
		private bool IsPointGenerated(Vector3 checkedPosition)
		{
			bool isGenerated = false;

			foreach (GridPoint point in points)
			{
				if (point.Position == checkedPosition)
				{
					isGenerated = true;
					break;
				}
			}

			return isGenerated;
		}

		/// <summary>
		/// Offsets a point in X and Z axis by two separate instances of noise.
		/// </summary>
		private Vector3 OffsetPositionByNoise(Vector3 position)
		{
			Vector2 noiseCoordinates = new Vector2(position.x, position.z);

			// Noise returns values 0-1
			// - subtracting 0.5 from it shifts the range -0.5 - 0.5
			// - multiplying by 2 extends the range to -1 - 1
			// - multiplying by max. offset shifts the value to the range of -maxObjectOffset - maxObjectOffset
			float limitedMaxOffset = OffsetParams.MaxOffset;

			if (limitedMaxOffset > GridParams.Spacing * 0.4f)
			{
				limitedMaxOffset = GridParams.Spacing * 0.4f;
			}

			float offsetX = (xOffsetNoise.GetValue(noiseCoordinates) - 0.5f) * 2f * limitedMaxOffset;
			float offsetZ = (zOffsetNoise.GetValue(noiseCoordinates) - 0.5f) * 2f * limitedMaxOffset;

			Vector3 newPosition = new Vector3();

			newPosition.x = position.x + offsetX;
			newPosition.y = position.y;
			newPosition.z = position.z + offsetZ;

			return newPosition;
		}

		/// <summary>
		/// Checks if the value of the noise at the given position if is above the threshold.
		/// </summary>
		private bool IsPointAboveThreshold(Vector3 position)
		{
			Vector2 noiseCoordinates = new Vector2(position.x, position.z);
			return thresholdNoise.GetValue(noiseCoordinates) >= OffsetParams.Threshold;
		}

		#endregion
	}
}