using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandAreaTester : MonoBehaviour
{
	public Transform player;

	[Header("Grid Parameters")]
	[SerializeField]
	private GridParams gridParams;
	[SerializeField]
	private GridOffsetParams gridOffsetParams;

	[Header("Generation Steps Parameters")]
	[SerializeField]
	private GenerateTerrainNodesParams generateTerrainNodesParams;

	private Vector3 gridBottomLeft = new Vector3(-40, 0, -40);
	private Vector3 gridTopRight = new Vector3(40, 0, 40);
	private BoundingBox3D generatedWorldArea;

	private ObjectGrid<IslandArea, PerlinNoise2D> islandGrid;

	// Start is called before the first frame update
	void Start()
    {
		islandGrid = new ObjectGrid<IslandArea, PerlinNoise2D>(gridParams, gridOffsetParams);
		islandGrid.SetParent(gameObject.transform);
	}

    // Update is called once per frame
    void Update()
    {
		generatedWorldArea = new BoundingBox3D(gridBottomLeft + player.position, gridTopRight + player.position);
		List<IslandArea> newIslandAreas = islandGrid.InstantiateInBoundingBox(generatedWorldArea);

		newIslandAreas?.ForEach(area => area.AssignParams(true, generateTerrainNodesParams));
		//newIslandAreas?.ForEach(area => area.Generate());
		islandGrid.GetObjects()?.ForEach(area => area.GenerateStep());
    }

    void OnValidate()
    {
		islandGrid?.UpdateParameters(gridParams, gridOffsetParams);
    }
}
