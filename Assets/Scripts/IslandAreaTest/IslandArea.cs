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
	public void AssignParams(bool previewProgress, GenerateTerrainNodesParams terrainNodesParams)
	{
		const int resolution = 99;

		// Calculate and assign round transparent mask

		// Terrain nodes
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
		//ShowTextures showGradients = new ShowTextures(Parameters.Radius * 2, resolution, gameObject.transform, generateNodesGradients.GetResult);
		//taskList.AddTask(showGradients);

		// Add heightmaps
		AddTextures addHeightmaps = new AddTextures(generateNodesGradients.GetResult, terrainNodesParams.MaxNodes);
		taskList.AddTask(addHeightmaps);

		// Show added heightmaps
		ShowTexture showAddedHeightmaps = new ShowTexture(Parameters.Radius * 2, 0.01f, resolution, gameObject.transform, addHeightmaps.GetResult);
		showAddedHeightmaps.Enabled = previewProgress;
		taskList.AddTask(showAddedHeightmaps);

		// Generate Nodes Noise
		Noise2DParams noiseParams = new Noise2DParams(0.05f, gameObject.transform.position.x * 2.42f, gameObject.transform.position.z * 2.42f);
		GenerateNoiseHeightmap generateNoiseHeightmap = new GenerateNoiseHeightmap(resolution, noiseParams, resolution * resolution);
		taskList.AddTask(generateNoiseHeightmap);

		// Show Nodes Noise
		ShowTexture showNoiseHeightmap =
			new ShowTexture(Parameters.Radius * 2, 0.02f, resolution, gameObject.transform, generateNoiseHeightmap.GetResult)
			{
				Enabled = previewProgress
			};
		taskList.AddTask(showNoiseHeightmap);

		// Multiply Nodes Gradients and Noises
		MultiplyTextureLists multiplyGradientsAndNoises = new MultiplyTextureLists(addHeightmaps.GetResultInList, generateNoiseHeightmap.GetResultInList, terrainNodesParams.MaxNodes);
		taskList.AddTask(multiplyGradientsAndNoises);

		// Show Multiplied Textures
		ShowTextures showMultiplicationResult = new ShowTextures(Parameters.Radius * 2, 0.03f, resolution, gameObject.transform, multiplyGradientsAndNoises.GetResult);
		taskList.AddTask(showMultiplicationResult);

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
