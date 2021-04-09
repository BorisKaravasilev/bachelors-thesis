using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sums all given textures into one.
/// </summary>
public class AddTextures : SingleTask
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

	protected override void ExecuteStep()
	{
		int textureIndex = ExecutedSteps;

		for (int i = 0; i < finalTexture.Length; i++)
		{
			finalTexture[i] += textures[textureIndex][i];
		}
	}

	protected override void GetInputFromPreviousStep()
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
