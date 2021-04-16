using System.Collections.Generic;
using UnityEngine;

public class IslandAreaTester : MonoBehaviour
{
	[SerializeField] private Transform player;
	[SerializeField] private SeaTileGrid seaTileGrid;
	[SerializeField] private TextAsset islandNames;

	[Header("Island Grid")]
	[SerializeField] private bool debugIslandAreas = false;
	[SerializeField] private Material meshMaterial;
	[SerializeField] private Material previewObjectMaterial;
	[SerializeField] private Material previewTextureMaterial;
	[SerializeField] private GridParams islandGridParams;
	[SerializeField] private GridOffsetParams islandGridOffsetParams;

	[Header("Generation Steps Parameters")]
	[Range(10, 100)]
	[SerializeField] private int texturePixelsPerUnit = 10;
	[SerializeField] private TerrainNodesParams terrainNodesParams;
	[SerializeField] private bool previewProgress = true;
	[SerializeField] private bool generateSequentially = true;
	[SerializeField] private bool generateInSteps = true;
	[Range(0f, 1f)]
	[SerializeField] private float visualStepTime = 0f;

	//private TileGrid seaTileGrid;
	private BoundingBox3D generatedWorldArea;
	private ObjectGrid<IslandArea, PerlinNoise2D> islandGrid;

	// Start is called before the first frame update
	void Start()
	{
		CheckMaterialsAssigned();

		// Islands grid
		islandGrid = new ObjectGrid<IslandArea, PerlinNoise2D>(islandGridParams, islandGridOffsetParams, gameObject.transform);
	}

    // Update is called once per frame
    void Update()
    {
	    generatedWorldArea = seaTileGrid.GetBoundingBox();
		List<IslandArea> newIslandAreas = islandGrid.InstantiateInBoundingBox(generatedWorldArea);
		List<IslandArea> islandAreas = islandGrid.GetObjects();

		GenerateIslandAreas(islandAreas);
    }

    void OnValidate()
    {
		islandGrid?.UpdateParameters(islandGridParams, islandGridOffsetParams);
    }

	/// <summary>
	/// Generates or initializes one or all island areas depending on "generateSequentially".
	/// </summary>
	private void GenerateIslandAreas(List<IslandArea> islandAreas)
	{
		if (generateSequentially)
		{
			IslandArea closestIslandArea = GetClosestNotFinishedIslandArea(islandAreas);
			if (closestIslandArea != null) InitOrGenerateArea(closestIslandArea);
		}
		else
		{
			foreach (IslandArea islandArea in islandAreas)
			{
				InitOrGenerateArea(islandArea);
			}
		}
	}

	private IslandArea GetClosestNotFinishedIslandArea(List<IslandArea> islandAreas)
	{
		float distanceToClosest = float.MaxValue;
		IslandArea closestIslandArea = null;

		foreach (IslandArea islandArea in islandAreas)
		{
			if (!islandArea.Initialized || !islandArea.Finished)
			{
				float distanceToArea = Vector3.Distance(islandArea.Position, player.position);

				if (distanceToArea < distanceToClosest)
				{
					distanceToClosest = distanceToArea;
					closestIslandArea = islandArea;
				}
			}
		} 

		return closestIslandArea;
	}

	/// <summary>
	/// Returns  true if area got initialized or generated.
	/// </summary>
	/// <param name="islandArea"></param>
	/// <returns></returns>
	private bool InitOrGenerateArea(IslandArea islandArea)
	{
		if (!islandArea.Initialized)
		{
			islandArea.Init(previewProgress, visualStepTime, texturePixelsPerUnit, terrainNodesParams, meshMaterial, previewObjectMaterial, previewTextureMaterial, islandNames);
			islandArea.DebugMode = debugIslandAreas;
			return true;
		}

		return GenerateIslandArea(islandArea);
	}

	/// <summary>
	/// Returns true if the given island area was not finished and generation step got executed.
	/// </summary>
	private bool GenerateIslandArea(IslandArea islandArea)
	{
		if (!islandArea.Finished)
		{
			if (generateInSteps)
				islandArea.GenerateStep();
			else
			{
				islandArea.Generate();
			}

			return true;
		}

		return false;
	}

	private void CheckMaterialsAssigned()
	{
		if (meshMaterial == null || previewObjectMaterial == null || previewTextureMaterial == null)
		{
			Debug.LogError("All materials are not assigned to \"Island Area Tester\".");
		}
	}
}
