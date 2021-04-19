using UnityEngine;

namespace Instantiators.ObjectGrid
{
	[System.Serializable]
	public class GridParams
	{
		[Range(10, 40)] public int spacing = 15;

		[Range(5f, 20f)] public float maxObjectRadius = 5;

		[Range(1, 300)] public int objectCountLimit = 500;

		public bool destroyFarObjects = true;
	}
}