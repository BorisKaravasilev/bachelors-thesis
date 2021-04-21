namespace MyRandom
{
	/// <summary>
	/// Wrapper around an object adding a probability field.
	/// </summary>
	public class ItemWithProbability<TItemType>
	{
		public TItemType Item { get; set; }
		public float Probability { get; set; }
	}
}
