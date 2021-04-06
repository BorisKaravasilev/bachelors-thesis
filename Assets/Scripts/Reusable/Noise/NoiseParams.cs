using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseParams
{
	public float Scale;
	public float OffsetX;
	public float OffsetY;

	public NoiseParams(float scale, float offsetX, float offsetY)
	{
		Scale = scale;
		OffsetX = offsetX;
		OffsetY = offsetY;
	}

	public override string ToString() => $"Noise parameters: (Scale: {Scale}, OffsetX: {OffsetX}, OffsetY: {OffsetY})";
}