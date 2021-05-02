using System;
using System.Collections;
using System.Collections.Generic;
using TaskManagement;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Sums all given textures into one (same size required).
	/// </summary>
	public class AddTextures : DividableTask
	{
		// Inputs from previous task
		private Func<List<Color[]>> getTextures;
		private List<Color[]> textures;

		// Outputs
		private Color[] finalTexture;

		public AddTextures(Func<List<Color[]>> getTextures)
		{
			Name = "Add Textures";
			this.getTextures = getTextures;
		}

		public Color[] GetResult()
		{
			if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
			return finalTexture;
		}

		public List<Color[]> GetResultInList()
		{
			if (!Finished) Debug.LogWarning($"\"GetResultInList()\" called on {Name} task before finished.");
			List<Color[]> resultWrapperList = new List<Color[]> {finalTexture};
			return resultWrapperList;
		}

		protected override void ExecuteStep()
		{
			int textureIndex = ExecutedSteps;

			for (int i = 0; i < finalTexture.Length; i++)
			{
				finalTexture[i] += textures[textureIndex][i];
			}
		}

		protected override void GetInputFromPreviousTask()
		{
			textures = getTextures();

			if (!TextureFunctions.SameLength(textures))
			{
				Debug.LogError("The added textures don't have the same number of pixels.");
				Enabled = false;
			}
			else
			{
				finalTexture = new Color[textures[0].Length];
			}
		}

		protected override void SetSteps()
		{
			if (textures != null)
			{
				// 1 less additions than total operands
				TotalSteps = textures.Count;
				RemainingSteps = TotalSteps;
			}
		}
	}
}