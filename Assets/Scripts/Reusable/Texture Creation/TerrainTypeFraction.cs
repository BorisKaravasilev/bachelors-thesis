/// <summary>
/// Terrain type with a specified amount used for blending.
/// </summary>
public class TerrainTypeFraction
{
	public TerrainType Type { get; private set; }
	public float Amount { get; set; } // 0 to 1

	public TerrainTypeFraction(TerrainType type, float amount)
	{
		Type = type;
		Amount = amount;
	}

	public override string ToString()
	{
		return $"(Type: {Type.Name}, Amount: {Amount})";
	}
}