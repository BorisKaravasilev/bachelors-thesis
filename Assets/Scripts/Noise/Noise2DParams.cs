using System;

[Serializable]
public class Noise2DParams
{
	public float Scale;
	public float OffsetX;
	public float OffsetY;
	public Noise2DType Type;

	public Noise2DParams(Noise2DType type, float scale, float offsetX, float offsetY)
	{
		Type = Noise2DType.PerlinNoise;
		Scale = scale;
		OffsetX = offsetX;
		OffsetY = offsetY;
	}

	public override string ToString() => $"Noise parameters: (Type: {Enum.GetName(typeof(Noise2DType), Type)}" +
	                                     " Scale: {Scale}, OffsetX: {OffsetX}, OffsetY: {OffsetY})";
}