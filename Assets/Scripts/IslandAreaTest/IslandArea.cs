using System;
using System.Collections;
using System.Collections.Generic;
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
		GenerateTerrainNodesPreviews generateTerrainNodesPreviews = new GenerateTerrainNodesPreviews(Parameters.Radius / 10f, gameObject.transform, generateTerrainNodes.GetResult);
		generateTerrainNodesPreviews.Enabled = previewProgress;

		generateAreaTask.AddTask(generateTerrainNodes);
		generateAreaTask.AddTask(generateTerrainNodesPreviews);

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

	//public override void Destroy()
	//{
	//	//terrainNodes.ForEach(node => node.Destroy()); TODO: Destroy terrain nodes previews
	//	terrainNodes.Clear();

	//	heightMapPreview?.Destroy();

	//	base.Destroy();
	//}




	//private void ShowHeightMapPreview()
	//{
	//	if (heightMapPreview == null)
	//	{
	//		heightMapPreview = new TexturePreview(gameObject.transform);
	//		heightMapPreview.SetDimensions(new Vector3(2f * Parameters.Radius, 1, 2f * Parameters.Radius));
	//	}
	//	else
	//	{
	//		heightMapPreview.Show();
	//	}
	//}

	//private void HideHeightMapPreview()
	//{
	//	heightMapPreview?.Hide();
	//}

	//private void ShowTerrainNodes()
	//{
	//	terrainNodes?.ForEach(node => node.ShowPreview(gameObject.transform, nodePreviewDimensions));
	//}

	//private void HideTerrainNodes()
	//{
	//	terrainNodes?.ForEach(node => node.HidePreview());
	//}

	//private void SetTerrainNodes(List<TerrainNode> newTerrainNodes)
	//{
	//	terrainNodes = newTerrainNodes;
	//}

	//private void SetTerrainNodesPreviews(List<TerrainNodePreview> newTerrainNodesPreviews)
	//{
	//	terrainNodesPreviews = newTerrainNodesPreviews;
	//}
}
