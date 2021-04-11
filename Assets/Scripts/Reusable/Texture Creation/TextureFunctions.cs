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

	/// <summary>
	/// Origin (0,0,0) is expected to be in the center of the texture.
	/// </summary>
	public static Vector2Int LocalPositionToPixel(Vector3 position, int resolution, float textureRadius)
	{
		float textureDiameter = textureRadius * 2;
		float worldPixelLength = textureDiameter / resolution;

		int x = (int)((position.x + textureRadius) / worldPixelLength);
		int z = (int)((position.z + textureRadius) / worldPixelLength);

		int maxPixelIndex = resolution - 1;
		if (x < 0) x = 0;
		if (x > maxPixelIndex) x = maxPixelIndex;

		if (z < 0) z = 0;
		if (z > maxPixelIndex) z = maxPixelIndex;

		return new Vector2Int(x, z);
	}

	/// <summary>
	/// Origin (0,0,0) is expected to be in the center of the texture.
	/// </summary>
	public static Vector3 PixelToLocalPosition(Vector2Int pixelCoords, float worldPixelLength, float textureRadius)
	{
		float worldPixelLengthHalf = worldPixelLength / 2;

		float localX = PixelsWorldLength(pixelCoords.x, worldPixelLength) + worldPixelLengthHalf - textureRadius;
		float localY = 0f;
		float localZ = PixelsWorldLength(pixelCoords.y, worldPixelLength) + worldPixelLengthHalf - textureRadius;

		return new Vector3(localX, localY, localZ);
	}

	private static float PixelsWorldLength(int pixelsCount, float worldPixelLength)
	{
		return pixelsCount * worldPixelLength;
	}

	/// <summary>
	/// Applies a Sobel filter with a 3x3 core to a gray scale image (edge detector).
	/// </summary>
	public static Color[] SobelFilter(Color[] grayScaleImage)
	{


		return new Color[1];
	}
}
