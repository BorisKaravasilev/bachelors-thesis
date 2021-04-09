using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Instantiates objects on a square grid and offsets their positions by noise.
/// </summary>
public class ObjectGrid<TObject, TNoise2D>
	where TObject : GridObject, new()
	where TNoise2D : Noise2D, new()
{
	private GridParams gridParams;
	private GridOffsetParams gridOffsetParams;

	TNoise2D xOffsetNoise;
	TNoise2D zOffsetNoise;
	TNoise2D thresholdNoise;

	private GameObject gameObject;
	private List<TObject> objects;

	private BoundingBox3D lastBoundingBox;

	/// <summary>
	/// Initializes an ObjectGrid from settings without any objects.
	/// </summary>
	public ObjectGrid(GridParams gridParams, GridOffsetParams gridOffsetParams, Transform parent = null)
	{
		gameObject = new GameObject("ObjectGrid");
		gameObject.transform.SetParent(parent);
		gameObject.transform.localPosition = Vector3.zero;

		this.gridParams = gridParams;
		this.gridOffsetParams = gridOffsetParams;

		objects = new List<TObject>();

		xOffsetNoise = new TNoise2D();
		zOffsetNoise = new TNoise2D();
		thresholdNoise = new TNoise2D();

		xOffsetNoise.SetParameters(gridOffsetParams.xOffsetParams);
		zOffsetNoise.SetParameters(gridOffsetParams.zOffsetParams);
		thresholdNoise.SetParameters(gridOffsetParams.thresholdParams);
	}

	/// <summary>
	/// Instantiates objects within the bounding box and may destroy the objects outside of it depending on the settings.
	/// </summary>
	public List<TObject> InstantiateInBoundingBox(BoundingBox3D boundingBox)
	{
		List<TObject> newlyInstantiated = InstantiateObjects(boundingBox);
		if (gridParams.destroyFarObjects) DestroyObjectsOutOfRange(boundingBox);
		lastBoundingBox = boundingBox;
		return newlyInstantiated;
	}

	public void UpdateParameters(GridParams newGridParams, GridOffsetParams newGridOffsetParams)
	{
		this.gridParams = newGridParams;
		this.gridOffsetParams = newGridOffsetParams;

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

		Vector3 BottomLeftGridPoint = FindBottomLeftGridPointInBounds(boundingBox);

		int objectMinX = (int)BottomLeftGridPoint.x;
		int objectMinZ = (int)BottomLeftGridPoint.z;

		int objectMaxX = Mathf.FloorToInt(boundingBox.TopRight.x);
		int objectMaxZ = Mathf.FloorToInt(boundingBox.TopRight.z);

		for (int z = objectMinZ; z <= objectMaxZ; z += gridParams.spacing)
		{
			for (int x = objectMinX; x <= objectMaxX; x += gridParams.spacing)
			{
				if (ObjectCountLimitReached()) return newlyInstantiated;

				Vector3 objectPosition = new Vector3(x, gameObject.transform.position.y, z);
				objectPosition = OffsetPositionByNoise(objectPosition);
				TObject newObject = InstantiateNewObject(objectPosition);

				if (newObject != null)
				{
					objects.Add(newObject);
					newlyInstantiated.Add(newObject);
				}
			}
		}

		return newlyInstantiated;
	}

	private TObject InstantiateNewObject(Vector3 position)
	{
		if (!IsObjectGenerated(position) && IsObjectAboveThreshold(position))
		{
			TObject newObject = new TObject();
			GridObjectParams newObjectParams = new GridObjectParams(position, GetObjectRadius());

			newObject.Init(newObjectParams, gameObject.transform);
			return newObject;
		}

		return null;
	}

	private bool ObjectCountLimitReached()
	{
		if (objects.Count >= gridParams.objectCountLimit)
		{
			Debug.LogWarning(gameObject.name + " instantiated objects limit of " + gridParams.objectCountLimit + " reached.");
			return true;
		}
		else
		{
			return false;
		}
	}

	private float GetObjectRadius()
	{
		float radius = gridParams.spacing / 2f; // TODO: Consider max. offset of the node
		if (radius > gridParams.maxObjectRadius) radius = gridParams.maxObjectRadius;

		return radius;
	}

	private void DestroyObjectsOutOfRange(BoundingBox3D boundingBox)
	{
		List<TObject> objectsToRemove = new List<TObject>();

		foreach (TObject obj in objects)
		{
			Vector3 currentObjectPosition = obj.GetPosition();

			if (!boundingBox.EnclosesInXZ(currentObjectPosition))
			{
				obj.Destroy();
				objectsToRemove.Add(obj);
			}
		}

		RemoveObjectsFromList(objectsToRemove);
	}

	private void RemoveObjectsFromList(List<TObject> objectsToRemove)
	{
		foreach (TObject obj in objectsToRemove)
		{
			objects.Remove(obj);
		}
	}

	private Vector3 FindBottomLeftGridPointInBounds(BoundingBox3D boundingBox)
	{
		int objectMinX = Mathf.CeilToInt(boundingBox.BottomLeft.x);
		int objectMinZ = Mathf.CeilToInt(boundingBox.BottomLeft.z);

		int xDistanceToGridPoint = objectMinX % gridParams.spacing;
		int mostLeftXOnGridInBounds = objectMinX - xDistanceToGridPoint;

		int zDistanceToGridPoint = objectMinZ % gridParams.spacing;
		int lowestZOnGridInBounds = objectMinZ - zDistanceToGridPoint;

		return new Vector3(mostLeftXOnGridInBounds, gameObject.transform.position.y, lowestZOnGridInBounds);
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

	private Vector3 OffsetPositionByNoise(Vector3 position)
	{
		Vector2 noiseCoordinates = new Vector2(position.x, position.z);

		// Noise returns values 0-1
		// - subtracting 0.5 from it shifts the range -0.5 - 0.5
		// - multiplying by 2 extends the range to -1 - 1
		// - multiplying by max. offset shifts the value to the range of -maxObjectOffset - maxObjectOffset
		float offsetX = (xOffsetNoise.GetValue(noiseCoordinates) - 0.5f) * 2f * gridOffsetParams.maxOffset;
		float offsetZ = (zOffsetNoise.GetValue(noiseCoordinates) - 0.5f) * 2f * gridOffsetParams.maxOffset;

		Vector3 newPosition = new Vector3();

		newPosition.x = position.x + offsetX;
		newPosition.y = position.y;
		newPosition.z = position.z + offsetZ;

		return newPosition;
	}

	private bool IsObjectAboveThreshold(Vector3 position)
	{
		Vector2 noiseCoordinates = new Vector2(position.x, position.z);
		return thresholdNoise.GetValue(noiseCoordinates) >= gridOffsetParams.threshold;
	}

	#endregion
}
