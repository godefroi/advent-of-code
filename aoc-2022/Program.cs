namespace aoc_2022;

public static partial class Program
{
	public static async Task Main(string[] args)
	{
		await ProblemRunner.Execute(args, Problems());
	}

	private static partial ProblemMetadata[] Problems();
}
