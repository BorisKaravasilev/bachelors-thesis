﻿using ObjectPlacement.JitteredGrid;
using UnityEditor;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	[CustomEditor(typeof(IslandGenerator))]
	public class IslandGeneratorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			IslandGenerator islandGenerator = (IslandGenerator) target;

			GUILayout.Space(10);
			if (GUILayout.Button("Update Parameters"))
			{
				islandGenerator.UpdateGridParameters();
			}
		}
	}
}