using System.Text;
using UnityEngine;
using xxHashSharp;

public static class RandomFromSeed
{
	public static uint PositionToHash(Vector3 position)
	{
		string stringCoords = position.x.ToString() + position.z.ToString();
		byte[] input = Encoding.UTF8.GetBytes(stringCoords);
		uint hash = xxHash.CalculateHash(input);
		return hash;
	}

	/// <summary>
	/// Return a random point between center [inclusive] and center +- radius [inclusive] in X and Z axis.
	/// Y coordinate is same as center.
	/// </summary>
	public static Vector3 RandomPointInRadius(Vector3 seedPosition, Vector3 center, float radius)
	{
		Random.InitState((int) PositionToHash(seedPosition));

		Vector3 randomPoint = new Vector3();
		do
		{
			randomPoint.x = center.x + Random.Range(-radius, radius);
			randomPoint.y = center.y;
			randomPoint.z = center.z + Random.Range(-radius, radius);
		} while (Vector3.Distance(randomPoint, center) > radius);

		return randomPoint;
	}

	/// <summary>
	/// Return a random integer between min [inclusive] and max [exclusive].
	/// </summary>
	public static int Range(Vector3 seedPosition, int min, int max)
	{
		Random.InitState((int)PositionToHash(seedPosition));
		return Random.Range(min, max);
	}

	/// <summary>
	/// Return a random float between 0.0f [inclusive] and 1.0f [inclusive].
	/// </summary>
	public static float UniformValue(Vector3 seedPosition)
	{
		Random.InitState((int)PositionToHash(seedPosition));
		return Random.value;
	}
}