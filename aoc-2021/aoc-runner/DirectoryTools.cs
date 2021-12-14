public static class DirectoryTools
{
	public static string FindSolutionFolder()
	{
		var thisPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

		while (true) {
			if (thisPath == null) {
				throw new InvalidOperationException("Current path cannot be null.");
			}

			if (Directory.GetFiles(thisPath, "*.csproj").Any()) {
				thisPath = Path.GetDirectoryName(thisPath);
				break;
			}

			thisPath = Path.GetDirectoryName(thisPath);
		}

		if (thisPath == null) {
			throw new InvalidOperationException("Failed to locate solution root");
		}

		return thisPath;
	}

}
