using UnityEngine;

public class PerlinNoise2D : Noise2D
{
	public PerlinNoise2D() { }
	public PerlinNoise2D(Noise2DParams parameters) : base(parameters) { }

	public override float GetValue(Vector2 coordinates)
	{
		float x = (parameters.OffsetX + coordinates.x) * parameters.Scale;
		float y = (parameters.OffsetY + coordinates.y) * parameters.Scale;

		return Mathf.PerlinNoise(x, y);
	}
}