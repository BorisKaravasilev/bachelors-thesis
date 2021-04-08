using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureFunctions
{
	/// <summary>
	/// Converts an flattened 2D array index to 2D array coordinates.
	/// </summary>
	public static Vector2Int ArrayIndexToCoords(int resolution, int index)
	{
		int x = index % resolution;
		int y = index / resolution;
		return new Vector2Int(x, y);
	}
}
