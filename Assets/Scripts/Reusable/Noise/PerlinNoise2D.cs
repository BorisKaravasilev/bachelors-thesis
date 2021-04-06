using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise2D : Noise2D
{
	public override float GetValue(Vector2 coordinates)
	{
		float x = (parameters.OffsetX + coordinates.x) * parameters.Scale;
		float y = (parameters.OffsetY + coordinates.x) * parameters.Scale;

		return Mathf.PerlinNoise(x, y);
	}
}