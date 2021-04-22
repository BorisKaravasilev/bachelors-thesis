using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ObjectPlacement.JitteredGrid
{
	[CreateAssetMenu]
	class JitteredGridParams : ScriptableObject
	{
		[SerializeField] public GridParams parameters;
		[SerializeField] public OffsetParams offsetParams;
	}
}
