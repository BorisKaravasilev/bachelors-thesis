using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleTask
{
	public string Name { get; protected set; }
	public bool Finished { get; protected set; }

	private float progress;
	/// <summary>
	/// In range of 0 to 1.
	/// </summary>
	public float Progress
	{
		get { return progress;}
		protected set
		{
			if (value < 0f) progress = 0f;
			else if (value > 1f) progress = 1f;
			else progress = value;
		}
	}

	public bool Enabled { get; set; }

	public SingleTask()
	{
		Name = "Single Task";
		Finished = false;
		Progress = 0f;
		Enabled = true;
	}

	/// <summary>
	/// Executes the whole task at once.
	/// </summary>
	public abstract void Execute();

	/// <summary>
	/// Executes a part of the task and returns true if finished.
	/// </summary>
	public abstract bool ExecuteStep();
}
