using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class IslandArea : GridObject
{
	private TaskList taskList;

	private TextMesh name;
	private TextMesh progressText;

	private string lastTaskName = "";
	private float progress = 0f;

	private const string DEFAULT_NAME = "IslandArea";
	
	public bool Initialized { get; private set; }
	public bool Finished => taskList?.Finished ?? false;
	public bool DebugMode
	{
		get { return taskList.DebugMode; }
		set { taskList.DebugMode = value; }
	}

	public IslandArea()
	{
		gameObject.name = DEFAULT_NAME;
		taskList = new TaskList();
		taskList.DebugMode = false;
		Initialized = false;
	}

	/// <summary>
	/// Creates a list of tasks defining the creation process of the area.
	/// </summary>
	public void Init(bool previewProgress, float visualStepTime, int pixelsPerUnit,
		TerrainNodesParams terrainNodesParams, Material meshMaterial, Material previewObjectMaterial,
		Material texturePreviewMaterial, TextAsset islandNames)
	{
		if (previewProgress)
		{
			InstantiateProgressText();
		}

		float diameter = Radius * 2;

		int maxNodes = terrainNodesParams.MaxNodes;
		float nodePreviewRadius = Radius / 10f;

		int resolution = (int) (diameter * pixelsPerUnit);
		Vector2Int resolution2D = new Vector2Int(resolution, resolution);
		float maxTerrainHeight = 2f;
		Vector3 dimensions = new Vector3(diameter, maxTerrainHeight, diameter);
		Vector2Int verticesCount = new Vector2Int(20, 20);

		InstantiateNameText(maxTerrainHeight, GetNameFromPosition(Position, islandNames));

		Transform parent = gameObject.transform;
		GridObjectParams gridObjectParams = base.GetParams();

		// Generate Terrain nodes
		GenerateTerrainNodes generateTerrainNodes = new GenerateTerrainNodes(terrainNodesParams, gridObjectParams, maxNodes);
		taskList.AddTask(generateTerrainNodes);

		// Show Terrain nodes
		ShowTerrainNodes showTerrainNodes = new ShowTerrainNodes(previewObjectMaterial, nodePreviewRadius, parent, generateTerrainNodes.GetResult);
		showTerrainNodes.SetParams(1, previewProgress, visualStepTime);
		taskList.AddTask(showTerrainNodes);

		// Generate Nodes Gradients
		GenerateNodesGradients generateNodesGradients = new GenerateNodesGradients(resolution, Radius, generateTerrainNodes.GetResult);
		taskList.AddTask(generateNodesGradients);

		// Show Gradients
		ShowTextures showGradients = new ShowTextures(texturePreviewMaterial, diameter, resolution, parent, generateNodesGradients.GetResult);
		showGradients.SetParams(1, previewProgress, visualStepTime);
		showGradients.Name = "Show Node Gradients";
		taskList.AddTask(showGradients);

		// Hide Gradients
		HidePreviewObjects<TexturePreview> hideGradients = new HidePreviewObjects<TexturePreview>(showGradients.GetResult);
		hideGradients.SetParams(1, previewProgress);
		hideGradients.Name = "Hide Node Gradients";
		taskList.AddTask(hideGradients);

		// Generate Nodes Noises
		GenerateNodesNoises generateNodesNoises = new GenerateNodesNoises(resolution, generateTerrainNodes.GetResult, maxNodes);
		taskList.AddTask(generateNodesNoises);

		// Show Nodes Noises
		ShowTextures showNodesNoises = new ShowTextures(previewObjectMaterial, diameter, resolution, parent, generateNodesNoises.GetResult);
		showNodesNoises.SetParams(1, previewProgress, visualStepTime);
		showNodesNoises.Name = "Show Nodes Noises";
		taskList.AddTask(showNodesNoises);

		// Hide Nodes Noises
		HidePreviewObjects<TexturePreview> hideNodesNoises = new HidePreviewObjects<TexturePreview>(showNodesNoises.GetResult);
		hideNodesNoises.SetParams(1, previewProgress);
		hideNodesNoises.Name = "Hide Node Gradients";
		taskList.AddTask(hideNodesNoises);

		// Multiply Nodes Gradients and Noises
		MultiplyTextureLists multiplyGradientsAndNoises = new MultiplyTextureLists(generateNodesGradients.GetResult, generateNodesNoises.GetResult, maxNodes);
		multiplyGradientsAndNoises.Name = "Multiply Node Gradients and Noises";
		taskList.AddTask(multiplyGradientsAndNoises);

		// Show Gradients and Noises Multiplication Result
		ShowTextures showMultiplicationResult = new ShowTextures(texturePreviewMaterial, diameter, resolution, parent, multiplyGradientsAndNoises.GetResult);
		showMultiplicationResult.SetParams(1, previewProgress, visualStepTime);
		showMultiplicationResult.Name = "Show Gradients and Noises Multiplication Result";
		taskList.AddTask(showMultiplicationResult);

		// Hide Gradients and Noises Multiplication Result
		HidePreviewObjects<TexturePreview> hideMultiplicationResult = new HidePreviewObjects<TexturePreview>(showMultiplicationResult.GetResult);
		hideMultiplicationResult.SetParams(1, previewProgress);
		hideMultiplicationResult.Name = "Hide Gradients and Noises Multiplication Result";
		taskList.AddTask(hideMultiplicationResult);

		// Add Multiplication Results Together (final heightmap)
		AddTextures addMultiplicationResults = new AddTextures(multiplyGradientsAndNoises.GetResult, maxNodes);
		addMultiplicationResults.Name = "Add Multiplication Results Together";
		taskList.AddTask(addMultiplicationResults);

		// Show Addition Result
		ShowTexture showGradientNoisesAddition = new ShowTexture(texturePreviewMaterial, diameter, resolution, parent, addMultiplicationResults.GetResult);
		showGradientNoisesAddition.Name = "Show Addition Result";
		showGradientNoisesAddition.SetParams(1, previewProgress, visualStepTime);
		taskList.AddTask(showGradientNoisesAddition);

		// Hide Addition Result
		HidePreviewObjects<TexturePreview> hideAdditionResult = new HidePreviewObjects<TexturePreview>(showGradientNoisesAddition.GetResultInList);
		hideAdditionResult.SetParams(1, previewProgress);
		hideAdditionResult.Name = "Hide Addition Result";
		taskList.AddTask(hideAdditionResult);

		// Generate Island Area Texture
		GenerateIslandAreaTexture generateTexture = new GenerateIslandAreaTexture(resolution, Radius, terrainNodesParams, generateTerrainNodes.GetResult, addMultiplicationResults.GetResult, multiplyGradientsAndNoises.GetResult);
		taskList.AddTask(generateTexture);

		// Show Island Area Texture
		ShowTexture showIslandAreaTexture = new ShowTexture(texturePreviewMaterial, diameter, resolution, parent, generateTexture.GetResult);
		showIslandAreaTexture.Name = "Show Island Area Texture";
		showIslandAreaTexture.SetParams(1, previewProgress, visualStepTime);
		taskList.AddTask(showIslandAreaTexture);

		// Hide Terrain Nodes
		HidePreviewObjects<PreviewObject> hideTerrainNodes = new HidePreviewObjects<PreviewObject>(showTerrainNodes.GetResult);
		hideTerrainNodes.SetParams(1, previewProgress);
		hideTerrainNodes.Name = "Hide Terrain Nodes";
		taskList.AddTask(hideTerrainNodes);

		// Generate Mesh Vertices
		GenerateMeshVertices generateMeshVertices = new GenerateMeshVertices(previewProgress, gameObject.transform, resolution2D, dimensions, verticesCount, addMultiplicationResults.GetResult);
		generateMeshVertices.SetParams(40, true, previewProgress ? visualStepTime : 0f);
		taskList.AddTask(generateMeshVertices);

		// Translate Vertices
		TranslateMeshVertices translateMeshVertices = new TranslateMeshVertices(previewProgress, generateMeshVertices.GetResult);
		translateMeshVertices.SetParams(10, true, previewProgress ? visualStepTime / 20 : 0f);
		taskList.AddTask(translateMeshVertices);

		// Hide Island Area Texture
		HidePreviewObjects<TexturePreview> hideIslandAreaTexture = new HidePreviewObjects<TexturePreview>(showIslandAreaTexture.GetResultInList);
		hideIslandAreaTexture.SetParams(1, previewProgress);
		hideIslandAreaTexture.Name = "Hide Island Area Texture";
		taskList.AddTask(hideIslandAreaTexture);

		// Generate Mesh
		GenerateMesh generateMesh = new GenerateMesh(meshMaterial, translateMeshVertices.GetResult, generateTexture.GetResult);
		generateMesh.SetParams(40, true, previewProgress ? visualStepTime : 0f);
		taskList.AddTask(generateMesh);

		Initialized = true;
	}

	private string GetNameFromPosition(Vector3 position, TextAsset namesAsset)
	{
		string[] allNames = namesAsset.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		int nameIndex = RandomFromSeed.Range(position, 0, allNames.Length);
		return allNames[nameIndex];
	}

	private GameObject InstantiateProgressText()
	{
		GameObject progressTextObject = new GameObject("Progress Text");
		progressTextObject.transform.SetParent(gameObject.transform);
		progressTextObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
		progressTextObject.transform.localPosition = new Vector3(-Radius, 0f, -Radius - 0.5f);
		progressTextObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

		progressText = progressTextObject.AddComponent<TextMesh>();
		progressText.fontSize = 20;

		return progressTextObject;
	}

	private void SetProgressText(string currentTaskName, float totalProgress)
	{
		if (progressText != null)
		{
			progressText.text = $"{currentTaskName}\n{totalProgress:P}";
		}
	}

	private GameObject InstantiateNameText(float height, string islandAreaName = "Island Area Name")
	{
		GameObject nameTextObject = new GameObject("Island Area Name");
		nameTextObject.transform.SetParent(gameObject.transform);
		nameTextObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
		nameTextObject.transform.localPosition = new Vector3(-Radius, height, 1f);
		nameTextObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

		name = nameTextObject.AddComponent<TextMesh>();
		name.fontSize = 20;

		name.text = islandAreaName;

		return nameTextObject;
	}

	public GameObject Generate()
	{
		if (Initialized)
		{
			taskList.Execute();
		}
		else
		{
			PrintErrorParamsNotAssigned("Generate()", "AssignParams()");
		}

		return gameObject;
	}

	public GameObject GenerateStep()
	{
		if (Initialized)
		{
			int executedSteps = taskList.ExecuteStepSize();

			if (taskList.CurrentTask != null)
			{
				lastTaskName = taskList.CurrentTask.Name;
			}

			progress = taskList.Progress;
			SetProgressText(lastTaskName, progress);
		}
		else
		{
			PrintErrorParamsNotAssigned("GenerateStep()", "AssignParams()");
		}

		return gameObject;
	}

	private void PrintErrorParamsNotAssigned(string requiringFuncName, string assignmentFuncName)
	{
		Debug.LogError(
			$"{DEFAULT_NAME}'s parameters not assigned before calling \"{requiringFuncName}\"." +
			$" Call \"{assignmentFuncName}\" first."
		);
	}
}
