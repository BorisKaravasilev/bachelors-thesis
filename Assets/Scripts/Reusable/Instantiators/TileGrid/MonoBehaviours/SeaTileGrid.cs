using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaTileGrid : MonoBehaviour
{
	[SerializeField] private Transform player;

	[Header("Materials")]
	[SerializeField] private Material seaMaterial;
	[SerializeField] private Material seaBedMaterial;

	[Header("Grid Parameters")]
	[SerializeField] private TileGridParams seaTileGridParams;
	[SerializeField] private float seaLevel = 1f;
	[SerializeField] private bool previewMode = false;

	private TileGrid<SeaTile> seaTileGrid;

	// Start is called before the first frame update
	void Start()
    {
		seaTileGrid = new TileGrid<SeaTile>(seaTileGridParams, gameObject.transform);
	}

    // Update is called once per frame
    void Update()
    {
	    List<SeaTile> newSeaTiles = seaTileGrid.InstantiateTiles(player.position);
	    newSeaTiles?.ForEach(tile => tile.SetSeaMaterials(seaMaterial, seaBedMaterial));

		if (previewMode)
		    newSeaTiles?.ForEach(tile => tile.GeneratePreview());
		else
		    newSeaTiles?.ForEach(tile => tile.Generate(seaLevel));
	}

    void OnValidate()
    {
	    seaTileGrid?.UpdateParameters(seaTileGridParams);
    }

    public BoundingBox3D GetBoundingBox()
    {
	    return seaTileGrid.GetBoundingBox();
    }
}
