using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureFunctions
{
	/// <summary>
	/// Converts a flattened 2D array index to 2D array coordinates.
	/// </summary>
	public static Vector2Int ArrayIndexToCoords(int width, int height, int index)
	{
		int x = index % width;
		int y = index / height;
		return new Vector2Int(x, y);
	}

	/// <summary>
	/// Converts texture 2D coordinates to flattened 2D array index.
	/// </summary>
	public static int CoordsToArrayIndex(int width, int height, Vector2Int pixelCoords)
	{
		return pixelCoords.y * width + pixelCoords.x;
	}

	public static Color[] GetBlackPixels(int count)
	{
		Color[] blackPixels = new Color[count];

		for (int i = 0; i < blackPixels.Length; i++)
		{
			blackPixels[i] = Color.black;
		}

		return blackPixels;
	}

	/// <summary>
	/// Checks if all pixel arrays have the same number of pixels.
	/// </summary>
	public static bool SameLength(List<Color[]> pixelArrays)
	{
		if (pixelArrays != null && pixelArrays.Count > 0)
		{
			int comparedPixelCount = pixelArrays[0].Length;

			foreach (Color[] array in pixelArrays)
			{
				if (array.Length != comparedPixelCount)
				{
					return false;
				}
			}
		}

		return true;
	}
}
