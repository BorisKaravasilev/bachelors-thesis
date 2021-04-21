using UnityEngine;

namespace TaskManagement
{
	public abstract class DividableTask
	{
		public string Name { get; set; }
		public bool Finished => RemainingSteps == 0 && !IsWaiting;
		public bool NotStarted => RemainingSteps == TotalSteps;

		private int totalSteps = 1;

		public int TotalSteps
		{
			get => totalSteps;
			protected set => totalSteps = value < 1 ? 1 : value;
		}

		private int remainingSteps = 1;

		public int RemainingSteps
		{
			get => remainingSteps;
			protected set
			{
				if (value < 0) remainingSteps = 0;
				else if (value > totalSteps) remainingSteps = totalSteps;
				else remainingSteps = value;
			}
		}

		public int ExecutedSteps => TotalSteps - RemainingSteps;
		public int StepSize { get; set; }
		public float MinStepDuration { get; set; } // For visualization purposes

		/// <summary>
		/// In range of 0 to 1.
		/// </summary>
		public float Progress => (float) ExecutedSteps / totalSteps;

		public bool Enabled { get; set; }

		public bool IsWaiting { get; private set; }
		private float lastStepStartTime;

		public DividableTask(string name = "Single Task")
		{
			Name = name;
			Enabled = true;
			StepSize = 1;
			MinStepDuration = 0f;
			lastStepStartTime = 0f;
			IsWaiting = false;
		}

		public void SetParams(int stepSize, bool enabled = true, float minStepDuration = 0)
		{
			StepSize = stepSize;
			Enabled = enabled;
			MinStepDuration = minStepDuration;
		}

		/// <summary>
		/// Executes the whole task at once and returns the number of steps it took.
		/// </summary>
		public int Execute()
		{
			if (NotStarted)
			{
				GetInputFromPreviousStep();
				SetSteps();
			}

			int executedSteps = 0;

			while (RemainingSteps > 0)
			{
				ExecuteStep();
				RemainingSteps--;
				executedSteps++;
			}

			return executedSteps;
		}

		/// <summary>
		/// Executes the number of steps defined by "StepSize" and returns the number of steps executed.
		/// </summary>
		public int ExecuteStepSize()
		{
			if (IsWaiting)
			{
				if (MinStepDurationElapsed(MinStepDuration))
				{
					IsWaiting = false;
				}

				return 0;
			}

			if (NotStarted)
			{
				GetInputFromPreviousStep();
				SetSteps();
			}
			else
			{
				if (!MinStepDurationElapsed(MinStepDuration)) return 0;
			}

			lastStepStartTime = Time.realtimeSinceStartup;
			int executedSteps = 0;
			int stepsToExecute = StepSize;

			while (RemainingSteps > 0 && stepsToExecute > 0)
			{
				ExecuteStep();
				stepsToExecute--;
				RemainingSteps--;
				executedSteps++;
			}

			if (RemainingSteps == 0 && !MinStepDurationElapsed(MinStepDuration)) IsWaiting = true;

			return executedSteps;
		}

		private bool MinStepDurationElapsed(float minStepDuration)
		{
			float intervalBetweenSteps = Time.realtimeSinceStartup - lastStepStartTime;

			if (intervalBetweenSteps > minStepDuration)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Defines how a single step is executed.
		/// </summary>
		protected abstract void ExecuteStep();

		/// <summary>
		/// Gets called before the first step is executed.
		/// </summary>
		protected abstract void GetInputFromPreviousStep();

		/// <summary>
		/// Gets called before the first step, but after "GetInputFromPreviousStep()" is called.
		/// "TotalSteps", "RemainingSteps" and "StepSize" can be set here.
		/// </summary>
		protected abstract void SetSteps();
	}
}