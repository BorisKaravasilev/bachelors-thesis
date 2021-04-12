using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class IslandArea : GridObject
{
	private TaskList taskList;
	private TextMesh progressText;

	private string lastTaskName = "";
	private float progress = 0f;

	private const string DEFAULT_NAME = "IslandArea";
	
	public bool Initialized { get; private set; }
	public bool Finished => taskList?.Finished ?? false;

	public IslandArea()
	{
		gameObject.name = DEFAULT_NAME;
		taskList = new TaskList();
		taskList.DebugMode = true;
		Initialized = false;
	}

	/// <summary>
	/// Creates a list of tasks defining the creation process of the area.
	/// </summary>
	public void Init(bool previewProgress, float visualStepTime, int resolution, TerrainNodesParams terrainNodesParams)
	{
		if (previewProgress)
		{
			InstantiateProgressText();
		}

		float diameter = Radius * 2;

		int maxNodes = terrainNodesParams.MaxNodes;
		float nodePreviewRadius = Radius / 10f;

		Vector2Int resolution2D = new Vector2Int(resolution, resolution);
		float maxTerrainHeight = 2f;
		Vector3 dimensions = new Vector3(diameter, maxTerrainHeight, diameter);
		Vector2Int verticesCount = new Vector2Int(20, 20);

		Transform parent = gameObject.transform;
		GridObjectParams gridObjectParams = base.GetParams();
		
		// TODO: Calculate and assign round transparent mask

		// Generate Terrain nodes
		GenerateTerrainNodes generateTerrainNodes = new GenerateTerrainNodes(terrainNodesParams, gridObjectParams, maxNodes);
		taskList.AddTask(generateTerrainNodes);

		// Show Terrain nodes
		ShowTerrainNodes showTerrainNodes = new ShowTerrainNodes(nodePreviewRadius, parent, generateTerrainNodes.GetResult);
		showTerrainNodes.SetParams(1, previewProgress, visualStepTime);
		taskList.AddTask(showTerrainNodes);

		// Generate Nodes Gradients
		GenerateNodesGradients generateNodesGradients = new GenerateNodesGradients(resolution, Radius, generateTerrainNodes.GetResult);
		taskList.AddTask(generateNodesGradients);

		// Show Gradients
		ShowTextures showGradients = new ShowTextures(diameter, resolution, parent, generateNodesGradients.GetResult);
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
		ShowTextures showNodesNoises = new ShowTextures(diameter, resolution, parent, generateNodesNoises.GetResult);
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
		ShowTextures showMultiplicationResult = new ShowTextures(diameter, resolution, parent, multiplyGradientsAndNoises.GetResult);
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
		ShowTexture showGradientNoisesAddition = new ShowTexture(diameter, resolution, parent, addMultiplicationResults.GetResult);
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
		ShowTexture showIslandAreaTexture = new ShowTexture(diameter, resolution, parent, generateTexture.GetResult);
		showIslandAreaTexture.Name = "Show Island Area Texture";
		showIslandAreaTexture.SetParams(1, previewProgress, visualStepTime);
		taskList.AddTask(showIslandAreaTexture);

		// Generate Mesh Vertices
		GenerateMeshVertices generateMeshVertices = new GenerateMeshVertices(previewProgress, gameObject.transform, resolution2D, dimensions, verticesCount, addMultiplicationResults.GetResult);
		taskList.AddTask(generateMeshVertices);

		Initialized = true;
	}

	private GameObject InstantiateProgressText()
	{
		GameObject progressTextObject = new GameObject("Progress text");
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
