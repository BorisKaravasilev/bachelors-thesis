public abstract class SingleTask
{
	public string Name { get; protected set; }
	public bool Finished => RemainingSteps == 0;
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
	public int StepSize { get; protected set; }

	/// <summary>
	/// In range of 0 to 1.
	/// </summary>
	public float Progress => (float) ExecutedSteps / totalSteps;

	public bool Enabled { get; set; }

	public SingleTask(string name = "Single Task")
	{
		Name = name;
		Enabled = true;
		StepSize = 1;
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
		if (NotStarted)
		{
			GetInputFromPreviousStep();
			SetSteps();
		}

		int executedSteps = 0;
		int stepsToExecute = StepSize;

		while (RemainingSteps > 0 && stepsToExecute > 0)
		{
			ExecuteStep();
			stepsToExecute--;
			RemainingSteps--;
			executedSteps++;
		}

		return executedSteps;
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
