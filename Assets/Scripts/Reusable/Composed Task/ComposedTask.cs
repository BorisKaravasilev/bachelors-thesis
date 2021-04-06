using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a list of SingleTasks. Tasks that are not enabled are ignored.
/// </summary>
public class ComposedTask
{
	public bool Finished => AreAllTasksFinished();

	/// <summary>
	/// In range of 0 to 1.
	/// </summary>
	public float Progress => GetProgress();

	private List<SingleTask> taskList;

	public ComposedTask()
	{
		taskList = new List<SingleTask>();
	}

	public void AddTask(SingleTask newTask)
	{
		taskList.Add(newTask);
	}

	/// <summary>
	/// Executes all tasks at once and returns the number of steps that it took.
	/// </summary>
	public int Execute()
	{
		int executedSteps = 0;
		SingleTask firstUnfinishedTask = FindFirstUnfinishedTask();

		while (firstUnfinishedTask != null)
		{
			executedSteps += firstUnfinishedTask.Execute();
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
		SingleTask firstUnfinishedTask = FindFirstUnfinishedTask();

		if (firstUnfinishedTask != null)
		{
			executedSteps = firstUnfinishedTask.ExecuteStepSize();
		}

		return executedSteps;
	}

	/// <summary>
	/// Returns the first enabled unfinished task in the "taskList" or null if none is found.
	/// </summary>
	private SingleTask FindFirstUnfinishedTask()
	{
		SingleTask firstUnfinishedTask = null;

		foreach (SingleTask task in taskList)
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
}
