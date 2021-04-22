using UnityEngine;

public sealed class TexturePreview : PreviewObject
{
	public TexturePreview(Material material, Transform parent = null) : base(PrimitiveType.Quad, material, parent)
	{
		SetName("TexturePreview");
		SetTexture(Texture2D.blackTexture);
		SetLocalEulerAngles(Vector3.zero);
		SetDimensions(Vector3.one);
		SetLocalPosition(Vector3.zero);
	}

	public void SetTexture(Texture2D texture)
	{
		Renderer hmRenderer = gameObject.GetComponent<Renderer>();
		hmRenderer.material.mainTexture = texture;
	}

	public override void SetDimensions(Vector3 dimensions)
	{
		Transform transform = gameObject.transform;

		// Scaling 'y' with 'z' value because quad is rotated by 90° on x axis compared to default plane rotation
		transform.localScale = new Vector3(dimensions.x, dimensions.z, dimensions.y);
	}

	public override void SetLocalEulerAngles(Vector3 localEulerAngles)
	{
		Transform transform = gameObject.transform;
		localEulerAngles.x += 90f; // Rotation offset by 90° to make Quad face up by default
		transform.localEulerAngles = localEulerAngles;
	}
}
