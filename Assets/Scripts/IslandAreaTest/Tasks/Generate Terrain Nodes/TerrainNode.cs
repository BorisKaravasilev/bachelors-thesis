using UnityEngine;

public class TerrainNode
{
	public Vector3 Position { get; private set; }
	public TerrainType Type { get; private set; }
	public float Radius { get; private set; }

	public TerrainNode(Vector3 position, TerrainType type, float radius)
	{
		Position = position;
		Type = type;
		Radius = radius;
	}
}