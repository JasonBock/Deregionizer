using System;
using System.IO;

namespace Deregionizer.ConsoleApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0 || args[0] == string.Empty)
			{
				Console.Out.WriteLine("Usage: Deregionizer.ConsoleApplication {solution or project file}");
			}

			//System.Diagnostics.Debugger.Launch();

			if (Path.GetExtension(args[0]) == ".sln")
			{
				WorkspaceDeregionizer.DeregionizeSolutionAsync(args[0]).Wait();
			}
			else if (Path.GetExtension(args[0]) == ".csproj")
			{
				WorkspaceDeregionizer.DeregionizeProjectAsync(args[0]).Wait();
			}
		}
	}
}
