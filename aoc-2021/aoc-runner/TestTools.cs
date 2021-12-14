using System.Diagnostics;

using Xunit;
using Xunit.Abstractions;

internal static class TestTools
{
	public static void ExecuteProject(ITestOutputHelper output, string projectFolder, string inputFileName = "input.txt")
	{
		string? p1    = null;
		string? p2    = null;
		string? p1Val = null;
		string? p2Val = null;

		var inputfile = Path.Combine(projectFolder, inputFileName);
		var valFile   = Path.Combine(projectFolder, Path.GetFileNameWithoutExtension(inputFileName) + ".val");

		Assert.True(Directory.Exists(projectFolder));
		Assert.True(File.Exists(inputfile));

		if (File.Exists(valFile)) {
			var lines = File.ReadAllLines(valFile);

			var v1 = lines.SingleOrDefault(l => l.StartsWith("part 1: "));
			var v2 = lines.SingleOrDefault(l => l.StartsWith("part 2: "));

			if (v1 != null) {
				p1Val = v1[8..];
			}

			if (v2 != null) {
				p2Val = v2[8..];
			}
		}

		var process = new Process() {
			EnableRaisingEvents = true,
			StartInfo           = new ProcessStartInfo(Path.GetFileName("dotnet")) {
				WorkingDirectory       = projectFolder,
				RedirectStandardOutput = true,
				Arguments              = $"run --verbosity quiet -- {inputfile}",
			},
		};

		process.OutputDataReceived += (s, e) => {
			if (e.Data != null) {
				if (e!.Data.StartsWith("part 1: ") && !string.IsNullOrWhiteSpace(p1Val)) {
					p1 = e?.Data?[8..];
				}

				if (e!.Data.StartsWith("part 2: ") && !string.IsNullOrWhiteSpace(p2Val)) {
					p2 = e?.Data?[8..];
				}
			}

			output.WriteLine($"{e?.Data}");
		};

		process.Start();
		process.BeginOutputReadLine();
		process.WaitForExit();

		if (p1 != null) {
			Assert.Equal(p1Val, p1);
			output.WriteLine($"Validated output {p1} against expected output {p1Val}");
		}

		if (p2 != null) {
			Assert.Equal(p2Val, p2);
			output.WriteLine($"Validated output {p2} against expected output {p2Val}");
		}

		Assert.Equal(0, process.ExitCode);
	}
}
