using UnityEditor;
using UnityEngine;

public class IslandArea : GridObject
{
	private TaskList taskList;

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
	public void Init(bool previewProgress, int resolution, TerrainNodesParams terrainNodesParams)
	{
		float radius = Parameters.Radius;
		float diameter = radius * 2;

		int maxNodes = terrainNodesParams.MaxNodes;
		float nodePreviewRadius = radius / 10f;

		Transform parent = gameObject.transform;
		
		// TODO: Calculate and assign round transparent mask

		// Generate Terrain nodes
		GenerateTerrainNodes generateTerrainNodes = new GenerateTerrainNodes(terrainNodesParams, Parameters, maxNodes);
		taskList.AddTask(generateTerrainNodes);

		// Show Terrain nodes
		ShowTerrainNodes showTerrainNodes = new ShowTerrainNodes(nodePreviewRadius, parent, generateTerrainNodes.GetResult);
		showTerrainNodes.SetParams(1, previewProgress);
		taskList.AddTask(showTerrainNodes);

		// Generate Nodes Gradients
		GenerateNodesGradients generateNodesGradients = new GenerateNodesGradients(resolution, radius, generateTerrainNodes.GetResult);
		taskList.AddTask(generateNodesGradients);

		// Show Gradients
		ShowTextures showGradients = new ShowTextures(diameter, resolution, parent, generateNodesGradients.GetResult);
		showGradients.SetParams(1, previewProgress);
		taskList.AddTask(showGradients);

		// Generate Nodes Noises
		GenerateNodesNoises generateNodesNoises = new GenerateNodesNoises(resolution, generateTerrainNodes.GetResult, maxNodes);
		taskList.AddTask(generateNodesNoises);

		// Show Nodes Noises
		ShowTextures showNodesNoises = new ShowTextures(diameter, resolution, parent, generateNodesNoises.GetResult);
		showNodesNoises.SetParams(1, previewProgress);
		taskList.AddTask(showNodesNoises);

		// Multiply Nodes Gradients and Noises
		MultiplyTextureLists multiplyGradientsAndNoises = new MultiplyTextureLists(generateNodesGradients.GetResult, generateNodesNoises.GetResult, maxNodes);
		taskList.AddTask(multiplyGradientsAndNoises);

		// Show Gradients and Noises Multiplication Result
		ShowTextures showMultiplicationResult = new ShowTextures(diameter, resolution, parent, multiplyGradientsAndNoises.GetResult);
		showMultiplicationResult.SetParams(1, previewProgress);
		taskList.AddTask(showMultiplicationResult);

		// Add Nodes Gradient Noises Together
		AddTextures addGradientNoises = new AddTextures(multiplyGradientsAndNoises.GetResult, maxNodes);
		taskList.AddTask(addGradientNoises);

		// Show Gradient Noises Addition Result
		ShowTexture showGradientNoisesAddition = new ShowTexture(diameter, resolution, parent, addGradientNoises.GetResult);
		taskList.AddTask(showGradientNoisesAddition);

		Initialized = true;
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
