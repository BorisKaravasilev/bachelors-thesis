using UnityEngine;

public class Tile
{
	public int Size { get; private set; }
	protected GameObject gameObject;
	protected GameObject preview;

	public Tile()
	{
		gameObject = new GameObject("Tile");
	}

	public Tile(int size, Vector3 position, Transform parent = null)
	{
		gameObject = new GameObject("Tile");
		SetParameters(size, position, parent);
	}

	public void SetParameters(int size, Vector3 position, Transform parent = null)
	{
		Size = size;
		gameObject.transform.position = position;
		gameObject.transform.SetParent(parent);
	}

	public GameObject GeneratePreview()
	{
		preview = GeneratePlane(Size);
		preview.name = "Preview";
		return preview;
	}

	protected GameObject GeneratePlane(float size, float height = 0f)
	{
		GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

		Transform pTransform = plane.transform;
		pTransform.SetParent(gameObject.transform);
		pTransform.localPosition = new Vector3(size / 2f, height, size / 2f);

		// Scaling the plane from 10x10 to 1x1
		pTransform.localScale = new Vector3(0.1f * size, 1f, 0.1f * size);
		return plane;
	}

	public void DestroyPreview()
	{
		Object.Destroy(preview);
	}

	public void Destroy()
	{
		DestroyPreview();
		Object.Destroy(gameObject);
	}

	public Vector3 GetPosition()
	{
		return gameObject.transform.position;
	}
}
