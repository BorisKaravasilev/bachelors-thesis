using UnityEngine;

namespace ObjectPlacement.JitteredGrid
{
	[System.Serializable]
	public class GridParams
	{
		[Range(10, 40)]
		[SerializeField] private int spacing;
		public int Spacing
		{
			get => spacing;
			set => spacing = value;
		}

		[Range(1, 500)]
		[SerializeField] private int pointCountLimit;
		public int PointCountLimit
		{
			get => pointCountLimit;
			set => pointCountLimit = value;
		}

		[SerializeField] private bool destroyFarPoints;
		public bool DestroyFarPoints
		{
			get => destroyFarPoints;
			set => destroyFarPoints = value;
		}

		public GridParams()
		{
			this.spacing = 15;
			this.pointCountLimit = 500;
			this.destroyFarPoints = true;
		}

		public GridParams(int spacing, int pointCountLimit, bool destroyFarPoints)
		{
			this.spacing = spacing;
			this.pointCountLimit = pointCountLimit;
			this.destroyFarPoints = destroyFarPoints;
		}
	}
}
