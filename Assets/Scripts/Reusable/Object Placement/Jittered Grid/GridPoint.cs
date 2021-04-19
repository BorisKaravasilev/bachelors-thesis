using UnityEngine;

namespace ObjectPlacement.JitteredGrid
{
	/// <summary>
	/// Point on a grid with a maximum radius without collisions.
	/// </summary>
	public class GridPoint
	{
		public Vector3 Position { get; }
		public float MaxRadius { get; }

		public GridPoint(Vector3 position, float maxRadius)
		{
			Position = position;
			MaxRadius = maxRadius;
		}
	}
}