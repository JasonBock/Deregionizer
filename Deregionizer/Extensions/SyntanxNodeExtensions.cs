using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Deregionizer.Extensions
{
	public static class SyntaxNodeExtensions
	{
		public static SyntaxNode Deregionize(this SyntaxNode @this)
		{
			var nodesWithRegionDirectives =
				from node in @this.DescendantNodesAndTokens()
				where node.HasLeadingTrivia
				from leadingTrivia in node.GetLeadingTrivia()
				where (leadingTrivia.RawKind == (int)SyntaxKind.RegionDirectiveTrivia ||
					leadingTrivia.RawKind == (int)SyntaxKind.EndRegionDirectiveTrivia)
				select node;

			var triviaToRemove = new List<SyntaxTrivia>();

			foreach (var nodeWithRegionDirective in nodesWithRegionDirectives)
			{
				var triviaList = nodeWithRegionDirective.GetLeadingTrivia();

				for (var i = 0; i < triviaList.Count; i++)
				{
					var currentTrivia = triviaList[i];

					if (currentTrivia.RawKind == (int)SyntaxKind.RegionDirectiveTrivia ||
						currentTrivia.RawKind == (int)SyntaxKind.EndRegionDirectiveTrivia)
					{
						triviaToRemove.Add(currentTrivia);

						if (i > 0)
						{
							var previousTrivia = triviaList[i - 1];

							if (previousTrivia.RawKind == (int)SyntaxKind.WhitespaceTrivia)
							{
								triviaToRemove.Add(previousTrivia);
							}
						}
					}
				}
			}

			return triviaToRemove.Count > 0 ?
				@this.ReplaceTrivia(triviaToRemove,
					(_, __) => new SyntaxTrivia()) : @this;
		}
	}
}
