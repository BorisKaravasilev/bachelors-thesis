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
}