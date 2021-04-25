using Instantiators.ObjectGrid;
using MyRandom;
using System;
using System.Collections.Generic;
using System.Linq;
using ObjectPlacement.JitteredGrid;
using TaskManagement;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Object representing and island area that can be placed on a grid.
	/// </summary>
	public class IslandArea : GridObject
	{
		// Properties
		public IslandType Type { get; private set; }
		public bool Initialized { get; private set; }
		public bool Finished => taskList?.Finished ?? false;

		// Basic
		private const string DEFAULT_NAME = "IslandArea";
		private IslandGenerationParams generationParams;
		private TaskList taskList;
		private TextMesh name;

		// Progress
		private string lastTaskName = "";
		private float progress = 0f;
		private TextMesh progressText;

		public IslandArea()
		{
			gameObject.name = DEFAULT_NAME;
			taskList = new TaskList();
			taskList.DebugMode = false;
			Initialized = false;
		}

		/// <summary>
		/// Executes a step of the generation process.
		/// </summary>
		/// <returns></returns>
		public GameObject GenerateStep()
		{
			if (Initialized)
			{
				taskList.ExecuteStepSize();
				UpdateProgress();
			}
			else
			{
				PrintErrorParamsNotAssigned("Init()");
			}

			return gameObject;
		}

		/// <summary>
		/// Generate island area at once (May drop frame rate).
		/// </summary>
		public GameObject Generate()
		{
			if (Initialized)
			{
				taskList.Execute();
				UpdateProgress();
			}
			else
			{
				PrintErrorParamsNotAssigned("Init()");
			}

			return gameObject;
		}

		/// <summary>
		/// Initializes all the partial tasks and adds them to a task list.
		/// </summary>
		public void Init(IslandGenerationParams generationParams)
		{
			this.generationParams = generationParams;
			Type = DetermineIslandType(generationParams.IslandTypes);

			// Name Visualization
			InstantiateNameText(Type.MaxTerrainHeight / 2, GetNameFromPosition(GetPosition(), Type.IslandNames));

			// Progress Visualization
			InstantiateProgressText();

			InitAllTasks();

			Initialized = true;
		}

		#region Generation Tasks Setup

		/// <summary>
		/// Initializes and adds all generation tasks to the task list.
		/// </summary>
		private void InitAllTasks()
		{
			// Terrain nodes
			GenerateTerrainNodes generateTerrainNodes = AddGenerateNodesTask();
			ShowTerrainNodes showTerrainNodes = AddShowTerrainNodesTask(generateTerrainNodes);

			// Gradients
			GenerateNodesGradients generateNodesGradients = AddGenerateNodesGradientsTask(generateTerrainNodes);
			ShowTextures showGradients = AddShowTexturesTask("Show Node Gradients", generateNodesGradients.GetResult);
			HideObjects<IHideable> hideGradients = AddHideObjectsTask("Hide Node Gradients", showGradients.GetResult);

			// Noises
			GenerateNodesNoises generateNodesNoises = AddGenerateNodesNoisesTask(generateTerrainNodes);
			ShowTextures showNodesNoises = AddShowTexturesTask("Show Nodes Noises", generateNodesNoises.GetResult);
			HideObjects<IHideable> hideNodesNoises = AddHideObjectsTask("Hide Nodes Noises", showNodesNoises.GetResult);

			// Multiplication
			MultiplyTextureLists multiplyGradientsAndNoises = AddMultiplyTextureListsTask("Multiply Node Gradients and Noises", generateNodesGradients.GetResult, generateNodesNoises.GetResult);
			ShowTextures showMultiplicationResult = AddShowTexturesTask("Show Gradients and Noises Multiplication Result", multiplyGradientsAndNoises.GetResult);
			HideObjects<IHideable> hideMultiplicationResult = AddHideObjectsTask("Hide Multiplication Result", showMultiplicationResult.GetResult);

			// Addition
			AddTextures addMultiplicationResults = AddAddTexturesTask("Add Multiplication Results Together", multiplyGradientsAndNoises.GetResult);
			ShowTextures showAdditionResult = AddShowTexturesTask("Show Addition Result", addMultiplicationResults.GetResultInList);

			// Texture
			GenerateIslandAreaTexture generateTexture = AddGenerateIslandAreaTextureTask(generateTerrainNodes.GetResult, multiplyGradientsAndNoises.GetResult, addMultiplicationResults.GetResult);
			HideObjects<IHideable> hideAdditionResult = AddHideObjectsTask("Hide Addition Result", showAdditionResult.GetResult);
			ShowTextures showTexture = AddShowTexturesTask("Show Island Area Texture", generateTexture.GetResultInList);

			// Mesh
			HideObjects<IHideable> hideTerrainNodes = AddHideObjectsTask("Hide Terrain Nodes", showTerrainNodes.GetResult);
			GenerateMeshVertices generateMeshVertices = AddGenerateMeshVerticesTask(addMultiplicationResults.GetResult, 40);
			TranslateMeshVertices translateMeshVertices = AddTranslateMeshVerticesTask(generateMeshVertices.GetResult, 10);
			HideObjects<IHideable> hideTexture = AddHideObjectsTask("Hide Island Area Texture", showTexture.GetResult);
			GenerateMesh generateMesh = AddGenerateMeshTask(translateMeshVertices.GetResult, generateTexture.GetResult, 40);

			// Object Placement
			foreach (PlacedObjectParams placedObjectParams in Type.PlacedObjectParams)
			{
				if (placedObjectParams == null) continue;
				GenerateObjectPositions generateObjectPositions = AddGenerateObjectPositionsTask(placedObjectParams, addMultiplicationResults.GetResult, translateMeshVertices.GetResult, generateTexture.GetTerrainTypesAtPixels);
				PlaceObjects placeObjects = AddPlaceObjectsTask(placedObjectParams, generateObjectPositions.GetResult);
			}
		}

		/// <summary>
		/// Initializes and adds to task list the "Generate Terrain Nodes" task.
		/// </summary>
		private GenerateTerrainNodes AddGenerateNodesTask()
		{
			GenerateTerrainNodes generateTerrainNodes = new GenerateTerrainNodes(Type.TerrainNodesParams, GetParams());
			generateTerrainNodes.SetParams(Type.TerrainNodesParams.MaxNodes);
			taskList.AddTask(generateTerrainNodes);

			return generateTerrainNodes;
		}

		/// <summary>
		/// Initializes and adds to task list the "Show Terrain nodes" task.
		/// </summary>
		private ShowTerrainNodes AddShowTerrainNodesTask(GenerateTerrainNodes generateTerrainNodes)
		{
			ShowTerrainNodesParams parameters = new ShowTerrainNodesParams
			{
				AreaRadius = Radius,
				Parent = gameObject.transform,
				PreviewMaterial = generationParams.PreviewsMaterial,
				GetTerrainNodes = generateTerrainNodes.GetResult
			};

			ShowTerrainNodes showTerrainNodes = new ShowTerrainNodes(parameters);
			showTerrainNodes.SetParams(1, generationParams.PreviewProgress, generationParams.VisualStepTime);
			taskList.AddTask(showTerrainNodes);

			return showTerrainNodes;
		}

		/// <summary>
		/// Initializes and adds to task list the "Generate Nodes Gradients" task.
		/// </summary>
		private GenerateNodesGradients AddGenerateNodesGradientsTask(GenerateTerrainNodes generateTerrainNodes)
		{
			GenerateNodesGradients generateNodesGradients = new GenerateNodesGradients(GetResolution(), Radius, generateTerrainNodes.GetResult);
			taskList.AddTask(generateNodesGradients);

			return generateNodesGradients;
		}

		/// <summary>
		/// Initializes and adds to task list any "Show Textures" task.
		/// </summary>
		private ShowTextures AddShowTexturesTask(string name, Func<List<Color[]>> getTextures)
		{
			// Show Gradients
			ShowTexturesParams parameters = new ShowTexturesParams
			{
				Resolution = GetResolution(),
				GetTextures = getTextures,
				Parent = gameObject.transform,
				SideLength = Radius * 2,
				TexturePreviewMaterial = generationParams.TexturePreviewMaterial
			};

			ShowTextures showTextures = new ShowTextures(parameters);
			showTextures.SetParams(1, generationParams.PreviewProgress, generationParams.VisualStepTime);
			showTextures.Name = name;
			taskList.AddTask(showTextures);

			return showTextures;
		}

		/// <summary>
		/// Initializes and adds to task list any "Hide Objects" task.
		/// </summary>
		private HideObjects<IHideable> AddHideObjectsTask(string name, Func<List<IHideable>> getPreviewObjects)
		{
			HideObjects<IHideable> hideObjects = new HideObjects<IHideable>(getPreviewObjects);
			hideObjects.SetParams(1, generationParams.PreviewProgress);
			hideObjects.Name = name;
			taskList.AddTask(hideObjects);

			return hideObjects;
		}

		/// <summary>
		/// Initializes and adds to task list the "Generate Nodes Noises" task.
		/// </summary>
		private GenerateNodesNoises AddGenerateNodesNoisesTask(GenerateTerrainNodes generateTerrainNodes)
		{
			GenerateNodesNoises generateNodesNoises = new GenerateNodesNoises(GetPosition(), Radius, GetResolution(), generateTerrainNodes.GetResult);
			taskList.AddTask(generateNodesNoises);

			return generateNodesNoises;
		}

		/// <summary>
		/// Initializes and adds to task list any "Multiply Texture Lists" task.
		/// </summary>
		private MultiplyTextureLists AddMultiplyTextureListsTask(string name, Func<List<Color[]>> getMultiplicands, Func<List<Color[]>> getMultipliers)
		{
			MultiplyTextureLists multiplyTextureLists = new MultiplyTextureLists(getMultiplicands, getMultipliers);
			multiplyTextureLists.Name = name;
			taskList.AddTask(multiplyTextureLists);

			return multiplyTextureLists;
		}

		/// <summary>
		/// Initializes and adds to task list any "Add Textures" task.
		/// </summary>
		private AddTextures AddAddTexturesTask(string name, Func<List<Color[]>> getTextures, int stepSize = 1)
		{
			AddTextures addTextures = new AddTextures(getTextures);
			addTextures.SetParams(stepSize);
			addTextures.Name = name;
			taskList.AddTask(addTextures);

			return addTextures;
		}

		/// <summary>
		/// Initializes and adds to task list the "Generate Island Area Texture" task.
		/// </summary>
		private GenerateIslandAreaTexture AddGenerateIslandAreaTextureTask(Func<List<TerrainNode>> getTerrainNodes, Func<List<Color[]>> getTerrainNodesHeightmaps, Func<Color[]> getHeightmap)
		{
			GenerateIslandAreaTextureParams parameters = new GenerateIslandAreaTextureParams
			{
				TerrainNodesParams = Type.TerrainNodesParams,
				GetTerrainNodes = getTerrainNodes,
				GetTerrainNodesHeightmaps = getTerrainNodesHeightmaps,
				GetHeightmap = getHeightmap,
				Resolution = GetResolution()
			};

			GenerateIslandAreaTexture generateTexture = new GenerateIslandAreaTexture(parameters);
			taskList.AddTask(generateTexture);

			return generateTexture;
		}

		/// <summary>
		/// Initializes and adds to task list the "Generate Mesh Vertices" task.
		/// </summary>
		private GenerateMeshVertices AddGenerateMeshVerticesTask(Func<Color[]> getHeightmap, int stepSize = 1)
		{
			float diameter = Radius * 2;
			Vector3 dimensions = new Vector3(diameter, Type.MaxTerrainHeight, diameter);

			int pixelCount = GetResolution();
			Vector2Int resolution = new Vector2Int(pixelCount, pixelCount);

			int verticesCount = (int) (diameter * Type.VerticesPerUnit);
			Vector2Int vertices = new Vector2Int(verticesCount, verticesCount);

			GenerateMeshVerticesParams parameters = new GenerateMeshVerticesParams
			{
				Resolution = resolution,
				Dimensions = dimensions,
				GetHeightmap = getHeightmap,
				Parent = gameObject.transform,
				VerticesCount = vertices,
				Visualize = generationParams.PreviewProgress
			};

			float minStepDuration = GetVisualStepTime(stepSize, verticesCount * verticesCount);

			GenerateMeshVertices generateMeshVertices = new GenerateMeshVertices(parameters);
			generateMeshVertices.SetParams(stepSize, true, minStepDuration);
			taskList.AddTask(generateMeshVertices);

			return generateMeshVertices;
		}

		/// <summary>
		/// Initializes and adds to task list the "Translate Mesh Vertices" task.
		/// </summary>
		private TranslateMeshVertices AddTranslateMeshVerticesTask(Func<TerrainMesh> getMesh, int stepSize = 1)
		{
			TranslateMeshVertices translateMeshVertices = new TranslateMeshVertices(generationParams.PreviewProgress, getMesh);

			float minStepDuration = GetVisualStepTime(stepSize, GetMeshTrianglesCount() / 2);

			translateMeshVertices.SetParams(stepSize, true, minStepDuration);
			taskList.AddTask(translateMeshVertices);

			return translateMeshVertices;
		}

		/// <summary>
		/// Initializes and adds to task list the "Generate Mesh" task.
		/// </summary>
		private GenerateMesh AddGenerateMeshTask(Func<TerrainMesh> getTerrainMesh, Func<Color[]> getTexturePixels, int stepSize = 1)
		{
			GenerateMesh generateMesh = new GenerateMesh(Type.TerrainMeshMaterial, getTerrainMesh, getTexturePixels);

			float minStepDuration = GetVisualStepTime(stepSize, GetMeshTrianglesCount() / 2);

			generateMesh.SetParams(stepSize, true, minStepDuration);
			taskList.AddTask(generateMesh);

			return generateMesh;
		}

		/// <summary>
		/// Initializes and adds to task list the "Generate Object Positions" task.
		/// </summary>
		private GenerateObjectPositions AddGenerateObjectPositionsTask(PlacedObjectParams placedObjectParams, Func<Color[]> getHeightmap, Func<TerrainMesh> getMesh, Func<TerrainBlend[]> getTerrainTypesAtPixels)
		{
			GenerateObjectPositionsParams parameters = new GenerateObjectPositionsParams
			{
				Radius = Radius,
				MaxTerrainHeight = Type.MaxTerrainHeight,
				GetHeightmap = getHeightmap,
				GetTerrainMesh = getMesh,
				GetTerrainTypesAtPixels = getTerrainTypesAtPixels,
				Resolution = GetResolution(),
				PlacedObjectParams = placedObjectParams
			};

			GenerateObjectPositions generateObjectPositions = new GenerateObjectPositions(parameters);
			taskList.AddTask(generateObjectPositions);
			return generateObjectPositions;
		}

		/// <summary>
		/// Initializes and adds to task list the "Place Objects" task.
		/// </summary>
		private PlaceObjects AddPlaceObjectsTask(PlacedObjectParams placedObjectParams, Func<List<GridPoint>> getPositions)
		{
			PlaceObjects placeObjects = new PlaceObjects(gameObject.transform, placedObjectParams, getPositions);
			taskList.AddTask(placeObjects);

			return placeObjects;
		}

		#endregion

		#region Progress Visualization

		private void UpdateProgress()
		{
			if (taskList.CurrentTask != null)
			{
				lastTaskName = taskList.CurrentTask.Name;
			}

			progress = taskList.Progress;
			SetProgressText(lastTaskName, progress);
		}

		private void InstantiateProgressText()
		{
			if (generationParams.PreviewProgress)
			{
				GameObject progressTextObject = new GameObject("Progress Text");
				progressTextObject.transform.SetParent(gameObject.transform);
				progressTextObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
				progressTextObject.transform.localPosition = new Vector3(-Radius, 0f, -Radius - 0.5f);
				progressTextObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

				progressText = progressTextObject.AddComponent<TextMesh>();
				progressText.fontSize = 20;
			}
		}

		private void SetProgressText(string currentTaskName, float totalProgress)
		{
			if (progressText != null)
			{
				progressText.text = $"{currentTaskName}\n{totalProgress:P}";
			}
		}

		#endregion

		private int GetMeshTrianglesCount()
		{
			float diameter = Radius * 2;
			int verticesCount = (int)(diameter * Type.VerticesPerUnit);
			return (verticesCount - 1) * (verticesCount - 1) * 2;
		}

		/// <summary>
		/// Randomly chooses a name from a list of names;
		/// </summary>
		private string GetNameFromPosition(Vector3 position, TextAsset namesAsset)
		{

			if (namesAsset != null)
			{
				RandomFromSeed random = new RandomFromSeed(position, "Name From Position");
				string[] allNames = namesAsset.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
				int nameIndex = random.Next(0, allNames.Length);
				return allNames[nameIndex];
			}

			return "";
		}

		/// <summary>
		/// Instantiates 3D Text to display the islands areas name.
		/// </summary>
		private void InstantiateNameText(float height, string islandAreaName = "Island Area Name")
		{
			if (generationParams.ShowIslandNames)
			{
				GameObject nameTextObject = new GameObject("Island Area Name");
				nameTextObject.transform.SetParent(gameObject.transform);
				nameTextObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
				nameTextObject.transform.localPosition = new Vector3(-Radius, height, -Radius);
				nameTextObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

				name = nameTextObject.AddComponent<TextMesh>();
				name.fontSize = generationParams.IslandNamesFontSize;

				name.text = islandAreaName;
			}
		}

		/// <summary>
		/// Calculates the minimum time that every visual step should be displayed for.
		/// </summary>
		private float GetVisualStepTime(int stepSize, int totalSteps)
		{
			float stepVisualizationTime = ((float)stepSize / totalSteps) * generationParams.VisualStepTime;
			return generationParams.PreviewProgress ? stepVisualizationTime : 0f;
		}

		/// <summary>
		/// Calculates the resolution of the textures for this area from the pixels per unit.
		/// </summary>
		private int GetResolution()
		{
			float diameter = Radius * 2;
			return (int)(diameter * Type.PixelsPerUnit);
		}

		/// <summary>
		/// Randomly chooses an island type for this island area.
		/// </summary>
		private IslandType DetermineIslandType(List<IslandType> islandTypes)
		{
			RandomFromSeed randomGenerator = new RandomFromSeed(GetPosition(), "IslandTypePicker");
			int randomIndex = randomGenerator.PickItemIndex(islandTypes.Cast<IHasProbability>().ToList());

			if (randomIndex < 0) Debug.LogError("No island type index was randomly picked.");
			return islandTypes[randomIndex];
		}

		/// <summary>
		/// Prints error message with hint.
		/// </summary>
		private void PrintErrorParamsNotAssigned(string assignmentFuncName)
		{
			Debug.LogError(
				$"{DEFAULT_NAME}'s parameters not assigned. Call \"{assignmentFuncName}\" first."
			);
		}
	}
}
