using UnityEngine;

[System.Serializable]
public class TerrainType
{
	public string Name;
	public Color Color;

	public TerrainType(string name, Color color)
	{
		Name = name;
		Color = color;
	}
}