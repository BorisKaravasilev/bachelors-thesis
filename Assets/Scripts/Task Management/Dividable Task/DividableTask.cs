using UnityEngine;

namespace TaskManagement
{
	/// <summary>
	/// Task that can be divided into multiple steps.
	/// </summary>
	public abstract class DividableTask
	{
		public string Name { get; set; }
		public bool Finished => RemainingSteps == 0 && !IsWaiting;
		public bool NotStarted => RemainingSteps == TotalSteps;

		private int totalSteps = 1;

		public int TotalSteps
		{
			get => totalSteps;
			protected set => totalSteps = value < 0 ? 0 : value;
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
		public float MinExecutionTime { get; set; } // Guarantees a minimum execution time (task can wait for visualization)
		public float MaxExecutionTime { get; set; } // Regulates how many steps should be executed to keep good frame rate

		/// <summary>
		/// In range of 0 to 1.
		/// </summary>
		public float Progress => totalSteps == 0 ? 1f : (float) ExecutedSteps / totalSteps;

		public bool Enabled { get; set; }

		public bool IsWaiting { get; private set; }
		private float lastStepStartTime;

		public DividableTask(string name = "Dividable Task")
		{
			Name = name;
			Enabled = true;
			MinExecutionTime = 0f;
			lastStepStartTime = 0f;
			IsWaiting = false;
			MaxExecutionTime = float.MaxValue;
		}

		/// <summary>
		/// Sets tasks parameters.
		/// </summary>
		public void SetParams(float maxExecutionTime, bool enabled = true, float minExecutionTime = 0)
		{
			Enabled = enabled;
			MinExecutionTime = minExecutionTime;
			MaxExecutionTime = maxExecutionTime;
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
				if (MinExecutionTimeElapsed())
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
				if (!MinExecutionTimeElapsed()) return 0;
			}

			lastStepStartTime = Time.realtimeSinceStartup;
			int executedSteps = 0;

			while (RemainingSteps > 0 && !MaxExecutionTimeElapsed())
			{
				ExecuteStep();
				RemainingSteps--;
				executedSteps++;
			}

			if (RemainingSteps == 0 && !MinExecutionTimeElapsed()) IsWaiting = true;

			return executedSteps;
		}

		/// <summary>
		/// Returns true if the time of execution is longer than the minimum.
		/// </summary>
		private bool MinExecutionTimeElapsed()
		{
			float interval = Time.realtimeSinceStartup - lastStepStartTime;

			if (interval > MinExecutionTime)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns true if the time of execution is longer than the maximum.
		/// </summary>
		private bool MaxExecutionTimeElapsed()
		{
			float interval = Time.realtimeSinceStartup - lastStepStartTime;

			if (interval > MaxExecutionTime)
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