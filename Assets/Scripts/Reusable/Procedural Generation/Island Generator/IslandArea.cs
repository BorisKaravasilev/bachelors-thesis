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

		/// <summary>
		/// Executes a step of the generation process.
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Initializes all the partial tasks and adds them to a task list.
		/// </summary>
		public void Init(IslandGenerationParams generationParams)
		{
			this.generationParams = generationParams;
			Type = DetermineIslandType(generationParams.IslandTypes);

			InstantiateProgressText();

			// Add generation tasks to the task list
			GenerateTerrainNodes generateTerrainNodes = AddGenerateNodesTask();
			ShowTerrainNodes showTerrainNodes = AddShowTerrainNodesTask(generateTerrainNodes);
			GenerateNodesGradients generateNodesGradients = AddGenerateNodesGradientsTask(generateTerrainNodes);
			ShowTextures showGradients = AddShowTexturesTask("Show Node Gradients", generateNodesGradients.GetResult);
			HideObjects<IHideable> hideGradients = AddHideObjectsTask("Hide Node Gradients", showGradients.GetResult);
			GenerateNodesNoises generateNodesNoises = AddGenerateNodesNoisesTask(generateTerrainNodes);
			ShowTextures showNodesNoises = AddShowTexturesTask("Show Nodes Noises", generateNodesNoises.GetResult);
			HideObjects<IHideable> hideNodesNoises = AddHideObjectsTask("Hide Nodes Noises", showNodesNoises.GetResult);

			// Multiply Nodes Gradients and Noises TODO: This
			//MultiplyTextureLists multiplyGradientsAndNoises = new MultiplyTextureLists(generateNodesGradients.GetResult, generateNodesNoises.GetResult, 1);
			//multiplyGradientsAndNoises.Name = "Multiply Node Gradients and Noises";
			//taskList.AddTask(multiplyGradientsAndNoises);

			Initialized = true;
		}

		#region Generation Tasks Setup

		/// <summary>
		/// Initializes and adds to task list the "Generate Terrain Nodes" task.
		/// </summary>
		private GenerateTerrainNodes AddGenerateNodesTask()
		{
			GenerateTerrainNodes generateTerrainNodes = new GenerateTerrainNodes(Type.TerrainNodesParams, GetParams());
			generateTerrainNodes.SetParams(Type.TerrainNodesParams.MaxNodes);
			taskList.AddTask(generateTerrainNodes);

			return generateTerrainNodes;
		}

		/// <summary>
		/// Initializes and adds to task list the "Show Terrain nodes" task.
		/// </summary>
		private ShowTerrainNodes AddShowTerrainNodesTask(GenerateTerrainNodes generateTerrainNodes)
		{
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

		/// <summary>
		/// Initializes and adds to task list the "Generate Nodes Gradients" task.
		/// </summary>
		private GenerateNodesGradients AddGenerateNodesGradientsTask(GenerateTerrainNodes generateTerrainNodes)
		{
			GenerateNodesGradients generateNodesGradients = new GenerateNodesGradients(GetResolution(), Radius, generateTerrainNodes.GetResult);
			taskList.AddTask(generateNodesGradients);

			return generateNodesGradients;
		}

		/// <summary>
		/// Initializes and adds to task list any "Show Textures" task.
		/// </summary>
		private ShowTextures AddShowTexturesTask(string name, Func<List<Color[]>> getTextures)
		{
			// Show Gradients
			ShowTexturesParams parameters = new ShowTexturesParams
			{
				Resolution = GetResolution(),
				GetTextures = getTextures,
				Parent = gameObject.transform,
				SideLength = Radius * 2,
				TexturePreviewMaterial = generationParams.TexturePreviewMaterial
			};

			ShowTextures showTextures = new ShowTextures(parameters);
			showTextures.SetParams(1, generationParams.PreviewProgress, generationParams.VisualStepTime);
			showTextures.Name = name;
			taskList.AddTask(showTextures);

			return showTextures;
		}

		/// <summary>
		/// Initializes and adds to task list any "Hide Objects" task.
		/// </summary>
		private HideObjects<IHideable> AddHideObjectsTask(string name, Func<List<IHideable>> getPreviewObjects)
		{
			HideObjects<IHideable> hideObjects = new HideObjects<IHideable>(getPreviewObjects);
			hideObjects.SetParams(1, generationParams.PreviewProgress);
			hideObjects.Name = name;
			taskList.AddTask(hideObjects);

			return hideObjects;
		}

		/// <summary>
		/// Initializes and adds to task list the "Generate Nodes Noises" task.
		/// </summary>
		private GenerateNodesNoises AddGenerateNodesNoisesTask(GenerateTerrainNodes generateTerrainNodes)
		{
			GenerateNodesNoises generateNodesNoises = new GenerateNodesNoises(Position, Radius, GetResolution(), generateTerrainNodes.GetResult);
			taskList.AddTask(generateNodesNoises);
			return generateNodesNoises;
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

		/// <summary>
		/// Calculates the resolution of the textures for this area from the pixels per unit.
		/// </summary>
		private int GetResolution()
		{
			float diameter = Radius * 2;
			return (int)(diameter * generationParams.PixelsPerUnit);
		}

		/// <summary>
		/// Randomly chooses an island type for this island area.
		/// </summary>
		private IslandType DetermineIslandType(List<IslandType> islandTypes)
		{
			RandomFromSeed randomGenerator = new RandomFromSeed(Position, "IslandTypePicker");
			int randomIndex = randomGenerator.PickItemIndex(islandTypes.Cast<IHasProbability>().ToList());

			if (randomIndex < 0) Debug.LogError("No island type index was randomly picked.");
			return islandTypes[randomIndex];
		}

		/// <summary>
		/// Prints error message with hint.
		/// </summary>
		private void PrintErrorParamsNotAssigned(string assignmentFuncName)
		{
			Debug.LogError(
				$"{DEFAULT_NAME}'s parameters not assigned. Call \"{assignmentFuncName}\" first."
			);
		}
	}
}
