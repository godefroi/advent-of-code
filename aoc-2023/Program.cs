namespace AdventOfCode.Year2023;

public class Program
{
	public static async Task Main(string[] args)
	{
		Day17.Problem.Execute(ReadFileLines("inputSample.txt"));
		await Task.CompletedTask;
	}
}
