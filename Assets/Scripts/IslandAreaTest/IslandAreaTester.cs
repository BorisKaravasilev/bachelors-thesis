using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IslandAreaTester : MonoBehaviour
{
	[SerializeField] private SeaTileGrid seaTileGrid;

	[Header("Island Grid")]
	[SerializeField] private GridParams islandGridParams;
	[SerializeField] private GridOffsetParams islandGridOffsetParams;

	[Header("Generation Steps Parameters")]
	[SerializeField] private GenerateTerrainNodesParams generateTerrainNodesParams;
	[SerializeField] private bool previewProgress = true;
	[SerializeField] private bool generateSequentially = true;
	[SerializeField] private bool generateInSteps = true;

	//private TileGrid seaTileGrid;
	private BoundingBox3D generatedWorldArea;
	private ObjectGrid<IslandArea, PerlinNoise2D> islandGrid;

	// Start is called before the first frame update
	void Start()
    {
		// Islands grid
		islandGrid = new ObjectGrid<IslandArea, PerlinNoise2D>(islandGridParams, islandGridOffsetParams, gameObject.transform);
	}

    // Update is called once per frame
    void Update()
    {
	    generatedWorldArea = seaTileGrid.GetBoundingBox();
		List<IslandArea> newIslandAreas = islandGrid.InstantiateInBoundingBox(generatedWorldArea);
		List<IslandArea> islandAreas = islandGrid.GetObjects();

		if (generateSequentially)
			GenerateNextIslandArea(islandAreas);
		else
			GenerateAllIslandAreas(islandAreas);
    }

    void OnValidate()
    {
		islandGrid?.UpdateParameters(islandGridParams, islandGridOffsetParams);
    }

	private void GenerateNextIslandArea(List<IslandArea> islandAreas)
    {
	    if (islandAreas.Count > 0)
	    {
		    foreach (IslandArea islandArea in islandAreas)
		    {
			    if (!islandArea.ParamsAssigned)
			    {
				    islandArea.AssignParams(previewProgress, generateTerrainNodesParams);
			    }
			    else if (!islandArea.Finished)
			    {
				    if (generateInSteps)
					    islandArea.GenerateStep();
					else
					    islandArea.Generate();
				    break;
			    }
		    }
	    }
	}

	private void GenerateAllIslandAreas(List<IslandArea> islandAreas)
	{
		foreach (IslandArea islandArea in islandAreas)
		{
			if (!islandArea.ParamsAssigned)
			{
				islandArea.AssignParams(previewProgress, generateTerrainNodesParams);
			}
			else if (!islandArea.Finished)
			{
				if (generateInSteps)
					islandArea.GenerateStep();
				else
					islandArea.Generate();
			}
		}
	}
}
