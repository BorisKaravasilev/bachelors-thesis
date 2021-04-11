using UnityEngine;

public class BlendingRegion
{
	public float BottomBorder { get; set; }
	public float TopBorder { get; set; }

	public Color BottomColor { get; set; }
	public Color TopColor { get; set; }

	public BlendingRegion(float bottomBorder, float topBorder, Color bottomColor, Color topColor)
	{
		BottomBorder = bottomBorder;
		TopBorder = topBorder;

		BottomColor = bottomColor;
		TopColor = topColor;
	}

	public bool IsWithin(float height)
	{
		bool belowUpperBorder = height <= TopBorder;
		bool aboveBottomBorder = height >= BottomBorder;

		return belowUpperBorder && aboveBottomBorder;
	}

	public Color GetColor(float height)
	{
		float fullRange = TopBorder - BottomBorder;
		float normalizedHeight = (height - BottomBorder) / fullRange;
		Color blendedColor = Color.Lerp(BottomColor, TopColor, normalizedHeight);

		return blendedColor;
	}
}