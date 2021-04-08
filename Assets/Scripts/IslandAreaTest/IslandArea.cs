using UnityEngine;

public class IslandArea : GridObject
{
	//private Texture2D heightMap;
	//private TexturePreview heightMapPreview;
	
	private ComposedTask generateAreaTask;

	private const string DEFAULT_NAME = "IslandArea";
	private bool paramsAssigned = false;
	//private const int HM_RESOLUTION = 100;

	public IslandArea()
	{
		gameObject.name = DEFAULT_NAME;
		//heightMap = new Texture2D(HM_RESOLUTION, HM_RESOLUTION);
		generateAreaTask = new ComposedTask();
	}

	/// <summary>
	/// Assigns all necessary parameters for island area generation.
	/// </summary>
	public void AssignParams(bool previewProgress, GenerateTerrainNodesParams terrainNodesParams)
	{
		// Terrain nodes
		GenerateTerrainNodes generateTerrainNodes = new GenerateTerrainNodes(terrainNodesParams, Parameters);

		// Terrain nodes (previews)
		ShowTerrainNodes showTerrainNodes =
			new ShowTerrainNodes(Parameters.Radius / 10f, gameObject.transform, generateTerrainNodes.GetResult)
			{
				Enabled = previewProgress
			};

		// Height map
		const int resolution = 99;

		GenerateNodesHeightmap generateNodesHeightmap =
			new GenerateNodesHeightmap(resolution, Parameters.Radius, generateTerrainNodes.GetResult);

		// Height map (preview)
		ShowTexture showNodesHeightmap =
			new ShowTexture(Parameters.Radius * 2, resolution, gameObject.transform, generateNodesHeightmap.GetResult)
			{
				Enabled = previewProgress
			};

		// Generate Nodes Noise
		Noise2DParams noiseParams = new Noise2DParams(0.01f, 11.5f, 29.43f);
		GenerateNoiseHeightmap generateNoiseHeightmap = new GenerateNoiseHeightmap(resolution, noiseParams);

		// Show Nodes Noise
		ShowTexture showNoiseHeightmap =
			new ShowTexture(Parameters.Radius * 2, resolution, gameObject.transform, generateNoiseHeightmap.GetResult)
			{
				Enabled = previewProgress
			};

		// Composed task
		generateAreaTask.AddTask(generateTerrainNodes);
		generateAreaTask.AddTask(showTerrainNodes);
		generateAreaTask.AddTask(generateNodesHeightmap);
		generateAreaTask.AddTask(showNodesHeightmap);
		//generateAreaTask.AddTask(generateNoiseHeightmap);
		//generateAreaTask.AddTask(showNoiseHeightmap);

		paramsAssigned = true;
	}

	public GameObject Generate()
	{
		if (paramsAssigned)
		{
			generateAreaTask.Execute();
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
			generateAreaTask.ExecuteStepSize();
			Debug.Log($"Progress of composed task is: {generateAreaTask.Progress}");
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
