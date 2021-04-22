using ObjectPlacement.JitteredGrid;
using System;
using System.Collections.Generic;
using TaskManagement;
using UnityEngine;
using Object = System.Object;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Places objects on pre-calculated positions.
	/// </summary>
	public class PlaceObjects : DividableTask
	{
		// Inputs
		private Transform parent;
		private PlacedObjectParams placedObjectParams;

		// Inputs from previous tasks
		private Func<List<GridPoint>> getPositions;
		List<GridPoint> positions;

		// Internal

		// Outputs
		private List<GameObject> placedObjects;

		public PlaceObjects(Transform parent, PlacedObjectParams placedObjectParams, Func<List<GridPoint>> getPositions)
		{
			this.parent = parent;
			this.placedObjectParams = placedObjectParams;
			this.getPositions = getPositions;

			placedObjects = new List<GameObject>();
		}

		protected override void ExecuteStep()
		{
			int positionIndex = ExecutedSteps;

			GameObject newObject = GameObject.Instantiate(placedObjectParams.ObjectToPlace, parent);

			float scale = positions[positionIndex].MaxRadius;
			newObject.transform.localScale = new Vector3(scale, scale, scale);

			Vector3 objectPosition = positions[positionIndex].Position;
			newObject.transform.localPosition = objectPosition;

			placedObjects.Add(newObject);
		}

		protected override void GetInputFromPreviousStep()
		{
			positions = getPositions();
		}

		protected override void SetSteps()
		{
			TotalSteps = positions.Count;
			RemainingSteps = TotalSteps;
		}
	}
}
