using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class TileGrid<TTile> where TTile : Tile, new()
{
	private TileGridParams parameters;

	private List<Tile> tiles;
	private Vector3 playerTilePosition = new Vector3(0, 0, 0);

	private Transform parent;

	public TileGrid(TileGridParams parameters, Transform parent)
	{
		this.parameters = parameters;
		this.parent = parent;

		tiles = new List<Tile>();
	}

	/// <summary>
	/// Instantiates new tiles when player moves to a different tile.
	/// </summary>
	public List<TTile> InstantiateTiles(Vector3 playerPosition)
	{
		List<TTile> newlyInstantiated = new List<TTile>();
		bool playerChangedTile = true;

		Vector3 newPlayerTilePosition = FindTilePosition(playerPosition);

		if (tiles.Count != 0)
		{
			playerChangedTile = playerTilePosition != newPlayerTilePosition;
		}

		playerTilePosition = newPlayerTilePosition;

		if (playerChangedTile)
		{
			newlyInstantiated = GenerateTilesAroundPlayer();
			if (parameters.destroyFarTiles) DestroyTilesOutOfRange();
		}

		return newlyInstantiated;
	}

	public void UpdateParameters(TileGridParams parameters)
	{
		this.parameters = parameters;
		DestroyAllTiles();
	}

	public BoundingBox3D GetBoundingBox()
	{
		Vector3 playerTileUpperRight = playerTilePosition;
		playerTileUpperRight.x += parameters.tileSize;
		playerTileUpperRight.z += parameters.tileSize;

		BoundingBox3D boundingBox = new BoundingBox3D(playerTilePosition, playerTileUpperRight);

		foreach (Tile tile in tiles)
		{
			boundingBox.ExtendLowerLeft(tile.GetPosition());

			Vector3 tileUpperRight = tile.GetPosition();
			tileUpperRight.x += parameters.tileSize;
			tileUpperRight.z += parameters.tileSize;

			boundingBox.ExtendUpperRight(tileUpperRight);
		}

		return boundingBox;
	}

	private List<TTile> GenerateTilesAroundPlayer()
	{
		List<TTile> newlyInstantiated = new List<TTile>();

		// Generate tiles around the player
		for (int offsetX = -parameters.xTiles; offsetX <= parameters.xTiles; offsetX++)
		{
			for (int offsetZ = -parameters.zTiles; offsetZ <= parameters.zTiles; offsetZ++)
			{
				Vector3 currentTilePosition = playerTilePosition;
				currentTilePosition.x += offsetX * parameters.tileSize;
				currentTilePosition.z += offsetZ * parameters.tileSize;

				if (!IsTileGenerated(currentTilePosition))
				{
					TTile newTile = new TTile();
					newTile.SetParameters(parameters.tileSize, currentTilePosition, parent);
					tiles.Add(newTile);
					newlyInstantiated.Add(newTile);
				}
			}
		}

		return newlyInstantiated;
	}

	private void DestroyTilesOutOfRange()
	{
		List<Tile> tilesToRemove = new List<Tile>();

		foreach (Tile tile in tiles)
		{
			Vector3 currentTilePosition = tile.GetPosition();

			if (!IsTileInRange(currentTilePosition))
			{
				tile.Destroy();
				tilesToRemove.Add(tile);
			}
		}

		RemoveTilesFromList(tilesToRemove);
	}

	private bool IsTileInRange(Vector3 tilePosition)
	{
		if (playerTilePosition.x < tilePosition.x - parameters.xTiles * parameters.tileSize)
		{
			return false;
		}
		else if (playerTilePosition.z < tilePosition.z - parameters.zTiles * parameters.tileSize)
		{
			return false;
		}
		else if (playerTilePosition.x >= tilePosition.x + (parameters.xTiles + 1) * parameters.tileSize)
		{
			return false;
		}
		else if (playerTilePosition.z >= tilePosition.z + ((parameters.zTiles + 1) * parameters.tileSize))
		{
			return false;
		}

		return true;
	}

	private void RemoveTilesFromList(List<Tile> tilesToRemove)
	{
		foreach (Tile tile in tilesToRemove)
		{
			tiles.Remove(tile);
		}
	}

	private bool IsTileGenerated(Vector3 checkedPosition)
	{
		bool isGenerated = false;

		foreach (var tile in tiles)
		{
			if (tile.GetPosition() == checkedPosition)
			{
				isGenerated = true;
				break;
			}
		}

		return isGenerated;
	}

	/// <summary>
	/// Finds the origin of the tile that the player is currently in.
	/// - Tile origin is in the lower left corner of the tile.
	/// - Tile size has to be integer (Because of modulo operation).
	/// </summary>
	private Vector3 FindTilePosition(Vector3 playerPosition)
	{
		int playerX = Mathf.FloorToInt(playerPosition.x);
		int playerZ = Mathf.FloorToInt(playerPosition.z);

		int playerOriginOffsetX = playerX % parameters.tileSize;
		int playerOriginOffsetZ = playerZ % parameters.tileSize;

		if (playerOriginOffsetX < 0)
		{
			playerOriginOffsetX = parameters.tileSize + playerOriginOffsetX;
		}

		if (playerOriginOffsetZ < 0)
		{
			playerOriginOffsetZ = parameters.tileSize + playerOriginOffsetZ;
		}

		float gridHeight = parent.position.y;
		Vector3 tileOrigin = new Vector3(playerX - playerOriginOffsetX, gridHeight, playerZ - playerOriginOffsetZ);

		return tileOrigin;
	}

	private void DestroyAllTiles()
	{
		foreach (Tile tile in tiles)
		{
			tile.Destroy();
		}

		tiles.Clear();
	}
}