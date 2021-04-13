﻿using UnityEngine;

[System.Serializable]
public class GridParams
{
	[Range(1, 40)]
	public int spacing = 15;

	[Range(0.5f, 20f)]
	public float maxObjectRadius = 5;

	[Range(1, 1000)]
	public int objectCountLimit = 500;

	public bool destroyFarObjects = true;
}