namespace aoc_2019.Intcode.Infrastructure;

internal class OutputStream<T>
{
	private readonly Queue<T> _output = new();

	public bool OutputAvailable => _output.Count > 0;

	public int Count => _output.Count;

	public void Write(T output) => _output.Enqueue(output);

	public T Read() => _output.Dequeue();
}
