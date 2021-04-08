using UnityEngine;

public class TerrainNodePreview : PreviewObject
{
	public TerrainNodePreview(TerrainNode terrainNode, float radius, Transform parent) : base(PrimitiveType.Sphere, parent)
	{
		SetName("Terrain Node Preview");
		SetColor(terrainNode.Type.Color);
		SetLocalPosition(terrainNode.Position);

		float diameter = radius * 2f;
		Vector3 dimensions = new Vector3(diameter, diameter, diameter);
		SetDimensions(dimensions);
	}
}
