using UnityEngine;

namespace Instantiators.ObjectGrid
{
	public class GridObject
	{
		public Vector3 Position { get; private set; }
		public float Radius { get; private set; }

		protected GameObject gameObject;

		private const string DEFAULT_NAME = "GridObject";

		public GridObject()
		{
			gameObject = new GameObject(DEFAULT_NAME);

			Position = Vector3.zero;
			Radius = 1f;
		}

		public GameObject Init(GridObjectParams parameters, Transform parent)
		{
			Position = parameters.Position;
			Radius = parameters.Radius;

			SetParent(parent);
			SetPosition(Position);

			return gameObject;
		}

		/// <summary>
		/// All descendents must call "base.Destroy()" in the end of the overriden method.
		/// </summary>
		public virtual void Destroy()
		{
			Object.Destroy(gameObject);
		}

		public void SetParent(Transform parent)
		{
			gameObject.transform.SetParent(parent);
		}

		public void SetPosition(Vector3 position)
		{
			Transform transform = gameObject.transform;
			transform.position = position;
		}

		public Vector3 GetPosition()
		{
			return gameObject.transform.position;
		}

		public GridObjectParams GetParams()
		{
			return new GridObjectParams(Position, Radius);
		}
	}
}