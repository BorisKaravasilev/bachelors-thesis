using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaTile : Tile
{
	private Material seaMaterial;
	private Material seaBedMaterial;

	private GameObject sea;
	private GameObject seaBed;

	private float seaLevel = 1f;

	public SeaTile()
	{
		gameObject.name = "SeaTile";
	}

	public void SetSeaMaterials(Material seaMaterial, Material seaBedMaterial)
	{
		this.seaMaterial = seaMaterial;
		this.seaBedMaterial = seaBedMaterial;
	}

	public void Generate(float seaLevel)
	{
		this.seaLevel = seaLevel;

		seaBed = GeneratePlane(Size);
		seaBed.name = "SeaBed";
		seaBed.GetComponent<Renderer>().material = seaBedMaterial;

		sea = GeneratePlane(Size, seaLevel);
		sea.name = "Sea";
		sea.GetComponent<Renderer>().material = seaMaterial;
		Component.Destroy(sea.GetComponent<MeshCollider>());
	}
}
