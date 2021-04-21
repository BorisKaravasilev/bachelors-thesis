using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Instantiators.ObjectGrid;

namespace ProceduralGeneration.IslandGenerator
{
	public class IslandArea : GridObject
	{
		public bool Initialized { get; private set; }
		public bool Finished { get; private set; }
		
		private TaskList generationTasks;

		public void Init()
		{
		}

		public void GenerateStep()
		{
			new NotImplementedException();
		}
	}
}
