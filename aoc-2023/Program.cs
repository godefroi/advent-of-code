namespace AdventOfCode.Year2023;

public class Program
{
	public static async Task Main(string[] args)
	{
		Day23.Problem.Execute(ReadFileLines("Day23\\inputSample.txt"));
		await Task.CompletedTask;
	}
}
