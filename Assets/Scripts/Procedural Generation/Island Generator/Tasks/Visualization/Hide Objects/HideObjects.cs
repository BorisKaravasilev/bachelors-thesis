using System;
using System.Collections.Generic;
using TaskManagement;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Disables renderer component on preview objects.
	/// </summary>
	class HideObjects<THideable> : DividableTask where THideable : IHideable
	{
		// Inputs from previous steps
		private List<THideable> previewObjects;
		private Func<List<THideable>> getObjects;

		public HideObjects(Func<List<THideable>> getObjects)
		{
			Name = "Hide Objects";
			this.getObjects = getObjects;
		}

		protected override void ExecuteStep()
		{
			int objectIndex = ExecutedSteps;
			previewObjects[objectIndex].Hide();
		}

		protected override void GetInputFromPreviousStep()
		{
			previewObjects = getObjects();
		}

		protected override void SetSteps()
		{
			TotalSteps = previewObjects.Count;
			RemainingSteps = TotalSteps;
		}
	}
}