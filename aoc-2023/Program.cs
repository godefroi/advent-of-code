namespace AdventOfCode.Year2023;

public class Program
{
	public static async Task Main(string[] args)
	{
		Day19.Problem.Execute(ReadFileLines("Day19\\inputSample.txt"));
		await Task.CompletedTask;
	}
}
