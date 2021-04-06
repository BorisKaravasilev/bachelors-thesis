using System.Collections;
using System.Collections.Generic;
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
		SetLocalPosition(parameters.Position);

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

	public void SetLocalPosition(Vector3 localPosition)
	{
		Transform transform = gameObject.transform;
		transform.localPosition = localPosition;
	}

	public Vector3 GetPosition()
	{
		return gameObject.transform.position;
	}
}
