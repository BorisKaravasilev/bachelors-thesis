using UnityEngine;

public struct PixelBoundingBox
{
	public PixelBoundingBox(Vector2Int bottomLeft, Vector2Int topRight)
	{
		BottomLeft = bottomLeft;
		TopRight = topRight;
	}

	public Vector2Int BottomLeft;
	public Vector2Int TopRight;

	public override string ToString() => $"({BottomLeft.x}, {BottomLeft.y}), ({TopRight.x}, {TopRight.y})";

	/// <summary>
	/// Returns true when the pixel's position is within the bounding box (including the edges).
	/// </summary>
	public bool Encloses(Vector2Int pixelCoords)
	{
		bool withinXBoundaries = pixelCoords.x >= BottomLeft.x && pixelCoords.x <= TopRight.x;
		bool withinZBoundaries = pixelCoords.y >= BottomLeft.y && pixelCoords.y <= TopRight.y;

		return withinXBoundaries && withinZBoundaries;
	}
}