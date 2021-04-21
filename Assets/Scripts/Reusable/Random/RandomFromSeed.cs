using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using xxHashSharp;
using Random = System.Random;

namespace MyRandom
{
	public class RandomFromSeed
	{
		private Random random;

		public RandomFromSeed(Vector3 seedPosition, string seedString = "")
		{
			seedString += seedPosition.ToString("F3");
			int seed = (int) StringToHash(seedString);
			random = new System.Random(seed);
		}

		private uint StringToHash(string str)
		{
			byte[] stringBytes = Encoding.UTF8.GetBytes(str);
			uint hash = xxHash.CalculateHash(stringBytes);
			return hash;
		}

		/// <summary>
		/// Return a random point between center [inclusive] and center +- radius [exclusive].
		/// </summary>
		public Vector2 Point2DInRadius(Vector2 center, float radius)
		{
			Vector2 randomPoint = new Vector3();

			float randRadius = NextFloat() * radius;
			Vector2 randDirection = new Vector2(NextFloat() - 0.5f, NextFloat() - 0.5f);
			randomPoint = randDirection.normalized * randRadius;

			return randomPoint;
		}

		/// <summary>
		/// Returns a random floating point number that is greater or equal to 0.0, and smaller than 1.0.
		/// </summary>
		public float NextFloat()
		{
			return (float) random.NextDouble();
		}

		/// <summary>
		/// Return a random integer between min [inclusive] and max [exclusive].
		/// </summary>
		public int Next(int min, int max)
		{
			return random.Next(min, max);
		}

		public bool NextBool(float trueProbability = 0.5f)
		{
			return NextFloat() < trueProbability;
		}

		/// <summary>
		/// Picks randomly an item from a list respecting items probabilities.
		/// </summary>
		public IHasProbability PickItem(List<IHasProbability> items)
		{
			float probabilitySum  = GetProbabilitySum(items);

			float cumulativeProbability = 0f;
			float randomNumber = NextFloat();

			foreach (IHasProbability item in items)
			{
				cumulativeProbability += item.Probability / probabilitySum; // Normalize by division
				if (randomNumber <= cumulativeProbability) return item;
			}

			return null;
		}

		/// <summary>
		/// Returns a sum of probabilities of all items in the given list.
		/// </summary>
		private float GetProbabilitySum(List<IHasProbability> items)
		{
			float sum = 0f;
			items.ForEach(item => sum += item.Probability);
			return sum;
		}
	}
}