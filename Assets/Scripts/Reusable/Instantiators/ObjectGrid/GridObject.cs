using UnityEngine;

public class GridObject
{
	protected GameObject gameObject;
	public GridObjectParams Parameters { get; private set; }

	private const string DEFAULT_NAME = "GridObject";

	public GridObject()
	{
		gameObject = new GameObject(DEFAULT_NAME);
	}

	public GameObject Init(GridObjectParams parameters, Transform parent)
	{
		SetParent(parent);
		Parameters = parameters;
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

	public Vector3 GetPosition()
	{
		return gameObject.transform.position;
	}
}
