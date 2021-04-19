using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObjectPlacement.JitteredGrid;
using UnityEngine;

/// <summary>
/// Generates positions on an island area.
/// </summary>
public class GenerateObjectPositions : SingleTask
{
	// Inputs
	private PlacedObjectParams placedObjectParams;
	private int resolution;
	private float radius;

	// Inputs from previous task
	private Func<Color[]> getHeightmap;
	private Color[] heightmap;

	private Func<TerrainBlend[]> getTerrainTypesAtPixels;
	private TerrainBlend[] terrainTypesAtPixels;

	// Internal
	private BoundingBox3D boundingBox;
	private JitteredGrid jitteredGrid;

	// Output
	private List<GridPoint> positions;

	public GenerateObjectPositions(PlacedObjectParams placedObjectParams, int resolution, float radius,
		GridParams gridParams, OffsetParams offsetParams, Func<Color[]> getHeightmap,
		Func<TerrainBlend[]> getTerrainTypesAtPixels)
	{
		this.placedObjectParams = placedObjectParams;
		this.resolution = resolution;
		this.radius = radius;
		this.getHeightmap = getHeightmap;
		this.getTerrainTypesAtPixels = getTerrainTypesAtPixels;

		Vector3 areaBottomLeft = new Vector3(-radius, 0f, -radius);
		Vector3 areaTopRight = new Vector3(radius, 0f, radius);
		boundingBox = new BoundingBox3D(areaBottomLeft, areaTopRight);

		jitteredGrid = new JitteredGrid(gridParams, offsetParams);
	}

	public List<GridPoint> GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return positions;
	}

	protected override void ExecuteStep()
	{
		positions = jitteredGrid.GetPointsInBoundingBox(boundingBox);
		positions.RemoveAll(IsPositionBad);
		SetPositionsHeights();
	}

	protected override void GetInputFromPreviousStep()
	{
		heightmap = getHeightmap();
		terrainTypesAtPixels = getTerrainTypesAtPixels();
	}

	protected override void SetSteps()
	{
		TotalSteps = 1;
		RemainingSteps = TotalSteps;
	}

	private void SetPositionsHeights()
	{
		foreach (GridPoint position in positions)
		{
			Vector2Int pixelCoords = TextureFunctions.LocalPositionToPixel(position.Position, resolution, radius);
			int pixelIndex = TextureFunctions.CoordsToArrayIndex(resolution, resolution, pixelCoords);

			Vector3 liftedPosition = position.Position;
			liftedPosition.y = heightmap[pixelIndex].r;
			position.Position = liftedPosition;
		}
	}

	private bool IsPositionBad(GridPoint position)
	{
		Vector2Int pixelCoords = TextureFunctions.LocalPositionToPixel(position.Position, resolution, radius);
		int pixelIndex = TextureFunctions.CoordsToArrayIndex(resolution, resolution, pixelCoords);

		return !HeightOk(pixelIndex) || !TerrainTypeOk(pixelIndex));
	}

	private bool HeightOk(int pixelIndex)
	{
		float pixelHeight = heightmap[pixelIndex].r;
		bool tooLow = pixelHeight < placedObjectParams.MinHeight;
		bool tooHigh = pixelHeight > placedObjectParams.MaxHeight;
		return !tooLow && !tooHigh;
	}

	private bool TerrainTypeOk(int pixelIndex)
	{
		List<TerrainTypeFraction> terrainTypes = terrainTypesAtPixels[pixelIndex].TerrainFractions;

		foreach (TerrainTypeFraction typeFraction in terrainTypes)
		{
			TerrainTypeFraction requiredTypeFraction = placedObjectParams.MinimumTerrainFraction;

			bool typeOk = typeFraction.Type == requiredTypeFraction.Type;
			bool minAmountOk = typeFraction.Amount >= requiredTypeFraction.Amount;

			if (typeOk && minAmountOk) return true;
		}

		return false;
	}
}