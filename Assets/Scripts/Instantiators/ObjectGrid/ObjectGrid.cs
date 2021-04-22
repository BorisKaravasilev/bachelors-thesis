using System.Collections.Generic;
using ObjectPlacement.JitteredGrid;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Instantiators.ObjectGrid
{
	/// <summary>
	/// Instantiates objects on a square grid and offsets their positions by noise.
	/// </summary>
	public class ObjectGrid<TObject>
		where TObject : GridObject, new()
	{
		private JitteredGrid jitteredGrid;
		private bool destroyFarObjects;

		private GameObject gameObject;
		private List<TObject> objects;

		private BoundingBox3D lastBoundingBox;

		/// <summary>
		/// Initializes an ObjectGrid from settings without any objects.
		/// </summary>
		public ObjectGrid(GridParams gridParams, OffsetParams offsetParams, Transform parent = null, bool destroyFarObjects = true)
		{
			gameObject = new GameObject("ObjectGrid");
			gameObject.transform.SetParent(parent);
			gameObject.transform.localPosition = Vector3.zero;

			jitteredGrid = new JitteredGrid(gridParams, offsetParams);
			this.destroyFarObjects = destroyFarObjects;

			objects = new List<TObject>();
		}

		/// <summary>
		/// Instantiates objects within the bounding box and may destroy the objects outside of it depending on the settings.
		/// </summary>
		public List<TObject> InstantiateInBoundingBox(BoundingBox3D boundingBox)
		{
			List<TObject> newlyInstantiated = InstantiateObjects(boundingBox);
			if (destroyFarObjects) DestroyObjectsOutOfRange(boundingBox);
			lastBoundingBox = boundingBox;
			return newlyInstantiated;
		}

		public void UpdateParameters(GridParams newGridParams, OffsetParams newOffsetParams)
		{
			jitteredGrid.UpdateParameters(newGridParams, newOffsetParams);

			DestroyAllObjects();
		}

		/// <summary>
		/// Sets a new parent in the hierarchy.
		/// </summary>
		public void SetParent(Transform parent)
		{
			gameObject.transform.parent = parent;
		}

		/// <summary>
		/// Returns a list of all objects instantiated by the object grid.
		/// </summary>
		public List<TObject> GetObjects()
		{
			return objects;
		}

		public void DestroyAllObjects()
		{
			foreach (TObject obj in objects)
			{
				obj.Destroy();
			}

			objects.Clear();
		}

		#region Private Methods

		/// <summary>
		/// Instantiates objects on a grid and returns the newly instantiated ones.
		/// </summary>
		private List<TObject> InstantiateObjects(BoundingBox3D boundingBox)
		{
			List<TObject> newlyInstantiated = new List<TObject>();
			List<GridPoint> gridPoints = jitteredGrid.GetPointsInBoundingBox(boundingBox);

			foreach (GridPoint gridPoint in gridPoints)
			{
				TObject newObject = InstantiateNewObject(gridPoint);

				if (newObject != null)
				{
					objects.Add(newObject);
					newlyInstantiated.Add(newObject);
				}
			}

			return newlyInstantiated;
		}

		private TObject InstantiateNewObject(GridPoint position)
		{
			if (!IsObjectGenerated(position.Position))
			{
				TObject newObject = new TObject();
				GridObjectParams newObjectParams = new GridObjectParams(position.Position, position.MaxRadius);

				newObject.Init(newObjectParams, gameObject.transform);
				return newObject;
			}

			return null;
		}

		private void DestroyObjectsOutOfRange(BoundingBox3D boundingBox)
		{
			List<GridPoint> removedGridPoints = jitteredGrid.RemovePointsOutOfRange(boundingBox);
			List<TObject> objectsToRemove = new List<TObject>();

			foreach (GridPoint gridPoint in removedGridPoints)
			{
				TObject objectToRemove = GetObjectOnGridPoint(gridPoint);

				if (objectToRemove != null)
				{
					objectToRemove.Destroy();
					objectsToRemove.Add(objectToRemove);
				}
			}

			RemoveObjectsFromList(objectsToRemove);
		}

		private TObject GetObjectOnGridPoint(GridPoint gridPoint)
		{
			foreach (TObject obj in objects)
			{
				if (obj.Position == gridPoint.Position) return obj;
			}

			return null;
		}

		private void RemoveObjectsFromList(List<TObject> objectsToRemove)
		{
			foreach (TObject obj in objectsToRemove)
			{
				objects.Remove(obj);
			}
		}

		/// <summary>
		/// Checks if object is already generated at the given position.
		/// </summary>
		private bool IsObjectGenerated(Vector3 checkedPosition)
		{
			bool isGenerated = false;

			foreach (TObject obj in objects)
			{
				if (obj.GetPosition() == checkedPosition)
				{
					isGenerated = true;
					break;
				}
			}

			return isGenerated;
		}

		#endregion
	}
}