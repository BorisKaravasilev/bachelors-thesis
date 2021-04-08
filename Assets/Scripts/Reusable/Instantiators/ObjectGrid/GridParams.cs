using UnityEngine;

[System.Serializable]
public class GridParams
{
	[Range(1, 20)]
	public int spacing = 15;

	[Range(0.5f, 10f)]
	public float maxObjectRadius = 5;

	[Range(100, 1000)]
	public int objectCountLimit = 500;

	public bool destroyFarObjects = true;
}