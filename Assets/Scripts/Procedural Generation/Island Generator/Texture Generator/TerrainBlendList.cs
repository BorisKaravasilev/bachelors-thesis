using System.Collections.Generic;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Blends a list of terrain blends into single one.
	/// </summary>
	public class TerrainBlendList
	{
		public float FreeCapacity { get; private set; } // 0 to 1
		public List<TerrainBlend> List { get; private set; }

		public TerrainBlendList()
		{
			FreeCapacity = 1f;
			List = new List<TerrainBlend>();
		}

		public void Add(TerrainBlend terrainBlend)
		{
			if (terrainBlend.Amount > FreeCapacity) terrainBlend.Amount = FreeCapacity;

			if (terrainBlend.Amount > 0f)
			{
				List.Add(terrainBlend);
				FreeCapacity -= terrainBlend.Amount;
			}
		}

		public TerrainBlend Blend()
		{
			TerrainBlend terrainBlend = new TerrainBlend();

			for (int i = 0; i < List.Count; i++)
			{
				List<TerrainTypeFraction> multipliedFractions = List[i].GetFractionsMultipliedByAmount();
				terrainBlend.AddFractions(multipliedFractions);
			}

			return terrainBlend;
		}
	}
}