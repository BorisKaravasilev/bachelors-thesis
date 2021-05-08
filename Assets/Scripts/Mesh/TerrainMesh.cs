using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a terrain mesh with the option to visualize the progress.
/// </summary>
public class TerrainMesh
{
	// Height Map
	public Color[] HeightMap { get; set; }
	public Vector2Int Resolution { get; private set; }

	// Terrain data
	public Mesh Mesh { get; set; }
	private int[] triangles;
	private Vector2[] uvs;

	public Vector3 Dimensions { get; set; }
	public Vector2Int VerticesCount { get; set; }
	public Vector3[] Vertices { get; set; }
	public Material VerticesPreviewMaterial { get; private set; }
	private Vector2 verticesSpacing;
	private float verticesTranslationProgress;

	// Generation progress
	public int TotalVerticesToGenerate { get; }
	private int generatedVertices;

	public int TotalTrianglesToGenerate { get; private set; }
	public int GeneratedTriangles { get; private set; }

	public int GeneratedVertices
	{
		get { return generatedVertices; }
	}

	public bool AllVerticesGenerated
	{
		get { return TotalVerticesToGenerate == GeneratedVertices; }
	}

	public bool VerticesReachedTarget
	{
		get { return verticesTranslationProgress == 1f; }
	}

	public bool MeshGenerationCompleted
	{
		get { return GeneratedTriangles == TotalVerticesToGenerate; }
	}

	// Visualization
	public Transform Parent { get; }

	public GameObject MeshGO { get; private set; }
	private const string MESH_GO_NAME = "Terrain Mesh";
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;

	public GameObject VerticesParent { get; private set; }
	private const string VERTICES_PARENT_NAME = "VertexVisualizations";
	public List<GameObject> VertexVisualizations { get; set; }
	private Vector3[] oldVertices;
	private Vector3[] targetVertices;
	private Color[] oldVerticesColors;
	private Color[] targetVerticesColors;

	public TerrainMesh(Transform parent, Color[] heightMap, Vector2Int resolution, Vector3 dimensions, Vector2Int verticesCount, Material verticesPreviewMaterial)
	{
		// Height Map
		HeightMap = heightMap;
		Resolution = resolution;

		// Terrain data
		Mesh = new Mesh();
		triangles = new int[(verticesCount.x - 1) * (verticesCount.y - 1) * 2 * 3];
		uvs = new Vector2[verticesCount.x * verticesCount.y];

		Dimensions = dimensions;
		VerticesCount = verticesCount;
		Vertices = new Vector3[(verticesCount.x) * (verticesCount.y)];

		// Generation progress
		TotalVerticesToGenerate = verticesCount.x * verticesCount.y;
		generatedVertices = 0;
		TotalTrianglesToGenerate = (verticesCount.x - 1) * (verticesCount.y - 1) * 2;
		GeneratedTriangles = 0;

		// Visualization
		Parent = parent;
		VertexVisualizations = new List<GameObject>();
		oldVertices = new Vector3[(verticesCount.x) * (verticesCount.y)];
		targetVertices = new Vector3[(verticesCount.x) * (verticesCount.y)];
		oldVerticesColors = new Color[(verticesCount.x) * (verticesCount.y)];
		targetVerticesColors = new Color[(verticesCount.x) * (verticesCount.y)];
		verticesTranslationProgress = 0f;
		VerticesPreviewMaterial = verticesPreviewMaterial;
	}

