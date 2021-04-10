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

		// TODO: Calculate and assign round transparent mask

		// Generate Terrain nodes
		GenerateTerrainNodes generateTerrainNodes = new GenerateTerrainNodes(terrainNodesParams, Parameters, terrainNodesParams.MaxNodes);
		taskList.AddTask(generateTerrainNodes);

		// Show Terrain nodes
		ShowTerrainNodes showTerrainNodes =
			new ShowTerrainNodes(Parameters.Radius / 10f, gameObject.transform, generateTerrainNodes.GetResult, terrainNodesParams.MaxNodes)
			{
				Enabled = previewProgress
			};
		taskList.AddTask(showTerrainNodes);

		// Generate Nodes Gradients
		GenerateNodesGradients generateNodesGradients = new GenerateNodesGradients(resolution, Parameters.Radius, generateTerrainNodes.GetResult, terrainNodesParams.MaxNodes);
		taskList.AddTask(generateNodesGradients);

		// Show Gradients
		ShowTextures showGradients = new ShowTextures(Parameters.Radius * 2, 0.01f, resolution, gameObject.transform,
				generateNodesGradients.GetResult)
			{Enabled = false};//previewProgress };
		taskList.AddTask(showGradients);

		// Generate Nodes Noises
		GenerateNodesNoises generateNodesNoises = new GenerateNodesNoises(resolution, generateTerrainNodes.GetResult, terrainNodesParams.MaxNodes);
		taskList.AddTask(generateNodesNoises);

		// Show Nodes Noises
		ShowTextures showNodesNoises = new ShowTextures(Parameters.Radius * 2, 0.01f * terrainNodesParams.MaxNodes + 0.2f, resolution, gameObject.transform,
				generateNodesNoises.GetResult)
			{ Enabled = previewProgress };
		taskList.AddTask(showNodesNoises);

		// Multiply Nodes Gradients and Noises
		MultiplyTextureLists multiplyGradientsAndNoises = new MultiplyTextureLists(generateNodesGradients.GetResult, generateNodesNoises.GetResult, terrainNodesParams.MaxNodes);
		taskList.AddTask(multiplyGradientsAndNoises);

		// Show Gradients and Noises Multiplication Result
		ShowTextures showMultiplicationResult = new ShowTextures(Parameters.Radius * 2, 0.01f * terrainNodesParams.MaxNodes + 0.6f, resolution, gameObject.transform,
				multiplyGradientsAndNoises.GetResult)
			{ Enabled = previewProgress };
		taskList.AddTask(showMultiplicationResult);

		// Add Nodes Gradient Noises Together
		AddTextures addGradientNoises = new AddTextures(multiplyGradientsAndNoises.GetResult, terrainNodesParams.MaxNodes);
		taskList.AddTask(addGradientNoises);

		// Show Gradient Noises Addition Result
		ShowTexture showGradientNoisesAddition = new ShowTexture(Parameters.Radius * 2, 0.01f * terrainNodesParams.MaxNodes + 2f, resolution, gameObject.transform, addGradientNoises.GetResult);
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
