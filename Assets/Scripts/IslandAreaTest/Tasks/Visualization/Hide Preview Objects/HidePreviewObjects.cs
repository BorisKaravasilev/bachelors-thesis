using System;
using System.Collections.Generic;

namespace IslandAreaTest
{
	class HidePreviewObjects<THideable> : SingleTask where THideable : IHideable
	{
		// Inputs from previous steps
		private List<THideable> previewObjects;
		private Func<List<THideable>> getPreviewObjects;

		public HidePreviewObjects(Func<List<THideable>> getPreviewObjects)
		{
			Name = "Hide Objects";
			this.getPreviewObjects = getPreviewObjects;
		}

		protected override void ExecuteStep()
		{
			int objectIndex = ExecutedSteps;
			previewObjects[objectIndex].Hide();
		}

		protected override void GetInputFromPreviousStep()
		{
			previewObjects = getPreviewObjects();
		}

		protected override void SetSteps()
		{
			TotalSteps = previewObjects.Count;
			RemainingSteps = TotalSteps;
		}
	}
}