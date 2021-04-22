using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates sea tiles on a square grid.
/// </summary>
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

	[Header("Output")]
	[SerializeField] private BoundingBox3DVariable boundingBox3D;

	private TileGrid<SeaTile> seaTileGrid;

	// Start is called before the first frame update
	void Start()
    {
		seaTileGrid = new TileGrid<SeaTile>(seaTileGridParams, gameObject.transform);
		UpdateFromPlayerPosition();
	}

    // Update is called once per frame
    void Update()
    {
	    UpdateFromPlayerPosition();
	}

    void OnValidate()
    {
	    seaTileGrid?.UpdateParameters(seaTileGridParams);
    }

	/// <summary>
	/// Instantiates new and destroys old sea tiles depending on players position.
	/// </summary>
    private void UpdateFromPlayerPosition()
    {
	    List<SeaTile> newSeaTiles = seaTileGrid.InstantiateTiles(player.position);
	    newSeaTiles?.ForEach(tile => tile.SetSeaMaterials(seaMaterial, seaBedMaterial));

	    if (previewMode)
		    newSeaTiles?.ForEach(tile => tile.GeneratePreview());
	    else
		    newSeaTiles?.ForEach(tile => tile.Generate(seaLevel));

	    UpdateBoundingBox(newSeaTiles);
    }

	private void UpdateBoundingBox(List<SeaTile> newSeaTiles)
    {
	    if (newSeaTiles?.Count > 0)
	    {
		    boundingBox3D.Value = seaTileGrid.GetBoundingBox();
		}
	}
}
