using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Blend of multiple terrain types.
	/// </summary>
	public class TerrainBlend
	{
		public float Amount { get; set; } // For blending with other terrain blends
		public float FreeCapacity { get; private set; } // 0 to 1
		public List<TerrainTypeFraction> TerrainFractions { get; private set; }

		public TerrainBlend()
		{
			Amount = 1f;
			FreeCapacity = 1f;
			TerrainFractions = new List<TerrainTypeFraction>();
		}

		public override string ToString()
		{
			string fractions = "";

			foreach (var fraction in TerrainFractions)
			{
				fractions += fraction + ", ";
			}

			return $"(Amount: {Amount}, Free Capacity: {FreeCapacity}, Terrain Fractions: {fractions})";
		}

		public void AddFractions(List<TerrainTypeFraction> terrainFractions)
		{
			terrainFractions.ForEach(AddFraction);
		}

		public void AddFraction(TerrainTypeFraction fraction)
		{
			AddTerrainType(fraction.Type, fraction.Amount);
		}

		public void AddTerrainType(TerrainType type, float amount)
		{
			float addedAmount = amount;

			if (addedAmount > FreeCapacity) addedAmount = FreeCapacity;

			TerrainTypeFraction existingFraction = FindTerrainTypeFraction(type);
			AddOrIncrementTerrainFraction(type, addedAmount, existingFraction);
		}

		public Color GetColor()
		{
			List<Color> blendedColors = new List<Color>();

			for (int i = 0; i < TerrainFractions.Count; i++)
			{
				Color colorToBlend = TerrainFractions[i].Type.Color * TerrainFractions[i].Amount;
				colorToBlend.a = 1f;
				blendedColors.Add(colorToBlend);
			}

			Color blendedColor = Color.black;

			blendedColors.ForEach(colorToBlend => blendedColor += colorToBlend);
			return blendedColor;
		}

		public List<TerrainTypeFraction> GetFractionsMultipliedByAmount()
		{
			List<TerrainTypeFraction> multipliedFractions = new List<TerrainTypeFraction>();

			foreach (TerrainTypeFraction fraction in TerrainFractions)
			{
				TerrainTypeFraction multiplied = new TerrainTypeFraction(fraction.Type, fraction.Amount * Amount);
				multipliedFractions.Add(multiplied);
			}

			return multipliedFractions;
		}

		private void AddOrIncrementTerrainFraction(TerrainType type, float addedAmount,
			TerrainTypeFraction existingFraction = null)
		{
			if (existingFraction != null)
			{
				existingFraction.Amount += addedAmount;
			}
			else
			{
				TerrainTypeFraction newFraction = new TerrainTypeFraction(type, addedAmount);
				TerrainFractions.Add(newFraction);
			}

			FreeCapacity -= addedAmount;
		}

		private TerrainTypeFraction FindTerrainTypeFraction(TerrainType searchedType)
		{
			foreach (TerrainTypeFraction fraction in TerrainFractions)
			{
				if (fraction.Type == searchedType) return fraction;
			}

			return null;
		}
	}
}