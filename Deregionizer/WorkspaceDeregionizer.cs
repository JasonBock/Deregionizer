using System.IO;
using System.Threading.Tasks;
using Deregionizer.Extensions;
using Microsoft.CodeAnalysis.MSBuild;

namespace Deregionizer
{
	public static class WorkspaceDeregionizer
	{
		public static async Task DeregionizeSolutionAsync(string solutionFile)
		{
			var workspace = MSBuildWorkspace.Create();
			var solution = await workspace.OpenSolutionAsync(solutionFile);
			var newSolution = solution;

			foreach (var projectId in solution.ProjectIds)
			{
				var project = newSolution.GetProject(projectId);

				foreach (var documentId in project.DocumentIds)
				{
					var document = newSolution.GetDocument(documentId);

					if (Path.GetExtension(document.FilePath).ToLower() == ".cs")
					{
						var root = await document.GetSyntaxRootAsync();
						var newRoot = root.Deregionize();

						if (root != newRoot)
						{
							newSolution = newSolution.WithDocumentSyntaxRoot(documentId, newRoot);
						}
					}
				}
			}

			workspace.TryApplyChanges(newSolution);
		}

		public static async Task DeregionizeProjectAsync(string projectFile)
		{
			var workspace = MSBuildWorkspace.Create();
			var originalProject = await workspace.OpenProjectAsync(projectFile);
			var solution = originalProject.Solution;
			var newSolution = solution;

			var project = newSolution.GetProject(originalProject.Id);

			foreach (var documentId in project.DocumentIds)
			{
				var document = newSolution.GetDocument(documentId);

				if (Path.GetExtension(document.FilePath).ToLower() == ".cs")
				{
					var root = await document.GetSyntaxRootAsync();
					var newRoot = root.Deregionize();

					if (root != newRoot)
					{
						newSolution = newSolution.WithDocumentSyntaxRoot(documentId, newRoot);
					}
				}
			}

			workspace.TryApplyChanges(newSolution);
		}
	}
}
