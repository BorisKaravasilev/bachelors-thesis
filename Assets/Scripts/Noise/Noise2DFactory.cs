using UnityEngine;

public static class Noise2DFactory
{
	public static Noise2D GetNoise(Noise2DParams noiseParams)
	{
		switch (noiseParams.Type)
		{
			case Noise2DType.PerlinNoise:
				return new PerlinNoise2D(noiseParams);
			default:
				string errorText = $"Noise2DFactory was given unimplemented noise type.";
				Debug.LogError(errorText);
				return new PerlinNoise2D(noiseParams);
		}
	}
}