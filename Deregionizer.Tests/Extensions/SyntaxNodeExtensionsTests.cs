using System.IO;
using Deregionizer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Deregionizer.Tests.Extensions
{
	[TestClass]
	public sealed class SyntaxNodeExtensionsTests
	{
		[TestMethod]
		public void DeregionizeWhenCodeDoesNotContainDirectives()
		{
			var code = "public class MyClass { }";
			var tree = SyntaxFactory.ParseSyntaxTree(code).GetRoot();
			var newTree = tree.Deregionize();

			Assert.AreSame(newTree, tree);
		}

		[TestMethod]
		public void Deregionize()
		{
			var code =
@"public class MyClass 
{ 
	#region Constructors
	public MyClass()
		: base() { }

	public string Data { get; set; }
}		
#endregion";

			var tree = SyntaxFactory.ParseSyntaxTree(code).GetRoot();
			var treeRegionCounts = new RegionWalker();
			treeRegionCounts.Visit(tree);

			Assert.AreEqual(1, treeRegionCounts.RegionCount);
			Assert.AreEqual(1, treeRegionCounts.EndRegionCount);

			var newTree = tree.Deregionize();
			Assert.AreNotSame(newTree, tree);

			var newTreeRegionCounts = new RegionWalker();
			newTreeRegionCounts.Visit(newTree);

			Assert.AreEqual(0, newTreeRegionCounts.RegionCount);
			Assert.AreEqual(0, newTreeRegionCounts.EndRegionCount);
		}

		[TestMethod]
		public void DeregionizeRange()
		{
			var code = File.ReadAllText("Range.cs");

			var tree = SyntaxFactory.ParseSyntaxTree(code).GetRoot();

			var treeRegionCounts = new RegionWalker();
			treeRegionCounts.Visit(tree);

			Assert.AreEqual(4, treeRegionCounts.RegionCount);
			Assert.AreEqual(4, treeRegionCounts.EndRegionCount);

			var newTree = tree.Deregionize();
			Assert.AreNotSame(newTree, tree);

			var newTreeRegionCounts = new RegionWalker();
			newTreeRegionCounts.Visit(newTree);

			File.WriteAllText("RangeNoRegions.cs", newTree.ToString());

			Assert.AreEqual(0, newTreeRegionCounts.RegionCount);
			Assert.AreEqual(0, newTreeRegionCounts.EndRegionCount);
		}

		private sealed class RegionWalker
			: CSharpSyntaxWalker
		{
			public RegionWalker()
				: base(depth: SyntaxWalkerDepth.StructuredTrivia)
			{ }

			public override void VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
			{
				base.VisitEndRegionDirectiveTrivia(node);
				this.EndRegionCount++;
			}

			public override void VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
			{
				base.VisitRegionDirectiveTrivia(node);
				this.RegionCount++;
			}

			public int EndRegionCount { get; private set; }
			public int RegionCount { get; private set; }
		}
	}
}
