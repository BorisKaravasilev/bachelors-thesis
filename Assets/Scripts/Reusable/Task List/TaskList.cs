using System.Collections.Generic;
using UnityEngine;

namespace TaskManagement
{
	/// <summary>
	/// Manages a list of SingleTasks. Tasks that are not enabled are ignored.
	/// </summary>
	public class TaskList
	{
		public bool Finished => AreAllTasksFinished();

		/// <summary>
		/// In range of 0 to 1.
		/// </summary>
		public float Progress => GetProgress();

		public DividableTask CurrentTask => FindFirstUnfinishedTask();
		public bool DebugMode { get; set; }
		public int ExecutedTasks { get; private set; }

		private List<DividableTask> taskList;

		public TaskList()
		{
			taskList = new List<DividableTask>();
			DebugMode = false;
			ExecutedTasks = 0;
		}

		public void AddTask(DividableTask newTask)
		{
			taskList.Add(newTask);
		}

		/// <summary>
		/// Executes all tasks at once and returns the number of steps that it took.
		/// </summary>
		public int Execute()
		{
			int executedSteps = 0;
			DividableTask firstUnfinishedTask = FindFirstUnfinishedTask();

			while (firstUnfinishedTask != null)
			{
				float timeBeforeExecution = 0;
				if (DebugMode) timeBeforeExecution = Time.realtimeSinceStartup;

				executedSteps += firstUnfinishedTask.Execute();
				ExecutedTasks++;

				LogExecutionStep(firstUnfinishedTask, executedSteps, timeBeforeExecution);
				firstUnfinishedTask = FindFirstUnfinishedTask();
			}

			return executedSteps;
		}

		/// <summary>
		/// Executes the number of steps defined by "StepSize" in each sub-task and returns the number of steps executed.
		/// </summary>
		public int ExecuteStepSize()
		{
			int executedSteps = 0;
			DividableTask firstUnfinishedTask = FindFirstUnfinishedTask();

			if (firstUnfinishedTask != null)
			{
				float timeBeforeExecution = 0;
				if (DebugMode) timeBeforeExecution = Time.realtimeSinceStartup;

				executedSteps = firstUnfinishedTask.ExecuteStepSize();
				if (firstUnfinishedTask.Finished) ExecutedTasks++;

				LogExecutionStep(firstUnfinishedTask, executedSteps, timeBeforeExecution);
			}

			return executedSteps;
		}

		/// <summary>
		/// Returns the first enabled unfinished task in the "taskList" or null if none is found.
		/// </summary>
		private DividableTask FindFirstUnfinishedTask()
		{
			DividableTask firstUnfinishedTask = null;

			foreach (DividableTask task in taskList)
			{
				if (task.Enabled && !task.Finished)
				{
					firstUnfinishedTask = task;
					break;
				}
			}

			return firstUnfinishedTask;
		}

		private bool AreAllTasksFinished()
		{
			bool allTasksFinished = true;

			taskList.ForEach(task =>
			{
				if (task.Enabled && task.Finished == false) allTasksFinished = false;
			});

			return allTasksFinished;
		}

		private float GetProgress()
		{
			float sumOfProgess = 0f;

			taskList.ForEach(task =>
			{
				if (task.Enabled)
				{
					sumOfProgess += task.Progress;
				}
			});

			float averageProgress = sumOfProgess / GetEnabledTasksCount();
			return averageProgress;
		}

		private int GetEnabledTasksCount()
		{
			int enabledTasks = 0;

			taskList.ForEach(task =>
			{
				if (task.Enabled) enabledTasks += 1;
			});

			return enabledTasks;
		}

		private void LogExecutionStep(DividableTask task, int executedSteps, float timeBeforeExecution)
		{
			if (DebugMode)
			{
				float executionTimeMs = (Time.realtimeSinceStartup - timeBeforeExecution) * 1000;
				Debug.Log(
					$"                                     {task.Name} ({task.Progress:P0})                           " +
					$"Executed steps: {executedSteps} (in {executionTimeMs:0.}ms)\n(Total: {Progress:P0})");
			}
		}
	}
}