using UnityEngine;

namespace Instantiators.ObjectGrid
{
	public class GridObjectParams
	{
		public Vector3 Position { get; }
		public float Radius { get; }

		public GridObjectParams(Vector3 position, float radius)
		{
			Position = position;
			Radius = radius;
		}
	}
}