using UnityEngine;

[System.Serializable]
public struct BoundingBox3D
{
	public BoundingBox3D(Vector3 bottomLeft, Vector3 topRight)
	{
		BottomLeft = bottomLeft;
		TopRight = topRight;
	}

	public Vector3 BottomLeft;
	public Vector3 TopRight;
	public Vector3 Center => (BottomLeft + TopRight) / 2;

	public override string ToString() => $"({BottomLeft.x}, {BottomLeft.y}, {BottomLeft.z}), ({TopRight.x}, {TopRight.y}, {TopRight.z})";

	public void ExtendLowerLeft(Vector3 point)
	{
		if (point.x < BottomLeft.x)
		{
			BottomLeft.x = point.x;
		}

		if (point.y < BottomLeft.y)
		{
			BottomLeft.y = point.y;
		}

		if (point.z < BottomLeft.z)
		{
			BottomLeft.z = point.z;
		}
	}

	public void ExtendUpperRight(Vector3 point)
	{
		if (point.x > TopRight.x)
		{
			TopRight.x = point.x;
		}

		if (point.y > TopRight.y)
		{
			TopRight.y = point.y;
		}

		if (point.z > TopRight.z)
		{
			TopRight.z = point.z;
		}
	}

	/// <summary>
	/// Returns true when the position's X and Z are within the bounding box.
	/// </summary>
	public bool EnclosesInXZ(Vector3 position)
	{
		bool withinXBoundaries = position.x >= BottomLeft.x && position.x < TopRight.x;
		bool withinZBoundaries = position.z >= BottomLeft.z && position.z < TopRight.z;

		return withinXBoundaries && withinZBoundaries;
	}
}