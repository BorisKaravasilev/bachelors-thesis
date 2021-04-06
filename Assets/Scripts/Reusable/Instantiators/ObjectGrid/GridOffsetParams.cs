using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridOffsetParams
{
	[Range(0f, 10f)]
	public float maxOffset;

	[Range(0f, 1f)]
	public float threshold;

	public NoiseParams xOffsetParams;
	public NoiseParams zOffsetParams;
	public NoiseParams thresholdParams;
}