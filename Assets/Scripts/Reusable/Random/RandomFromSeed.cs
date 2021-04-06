using UnityEngine;

public static class RandomFromSeed
{
	/// <summary>
	/// Return a random point between center [inclusive] and center + radius [inclusive] in X and Z axis.
	/// Y coordinate is same as center.
	/// </summary>
	public static Vector3 RandomPointInRadius(Vector3 seedPosition, Vector3 center, float radius)
	{
		//Random.InitState(PositionToIntSeed(seedPosition));
		float x = center.x + Random.Range(-radius, radius);
		float y = center.y;
		float z = center.z + Random.Range(-radius, radius);

		return new Vector3(x, y, z);
	}

	/// <summary>
	/// Return a random integer between min [inclusive] and max [exclusive].
	/// </summary>
	public static int Range(Vector3 seedPosition, int min, int max)
	{
		//Random.InitState(PositionToIntSeed(seedPosition));
		return Random.Range(min, max);
	}

	private static int PositionToIntSeed(Vector3 seedPosition)
	{
		return (int) seedPosition.x + (int)seedPosition.z;
	}
}