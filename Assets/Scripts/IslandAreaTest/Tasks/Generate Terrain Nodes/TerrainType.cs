using UnityEngine;

[System.Serializable]
public class TerrainType
{
	public string Name;
	public Color Color;
	public Noise2DParams NoiseParams;

	public TerrainType(string name, Color color, Noise2DParams noiseParams)
	{
		Name = name;
		Color = color;
		NoiseParams = noiseParams;
	}
}