	public void GenerateMeshStep(int trianglesToGenerate)
	{
		// Number of square tiles in the grid
		// (example: 3 x 3 vertices makes 2 x 2 tiles grid)
		// .  .  .
		// .  .  .   <- example grid
		// .  .  .

		int xTiles = VerticesCount.x - 1;
		int zTiles = VerticesCount.y - 1;

		int trianglesPerTile = 2;
		int verticesPerTriangle = 3;
		int verticesPerTile = trianglesPerTile * verticesPerTriangle;
		int verticesPerTilesRow = xTiles * verticesPerTile;

		// Three points in a row form one triangle (polygon)

		int z = (GeneratedTriangles / 2) / xTiles;

		// Go through the whole grid tile by tile and connect the vertices
		// to create each tile from two triangles
		for (; z < zTiles; z++)
		{
			int verticesArrayRowStartIndex = VerticesCount.x * z;
			int x = (GeneratedTriangles / 2) % xTiles;

			for (; x < xTiles; x++)
			{
				if (trianglesToGenerate > 0)
				{
					int trianglesArrayRowStartIndex = x * verticesPerTile + z * verticesPerTilesRow;

					// Lower left triangle
					// 1  .                                             VerticesCount.x  VerticesCount.x+1  VerticesCount.x+2 ...
					// 0  2  mapping triangles array to vertices array  0  1  2  3 ...
					triangles[trianglesArrayRowStartIndex + 0] = verticesArrayRowStartIndex + x;
					triangles[trianglesArrayRowStartIndex + 1] = verticesArrayRowStartIndex + x + VerticesCount.x; // Shift a row up
					triangles[trianglesArrayRowStartIndex + 2] = verticesArrayRowStartIndex + x + 1;

					// Upper right triangle
					// 4  5                                             VerticesCount.x  VerticesCount.x+1  VerticesCount.x+2 ...
					// .  3  mapping triangles array to vertices array  0  1  2  3 ...
					triangles[trianglesArrayRowStartIndex + 3] = verticesArrayRowStartIndex + x + 1;
					triangles[trianglesArrayRowStartIndex + 4] = verticesArrayRowStartIndex + x + VerticesCount.x; // Shift a row up
					triangles[trianglesArrayRowStartIndex + 5] = verticesArrayRowStartIndex + x + VerticesCount.x + 1;

					GeneratedTriangles += 2;
					trianglesToGenerate -= 2;
				}
			}
		}

		Mesh.vertices = Vertices;
		Mesh.triangles = triangles;
		Mesh.uv = uvs;

		Mesh.RecalculateNormals();
	}

	/// <summary>
	/// Sets target positions for vertices from height map for following interpolation.
	/// </summary>
	public void SetTargetVerticesPositions(bool visualize)
	{
		float maxVertexHeight = Dimensions.y;

		for (int i = 0; i < targetVertices.Length; i++)
		{
			float pixelsPerVertex = (float) Resolution.x / (float)VerticesCount.x;
			int pixelX = (int)(pixelsPerVertex * (i % VerticesCount.x));
			int pixelY = (int)(pixelsPerVertex * (i / VerticesCount.x));
			Vector2Int pixelCoords = new Vector2Int(pixelX, pixelY);
			int pixelIndex = TextureFunctions.CoordsToArrayIndex(Resolution.x, Resolution.y, pixelCoords);

			Color heightMapColor = HeightMap[pixelIndex];
			targetVertices[i] = new Vector3(Vertices[i].x, heightMapColor.r * maxVertexHeight, Vertices[i].z);

			targetVerticesColors[i] = heightMapColor;

			oldVertices[i] = Vertices[i];

			if (visualize)
			{
				oldVerticesColors[i] = VertexVisualizations[i].GetComponent<Renderer>().material.color;
			}
		}
	}

	/// <summary>
	/// Linearly interpolates between old and target vertices positions.
	/// </summary>
	/// <param name="step">A fraction of the way between point A and B to be taken next step (clamped to 1).</param>	
	public void UpdateVerticesPositions(float step, bool visualize)
	{
		verticesTranslationProgress += step;
		if (verticesTranslationProgress > 1f) verticesTranslationProgress = 1f;

		for (int i = 0; i < targetVertices.Length; i++)
		{
			Vertices[i] = Vector3.Lerp(oldVertices[i], targetVertices[i], verticesTranslationProgress);

			if (visualize)
			{
				VertexVisualizations[i].transform.localPosition = Vertices[i];
				Color newColor = Color.Lerp(oldVerticesColors[i], targetVerticesColors[i], verticesTranslationProgress);
				VertexVisualizations[i].GetComponent<Renderer>().material.color = newColor;
			}
		}
	}

	/// <summary>
	/// Executes the next step in the vertices generation process.
	/// </summary>
	public void GenerateVerticesStep(int verticesToGenerate, bool visualize)
	{
		verticesSpacing.x = Dimensions.x / VerticesCount.x;
		verticesSpacing.y = Dimensions.z / VerticesCount.y;

		int y = generatedVertices / VerticesCount.x;

		for (; y < VerticesCount.y; y++)
		{
			verticesToGenerate = GenerateVerticesRow(y, verticesToGenerate, visualize);
		}
	}

	public void GenerateAllVertices(bool visualize = false)
	{
		GenerateVerticesStep(TotalVerticesToGenerate, visualize);
	}

