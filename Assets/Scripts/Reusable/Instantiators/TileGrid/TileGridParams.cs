using UnityEngine;

[System.Serializable]
public class TileGridParams
{
	[Range(20, 50)]
	public int tileSize = 20;
	[Range(0, 10)]
	public int xTiles = 0;
	[Range(0, 10)]
	public int zTiles = 0;

	public bool destroyFarTiles = true;
}