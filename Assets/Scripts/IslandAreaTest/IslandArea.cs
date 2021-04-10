using UnityEditor;
using UnityEngine;

public class IslandArea : GridObject
{
	private TaskList taskList;

	private const string DEFAULT_NAME = "IslandArea";

	public bool ParamsAssigned { get; private set; }
	public bool Finished => taskList?.Finished ?? false;

	public IslandArea()
	{
		gameObject.name = DEFAULT_NAME;
		//heightMap = new Texture2D(HM_RESOLUTION, HM_RESOLUTION);
		taskList = new TaskList();
		taskList.DebugMode = true;
		ParamsAssigned = false;
	}

	/// <summary>
	/// Assigns all necessary parameters for island area generation.
	/// </summary>
	public void AssignParams(bool previewProgress, TerrainNodesParams terrainNodesParams)
	{
		const int resolution = 99;
		int maxNodes = terrainNodesParams.MaxNodes;
		float nodePreviewRadius = Parameters.Radius / 10f;
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
		GenerateNodesGradients generateNodesGradients = new GenerateNodesGradients(resolution, Parameters.Radius, generateTerrainNodes.GetResult, maxNodes);
		taskList.AddTask(generateNodesGradients);

		// Show Gradients
		ShowTextures showGradients = new ShowTextures(Parameters.Radius * 2, 0.01f, resolution, parent,
				generateNodesGradients.GetResult)
			{Enabled = false};//previewProgress };
		taskList.AddTask(showGradients);

		// Generate Nodes Noises
		GenerateNodesNoises generateNodesNoises = new GenerateNodesNoises(resolution, generateTerrainNodes.GetResult, maxNodes);
		taskList.AddTask(generateNodesNoises);

		// Show Nodes Noises
		ShowTextures showNodesNoises = new ShowTextures(Parameters.Radius * 2, 0.01f * maxNodes + 0.2f, resolution, parent,
				generateNodesNoises.GetResult)
			{ Enabled = previewProgress };
		taskList.AddTask(showNodesNoises);

		// Multiply Nodes Gradients and Noises
		MultiplyTextureLists multiplyGradientsAndNoises = new MultiplyTextureLists(generateNodesGradients.GetResult, generateNodesNoises.GetResult, maxNodes);
		taskList.AddTask(multiplyGradientsAndNoises);

		// Show Gradients and Noises Multiplication Result
		ShowTextures showMultiplicationResult = new ShowTextures(Parameters.Radius * 2, 0.01f * maxNodes + 0.6f, resolution, parent,
				multiplyGradientsAndNoises.GetResult)
			{ Enabled = previewProgress };
		taskList.AddTask(showMultiplicationResult);

		// Add Nodes Gradient Noises Together
		AddTextures addGradientNoises = new AddTextures(multiplyGradientsAndNoises.GetResult, maxNodes);
		taskList.AddTask(addGradientNoises);

		// Show Gradient Noises Addition Result
		ShowTexture showGradientNoisesAddition = new ShowTexture(Parameters.Radius * 2, 0.01f * maxNodes + 2f, resolution, parent, addGradientNoises.GetResult);
		taskList.AddTask(showGradientNoisesAddition);

		ParamsAssigned = true;
	}

	public GameObject Generate()
	{
		if (ParamsAssigned)
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
		if (ParamsAssigned)
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
