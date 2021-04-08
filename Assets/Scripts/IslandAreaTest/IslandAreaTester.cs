using System.Collections.Generic;
using UnityEngine;

public class IslandAreaTester : MonoBehaviour
{
	[SerializeField] private SeaTileGrid seaTileGrid;

	[Header("Island Grid")]
	[SerializeField] private GridParams islandGridParams;
	[SerializeField] private GridOffsetParams islandGridOffsetParams;

	[Header("Generation Steps Parameters")]
	[SerializeField] private GenerateTerrainNodesParams generateTerrainNodesParams;

	//private TileGrid seaTileGrid;
	private BoundingBox3D generatedWorldArea;
	private ObjectGrid<IslandArea, PerlinNoise2D> islandGrid;

	// Start is called before the first frame update
	void Start()
    {
		// Islands grid
		islandGrid = new ObjectGrid<IslandArea, PerlinNoise2D>(islandGridParams, islandGridOffsetParams);
		islandGrid.SetParent(gameObject.transform);
	}

    // Update is called once per frame
    void Update()
    {
	    generatedWorldArea = seaTileGrid.GetBoundingBox();
		List<IslandArea> newIslandAreas = islandGrid.InstantiateInBoundingBox(generatedWorldArea);

		newIslandAreas?.ForEach(area => area.AssignParams(true, generateTerrainNodesParams));
		islandGrid.GetObjects()?.ForEach(area => area.GenerateStep());
    }

    void OnValidate()
    {
		islandGrid?.UpdateParameters(islandGridParams, islandGridOffsetParams);
    }
}
