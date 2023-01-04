public abstract class ProblemBase
{
	protected static string GetFilePath(string fileName, [System.Runtime.CompilerServices.CallerFilePath] string sourceFilepPath = "") => Path.Combine(Path.GetDirectoryName(sourceFilepPath)!, fileName);
}
