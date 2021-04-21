using Instantiators.ObjectGrid;
using MyRandom;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManagement;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
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

		public void Init(IslandGenerationParams generationParams)
		{
			this.generationParams = generationParams;
			Type = DetermineIslandType(generationParams.IslandTypes);

			InstantiateProgressText();

			// Add generation tasks to the task list
			GenerateTerrainNodes generateTerrainNodes = AddGenerateNodesTask();
			ShowTerrainNodes showTerrainNodes = AddShowTerrainNodesTask(generateTerrainNodes);
			GenerateNodesGradients generateNodesGradients = AddGenerateNodesGradientsTask(generateTerrainNodes);
			ShowTextures showGradients = AddShowGradientsTask(generateNodesGradients);

			Initialized = true;
		}

		#region Generation Tasks Setup
		
		private GenerateTerrainNodes AddGenerateNodesTask()
		{
			// Generate Terrain nodes
			GenerateTerrainNodes generateTerrainNodes = new GenerateTerrainNodes(Type.TerrainNodesParams, GetParams());
			generateTerrainNodes.SetParams(Type.TerrainNodesParams.MaxNodes);
			taskList.AddTask(generateTerrainNodes);

			return generateTerrainNodes;
		}

		private ShowTerrainNodes AddShowTerrainNodesTask(GenerateTerrainNodes generateTerrainNodes)
		{
			// Show Terrain nodes
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

		private GenerateNodesGradients AddGenerateNodesGradientsTask(GenerateTerrainNodes generateTerrainNodes)
		{
			// Generate Nodes Gradients
			GenerateNodesGradients generateNodesGradients = new GenerateNodesGradients(GetResolution(), Radius, generateTerrainNodes.GetResult);
			taskList.AddTask(generateNodesGradients);

			return generateNodesGradients;
		}

		private ShowTextures AddShowGradientsTask(GenerateNodesGradients generateNodesGradients)
		{
			// Show Gradients
			ShowTexturesParams parameters = new ShowTexturesParams
			{
				Resolution = GetResolution(),
				GetTextures = generateNodesGradients.GetResult,
				Parent = gameObject.transform,
				SideLength = Radius * 2,
				TexturePreviewMaterial = generationParams.TexturePreviewMaterial
			};

			ShowTextures showGradients = new ShowTextures(parameters);
			showGradients.SetParams(1, generationParams.PreviewProgress, generationParams.VisualStepTime);
			showGradients.Name = "Show Node Gradients";
			taskList.AddTask(showGradients);

			return showGradients;
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

		private int GetResolution()
		{
			float diameter = Radius * 2;
			return (int)(diameter * generationParams.PixelsPerUnit);
		}

		private IslandType DetermineIslandType(List<IslandType> islandTypes)
		{
			RandomFromSeed randomGenerator = new RandomFromSeed(Position, "IslandTypePicker");
			int randomIndex = randomGenerator.PickItemIndex(islandTypes.Cast<IHasProbability>().ToList());

			if (randomIndex < 0) Debug.LogError("No island type index was randomly picked.");
			return islandTypes[randomIndex];
		}

		private void PrintErrorParamsNotAssigned(string assignmentFuncName)
		{
			Debug.LogError(
				$"{DEFAULT_NAME}'s parameters not assigned. Call \"{assignmentFuncName}\" first."
			);
		}
	}
}
