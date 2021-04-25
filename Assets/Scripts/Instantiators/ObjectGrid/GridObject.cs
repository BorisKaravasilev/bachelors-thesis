using UnityEngine;

namespace Instantiators.ObjectGrid
{
	public class GridObject
	{
		public float Radius { get; private set; }

		protected GameObject gameObject;

		private const string DEFAULT_NAME = "GridObject";

		public GridObject()
		{
			gameObject = new GameObject(DEFAULT_NAME);
			Radius = 1f;
		}

		public GameObject Init(GridObjectParams parameters, Transform parent)
		{
			Radius = parameters.Radius;

			SetParent(parent);
			SetPosition(parameters.Position);

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

		public void SetLocalPosition(Vector3 localPosition)
		{
			Transform transform = gameObject.transform;
			transform.localPosition = localPosition;
		}

		public Vector3 GetPosition()
		{
			return gameObject.transform.position;
		}

		public Vector3 GetLocalPosition()
		{
			return gameObject.transform.localPosition;
		}

		public GridObjectParams GetParams()
		{
			return new GridObjectParams(GetPosition(), Radius);
		}
	}
}