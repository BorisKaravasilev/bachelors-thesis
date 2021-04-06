public abstract class SingleTask
{
	public string Name { get; protected set; }
	public bool Finished => remainingSteps == 0;

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

	public int StepSize { get; protected set; }

	/// <summary>
	/// In range of 0 to 1.
	/// </summary>
	public float Progress
	{
		get
		{
			float executedSteps = totalSteps - remainingSteps;
			return executedSteps / totalSteps;
		}
	}

	public bool Enabled { get; set; }

	public SingleTask()
	{
		Name = "Single Task";
		Enabled = true;
		StepSize = 1;
	}

	/// <summary>
	/// Executes the whole task at once and returns the number of steps it took.
	/// </summary>
	public int Execute()
	{
		int executedSteps = 0;

		while (RemainingSteps > 0)
		{
			ExecuteStep();
			RemainingSteps--;
			executedSteps++;
		}

		PassTaskResults();
		return executedSteps;
	}

	/// <summary>
	/// Executes the number of steps defined by "StepSize" and returns the number of steps executed.
	/// </summary>
	public int ExecuteStepSize()
	{
		int executedSteps = 0;
		int stepsToExecute = StepSize;

		while (RemainingSteps > 0 && stepsToExecute > 0)
		{
			ExecuteStep();
			stepsToExecute--;
			RemainingSteps--;
			executedSteps++;
		}

		if (Finished) PassTaskResults();
		return executedSteps;
	}

	protected abstract void ExecuteStep();
	protected abstract void PassTaskResults();
}
