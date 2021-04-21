using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	public class TerrainNode
	{
		public Vector3 Position { get; private set; }
		public TerrainType Type { get; private set; }
		public float Radius { get; private set; }
		public bool IsDominant { get; private set; }

		public TerrainNode(Vector3 position, TerrainType type, float radius, bool isDominant)
		{
			Position = position;
			Type = type;
			Radius = radius;
			IsDominant = isDominant;
		}

		public PixelBoundingBox GetPixelBoundingBox(int resolution, float areaRadius)
		{
			Vector3 areaLocalBottomLeft = Position;
			areaLocalBottomLeft.x -= Radius;
			areaLocalBottomLeft.z -= Radius;

			Vector3 areaLocalTopRight = Position;
			areaLocalTopRight.x += Radius;
			areaLocalTopRight.z += Radius;

			Vector2Int areaPixelBottomLeft =
				TextureFunctions.LocalPositionToPixel(areaLocalBottomLeft, resolution, areaRadius);
			Vector2Int areaPixelTopRight =
				TextureFunctions.LocalPositionToPixel(areaLocalTopRight, resolution, areaRadius);

			return new PixelBoundingBox(areaPixelBottomLeft, areaPixelTopRight);
		}
	}
}