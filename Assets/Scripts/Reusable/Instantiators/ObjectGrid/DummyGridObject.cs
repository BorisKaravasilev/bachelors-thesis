using UnityEngine;

public class DummyGridObject : GridObject
{
	private const string DEFAULT_NAME = "DummyGridObject";
	private GameObject preview;

	public DummyGridObject()
	{
		gameObject.name = DEFAULT_NAME;
		GeneratePreview();
	}

	public GameObject GeneratePreview()
	{
		preview = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		preview.name = "Preview";

		Transform pTransform = preview.transform;
		pTransform.SetParent(gameObject.transform);
		pTransform.localPosition = new Vector3(0, 0, 0);
		pTransform.localScale = new Vector3(Parameters.Radius * 2, Parameters.Radius * 2, Parameters.Radius * 2);

		return preview;
	}

	public override void Destroy()
	{
		Object.Destroy(preview);
		Object.Destroy(gameObject);
	}
}
