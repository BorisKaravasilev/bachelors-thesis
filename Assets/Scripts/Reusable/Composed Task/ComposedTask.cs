using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a list of SingleTasks. Tasks that are not enabled are ignored.
/// </summary>
public class ComposedTask
{
	public bool Finished { get; set; }

	private float progress;
	/// <summary>
	/// In range of 0 to 1.
	/// </summary>
	public float Progress
	{
		get { return progress; }
		protected set
		{
			if (value < 0f) progress = 0f;
			else if (value > 1f) progress = 1f;
			else progress = value;
		}
	}

	private List<SingleTask> taskList;

	public ComposedTask()
	{
		taskList = new List<SingleTask>();
	}

	public void AddTask(SingleTask newTask)
	{
		taskList.Add(newTask);
		Finished = AreAllTasksFinished();
		Progress = GetProgress();
	}

	public void Execute()
	{

	}

	public bool ExecuteStep()
	{
		SingleTask firstUnfinishedTask = FindFirstUnfinishedTask();
		firstUnfinishedTask?.ExecuteStep();
		return AreAllTasksFinished();
	}

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
