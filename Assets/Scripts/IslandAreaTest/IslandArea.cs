using UnityEngine;

public class IslandArea : GridObject
{
	//private Texture2D heightMap;
	//private TexturePreview heightMapPreview;
	
	private TaskList taskList;

	private const string DEFAULT_NAME = "IslandArea";
	private bool paramsAssigned = false;
	//private const int HM_RESOLUTION = 100;

	public IslandArea()
	{
		gameObject.name = DEFAULT_NAME;
		//heightMap = new Texture2D(HM_RESOLUTION, HM_RESOLUTION);
		taskList = new TaskList();
		taskList.DebugMode = true;
	}

	/// <summary>
	/// Assigns all necessary parameters for island area generation.
	/// </summary>
	public void AssignParams(bool previewProgress, GenerateTerrainNodesParams terrainNodesParams)
	{
		const int resolution = 99;

		// Display radius
		//Color[] blackPixels = TextureFunctions.GetBlackPixels(resolution * resolution);
		//ShowTexture showRadius = new ShowTexture(Parameters.Radius * 2, resolution, gameObject.transform, () => blackPixels);
		//taskList.AddTask(showRadius);

		// Terrain nodes
		GenerateTerrainNodes generateTerrainNodes = new GenerateTerrainNodes(terrainNodesParams, Parameters);
		taskList.AddTask(generateTerrainNodes);

		// Show Terrain nodes
		ShowTerrainNodes showTerrainNodes =
			new ShowTerrainNodes(Parameters.Radius / 10f, gameObject.transform, generateTerrainNodes.GetResult)
			{
				Enabled = previewProgress
			};
		taskList.AddTask(showTerrainNodes);

		// Height map
		//GenerateNodesHeightmap generateNodesHeightmap =
		//	new GenerateNodesHeightmap(resolution, Parameters.Radius, generateTerrainNodes.GetResult);

		// Height map (preview)
		//ShowTexture showNodesHeightmap =
		//	new ShowTexture(Parameters.Radius * 2, resolution, gameObject.transform, generateNodesHeightmap.GetResult)
		//	{
		//		Enabled = previewProgress
		//	};

		// Generate Nodes Gradients
		GenerateNodesGradients generateNodesGradients = new GenerateNodesGradients(resolution, Parameters.Radius, generateTerrainNodes.GetResult);
		taskList.AddTask(generateNodesGradients);

		// Show Gradients
		//ShowTextures showGradients = new ShowTextures(Parameters.Radius * 2, resolution, gameObject.transform, generateNodesGradients.GetResult);
		//taskList.AddTask(showGradients);

		// Add heightmaps
		AddTextures addHeightmaps = new AddTextures(generateNodesGradients.GetResult);
		taskList.AddTask(addHeightmaps);

		// Show added heightmaps
		ShowTexture showAddedHeightmaps = new ShowTexture(Parameters.Radius * 2, 0.01f, resolution, gameObject.transform, addHeightmaps.GetResult);
		showAddedHeightmaps.Enabled = previewProgress;
		taskList.AddTask(showAddedHeightmaps);

		// Generate Nodes Noise
		Noise2DParams noiseParams = new Noise2DParams(0.03f, gameObject.transform.position.x * 2.42f, gameObject.transform.position.z * 2.42f);
		GenerateNoiseHeightmap generateNoiseHeightmap = new GenerateNoiseHeightmap(resolution, noiseParams);
		taskList.AddTask(generateNoiseHeightmap);

		// Show Nodes Noise
		ShowTexture showNoiseHeightmap =
			new ShowTexture(Parameters.Radius * 2, 0.02f, resolution, gameObject.transform, generateNoiseHeightmap.GetResult)
			{
				Enabled = previewProgress
			};
		taskList.AddTask(showNoiseHeightmap);

		// Multiply Textures


		// Show Multiplied Textures


		paramsAssigned = true;
	}

	public GameObject Generate()
	{
		if (paramsAssigned)
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
		if (paramsAssigned)
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
