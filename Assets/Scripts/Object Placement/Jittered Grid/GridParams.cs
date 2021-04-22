using UnityEngine;

namespace ObjectPlacement.JitteredGrid
{
	[System.Serializable]
	public class GridParams
	{
		[Range(1, 40)]
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

		public GridParams()
		{
			spacing = 15;
			pointCountLimit = 500;
		}

		public GridParams(int spacing, int pointCountLimit)
		{
			this.spacing = spacing;
			this.pointCountLimit = pointCountLimit;
		}
	}
}
