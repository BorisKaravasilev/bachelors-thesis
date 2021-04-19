using Noise;
using UnityEngine;

public abstract class Noise2D : INoise2D
{
	protected Noise2DParams parameters;

	public Noise2D()
	{
		this.parameters = new Noise2DParams(Noise2DType.PerlinNoise, 1f, 0f, 0f);
	}

	public Noise2D(Noise2DParams parameters)
	{
		SetParameters(parameters);
	}

	public void SetParameters(Noise2DParams parameters)
	{
		this.parameters = parameters;
	}

	public abstract float GetValue(Vector2 coordinates);
}
