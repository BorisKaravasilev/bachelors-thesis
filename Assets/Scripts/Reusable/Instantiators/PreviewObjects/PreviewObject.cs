using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject
{
	protected GameObject gameObject;

	public PreviewObject(PrimitiveType objectType, Transform parent = null)
	{
		Instantiate(objectType, parent);
		SetName("PreviewObject");
	}

	public void Show()
	{
		if (gameObject != null)
		{
			gameObject.GetComponent<MeshRenderer>().enabled = true;
		}
	}

	public void Hide()
	{
		if (gameObject != null)
		{
			gameObject.GetComponent<MeshRenderer>().enabled = false;
		}
	}

	public virtual void SetDimensions(Vector3 dimensions)
	{
		Transform transform = gameObject.transform;
		transform.localScale = new Vector3(dimensions.x, dimensions.y, dimensions.z);
	}

	public void SetLocalPosition(Vector3 localPosition)
	{
		Transform transform = gameObject.transform;
		transform.localPosition = localPosition;
	}

	public virtual void SetLocalEulerAngles(Vector3 localEulerAngles)
	{
		Transform transform = gameObject.transform;
		transform.localEulerAngles = localEulerAngles;
	}

	public void SetParent(Transform parent)
	{
		gameObject.transform.SetParent(parent);
	}

	public void SetName(string newName)
	{
		gameObject.name = newName;
	}

	public void Destroy()
	{
		Object.Destroy(gameObject);
	}

	public void SetColor(Color newColor)
	{
		gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", newColor);
	}

	private GameObject Instantiate(PrimitiveType objectType, Transform parent = null)
	{
		gameObject = GameObject.CreatePrimitive(objectType);
		SetParent(parent);

		gameObject.GetComponent<Renderer>().material.SetFloat("_Smoothness", 0f);

		return gameObject;
	}
}
