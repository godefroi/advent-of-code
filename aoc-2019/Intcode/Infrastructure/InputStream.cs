namespace aoc_2019.Intcode.Infrastructure;

internal class InputStream<T>
{
	private readonly Queue<T> _input;

	public InputStream(IEnumerable<T> input) => _input = new Queue<T>(input);

	public T Next() => _input.Count > 0 ? _input.Dequeue() : throw new InputNeededException();

	public void Add(T input) => _input.Enqueue(input);
}
