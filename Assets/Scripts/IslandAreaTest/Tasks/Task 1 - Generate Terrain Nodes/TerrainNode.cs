using System;
using UnityEngine;

public class TerrainNode
{
	public Vector3 Position { get; private set; }
	public TerrainType Type { get; private set; }
	//private PointPreview preview;

	public TerrainNode(Vector3 position, TerrainType type)
	{
		Position = position;
		Type = type;
	}

	//public void ShowPreview(Transform parent, Vector3 dimensions)
	//{
	//	if (preview == null)
	//	{
	//		preview = new PointPreview();
	//		preview.SetName(Type.Name + "Node");
	//	}

	//	preview.SetParent(parent);
	//	preview.SetLocalPosition(Position);
	//	preview.SetDimensions(dimensions);
	//	preview.Show();
	//}

	//public void HidePreview()
	//{
	//	preview.Hide();
	//}

	//public void SetPreviewColor(Color newColor)
	//{
	//	preview.SetColor(newColor);
	//}

	//public void Destroy()
	//{
	//	preview?.Destroy();
	//}
}