	/// <summary>
	/// Generates a row of vertices (or it's part).
	/// </summary>
	/// <returns>Vertices left to generate</returns>
	private int GenerateVerticesRow(int rowIndex, int verticesToGenerate, bool visualize)
	{
		// rowIndex is y coordinate
		float offsetX = verticesSpacing.x / 2;
		float offsetY = verticesSpacing.y / 2;

		int x = generatedVertices % VerticesCount.x;

		for (; x < VerticesCount.x; x++)
		{
			if (verticesToGenerate > 0)
			{
				Vector3 vertexPosition;
				vertexPosition.x = verticesSpacing.x * x + offsetX;
				vertexPosition.y = 0f;
				vertexPosition.z = verticesSpacing.y * rowIndex + offsetY;

				int vertexIndex = rowIndex * VerticesCount.x + x;
				GenerateVertex(vertexIndex, vertexPosition, visualize);

				uvs[vertexIndex].x = x / (float)VerticesCount.x;
				uvs[vertexIndex].y = rowIndex / (float)VerticesCount.y;

				verticesToGenerate--;
			}
		}

		return verticesToGenerate;
	}

	/// <summary>
	/// Generates a vertex and visualizes it (if enabled).
	/// </summary>
	private void GenerateVertex(int vertexIndex, Vector3 position, bool visualize)
	{
		Vertices[vertexIndex] = position;

		if (visualize)
		{
			VisualizeVertex(verticesSpacing.x, vertexIndex);
		}

		generatedVertices += 1;
	}

	/// <summary>
	/// Creates a representation of the position of a vertex.
	/// </summary>
	private void VisualizeVertex(float verticesSpacingX, int vertexIndex)
	{
		// Spawn game object
		GameObject vertexMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		vertexMarker.transform.SetParent(VerticesParent.transform);

		// Calculate radius
		float markerRadius = verticesSpacingX / 2;
		if (markerRadius > 1f) markerRadius = 1f;

		// Set vertex marker properties
		vertexMarker.transform.localScale = new Vector3(markerRadius, markerRadius, markerRadius);
		vertexMarker.transform.localPosition = Vertices[vertexIndex];
		vertexMarker.GetComponent<Renderer>().material = VerticesPreviewMaterial;
		vertexMarker.GetComponent<Renderer>().material.color = Color.black;
		VertexVisualizations.Add(vertexMarker);
	}

	/// <summary>
	/// Deletes vertex markers and initializes properties.
	/// </summary>
	public void ClearVertices()
	{
		DestroyVertexMarkers();

		// Initialize properties
		Vertices = new Vector3[(VerticesCount.x) * (VerticesCount.y)];
		generatedVertices = 0;
	}

	/// <summary>
	/// Destroys vertex visualizations.
	/// </summary>
	public void DestroyVertexMarkers()
	{
		// Destroy vertex markers
		for (int i = 0; i < VertexVisualizations.Count; i++)
		{
			GameObject.Destroy(VertexVisualizations[i].gameObject);
		}

		VertexVisualizations = new List<GameObject>();
	}

	/// <summary>
	/// Creates a GameObject to encapsulate all vertex markers.
	/// </summary>
	public void CreateVerticesParent(Vector3 localPosition)
	{
		if (VerticesParent == null)
		{
			VerticesParent = new GameObject(VERTICES_PARENT_NAME);
			VerticesParent.transform.SetParent(Parent.transform);
			VerticesParent.transform.localPosition = localPosition;
		}
	}

	public void HideVertexVisualizations()
	{
		// Hide vertex markers
		for (int i = 0; i < VertexVisualizations.Count; i++)
		{
			MeshRenderer renderer = VertexVisualizations[i].gameObject.GetComponent<MeshRenderer>();
			renderer.enabled = false;
		}
	}

	public void CreateMeshGameObject(Color[] texturePixels, Vector3 localPosition, Material material)
	{
		if (MeshGO == null)
		{
			MeshGO = new GameObject(MESH_GO_NAME);
			MeshGO.transform.SetParent(Parent.transform);
			MeshGO.transform.localPosition = localPosition;

			Texture2D texture = new Texture2D(Resolution.x, Resolution.y);
			texture.SetPixels(texturePixels);
			texture.Apply();

			MeshFilter meshFilter = MeshGO.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = MeshGO.AddComponent<MeshRenderer>();

			meshFilter.mesh = Mesh;
			meshRenderer.material = material;
			meshRenderer.material.mainTexture = texture;

			// Needs fixing
			//Rigidbody rigidbody = MeshGO.AddComponent<Rigidbody>();
			//rigidbody.useGravity = false;
			//rigidbody.constraints = RigidbodyConstraints.FreezeAll;

			//MeshCollider collider = MeshGO.AddComponent<MeshCollider>();
			//collider.sharedMesh = meshFilter.mesh;
			//collider.convex = true;
		}
	}
}
