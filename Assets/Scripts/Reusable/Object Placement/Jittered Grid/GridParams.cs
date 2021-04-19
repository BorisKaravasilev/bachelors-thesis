using UnityEngine;

namespace ObjectPlacement.JitteredGrid
{
	[System.Serializable]
	public class GridParams
	{
		[Range(10, 40)]
		private int spacing = 15;
		public int Spacing
		{
			get => spacing;
			set => spacing = value;
		}

		[Range(1, 500)]
		private int pointCountLimit = 500;
		public int PointCountLimit
		{
			get => pointCountLimit;
			set => pointCountLimit = value;
		}

		private bool destroyFarPoints = true;
		public bool DestroyFarPoints
		{
			get => destroyFarPoints;
			set => destroyFarPoints = value;
		}

		public GridParams(int spacing, int pointCountLimit, bool destroyFarPoints)
		{
			this.spacing = spacing;
			this.pointCountLimit = pointCountLimit;
			this.destroyFarPoints = destroyFarPoints;
		}
	}
}
