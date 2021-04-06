using UnityEngine;

[System.Serializable]
public class GridParams
{
	[Range(1, 20)]
	public int spacing;

	[Range(0.5f, 10f)]
	public float maxObjectRadius;

	[Range(100, 1000)]
	public int objectCountLimit;

	public bool destroyFarObjects;
}