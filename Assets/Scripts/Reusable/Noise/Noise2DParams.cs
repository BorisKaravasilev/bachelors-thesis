[System.Serializable]
public class Noise2DParams
{
	public float Scale;
	public float OffsetX;
	public float OffsetY;

	public Noise2DParams(float scale, float offsetX, float offsetY)
	{
		Scale = scale;
		OffsetX = offsetX;
		OffsetY = offsetY;
	}

	public override string ToString() => $"Noise parameters: (Scale: {Scale}, OffsetX: {OffsetX}, OffsetY: {OffsetY})";
}