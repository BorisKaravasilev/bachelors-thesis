using Instantiators.ObjectGrid;
using MyRandom;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManagement;

namespace ProceduralGeneration.IslandGenerator
{
	public class IslandArea : GridObject
	{
		public IslandType Type { get; private set; }
		public bool Initialized { get; private set; }
		public bool Finished { get; private set; }
		
		private TaskList taskList;

		public void Init(IslandGenerationParams generationParams)
		{
			Type = DetermineIslandType(generationParams.IslandTypes);

			// Generate Terrain nodes
			GenerateTerrainNodes generateTerrainNodes = new GenerateTerrainNodes(Type.TerrainNodesParams, GetParams());
			generateTerrainNodes.SetParams(Type.TerrainNodesParams.MaxNodes);
			taskList.AddTask(generateTerrainNodes);

			// Show Terrain nodes
			ShowTerrainNodesParams previewParams = new ShowTerrainNodesParams();
			previewParams.AreaRadius = Radius;
			previewParams.Parent = gameObject.transform;
			previewParams.PreviewMaterial = generationParams.PreviewsMaterial;
			previewParams.GetTerrainNodes = generateTerrainNodes.GetResult;

			ShowTerrainNodes showTerrainNodes = new ShowTerrainNodes(previewParams);
			showTerrainNodes.SetParams(1, generationParams.PreviewProgress, generationParams.VisualStepTime);
			taskList.AddTask(showTerrainNodes);
		}

		private IslandType DetermineIslandType(List<IslandType> islandTypes)
		{
			RandomFromSeed randomGenerator = new RandomFromSeed(Position, "IslandTypePicker");
			List<IHasProbability> typesToPickFrom = islandTypes.Cast<IHasProbability>().ToList();
			return (IslandType) randomGenerator.PickItem(typesToPickFrom);
		}

		public void GenerateStep()
		{
			new NotImplementedException();
		}
	}
}
