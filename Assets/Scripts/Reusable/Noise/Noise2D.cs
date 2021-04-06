using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Noise2D
{
	protected NoiseParams parameters;

	public Noise2D()
	{
		this.parameters = new NoiseParams(1f, 0f, 0f);
	}

	public Noise2D(NoiseParams parameters)
	{
		this.parameters = parameters;
	}

	public void SetParameters(NoiseParams parameters)
	{
		this.parameters = parameters;
	}

	public abstract float GetValue(Vector2 coordinates);
}
