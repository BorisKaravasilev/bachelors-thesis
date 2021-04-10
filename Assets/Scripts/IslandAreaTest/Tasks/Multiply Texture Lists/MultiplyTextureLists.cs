using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplyTextureLists : SingleTask
{
	// Inputs from previous tasks
	private Func<List<Color[]>> getMultiplicandTextures;
	private Func<List<Color[]>> getMultiplierTextures;

	private List<Color[]> multiplicandTextures; // what gets multiplied   (first operand)
	private List<Color[]> multiplierTextures;   // what is the multiplier (second operand)

	// Outputs
	private List<Color[]> resultTextures;

	// Internal
	private int texturePixelCount;

	public MultiplyTextureLists(Func<List<Color[]>> getMultiplicandTextures, Func<List<Color[]>> getMultiplierTextures, int stepSize = 1)
	{
		StepSize = stepSize;

		this.getMultiplicandTextures = getMultiplicandTextures;
		this.getMultiplierTextures = getMultiplierTextures;

		resultTextures = new List<Color[]>();
	}

	public List<Color[]> GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return resultTextures;
	}

	protected override void ExecuteStep()
	{
		int textureIndex = ExecutedSteps;

		Color[] resultTexture = new Color[texturePixelCount];

		for (int pixelIndex = 0; pixelIndex < texturePixelCount; pixelIndex++)
		{
			resultTexture[pixelIndex] = multiplicandTextures[textureIndex][pixelIndex] * multiplierTextures[textureIndex][pixelIndex];
		}

		resultTextures.Add(resultTexture);
	}

	protected override void GetInputFromPreviousStep()
	{
		multiplicandTextures = getMultiplicandTextures();
		multiplierTextures = getMultiplierTextures();

		bool multiplicandsSameLength = TextureFunctions.SameLength(multiplicandTextures);
		bool multipliersSameLength = TextureFunctions.SameLength(multiplierTextures);

		if (multiplicandsSameLength && multipliersSameLength)
		{
			int multiplicandPixelCount = multiplicandTextures[0].Length;
			int multiplierPixelCount = multiplierTextures[0].Length;

			bool bothLenghtsEqual = multiplicandPixelCount == multiplierPixelCount;

			if (bothLenghtsEqual)
			{
				texturePixelCount = multiplierPixelCount;
			}
			else
			{
				Debug.LogError("All textures in the.");
				Enabled = false;
			}
		}
		else
		{
			Debug.LogError("The multiplicand or multiplier texture lists don't contain textures with the same pixel count.");
			Enabled = false;
		}
	}

	protected override void SetSteps()
	{
		TotalSteps = multiplicandTextures.Count;
		RemainingSteps = TotalSteps;
	}
}
